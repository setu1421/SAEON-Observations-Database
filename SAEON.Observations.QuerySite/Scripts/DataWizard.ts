namespace DataWizard {
    export function ShowWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }
    export function HideWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.hide();
    }
    export function ShowResults() {
        $("#TableTab").removeClass("hidden");
        $("#ChartTab").removeClass("hidden");
        //$("#CardsTab").removeClass("hidden");
    }
    export function HideResults() {
        $("#TableTab").addClass("hidden");
        $("#ChartTab").addClass("hidden");
        //$("#CardsTab").addClass("hidden");
    }

    function ErrorInFunc(msg) {
        HideWaiting();
        alert(msg);
    }

    export function EnableButtons() {
        var btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        var btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        var btnSearch = $("#btnSearch").data("ejButton");
        var btnDownload = $("#btnDownload").data("ejButton");
        var selectedLocations = 0;
        //var treeObj = $("#treeViewLocations").data('ejTreeView');
        //var checkedNodes = treeObj.getCheckedNodes();
        //var i;
        //for (i = 0; i < checkedNodes.length; i++) {
        //    var checkedNode = checkedNodes[i];
        //    var nodeData = treeObj.getNode(checkedNode);
        //    if (nodeData.id.startsWith("STA~")) {
        //        selectedLocations = selectedLocations + 1;
        //    }
        //}
        var selectedFeatures = 0;
        //treeObj = $("#treeViewFeatures").data('ejTreeView');
        //var checkedNodes = treeObj.getCheckedNodes();
        //for (i = 0; i < checkedNodes.length; i++) {
        //    var checkedNode = checkedNodes[i];
        //    var nodeData = treeObj.getNode(checkedNode);
        //    if (nodeData.id.startsWith("OFF~")) {
        //        selectedFeatures = selectedFeatures + 1;
        //    }
        //}
        btnLoadQuery.enable();
        btnLoadQuery.disable(); // remove later
        if ((selectedLocations > 0) && (selectedFeatures > 0)) {
            btnSaveQuery.enable();
            btnSearch.enable();
        }
        else {
            btnSaveQuery.disable();
            btnSearch.disable();
        }
        btnDownload.disable();
    }

    export function DisableButtons() {
        var btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        var btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        var btnSearch = $("#btnSearch").data("ejButton");
        var btnDownload = $("#btnDownload").data("ejButton");
        btnLoadQuery.disable();
        btnSaveQuery.disable();
        btnSearch.disable();
        btnDownload.disable();
    }

    export function TabActive() {
        var tab = $("#DataWizardTabs").data("ejTab");
        if (tab.selectedItemIndex() == 3) {
            UpdateMap();
        }
    }


    export function LocationsReady() {
        HideWaiting();
    }

    export function UpdateLocationsSelected() {
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = []
        var i: number;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
        }
        $.post("/DataWizard/UpdateLocationsSelected", { locations: selected })
            .done(function (data) {
                $('#PartialLocationsSelected').html(data);
                UpdateMap();
                EnableButtons();
                HideResults();
            })
            .fail(function () { ErrorInFunc("Error in UpdateLocationsSelected"); });
    }

    class MapPoint {
        Title: string;
        Latitude: number;
        Longitude: number;
        Elevation: number;
        Url: string;
        IsSelected: boolean;
    }

    let map: google.maps.Map;
    let markers: google.maps.Marker[] = [];
    let mapPoints: MapPoint[];
    let mapInitialized: boolean = false;

    export function InitMap() {
        let mapOpts: google.maps.MapOptions = {
            center: new google.maps.LatLng(-34, 25.5),
            zoom: 5
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
        UpdateMap();
    }

    export function UpdateMap() {
        $.getJSON("/DataWizard/GetMapPoints")
            .done(function (json) {
                for (let i = 0; i < markers.length; i++) {
                    markers[i].setMap(null);
                }
                markers = [];
                mapPoints = json;
                var bounds = new google.maps.LatLngBounds();
                for (let i = 0; i < mapPoints.length; i++) {
                    let mapPoint = mapPoints[i];
                    let marker = new google.maps.Marker({
                        position: { lat: mapPoint.Latitude, lng: mapPoint.Longitude },
                        map: map,
                        title: mapPoint.Title
                    });
                    markers.push(marker);
                    bounds.extend(marker.getPosition());
                    if (mapPoint.IsSelected) {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
                    }
                    else {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');
                    }
                }
                if (markers.length > 0) {
                    map.setCenter(bounds.getCenter());
                    map.fitBounds(bounds);
                    mapInitialized = true;
                }
            })
            .fail(function () { ErrorInFunc('Error in GetMapPoints'); });
    }


}
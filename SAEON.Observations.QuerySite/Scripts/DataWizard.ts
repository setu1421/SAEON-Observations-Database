namespace DataWizard {
    export function ShowWaiting() {
        let wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }
    export function HideWaiting() {
        let wp = $("#waiting").data("ejWaitingPopup");
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
        let btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        let btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        let btnSearch = $("#btnSearch").data("ejButton");
        let btnDownload = $("#btnDownload").data("ejButton");
        let selectedLocations = 0;
        //let treeObj = $("#treeViewLocations").data('ejTreeView');
        //let checkedNodes = treeObj.getCheckedNodes();
        //let i;
        //for (i = 0; i < checkedNodes.length; i++) {
        //    let checkedNode = checkedNodes[i];
        //    let nodeData = treeObj.getNode(checkedNode);
        //    if (nodeData.id.startsWith("STA~")) {
        //        selectedLocations = selectedLocations + 1;
        //    }
        //}
        let selectedFeatures = 0;
        //treeObj = $("#treeViewFeatures").data('ejTreeView');
        //let checkedNodes = treeObj.getCheckedNodes();
        //for (i = 0; i < checkedNodes.length; i++) {
        //    let checkedNode = checkedNodes[i];
        //    let nodeData = treeObj.getNode(checkedNode);
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
        let btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        let btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        let btnSearch = $("#btnSearch").data("ejButton");
        let btnDownload = $("#btnDownload").data("ejButton");
        btnLoadQuery.disable();
        btnSaveQuery.disable();
        btnSearch.disable();
        btnDownload.disable();
    }

    export function TabActive() {
        let tab = $("#DataWizardTabs").data("ejTab");
        if (tab.selectedItemIndex() == 3) {
            FitMap();
        }
    }

    let locationsReady: boolean = false;

    export function LocationsReady() {
        locationsReady = true;
        if (locationsReady && featuresReady) {
            HideWaiting();
        }
    }

    export function UpdateLocationsSelected() {
        let treeObj = $("#treeViewLocations").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let selected = []
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let nodeData = treeObj.getNode(nodes[i]);
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

    let featuresReady: boolean = false;

    export function FeaturesReady() {
        featuresReady = true;
        //if (locationsReady && featuresReady) {
            HideWaiting();
        //}
    }

    export function UpdateFeaturesSelected() {
        let treeObj = $("#treeViewFeatures").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let selected = []
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
        }
        $.post("/DataWizard/UpdateFeaturesSelected", { features: selected })
            .done(function (data) {
                $('#PartialFeaturesSelected').html(data);
                EnableButtons();
                HideResults();
            })
            .fail(function () { ErrorInFunc("Error in UpdateFeaturesSelected"); });
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
    let mapBounds: google.maps.LatLngBounds;
    let mapFitted: boolean = false;

    export function InitMap() {
        let mapOpts: google.maps.MapOptions = {
            center: new google.maps.LatLng(-34, 25.5),
            zoom: 5
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
        UpdateMap();
        FitMap();
    }

    export function UpdateMap() {
        $.getJSON("/DataWizard/GetMapPoints")
            .done(function (json) {
                for (let i = 0; i < markers.length; i++) {
                    markers[i].setMap(null);
                }
                markers = [];
                mapPoints = json;
                mapBounds = new google.maps.LatLngBounds();
                for (let i = 0; i < mapPoints.length; i++) {
                    let mapPoint = mapPoints[i];
                    let marker = new google.maps.Marker({
                        position: { lat: mapPoint.Latitude, lng: mapPoint.Longitude },
                        map: map,
                        title: mapPoint.Title
                    });
                    markers.push(marker);
                    mapBounds.extend(marker.getPosition());
                    if (mapPoint.IsSelected) {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
                    }
                    else {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');
                    }
                }
            })
            .fail(function () { ErrorInFunc('Error in GetMapPoints'); });
    }

    export function FitMap(override: boolean = false) {
        if (override || (!mapFitted && (mapBounds != null) && !mapBounds.isEmpty())) {
            map.setCenter(mapBounds.getCenter());
            map.fitBounds(mapBounds);
            mapFitted = true;
        }
    }

    export function FixMap() {
        UpdateMap();
        FitMap(true);
    }
}
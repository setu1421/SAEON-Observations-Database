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

    export function LocationsReady() {
        HideWaiting();
    }

    export function UpdateSelectedLocations() {
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
            .fail(function () { ErrorInFunc("Error in UpdateSelectedLocations"); });
    }

    export class Location {
        Latitude: number;
        Longitude: number;
        constructor(latitude: number, longitude: number) {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
    let map: google.maps.Map;
    let locations: Array<Location> = new Array<Location>();

    export function InitMap() {
        let mapOpts: google.maps.MapOptions = {
            center: new google.maps.LatLng(-34, 25.5),
            zoom: 5
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
        let bounds = new google.maps.LatLngBounds();
        if (locations.length > 0) {
            for (var i = 0; i < locations.length; i++) {
                var markerOpts: google.maps.MarkerOptions = {
                    position: new google.maps.LatLng(locations[i].Latitude, locations[i].Longitude),
                    icon: {
                        path: google.maps.SymbolPath.CIRCLE,
                        scale: 5,
                        fillColor: 'green',
                        fillOpacity: 0.8,
                        strokeColor: 'gold',
                        strokeWeight: 2
                    },
                    map: map
                };
                var marker = new google.maps.Marker(markerOpts);
                bounds.extend(marker.getPosition());
            }
            map.fitBounds(bounds);
        }
    }

    export function SetLocations(Locations: Location[]) {
        for (var i = 0; i < Locations.length; i++) {
            locations.push(Locations[i]);
        }
    }

    export function UpdateMap() {

    }


}
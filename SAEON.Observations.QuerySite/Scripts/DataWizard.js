var DataWizard;
(function (DataWizard) {
    function ShowWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }
    DataWizard.ShowWaiting = ShowWaiting;
    function HideWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.hide();
    }
    DataWizard.HideWaiting = HideWaiting;
    function ShowResults() {
        $("#TableTab").removeClass("hidden");
        $("#ChartTab").removeClass("hidden");
        //$("#CardsTab").removeClass("hidden");
    }
    DataWizard.ShowResults = ShowResults;
    function HideResults() {
        $("#TableTab").addClass("hidden");
        $("#ChartTab").addClass("hidden");
        //$("#CardsTab").addClass("hidden");
    }
    DataWizard.HideResults = HideResults;
    function ErrorInFunc(msg) {
        HideWaiting();
        alert(msg);
    }
    function EnableButtons() {
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
    DataWizard.EnableButtons = EnableButtons;
    function DisableButtons() {
        var btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        var btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        var btnSearch = $("#btnSearch").data("ejButton");
        var btnDownload = $("#btnDownload").data("ejButton");
        btnLoadQuery.disable();
        btnSaveQuery.disable();
        btnSearch.disable();
        btnDownload.disable();
    }
    DataWizard.DisableButtons = DisableButtons;
    function LocationsReady() {
        HideWaiting();
    }
    DataWizard.LocationsReady = LocationsReady;
    function UpdateSelectedLocations() {
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
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
    DataWizard.UpdateSelectedLocations = UpdateSelectedLocations;
    var Location = /** @class */ (function () {
        function Location(latitude, longitude) {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
        return Location;
    }());
    DataWizard.Location = Location;
    var map;
    var locations = new Array();
    function InitMap() {
        var mapOpts = {
            center: new google.maps.LatLng(-34, 25.5),
            zoom: 5
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
        var bounds = new google.maps.LatLngBounds();
        if (locations.length > 0) {
            for (var i = 0; i < locations.length; i++) {
                var markerOpts = {
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
    DataWizard.InitMap = InitMap;
    function SetLocations(Locations) {
        for (var i = 0; i < Locations.length; i++) {
            locations.push(Locations[i]);
        }
    }
    DataWizard.SetLocations = SetLocations;
    function UpdateMap() {
    }
    DataWizard.UpdateMap = UpdateMap;
})(DataWizard || (DataWizard = {}));
//# sourceMappingURL=DataWizard.js.map
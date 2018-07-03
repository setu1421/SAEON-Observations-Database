namespace SpacialCoverage {
    export function ShowWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }
    export function HideWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.hide();
    }
    var mapVisible = false;
    export function onTabActive() {
        if (!mapVisible) {
            var tab = $("#SpacialCoverageTabs").data("ejTab");
            if (tab.selectedItemIndex() === 3) {
                UpdateMap();
                mapVisible = true;
            }
        }
    }
    function ErrorInFunc(msg) {
        HideWaiting();
        alert(msg);
    }
    export function EnableButtons() {
        var btnSearch = $("#btnSearch").data("ejButton");
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var checkedNodes = treeObj.getCheckedNodes();
        var selectedLocations = 0;
        var i;
        for (i = 0; i < checkedNodes.length; i++) {
            var checkedNode = checkedNodes[i];
            var nodeData = treeObj.getNode(checkedNode);
            if (nodeData.id.startsWith("STA~")) {
                selectedLocations = selectedLocations + 1;
            }
        }
        treeObj = $("#treeViewFeatures").data('ejTreeView');
        var checkedNodes = treeObj.getCheckedNodes();
        var selectedFeatures = 0;
        for (i = 0; i < checkedNodes.length; i++) {
            var checkedNode = checkedNodes[i];
            var nodeData = treeObj.getNode(checkedNode);
            if (nodeData.id.startsWith("OFF~")) {
                selectedFeatures = selectedFeatures + 1;
            }
        }
        if ((selectedLocations > 0) && (selectedFeatures > 0)) {
            btnSearch.enable();
        }
        else {
            btnSearch.disable();
        }
    }
    export function DisableButtons() {
        var btnSearch = $("#btnSearch").data("ejButton");
        btnSearch.disable();
    }
    export function onUpdateSelectedLocations() {
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
        }
        $.post("/SpacialCoverage/UpdateSelectedLocations", { Locations: selected })
            .done(function (data) {
                $('#PartialSelectedLocations').html(data);
                EnableButtons();
            })
            .fail(function () { alert("Error in UpdateSelectedLocations"); });
    }
    export function onUpdateSelectedFeatures() {
        var treeObj = $("#treeViewFeatures").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
        }
        $.post("/SpacialCoverage/UpdateSelectedFeatures", { Features: selected })
            .done(function (data) {
                $('#PartialSelectedFeatures').html(data);
                EnableButtons();
            })
            .fail(function () { alert("Error in UpdateSelectedFeatures"); });
    }
    export function onUpdateFilters() {
        var startDate = $("#StartDate").ejDatePicker("instance").getValue();
        var endDate = $("#EndDate").ejDatePicker("instance").getValue();
        $.ajax({
            url: "/SpacialCoverage/UpdateFilters",
            data: JSON.stringify({
                StartDate: startDate,
                EndDate: endDate
            }),
            async: false,
            type: "POST",
            contentType: "application/json",
            error: function (jqXHR, textStatus, errorThrown) {
                ErrorInFunc("Error in UpdateFilters Status: " + textStatus + " Error: " + errorThrown);
            }
        });
    }
    export function onSearchClick() {
        var tab = $("#SpacialCoverageTabs").data("ejTab");
        tab.showItem(3);
        ShowWaiting();
        $.get("/SpacialCoverage/GetData")
            .done(function () {
                UpdateMap();
                EnableButtons();
                HideWaiting();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                ErrorInFunc("Error in GetData Status: " + textStatus + " Error: " + errorThrown);
            });
    }
    var markers = [];
    var map;
    export function initMap() {
        var opts: google.maps.MapOptions = {
            center: new google.maps.LatLng(-28.7238579, 124.6531662),
            zoom: 5
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), opts);
        UpdateMap(true);
    }
    export function UpdateMap(isInit: boolean = false) {
        $.getJSON("/SpacialCoverage/GetMapPoints")
            .done(function (json) {
                for (var i = 0; i < markers.length; i++) {
                    markers[i].setMap(null);
                }
                markers = [];
                var bounds = new google.maps.LatLngBounds();
                var locations = json;
                for (var i = 0; i < locations.length; i++) {
                    var location = locations[i];
                    var opts: google.maps.MarkerOptions = {
                        position: new google.maps.LatLng(location.Latitude, location.Longitude),
                        map: map,
                        title: location.Title
                    };
                    var marker = new google.maps.Marker(opts);
                    markers.push(marker);
                    bounds.extend(marker.getPosition());
                    if (location.Status === 0) {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/blue-dot.png');
                        marker.setTitle(marker.getTitle() + " - No Status");
                    }
                    else if (location.Status === 1) {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');
                        marker.setTitle(marker.getTitle() + " - Unverfied");
                    }
                    else if (location.Status === 2) {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/orange-dot.png');
                        marker.setTitle(marker.getTitle() + " - Being Verified");
                    }
                    else {
                        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
                        marker.setTitle(marker.getTitle() + " - Verfied");
                    }
                }
                if (isInit) {
                    if (markers.length > 0) {
                        map.fitBounds(bounds);
                    }
                    var center = map.getCenter();
                }
                else {

                }
                google.maps.event.trigger(map, "resize");
                map.setCenter(center);
            })
            .fail(function () { alert('Error in GetMapPoints'); });
    }
}
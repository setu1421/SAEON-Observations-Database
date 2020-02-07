var DataWizard;
(function (DataWizard) {
    // Waiting
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
    // Results
    function ShowResults() {
        $("#TableTab").removeClass("d-none");
        $("#PartialTable").removeClass("d-none");
        $("#ChartTab").removeClass("d-none");
        $("#PartialChart").removeClass("d-none");
        //$("#CardsTab").removeClass("hidden");
    }
    DataWizard.ShowResults = ShowResults;
    function HideResults() {
        $("#TableTab").addClass("d-none");
        $("#PartialTable").addClass("d-none");
        $("#ChartTab").addClass("d-none");
        $("#PartialChart").addClass("d-none");
        searched = false;
        //$("#CardsTab").addClass("hidden");
    }
    DataWizard.HideResults = HideResults;
    // Errors
    function ErrorInFunc(method, status, error) {
        HideWaiting();
        alert("Error in " + method + " Status: " + status + " Error: " + error);
    }
    // Tabs
    function SelectTab(index) {
        var tab = $("#DataWizardTabs").data("ejTab");
        tab.option("selectedItemIndex", index);
    }
    function SelectedTab() {
        var tab = $("#DataWizardTabs").data("ejTab");
        return tab.model.selectedItemIndex;
    }
    // Features fix on 1st load
    var locationsSelected = false;
    var locationsExpand = true;
    var featuresSelected = false;
    var featuresExpand = true;
    function TabActive() {
        var selectedTab = SelectedTab();
        if (selectedTab == 0) {
            if (!locationsSelected) {
                UpdateLocationsSelected();
                locationsSelected = true;
            }
        }
        else if (selectedTab == 1) {
            if (!featuresSelected) {
                UpdateFeaturesSelected();
                featuresSelected = true;
            }
        }
        else if (selectedTab == 3) {
            FitMap();
        }
    }
    DataWizard.TabActive = TabActive;
    function TabCreate() {
        //$("a[href='#LocationsTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Organisations, Sites, Stations"></i>');
        //$("a[href='#FeaturesTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Phenomena, Offerings, Units"></i>');
        //$("a[href='#FiltersTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Dates, Depth"></i>');
    }
    DataWizard.TabCreate = TabCreate;
    // State
    var State = /** @class */ (function () {
        function State() {
        }
        return State;
    }());
    // Buttons
    function EnableButtons() {
        var btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        var btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        var btnSearch = $("#btnSearch").data("ejButton");
        var btnDownload = $("#btnDownload").data("ejButton");
        $.get("/DataWizard/GetState")
            .done(function (state) {
            if (state.LoadEnabled) {
                btnLoadQuery.enable();
            }
            else {
                btnLoadQuery.disable();
            }
            if (state.SaveEnabled) {
                btnSaveQuery.enable();
            }
            else {
                btnSaveQuery.disable();
            }
            if (state.SearchEnabled) {
                btnSearch.enable();
            }
            else {
                btnSearch.disable();
            }
            if (state.DownloadEnabled) {
                btnDownload.enable();
            }
            else {
                btnDownload.disable();
            }
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("GetState", status, error);
        });
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
    // Locations
    var locationsReady = false;
    var locationsLoaded = false;
    function LocationsReady() {
        locationsReady = true;
        CheckReady();
    }
    DataWizard.LocationsReady = LocationsReady;
    function LocationsLoadComplete() {
        if (!locationsLoaded) {
            locationsLoaded = true;
            //UpdateLocationsSelected();
            //UpdateFeaturesSelected();
            HideWaiting();
            EnableButtons();
        }
    }
    DataWizard.LocationsLoadComplete = LocationsLoadComplete;
    function LocationsChanged() {
        UpdateLocationsSelected(true);
    }
    DataWizard.LocationsChanged = LocationsChanged;
    function UpdateLocationsSelected(isClick) {
        if (isClick === void 0) { isClick = false; }
        if (loading)
            return;
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
            if (locationsExpand) {
                var parent_1 = treeObj.getParent(nodes[i]);
                while (parent_1 != null) {
                    treeObj.expandNode(parent_1);
                    parent_1 = treeObj.getParent(parent_1);
                }
            }
        }
        $.post("/DataWizard/UpdateLocationsSelected", { locations: selected })
            .done(function (data) {
            $('#PartialLocationsSelected').html(data);
            SetApproximation();
            UpdateMap();
            if (isClick) {
                HideResults();
            }
            EnableButtons();
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("UpdateLocationsSelected", status, error);
        });
    }
    function ExpandLocationsSelected() {
        var treeObj = $("#treeViewLocations").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var i;
        for (i = 0; i < nodes.length; i++) {
            var parent_2 = treeObj.getParent(nodes[i]);
            while (parent_2 != null) {
                treeObj.expandNode(parent_2);
                parent_2 = treeObj.getParent(parent_2);
            }
        }
    }
    // Features
    var featuresReady = false;
    function FeaturesReady() {
        featuresReady = true;
        CheckReady();
    }
    DataWizard.FeaturesReady = FeaturesReady;
    function CheckReady() {
        if (locationsReady && featuresReady) {
            UpdateLocationsSelected();
            UpdateFeaturesSelected();
            //HideWaiting();
            //EnableButtons();
        }
    }
    function FeaturesChanged() {
        UpdateFeaturesSelected(true);
    }
    DataWizard.FeaturesChanged = FeaturesChanged;
    function UpdateFeaturesSelected(isClick) {
        if (isClick === void 0) { isClick = false; }
        if (loading)
            return;
        var treeObj = $("#treeViewFeatures").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
            if (featuresExpand) {
                var parent_3 = treeObj.getParent(nodes[i]);
                while (parent_3 != null) {
                    treeObj.expandNode(parent_3);
                    parent_3 = treeObj.getParent(parent_3);
                }
            }
        }
        $.post("/DataWizard/UpdateFeaturesSelected", { features: selected })
            .done(function (data) {
            $('#PartialFeaturesSelected').html(data);
            SetApproximation();
            if (isClick) {
                HideResults();
            }
            EnableButtons();
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("UpdateFeaturesSelected", status, error);
        });
    }
    function ExpandFeaturesSelected() {
        var treeObj = $("#treeViewFeatures").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var i;
        for (i = 0; i < nodes.length; i++) {
            var parent_4 = treeObj.getParent(nodes[i]);
            while (parent_4 != null) {
                treeObj.expandNode(parent_4);
                parent_4 = treeObj.getParent(parent_4);
            }
        }
    }
    // Filters
    function FiltersChanged() {
        UpdateFilters(true);
    }
    DataWizard.FiltersChanged = FiltersChanged;
    function UpdateFilters(isClick) {
        if (isClick === void 0) { isClick = false; }
        var startDate = $("#StartDate").ejDatePicker("instance").getValue();
        var endDate = $("#EndDate").ejDatePicker("instance").getValue();
        var range = $("#ElevationSlider").ejSlider("instance").getValue();
        var elevationMinimum = range[0];
        var elevationMaximum = range[1];
        $.post("/DataWizard/UpdateFilters", { startDate: startDate, endDate: endDate, elevationMinimum: elevationMinimum, elevationMaximum: elevationMaximum })
            .done(function (data) {
            SetApproximation();
            if (isClick) {
                HideResults();
            }
            EnableButtons();
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("UpdateFilters", status, error);
        });
    }
    // Map
    var MapPoint = /** @class */ (function () {
        function MapPoint() {
        }
        return MapPoint;
    }());
    var map;
    var markers = [];
    var mapPoints;
    var mapBounds;
    var mapFitted = false;
    function InitMap() {
        var mapOpts = {
            center: new google.maps.LatLng(-34, 25.5),
            zoom: 5,
            mapTypeId: google.maps.MapTypeId.SATELLITE
        };
        map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
        UpdateMap();
        FitMap();
    }
    DataWizard.InitMap = InitMap;
    function UpdateMap() {
        $.getJSON("/DataWizard/GetMapPoints")
            .done(function (json) {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
            mapPoints = json;
            mapBounds = new google.maps.LatLngBounds();
            for (var i = 0; i < mapPoints.length; i++) {
                var mapPoint = mapPoints[i];
                var marker = new google.maps.Marker({
                    position: { lat: mapPoint.Latitude, lng: mapPoint.Longitude },
                    map: map,
                    title: mapPoint.Title
                });
                markers.push(marker);
                mapBounds.extend(marker.getPosition());
                if (mapPoint.IsSelected) {
                    marker.setIcon('https://maps.google.com/mapfiles/ms/icons/green-dot.png');
                }
                else {
                    marker.setIcon('https://maps.google.com/mapfiles/ms/icons/red-dot.png');
                }
            }
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("UpdateMap", status, error);
        });
    }
    DataWizard.UpdateMap = UpdateMap;
    function FitMap(override) {
        if (override === void 0) { override = false; }
        if (override || (!mapFitted && (mapBounds != null) && !mapBounds.isEmpty())) {
            map.setCenter(mapBounds.getCenter());
            map.fitBounds(mapBounds);
            mapFitted = true;
        }
    }
    DataWizard.FitMap = FitMap;
    function FixMap() {
        UpdateMap();
        FitMap(true);
    }
    DataWizard.FixMap = FixMap;
    // Aproximation
    //export function GetApproximation(): string {
    //    let approximation: string = "{}";
    //    $.get("/DataWizard/GetApproximation")
    //        .done(function (data) {
    //            approximation = data;
    //        })
    //        .fail(function (jqXHR, status, error) {
    //            ErrorInFunc("GetApproximation", status, error)
    //        });
    //    return approximation;
    //}
    function SetApproximation() {
        $.get("/DataWizard/SetApproximation")
            .done(function (data) {
            $('#PartialApproximation').html(data);
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("SetApproximation", status, error);
        });
    }
    DataWizard.SetApproximation = SetApproximation;
    // Search
    var searched = false;
    function Search() {
        ShowWaiting();
        $.get("/DataWizard/GetData")
            .done(function () {
            $("#PartialTable").load("/DataWizard/GetTableHtml", function () {
                $("#PartialChart").load("/DataWizard/GetChartHtml", function () {
                    //$("#PartialCards").load("/DataWizard/GetCardsHtml", function () {
                    ShowResults();
                    searched = true;
                    EnableButtons();
                    HideWaiting();
                    SelectTab(5);
                });
                //    });
            });
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("GetData", status, error);
        });
    }
    DataWizard.Search = Search;
    // User Queries
    // LoadQuery
    function LoadQueryOpen() {
        DisableButtons();
        $("#dialogLoadQuery").ejDialog("open");
        $("#editLoadQueryName").ejAutocomplete("open");
    }
    DataWizard.LoadQueryOpen = LoadQueryOpen;
    function LoadQueryClose() {
        $("#editLoadQueryName").ejAutocomplete("hide");
        $("#dialogLoadQuery").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }
    DataWizard.LoadQueryClose = LoadQueryClose;
    function LoadQueryNameChange() {
        var loadName = $("#editLoadQueryName").val();
        var btnLoad = $("#btnLoadQueryLoad").data("ejButton");
        if (loadName == "") {
            btnLoad.disable();
        }
        else {
            btnLoad.enable();
        }
    }
    DataWizard.LoadQueryNameChange = LoadQueryNameChange;
    var loading = false;
    function LoadQuery() {
        ShowWaiting();
        $.ajax({
            url: "/DataWizard/LoadQuery",
            data: JSON.stringify({ Name: $("#editLoadQueryName").val() }),
            async: true,
            type: "POST",
            contentType: "application/json",
            success: function () {
                var selectedTab = SelectedTab();
                $("#PartialLocations").load("/DataWizard/GetLocationsHtml", function () {
                    $("#PartialLocationsSelected").load("/DataWizard/GetLocationsSelectedHtml", function () {
                        $("#PartialFeatures").load("/DataWizard/GetFeaturesHtml", function () {
                            $("#PartialFeaturesSelected").load("/DataWizard/GetFeaturesSelectedHtml", function () {
                                $("#PartialFilters").load("/DataWizard/GetFiltersHtml", function () {
                                    //UpdateMap();
                                    $("PartialTable").text("");
                                    $("PartialCards").text("");
                                    $("PartialChart").text("");
                                    HideResults();
                                    LoadQueryClose();
                                    locationsSelected = false;
                                    featuresSelected = false;
                                    loading = false;
                                    ExpandLocationsSelected();
                                    ExpandFeaturesSelected();
                                });
                            });
                        });
                    });
                });
            },
            error: function (jqXHR, status, error) {
                ErrorInFunc("LoadQueryLoad", status, error);
            }
        });
    }
    DataWizard.LoadQuery = LoadQuery;
    // Save Query
    function SaveQueryOpen() {
        DisableButtons();
        $("#dialogSaveQuery").ejDialog("open");
    }
    DataWizard.SaveQueryOpen = SaveQueryOpen;
    function SaveQueryClose() {
        $("#dialogSaveQuery").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }
    DataWizard.SaveQueryClose = SaveQueryClose;
    function SaveQueryNameChange() {
        var saveName = $("#editSaveQueryName").val();
        var btnSave = $("#btnSaveQuerySave").data("ejButton");
        if (saveName == "") {
            btnSave.disable();
        }
        else {
            btnSave.enable();
        }
    }
    DataWizard.SaveQueryNameChange = SaveQueryNameChange;
    function SaveQuery() {
        ShowWaiting();
        $.ajax({
            url: "/DataWizard/SaveQuery",
            data: JSON.stringify({
                Name: $("#editSaveQueryName").val(),
                Description: $("#editSaveQueryDescription").val()
            }),
            async: true,
            type: "POST",
            contentType: "application/json"
        })
            .done(function () {
            SaveQueryClose();
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("SaveQuery", status, error);
        });
    }
    DataWizard.SaveQuery = SaveQuery;
    function Test() {
        //locationsSelected = false;
        //featuresSelected = false;
        //if (SelectedTab() == 0) {
        //    SelectTab(1)
        //} else {
        //    SelectTab(0);
        //}
        ExpandLocationsSelected();
        ExpandFeaturesSelected();
    }
    DataWizard.Test = Test;
    // Download
    function DownloadOpen() {
        DisableButtons();
        $("#dialogDownload").ejDialog("open");
    }
    DataWizard.DownloadOpen = DownloadOpen;
    var downloading = false;
    function DownloadClose() {
        $("#dialogDownload").ejDialog("close");
        if (!downloading) {
            HideWaiting();
            EnableButtons();
        }
    }
    DataWizard.DownloadClose = DownloadClose;
    function Download() {
        downloading = true;
        $("#dialogDownload").ejDialog("close");
        ShowWaiting();
        $.get("/DataWizard/GetDownload")
            .done(function (data) {
            downloading = false;
            //EnableButtons();
            HideWaiting();
            window.location = data.url;
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("GetDownload", status, error);
        });
    }
    DataWizard.Download = Download;
})(DataWizard || (DataWizard = {}));
//# sourceMappingURL=DataWizard.js.map
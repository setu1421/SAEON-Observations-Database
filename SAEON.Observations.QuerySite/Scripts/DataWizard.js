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
        tab.option("selectedItemIndex", 0);
    }
    function SelectedTab() {
        var tab = $("#DataWizardTabs").data("ejTab");
        return tab.model.selectedItemIndex;
    }
    // Features fix on 1st load
    var locationsSelected = false;
    var featuresSelected = false;
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
    function LocationsReady() {
        locationsReady = true;
        CheckReady();
    }
    DataWizard.LocationsReady = LocationsReady;
    function LocationsChanged() {
        UpdateLocationsSelected(true);
    }
    DataWizard.LocationsChanged = LocationsChanged;
    function UpdateLocationsSelected(isClick) {
        if (isClick === void 0) { isClick = false; }
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
            HideWaiting();
            EnableButtons();
        }
    }
    function FeaturesChanged() {
        UpdateFeaturesSelected(true);
    }
    DataWizard.FeaturesChanged = FeaturesChanged;
    function UpdateFeaturesSelected(isClick) {
        if (isClick === void 0) { isClick = false; }
        var treeObj = $("#treeViewFeatures").data('ejTreeView');
        var nodes = treeObj.getCheckedNodes();
        var selected = [];
        var i;
        for (i = 0; i < nodes.length; i++) {
            var nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
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
    // Filters 
    function FiltersChanged() {
        UpdateFilters(true);
    }
    DataWizard.FiltersChanged = FiltersChanged;
    function UpdateFilters(isClick) {
        if (isClick === void 0) { isClick = false; }
        var startDate = $("#StartDate").ejDatePicker("instance").getValue();
        var endDate = $("#EndDate").ejDatePicker("instance").getValue();
        $.post("/DataWizard/UpdateFilters", { startDate: startDate, endDate: endDate })
            .done(function (data) {
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
            zoom: 5
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
                setTimeout(SelectTab(0), 1000);
                $("#PartialLocations").load("/DataWizard/GetLocationsHtml", function () {
                    $("#PartialLocationsSelected").load("/DataWizard/GetLocationsSelectedHtml", function () {
                        setTimeout(SelectTab(1), 1000);
                        $("#PartialFeatures").load("/DataWizard/GetFeaturesHtml", function () {
                            $("#PartialFeaturesSelected").load("/DataWizard/GetFeaturesSelectedHtml", function () {
                                setTimeout(SelectTab(2), 1000);
                                $("#PartialFilters").load("/DataWizard/GetFiltersHtml", function () {
                                    locationsSelected = false;
                                    featuresSelected = false;
                                    setTimeout(SelectTab(selectedTab), 1000);
                                    //UpdateMap();
                                    $("PartialTable").text("");
                                    $("PartialCards").text("");
                                    $("PartialChart").text("");
                                    HideResults();
                                    LoadQueryClose();
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
        EnableButtons();
    }
    DataWizard.Test = Test;
    // Download
    function DownloadOpen() {
        DisableButtons();
        $("#dialogDownload").ejDialog("open");
    }
    DataWizard.DownloadOpen = DownloadOpen;
    function DownloadClose() {
        $("#dialogDownload").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }
    DataWizard.DownloadClose = DownloadClose;
    function Download() {
        $("#dialogDownload").ejDialog("close");
        ShowWaiting();
        $.get("/DataWizard/GetDownload")
            .done(function (data) {
            EnableButtons();
            HideWaiting();
        })
            .fail(function (jqXHR, status, error) {
            ErrorInFunc("GetDownload", status, error);
        });
    }
    DataWizard.Download = Download;
})(DataWizard || (DataWizard = {}));
//# sourceMappingURL=DataWizard.js.map
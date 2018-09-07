namespace DataWizard {
    // Waiting

    export function ShowWaiting() {
        let wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }

    export function HideWaiting() {
        let wp = $("#waiting").data("ejWaitingPopup");
        wp.hide();
    }

    // Results

    export function ShowResults() {
        $("#TableTab").removeClass("d-none");
        $("#PartialTable").removeClass("d-none");
        $("#ChartTab").removeClass("d-none");
        $("#PartialChart").removeClass("d-none");
        //$("#CardsTab").removeClass("hidden");
    }

    export function HideResults() {
        $("#TableTab").addClass("d-none");
        $("#PartialTable").addClass("d-none");
        $("#ChartTab").addClass("d-none");
        $("#PartialChart").addClass("d-none");
        searched = false;
        //$("#CardsTab").addClass("hidden");
    }

    // Errors

    function ErrorInFunc(method: string, status: string, error: string) {
        HideWaiting();
        alert("Error in " + method + " Status: " + status + " Error: " + error);
    }

    // Tabs

    function SelectTab(index: number) {
        let tab = $("#DataWizardTabs").data("ejTab");
        tab.option("selectedItemIndex", 0);
    }

    function SelectedTab(): number {
        let tab = $("#DataWizardTabs").data("ejTab");
        return tab.model.selectedItemIndex;
    }

    // Features fix on 1st load

    let locationsSelected: boolean = false;
    let featuresSelected: boolean = false;

    export function TabActive() {
        let selectedTab = SelectedTab();
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

    // Buttons

    let IsAuthenticated: boolean = false;
    export function SetIsAuthenticated(aValue: boolean) {
        IsAuthenticated = aValue;
    }

    export function EnableButtons() {
        let btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        let btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        let btnSearch = $("#btnSearch").data("ejButton");
        let btnDownload = $("#btnDownload").data("ejButton");
        btnLoadQuery.disable();
        btnSaveQuery.disable();
        btnSearch.disable();
        btnDownload.disable();
        let locationsSelected: boolean = $("#treeViewLocations").data('ejTreeView').getCheckedNodes().length > 0;
        let featuresSelected: boolean = $("#treeViewFeatures").data('ejTreeView').getCheckedNodes().length > 0;
        SetApproximation();
        if (IsAuthenticated && UserQueriesCount > 0) {
            btnLoadQuery.enable();
        }
        if (locationsSelected && featuresSelected) {
            if (IsAuthenticated) {
                btnSaveQuery.enable();
            }
            btnSearch.enable();
            if (searched) {
                btnDownload.enable();
            }
        }
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

    // Locations

    let locationsReady: boolean = false;

    export function LocationsReady() {
        locationsReady = true;
        CheckReady();
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
                HideResults();
                EnableButtons();
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("UpdateLocationsSelected", status, error)
            });
    }

    // Features

    let featuresReady: boolean = false;

    export function FeaturesReady() {
        featuresReady = true;
        CheckReady();
    }

    function CheckReady() {
        if (locationsReady && featuresReady) {
            UpdateLocationsSelected();
            UpdateFeaturesSelected();
            HideWaiting();
            EnableButtons();
        }
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
                HideResults();
                EnableButtons();
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("UpdateFeaturesSelected", status, error)
            });
    }

    // Filters 

    export function UpdateFilters() {
        let startDate = $("#StartDate").ejDatePicker("instance").getValue();
        let endDate = $("#EndDate").ejDatePicker("instance").getValue();
        $.post("/DataWizard/UpdateFilters", { startDate: startDate, endDate: endDate })
            .done(function (data) {
                HideResults();
                EnableButtons();
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("UpdateFilters", status, error)
            });
    }

    // Map 

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
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("UpdateMap", status, error)
            });
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

    // Aproximation

    export function GetApproximation(): string {
        let approximation: string = "{}";
        $.get("/DataWizard/GetApproximation")
            .done(function (data) {
                approximation = data;
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("GetApproximation", status, error)
            });
        return approximation;
    }

    export function SetApproximation() {
        $.get("/DataWizard/SetApproximation")
            .done(function (data) {
                $('#PartialApproximation').html(data);
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("SetApproximation", status, error)
            });
    }

    // Search

    let searched: boolean = false;

    export function Search() {
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
                ErrorInFunc("GetData", status, error)
            });
    }

    // User Queries

    let UserQueriesCount: number = 0;
    export function SetUserQueriesCount(aValue: number) {
        UserQueriesCount = aValue;
    }

    // LoadQuery
    export function LoadQueryOpen() {
        DisableButtons();
        $("#dialogLoadQuery").ejDialog("open");
        $("#editLoadQueryName").ejAutocomplete("open");
    }

    export function LoadQueryClose() {
        $("#editLoadQueryName").ejAutocomplete("hide");
        $("#dialogLoadQuery").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }

    export function LoadQueryNameChange() {
        let loadName = $("#editLoadQueryName").val();
        let btnLoad = $("#btnLoadQueryLoad").data("ejButton");
        if (loadName == "") {
            btnLoad.disable()
        }
        else {
            btnLoad.enable()
        }
    }

    export function LoadQuery() {
        ShowWaiting();
        $.ajax({
            url: "/DataWizard/LoadQuery",
            data: JSON.stringify({ Name: $("#editLoadQueryName").val() }),
            async: true,
            type: "POST",
            contentType: "application/json",
            success: function () {
                let selectedTab = SelectedTab();
                setTimeout(SelectTab(0), 1000);
                $("#PartialLocations").load("/DataWizard/GetLocationsHtml", function () {
                    setTimeout(SelectTab(0), 1000);
                    $("#PartialLocationsSelected").load("/DataWizard/GetLocationsSelectedHtml", function () {
                        setTimeout(SelectTab(1), 1000);
                        $("#PartialFeatures").load("/DataWizard/GetFeaturesHtml", function () {
                            setTimeout(SelectTab(1), 1000);
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
                ErrorInFunc("LoadQueryLoad", status, error)
            }
        });
    }

    // Save Query
    export function SaveQueryOpen() {
        DisableButtons();
        $("#dialogSaveQuery").ejDialog("open");
    }

    export function SaveQueryClose() {
        $("#dialogSaveQuery").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }

    export function SaveQueryNameChange() {
        var saveName = $("#editSaveQueryName").val();
        var btnSave = $("#btnSaveQuerySave").data("ejButton");
        if (saveName == "") {
            btnSave.disable()
        }
        else {
            btnSave.enable()
        }
    }

    export function SaveQuery() {
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
                ErrorInFunc("SaveQuery", status, error)
            });
    }

    export function Test() {
        if (SelectedTab() == 0) {
            SelectTab(1);
        }
        else {
            SelectTab(0);
        }
    }

    // Download
    export function DownloadOpen() {
        DisableButtons();
        $("#dialogDownload").ejDialog("open");
    }

    export function DownloadClose() {
        $("#dialogDownload").ejDialog("close");
        HideWaiting();
        EnableButtons();
    }

    export function Download() {
        $("#dialogDownload").ejDialog("close");
        ShowWaiting();
        setTimeout(null, 10000);
        HideWaiting();
        EnableButtons();
    }
}
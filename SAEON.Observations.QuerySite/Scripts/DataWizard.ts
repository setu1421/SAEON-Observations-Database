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
        tab.option("selectedItemIndex", index);
    }

    function SelectedTab(): number {
        let tab = $("#DataWizardTabs").data("ejTab");
        return tab.model.selectedItemIndex;
    }

    // Features fix on 1st load

    let locationsSelected: boolean = false;
    let locationsExpand: boolean = false;
    let featuresSelected: boolean = false;
    let featuresExpand: boolean = false;

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

    export function TabCreate() {
        //$("a[href='#LocationsTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Organisations, Sites, Stations"></i>');
        //$("a[href='#FeaturesTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Phenomena, Offerings, Units"></i>');
        //$("a[href='#FiltersTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Dates, Depth"></i>');
    }

    // State
    class State {
        IsAuthenticated: boolean;
        LoadEnabled: boolean;
        SaveEnabled: boolean;
        SearchEnabled: boolean;
        DownloadEnabled: boolean;
    }

    // Buttons

    export function EnableButtons() {
        let btnLoadQuery = $("#btnLoadQuery").data("ejButton");
        let btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        let btnSearch = $("#btnSearch").data("ejButton");
        let btnDownload = $("#btnDownload").data("ejButton");
        $.get("/DataWizard/GetState")
            .done(function (state: State) {
                if (state.LoadEnabled) {
                    btnLoadQuery.enable();
                }
                else {
                    btnLoadQuery.disable();
                }

                if (state.SaveEnabled) {
                    btnSaveQuery.enable();
                } else {
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
                ErrorInFunc("GetState", status, error)
            });
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
    let locationsLoaded: boolean = false;

    export function LocationsReady() {
        locationsReady = true;
        CheckReady();
    }

    export function LocationsLoadComplete() {
        if (!locationsLoaded) {
            locationsLoaded = true;
            //UpdateLocationsSelected();
            //UpdateFeaturesSelected();
            HideWaiting();
            EnableButtons();
        }
    }

    export function LocationsChanged() {
        UpdateLocationsSelected(true);
    }

    function UpdateLocationsSelected(isClick: boolean = false) {
        if (loading) return;
        let treeObj = $("#treeViewLocations").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let selected = []
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
            if (locationsExpand) {
                let parent = treeObj.getParent(nodes[i]);
                while (parent != null) {
                    treeObj.expandNode(parent);
                    parent = treeObj.getParent(parent);
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
                ErrorInFunc("UpdateLocationsSelected", status, error)
            });
    }


    function ExpandLocationsSelected() {
        let treeObj = $("#treeViewLocations").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let parent = treeObj.getParent(nodes[i]);
            while (parent != null)
            {
                treeObj.expandNode(parent);
                parent = treeObj.getParent(parent);
            }
        }
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
            //HideWaiting();
            //EnableButtons();
        }
    }

    export function FeaturesChanged() {
        UpdateFeaturesSelected(true);
    }

    function UpdateFeaturesSelected(isClick: boolean = false) {
        if (loading) return;
        let treeObj = $("#treeViewFeatures").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let selected = []
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let nodeData = treeObj.getNode(nodes[i]);
            selected.push(nodeData.id);
            if (featuresExpand) {
                let parent = treeObj.getParent(nodes[i]);
                while (parent != null) {
                    treeObj.expandNode(parent);
                    parent = treeObj.getParent(parent);
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
                ErrorInFunc("UpdateFeaturesSelected", status, error)
            });
    }

    function ExpandFeaturesSelected() {
        let treeObj = $("#treeViewFeatures").data('ejTreeView');
        let nodes = treeObj.getCheckedNodes();
        let i: number;
        for (i = 0; i < nodes.length; i++) {
            let parent = treeObj.getParent(nodes[i]);
            while (parent != null) {
                treeObj.expandNode(parent);
                parent = treeObj.getParent(parent);
            }
        }
    }

    // Filters

    export function FiltersChanged() {
        UpdateFilters(true);
    }

    function UpdateFilters(isClick: boolean = false) {
        let startDate = $("#StartDate").ejDatePicker("instance").getValue();
        let endDate = $("#EndDate").ejDatePicker("instance").getValue();
        let range = $("#ElevationSlider").ejSlider("instance").getValue();
        let elevationMinimum = range[0];
        let elevationMaximum = range[1];
        $.post("/DataWizard/UpdateFilters", { startDate: startDate, endDate: endDate, elevationMinimum: elevationMinimum, elevationMaximum: elevationMaximum })
            .done(function (data) {
                SetApproximation();
                if (isClick) {
                    HideResults();
                }
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
            zoom: 5,
            mapTypeId: google.maps.MapTypeId.SATELLITE
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
                        marker.setIcon('https://maps.google.com/mapfiles/ms/icons/green-dot.png');
                    }
                    else {
                        marker.setIcon('https://maps.google.com/mapfiles/ms/icons/red-dot.png');
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
                        SelectTab(5);
                    });
                    //    });
                });
            })
            .fail(function (jqXHR, status, error) {
                ErrorInFunc("GetData", status, error)
            });
    }

    // User Queries

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

    let loading: boolean = false;
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

    // Download
    export function DownloadOpen() {
        DisableButtons();
        $("#dialogDownload").ejDialog("open");
    }

    let downloading: boolean = false;

    export function DownloadClose() {
        $("#dialogDownload").ejDialog("close");
        if (!downloading) {
            HideWaiting();
            EnableButtons();
        }
    }

    export function Download() {
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
                ErrorInFunc("GetDownload", status, error)
            });
    }
}
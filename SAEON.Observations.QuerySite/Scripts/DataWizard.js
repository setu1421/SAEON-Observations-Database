// Waiting
export function ShowWaiting() {
    const wp = $("#waiting").data("ejWaitingPopup");
    wp.show();
}
export function HideWaiting() {
    const wp = $("#waiting").data("ejWaitingPopup");
    wp.hide();
}
export function Testing() {
    alert("Testing");
}
// Variables
const locationsExpand = true;
let locationsLoaded = false;
let locationsReady = false;
let locationsSelected = false;
let loading = false;
//let searched = false;
let selectedTab = -1;
const variablesExpand = true;
let variablesReady = false;
let variablesSelected = false;
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
    //searched = false;
    //$("#CardsTab").addClass("hidden");
}
// Errors
function ErrorInFunc(method, status, error) {
    HideWaiting();
    alert("Error in " + method + " Status: " + status + " Error: " + error);
}
// Tabs
function SelectTab(index) {
    const tab = $("#DataWizardTabs").data("ejTab");
    tab.option("selectedItemIndex", index);
}
function SelectedTab() {
    const tab = $("#DataWizardTabs").data("ejTab");
    return tab.model.selectedItemIndex;
}
export function TabCreate() {
    //$("a[href='#LocationsTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Organisations, Sites, Stations"></i>');
    //$("a[href='#VariablesTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Phenomena, Offerings, Units"></i>');
    //$("a[href='#FiltersTab']").append('&nbsp<i class="fa fa-info" data-toggle="tooltip" data-placement="bottom" title="Dates, Depth"></i>');
}
// State
class State {
    constructor() {
        this.IsAuthenticated = false;
        this.IsDataset = false;
        this.LoadEnabled = false;
        this.SaveEnabled = false;
        this.SearchEnabled = false;
        this.DownloadEnabled = false;
    }
}
// Buttons
export function EnableButtons() {
    const btnLoadQuery = $("#btnLoadQuery").data("ejButton");
    const btnSaveQuery = $("#btnSaveQuery").data("ejButton");
    const btnSearch = $("#btnSearch").data("ejButton");
    const btnDownload = $("#btnDownload").data("ejButton");
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
export function DisableButtons() {
    const btnLoadQuery = $("#btnLoadQuery").data("ejButton");
    const btnSaveQuery = $("#btnSaveQuery").data("ejButton");
    const btnSearch = $("#btnSearch").data("ejButton");
    const btnDownload = $("#btnDownload").data("ejButton");
    btnLoadQuery.disable();
    btnSaveQuery.disable();
    btnSearch.disable();
    btnDownload.disable();
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
        ErrorInFunc("SetApproximation", status, error);
    });
}
let map;
let markers = [];
let mapPoints;
let mapBounds;
let mapFitted = false;
export function FitMap(override = false) {
    if (override || (!mapFitted && (mapBounds !== null) && !mapBounds.isEmpty())) {
        map.setCenter(mapBounds.getCenter());
        map.fitBounds(mapBounds);
        mapFitted = true;
    }
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
            const mapPoint = mapPoints[i];
            if (mapPoint.Latitude === null || mapPoint.Longitude === null)
                alert("Null");
            const marker = new google.maps.Marker({
                position: { lat: mapPoint.Latitude, lng: mapPoint.Longitude },
                map: map,
                title: mapPoint.Title
            });
            markers.push(marker);
            mapBounds.extend(marker.getPosition());
            if (mapPoint.IsSelected) {
                marker.setIcon('/Images/green-dot.png');
            }
            else {
                marker.setIcon('/Images/red-dot.png');
            }
        }
    })
        .fail(function (jqXHR, status, error) {
        ErrorInFunc("UpdateMap", status, error);
    });
}
export function InitMap() {
    const mapOpts = {
        center: new google.maps.LatLng(-34, 25.5),
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.SATELLITE
    };
    map = new google.maps.Map(document.getElementById('mapLocations'), mapOpts);
    UpdateMap();
}
export function FixMap() {
    UpdateMap();
    FitMap(true);
}
// Locations
export function LocationsLoadComplete() {
    if (!locationsLoaded) {
        locationsLoaded = true;
        //UpdateLocationsSelected();
        //UpdateVariablesSelected();
        HideWaiting();
        EnableButtons();
    }
}
function UpdateLocationsSelected(isClick = false) {
    if (loading)
        return;
    const treeObj = $("#treeViewLocations").data('ejTreeView');
    const nodes = treeObj.getCheckedNodes();
    const selected = [];
    let i;
    for (i = 0; i < nodes.length; i++) {
        const nodeData = treeObj.getNode(nodes[i]);
        selected.push(nodeData.id);
        if (locationsExpand) {
            let parent = treeObj.getParent(nodes[i]);
            while (parent) {
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
        ErrorInFunc("UpdateLocationsSelected", status, error);
    });
}
export function LocationsChanged() {
    UpdateLocationsSelected(true);
}
function ExpandLocationsSelected() {
    const treeObj = $("#treeViewLocations").data('ejTreeView');
    const nodes = treeObj.getCheckedNodes();
    let i;
    for (i = 0; i < nodes.length; i++) {
        let parent = treeObj.getParent(nodes[i]);
        while (parent) {
            treeObj.expandNode(parent);
            parent = treeObj.getParent(parent);
        }
    }
}
// Variables
function UpdateVariablesSelected(isClick = false) {
    if (loading)
        return;
    const treeObj = $("#treeViewVariables").data('ejTreeView');
    const nodes = treeObj.getCheckedNodes();
    const selected = [];
    let i;
    for (i = 0; i < nodes.length; i++) {
        const nodeData = treeObj.getNode(nodes[i]);
        selected.push(nodeData.id);
        if (variablesExpand) {
            let parent = treeObj.getParent(nodes[i]);
            while (parent) {
                treeObj.expandNode(parent);
                parent = treeObj.getParent(parent);
            }
        }
    }
    $.post("/DataWizard/UpdateVariablesSelected", { variables: selected })
        .done(function (data) {
        $('#PartialVariablesSelected').html(data);
        SetApproximation();
        if (isClick) {
            HideResults();
        }
        EnableButtons();
    })
        .fail(function (jqXHR, status, error) {
        ErrorInFunc("UpdateVariablesSelected", status, error);
    });
}
function ExpandVariablesSelected() {
    const treeObj = $("#treeViewVariables").data('ejTreeView');
    const nodes = treeObj.getCheckedNodes();
    let i;
    for (i = 0; i < nodes.length; i++) {
        let parent = treeObj.getParent(nodes[i]);
        while (parent) {
            treeObj.expandNode(parent);
            parent = treeObj.getParent(parent);
        }
    }
}
export function VariablesChanged() {
    UpdateVariablesSelected(true);
}
// Ready
function CheckReady() {
    if (locationsReady && variablesReady) {
        UpdateLocationsSelected();
        UpdateVariablesSelected();
        //HideWaiting();
        //EnableButtons();
    }
}
export function LocationsReady() {
    locationsReady = true;
    CheckReady();
}
export function VariablesReady() {
    variablesReady = true;
    CheckReady();
}
// Filters
function UpdateFilters(isClick = false) {
    const startDate = $("#StartDate").ejDatePicker("instance").getValue();
    const endDate = $("#EndDate").ejDatePicker("instance").getValue();
    const range = $("#ElevationSlider").ejSlider("instance").getValue();
    const elevationMinimum = range[0];
    const elevationMaximum = range[1];
    $.post("/DataWizard/UpdateFilters", { startDate: startDate, endDate: endDate, elevationMinimum: elevationMinimum, elevationMaximum: elevationMaximum })
        .done(function () {
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
export function FiltersChanged() {
    UpdateFilters(true);
}
// Datasets
export function DatasetOpen() {
    DisableButtons();
    $("#dialogDataset").ejDialog("open");
}
export function DatasetClose() {
    $("#dialogDataset").ejDialog("close");
    HideWaiting();
    EnableButtons();
}
//export function DatasetDownload() {
//    DatasetClose();
//    $.get("/DataWizard/DatasetDownload")
//        .done(function () {
//        })
//        .fail(function (jqXHR, status, error) {
//            ErrorInFunc("DatasetDownload", status, error)
//        });
//}
// Search
export function Search(event, isDataset = false) {
    ShowWaiting();
    $.get("/DataWizard/GetData")
        .done(function () {
        $("#PartialTable").load("/DataWizard/GetTableHtml", function () {
            $("#PartialChart").load("/DataWizard/GetChartHtml", function () {
                //$("#PartialCards").load("/DataWizard/GetCardsHtml", function () {
                ShowResults();
                //searched = true;
                EnableButtons();
                HideWaiting();
                if (isDataset === false) {
                    SelectTab(5);
                }
                else {
                    SelectTab(4);
                    DatasetOpen();
                }
            });
            //    });
        });
    })
        .fail(function (jqXHR, status, error) {
        ErrorInFunc("GetData", status, error);
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
    const loadName = $("#editLoadQueryName").val();
    const btnLoad = $("#btnLoadQueryLoad").data("ejButton");
    if (loadName === "") {
        btnLoad.disable();
    }
    else {
        btnLoad.enable();
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
            selectedTab = SelectedTab();
            $("#PartialLocations").load("/DataWizard/GetLocationsHtml", function () {
                $("#PartialLocationsSelected").load("/DataWizard/GetLocationsSelectedHtml", function () {
                    $("#PartialVariables").load("/DataWizard/GetVariablesHtml", function () {
                        $("#PartialVariablesSelected").load("/DataWizard/GetVariablesSelectedHtml", function () {
                            $("#PartialFilters").load("/DataWizard/GetFiltersHtml", function () {
                                //UpdateMap();
                                $("PartialTable").text("");
                                $("PartialCards").text("");
                                $("PartialChart").text("");
                                HideResults();
                                LoadQueryClose();
                                locationsSelected = false;
                                variablesSelected = false;
                                loading = false;
                                ExpandLocationsSelected();
                                ExpandVariablesSelected();
                                HideWaiting();
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
    const saveName = $("#editSaveQueryName").val();
    const btnSave = $("#btnSaveQuerySave").data("ejButton");
    if (saveName === "") {
        btnSave.disable();
    }
    else {
        btnSave.enable();
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
        ErrorInFunc("SaveQuery", status, error);
    });
}
export function Test() {
    //locationsSelected = false;
    //variablesSelected = false;
    //if (SelectedTab() == 0) {
    //    SelectTab(1)
    //} else {
    //    SelectTab(0);
    //}
    ExpandLocationsSelected();
    ExpandVariablesSelected();
}
// Download
export function DownloadOpen() {
    DisableButtons();
    $("#dialogDownload").ejDialog("open");
}
let downloading = false;
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
        window.open(data.downloadUrl, '_blank');
        window.location = data.viewDownloadUrl;
    })
        .fail(function (jqXHR, status, error) {
        ErrorInFunc("GetDownload", status, error);
    });
}
// Variables fix on 1st load
export function TabActive() {
    selectedTab = SelectedTab();
    if (selectedTab === 0) {
        if (!locationsSelected) {
            UpdateLocationsSelected();
            locationsSelected = true;
        }
    }
    else if (selectedTab === 1) {
        if (!variablesSelected) {
            UpdateVariablesSelected();
            variablesSelected = true;
        }
    }
    else if (selectedTab === 3) {
        FitMap();
    }
}

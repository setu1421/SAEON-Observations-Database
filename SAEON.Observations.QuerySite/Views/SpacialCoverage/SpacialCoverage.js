function ShowWaiting() {
    var wp = $("#waiting").data("ejWaitingPopup");
    wp.show();
}
function HideWaiting() {
    var wp = $("#waiting").data("ejWaitingPopup");
    wp.hide();
}
var mapVisible = false;
function onTabActive() {
    if (!mapVisible) {
        var tab = $("#SpacialCoverageTabs").data("ejTab");
        if (tab.selectedItemIndex() === 3) {
            UpdateMap();
            mapVisible = true;
        }
    }
}
function onSearchClick() {
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
function ErrorInFunc(msg) {
    HideWaiting();
    alert(msg);
}
function EnableButtons() {
    var btnSearch = $("#btnSearch").data("ejButton");
    treeObj = $("#treeViewLocations").data('ejTreeView');
    var checkedNodes = treeObj.getCheckedNodes();
    var selectedLocations = 0;
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
    if ((selectedLocations > 0) & (selectedFeatures > 0)) {
        btnSearch.enable();
    }
    else {
        btnSearch.disable();
    }
}
function DisableButtons() {
    var btnSearch = $("#btnSearch").data("ejButton");
    btnSearch.disable();
}
function onUpdateSelectedLocations() {
    treeObj = $("#treeViewLocations").data('ejTreeView');
    var nodes = treeObj.getCheckedNodes();
    var selected = [];
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
function onUpdateSelectedFeatures() {
    treeObj = $("#treeViewFeatures").data('ejTreeView');
    var nodes = treeObj.getCheckedNodes();
    var selected = [];
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
function onUpdateFilters() {
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

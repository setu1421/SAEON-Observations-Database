namespace Query {
    export function ShowWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.show();
    }
    export function HideWaiting() {
        var wp = $("#waiting").data("ejWaitingPopup");
        wp.hide();
    }
    export function onSplitterResize() {
        UpdateMap();
    }
    export function onLoadQueryClick() {
        DisableButtons();
        $("#dialogLoadQuery").ejDialog("open");
    }
    export function onLoadNameChange() {
        var loadName = $("#editLoadName").val();
        var btnLoad = $("#btnLoad").data("ejButton");
        if (loadName == "") {
            btnLoad.disable()
        }
        else {
            btnLoad.enable()
        }
    }
    export function onLoadClick() {
        ShowWaiting();
        $.ajax({
            url: "/Query/LoadQuery",
            data: JSON.stringify({
                Name: $("#editLoadName").val()
            }),
            async: false,
            type: "POST",
            contentType: "application/json",
            success: function () {
                $("#PartialLocations").load("/Query/GetLocationsHtml", function () {
                    $("#PartialSelectedLocations").load("/Query/GetSelectedLocationsHtml", function () {
                        $("#PartialFeatures").load("/Query/GetFeaturesHtml", function () {
                            $("#PartialSelectedFeatures").load("/Query/GetSelectedFeaturesHtml", function () {
                                $("#PartialFilters").load("/Query/GetFiltersHtml", function () {
                                    UpdateMap();
                                    $("PartialTable").text("");
                                    $("PartialCards").text("");
                                    $("PartialChart").text("");
                                    HideResults();
                                    $("#dialogLoadQuery").ejDialog("close");
                                    HideWaiting();
                                    EnableButtons();
                                });
                            });
                        });
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ErrorInFunc("Error in onLoadClick Status: " + textStatus + " Error: " + errorThrown);
            }
        });
    }
    export function onLoadQueryClose() {
        HideWaiting();
        EnableButtons();
    }
    export function onSaveQueryClick() {
        DisableButtons();
        $("#dialogSaveQuery").ejDialog("open");
    }
    export function onSaveNameChange() {
        var saveName = $("#editSaveName").val();
        var btnSave = $("#btnSave").data("ejButton");
        if (saveName == "") {
            btnSave.disable()
        }
        else {
            btnSave.enable()
        }
    }
    export function onSaveClick() {
        ShowWaiting();
        $.ajax({
            url: "/Query/SaveQuery",
            data: JSON.stringify({
                Name: $("#editSaveName").val(),
                Description: $("#SaveDescription").val()
            }),
            async: true,
            type: "POST",
            contentType: "application/json"
        })
            .done(function () {
                $("#dialogSaveQuery").ejDialog("close");
                HideWaiting();
                EnableButtons();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                if (errorThrown == "Conflict") {
                    $("#SaveError").text("A query with that name already exists!");
                    HideWaiting();
                }
                else {
                    ErrorInFunc("Error in onSaveClick Status: " + textStatus + " Error: " + errorThrown);
                }
            });
    }
    export function onSaveQueryClose() {
        HideWaiting();
        EnableButtons();
    }
    export function onSearchClick() {
        ShowWaiting();
        $.get("/Query/GetData")
            .done(function () {
                $("#PartialTable").load("/Query/GetTableHtml", function () {
                    $("#PartialCards").load("/Query/GetCardsHtml", function () {
                        $("#PartialChart").load("/Query/GetChartHtml", function () {
                            ShowResults();
                            EnableButtons();
                            HideWaiting();
                        });
                    });
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                ErrorInFunc("Error in GetData Status: " + textStatus + " Error: " + errorThrown)
            });
    }
    export function onDownloadClick() {

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
        btnLoadQuery.enable();
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
        btnLoadQuery.disable();
        var btnSaveQuery = $("#btnSaveQuery").data("ejButton");
        btnSaveQuery.disable();
        var btnSearch = $("#btnSearch").data("ejButton");
        btnSearch.disable();
        var btnDownload = $("#btnDownload").data("ejButton");
        btnDownload.disable();
    }
    export function ShowResults() {
        $("#TableTab").removeClass("hidden");
        $("#CardsTab").removeClass("hidden");
        $("#ChartTab").removeClass("hidden");
    }
    export function HideResults() {
        $("#TableTab").addClass("hidden");
        $("#CardsTab").addClass("hidden");
        $("#ChartTab").addClass("hidden");
    }

    function UpdateMap() {
    }

}
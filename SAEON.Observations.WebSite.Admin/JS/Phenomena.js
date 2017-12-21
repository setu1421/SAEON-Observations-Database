function onCommand(e, record) {
    if (e === "Edit") {
        DetailsFormPanel.getForm().reset();
        DetailsFormPanel.getForm().loadRecord(record);
        DetailsFormPanel.getForm().clearInvalid();

        tfCode.rvConfig.remoteValidated = false;
        tfCode.rvConfig.remoteValid = false;

        tfName.rvConfig.remoteValidated = false;
        tfName.rvConfig.remoteValid = false;

        tfCode.markAsValid();
        tfName.markAsValid();
        DetailWindow.show();
    }
}

function PhenomenaRowSelect(e, record) {
    if (tpCenter.isVisible()) {
        PhenomenonOfferingGrid.getStore().reload();
        PhenomenonUOMGrid.getStore().reload();
    }
}

function New() {

    DetailsFormPanel.getForm().reset();
    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    DetailWindow.show();
}

function CloseAvailableOffering() {
    OfferingGrid.selModel.clearSelections();
}

function CloseAvailableUnit() {
    UnitOfMeasureGrid.selModel.clearSelections();
}
function onCommand(e, record) {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    tfCode.markAsValid();
    tfName.markAsValid();
    DetailWindow.show()
}

function PhenomenonRowSelect(e, record) {
    if (pnlSouth.isVisible())
        PhenomenonUOMGrid.getStore().reload();

    if (pnlEast.isVisible())
        PhenomenonOfferingGrid.getStore().reload();
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
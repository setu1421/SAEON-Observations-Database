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
    DetailWindow.show();
}

function New() {

    DetailsFormPanel.getForm().reset();
    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    DetailWindow.show();

}

function MasterRowSelect(e, record) {
    if (pnlSouth.isVisible())
        StationGrid.getStore().reload();
    if (pnlEast.isVisible())
        OrganisationGrid.getStore().reload();
}

function CloseAvailableStations() {
    AvailableStationsGrid.selModel.clearSelections();
}

function ClearOrganisationForm() {
    OrganisationFormPanel.getForm().reset();
}

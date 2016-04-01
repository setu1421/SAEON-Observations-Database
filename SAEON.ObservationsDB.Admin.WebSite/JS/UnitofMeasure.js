function onCommand(e, record) {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValid = false;
    tfCode.rvConfig.remoteValid = false;

    tfUnit.rvConfig.remoteValidated = false;
    tfUnit.rvConfig.remoteValid = false;

    tfCode.markAsValid();
    tfUnit.markAsValid();
    DetailWindow.show()
}

function New() {

    DetailsFormPanel.getForm().reset();
    tfCode.rvConfig.remoteValid = false;
    tfCode.rvConfig.remoteValid = false;

    tfUnit.rvConfig.remoteValidated = false;
    tfUnit.rvConfig.remoteValid = false;
    DetailWindow.show();

}
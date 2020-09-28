function onCommand(e, record) {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;
    tfCode.markAsValid();

    tfUnit.rvConfig.remoteValidated = false;
    tfUnit.rvConfig.remoteValid = false;
    tfUnit.markAsValid();

    DetailWindow.show();
}

function New() {

    DetailsFormPanel.getForm().reset();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;
    tfCode.markAsValid();

    tfUnit.rvConfig.remoteValidated = false;
    tfUnit.rvConfig.remoteValid = false;
    tfUnit.markAsValid();

    DetailWindow.show();
}

function MasterRowSelect(e, record) {
    if (tpCenter.isVisible()) {
        UnitOfMeasurePhenomenaGrid.getStore().reload();
    }
}

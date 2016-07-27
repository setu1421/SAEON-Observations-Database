function onCommand(e, record) {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    tfDescription.rvConfig.remoteValidated = false;
    tfDescription.rvConfig.remoteValid = false;

    tfCode.markAsValid();
    tfName.markAsValid();
    tfDescription.markAsValid();

    DetailWindow.show();
}

function New() {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    tfDescription.rvConfig.remoteValidated = false;
    tfDescription.rvConfig.remoteValid = false;

    tfCode.markAsValid();
    tfName.markAsValid();
    tfDescription.markAsValid();

    DetailWindow.show();
}

function MasterRowSelect(e, record) {
    if (pnlSouth.isVisible()) {
        OrganisationLinksGrid.getStore().reload();
        StationLinksGrid.getStore().reload();
        DataSourceLinksGrid.getStore().reload();
        SensorLinksGrid.getStore().reload();
    }
}

function ClearOrganisationLinkForm() {
    OrganisationLinkFormPanel.getForm().reset();
}

function PrepareOrganisationLinkToolbar(grid, toolbar, rowIndex, record) {
    if (record.data.IsReadOnly) {
        toolbar.items.get(0).setDisabled(true);
        toolbar.items.get(1).setDisabled(true);
    }
}

function OnOrganisationLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteOrganisationLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        OrganisationLinkFormPanel.getForm().reset();
        OrganisationLinkFormPanel.getForm().loadRecord(record);
        OrganisationLinkFormPanel.getForm().clearInvalid();
        OrganisationLinkWindow.show();
    }
}

function ClearStationLinkForm() {
    StationLinkFormPanel.getForm().reset();
}

function OnStationLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteStationLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        StationLinkFormPanel.getForm().reset();
        StationLinkFormPanel.getForm().loadRecord(record);
        StationLinkFormPanel.getForm().clearInvalid();
        StationLinkWindow.show();
    }
}

function ClearDataSourceLinkForm() {
    DataSourceLinkFormPanel.getForm().reset();
}

function OnDataSourceLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteDataSourceLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        DataSourceLinkFormPanel.getForm().reset();
        DataSourceLinkFormPanel.getForm().loadRecord(record);
        DataSourceLinkFormPanel.getForm().clearInvalid();
        DataSourceLinkWindow.show();
    }
}

function ClearSensorLinkForm() {
    SensorLinkFormPanel.getForm().reset();
}

function OnSensorLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSensorLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SensorLinkFormPanel.getForm().reset();
        SensorLinkFormPanel.getForm().loadRecord(record);
        SensorLinkFormPanel.getForm().clearInvalid();
        SensorLinkWindow.show();
    }
}


﻿function onCommand(e, record) {
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
    {
        OrganisationLinksGrid.getStore().reload();
        SensorsGrid.getStore().reload();
    }
}

function ClearOrganisationLinkForm() {
    OrganisationLinkFormPanel.getForm().reset();
}

function CloseAvailableSensors() {
    AvailableSensorsGrid.selModel.clearSelections();
}

function onOrganisationLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteOrganisationLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        OrganisationLinkFormPanel.getForm().reset();
        OrganisationLinkFormPanel.getForm().loadRecord(record);
        OrganisationLinkFormPanel.getForm().clearInvalid();
        OrganisationLinkWindow.show();
    }
}

function onSensorLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSensorLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SensorLinkFormPanel.getForm().reset();
        SensorLinkFormPanel.getForm().loadRecord(record);
        SensorLinkFormPanel.getForm().clearInvalid();
        SensorLinkWindow.show();
    }
}


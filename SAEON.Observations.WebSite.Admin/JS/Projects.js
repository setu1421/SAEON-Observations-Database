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

    DetailWindow.show()
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
    if (pnlSouth.isVisible())
        StationLinksGrid.getStore().reload();
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



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

function New() {

    DetailsFormPanel.getForm().reset();
    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    DetailWindow.show();

}

function MasterRowSelect(e, record) {
    if (pnlEast.isVisible()) {
        SiteLinksGrid.getStore().reload();
        StationLinksGrid.getStore().reload();
        InstrumentLinksGrid.getStore().reload();
    }
}

function ClearSiteLinkForm() {
    SiteLinkFormPanel.getForm().reset();
}

function onSiteLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSiteLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SiteLinkFormPanel.getForm().reset();
        SiteLinkFormPanel.getForm().loadRecord(record);
        SiteLinkFormPanel.getForm().clearInvalid();
        SiteLinkWindow.show();
    }
}

function ClearStationLinkForm() {
    StationLinkFormPanel.getForm().reset();
}

function onStationLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteStationLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        StationLinkFormPanel.getForm().reset();
        StationLinkFormPanel.getForm().loadRecord(record);
        StationLinkFormPanel.getForm().clearInvalid();
        StationLinkWindow.show();
    }
}

function ClearInstrumentLinkForm() {
    InstrumentLinkFormPanel.getForm().reset();
}

function onInstrumentLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteInstrumentLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        InstrumentLinkFormPanel.getForm().reset();
        InstrumentLinkFormPanel.getForm().loadRecord(record);
        InstrumentLinkFormPanel.getForm().clearInvalid();
        InstrumentLinkWindow.show();
    }
}




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
        SiteLinksGrid.getStore().reload();
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



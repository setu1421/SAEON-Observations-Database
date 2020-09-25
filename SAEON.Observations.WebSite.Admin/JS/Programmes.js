function onCommand(e, record) {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;
    tfCode.markAsValid();

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;
    tfName.markAsValid();

    DetailWindow.show();
}

function New() {
    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;
    tfCode.markAsValid();

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;
    tfName.markAsValid();

    DetailWindow.show();
}

function MasterRowSelect(e, record) {
    if (tpCenter.isVisible())
        ProjectLinksGrid.getStore().reload();
}

function ClearProjectLinkForm() {
    ProjectLinkFormPanel.getForm().reset();
}

function CloseAvailableProjects() {
    AvailableProjectsGrid.selModel.clearSelections();
}

function OnProjectLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteProjectLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        ProjectLinkFormPanel.getForm().reset();
        ProjectLinkFormPanel.getForm().loadRecord(record);
        ProjectLinkFormPanel.getForm().clearInvalid();
        ProjectLinkWindow.show();
    }
}


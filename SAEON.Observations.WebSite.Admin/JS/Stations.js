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
    {
        OrganisationLinksGrid.getStore().reload();
        ProjectLinksGrid.getStore().reload();
        InstrumentLinksGrid.getStore().reload();
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

function ClearProjectLinkForm() {
    ProjectLinkFormPanel.getForm().reset();
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

function ClearInstrumentLinkForm() {
    InstrumentLinkFormPanel.getForm().reset();
}

function OnInstrumentLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteInstrumentLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        InstrumentLinkFormPanel.getForm().reset();
        InstrumentLinkFormPanel.getForm().loadRecord(record);
        InstrumentLinkFormPanel.getForm().clearInvalid();
        InstrumentLinkWindow.show();
    }
}

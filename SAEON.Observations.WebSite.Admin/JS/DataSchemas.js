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
        SchemaColumnLinksGrid.getStore().reload();
    }
}

function ClearSchemaColumnLinkForm() {
    SchemaColumnLinkFormPanel.getForm().reset();
}

function OnSchemaColumnLinkCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSchemaColumnLink(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SchemaColumnLinkFormPanel.getForm().reset();
        SchemaColumnLinkFormPanel.getForm().loadRecord(record);
        SchemaColumnLinkFormPanel.getForm().clearInvalid();
        SchemaColumnLinkWindow.show();
    }
}




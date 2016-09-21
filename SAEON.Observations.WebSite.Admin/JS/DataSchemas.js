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
    if (pnlSouth.isVisible()) {
        SchemaColumnsGrid.getStore().reload();
    }
}

function ClearSchemaColumnAddForm() {
    SchemaColumnAddFormPanel.getForm().reset();
}

function OnSchemaColumnAddCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSchemaColumn(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SchemaColumnAddFormPanel.getForm().reset();
        SchemaColumnAddFormPanel.getForm().loadRecord(record);
        SchemaColumnAddFormPanel.getForm().clearInvalid();
        SchemaColumnAddWindow.show();
    }
}




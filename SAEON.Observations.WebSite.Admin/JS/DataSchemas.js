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

function PrepareSchemaColumnsToolbar(grid, toolbar, rowIndex, record) {
    if (rowIndex === 0) {
        toolbar.items.get(2).setDisabled(true);
    } else if (rowIndex === SchemaColumnsGrid.getStore().getCount()-1) {
        toolbar.items.get(3).setDisabled(true);
    }
}

function OnSchemaColumnCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSchemaColumn(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
        SchemaColumnAddFormPanel.getForm().reset();
        SchemaColumnAddFormPanel.getForm().loadRecord(record);
        DirectCall.LoadCombos(record.get("SchemaColumnTypeID"), record.get("PhenomenonID"), record.get("PhenomenonOfferingID"), record.get("PhenomenonUOMID"));
        SchemaColumnAddFormPanel.getForm().clearInvalid();
        SchemaColumnAddWindow.show();
    } else if (e === 'Up') {
        DirectCall.SchemaColumnUp(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Down') {
        DirectCall.SchemaColumnDown(record.get('Id'), { eventMask: { showMask: true } });
    }
}




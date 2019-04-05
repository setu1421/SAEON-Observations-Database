function onCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteSchema(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Edit') {
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
    } else if (e === 'Copy') {
        SchemaCopyFormPanel.getForm().reset();
        SchemaCopyFormPanel.getForm().loadRecord(record);
        SchemaCopyFormPanel.getForm().clearInvalid();
        DirectCall.SetSchemaCopyFields();
        SchemaCopyWindow.show();
    }
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
    if (tpCenter.isVisible()) {
        SchemaColumnsGrid.getStore().reload();
        DataSourcesGrid.getStore().reload();
    }
}

function HideSchemaColumnForm() {
    SchemaColumnFormPanel.getForm().reset();
}

function ShowSchemaColumnForm() {
    DirectCall.SetFields();
    tfColumnName.setDisabled(false);
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
        SchemaColumnFormPanel.getForm().reset();
        SchemaColumnFormPanel.getForm().loadRecord(record);
        DirectCall.LoadCombos(record.get("SchemaColumnTypeID"), record.get("PhenomenonID"), record.get("PhenomenonOfferingID"), record.get("PhenomenonUOMID"));
        SchemaColumnFormPanel.getForm().clearInvalid();
        DirectCall.SetFields();
        SchemaColumnWindow.show();
        tfColumnName.setDisabled(true);
    } else if (e === 'Up') {
        DirectCall.SchemaColumnUp(record.get('Id'), { eventMask: { showMask: true } });
    } else if (e === 'Down') {
        DirectCall.SchemaColumnDown(record.get('Id'), { eventMask: { showMask: true } });
    }
}

function CloseAvailableDataSources() {
    AvailableDataSourcesGrid.selModel.clearSelections();
}

function OnDataSourceCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteDataSource(record.get('Id'), { eventMask: { showMask: true } });
    }
}

function GetInvalidFields(formPanel) {
    var s = 'Invalid: ';
    var form = formPanel.getForm();
    var fields = form.items;
    var i = 0;
    for (i = 0; i < fields.length; i += 1) {
        var field = fields.items[i];
        var input = form.findField(field.id);
        if (input) {
            input.validate();
            if (!input.isValid()) {
                s = s + " " + input.id;
            }
        }
    }
    if (s === 'Invalid: ') {
        s = 'Valid';
    }
    alert(s);
}



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

    var rd = cbDataSource.findRecord(cbDataSource.valueField, record.data.DataSourceID);

    if (rd.data.DataSchemaID === null)
        cbDataSchema.allowBlank = false;
    else
        cbDataSchema.allowBlank = true;

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

    cbDataSchema.markAsValid();

    cbDataSchema.allowBlank = true;

    DetailWindow.show();
}

function SelectDataSource(e, record) {

    if (record.data.DataSchemaID === null)
        cbDataSchema.allowBlank = false;
    else
        cbDataSchema.allowBlank = true;
}

function MasterRowSelect(e, record) {
    if (pnlSouth.isVisible()) {
        InstrumentLinksGrid.getStore().reload();
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


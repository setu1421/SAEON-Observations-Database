
function LoadCustomValidators() {
    Ext.apply(Ext.form.VTypes, {
        // Password Check
        duplicateNameText: 'The Field Name already exists.',
        duplicateName: function (v, d, e) {

            if (typeof (d.ownerCt.ownerCt.getForm()._record) != 'undefined' && d.ownerCt.ownerCt.getForm()._record.data.Name == v)
                return true;

            return DelimetedFieldsGrid.getStore().queryBy(function (record) {
                if (v != '')
                    return record.get('Name') == v;
            }).getCount() == 0;
        }
    });
}

function onCommand(e, record) {

    if (e != 'Edit')
        return;

    DetailsFormPanel.getForm().reset();
    DetailsFormPanel.getForm().loadRecord(record);
    DetailsFormPanel.getForm().clearInvalid();

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    delete tfCode.rvConfig.lastValue;


    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    delete tfName.rvConfig.lastValue;

    tfCode.markAsValid();
    tfName.markAsValid();

    if (record.data.DataSourceTypeID == '25839703-3cb3-4c23-aca3-4399cc52ecde')
        cbDelimiter.allowBlank = false;
    else
        cbDelimiter.allowBlank = true;


    DetailWindow.show();

}

function New() {

    DetailsFormPanel.getForm().reset();
    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;

    delete tfCode.rvConfig.lastValue;

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;

    delete tfName.rvConfig.lastValue;

    cbDelimiter.allowBlank = true;
    cbDelimiter.clearValue();
    cbDelimiter.clearInvalid();
    cbDelimiter.markAsValid();

    DetailWindow.show();

}

function onFieldsCommand(e, record, rowIndex, colIndex) {

    if (e == 'Edit') {

        if (record.data.Datefield) {
            DateFieldEditor.getForm().reset();
            DateFieldEditor.getForm().loadRecord(record);
            DateFieldEditor.getForm().clearInvalid();
            DateFieldEditor.getForm()._record = record;

            DateFieldName.rvConfig.remoteValidated = false;
            DateFieldName.rvConfig.remoteValid = false;
            delete DateFieldName.rvConfig.lastValue;

            DateFieldFormat.rvConfig.remoteValidated = false;
            DateFieldFormat.rvConfig.remoteValid = false;
            delete DateFieldFormat.rvConfig.lastValue;

            DateFieldName.markAsValid();
            DateFieldFormat.markAsValid();

            DateFieldWindow.show();
        }
        else if (record.data.Timefield) {
            TimeFieldEditor.getForm().reset();
            TimeFieldEditor.getForm().loadRecord(record);
            TimeFieldEditor.getForm()._record = record;

            TimeFieldEditor.getForm().clearInvalid();

            TimeFieldName.rvConfig.remoteValidated = false;
            TimeFieldName.rvConfig.remoteValid = false;
            delete TimeFieldName.rvConfig.lastValue;

            TimeFieldFormat.rvConfig.remoteValidated = false;
            TimeFieldFormat.rvConfig.remoteValid = false;
            delete TimeFieldFormat.rvConfig.lastValue;

            TimeFieldName.markAsValid();
            TimeFieldFormat.markAsValid();

            TimeFieldWindow.show();
        }
        else if (record.data.Ignorefield) {
            IgnoreFieldEditor.getForm().reset();
            IgnoreFieldEditor.getForm().loadRecord(record);
            IgnoreFieldEditor.getForm()._record = record;

            IgnoreFieldEditor.getForm().clearInvalid();

            IgnoreFieldName.rvConfig.remoteValidated = false;
            IgnoreFieldName.rvConfig.remoteValid = false;
            delete IgnoreFieldName.rvConfig.lastValue;

            IgnoreFieldName.markAsValid();

            IgnoreFieldWindow.show();
        }
        else if (record.data.Commentfield) {
            CommentFieldEditor.getForm().reset();
            CommentFieldEditor.getForm().loadRecord(record);
            CommentFieldEditor.getForm()._record = record;

            CommentFieldEditor.getForm().clearInvalid();

            CommentFieldName.rvConfig.remoteValidated = false;
            CommentFieldName.rvConfig.remoteValid = false;
            delete CommentFieldName.rvConfig.lastValue;

            CommentFieldName.markAsValid();

            CommentFieldWindow.show();
        }
        else if (record.data.PhenomenonID != null) {
            OfferingFieldEditor.getForm().reset();

            DelimetedFieldsGrid.el.mask('Please wait', 'x-mask-loading');

            OfferingFieldName.rvConfig.remoteValidated = false;
            OfferingFieldName.rvConfig.remoteValid = false;
            delete OfferingFieldName.rvConfig.lastValue;

            OfferingFieldName.markAsValid();

            var loadcallback = function () {
                cbOffering.getStore().removeListener('load', loadcallback);
                cbOffering.setValue(record.data.OfferingID);
            }

            var uomloadcallback = function () {
                cbUnitofMeasure.getStore().removeListener('load', uomloadcallback);
                cbUnitofMeasure.setValue(record.data.UnitofMeasureID);
                DelimetedFieldsGrid.el.unmask();
                OfferingFieldWindow.show();
            }

            if (record.data.FixedTimeField) {
                FieldFixedTimeValue.allowBlank = false;
                FixedTimePanel.show();
            }
            else {
                FieldFixedTimeValue.allowBlank = true;
                FixedTimePanel.hide();
            }


            OfferingFieldEditor.getForm().loadRecord(record);

            OfferingFieldEditor.getForm()._record = record;

            OfferingFieldEditor.getForm().clearInvalid();

            cbOffering.getStore().on("load", loadcallback);

            cbUnitofMeasure.getStore().on("load", uomloadcallback);

            cbPhenomenon.setValueAndFireSelect(record.data.PhenomenonID);

        }
    }
    else if (e == 'Delete') {
        DelimetedFieldsGrid.deleteRecord(record);
    }
}

function ShowDateFieldEditor() {

    if (Checkfield("Datefield", true)) {
        DateFieldEditor.getForm().reset();
        var rawdata = {};
        rawdata.Name = '';
        rawdata.Datefield = true;
        rawdata.Dateformat = '';

        var dsStore = DelimetedFieldsGrid.getStore();
        var emptydata = new dsStore.recordType(rawdata, Ext.id());

        DateFieldEditor.getForm()._record = emptydata;
        DateFieldEditor.getForm().loadRecord(emptydata);

        DateFieldEditor.getForm().clearInvalid();

        DateFieldFormat.rvConfig.remoteValidated = false;
        DateFieldFormat.rvConfig.remoteValid = false;
        delete DateFieldFormat.rvConfig.lastValue;

        DateFieldWindow.show();
    }
    else {
        Ext.Msg.show(
        {
            icon: Ext.MessageBox.ERROR, msg: 'A date field with the specified name has already been defined for this schema.', buttons: Ext.Msg.OK
        });
    }
}

function SaveDateField() {

    var record = DateFieldEditor.getForm()._record;

    DateFieldEditor.getForm().updateRecord(record);

    if (typeof record.store == 'undefined')
        DelimetedFieldsGrid.getStore().add(record);

    DelimetedFieldsGrid.getStore().commitChanges();

    DateFieldWindow.hide();

}

function ShowTimeFieldEditor() {

    if (Checkfield("Timefield", true)) {
        TimeFieldEditor.getForm().reset();
        var rawdata = {};
        rawdata.Name = '';
        rawdata.Timefield = true;
        rawdata.Timeformat = '';

        var dsStore = DelimetedFieldsGrid.getStore();
        var emptydata = new dsStore.recordType(rawdata, Ext.id());

        TimeFieldEditor.getForm()._record = emptydata;
        TimeFieldEditor.getForm().loadRecord(emptydata);

        TimeFieldEditor.getForm().clearInvalid();

        TimeFieldFormat.rvConfig.remoteValidated = false;
        TimeFieldFormat.rvConfig.remoteValid = false;
        delete TimeFieldFormat.rvConfig.lastValue;

        TimeFieldWindow.show();
    }
    else {
        Ext.Msg.show(
        {
            icon: Ext.MessageBox.ERROR, msg: 'A time field with the specified has already been defined for this schema.', buttons: Ext.Msg.OK
        });
    }
}

function SaveTimeField() {

    var record = TimeFieldEditor.getForm()._record;

    TimeFieldEditor.getForm().updateRecord(record);

    if (typeof record.store == 'undefined')
        DelimetedFieldsGrid.getStore().add(record);

    DelimetedFieldsGrid.getStore().commitChanges();

    TimeFieldWindow.hide();

}

function ShowIgnoreFieldEditor() {

    IgnoreFieldEditor.getForm().reset();
    var rawdata = {};
    rawdata.Name = '';
    rawdata.Ignorefield = true;

    var dsStore = DelimetedFieldsGrid.getStore();
    var emptydata = new dsStore.recordType(rawdata, Ext.id());

    IgnoreFieldEditor.getForm()._record = emptydata;
    IgnoreFieldEditor.getForm().loadRecord(emptydata);

    IgnoreFieldEditor.getForm().clearInvalid();

    IgnoreFieldWindow.show();
}

function SaveIgnoreField() {

    var record = IgnoreFieldEditor.getForm()._record;

    IgnoreFieldEditor.getForm().updateRecord(record);

    if (typeof record.store == 'undefined')
        DelimetedFieldsGrid.getStore().add(record);

    DelimetedFieldsGrid.getStore().commitChanges();

    IgnoreFieldWindow.hide();

}

function ShowCommentFieldEditor() {

    CommentFieldEditor.getForm().reset();
    var rawdata = {};
    rawdata.Name = '';
    rawdata.Commentfield = true;

    var dsStore = DelimetedFieldsGrid.getStore();
    var emptydata = new dsStore.recordType(rawdata, Ext.id());

    CommentFieldEditor.getForm()._record = emptydata;
    CommentFieldEditor.getForm().loadRecord(emptydata);

    CommentFieldEditor.getForm().clearInvalid();

    CommentFieldWindow.show();
}

function SaveCommentField() {

    var record = CommentFieldEditor.getForm()._record;

    CommentFieldEditor.getForm().updateRecord(record);

    if (typeof record.store == 'undefined')
        DelimetedFieldsGrid.getStore().add(record);

    DelimetedFieldsGrid.getStore().commitChanges();

    CommentFieldWindow.hide();
}

function ShowFixedTimeField() {


    ShowOfferingFieldEditor();
    FieldFixedTimeValue.allowBlank = false;
    FixedTimePanel.show();

    OfferingFieldEditor.getForm()._record.data.FixedTimeField = true;
}

function ShowOfferingFieldEditor() {

    OfferingFieldEditor.getForm().reset();
    var rawdata = {};
    rawdata.Name = '';
    rawdata.PhenomenonID = null;
    rawdata.OfferingID = null;
    rawdata.UnitofMeasure = null;
    rawdata.Offeringfield = true;
    rawdata.FixedTimeField = false;
    rawdata.FixedTime = '';

    FieldFixedTimeValue.allowBlank = true;
    FixedTimePanel.hide();

    cbOffering.getStore().removeAll();
    cbOffering.clearValue();

    cbUnitofMeasure.getStore().removeAll();
    cbUnitofMeasure.clearValue();

    var dsStore = DelimetedFieldsGrid.getStore();
    var emptydata = new dsStore.recordType(rawdata, Ext.id());
    OfferingFieldEditor.getForm()._record = emptydata;
    OfferingFieldEditor.getForm().loadRecord(emptydata);

    OfferingFieldEditor.getForm().clearInvalid();

    OfferingFieldWindow.show();
}

function SaveOfferingField() {

    var record = OfferingFieldEditor.getForm()._record;

    OfferingFieldEditor.getForm().updateRecord(record);

    if (typeof record.store == 'undefined')
        DelimetedFieldsGrid.getStore().add(record);

    DelimetedFieldsGrid.getStore().commitChanges();

    OfferingFieldWindow.hide();

}

function Checkfield(field, val) {

    if (DelimetedFieldsGrid.getStore().query(field, val).getCount() > 0)
        return false;
    else
        return true;
}

function closepreview() {

    PreviewFileUpload.reset();
    PreviewFileUpload.markAsValid();

    while (PreviewGrid.colModel.config.length > 0) {
        var dataIndex = PreviewGrid.colModel.config[0].dataIndex;
        PreviewGrid.removeColumn(0);
        PreviewGrid.getStore().removeField(dataIndex);
    }

}

var notifyDrop = function (ddSource, e, data) {
    var index = ddSource.grid.getView().findRowIndex(e.target),
                store = ddSource.grid.getStore();
    store.remove(ddSource.dragData.selections);
    index = index > store.getCount() ? store.getCount() : index;
    store.insert(index, ddSource.dragData.selections);
    ddSource.grid.view.refresh(); 
    return true;
};

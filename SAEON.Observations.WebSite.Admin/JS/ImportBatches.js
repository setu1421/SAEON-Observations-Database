function ImportBatchRowSelect(e, record) {
    if (pnlSouth.isVisible()) {
        DSLogGrid.getStore().reload();
        ObservationsGrid.getStore().reload();
    }
}

//in PrepareCommand we can modify command
function prepareCommand(grid, cmd, record, row) {


    if (cmd.command === 'InvalidDate') {
        if (record.get("DateValueInvalid") || record.get("TimeValueInvalid"))
            cmd.hidden = false;
    }
    else if (cmd.command === 'InvalidSensor') {
        if (record.get("SensorInvalid"))
            cmd.hidden = false;
    }
    else if (cmd.command === 'InvalidRawValue') {
        if (record.get("RawValueInvalid"))
            cmd.hidden = false;
    }
    else if (cmd.command === 'InvalidDataValue') {
        if (record.get("DataValueInvalid"))
            cmd.hidden = false;
    }
    else if (cmd.command === 'InvalidOffering') {
        if (record.get("OfferingInvalid"))
            cmd.hidden = false;
    }
    else if (cmd.command === 'InvalidUOM') {
        if (record.get("UOMInvalid"))
            cmd.hidden = false;
    }
}

function onLogCommand(e, record)
{
	//////////////////////
	if (e === 'Delete')
	{
		DirectCall.ConfirmDeleteEntry(record.get('Id'), { eventMask: { showMask: true} });
	}
	else if (e === 'MoveToObservation')
	{
		if (record.get("StatusID") !== "edb0a37c-f68d-4693-8ba6-d14d1b4fabe8")
		{
			Ext.Msg.alert('Error', 'You can only move items with the status "Duplicate of a previous empty value".');

		}
		else
		{
			DirectCall.ConfirmMoveToObservation(record.get('Id'), { eventMask: { showMask: true} });
		}

	}
	//////////////////////
	else
	{
		DetailsFormPanel.getForm().reset();
		DetailsFormPanel.getForm().loadRecord(record);
		DetailsFormPanel.getForm()._record = record;
		DetailsFormPanel.getForm().clearInvalid();

		if (!record.get('SensorInvalid'))
		{
			cbSensorProcedure.setReadOnly(true);
		}
		else
		{
			cbSensorProcedure.markInvalid();
			cbSensorProcedure.setReadOnly(false);
		}

		if (!record.get('DateValueInvalid'))
		{
			ValueDate.setReadOnly(true);
		}
		else
		{
			ValueDate.markInvalid();
			ValueDate.setReadOnly(false);
		}

		if (!record.get('TimeValueInvalid'))
		{
			TimeValueContainer.hide();
			TimeValue.allowBlank = true;
		}
		else
		{
			TimeValueContainer.hide();
			TimeValue.allowBlank = false;
			TimeValue.markInvalid();
			TimeValueContainer.show();
		}

		if (!record.get('RawValueInvalid'))
		{
			RawValue.setReadOnly(true);
		}
		else
		{
			RawValue.markInvalid();
			RawValue.setReadOnly(false);
		}

		if (!record.get('DataValueInvalid'))
		{
			DataValue.setReadOnly(true);
		}
		else
		{
			DataValue.markInvalid();
			DataValue.setReadOnly(false);
		}

		var loadcallback = function ()
		{
			DSLogGrid.el.mask('Please wait', 'x-mask-loading');
			cbOffering.getStore().removeListener('load', loadcallback);

			var val = record.get('PhenomenonOfferingID');
			var rd = cbOffering.findRecord(cbOffering.valueField, val);
			if (rd !== null)
			{
				cbOffering.setValue(val);
				cbOffering.setReadOnly(true);
			}
			else
			{
				cbOffering.setReadOnly(false);
			}
		}

		if (!record.get('OfferingInvalid'))
		{
			cbOffering.setReadOnly(true);
		}
		else
		{

			if (!record.get('SensorInvalid'))
			{
				cbOffering.getStore().on("load", loadcallback);
			}

			cbOffering.setReadOnly(false);
			cbOffering.markInvalid();
		}

		var uomloadcallback = function ()
		{
			cbUnitofMeasure.getStore().removeListener('load', uomloadcallback);

			var val = DetailsFormPanel.getForm()._record.get('PhenomenonUOMID')
			var rd = cbUnitofMeasure.findRecord(cbUnitofMeasure.valueField, val);
			if (rd !== null)
			{
				cbUnitofMeasure.setValue(val);
				cbUnitofMeasure.setReadOnly(true);
			}
			else
			{
				cbUnitofMeasure.setReadOnly(false);
			}
			DetailWindow.el.unmask();
		}

		if (!record.get('UOMInvalid'))
		{
			cbUnitofMeasure.setReadOnly(true);
		}
		else
		{
			if (!record.get('SensorInvalid'))
			{
				cbUnitofMeasure.getStore().on("load", uomloadcallback);
			}

			cbUnitofMeasure.setReadOnly(false);
			cbUnitofMeasure.markInvalid();
		}

		DetailWindow.show()
	}
}

function SelectSensor() { 
    cbOffering.clearValue();
    cbOffering.getStore().reload();

    cbUnitofMeasure.clearValue();
    cbUnitofMeasure.getStore().reload();


    DetailWindow.el.mask('Please wait', 'x-mask-loading');

    var loadcallback = function () {
        cbOffering.getStore().removeListener('load', loadcallback);

        var val =  DetailsFormPanel.getForm()._record.get('PhenomenonOfferingID');
        var rd = cbOffering.findRecord(cbOffering.valueField, val);
        if (rd !== null) {
            cbOffering.setValue(val);
            cbOffering.setReadOnly(true);
        }
        else {
            cbOffering.setReadOnly(false);
        }
    }

    var uomloadcallback = function () {
        cbUnitofMeasure.getStore().removeListener('load', uomloadcallback);

        var val = DetailsFormPanel.getForm()._record.get('PhenomenonUOMID')
        var rd = cbUnitofMeasure.findRecord(cbUnitofMeasure.valueField, val);
        if (rd !== null) {
            cbUnitofMeasure.setValue(val);
            cbUnitofMeasure.setReadOnly(true);
        }
        else {
            cbUnitofMeasure.setReadOnly(false);
        }
        DetailWindow.el.unmask();
    }

    cbOffering.getStore().on("load", loadcallback);

    cbUnitofMeasure.getStore().on("load", uomloadcallback);

}

function onBatchCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteBatch(record.get('Id'), { eventMask: { showMask: true}});
    }
    else if (e === 'Move')
    {
        DirectCall.ConfirmMoveBatch(record.get('Id'),{eventMask: {showMask:true}});
    }
}

function prepareToolbarCommand (grid, toolbar, rowIndex, record) 
{
    var moveButton = toolbar.items.get(0);
    var deleteButton = toolbar.items.get(1);

    if (record.data.Status === 0) {
        moveButton.setDisabled(true);
        moveButton.setTooltip("Disabled");
    }
    else {
        moveButton.setDisabled(false);
        moveButton.setTooltip("Move Batch");
    }
}

function prepareToolbarTransformation(grid, toolbar, rowIndex, record)
{
	if (record.get("StatusID") !== "edb0a37c-f68d-4693-8ba6-d14d1b4fabe8")
	{
		toolbar.items.itemAt(2).hide();
	}
	
}

//var rendererData = function (value, metadata, record, rowIndex, colIndex, store)
//{
//	//edb0a37c-f68d-4693-8ba6-d14d1b4fabe8	QA-99	Duplicate of a previous empty value	Duplicate of a previous empty value
////	if (record.get("StatusID") !== "edb0a37c-f68d-4693-8ba6-d14d1b4fabe8")
////	{
////		//metadata.style += "background-color: #FFFAC8;";
////	}
////	return value;
//}
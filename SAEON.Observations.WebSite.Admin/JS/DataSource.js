function onCommand(e, record)
{
	DetailsFormPanel.getForm().reset();
	DetailsFormPanel.getForm().loadRecord(record);
	DetailsFormPanel.getForm().clearInvalid();


	tfCode.rvConfig.remoteValidated = false;
	tfCode.rvConfig.remoteValid = false;

	tfCode.markAsValid();
	tfName.markAsValid();
	DetailWindow.show();
}

function New()
{

	DetailsFormPanel.getForm().reset();
	tfCode.rvConfig.remoteValidated = false;
	tfCode.rvConfig.remoteValid = false;
	DetailWindow.show();

}

function DataSourceRowSelect(e, record)
{
	if (pnlSouth.isVisible())
		DSTransformGrid.getStore().reload();


	if (pnleast.isVisible())
		DataSourceRoleGrid.getStore().reload();
}

function handlechange(e)
{
	tfDefinition.rvConfig.remoteValidated = false;
	tfDefinition.rvConfig.remoteValid = false;
	delete tfDefinition.rvConfig.lastValue;
	tfDefinition.performRemoteValidation();
}

function NewTransform()
{

	if (DataSourceGrid.getSelectionModel().hasSelection())
	{
		TransformationDetailPanel.getForm().reset();

		tfDefinition.rvConfig.remoteValidated = false;
		tfDefinition.rvConfig.remoteValid = false;
		delete tfDefinition.rvConfig.lastValue;

		TransformationDetailWindow.show();
	}
	else
	{
	    Ext.Msg.alert('Invalid Selection', 'Select a Data Source.');
	}
}

function onTransformCommand(e, record)
{
	TransformationDetailPanel.getForm().reset();
	TransformationDetailPanel.getForm().loadRecord(record);
	TransformationDetailPanel.getForm().clearInvalid();

	DSTransformGrid.el.mask('Please wait', 'x-mask-loading');

	var loadcallback = function () {
	    cbOffering.getStore().removeListener('load', loadcallback);
	    cbOffering.setValue(record.data.PhenomenonOfferingId);
	};

	var uomloadcallback = function () {
	    cbUnitofMeasure.getStore().removeListener('load', uomloadcallback);
	    cbUnitofMeasure.setValue(record.data.UnitOfMeasureId);
	    //		DSTransformGrid.el.unmask();
	    //		TransformationDetailWindow.show()
	};

	//
	var newOloadcallback = function () {
	    sbNewOffering.getStore().removeListener('load', newOloadcallback);
	    sbNewOffering.setValue(record.data.NewPhenomenonOfferingID);
	};

	var newUOMloadcallback = function () {
	    sbNewUoM.getStore().removeListener('load', newUOMloadcallback);
	    sbNewUoM.setValue(record.data.NewPhenomenonUOMID);
	    DSTransformGrid.el.unmask();
	    TransformationDetailWindow.show();
	};
	//

	tfDefinition.rvConfig.remoteValidated = false;
	tfDefinition.rvConfig.remoteValid = false;
	tfDefinition.markAsValid();
	delete tfDefinition.rvConfig.lastValue;

	cbOffering.getStore().on("load", loadcallback);
	cbUnitofMeasure.getStore().on("load", uomloadcallback);

	//
	sbNewOffering.getStore().on("load", newOloadcallback);
	sbNewUoM.getStore().on("load", newUOMloadcallback);
	//

	cbPhenomenon.setValueAndFireSelect(record.data.PhenomenonID);



}

function CloseAvailableRole()
{
	RoleGrid.selModel.clearSelections();
}

function FrequencyUpdate()
{

	if (cbUpdateFrequency.getValue() === '0')
	{
		tfUrl.allowBlank = true;
		tfUrl.markAsValid();


		StartDate.allowBlank = true;
		StartDate.markAsValid();
	}
	else
	{
		tfUrl.allowBlank = false;
		StartDate.allowBlank = false;
	}
}

function onRoleCommand(e, record)
{
	if (e === 'Delete')
	{
		DirectCall.ConfirmDeleteRole(record.get('Id'), { eventMask: { showMask: true} });
	}
	if (e === 'Edit')
	{
		RoleDetailFormPanel.getForm().reset();
		RoleDetailFormPanel.getForm().loadRecord(record);
		RoleDetailFormPanel.getForm().clearInvalid();
		//$('#hiddenRoleDetail').val(record.id);

		RoleDetailWindow.show();
	}

}
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

    tfCode.rvConfig.remoteValidated = false;
    tfCode.rvConfig.remoteValid = false;
    tfCode.markAsValid();

    tfName.rvConfig.remoteValidated = false;
    tfName.rvConfig.remoteValid = false;
    tfName.markAsValid();

    DetailWindow.show();

}

function DataSourceRowSelect(e, record) {
    if (tpCenter.isVisible()) {
        //InstrumentLinksGrid.getStore().reload();
        //RolesGrid.getStore().reload();
        TransformationsGrid.getStore().reload();
    }
}

function handlechange(e) {
    tfDefinition.rvConfig.remoteValidated = false;
    tfDefinition.rvConfig.remoteValid = false;
    delete tfDefinition.rvConfig.lastValue;
    tfDefinition.performRemoteValidation();
}

function NewTransform() {

    if (DataSourcesGrid.getSelectionModel().hasSelection()) {
        TransformationDetailPanel.getForm().reset();
        TransformationDetailPanel.getForm().clearInvalid();

        tfDefinition.rvConfig.remoteValidated = false;
        tfDefinition.rvConfig.remoteValid = false;
        delete tfDefinition.rvConfig.lastValue;
        tfDefinition.markAsValid();

        DirectCall.SetFields();
        TransformationDetailWindow.show();
    }
    else {
        Ext.Msg.alert('Invalid Selection', 'Select a Data Source.');
    }
}

function onTransformCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteTransformation(record.get('Id'), { eventMask: { showMask: true } });
    }
    else if (e === "Edit") {
        TransformationDetailPanel.getForm().reset();
        TransformationDetailPanel.getForm().loadRecord(record);
        DirectCall.LoadCombos(record.get("TransformationTypeID"), record.get("PhenomenonID"), record.get("PhenomenonOfferingID"), record.get("PhenomenonUOMID"), record.get("NewPhenomenonID"), record.get("NewPhenomenonOfferingID"), record.get("NewPhenomenonUOMID"));

        tfDefinition.rvConfig.remoteValidated = false;
        tfDefinition.rvConfig.remoteValid = false;
        tfDefinition.markAsValid();
        delete tfDefinition.rvConfig.lastValue;

        DirectCall.SetFields();
        TransformationDetailWindow.show();
        TransformationDetailPanel.getForm().clearInvalid();
        TransformationDetailPanel.validate();
    }
}

function CloseAvailableRole() {
    RolesGrid.selModel.clearSelections();
}

function FrequencyUpdate() {

    if (cbUpdateFrequency.getValue() === '0') {
        tfUrl.allowBlank = true;
        tfUrl.markAsValid();


        StartDate.allowBlank = true;
        StartDate.markAsValid();
    }
    else {
        tfUrl.allowBlank = false;
        StartDate.allowBlank = false;
    }
}

function onRoleCommand(e, record) {
    if (e === 'Delete') {
        DirectCall.ConfirmDeleteRole(record.get('Id'), { eventMask: { showMask: true } });
    }
    if (e === 'Edit') {
        RoleDetailFormPanel.getForm().reset();
        RoleDetailFormPanel.getForm().loadRecord(record);
        RoleDetailFormPanel.getForm().clearInvalid();
        //$('#hiddenRoleDetail').val(record.id);

        RoleDetailWindow.show();
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

function CloseAvailableRoles() {
    AvailableStationsGrid.selModel.clearSelections();
}


<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataSchema.aspx.cs" Inherits="_DataSchema"
    MasterPageFile="~/Site.master" ValidateRequest="false" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/DataSchema.js"></script>
	<script type="text/javascript" src="../JS/generic.js"></script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <script type="text/javascript">
        Ext.apply(Ext.form.VTypes, {
            // Password Check
            duplicateNameText: 'The Field Name already exists.',
            duplicateName: function (v, d, e) {

                if (typeof (d.ownerCt.ownerCt.getForm()._record) != 'undefined' && d.ownerCt.ownerCt.getForm()._record.data.Name == v.trim())
                    return true;

                return DelimetedFieldsGrid.getStore().queryBy(function (record) {
                    if (v != '')
                        return record.get('Name') == v.trim();
                }).getCount() == 0;
            }
        });

        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_DataSchemaGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(ContentPlaceHolder1_DataSchemaGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);

            ContentPlaceHolder1_DataSchemaGrid.submitData(false);
        };
	
    </script>
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Title="Data Schemas" Layout="Fit">
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Icon="Add" Text="Add Schema">
                                <ToolTips>
                                    <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                </ToolTips>
                                <Listeners>
                                    <Click Fn="New" />
                                </Listeners>
                            </ext:Button>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="Button10" runat="server" Text="To Excel" Icon="PageExcel">
                                <Listeners>
                                    <Click Handler="submitValue('exc');" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button11" runat="server" Text="To CSV" Icon="PageAttach">
                                <Listeners>
                                    <Click Handler="submitValue('csv');" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:GridPanel ID="DataSchemaGrid" runat="server" Border="false">
                        <Store>
                            <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="DataSchemaStore_RefreshData"
                                OnSubmitData="DataSchemaStore_Submit">
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>
                                <Reader>
                                    <ext:JsonReader IDProperty="Id">
                                        <Fields>
                                            <ext:RecordField Name="Id" Type="Auto" />
                                            <ext:RecordField Name="Code" Type="String" />
                                            <ext:RecordField Name="Name" Type="String" />
                                            <ext:RecordField Name="IgnoreFirst" Type="Int" />
                                            <ext:RecordField Name="IgnoreLast" Type="Int" />
                                            <ext:RecordField Name="Condition" Type="String" />
                                            <ext:RecordField Name="Delimiter" Type="String" />
                                            <ext:RecordField Name="Description" Type="String" />
                                            <ext:RecordField Name="DataSourceTypeID" Type="String" />
                                            <ext:RecordField Name="DataSourceTypeCode" Type="String" />
                                            <ext:RecordField Name="DataSourceTypeDesc" Type="String" />
                                            <ext:RecordField Name="SplitSelector" Type="String" />
                                            <ext:RecordField Name="SplitIndex" Type="Int" UseNull="true" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                    <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="" />
                                    <ext:Parameter Name="dir" Value="" />
                                </BaseParams>
                                <SortInfo Field="Name" Direction="ASC" />
                                <DirectEventConfig IsUpload="true" />
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                <ext:Column Header="Name" DataIndex="Name" Width="200" />
                                <ext:Column Header="Description" DataIndex="Description" Width="200" />
                                <ext:Column Header="Source Type Code" Width="200" DataIndex="DataSourceTypeCode" />
                                <ext:Column Header="Source Type Description" Width="200" DataIndex="DataSourceTypeDesc" />
                                <ext:CommandColumn Width="100">
                                    <Commands>
                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                        <ext:GridCommand Icon="NoteGo" CommandName="Fields" Text="" ToolTip-Text="Edit" />
                                        <ext:GridCommand Icon="Zoom" CommandName="Preview" Text="" ToolTip-Text="Edit" />
                                    </Commands>
                                </ext:CommandColumn>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <LoadMask ShowMask="true" />
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1">
                                <Filters>
                                    <ext:StringFilter DataIndex="ID" />
                                    <ext:StringFilter DataIndex="Code" />
                                    <ext:StringFilter DataIndex="Name" />
                                    <ext:StringFilter DataIndex="Description" />
                                    <ext:StringFilter DataIndex="DataSourceTypeCode" />
                                    <ext:StringFilter DataIndex="DataSourceTypeDesc" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                        </BottomBar>
                        <Listeners>
                            <Command Fn="onCommand" />
                        </Listeners>
                        <DirectEvents>
                            <Command OnEvent="onCommand">
                                <ExtraParams>
                                    <ext:Parameter Name="type" Value="params[0]" Mode="Raw" />
                                    <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                </ExtraParams>
                                <EventMask ShowMask="true" />
                            </Command>
                        </DirectEvents>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="640" Height="470" Closable="true"
        Hidden="true" Collapsible="false" Title="Data Schema Detail" Maximizable="false"
        Layout="Fit" AutoScroll="true" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Container ID="Container1" runat="server" Layout="Column" Height="100">
                        <Items>
                            <ext:Container ID="Container2" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                        runat="server" FieldLabel="Code" AnchorHorizontal="93%" AllowBlank="false" BlankText="Code is a required"
                                        MsgTarget="Side" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbDataSourceType" runat="server" Editable="false" BlankText="Source Type is required"
                                        MsgTarget="Side" DisplayField="Description" ValueField="Id" TypeAhead="true"
                                        Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false" DataIndex="DataSourceTypeID"
                                        EmptyText="Select Source Type" SelectOnFocus="true" FieldLabel="Source Type"
                                        AnchorHorizontal="93%">
                                        <Store>
                                            <ext:Store ID="DataSourceTypeStore" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Description" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{cbDataSourceType}.getValue() == '25839703-3cb3-4c23-aca3-4399cc52ecde'?#{cbDelimiter}.allowBlank=false:#{cbDelimiter}.allowBlank=true;#{cbDelimiter}.clearValue();#{cbDelimiter}.clearInvalid();#{cbDelimiter}.markAsValid();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container3" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                        runat="server" FieldLabel="Name" AnchorHorizontal="93%" AllowBlank="false" BlankText="Name is a required"
                                        MsgTarget="Side" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbDelimiter" runat="server" Editable="false" MsgTarget="Side" TypeAhead="true"
                                        Mode="Local" ForceSelection="true" TriggerAction="All" DataIndex="Delimiter"
                                        EmptyText="Select Delimiter" SelectOnFocus="true" BlankText="Delimiter is required for this Source Type"
                                        FieldLabel="Delimiter" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <Items>
                                            <ext:ListItem Text="Comma Delimited (,)" Value="," />
                                            <ext:ListItem Text="Pipe Delimited (|)" Value="|" />
                                            <ext:ListItem Text="Tab Delimited (\t)" Value="\t" />
                                            <ext:ListItem Text="SemiColon Delimited (;)" Value=";" />
                                        </Items>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="this.clearValue();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" Layout="Column" Height="50">
                        <Items>
                            <ext:Container ID="Container5" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:NumberField AllowDecimals="false" ID="nfIgnoreFirst" DataIndex="IgnoreFirst"
                                        MaxLength="10" runat="server" FieldLabel="Ignore first # of lines on import"
                                        AnchorHorizontal="93%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container6" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:NumberField AllowDecimals="false" ID="nfIgnoreLast" DataIndex="IgnoreLast" MaxLength="10"
                                        runat="server" FieldLabel="Ignore last # of lines on import" AnchorHorizontal="95%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container10" runat="server" Layout="Column" Height="100">
                        <Items>
                            <ext:Container ID="Container11" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextArea ID="tfCondition" DataIndex="Condition" MaxLength="150" runat="server"
                                        FieldLabel="Record Condition - Ignore fields that start with" AnchorHorizontal="93%"
                                        MsgTarget="Side">
                                    </ext:TextArea>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container12" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="150" runat="server"
                                        FieldLabel="Description" AnchorHorizontal="93%" AllowBlank="false" BlankText="Name is a required"
                                        MsgTarget="Side">
                                    </ext:TextArea>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container7" runat="server" Layout="Column" Height="100">
                        <Items>
                            <ext:Container ID="Container8" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfSplit" DataIndex="SplitSelector" MaxLength="150" runat="server"
                                        FieldLabel="File Split Condition - Data Split" AnchorHorizontal="93%" MsgTarget="Side">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container9" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:NumberField ID="nfSplitIndex" DataIndex="SplitIndex" MaxLength="150" runat="server"
                                        FieldLabel="Data file Split Index" AnchorHorizontal="93%" AllowDecimals="true"
                                        BlankText="Split index is required for a split data file" MsgTarget="Side">
                                        <Listeners>
                                            <Valid Handler="if(#{tfSplit}.getValue() == '') this.allowBlank = true;else this.allowBlank = false;" />
                                        </Listeners>
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSave" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="Save" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar1" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="DefinitionWindow" runat="server" Title="Schema Fields" Closable="true"
        Width="700" Height="450" Border="false" Collapsible="false" Layout="Fit" Hidden="true"
        ClientIDMode="Static">
        <Listeners>
            <Hide Handler="#{DelimetedFieldsGrid}.getStore().removeAll();" />
        </Listeners>
        <TopBar>
            <ext:Toolbar ID="Toolbar2" runat="server">
                <Items>
                    <ext:Button ID="Button2" runat="server" Icon="Add" Text="Add Field">
                        <ToolTips>
                            <ext:ToolTip ID="ToolTip2" runat="server" Html="Add Field" />
                        </ToolTips>
                        <Menu>
                            <ext:Menu ID="Menu1" runat="server">
                                <Items>
                                    <ext:MenuItem ID="MenuItem1" runat="server" Icon="Add" Text="Add Date Field">
                                        <Listeners>
                                            <Click Fn="ShowDateFieldEditor" />
                                        </Listeners>
                                    </ext:MenuItem>
                                    <ext:MenuItem ID="MenuItem2" runat="server" Icon="Add" Text="Add Time Field">
                                        <Listeners>
                                            <Click Fn="ShowTimeFieldEditor" />
                                        </Listeners>
                                    </ext:MenuItem>
                                    <ext:MenuItem ID="MenuItem3" runat="server" Icon="Add" Text="Add Ignore Field">
                                        <Listeners>
                                            <Click Fn="ShowIgnoreFieldEditor" />
                                        </Listeners>
                                    </ext:MenuItem>
                                    <ext:MenuItem ID="MenuItem4" runat="server" Icon="Add" Text="Add Offering Field">
                                        <Listeners>
                                            <Click Fn="ShowOfferingFieldEditor" />
                                        </Listeners>
                                    </ext:MenuItem>
                                    <ext:MenuItem ID="MenuItem5" runat="server" Icon="Add" Text="Add Comment Field">
                                        <Listeners>
                                            <Click Fn="ShowCommentFieldEditor" />
                                        </Listeners>
                                    </ext:MenuItem>
                                </Items>
                            </ext:Menu>
                        </Menu>
                    </ext:Button>
                    <ext:Button ID="Button9" runat="server" Icon="Add" Text="Add Fixed Field">
                        <ToolTips>
                            <ext:ToolTip ID="ToolTip3" runat="server" Html="Add Fixed Field" />
                        </ToolTips>
                        <Menu>
                            <ext:Menu ID="Menu2" runat="server">
                                <Items>
                                    <ext:MenuItem ID="MenuItem12" runat="server" Icon="Add" Text="Add Offering Field with Fixed Times">
                                        <Listeners>
                                            <Click Fn="ShowFixedTimeField" />
                                        </Listeners>
                                    </ext:MenuItem>
                                </Items>
                            </ext:Menu>
                        </Menu>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:GridPanel ID="DelimetedFieldsGrid" runat="server" StripeRows="true" Header="false"
                Layout="FitLayout" Border="false" EnableColumnHide="false" EnableColumnMove="false"
                EnableHdMenu="true" ClientIDMode="Static" EnableDragDrop="true" DDGroup="firstGridDDGroup">
                <LoadMask ShowMask="true" />
                <SaveMask ShowMask="true" />
                <SelectionModel>
                    <ext:RowSelectionModel ID="SelectedRowModel1" runat="server" SingleSelect="true" />
                </SelectionModel>
                <ColumnModel ID="ColumnModel2" runat="server">
                    <Columns>
                        <ext:Column Header="Name" Width="100" DataIndex="Name" Sortable="false" Hideable="false"
                            MenuDisabled="true" />
                        <ext:Column Header="Field Length" Width="80" DataIndex="FieldLength" Sortable="false"
                            Hideable="false" MenuDisabled="true" />
                        <ext:CheckColumn Header="Date Field" Width="80" DataIndex="Datefield" Sortable="false"
                            Hideable="false" MenuDisabled="true" Locked="true" />
                        <ext:CheckColumn Header="Time Field" Width="80" DataIndex="Timefield" Sortable="false"
                            Hideable="false" MenuDisabled="true" Locked="true" />
                        <ext:CheckColumn Header="Ignore Field" Width="80" DataIndex="Ignorefield" Sortable="false"
                            Hideable="false" MenuDisabled="true" Locked="true" />
                        <ext:CheckColumn Header="Offering Field" Width="80" DataIndex="Offeringfield" Sortable="false"
                            Hideable="false" MenuDisabled="true" Locked="true" />
                        <ext:CheckColumn Header="Fixed Field" Width="80" DataIndex="FixedTimeField" Sortable="false"
                            Hideable="false" MenuDisabled="true" Locked="true" />
                        <ext:CommandColumn Width="50">
                            <Commands>
                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <Store>
                    <ext:Store ID="FieldsStore" runat="server" OnSubmitData="SubmitFieldsData">
                        <Reader>
                            <ext:JsonReader IDProperty="id">
                                <Fields>
                                    <ext:RecordField Name="id" Type="String" />
                                    <ext:RecordField Name="Name" Type="String" />
                                    <ext:RecordField Name="Datefield" Type="Boolean" />
                                    <ext:RecordField Name="Dateformat" Type="String" />
                                    <ext:RecordField Name="Timefield" Type="Boolean" />
                                    <ext:RecordField Name="Timeformat" Type="string" />
                                    <ext:RecordField Name="Ignorefield" Type="Boolean" />
                                    <ext:RecordField Name="PhenomenonID" Type="Auto" />
                                    <ext:RecordField Name="OfferingID" Type="Auto" />
                                    <ext:RecordField Name="EmptyValue" Type="String" />
                                    <ext:RecordField Name="UnitofMeasureID" Type="Auto" />
                                    <ext:RecordField Name="FieldLength" Type="Int" />
                                    <ext:RecordField Name="Offeringfield" Type="Boolean" />
                                    <ext:RecordField Name="FixedTimeField" Type="Boolean" />
                                    <ext:RecordField Name="FixedTime" Type="String" />
                                    <ext:RecordField Name="Commentfield" Type="Boolean" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <Listeners>
                    <Command Fn="onFieldsCommand" />
                </Listeners>
            </ext:GridPanel>
            <ext:Hidden ID="hfSchemaID" runat="server" ClientIDMode="Static">
            </ext:Hidden>
        </Items>
        <Buttons>
            <ext:Button ID="Button4" runat="server" Text="Save" FormBind="true">
                <Listeners>
                    <Click Handler="#{DelimetedFieldsGrid}.submitData();" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
    <ext:Window ID="DateFieldWindow" runat="server" Title="Date field Detail" Closable="true"
        Width="400" Height="300" Border="false" Collapsible="false" Layout="Fit" Hidden="true"
        ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DateFieldEditor" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="DateFieldName" DataIndex="Name" MaxLength="50" AllowBlank="false"
                                BlankText="Field Name is Required" Vtype="duplicateName" MsgTarget="Side" runat="server"
                                FieldLabel="Name" AnchorHorizontal="95%" ClientIDMode="Static" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="Name" Mode="Value" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="DateFieldFormat" DataIndex="Dateformat" MaxLength="50" AllowBlank="false"
                                BlankText="Date Format is Required" MsgTarget="Side" runat="server" FieldLabel="Date Format"
                                IsRemoteValidation="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="DateFieldLengthPanel" runat="server" Border="false" Header="false"
                        Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="DateFieldLength" DataIndex="FieldLength"
                                MaxLength="3" runat="server" FieldLabel="Field Length" AnchorHorizontal="95%"
                                BlankText="Field Length is required." MsgTarget="Side" AllowBlank="false">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="Button3" runat="server" Text="Save" FormBind="true" Icon="Accept">
                        <Listeners>
                            <Click Fn="SaveDateField" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar2" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="TimeFieldWindow" runat="server" Title="Time field Detail" Closable="true"
        Width="400" Height="300" Border="false" Collapsible="false" Layout="Fit" Hidden="true"
        ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="TimeFieldEditor" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="TimeFieldName" DataIndex="Name" MaxLength="50" AllowBlank="false"
                                BlankText="Field Name is Required" MsgTarget="Side" runat="server" FieldLabel="Name"
                                AnchorHorizontal="95%" Vtype="duplicateName" ClientIDMode="Static" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="Name" Mode="Value" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="TimeFieldFormat" DataIndex="Timeformat" MaxLength="50" AllowBlank="false"
                                BlankText="Time Format is Required" MsgTarget="Side" runat="server" FieldLabel="Time Format"
                                IsRemoteValidation="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="TimeFieldLengthPanel" runat="server" Border="false" Header="false"
                        Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="TimeFieldLength" DataIndex="FieldLength"
                                MaxLength="3" runat="server" FieldLabel="Field Length" AnchorHorizontal="95%"
                                BlankText="Field Length is required." MsgTarget="Side" AllowBlank="false">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="Button5" runat="server" Text="Save" FormBind="true" Icon="Accept">
                        <Listeners>
                            <Click Fn="SaveTimeField" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar3" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="IgnoreFieldWindow" runat="server" Title="Ignore Field Detail" Closable="true"
        Width="400" Height="300" Border="false" Collapsible="false" Layout="Fit" Hidden="true"
        ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="IgnoreFieldEditor" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="IgnoreFieldName" DataIndex="Name" MaxLength="50" AllowBlank="false"
                                BlankText="Field Name is Required" MsgTarget="Side" runat="server" FieldLabel="Name"
                                AnchorHorizontal="95%" Vtype="duplicateName" ClientIDMode="Static" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="Name" Mode="Value" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="IgnoreFieldLengthPanel" runat="server" Border="false" Header="false"
                        Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="IgnoreFieldLength" DataIndex="FieldLength"
                                MaxLength="3" runat="server" FieldLabel="Field Length" AnchorHorizontal="95%"
                                BlankText="Field Length is required." MsgTarget="Side" AllowBlank="false">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="Button6" runat="server" Text="Save" FormBind="true" Icon="Accept">
                        <Listeners>
                            <Click Fn="SaveIgnoreField" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar4" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="OfferingFieldWindow" runat="server" Title="Offering Field Detail"
        Closable="true" Width="800" Height="500" Border="false" Collapsible="false" Layout="Fit"
        Hidden="true" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="OfferingFieldEditor" runat="server" Title="" MonitorPoll="500"
                MonitorValid="true" MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout"
                ClientIDMode="Static">
                <Items>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="OfferingFieldName" DataIndex="Name" MaxLength="50" AllowBlank="false"
                                BlankText="Field Name is Required" MsgTarget="Side" runat="server" FieldLabel="Name"
                                AnchorHorizontal="95%" Vtype="duplicateName" ClientIDMode="Static" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="Name" Mode="Value" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbPhenomenon" runat="server" DataIndex="PhenomenonID" DisplayField="Name"
                                AllowBlank="false" BlankText="Phenomenon is required" ForceSelection="true" MsgTarget="Side"
                                ValueField="Id" FieldLabel="Phenomenon" EmptyText="Please select" AnchorHorizontal="95%"
                                ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="storePhenomenon" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" />
                                                    <ext:RecordField Name="Name" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <Select Handler="#{cbOffering}.clearValue();#{cbOffering}.getStore().reload();#{cbUnitofMeasure}.clearValue();#{cbUnitofMeasure}.getStore().reload()" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbOffering" runat="server" DataIndex="OfferingID" DisplayField="Name"
                                AllowBlank="false" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store1" runat="server" AutoLoad="false" OnRefreshData="cbOffering_RefreshData">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Name" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel11" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbUnitofMeasure" runat="server" DataIndex="UnitofMeasureID" DisplayField="Unit"
                                AllowBlank="false" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store3" runat="server" AutoLoad="false" OnRefreshData="cbUnitofMeasure_RefreshData">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Unit" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="EmptyValue" DataIndex="EmptyValue" MaxLength="50" runat="server"
                                FieldLabel="Empty Value" AnchorHorizontal="95%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="FixedTimePanel" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top" ClientIDMode="Static" HideMode="Offsets" MonitorResize="true">
                        <Items>
                            <ext:TimeField ID="FieldFixedTimeValue" runat="server" DataIndex="FixedTime" FieldLabel="Fixed Time Value"
                                EmptyText="Please select" AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="true"
                                BlankText="Fixed Time is required" ClientIDMode="Static" Format="H:mm" Increment="60">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:TimeField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="OfferingFieldLengthPanel" runat="server" Border="false" Header="false"
                        Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="OfferingFieldLength" DataIndex="FieldLength"
                                MaxLength="3" runat="server" FieldLabel="Field Length" AnchorHorizontal="95%"
                                AllowBlank="false" BlankText="Field Length is required." MsgTarget="Side">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="Button7" runat="server" Text="Save" FormBind="true" Icon="Accept">
                        <Listeners>
                            <Click Fn="SaveOfferingField" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar5" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="CommentFieldWindow" runat="server" Title="Comment Field Detail" Closable="true"
        Width="400" Height="300" Border="false" Collapsible="false" Layout="Fit" Hidden="true"
        ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="CommentFieldEditor" runat="server" Title="" MonitorPoll="500"
                MonitorValid="true" MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout"
                ClientIDMode="Static">
                <Items>
                    <ext:Panel ID="Panel12" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="CommentFieldName" DataIndex="Name" MaxLength="50" AllowBlank="false"
                                BlankText="Field Name is Required" MsgTarget="Side" runat="server" FieldLabel="Name"
                                AnchorHorizontal="95%" Vtype="duplicateName" ClientIDMode="Static" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="Name" Mode="Value" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="CommentFieldLengthPanel" runat="server" Border="false" Header="false"
                        Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="CommentFieldLength" DataIndex="FieldLength"
                                MaxLength="3" runat="server" FieldLabel="Field Length" AnchorHorizontal="95%"
                                BlankText="Field Length is required." MsgTarget="Side" AllowBlank="false">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="Button12" runat="server" Text="Save" FormBind="true" Icon="Accept">
                        <Listeners>
                            <Click Fn="SaveCommentField" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar6" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="PreviewWindow" runat="server" Title="Schema Preview" Closable="true"
        Width="800" Height="500" Border="false" Collapsible="false" Maximizable="true"
        Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="closepreview" />
        </Listeners>
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <North MarginsSummary="5 5 5 5">
                    <ext:FormPanel ID="BasicForm" runat="server" Height="100" Frame="true" Title="File Upload"
                        MonitorValid="true" PaddingSummary="10px 10px 0 10px" LabelWidth="90" ClientIDMode="Static">
                        <Defaults>
                            <ext:Parameter Name="anchor" Value="95%" Mode="Value" />
                            <ext:Parameter Name="allowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
                        </Defaults>
                        <Items>
                            <ext:Hidden ID="PreviewSchemaID" runat="server">
                            </ext:Hidden>
                            <ext:FileUploadField ID="PreviewFileUpload" runat="server" EmptyText="Select a File"
                                FieldLabel="Data File" ButtonText="" Icon="Zoom" BlankText="input file is required"
                                ClientIDMode="Static" />
                        </Items>
                        <Listeners>
                            <ClientValidation Handler="#{SaveButton}.setDisabled(!valid);" />
                        </Listeners>
                        <Buttons>
                            <ext:Button ID="SaveButton" runat="server" Text="Load And Test" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="UploadClick" Before="if (!#{BasicForm}.getForm().isValid()) { return false; } 
                                                    Ext.Msg.wait('Uploading your file...', 'Uploading');" Failure="Ext.Msg.show({ 
                                                    title   : 'Error', 
                                                    msg     : 'Error during uploading', 
                                                    minWidth: 200, 
                                                    modal   : true, 
                                                    icon    : Ext.Msg.ERROR, 
                                                    buttons : Ext.Msg.OK 
                                                });">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="Button8" runat="server" Text="Reset">
                                <Listeners>
                                    <Click Handler="#{PreviewFileUpload}.reset();#{PreviewFileUpload}.markAsValid();" />
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>
                </North>
                <Center MarginsSummary="0 5 0 5">
                    <ext:GridPanel ID="PreviewGrid" runat="server" Title="Test Results" Layout="FitLayout"
                        ClientIDMode="Static">
                        <ColumnModel runat="server" ID="ColumnModel21">
                        </ColumnModel>
                        <Store>
                            <ext:Store ID="Store22" runat="server">
                            </ext:Store>
                        </Store>
                    </ext:GridPanel>
                </Center>
                <South MarginsSummary="0 0 5 5" Split="true">
                    <ext:GridPanel ID="ErrorGrid" runat="server" Title="Errors" Layout="FitLayout" ClientIDMode="Static"
                        Height="100" EnableHdMenu="false">
                        <ColumnModel runat="server" ID="ColumnModel3">
                        </ColumnModel>
                        <Store>
                            <ext:Store ID="ErrorGridStore" runat="server">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="ErrorMessage" Type="String">
                                            </ext:RecordField>
                                            <ext:RecordField Name="LineNo" Type="Int">
                                            </ext:RecordField>
                                            <ext:RecordField Name="RecordString" Type="String">
                                            </ext:RecordField>
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:Column Header="Error Message" DataIndex="ErrorMessage" Width="400" />
                                <ext:Column Header="Line No" DataIndex="LineNo" Width="50" />
                                <ext:Column Header="Raw Data" DataIndex="RecordString" Width="200" />
                            </Columns>
                        </ColumnModel>
                    </ext:GridPanel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Window>
    <ext:DropTarget ID="DropTarget1" runat="server" Target="={DelimetedFieldsGrid.view.scroller.dom}"
        Group="firstGridDDGroup">
        <NotifyDrop Fn="notifyDrop" />
    </ext:DropTarget>
</asp:Content>

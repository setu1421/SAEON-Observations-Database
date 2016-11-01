<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataSource.aspx.cs" Inherits="_DataSource"   MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../../JS/DataSource.js"></script>
    <script type="text/javascript" src="../../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format)
        {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(DataSourceGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(DataSourceGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);

            DataSourceGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Store ID="DataSchemaStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="String" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="TransformationTypeStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="String" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="PhenomenonStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="String" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="5 0 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Data Sources" Layout="FitLayout" Hidden="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="Add" Text="Add Data Source">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Fn="New" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:Button ID="Button2" runat="server" Text="To Excel" Icon="PageExcel">
                                        <Listeners>
                                            <Click Handler="submitValue('exc');" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button3" runat="server" Text="To CSV" Icon="PageAttach">
                                        <Listeners>
                                            <Click Handler="submitValue('csv');" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="DataSourceGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="DataSourceStore_RefreshData"
                                        OnSubmitData="DataSourceStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                    <ext:RecordField Name="Description" Type="String" />
                                                    <ext:RecordField Name="Url" Type="String" />
                                                    <ext:RecordField Name="DefaultNullValue" Type="Float" />
                                                    <ext:RecordField Name="UpdateFreq" Type="Float" />
                                                    <ext:RecordField Name="StartDate" Type="Date" />
                                                    <ext:RecordField Name="LastUpdate" Type="Date" />
                                                    <ext:RecordField Name="DataSchemaID" Type="Auto" />
                                                    <ext:RecordField Name="DataSchemaName" Type="String" />
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
                                        <ext:Column Header="Code" DataIndex="Code" Width="100" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="100" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="150" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="200" />
                                        <ext:Column Header="Data Schema" DataIndex="DataSchemaName" Width="100" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="DataSourceRowSelect" Buffer="250" />
                                        </Listeners>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1">
                                        <Filters>
                                            <ext:StringFilter DataIndex="ID" />
                                            <ext:StringFilter DataIndex="Code" />
                                            <ext:StringFilter DataIndex="Name" />
                                            <ext:StringFilter DataIndex="Url" />
                                            <ext:StringFilter DataIndex="Description" />
                                            <ext:DateFilter DataIndex="LastUpdate" />
                                            <ext:StringFilter DataIndex="StartDate" />
                                            <ext:StringFilter DataIndex="DataSchemaName" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <Command Fn="onCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <East Collapsible="true" Split="true" MarginsSummary="5 5 0 0">
                    <ext:Panel ID="pnleast" runat="server" Title="Roles" Layout="Fit" Width="425" ClientIDMode="Static">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar3" runat="server">
                                <Items>
                                    <ext:Button ID="AddRole" runat="server" Icon="Add" Text="Add Role">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip3" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Handler="if(Ext.getCmp('#{DataSourceGrid}') && #{DataSourceGrid}.getSelectionModel().hasSelection()){#{RoleGridStore}.reload();#{AvailableRoleWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a Data Source.')}" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="DataSourceRoleGrid" runat="server" Border="false" Layout="FitLayout"
                                ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store1" runat="server" OnRefreshData="DataSourceRoleGrid_RefreshData">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="RoleName" Type="String" />
                                                    <ext:RecordField Name="Description" Type="String" />
                                                    <ext:RecordField Name="DateStart" Type="Date" />
                                                    <ext:RecordField Name="DateEnd" Type="Date" />
                                                    <ext:RecordField Name="IsRoleReadOnly" Type="Boolean" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="DataSourceID" Value="Ext.getCmp('#{DataSourceGrid}') && #{DataSourceGrid}.getSelectionModel().hasSelection() ? #{DataSourceGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel5" runat="server">
                                    <Columns>
                                        <ext:Column Header="Role Name" DataIndex="RoleName" Width="100" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="80" />
                                        <ext:DateColumn Header="Date Start" DataIndex="DateStart" Width="75" Format="yyyy/MM/dd" />
                                        <ext:DateColumn Header="Date End" DataIndex="DateEnd" Width="75" Format="yyyy/MM/dd" />
                                        <ext:CheckColumn Header="Read Only" DataIndex="IsRoleReadOnly" Width="40" Tooltip="Read Only" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit">
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Delete" CommandName="Delete" Text="" ToolTip-Text="Remove">
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Listeners>
                                    <Command Fn="onRoleCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </East>
                <South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
                    <ext:Panel ID="pnlSouth" runat="server" Title="Add Transformations" Layout="FitLayout"
                        Height="200" ClientIDMode="Static">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:Button ID="btnAddTransformation" runat="server" Icon="Add" Text="Add Transformation">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip2" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Fn="NewTransform" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="DSTransformGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store4" runat="server" OnRefreshData="DSTransformGrid_RefreshData">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="TransformationTypeID" Type="Auto" />
                                                    <ext:RecordField Name="PhenomenonID" Type="Auto" />
                                                    <ext:RecordField Name="PhenomenonOfferingID" Type="Auto" />
                                                    <ext:RecordField Name="StartDate" Type="Date" />
                                                    <ext:RecordField Name="EndDate" Type="Date" />
                                                    <ext:RecordField Name="DataSourceID" Type="Auto" />
                                                    <ext:RecordField Name="Definition" Type="String" />
                                                    <ext:RecordField Name="PhenomenonName" Type="String" />
                                                    <ext:RecordField Name="TransformationName" Type="String" />
                                                    <ext:RecordField Name="PhenomenonOfferingId" Type="Auto" />
                                                    <ext:RecordField Name="UnitOfMeasureId" Type="Auto" />
                                                    <ext:RecordField Name="OfferingName" Type="String" />
                                                    <ext:RecordField Name="UnitofMeasure" Type="String" />
                                                    <ext:RecordField Name="NewPhenomenonOfferingID" Type="Auto" />
                                                    <ext:RecordField Name="NewPhenomenonUOMID" Type="Auto" />
                                                    <ext:RecordField Name="Rank" Type="Int" UseNull="true" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="DataSourceID" Value="Ext.getCmp('#{DataSourceGrid}') && #{DataSourceGrid}.getSelectionModel().hasSelection() ? #{DataSourceGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="150" />
                                        <ext:Column Header="Transformation" DataIndex="TransformationName" Width="150" />
                                        <ext:Column Header="Offering" DataIndex="OfferingName" Width="150" />
                                        <ext:Column Header="UOM" DataIndex="UnitofMeasure" Width="150" />
                                        <ext:DateColumn Header="Effective Date" DataIndex="StartDate" Width="150" Format="dd MMM yyyy" />
                                        <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="150" Format="dd MMM yyyy" />
                                        <ext:Column Header="Order" DataIndex="Rank" Width="150" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Listeners>
                                    <Command Fn="onTransformCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="700" Height="400" Closable="true"
        Hidden="true" Collapsible="false" Title="Data Source Detail" Maximizable="false"
        Layout="Fit" ClientIDMode="Static">
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
                                    <ext:ComboBox ID="cbDataSchema" runat="server" StoreID="DataSchemaStore" Editable="true"
                                        DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true"
                                        TriggerAction="All" AllowBlank="true" DataIndex="DataSchemaID" EmptyText="Select Data Schema"
                                        SelectOnFocus="true" FieldLabel="Data Schema" AnchorHorizontal="93%" BlankText="Data Schema is required"
                                        MsgTarget="Side">
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="this.clearValue();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container3" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                        AllowBlank="false" BlankText="Name is a required" MsgTarget="Side" runat="server"
                                        FieldLabel="Name" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" Vtype="url" runat="server"
                                        MsgTarget="Side" FieldLabel="Url" ClientIDMode="Static" BlankText="URL is required."
                                        AnchorHorizontal="95%">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container5" runat="server" Layout="Column" Height="50">
                        <Items>
                            <ext:Container ID="Container6" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:ComboBox ID="cbUpdateFrequency" runat="server" Editable="false" MsgTarget="Side"
                                        ForceSelection="true" TriggerAction="All" AllowBlank="false" DataIndex="UpdateFreq"
                                        SelectOnFocus="true" BlankText="Frequency is required." EmptyText="Select Update Frequency"
                                        FieldLabel="Update Frequency" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <Listeners>
                                            <Select Fn="FrequencyUpdate" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container7" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:DateField ID="StartDate" DataIndex="StartDate" runat="server" FieldLabel="Start Date"
                                        BlankText="Start Date is required." AnchorHorizontal="93%" Format="dd MMM yyyy"
                                        ClientIDMode="Static">
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Description is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server" AllowBlank="false"
                                BlankText="Description is required" MsgTarget="Side" FieldLabel="Description"
                                AnchorHorizontal="95%">
                            </ext:TextArea>
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
    <ext:Window ID="TransformationDetailWindow" runat="server" Width="450" Height="600"
        Closable="true" Hidden="true" Collapsible="false" Title="Transformation Detail"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="TransformationDetailPanel" runat="server" Title="" MonitorPoll="500"
                MonitorValid="true" MonitorResize="true" Padding="10" Width="440" Height="520"
                ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Hidden ID="tfTransID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:ComboBox ID="cbTransformType" runat="server" StoreID="TransformationTypeStore"
                                Editable="true" BlankText="Transform Type is required" MsgTarget="Side" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All"
                                AllowBlank="false" DataIndex="TransformationTypeID" EmptyText="Select Transform Type"
                                SelectOnFocus="true" FieldLabel="Transformation Type" AnchorHorizontal="95%">
                                <Listeners>
                                    <Select Fn="handlechange" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbPhenomenon" runat="server" StoreID="PhenomenonStore" Editable="true"
                                BlankText="Phenomenon is required" MsgTarget="Side" DisplayField="Name" ValueField="Id"
                                TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false"
                                DataIndex="PhenomenonID" EmptyText="Select Phenomenon" SelectOnFocus="true" FieldLabel="Phenomenon"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Listeners>
                                    <Select Handler="#{cbOffering}.clearValue();#{cbOffering}.getStore().reload();#{cbUnitofMeasure}.clearValue();#{cbUnitofMeasure}.getStore().reload()
                                                    ;#{sbNewOffering}.clearValue();#{sbNewOffering}.getStore().reload();#{sbNewUoM}.clearValue();#{sbNewUoM}.getStore().reload()" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbOffering" runat="server" DataIndex="PhenomenonOfferingId" DisplayField="Name"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store3" runat="server" AutoLoad="false" OnRefreshData="cbOffering_RefreshData">
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
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel11" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="cbUnitofMeasure" runat="server" DataIndex="UnitOfMeasureId" DisplayField="Unit"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store5" runat="server" AutoLoad="false" OnRefreshData="cbUnitofMeasure_RefreshData">
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
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container9" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:DateField ID="dfTransStart" DataIndex="StartDate" runat="server" FieldLabel="Effective Date"
                                AnchorHorizontal="95%"  AllowBlank="false" BlankText="Effective Date is required">
                            </ext:DateField>
                        </Items>
                    </ext:Container><%--Format="dd/MM/yyyy"--%>
                    <ext:Container ID="Container10" runat="server" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextArea ID="tfDefinition" DataIndex="Definition" runat="server" AllowBlank="false"
                                BlankText="Transformation Definition is required" MsgTarget="Side" IsRemoteValidation="true"
                                FieldLabel="Transformation Definition" AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="OnDefinitionValidation">
                                </RemoteValidation>
                            </ext:TextArea>
                        </Items>
                    </ext:Container>
                    <%--=============--%>
                    <ext:Panel ID="Panel2" runat="server" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="sbNewOffering" runat="server" DataIndex="NewPhenomenonOfferingID" DisplayField="Name"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="false"
                                ValueField="ID" FieldLabel="New Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store6" runat="server" AutoLoad="false" OnRefreshData="cbNewOffering_RefreshData">
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
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="sbNewUoM" runat="server" DataIndex="NewPhenomenonUOMID" DisplayField="Unit"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="false"
                                ValueField="ID" FieldLabel="New Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store7" runat="server" AutoLoad="false" OnRefreshData="cbNewUnitofMeasure_RefreshData">
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
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="tfRank" DataIndex="Rank" MaxLength="10"
                                runat="server" FieldLabel="Transformation Rank" AnchorHorizontal="95%"
                                   AllowBlank="false" BlankText="Rank is required">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <%--=============--%>
                </Items>
                <Buttons>
                    <ext:Button ID="SaveTransform" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="SaveTransformation" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
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
    <ext:Window ID="AvailableRoleWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Roles" Width="750" Height="300" X="50" Y="50" 
        Layout="Fit" Hidden="true" ClientIDMode="Static">
        <Items>
            <ext:GridPanel ID="RoleGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="RoleGridStore" runat="server" OnRefreshData="RoleGridStore_RefreshData">
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <Reader>
                            <ext:JsonReader IDProperty="RoleId">
                                <Fields>
                                    <ext:RecordField Name="RoleId" Type="Auto" />
                                    <ext:RecordField Name="RoleName" Type="String" />
                                    <ext:RecordField Name="Description" Type="String" />
                                    <ext:RecordField Name="DateStart" Type="Date" />
                                    <ext:RecordField Name="DateEnd" Type="Date" />
                                    <ext:RecordField Name="IsRoleReadOnly" Type="Boolean" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <BaseParams>
                            <ext:Parameter Name="DataSourceID" Value="Ext.getCmp('#{DataSourceGrid}') && #{DataSourceGrid}.getSelectionModel().hasSelection() ? #{DataSourceGrid}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel4" runat="server">
                    <Columns>
                        <ext:Column Header="Role Name" DataIndex="RoleName" Width="150" />
                        <ext:Column Header="Description" DataIndex="Description" Width="150" />
                        <ext:DateColumn Header="Date Start" DataIndex="DateStart" Width="150" Format="yyyy/MM/dd">
                            <Editor>
                                <ext:DateField runat="server" Format="yyyy/MM/dd">
                                </ext:DateField>
                            </Editor>
                        </ext:DateColumn>
                        <ext:DateColumn Header="Date End" DataIndex="DateEnd" Width="150" Format="yyyy/MM/dd">
                            <Editor>
                                <ext:DateField runat="server" Format="yyyy/MM/dd">
                                </ext:DateField>
                            </Editor>
                        </ext:DateColumn>
                        <ext:CheckColumn Header="Read Only" Tooltip="Read only means users from this role cant import data"
                            DataIndex="IsRoleReadOnly" Editable="true">
                            <Editor>
                                <ext:Checkbox runat="server">
                                </ext:Checkbox>
                            </Editor>
                        </ext:CheckColumn>
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="AcceptRoleButton" runat="server" Text="Save" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="AcceptRoleButton_Click">
                                <EventMask ShowMask="true" />
                                <ExtraParams>
                                    <ext:Parameter Name="RoleValues" Value="Ext.encode(#{RoleGrid}.getRowsValues({selectedOnly:true}))"
                                        Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Items>
        <Listeners>
            <Hide Fn="CloseAvailableRole" />
        </Listeners>
    </ext:Window>
    <ext:Window ID="RoleDetailWindow" runat="server" Width="450" Height="400" Closable="true"
        Hidden="true" Collapsible="false" Title="Role Detail" Maximizable="false" Layout="Fit"
        ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="RoleDetailFormPanel" runat="server" Title="" MonitorPoll="500"
                MonitorValid="true" MonitorResize="true" Padding="10" Width="440" Height="370"
                ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Hidden ID="hiddenRoleDetail" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Container ID="Container8" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:DateField ID="dfRoleDetailStart" DataIndex="DateStart" runat="server" FieldLabel="Start Date"
                                AnchorHorizontal="95%" Format="yyyy/MM/dd" AllowBlank="false" BlankText="Start Date is required">
                            </ext:DateField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container11" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:DateField ID="dfRoleDetailEnd" DataIndex="DateEnd" runat="server" FieldLabel="End Date"
                                AnchorHorizontal="95%" Format="yyyy/MM/dd" AllowBlank="false" BlankText="End Date is required">
                            </ext:DateField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container12" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:Checkbox ID="cbIsRoleReadOnly" DataIndex="IsRoleReadOnly" runat="server" FieldLabel="Read Only"
                                AnchorHorizontal="95%" >
                            </ext:Checkbox>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="Button4" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="SaveRoleDetail" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
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
</asp:Content>

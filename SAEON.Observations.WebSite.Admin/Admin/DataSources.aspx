<%@ Page Title="Data Sources" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DataSources.aspx.cs" Inherits="Admin_DataSources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../JS/DataSources.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(DataSourcesGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(DataSourcesGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            DataSourcesGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
    <ext:Store ID="InstrumentStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
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
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Data Sources" Height="350" Layout="FitLayout">
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
                            <ext:GridPanel ID="DataSourcesGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="DataSourcesGridStore" runat="server" RemoteSort="true" OnRefreshData="DataSourcesGridStore_RefreshData"
                                        OnSubmitData="DataSourcesGridStore_Submit">
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
                                                    <ext:RecordField Name="EndDate" Type="Date" />
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
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="500" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="150" />
                                        <ext:Column Header="Data Schema" DataIndex="DataSchemaName" Width="300" />
                                        <ext:CommandColumn Width="75">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" ToolTip-Text="Edit" />
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
                                            <ext:DateFilter DataIndex="StartDate" />
                                            <ext:DateFilter DataIndex="EndDate" />
                                            <ext:DateFilter DataIndex="LastUpdate" />
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
                </North>
                <Center>
                    <ext:TabPanel ID="tpCenter" runat="server" TabPosition="Top" Border="false" ClientIDMode="Static">
                        <Items>
                            <ext:Panel ID="pnlTransformations" runat="server" Title="Transformations" ClientIDMode="Static" Layout="FitLayout">
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
                                    <ext:GridPanel ID="TransformationsGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="TransformationsGridStore" runat="server" OnRefreshData="TransformationsGridStore_RefreshData">
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
                                                            <ext:RecordField Name="OfferingName" Type="String" />
                                                            <ext:RecordField Name="UnitOfMeasureId" Type="Auto" />
                                                            <ext:RecordField Name="UnitOfMeasureUnit" Type="String" />
                                                            <ext:RecordField Name="NewPhenomenonOfferingID" Type="Auto" />
                                                            <ext:RecordField Name="NewOfferingName" Type="Auto" />
                                                            <ext:RecordField Name="NewPhenomenonUOMID" Type="Auto" />
                                                            <ext:RecordField Name="NewUnitOfMeasureUnit" Type="Auto" />
                                                            <ext:RecordField Name="Rank" Type="Int" UseNull="true" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="DataSourceID" Value="Ext.getCmp('#{DataSourcesGrid}') && #{DataSourcesGrid}.getSelectionModel().hasSelection() ? #{DataSourcesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="200" />
                                                <ext:Column Header="Transformation" DataIndex="TransformationName" Width="150" />
                                                <ext:Column Header="Offering" DataIndex="OfferingName" Width="200" />
                                                <ext:Column Header="Unit of Measure" DataIndex="UnitOfMeasureUnit" Width="200" />
                                                <ext:Column Header="New Offering" DataIndex="NewOfferingName" Width="200" />
                                                <ext:Column Header="New Unit of Measure" DataIndex="NewUnitOfMeasureUnit" Width="200" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="150" Format="dd MMM yyyy HH:mm" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="150" Format="dd MMM yyyy HH:mm" />
                                                <ext:CommandColumn Width="150">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" ToolTip-Text="Edit" />
                                                        <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="Delete" ToolTip-Text="Delete" />
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
                        </Items>
                    </ext:TabPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="700" Height="450" Closable="true"
        Hidden="true" Collapsible="false" Title="Data Source Detail" Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true" LabelAlign="Top"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Container2" runat="server" Layout="FormLayout" Border="false">
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50" AnchorHorizontal="96%"
                                runat="server" FieldLabel="Code" AllowBlank="false" BlankText="Code is a required"
                                MsgTarget="Side" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel12" runat="server" Layout="FormLayout" Border="false">
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true" AnchorHorizontal="96%"
                                AllowBlank="false" BlankText="Name is a required" MsgTarget="Side" runat="server"
                                FieldLabel="Name" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container4" runat="server" Layout="Form">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server" AllowBlank="false" AnchorHorizontal="96%"
                                BlankText="Description is required" MsgTarget="Side" FieldLabel="Description">
                            </ext:TextArea>
                        </Items>
                    </ext:Container>
                    <ext:Panel ID="Panel14" runat="server" Layout="FormLayout" Border="false">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" Vtype="url" runat="server" AnchorHorizontal="96%"
                                MsgTarget="Side" FieldLabel="Url" ClientIDMode="Static" BlankText="URL is required.">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container7" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container13" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:ComboBox ID="cbDataSchema" runat="server" StoreID="DataSchemaStore" Editable="true" AnchorHorizontal="98%"
                                        DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true"
                                        TriggerAction="All" AllowBlank="true" DataIndex="DataSchemaID" EmptyText="Select Data Schema"
                                        SelectOnFocus="true" FieldLabel="Data Schema" BlankText="Data Schema is required"
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
                            <ext:Container ID="Container15" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:ComboBox ID="cbUpdateFrequency" runat="server" Editable="false" MsgTarget="Side" AnchorHorizontal="100%"
                                        ForceSelection="true" TriggerAction="All" AllowBlank="false" DataIndex="UpdateFreq"
                                        SelectOnFocus="true" BlankText="Frequency is required." EmptyText="Select Update Frequency"
                                        FieldLabel="Update Frequency" ClientIDMode="Static">
                                        <Listeners>
                                            <Select Fn="FrequencyUpdate" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container6" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container1" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container3" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:DateField ID="dfStartDate" DataIndex="StartDate" runat="server" FieldLabel="Start Date" AnchorHorizontal="98%"
                                        BlankText="Start Date is required." Format="dd MMM yyyy" ClientIDMode="Static">
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container5" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:DateField ID="dfEndDate" DataIndex="EndDate" runat="server" FieldLabel="End Date" AnchorHorizontal="100%"
                                        BlankText="End Date is required." Format="dd MMM yyyy" ClientIDMode="Static">
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container16" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
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
    <ext:Window ID="TransformationDetailWindow" runat="server" Width="450" Height="650"
        Closable="true" Hidden="true" Collapsible="false" Title="Transformation Detail"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="TransformationDetailPanel" runat="server" Title="" MonitorPoll="500"
                MonitorValid="true" MonitorResize="true" Padding="10" Width="440" Height="520"
                ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <Items>
                    <ext:Hidden ID="tfTransID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbTransformType" runat="server" StoreID="TransformationTypeStore"
                                Editable="true" BlankText="Transform Type is required" MsgTarget="Side" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All"
                                AllowBlank="false" DataIndex="TransformationTypeID" EmptyText="Select Transform Type"
                                SelectOnFocus="true" FieldLabel="Transformation Type" AnchorHorizontal="96%">
                                <Listeners>
                                    <Select Fn="handlechange" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:SelectBox ID="cbPhenomenon" runat="server" StoreID="PhenomenonStore" Editable="true"
                                BlankText="Phenomenon is required" MsgTarget="Side" DisplayField="Name" ValueField="Id"
                                TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false"
                                DataIndex="PhenomenonID" EmptyText="Select Phenomenon" SelectOnFocus="true" FieldLabel="Phenomenon"
                                AnchorHorizontal="96%" ClientIDMode="Static">
                                <Listeners>
                                    <Select Handler="#{cbOffering}.clearValue();#{cbOffering}.getStore().reload();#{cbUnitofMeasure}.clearValue();#{cbUnitofMeasure}.getStore().reload()
                                                    ;#{sbNewOffering}.clearValue();#{sbNewOffering}.getStore().reload();#{sbNewUoM}.clearValue();#{sbNewUoM}.getStore().reload()" />
                                </Listeners>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:SelectBox ID="cbOffering" runat="server" DataIndex="PhenomenonOfferingId" DisplayField="Name"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="96%" ClientIDMode="Static">
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
                    <ext:Panel ID="Panel11" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:SelectBox ID="cbUnitofMeasure" runat="server" DataIndex="UnitOfMeasureId" DisplayField="Unit"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="96%" ClientIDMode="Static">
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
                    <ext:Container ID="Container9" runat="server" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:DateField ID="dfTransStart" DataIndex="StartDate" runat="server" FieldLabel="Start Date"
                                AnchorHorizontal="96%" AllowBlank="true" BlankText="Start Date is required" Format="dd MMM yyyy HH:mm">
                            </ext:DateField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container14" runat="server" Layout="Form" ColumnWidth=".5">
                        <Items>
                            <ext:DateField ID="dfTransEnd" DataIndex="EndDate" runat="server" FieldLabel="End Date"
                                AnchorHorizontal="96%" AllowBlank="true" BlankText="End Date is required" Format="dd MMM yyyy HH:mm">
                            </ext:DateField>
                        </Items>
                    </ext:Container>
                    <%--Format="dd/MM/yyyy"--%>
                    <ext:Container ID="Container10" runat="server" Layout="Form">
                        <Items>
                            <ext:TextArea ID="tfDefinition" DataIndex="Definition" runat="server" AllowBlank="false"
                                BlankText="Transformation Definition is required" MsgTarget="Side" IsRemoteValidation="true"
                                FieldLabel="Transformation Definition" AnchorHorizontal="96%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="OnDefinitionValidation">
                                </RemoteValidation>
                            </ext:TextArea>
                        </Items>
                    </ext:Container>
                    <%--=============--%>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:SelectBox ID="sbNewOffering" runat="server" DataIndex="NewPhenomenonOfferingID" DisplayField="Name"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="false"
                                ValueField="ID" FieldLabel="New Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="96%" ClientIDMode="Static">
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
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:SelectBox ID="sbNewUoM" runat="server" DataIndex="NewPhenomenonUOMID" DisplayField="Unit"
                                AllowBlank="true" BlankText="Offering is required" MsgTarget="Side" ForceSelection="false"
                                ValueField="ID" FieldLabel="New Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="96%" ClientIDMode="Static">
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
                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:NumberField AllowDecimals="false" ID="tfRank" DataIndex="Rank" MaxLength="10"
                                runat="server" FieldLabel="Transformation Rank" AnchorHorizontal="96%"
                                AllowBlank="false" BlankText="Rank is required" MinValue="0" Number="0">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <%--=============--%>
                </Items>
                <Buttons>
                    <%--                    <ext:Button ID="btnValidate" runat="server" Text="Validate" Icon="Tick" ClientIDMode="Static">
                        <Listeners>
                            <Click Handler="alert(#{TransformationDetailPanel}.validate())" />
                        </Listeners>
                    </ext:Button>--%>
                    <ext:Button ID="SaveTransform" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="SaveTransformation" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar2" runat="server" Height="25">
                        <%--                        <Plugins>
                            <ext:ValidationStatus
                                runat="server"
                                FormPanelID="TransformationDetailPanel"
                                ValidIcon="Accept"
                                ErrorIcon="Exclamation" />
                        </Plugins>--%>
                    </ext:StatusBar>
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
</asp:Content>


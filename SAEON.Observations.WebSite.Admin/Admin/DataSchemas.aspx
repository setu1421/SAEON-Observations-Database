<%@ Page Title="Data Schemas" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DataSchemas.aspx.cs" Inherits="Admin_DataSchemas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../JS/DataSchemas.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var formMode = 'Add';
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(DataSchemasGridFilters.buildQuery(DataSchemasGridFilters.getFilterData())));
            //VisCols.setValue(Ext.encode(DataSchemaGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(DataSchemasGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(DataSchemasGridFilters.store.sortInfo.field + "|" + DataSchemasGridFilters.store.sortInfo.direction);
            DataSchemasGrid.submitData(false);
        };

    </script>
    <ext:Store ID="DataSourceTypeStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Description" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="SchemaColumnTypeStore" runat="server" AutoLoad="true">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="SchemaPickerStore" runat="server" AutoLoad="true">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="PhenomenonStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="OfferingStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="PhenomenonName" Type="String" />
                    <ext:RecordField Name="OfferingName" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="UnitOfMeasureStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="PhenomenonName" Type="String" />
                    <ext:RecordField Name="UnitOfMeasureUnit" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Data Schemas" Height="350" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Schema" ClientIDMode="Static">
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
                            <ext:GridPanel ID="DataSchemasGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="DataSchemasGridStore" runat="server" RemoteSort="true" OnRefreshData="DataSchemasGridStore_RefreshData"
                                        OnSubmitData="DataSchemasGridStore_Submit">
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
                                                    <ext:RecordField Name="HasColumnNames" Type="Boolean" />
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
                                        <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="500" />
                                        <ext:Column Header="Source Type Code" Width="100" DataIndex="DataSourceTypeCode" />
                                        <ext:Column Header="Source Type Description" Width="150" DataIndex="DataSourceTypeDesc" />
                                        <ext:Column Header="Ignore First" DataIndex="IgnoreFirst" Width="100" />
                                        <ext:Column Header="Ignore Last" DataIndex="IgnoreLast" Width="100" />
                                        <ext:CheckColumn Header="Column Names" DataIndex="HasColumnNames" Width="100" Tooltip="Column Names" />
                                        <ext:Column Header="Delimiter" DataIndex="Delimiter" Width="100" />
                                        <ext:CommandColumn Width="200">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="Delete" />
                                                <ext:GridCommand Icon="PageCopy" CommandName="Copy" Text="Copy" />
                                                <%--<ext:GridCommand Icon="Zoom" CommandName="Preview" Text="Test" ToolTip-Text="Test" />--%>
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="MasterRowSelect" Buffer="250" />
                                        </Listeners>
                                        <DirectEvents>
                                            <RowSelect OnEvent="MasterRowSelect" />
                                        </DirectEvents>

                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="DataSchemasGridFilters" ClientIDMode="Static">
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
                                <%--                                <DirectEvents>
                                    <Command OnEvent="onCommand">
                                        <ExtraParams>
                                            <ext:Parameter Name="type" Value="params[0]" Mode="Raw" />
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                        </ExtraParams>
                                        <EventMask ShowMask="true" />
                                    </Command>
                                </DirectEvents>--%>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </North>
                <Center>
                    <ext:TabPanel ID="tpCenter" runat="server" TabPosition="Top" Border="false" ClientIDMode="Static">
                        <Items>
                            <ext:Panel ID="pnlColumns" runat="server" Title="Columns" ClientIDMode="Static" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:Button ID="btnAddSchemaColumn" runat="server" Icon="Add" Text="Add Column" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip4" runat="server" Html="Add" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection()){#{SchemaColumnWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a Data Schema.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnCopySchemaColumns" runat="server" Icon="PageCopy" Text="Copy Columns" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip3" runat="server" Html="Copy columns" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection()){#{SchemaPickerWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a Data Schema.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddSchemaColumn" runat="server" Icon="Add" Text="Add SchemaColumn">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddSchemaColumnClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="SchemaColumnsGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="SchemaColumnsGridStore" runat="server" OnRefreshData="SchemaColumnsGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="DataSchemaID" Type="Auto" />
                                                            <ext:RecordField Name="Name" Type="Auto" />
                                                            <ext:RecordField Name="Number" Type="Auto" />
                                                            <ext:RecordField Name="SchemaColumnTypeID" Type="Auto" />
                                                            <ext:RecordField Name="SchemaColumnTypeName" Type="Auto" />
                                                            <ext:RecordField Name="Width" Type="Auto" />
                                                            <ext:RecordField Name="Format" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonID" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonName" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonOfferingID" Type="Auto" />
                                                            <ext:RecordField Name="OfferingName" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonUOMID" Type="Auto" />
                                                            <ext:RecordField Name="UnitOfMeasureUnit" Type="Auto" />
                                                            <ext:RecordField Name="EmptyValue" Type="Auto" />
                                                            <ext:RecordField Name="FixedTime" Type="Auto" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="DataSchemaID" Value="Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection() ? #{DataSchemasGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel4" runat="server">
                                            <Columns>
                                                <ext:Column Header="Name" DataIndex="Name" Width="200" />
                                                <ext:Column Header="Type" DataIndex="SchemaColumnTypeName" Width="100" />
                                                <ext:Column Header="Width" DataIndex="Width" Width="50" Hideable="true" />
                                                <ext:Column Header="Format" DataIndex="Format" Width="150" />
                                                <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="150" />
                                                <ext:Column Header="Offering" DataIndex="OfferingName" Width="150" />
                                                <ext:Column Header="Unit of measure" DataIndex="UnitOfMeasureUnit" Width="150" />
                                                <ext:Column Header="Empty value" DataIndex="EmptyValue" Width="75" />
                                                <ext:Column Header="Fixed time" DataIndex="FixedTime" Width="75" />
                                                <ext:CommandColumn Width="200">
                                                    <PrepareToolbar Fn="PrepareSchemaColumnsToolbar" />
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                        <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="Delete" />
                                                        <ext:GridCommand Icon="ArrowUp" CommandName="Up" Text="Up" />
                                                        <ext:GridCommand Icon="ArrowDown" CommandName="Down" Text="Down" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar3" runat="server" PageSize="25" EmptyMsg="No data found" />
                                        </BottomBar>
                                        <Listeners>
                                            <Command Fn="OnSchemaColumnCommand" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel2" runat="server" Title="Data Sources" Layout="FitLayout" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="Button1" runat="server" Icon="Link" Text="Link Data Sources" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection()){#{AvailableDataSourcesGridStore}.reload();#{AvailableDataSourcesWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a Data Schema.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddDataSource" runat="server" Icon="Add" Text="Add DataSource">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddDataSourceClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="DataSourcesGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="DataSourcesGridStore" runat="server" OnRefreshData="DataSourcesGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="Code" Type="Auto" />
                                                            <ext:RecordField Name="Name" Type="Auto" />
                                                            <ext:RecordField Name="Url" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="LastUpdate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="DataSchemaID" Value="Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection() ? #{DataSchemasGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="Code" Width="100" />
                                                <ext:Column Header="Name" DataIndex="Name" Width="200" />
                                                <ext:Column Header="Url" DataIndex="Url" Width="100" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="150" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="Last Update" DataIndex="LastUpdate" Width="150" Format="dd MMM yyyy" />
                                                <ext:CommandColumn Width="200">
                                                    <Commands>
                                                        <%--<ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />--%>
                                                        <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="Delete" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="25" EmptyMsg="No data found" />
                                        </BottomBar>
                                        <Listeners>
                                            <Command Fn="OnDataSourceCommand" />
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
    <ext:Window ID="DetailWindow" runat="server" Width="640" Height="550" Closable="true"
        Hidden="true" Collapsible="false" Title="Data Schema Detail" Maximizable="false" Layout="Fit" AutoScroll="true" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true" LabelAlign="Top"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static" />
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                runat="server" FieldLabel="Code" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Code is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField">
                                    <ExtraParams>
                                        <ext:Parameter Name="id" Value="1" Mode="Raw" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel15" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                runat="server" FieldLabel="Name" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Name is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel16" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="150" runat="server"
                                FieldLabel="Description" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Description is a required" MsgTarget="Side">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container2" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container12" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:ComboBox ID="cbDataSourceType" runat="server" StoreID="DataSourceTypeStore" BlankText="Data Source Type is required"
                                        MsgTarget="Side" DisplayField="Description" ValueField="Id" TypeAhead="true"
                                        ForceSelection="true" TriggerAction="All" AllowBlank="false" DataIndex="DataSourceTypeID"
                                        EmptyText="Select Data Source Type" SelectOnFocus="true" FieldLabel="Data Source Type"
                                        AnchorHorizontal="90%">
                                        <Listeners>
                                            <%--<Select Handler="#{cbDataSourceType}.getValue() == '25839703-3cb3-4c23-aca3-4399cc52ecde'?#{cbDelimiter}.allowBlank=false:#{cbDelimiter}.allowBlank=true;#{cbDelimiter}.clearValue();#{cbDelimiter}.clearInvalid();#{cbDelimiter}.markAsValid();" />--%>
                                        </Listeners>
                                        <DirectEvents>
                                            <Select OnEvent="cbDataSourceTypeSelect" />
                                        </DirectEvents>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container13" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:ComboBox ID="cbDelimiter" runat="server" Editable="false" MsgTarget="Side" TypeAhead="true"
                                        ForceSelection="true" TriggerAction="All" DataIndex="Delimiter"
                                        EmptyText="Select Delimiter" SelectOnFocus="true" BlankText="Delimiter is required for this Source Type"
                                        FieldLabel="Delimiter" AnchorHorizontal="90%" ClientIDMode="Static">
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
                            <ext:Container ID="Container14" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container1" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container3" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:NumberField AllowDecimals="false" ID="nfIgnoreFirst" DataIndex="IgnoreFirst" MinValue="0"
                                        MaxLength="10" runat="server" FieldLabel="Ignore first # of lines on import"
                                        AnchorHorizontal="98%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container15" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:NumberField AllowDecimals="false" ID="nfIgnoreLast" DataIndex="IgnoreLast" MaxLength="10" MinValue="0"
                                        runat="server" FieldLabel="Ignore last # of lines on import" AnchorHorizontal="100%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container20" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container5" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:TextField ID="tfSplit" DataIndex="SplitSelector" MaxLength="150" runat="server"
                                        FieldLabel="File Split Condition - Data Split" AnchorHorizontal="98%" MsgTarget="Side">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container6" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:NumberField ID="nfSplitIndex" DataIndex="SplitIndex" MaxLength="150" runat="server"
                                        FieldLabel="Data file Split Index" AnchorHorizontal="100%" AllowDecimals="true"
                                        BlankText="Split index is required for a split data file" MsgTarget="Side">
                                        <Listeners>
                                            <Valid Handler="if(#{tfSplit}.getValue() == '') this.allowBlank = true;else this.allowBlank = false;" />
                                        </Listeners>
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container21" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container10" runat="server" Layout="Column" Height="50">
                        <Items>
                            <ext:Container ID="Container11" runat="server" Layout="Form" ColumnWidth="1">
                                <Items>
                                    <ext:TextField ID="tfCondition" DataIndex="Condition" MaxLength="150" runat="server"
                                        FieldLabel="Record Condition - Ignore fields that start with" AnchorHorizontal="96%"
                                        MsgTarget="Side">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container17" runat="server" Layout="Form">
                        <Items>
                            <ext:Checkbox ID="cbHasColumnNames" DataIndex="HasColumnNames" runat="server" FieldLabel="Column Names" AnchorHorizontal="96%" ColumnWidth="1">
                            </ext:Checkbox>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="btnValidate" runat="server" Text="Validate" Icon="Tick" ClientIDMode="Static">
                        <Listeners>
                            <Click Handler="GetInvalidFields(#{DetailsFormPanel});" />
                        </Listeners>
                    </ext:Button>
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
                    <%--<ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />--%>
                    <ClientValidation Handler="#{btnSave}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="SchemaColumnWindow" runat="server" Width="800" Height="500" Closable="true"
        Hidden="true" Collapsible="false" Title=" Column"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Show Fn="ShowSchemaColumnForm" />
            <Hide Fn="HideSchemaColumnForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="SchemaColumnFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="SchemaColumnID" DataIndex="Id" runat="server" ClientIDMode="Static" />
                    <ext:Container ID="Container16" runat="server" Layout="Form">
                        <Items>
                            <ext:TextField ID="tfColumnName" DataIndex="Name" MaxLength="50" runat="server" IsRemoteValidation="true" ClientIDMode="Static"
                                FieldLabel="Name" AnchorHorizontal="96%" MsgTarget="Side" AllowBlank="false" EmptyText="Enter a name"
                                Regex="^[A-Za-z]+\w*$" RegexText="Name must start with a character and can only contain characters, numbers and underscores">
                                <RemoteValidation OnValidation="ValidateColumnField" />
                            </ext:TextField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container18" runat="server" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbSchemaColumnType" runat="server" StoreID="SchemaColumnTypeStore" IsRemoteValidation="true" MsgTarget="Side"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                ValueField="Id" DisplayField="Name" DataIndex="SchemaColumnTypeID" FieldLabel="Column type" EmptyText="Select a column type"
                                AnchorHorizontal="96%" ClientIDMode="Static" FireSelectOnLoad="true">
                                <RemoteValidation OnValidation="ValidateColumnField" />
                                <DirectEvents>
                                    <Select OnEvent="cbSchemaColumnTypeSelect" />
                                </DirectEvents>
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="ctWidth" runat="server" Layout="Form">
                        <Items>
                            <ext:NumberField ID="nfWidth" DataIndex="Width" MaxLength="5" runat="server" AllowBlank="false" EmptyText="Enter a width" MsgTarget="Side"
                                FieldLabel="Width" AnchorHorizontal="96%" AllowDecimals="false" MinValue="1" ClientIDMode="Static">
                            </ext:NumberField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="ctFormat" runat="server" Layout="Form">
                        <Items>
                            <%--                            <ext:TextField ID="tfFormat" runat="server" IsRemoteValidation="true" ClientIDMode="Static" MsgTarget="Side" MaxLength="50"
                                AllowBlank="false" FieldLabel="Format" DataIndex="Format" EmptyText="Enter a format" AnchorHorizontal="96%">
                                <RemoteValidation OnValidation="ValidateColumnField" />
                            </ext:TextField>--%>
                            <ext:ComboBox ID="cbFormat" runat="server" IsRemoteValidation="true" ClientIDMode="Static" MsgTarget="Side"
                                TriggerAction="All" Editable="true" TypeAhead="true" ForceSelection="false" AllowBlank="false" FieldLabel="Format"
                                DataIndex="Format" EmptyText="Enter a format" AnchorHorizontal="96%">
                                <RemoteValidation OnValidation="ValidateColumnField" />
                                <Items>
                                    <ext:ListItem Text="d/M/yyyy (21 Sep 2017 13:14:15 -> 21/9/2017)" Value="d/M/yyyy" />
                                    <ext:ListItem Text="dd/MM/yy HH:mm:ss (21 Sep 2017 13:14:15 -> 21/09/17 13:14:15)" Value="dd/MM/yy HH:mm:ss" />
                                    <ext:ListItem Text="ddMMyy (21 Sep 2017 13:14:15 -> 210917)" Value="ddMMyy" />
                                    <ext:ListItem Text="HH:mm (21 Sep 2017 13:14:15 -> 13:14)" Value="HH:mm" />
                                    <ext:ListItem Text="hh:mm t (21 Sep 2017 13:14:15 -> 01:14 P)" Value="hh:mm t" />
                                    <ext:ListItem Text="hh:mm tt (21 Sep 2017 13:14:15 -> 01:14 PM)" Value="hh:mm tt" />
                                    <ext:ListItem Text="HH:mm:ss (21 Sep 2017 13:14:15 -> 13:14:15)" Value="HH:mm:ss" />
                                    <ext:ListItem Text="hh:mm:ss tt (21 Sep 2017 13:14:15 -> 01:14:15 PM)" Value="hh:mm:ss tt" />
                                    <ext:ListItem Text="HHmm (21 Sep 2017 13:14:15 -> 1314)" Value="HHmm" />
                                    <ext:ListItem Text="MM/dd/yy HH:mm:ss (21 Sep 2017 13:14:15 -> 09/21/17)" Value="MM/dd/yy HH:mm:ss" />
                                    <ext:ListItem Text="MM/dd/yy hh:mm:ss AM/PM (21 Sep 2017 13:14:15 -> 09/21/17 01:14:15 PM)" Value="MM/dd/yy hh:mm:ss AM/PM" />
                                    <ext:ListItem Text="yy/MM/dd (21 Sep 2017 13:14:15 -> 17/09/21)" Value="yy/MM/dd" />
                                    <ext:ListItem Text="yyMMdd (21 Sep 2017 13:14:15 -> 170921)" Value="yyMMdd" />
                                    <ext:ListItem Text="yyyy/MM/dd (21 Sep 2017 13:14:15 -> 2017/09/21)" Value="yyyy/MM/dd" />
                                    <ext:ListItem Text="yyyy/MM/dd HH:mm (21 Sep 2017 13:14:15 -> 2017/09/21 13:14)" Value="yyyy/MM/dd HH:mm" />
                                    <ext:ListItem Text="yyyy/MM/dd HH:mm:ss (21 Sep 2017 13:14:15 -> 2017/09/21 13:14:15)" Value="yyyy/MM/dd HH:mm:ss" />
                                    <ext:ListItem Text="yyyyMMdd (21 Sep 2017 13:14:15 -> 20170921)" Value="yyyyMMdd" />
                                    <ext:ListItem Text="yyyy-MM-dd (21 Sep 2017 13:14:15 -> 2017-09-21)" Value="yyyy-MM-dd" />
                                    <ext:ListItem Text="yyyy-MM-dd HH:mm (21 Sep 2017 13:14:15 -> 2017-09-21 13:14)" Value="yyyy-MM-dd HH:mm" />
                                    <ext:ListItem Text="yyyy-MM-dd HH:mm:ss (21 Sep 2017 13:14:15 -> 2017-09-21 13:14:15)" Value="yyyy-MM-dd HH:mm:ss" />
                                </Items>
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container19" runat="server" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbPhenomenon" runat="server" StoreID="PhenomenonStore" MsgTarget="Side" DisplayField="Name"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                ValueField="Id" FieldLabel="Phenomenon" DataIndex="PhenomenonID" EmptyText="Select a phenomenon" ValueNotFoundText="Select a phenomenon"
                                AnchorHorizontal="96%" ClientIDMode="Static" FireSelectOnLoad="true">
                                <DirectEvents>
                                    <Select OnEvent="cbPhenomenonSelect" />
                                </DirectEvents>
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container23" runat="server" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbOffering" runat="server" StoreID="OfferingStore" MsgTarget="Side" DisplayField="OfferingName"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                ValueField="Id" FieldLabel="Offering" DataIndex="PhenomenonOfferingID" EmptyText="Select an offering" ValueNotFoundText="Select an offering"
                                AnchorHorizontal="96%" ClientIDMode="Static" FireSelectOnLoad="true">
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container24" runat="server" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbUnitOfMeasure" runat="server" StoreID="UnitOfMeasureStore" MsgTarget="Side" DisplayField="UnitOfMeasureUnit"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                ValueField="Id" FieldLabel="Unit of measure" DataIndex="PhenomenonUOMID" EmptyText="Select a unit of measure" ValueNotFoundText="Select a unit of measure"
                                AnchorHorizontal="96%" ClientIDMode="Static" FireSelectOnLoad="true">
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container25" runat="server" Layout="Form">
                        <Items>
                            <ext:TextField ID="tfEmptyValue" DataIndex="EmptyValue" MaxLength="50" runat="server" FieldLabel="Empty Value" AnchorHorizontal="96%" ClientIDMode="Static">
                            </ext:TextField>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container26" runat="server" Layout="Form">
                        <Items>
                            <ext:TimeField ID="ttFixedTime" runat="server" DataIndex="FixedTime" FieldLabel="Fixed Time" MsgTarget="Side"
                                EmptyText="Please select" AnchorHorizontal="96%" AllowBlank="false" 
                                BlankText="Fixed Time is required" ClientIDMode="Static" Format="H:mm" Increment="60">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:TimeField>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="btnValidateSchemaColumn" runat="server" Text="Validate" Icon="Tick" ClientIDMode="Static">
                        <Listeners>
                            <Click Handler="GetInvalidFields(#{SchemaColumnFormPanel});" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnSchemaColumnSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="SchemaColumnSave">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar2" runat="server" Height="25">
                        <Plugins>
                            <%--                            <ext:ValidationStatus
                                runat="server"
                                FormPanelID="SchemaColumnFormPanel"
                                ValidIcon="Accept"
                                ErrorIcon="Exclamation" />--%>
                        </Plugins>
                    </ext:StatusBar>
                </BottomBar>
                <Listeners>
                    <%--                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />--%>
                    <ClientValidation Handler="#{btnSchemaColumnSave}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="AvailableDataSourcesWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available DataSources" Width="750" Height="300" X="50" Y="50"
        Layout="Fit" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableDataSources" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="AvailableDataSourcesGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="AvailableDataSourcesGridStore" runat="server" OnRefreshData="AvailableDataSourcesGridStore_RefreshData">
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <Reader>
                            <ext:JsonReader IDProperty="Id">
                                <Fields>
                                    <ext:RecordField Name="Id" Type="Auto" />
                                    <ext:RecordField Name="Code" Type="String" />
                                    <ext:RecordField Name="Name" Type="String" />
                                    <ext:RecordField Name="StartDate" Type="Date" />
                                    <ext:RecordField Name="LastUpdate" Type="Date" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <BaseParams>
                            <ext:Parameter Name="DataSchemaID" Value="Ext.getCmp('#{DataSchemasGrid}') && #{DataSchemasGrid}.getSelectionModel().hasSelection() ? #{DataSchemasGrid}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                        <ext:Column Header="Name" DataIndex="Name" Width="200" />
                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                        <ext:DateColumn Header="Last Update" DataIndex="LastUpdate" Width="100" Format="dd MMM yyyy" />
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="btnDataSourceLinksSave" runat="server" Text="Save" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="DataSourceLinksSave">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <Plugins>
                    <ext:GridFilters runat="server" ID="DataSourcesFilters" ClientIDMode="Static" Local="true">
                        <Filters>
                            <ext:StringFilter DataIndex="Code" />
                            <ext:StringFilter DataIndex="Name" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>

            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="SchemaPickerWindow" runat="server" Width="800" Height="150" Closable="true"
        Hidden="true" Collapsible="false" Title="Pick schema" Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="SchemaPickerFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Container ID="Container7" runat="server" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbSchemaPickerID" runat="server" StoreID="SchemaPickerStore" MsgTarget="Side"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                ValueField="Id" DisplayField="Name" DataIndex="ID" FieldLabel="Data Schema" EmptyText="Select a data schema"
                                AnchorHorizontal="96%" ClientIDMode="Static" FireSelectOnLoad="true">
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSchemaPickerOk" runat="server" Text="Ok" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="SchemaPickerOk">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar3" runat="server" Height="25">
                        <Plugins>
                        </Plugins>
                    </ext:StatusBar>
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="SchemaCopyWindow" runat="server" Width="640" Height="300" Closable="true"
        Hidden="true" Collapsible="false" Title="Copy Schema" Maximizable="false" Layout="Fit" AutoScroll="true" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="SchemaCopyFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true" LabelAlign="Top"
                MonitorResize="true" Padding="10" ButtonAlign="Right" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfSchemaCopyID" DataIndex="Id" runat="server" ClientIDMode="Static" />
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfSchemaCopyCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                runat="server" FieldLabel="Code" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Code is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateSchemaCopyField">
                                    <ExtraParams>
                                        <ext:Parameter Name="id" Value="1" Mode="Raw" />
                                    </ExtraParams>
                                </RemoteValidation>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfSchemaCopyName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                runat="server" FieldLabel="Name" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Name is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateSchemaCopyField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextArea ID="tfSchemaCopyDescription" DataIndex="Description" MaxLength="150" runat="server" IsRemoteValidation="true"
                                FieldLabel="Description" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Description is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSchemaCopySave" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="SchemaCopySave" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
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
</asp:Content>


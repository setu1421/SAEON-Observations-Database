﻿<%@ Page Title="Sensors" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Sensors.aspx.cs" Inherits="Admin_Sensors" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../JS/Sensors.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>

    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_SensorGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(SensorsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            SensorsGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Sensors" Height="350" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Sensor" ClientIDMode="Static">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Fn="New" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:Button ID="Button2" runat="server" Text="To Excel"
                                        Icon="PageExcel">
                                        <Listeners>
                                            <Click Handler="submitValue('exc');" />
                                        </Listeners>

                                    </ext:Button>
                                    <ext:Button ID="Button3" runat="server" Text="To CSV"
                                        Icon="PageAttach">
                                        <Listeners>
                                            <Click Handler="submitValue('csv');" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="SensorsGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="SensorsGridStore" runat="server" RemoteSort="true" OnRefreshData="SensorsGridStore_RefreshData" OnSubmitData="SensorsGridStore_Submit">
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
                                                    <ext:RecordField Name="StationID" Type="String" />
                                                    <ext:RecordField Name="PhenomenonID" Type="Auto" />
                                                    <ext:RecordField Name="PhenomenonName" Type="Auto" />
                                                    <ext:RecordField Name="DataSourceID" Type="Auto" />
                                                    <ext:RecordField Name="DataSourceName" Type="String" />
                                                    <ext:RecordField Name="DataSchemaID" Type="Auto" />
                                                    <ext:RecordField Name="DataSchemaName" Type="String" />
                                                    <ext:RecordField Name="UserId" Type="String" />
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
                                        <ext:Column Header="Description" DataIndex="Description" Width="480" />
                                        <ext:Column Header="Data Source Name" DataIndex="DataSourceName" Width="300" />
                                        <ext:Column Header="Data Schema Name" DataIndex="DataSchemaName" Width="200" />
                                        <ext:Column Header="Phenomenon Name" DataIndex="PhenomenonName" Width="200" />
                                        <ext:CommandColumn Width="75">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="MasterRowSelect" Buffer="250" />
                                        </Listeners>
                                        <%-- 
                                        <DirectEvents>
                                            <RowSelect OnEvent="MasterRowSelect" />
                                        </DirectEvents>
                                        --%>
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
                            <ext:Panel ID="pnlInstruments" runat="server" Title="Instruments" ClientIDMode="Static" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:Button ID="btnInstrumentLinkAdd" runat="server" Icon="LinkAdd" Text="Link Instrument" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip4" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{SensorsGrid}') && #{SensorsGrid}.getSelectionModel().hasSelection()){#{InstrumentLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a sensor.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddInstrument" runat="server" Icon="Add" Text="Add Instrument">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddInstrumentClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="InstrumentLinksGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="InstrumentLinksGridStore" runat="server" OnRefreshData="InstrumentLinksGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="InstrumentID" Type="Auto" />
                                                            <ext:RecordField Name="InstrumentCode" Type="Auto" />
                                                            <ext:RecordField Name="InstrumentName" Type="Auto" />
                                                            <ext:RecordField Name="Latitude" Type="Auto" />
                                                            <ext:RecordField Name="Longitude" Type="Auto" />
                                                            <ext:RecordField Name="Elevation" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="SensorID" Value="Ext.getCmp('#{SensorsGrid}') && #{SensorsGrid}.getSelectionModel().hasSelection() ? #{SensorsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel4" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="InstrumentCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="InstrumentName" Width="300" />
                                                <ext:NumberColumn Header="Latitude" DataIndex="Latitude" Width="70" Groupable="false" Format="0.000000" />
                                                <ext:NumberColumn Header="Longitude" DataIndex="Longitude" Width="70" Groupable="false" Format="0.000000" />
                                                <ext:NumberColumn Header="Elevation" DataIndex="Elevation" Width="70" Groupable="false" Format="0.000" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="125" Format="dd MMM yyyy HH:mm" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="125" Format="dd MMM yyyy HH:mm" />
                                                <ext:CommandColumn Width="150">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                        <ext:GridCommand Icon="LinkDelete" CommandName="Delete" Text="Unlink" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <Listeners>
                                            <Command Fn="OnInstrumentLinkCommand" />
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
    <ext:Window ID="DetailWindow" runat="server" Width="800" Height="500" Closable="true"
        Hidden="true" Collapsible="false" Title="Sensor Detail" Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                runat="server" FieldLabel="Code" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Code is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur">
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
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel16" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="5000" runat="server"
                                FieldLabel="Description" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Description is a required" MsgTarget="Side">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel17" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server"
                                FieldLabel="Url" AnchorHorizontal="96%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <%--                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        >
                        <Items>
                            <ext:ComboBox ID="cbStation" runat="server" DataIndex="StationID" DisplayField="Name" ClientIDMode="Static"
                                TypeAhead="true" Mode="Local" ValueField="ID" FieldLabel="Station" EmptyText="Please select" AnchorHorizontal="96%"
                                TriggerAction="All" MsgTarget="Side" AllowBlank="false" BlankText="Station is required">
                                <Store>
                                    <ext:Store ID="storeStation" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
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
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>--%>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbPhenomenon" runat="server" DataIndex="PhenomenonID" DisplayField="Name" ClientIDMode="Static"
                                TypeAhead="true" Mode="Local" ValueField="ID" FieldLabel="Phenomenon" EmptyText="Please select" AnchorHorizontal="96%"
                                TriggerAction="All" MsgTarget="Side" AllowBlank="false" BlankText="Phenomenon is required">
                                <Store>
                                    <ext:Store ID="storePhenomenon" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
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
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbDataSource" runat="server" DataIndex="DataSourceID" DisplayField="Name"
                                TypeAhead="true" Mode="Local" ValueField="ID" FieldLabel="Data Source" EmptyText="Please select" AnchorHorizontal="96%"
                                TriggerAction="All" MsgTarget="Side" AllowBlank="false" BlankText="DataSource is required" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="storeDataSource" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Name" />
                                                    <ext:RecordField Name="DataSchemaID" Type="Auto" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <Select Fn="SelectDataSource" />
                                </Listeners>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:ComboBox ID="cbDataSchema" runat="server" Editable="true" ValueField="Id" DisplayField="Name"
                                TypeAhead="true" Mode="Local" AllowBlank="true" BlankText="Data Schema is required for the selected Data Source"
                                TriggerAction="All" DataIndex="DataSchemaID" EmptyText="Select Data Schema" SelectOnFocus="true" MsgTarget="Side"
                                FieldLabel="Data Schema" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="DataSchemaStore" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnValidate" runat="server" Text="Validate" Icon="Tick" ClientIDMode="Static">
                        <Listeners>
                            <Click Handler="GetInvalidFields(#{DetailsFormPanel});" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnSave" runat="server" Text="Save" FormBind="true" ClientIDMode="Static">
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
    <ext:Window ID="InstrumentLinkWindow" runat="server" Width="450" Height="275" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Instrument"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearInstrumentLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="InstrumentLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="InstrumentLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Instrument is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbInstrument" runat="server" StoreID="InstrumentStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Instrument"
                                AllowBlank="false" DataIndex="InstrumentID" EmptyText="Select Instrument"
                                SelectOnFocus="true" AnchorHorizontal="96%" ClientIDMode="Static">
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfInstrumentLatitude" DataIndex="Latitude" TrimTrailedZeros="false"
                                MaxLength="15" runat="server" FieldLabel="Latitude" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfInstrumentLongitude" DataIndex="Longitude" TrimTrailedZeros="false"
                                MaxLength="15" runat="server" FieldLabel="Longitude" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="3" ID="nfInstrumentElevation" DataIndex="Elevation" MaxLength="15" TrimTrailedZeros="false"
                                runat="server" FieldLabel="Elevation" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfInstrumentStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy H:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfInstrumentEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static" 
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy H:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnInstrumentLinkSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="InstrumentLinkSave">
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


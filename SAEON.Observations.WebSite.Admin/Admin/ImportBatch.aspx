﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportBatch.aspx.cs" Inherits="_ImportBatch"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/ImportBatch.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format)
        {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ImportBatchGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(ImportBatchGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);

            ImportBatchGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="5 5 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Import Batches" Layout="FitLayout" Hidden="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="Add" Text="New Import">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Handler="#{BasicForm}.getForm().reset();#{ErrorGrid}.getStore().removeAll();#{ImportWindow}.show();" />
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
                            <ext:GridPanel ID="ImportBatchGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="ImportBatchStore_RefreshData"
                                        OnSubmitData="ImportBatchStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Int" />
                                                    <ext:RecordField Name="ImportDate" Type="Date" />
                                                    <ext:RecordField Name="Status" Type="Int" />
                                                    <ext:RecordField Name="DataSourceName" Type="String" />
                                                    <ext:RecordField Name="StatusDescription" Type="String" />
                                                    <ext:RecordField Name="FileName" Type="String" />
                                                    <ext:RecordField Name="LogFileName" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="ImportDate" Direction="DESC" />
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Number" DataIndex="Id" Width="80" />
                                        <ext:DateColumn Header="Import Date" DataIndex="ImportDate" Width="150" Format="dd-MM-yyyy HH:mm:ss" />
                                        <ext:Column Header="DataSourceID" DataIndex="DataSourceID" Width="150" Hidden="true">
                                        </ext:Column>
                                        <ext:Column Header="DataSourceName" DataIndex="DataSourceName" Width="250" />
                                        <ext:Column Header="FileName" DataIndex="FileName" Width="200" />
                                        <ext:Column Header="LogFileName" DataIndex="LogFileName" Width="200" />
                                        <ext:Column Header="Status" DataIndex="StatusDescription" Width="150" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="Pencil" CommandName="Move" Text="" ToolTip-Text="Move Batch">
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Delete" CommandName="Delete" Text="" ToolTip-Text="Delete Batch">
                                                </ext:GridCommand>
                                            </Commands>
                                            <PrepareToolbar Fn="prepareToolbarCommand" />
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="ImportBatchRowSelect" Buffer="250" />
                                        </Listeners>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Id" />
                                            <ext:DateFilter DataIndex="ImportDate" />
                                            <ext:StringFilter DataIndex="DataSourceID" />
                                            <ext:StringFilter DataIndex="DataSourceName" />
                                            <ext:StringFilter DataIndex="Description" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <Command Fn="onBatchCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
                    <ext:Panel ID="pnlSouth" runat="server" Title="Data Log" Layout="FitLayout" Height="350"
                        ClientIDMode="Static">
                        <Items>
                            <ext:GridPanel ID="DSLogGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store4" runat="server" RemoteSort="true" OnRefreshData="DSLogGrid_RefreshData"  >
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Organisation" Type="String" />
                                                    <ext:RecordField Name="ProjectSite" Type="String" />
                                                    <ext:RecordField Name="StationName" Type="String" />
                                                    <ext:RecordField Name="ImportDate" Type="Date" />
                                                    <ext:RecordField Name="SensorName" Type="String" />
                                                    <ext:RecordField Name="SensorID" Type="Auto" />
                                                    <ext:RecordField Name="SensorInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="ValueDate" Type="Date" />
                                                    <ext:RecordField Name="InvalidDateValue" Type="String" />
                                                    <ext:RecordField Name="DateValueInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="ValueTime" Type="Date" UseNull="true" />
                                                    <ext:RecordField Name="InvalidTimeValue" Type="String" />
                                                    <ext:RecordField Name="TimeValueInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="RawValue" Type="Float" />
                                                    <ext:RecordField Name="ValueText" Type="String" />
                                                    <ext:RecordField Name="RawValueInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="DataValue" Type="Float" UseNull="true" />
                                                    <ext:RecordField Name="TransformValueText" Type="String" />
                                                    <ext:RecordField Name="DataValueInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="PhenomenonName" Type="String" />
                                                    <ext:RecordField Name="OfferingName" Type="String" />
                                                    <ext:RecordField Name="Unit" Type="String" />
                                                    <ext:RecordField Name="PhenomenonOfferingID" Type="Auto" />
                                                    <ext:RecordField Name="OfferingInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="PhenomenonUOMID" Type="Auto" />
                                                    <ext:RecordField Name="UOMInvalid" Type="Boolean" />
                                                    <ext:RecordField Name="RawFieldValue" Type="String" />
                                                    <ext:RecordField Name="Status" Type="String" />
                                                    <ext:RecordField Name="StatusID" Type="String" />
                                                    <ext:RecordField Name="DataSourceTransformationID" Type="Auto" />
                                                    <ext:RecordField Name="Transformation" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="ImportBatchID" Value="Ext.getCmp('#{ImportBatchGrid}') && #{ImportBatchGrid}.getSelectionModel().hasSelection() ? #{ImportBatchGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="Id" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <%--<ext:Column Header="Data" DataIndex="Data">
                                            <Renderer Fn="rendererData " />
                                        </ext:Column>--%>
                                        <ext:Column Header="Organisation" DataIndex="Organisation" Width="100" Hidden="true" />
                                        <ext:Column Header="ProjectSite" DataIndex="ProjectSite" Width="100" Hidden="true" />
                                        <ext:Column Header="Station" DataIndex="StationName" Width="100" Hidden="true" />
                                        <ext:DateColumn Header="Import Date" DataIndex="ImportDate" Width="100" Format="dd-MM-yyyy HH:mm:ss"
                                            Hidden="true" />
                                        <ext:Column Header="Sensor ID" DataIndex="SensorID" Width="40" Hidden="true">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidSensor" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid Sensor" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:Column>
                                        <ext:CheckColumn Locked="true" Header="Sensor Invalid" DataIndex="SensorInvalid"
                                            Width="30" Hidden="true" />
                                        <ext:Column Header="Sensor" DataIndex="SensorName" Width="150" />
                                        <ext:DateColumn Header="Date" DataIndex="ValueDate" Width="120" Format="dd-MM-yyyy HH:mm:ss">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidDate" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid Date" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:DateColumn>
                                        <ext:CheckColumn Header="Date Invalid" DataIndex="DateValueInvalid" Width="30" Hidden="true" />
                                        <ext:Column Header="Invalid Date" DataIndex="InvalidDateValue" Width="80" Hidden="true" />
                                        <ext:CheckColumn Locked="true" Header="Time Invalid" DataIndex="TimeValueInvalid"
                                            Width="30" Hidden="true" />
                                        <ext:DateColumn Header="Time Value" DataIndex="ValueTime" Width="80" Format="HH:mm:ss"
                                            Hidden="true" />
                                        <ext:Column Header="Invalid Time" DataIndex="InvalidTimeValue" Width="80" Hidden="true" />
                                        <ext:Column Header="Raw converted value" DataIndex="RawValue" Width="80" Hidden="true" />
                                        <ext:CheckColumn Locked="true" Header="Raw Value Invalid" DataIndex="RawValueInvalid"
                                            Width="30" Hidden="true" />
                                        <ext:Column Header="Invalid Raw value" DataIndex="ValueText" Width="150" Hidden="true" />
                                        <ext:Column Header="Raw Value" DataIndex="RawFieldValue" Width="90">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidRawValue" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid Raw Value" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:Column>
                                        <ext:CheckColumn Locked="true" Header="Data Value Invalid" DataIndex="DataValueInvalid"
                                            Width="30" Hidden="true" />
                                        <ext:Column Header="Invalid Data Value" DataIndex="TransformValueText" Width="150"
                                            Hidden="true" />
                                        <ext:Column Header="Data Value" DataIndex="DataValue" Width="90">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidDataValue" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid data Value (Transformed Value)" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:Column>
                                        <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="150" />
                                        <ext:Column Header="Offering ID" DataIndex="PhenomenonOfferingID" Width="150" Hidden="true" />
                                        <ext:CheckColumn Locked="true" Header="Offering Invalid" DataIndex="OfferingInvalid"
                                            Width="30" Hidden="true" />
                                        <ext:Column Header="Offering" DataIndex="OfferingName" Width="150">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidOffering" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid Offering" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:Column>
                                        <ext:Column Header="UOM ID" DataIndex="PhenomenonUOMID" Width="150" Hidden="true" />
                                        <ext:CheckColumn Locked="true" Header="UOM Invalid" DataIndex="UOMInvalid" Width="30"
                                            Hidden="true" />
                                        <ext:Column Header="Unit" DataIndex="Unit" Width="150">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="InvalidUOM" Hidden="true" HideMode="Display">
                                                    <ToolTip Text="Invalid Unit of measurement" />
                                                </ext:ImageCommand>
                                            </Commands>
                                            <PrepareCommand Fn="prepareCommand" />
                                        </ext:Column>
                                        <ext:Column Header="Status" DataIndex="Status" Width="150" />
                                        <ext:Column Header="Data Transformation ID" DataIndex="DataSourceTransformationID"
                                            Width="150" Hidden="true" />
                                        <ext:Column Header="Transformation" DataIndex="Transformation" Width="150" Hidden="true" />
                                        <ext:CommandColumn Width="75">
                                            
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                                <ext:GridCommand Icon="Add" CommandName="MoveToObservation" Text="" ToolTip-Text="Move to observation"  />
                                            </Commands>
                                            <PrepareToolbar Fn="prepareToolbarTransformation"/>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters2">
                                        <Filters>
                                            <ext:DateFilter DataIndex="ImportDate" />
                                            <ext:StringFilter DataIndex="Organisation" />
                                            <ext:StringFilter DataIndex="ProjectSite" />
                                            <ext:StringFilter DataIndex="ProjectSite" />
                                            <ext:StringFilter DataIndex="SensorName" />
                                            <ext:DateFilter DataIndex="ValueDate" />
                                            <ext:StringFilter DataIndex="RawFieldValue" />
                                            <ext:NumericFilter DataIndex="DataValue" />
                                            <ext:StringFilter DataIndex="PhenomenonName" />
                                            <ext:StringFilter DataIndex="OfferingName" />
                                            <ext:StringFilter DataIndex="Unit" />
                                            <ext:StringFilter DataIndex="Status" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <Command Fn="onLogCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="ImportWindow" runat="server" Title="Import Data File" Closable="true"
        Width="800" Height="500" Border="false" Collapsible="false" Maximizable="true"
        Hidden="true" ClientIDMode="Static">
        <%-- <Listeners>
            <Hide Fn="closepreview" />
        </Listeners>--%>
        <Items>
            <ext:BorderLayout ID="BorderLayout2" runat="server">
                <North MarginsSummary="5 5 5 5">
                    <ext:FormPanel ID="BasicForm" runat="server" AutoHeight="true" Frame="true" Title="Select File"
                        MonitorValid="true" PaddingSummary="10px 10px 0 10px" LabelWidth="90" ClientIDMode="Static">
                        <Defaults>
                            <ext:Parameter Name="anchor" Value="95%" Mode="Value" />
                            <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbDataSource" runat="server" DataIndex="DataSourceID" DisplayField="Name"
                                ValueField="ID" FieldLabel="Data Source" EmptyText="Please select" AnchorHorizontal="95%"
                                MsgTarget="Side" AllowBlank="false" BlankText="Data Source is required">
                                <Store>
                                    <ext:Store ID="storeDataSource" runat="server">
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
                            </ext:ComboBox>
                            <ext:FileUploadField ID="DataFileUpload" runat="server" EmptyText="Select a File"
                                AllowBlank="false" FieldLabel="Data File" ButtonText="" Icon="Zoom" BlankText="input file is required"
                                ClientIDMode="Static" />
                            <ext:FileUploadField ID="LogFileUpload" runat="server" AllowBlank="true" EmptyText="Select a log File"
                                FieldLabel="Log File" ButtonText="" Icon="Zoom" ClientIDMode="Static" />
                        </Items>
                        <Listeners>
                            <ClientValidation Handler="#{SaveButton}.setDisabled(!valid);" />
                        </Listeners>
                        <Buttons>
                            <ext:Button ID="SaveButton" runat="server" Text="Import file" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="UploadClick" Before="if (!#{BasicForm}.getForm().isValid()) { return false; } 
                                                    Ext.Msg.wait('Uploading and processing...', 'Processing');" Failure="Ext.Msg.show({ 
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
                                    <Click Handler="#{BasicForm}.getForm().reset();" />
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>
                </North>
                <Center MarginsSummary="0 5 0 5">
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
                </Center>
                <%-- <South MarginsSummary="0 0 5 5" Split="true">
                   <ext:GridPanel ID="DataLogGrid" runat="server" Title="Test Results" Layout="FitLayout"
                        ClientIDMode="Static">
                        <ColumnModel runat="server" ID="ColumnModel21">
                        </ColumnModel>
                        <Store>
                            <ext:Store ID="Store22" runat="server">
                            </ext:Store>
                        </Store>
                    </ext:GridPanel>
                </South>--%>
            </ext:BorderLayout>
        </Items>
    </ext:Window>
    <ext:Window ID="DetailWindow" runat="server" Width="500" Height="510" Closable="true"
        Hidden="true" Collapsible="false" Title="Datalog record Details" Maximizable="false"
        Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="500" Height="500" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Container ID="Container2" runat="server" LabelAlign="Top" Layout="FitLayout">
                        <Items>
                            <ext:ComboBox ID="cbSensor" runat="server" DataIndex="SensorID"
                                DisplayField="Name" ValueField="ID" FieldLabel="Sensor" EmptyText="Please select"
                                AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="false" BlankText="Station is required"
                                ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store3" runat="server">
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
                                <Listeners>
                                    <Select Fn="SelectSensor" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container1" runat="server" LabelAlign="Top" Layout="RowLayout">
                        <Items>
                            <ext:DateField ID="ValueDate" runat="server" DataIndex="ValueDate" FieldLabel="Date Value"
                                EmptyText="Please select" AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="false"
                                BlankText="Date Value is required" ClientIDMode="Static" />
                        </Items>
                    </ext:Container>
                    <ext:Container ID="TimeValueContainer" runat="server" LabelAlign="Top" Layout="RowLayout"
                        ClientIDMode="Static" HideMode="Offsets">
                        <Items>
                            <ext:TimeField ID="TimeValue" runat="server" DataIndex="ValueTime" FieldLabel="Time Value"
                                EmptyText="Please select" AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="false"
                                BlankText="Time is required" ClientIDMode="Static" Increment="1" />
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container3" runat="server" LabelAlign="Top" Layout="RowLayout">
                        <Items>
                            <ext:NumberField ID="RawValue" runat="server" DataIndex="RawValue" AllowDecimals="true"
                                DecimalPrecision="6" FieldLabel="Raw Value" AnchorHorizontal="95%" MsgTarget="Side"
                                AllowBlank="false" BlankText="Raw Value is required" ClientIDMode="Static" />
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" LabelAlign="Top" Layout="RowLayout">
                        <Items>
                            <ext:NumberField ID="DataValue" runat="server" DataIndex="DataValue" AllowDecimals="true"
                                DecimalPrecision="6" FieldLabel="Data Value" AnchorHorizontal="95%" MsgTarget="Side"
                                AllowBlank="false" BlankText="Data Value is required" ClientIDMode="Static" />
                        </Items>
                    </ext:Container>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="RowLayout"
                        LabelAlign="Top">
                        <Items>
                            <ext:ComboBox ID="cbOffering" runat="server" DataIndex="PhenomenonOfferingID" DisplayField="Name"
                                AllowBlank="false" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Offering" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store1" runat="server" OnRefreshData="cbOffering_RefreshData">
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
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="RowLayout"
                        LabelAlign="Top">
                        <Items>
                            <ext:ComboBox ID="cbUnitofMeasure" runat="server" DataIndex="PhenomenonUOMID" DisplayField="Unit"
                                AllowBlank="false" BlankText="Offering is required" MsgTarget="Side" ForceSelection="true"
                                ValueField="ID" FieldLabel="Unit of Measure" EmptyText="Please select" ValueNotFoundText="Please select"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="store5" runat="server" OnRefreshData="cbUnitofMeasure_RefreshData">
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
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="RowLayout"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextArea ID="tfComment" DataIndex="Description" runat="server" AllowBlank="true"
                                FieldLabel="Comment" AnchorHorizontal="95%">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSave" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="SaveObservation" Method="POST">
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
</asp:Content>
<%@ Page Title="Instruments" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Instruments.aspx.cs" Inherits="Admin_Instruments" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Instruments.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(InstrumentGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(InstrumentsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            InstrumentsGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Store ID="OrganisationStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="OrganisationRoleStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StationStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="SensorStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="DataSourceStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Viewport ID="ViewPort1" runat="server" Layout="FitLayout">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Instruments" Height="350" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Instrument" ClientIDMode="Static">
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
                            <ext:GridPanel ID="InstrumentsGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="InstrumentsGridStore" runat="server" RemoteSort="true" OnRefreshData="InstrumentsGridStore_RefreshData"
                                        OnSubmitData="InstrumentsGridStore_Submit">
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
                                                    <ext:RecordField Name="Latitude" Type="Auto" />
                                                    <ext:RecordField Name="Longitude" Type="Auto" />
                                                    <ext:RecordField Name="Elevation" Type="Auto" />
                                                    <ext:RecordField Name="StartDate" Type="Date" />
                                                    <ext:RecordField Name="EndDate" Type="Date" />
                                                    <ext:RecordField Name="LastUpdate" Type="Date" />
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
                                        <ext:NumberColumn Header="Latitude" DataIndex="Latitude" Width="70" Groupable="false" Format="0.000000" />
                                        <ext:NumberColumn Header="Longitude" DataIndex="Longitude" Width="70" Groupable="false" Format="0.000000" />
                                        <ext:NumberColumn Header="Elevation" DataIndex="Elevation" Width="70" Groupable="false" Format="0.000" />
                                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="200" />
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
                            <ext:Panel ID="pnlSensors" runat="server" Title="Sensors" ClientIDMode="Static" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="btnSensorLinkAdd" runat="server" Icon="LinkAdd" Text="Link Sensor" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection()){#{SensorLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select an instrument.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddSensor" runat="server" Icon="Add" Text="Add Sensor">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddSensorClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="SensorLinksGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="SensorLinksGridStore" runat="server" OnRefreshData="SensorLinksGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="SensorID" Type="Auto" />
                                                            <ext:RecordField Name="SensorCode" Type="Auto" />
                                                            <ext:RecordField Name="SensorName" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="InstrumentID" Value="Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection() ? #{InstrumentsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="SensorCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="SensorName" Width="500" />
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
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <Listeners>
                                            <Command Fn="OnSensorLinkCommand" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="pnlStations" runat="server" Title="Stations" Layout="FitLayout" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:Button ID="btnStationLinkAdd" runat="server" Icon="LinkAdd" Text="Link Station" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip4" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection()){#{StationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select an instrument.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddStation" runat="server" Icon="Add" Text="Add Station">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddStationClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="StationLinksGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="StationLinksGridStore" runat="server" OnRefreshData="StationLinksGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="StationID" Type="Auto" />
                                                            <ext:RecordField Name="StationCode" Type="Auto" />
                                                            <ext:RecordField Name="StationName" Type="Auto" />
                                                            <ext:RecordField Name="Latitude" Type="Auto" />
                                                            <ext:RecordField Name="Longitude" Type="Auto" />
                                                            <ext:RecordField Name="Elevation" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="InstrumentID" Value="Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection() ? #{InstrumentsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel4" runat="server" ClientIDMode="Static">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="StationCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="StationName" Width="500" />
                                                <ext:NumberColumn Header="Latitude" DataIndex="Latitude" Width="70" Format="0.000000" />
                                                <ext:NumberColumn Header="Longitude" DataIndex="Longitude" Width="70" Format="0.000000" />
                                                <ext:NumberColumn Header="Elevation" DataIndex="Elevation" Width="70" Format="0.000" />
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
                                            <Command Fn="OnStationLinkCommand" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="pnlOrganisations" runat="server" Title="Organisations" Layout="FitLayout" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                        <Items>
                                            <ext:Button ID="btnOrganisationLinkAdd" runat="server" Icon="LinkAdd" Text="Link Organisation" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip3" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection()){#{OrganisationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select an instrument.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddOrganisation" runat="server" Icon="Add" Text="Add Organisation">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip4" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddOrganisationClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="OrganisationLinksGrid" runat="server" Border="false" Layout="FitLayout"
                                        ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="OrganisationLinksGridStore" runat="server" OnRefreshData="OrganisationLinksGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="Level" Type="Auto" />
                                                            <ext:RecordField Name="LevelCode" Type="Auto" />
                                                            <ext:RecordField Name="LevelName" Type="Auto" />
                                                            <ext:RecordField Name="IsReadOnly" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationID" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationName" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationRoleID" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationRoleName" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="InstrumentID" Value="Ext.getCmp('#{InstrumentsGrid}') && #{InstrumentsGrid}.getSelectionModel().hasSelection() ? #{InstrumentsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel5" runat="server" ClientIDMode="Static">
                                            <Columns>
                                                <ext:Column Header="Level" DataIndex="Level" Width="100" />
                                                <ext:Column Header="Code" DataIndex="LevelCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="LevelName" Width="200" />
                                                <ext:Column Header="Organisation" DataIndex="OrganisationName" Width="300" />
                                                <ext:Column Header="Role" DataIndex="OrganisationRoleName" Width="100" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="75" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="75" Format="dd MMM yyyy" />
                                                <ext:CommandColumn Width="150">
                                                    <PrepareToolbar Fn="PrepareOrganisationLinkToolbar" />
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                        <ext:GridCommand Icon="LinkDelete" CommandName="Delete" Text="Unlink" />
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
                                            <Command Fn="OnOrganisationLinkCommand" />
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
    <ext:Window ID="DetailWindow" runat="server" Width="800" Height="450" Closable="true"
        Hidden="true" Collapsible="false" Title="Instrument Detail" Maximizable="false"
        Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true" LabelAlign="Top"
                Padding="10" ButtonAlign="Right" MonitorResize="true" Layout="FitLayout" ClientIDMode="Static">
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
                    <ext:Container ID="Container4" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container8" runat="server" Layout="Form" ColumnWidth=".32">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLatitude" DataIndex="Latitude" TrimTrailedZeros="false"
                                        MaxLength="15" runat="server" FieldLabel="Latitude" AnchorHorizontal="98%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container9" runat="server" Layout="Form" ColumnWidth=".32">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLongitude" DataIndex="Longitude" TrimTrailedZeros="false"
                                        MaxLength="15" runat="server" FieldLabel="Longitude" AnchorHorizontal="98%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container10" runat="server" Layout="Form" ColumnWidth=".32">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="3" ID="nfElevation" DataIndex="Elevation" MaxLength="15" TrimTrailedZeros="false"
                                        runat="server" FieldLabel="Elevation" AnchorHorizontal="100%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container13" runat="server" Layout="Form" ColumnWidth=".04">
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
                                        Format="dd MMM yyyy" ClientIDMode="Static" IsRemoteValidation="true">
                                        <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container2" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:DateField ID="dfEndDate" DataIndex="EndDate" runat="server" FieldLabel="End Date" ColumnWidth=".5" AnchorHorizontal="100%"
                                        Format="dd MMM yyyy" ClientIDMode="Static" IsRemoteValidation="true">
                                        <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container12" runat="server" Layout="Form" ColumnWidth=".04">
                                <Items>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
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
    <ext:Window ID="OrganisationLinkWindow" runat="server" Width="400" Height="310" Closable="true" Hidden="true" Collapsible="false" Title="Link Organisation"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearOrganisationLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="OrganisationLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="OrganisationLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:ComboBox ID="cbOrganisation" runat="server" StoreID="OrganisationStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Organisation"
                                AllowBlank="false" DataIndex="OrganisationID" EmptyText="Select Organisation"
                                SelectOnFocus="true" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel14" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:ComboBox ID="cbOrganisationRole" runat="server" StoreID="OrganisationRoleStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Role"
                                AllowBlank="false" DataIndex="OrganisationRoleID" EmptyText="Select Role"
                                SelectOnFocus="true" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel12" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfOrganisationStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel13" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfOrganisationEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnOrganisationLinkSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="OrganisationLinkSave">
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
    <ext:Window ID="StationLinkWindow" runat="server" Width="800" Height="400" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Station" Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearStationLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="StationLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="StationLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:ComboBox ID="cbStation" runat="server" StoreID="StationStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Station"
                                AllowBlank="false" DataIndex="StationID" EmptyText="Select Station"
                                SelectOnFocus="true" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfStationLatitude" DataIndex="Latitude" TrimTrailedZeros="false"
                                MaxLength="15" runat="server" FieldLabel="Latitude" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfStationLongitude" DataIndex="Longitude" TrimTrailedZeros="false"
                                MaxLength="15" runat="server" FieldLabel="Longitude" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:NumberField AllowDecimals="true" DecimalPrecision="3" ID="nfStationElevation" DataIndex="Elevation" MaxLength="15" TrimTrailedZeros="false"
                                runat="server" FieldLabel="Elevation" AnchorHorizontal="96%">
                            </ext:NumberField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel18" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfStationStartDate" DataIndex="StartDate" MaxLength="125" runat="server" ClientIDMode="Static" 
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy HH:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfStationEndDate" DataIndex="EndDate" MaxLength="125" runat="server" ClientIDMode="Static" 
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy HH:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnStationLinkSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="StationLinkSave">
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
    <ext:Window ID="SensorLinkWindow" runat="server" Width="450" Height="275" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Sensor"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearSensorLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="SensorLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="SensorLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:ComboBox ID="cbSensor" runat="server" StoreID="SensorStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Sensor"
                                AllowBlank="false" DataIndex="SensorID" EmptyText="Select Sensor"
                                SelectOnFocus="true" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfSensorStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy HH:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel11" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:DateField ID="dfSensorEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy HH:mm" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSensorLinkSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="SensorLinkSave">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar4" runat="server" Height="25">
                        <Plugins>
                            <%--                            <ext:ValidationStatus
                                runat="server"
                                FormPanelID="SensorLinkFormPanel"
                                ValidIcon="Accept"
                                ErrorIcon="Exclamation" />--%>
                        </Plugins>
                    </ext:StatusBar>
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
</asp:Content>


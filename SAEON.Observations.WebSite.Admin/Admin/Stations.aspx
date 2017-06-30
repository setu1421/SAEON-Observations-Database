<%@ Page Title="Stations" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Stations.aspx.cs" Inherits="Admin_Stations" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Stations.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_StationsGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(StationsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            StationsGrid.submitData(false);
        };
    </script>
</asp:Content>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Store ID="SiteStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
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
    <ext:Store ID="ProjectStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="Auto" />
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
    <ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="5 0 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Stations" Layout="FitLayout" Hidden="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Station" ClientIDMode="Static">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Fn="New" />
                                        </Listeners>
                                        <DirectEvents>
                                            <Click OnEvent="AddStationClick" />
                                        </DirectEvents>
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
                            <ext:GridPanel ID="StationsGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="StationsGridStore" runat="server" RemoteSort="true" OnRefreshData="StationsGridStore_RefreshData" OnSubmitData="StationsGridStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                    <ext:RecordField Name="SiteID" Type="Auto" />
                                                    <ext:RecordField Name="SiteName" Type="String" />
                                                    <ext:RecordField Name="Description" Type="String" />
                                                    <ext:RecordField Name="Url" Type="String" />
                                                    <ext:RecordField Name="Latitude" Type="Auto" />
                                                    <ext:RecordField Name="Longitude" Type="Auto" />
                                                    <ext:RecordField Name="Elevation" Type="Auto" />
                                                    <ext:RecordField Name="StartDate" Type="Date" />
                                                    <ext:RecordField Name="EndDate" Type="Date" />
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
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" Groupable="false" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="200" Groupable="false" />
                                        <ext:Column Header="Site" DataIndex="SiteName" Width="200" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="200" Groupable="false" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="150" Groupable="false" />
                                        <ext:Column Header="Latitude" DataIndex="Latitude" Width="70" Groupable="false" />
                                        <ext:Column Header="Longitude" DataIndex="Longitude" Width="70" Groupable="false" />
                                        <ext:Column Header="Elevation" DataIndex="Elevation" Width="70" Groupable="false" />
                                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:CommandColumn Width="75" Groupable="false">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                <%--                                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />--%>
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="MasterRowSelect" Buffer="250" />
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
                                            <ext:StringFilter DataIndex="Description" />
                                            <ext:StringFilter DataIndex="Url" />
                                            <ext:NumericFilter DataIndex="Latitude" />
                                            <ext:NumericFilter DataIndex="Longitude" />
                                            <ext:NumericFilter DataIndex="Elevation" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <%--<Command Handler="#{DetailsFormPanel}.getForm().reset();#{DetailsFormPanel}.getForm().loadRecord(record);#{DetailsFormPanel}.clearInvalid();#{DetailWindow}.show()" />--%>
                                    <Command Fn="onCommand" />
                                </Listeners>
                                <DirectEvents>
                                    <Command OnEvent="StationGridCommand" />
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <South Collapsible="true" Split="true" MinHeight="250">
                    <ext:TabPanel ID="pnlSouth" runat="server" Height="250" TabPosition="Top" Border="false" ClientIDMode="Static">
                        <Items>
                            <ext:Panel ID="pnlInstruments" runat="server" Title="Instruments" Layout="FitLayout"
                                Height="200" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="btnInstrumentLinkAdd" runat="server" Icon="LinkAdd" Text="Link Instrument" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection()){#{InstrumentLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a station.')}" />
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
                                                    <ext:Parameter Name="StationID" Value="Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection() ? #{StationsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="InstrumentCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="InstrumentName" Width="200" />
                                                <ext:Column Header="Latitude" DataIndex="Latitude" Width="70" />
                                                <ext:Column Header="Longitude" DataIndex="Longitude" Width="70" />
                                                <ext:Column Header="Elevation" DataIndex="Elevation" Width="70" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
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
                                            <Command Fn="OnInstrumentLinkCommand" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="pnlProjects" runat="server" Title="Projects" Layout="FitLayout" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:Button ID="btnProjectLinkAdd" runat="server" Icon="LinkAdd" Text="Link Project" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip6" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection()){#{ProjectLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a station.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%-- 
                                            <ext:Button ID="btnAddProject" runat="server" Icon="Add" Text="Add Project">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip7" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddProjectClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="ProjectLinksGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="ProjectLinksGridStore" runat="server" OnRefreshData="ProjectLinksGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="ProjectID" Type="Auto" />
                                                            <ext:RecordField Name="ProjectCode" Type="Auto" />
                                                            <ext:RecordField Name="ProjectName" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="StationID" Value="Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection() ? #{StationsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel3" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="ProjectCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="ProjectName" Width="200" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
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
                                            <Command Fn="OnProjectLinkCommand" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="pnlOrganisations" runat="server" Title="Organisations" Layout="FitLayout" Width="425" ClientIDMode="Static">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                        <Items>
                                            <ext:Button ID="btnOrganisationLinkAdd" runat="server" Icon="LinkAdd" Text="Link Organisation" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip3" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection()){#{OrganisationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a station.')}" />
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
                                                    <ext:Parameter Name="StationID" Value="Ext.getCmp('#{StationsGrid}') && #{StationsGrid}.getSelectionModel().hasSelection() ? #{StationsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel5" runat="server">
                                            <Columns>
                                                <ext:Column Header="Level" DataIndex="Level" Width="100" />
                                                <ext:Column Header="Code" DataIndex="LevelCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="LevelName" Width="200" />
                                                <ext:Column Header="Organisation" DataIndex="OrganisationName" Width="200" />
                                                <ext:Column Header="Role" DataIndex="OrganisationRoleName" Width="200" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="yyyy/MM/dd" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="yyyy/MM/dd" />
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
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="800" Height="450" Closable="true"
        Hidden="true" Collapsible="false" Title="Station Detail" Maximizable="false"
        Layout="FitLayout" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                Padding="10" ButtonAlign="Right" MonitorResize="true" Layout="FitLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Container ID="Container1" runat="server" Layout="Column" Height="100">
                        <Items>
                            <ext:Container ID="Container2" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfCode" AllowBlank="false" DataIndex="Code" IsRemoteValidation="true"
                                        MaxLength="50" runat="server" FieldLabel="Code" BlankText="Code is a required"
                                        MsgTarget="Side" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbSite" runat="server" StoreID="SiteStore" Editable="true"
                                        BlankText="Site is required" MsgTarget="Side" DisplayField="Name" ValueField="Id"
                                        TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false"
                                        DataIndex="SiteID" EmptyText="Select Site" SelectOnFocus="true"
                                        FieldLabel="Site" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="this.clearValue();this.focus();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container3" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfName" AllowBlank="false" DataIndex="Name" IsRemoteValidation="true"
                                        MaxLength="150" runat="server" FieldLabel="Name" AnchorHorizontal="95%" MsgTarget="Side"
                                        BlankText="Name is a required" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server" FieldLabel="Url"
                                        AnchorHorizontal="95%" MsgTarget="Side" ClientIDMode="Static">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Panel4" runat="server" Layout="Form" LabelAlign="Top">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server" IsRemoteValidation="true"
                                AllowBlank="false" BlankText="Description is required" MsgTarget="Side" FieldLabel="Description"
                                AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextArea>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" Layout="Column" Height="50" Margins="10px 0px 0px 0px">
                        <Items>
                            <ext:Container ID="Container8" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLatitude" DataIndex="Latitude"
                                        MaxLength="150" runat="server" FieldLabel="Latitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container9" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLongitude" DataIndex="Longitude"
                                        MaxLength="150" runat="server" FieldLabel="Longitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container10" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="2" ID="nfElevation" DataIndex="Elevation" MaxLength="7"
                                        runat="server" FieldLabel="Elevation" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfStartDate" DataIndex="StartDate" MaxLength="100" runat="server"
                                FieldLabel="Start Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="End Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfEndDate" DataIndex="EndDate" MaxLength="100" runat="server"
                                FieldLabel="End Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
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
    <ext:Window ID="OrganisationLinkWindow" runat="server" Width="400" Height="310" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Organisation"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearOrganisationLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="OrganisationLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="OrganisationLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="FormLayout"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Organisation is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbOrganisation" runat="server" StoreID="OrganisationStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Organisation"
                                AllowBlank="false" DataIndex="OrganisationID" EmptyText="Select Organisation"
                                SelectOnFocus="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel14" runat="server" Border="false" Header="false" Layout="FormLayout"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Role is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbOrganisationRole" runat="server" StoreID="OrganisationRoleStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Role"
                                AllowBlank="false" DataIndex="OrganisationRoleID" EmptyText="Select Role"
                                SelectOnFocus="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel12" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfOrganisationStartDate" DataIndex="StartDate" MaxLength="100" runat="server"
                                FieldLabel="Start Date" AnchorHorizontal="95%" Format="dd MMM yyyy" ClientIDMode="Static">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel13" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="End Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfOrganisationEndDate" DataIndex="EndDate" MaxLength="100" runat="server"
                                FieldLabel="End Date" AnchorHorizontal="95%" Format="dd MMM yyyy" ClientIDMode="Static">
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
    <ext:Window ID="ProjectLinkWindow" runat="server" Width="450" Height="260" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Project"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearProjectLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="ProjectLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="370" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="ProjectLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Layout="FormLayout"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Project is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbProject" runat="server" StoreID="ProjectStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Project"
                                AllowBlank="false" DataIndex="ProjectID" EmptyText="Select Project"
                                SelectOnFocus="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel16" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfProjectStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel17" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="End Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfProjectEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="End Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnProjectLinkSave" runat="server" Text="Save" FormBind="true" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="ProjectLinkSave">
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
    <ext:Window ID="InstrumentLinkWindow" runat="server" Width="450" Height="310" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Instrument"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearInstrumentLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="InstrumentLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="250" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="InstrumentLinkID" DataIndex="Id" runat="server" ClientIDMode="Static" />
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="FormLayout"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Instrument is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:ComboBox ID="cbInstrument" runat="server" StoreID="InstrumentStore" Editable="true" DisplayField="Name"
                                ValueField="Id" TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" FieldLabel="Instrument"
                                AllowBlank="false" DataIndex="InstrumentID" EmptyText="Select Instrument"
                                SelectOnFocus="true" AnchorHorizontal="95%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container5" runat="server" Layout="Column" Height="50" Margins="10px 0px 0px 0px">
                        <Items>
                            <ext:Container ID="Container6" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfInstrumentLatitude" DataIndex="Latitude"
                                        MaxLength="150" runat="server" FieldLabel="Latitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container7" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfInstrumentLongitude" DataIndex="Longitude"
                                        MaxLength="150" runat="server" FieldLabel="Longitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container11" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="2" ID="nfInstrumentElevation" DataIndex="Elevation" MaxLength="7"
                                        runat="server" FieldLabel="Elevation" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfInstrumentStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="FormLayout" LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="End Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfInstrumentEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="End Date" AnchorHorizontal="95%" Format="dd MMM yyyy">
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


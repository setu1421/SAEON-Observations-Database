<%@ Page Title="Sites" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Sites.aspx.cs" Inherits="Admin_Sites" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Sites.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_SitesGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(SitesGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            SitesGrid.submitData(false);
        };
    </script>
</asp:Content>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
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
    <ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Sites" Height="350" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Site" ClientIDMode="Static">
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
                            <ext:GridPanel ID="SitesGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="SitesGridStore" runat="server" RemoteSort="true" OnRefreshData="SitesGridStore_RefreshData" OnSubmitData="SitesGridStore_Submit">
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
                                        <ext:CommandColumn Width="75">
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
                            <ext:Panel ID="pnlStations" runat="server" Title="Stations" ClientIDMode="Static" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
<%--                                            <ext:Button ID="btnStationLinksAdd" runat="server" Icon="LinkAdd" Text="Link Stations" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection()){#{StationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a site.')}" />
                                                    <Click Handler="if(Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection()){#{AvailableStationsGridStore}.reload();#{AvailableStationsWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a site.')}" />
                                                </Listeners>
                                            </ext:Button>--%>
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
                                                            <ext:RecordField Name="Code" Type="Auto" />
                                                            <ext:RecordField Name="Name" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="SiteID" Value="Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection() ? #{SitesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                                <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
<%--                                                <ext:CommandColumn Width="75">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                                        <ext:GridCommand Icon="LinkDelete" CommandName="Delete" Text="Unlink" />
                                                    </Commands>
                                                </ext:CommandColumn>--%>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
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
                                                    <Click Handler="if(Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection()){#{OrganisationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a site.')}" />
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
                                    <ext:GridPanel ID="OrganisationLinksGrid" runat="server" Border="false" ClientIDMode="Static">
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
                                                            <ext:RecordField Name="OrganisationCode" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationName" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationRoleID" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationRoleCode" Type="Auto" />
                                                            <ext:RecordField Name="OrganisationRoleName" Type="Auto" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="SiteID" Value="Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection() ? #{SitesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel5" runat="server">
                                            <Columns>
                                                <ext:Column Header="Level" DataIndex="Level" Width="100" />
                                                <ext:Column Header="Code" DataIndex="LevelCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="LevelName" Width="300" />
                                                <ext:Column Header="Organisation" DataIndex="OrganisationName" Width="300" />
                                                <ext:Column Header="Role" DataIndex="OrganisationRoleName" Width="200" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
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
    <ext:Window ID="DetailWindow" runat="server" Width="800" Height="450" Closable="true" Hidden="true" Collapsible="false" Title="Site Detail"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="FormLayout">
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
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true" 
                                runat="server" FieldLabel="Name" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Name is a required" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="150" runat="server" 
                                FieldLabel="Description" AnchorHorizontal="96%" ClientIDMode="Static" 
                                AllowBlank="false" BlankText="Description is a required" MsgTarget="Side">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server"
                                FieldLabel="Url" AnchorHorizontal="96%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
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
                            <ext:Container ID="Container2" runat="server" Layout="Form" ColumnWidth=".04">
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
    <ext:Window ID="OrganisationLinkWindow" runat="server" Width="400" Height="310" Closable="true"
        Hidden="true" Collapsible="false" Title="Link Organisation"
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
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Organisation is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
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
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Role is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
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
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfOrganisationStartDate" DataIndex="StartDate" MaxLength="100" runat="server"
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy" ClientIDMode="Static">
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel13" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="End Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfOrganisationEndDate" DataIndex="EndDate" MaxLength="100" runat="server"
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy" ClientIDMode="Static">
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
    <ext:Window ID="AvailableStationsWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Stations" Width="750" Height="300" X="50" Y="50" Layout="Fit" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableStations" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="AvailableStationsGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="AvailableStationsGridStore" runat="server" OnRefreshData="AvailableStationsGridStore_RefreshData">
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
                                    <ext:RecordField Name="EndDate" Type="Date" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <BaseParams>
                            <ext:Parameter Name="SiteID" Value="Ext.getCmp('#{SitesGrid}') && #{SitesGrid}.getSelectionModel().hasSelection() ? #{SitesGrid}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel4" runat="server">
                    <Columns>
                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                        <ext:Column Header="Name" DataIndex="Name" Width="200" />
                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                        <ext:DateColumn Header="Date End" DataIndex="DateEnd" Width="100" Format="dd MMM yyyy" />
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="btnStationLinksSave" runat="server" Text="Save" Icon="Accept" ClientIDMode="Static">
                        <DirectEvents>
                            <Click OnEvent="StationLinksSave">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Items>
    </ext:Window>
</asp:Content>


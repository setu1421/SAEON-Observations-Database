﻿<%@ Page Title="Projects" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Projects.aspx.cs" Inherits="Admin_Projects" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Projects.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_ProjectsGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(ProjectsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            ProjectsGrid.submitData(false);
        };
    </script>
</asp:Content>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Store ID="ProgrammeStore" runat="server">
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
                    <ext:Panel ID="pnlNorth" runat="server" Title="Projects" Height="350" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnAdd" runat="server" Icon="Add" Text="Add Project" ClientIDMode="Static">
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
                            <ext:GridPanel ID="ProjectsGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="ProjectsGridStore" runat="server" RemoteSort="true" OnRefreshData="ProjectsGridStore_RefreshData" OnSubmitData="ProjectsGridStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                    <ext:RecordField Name="ProgrammeID" Type="Auto" />
                                                    <ext:RecordField Name="ProgrammeName" Type="String" />
                                                    <ext:RecordField Name="Description" Type="String" />
                                                    <ext:RecordField Name="Url" Type="String" />
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
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="500" />
                                        <ext:Column Header="Programme" DataIndex="ProgrammeName" Width="300" />
                                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="100" Format="dd MMM yyyy" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="300" />
                                        <ext:CommandColumn Width="75">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                <%--                                                
                                                <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                                --%>
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
                                            <ext:Button ID="btnStationLinkAdd" runat="server" Icon="LinkAdd" Text="Link Station" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Link" />
                                                </ToolTips>
                                                <Listeners>
                                                    <Click Handler="if(Ext.getCmp('#{ProjectsGrid}') && #{ProjectsGrid}.getSelectionModel().hasSelection()){#{StationLinkWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a project.')}" />
                                                </Listeners>
                                            </ext:Button>
                                            <%--
                                            <ext:Button ID="btnAddStation" runat="server" Icon="Add" Text="Add Station">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip3" runat="server" Html="Add" />
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
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="ProjectID" Value="Ext.getCmp('#{ProjectsGrid}') && #{ProjectsGrid}.getSelectionModel().hasSelection() ? #{ProjectsGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                </BaseParams>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="Code" DataIndex="StationCode" Width="200" />
                                                <ext:Column Header="Name" DataIndex="StationName" Width="300" />
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
                                            <Command Fn="OnStationLinkCommand" />
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
        Hidden="true" Collapsible="false" Title="Project Detail"
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
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="5000" runat="server"
                                FieldLabel="Description" AnchorHorizontal="96%" ClientIDMode="Static"
                                AllowBlank="false" BlankText="Description is a required" MsgTarget="Side">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:ComboBox ID="cbProgramme" runat="server" StoreID="ProgrammeStore" Editable="true"
                                BlankText="Programme is required" MsgTarget="Side" DisplayField="Name" ValueField="Id"
                                TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false"
                                DataIndex="ProgrammeID" EmptyText="Select Programme" SelectOnFocus="true"
                                FieldLabel="Programme" AnchorHorizontal="96%" ClientIDMode="Static">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server" ClientIDMode="Static"
                                FieldLabel="Url" AnchorHorizontal="96%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Container ID="Container1" runat="server" Layout="ColumnLayout" Height="50">
                        <Items>
                            <ext:Container ID="Container3" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:DateField ID="dfStartDate" DataIndex="StartDate" runat="server" FieldLabel="Start Date" AnchorHorizontal="98%"
                                        BlankText="Start Date is required." Format="dd MMM yyyy" ClientIDMode="Static" IsRemoteValidation="true">
                                        <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
                                    </ext:DateField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container5" runat="server" Layout="Form" ColumnWidth=".48">
                                <Items>
                                    <ext:DateField ID="dfEndDate" DataIndex="EndDate" runat="server" FieldLabel="End Date" AnchorHorizontal="100%"
                                        BlankText="End Date is required." Format="dd MMM yyyy" ClientIDMode="Static" IsRemoteValidation="true">
                                        <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
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
    <ext:Window ID="StationLinkWindow" runat="server" Width="450" Height="260" Closable="true" Hidden="true" Collapsible="false" Title="Link Station" LabelAlign="Top"
        Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="ClearStationLinkForm" />
        </Listeners>
        <Content>
            <ext:FormPanel ID="StationLinkFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="370" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="ProjectStationLinkID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Station is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
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
                    <ext:Panel ID="Panel12" runat="server" Border="false" Header="false" Layout="FormLayout">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Start Date is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:DateField ID="dfStationStartDate" DataIndex="StartDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="Start Date" AnchorHorizontal="96%" Format="dd MMM yyyy" IsRemoteValidation="true">
                                <RemoteValidation OnValidation="ValidateField" ValidationEvent="blur" />
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
                            <ext:DateField ID="dfStationEndDate" DataIndex="EndDate" MaxLength="100" runat="server" ClientIDMode="Static"
                                FieldLabel="End Date" AnchorHorizontal="96%" Format="dd MMM yyyy" IsRemoteValidation="true">
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
                    <ext:StatusBar ID="StatusBar2" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
</asp:Content>


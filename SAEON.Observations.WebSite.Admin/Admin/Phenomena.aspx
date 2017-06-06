<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Phenomena.aspx.cs" Inherits="_Phenomena"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Phenomena.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_PhenomenaGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(PhenomenaGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            PhenomenaGrid.submitData(false);
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
                <Center MarginsSummary="5 0 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Phenomenons" Layout="Fit">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="Add" Text="Add Phenomenon">
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
                            <ext:GridPanel ID="PhenomenaGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="PhenomenaGridStore" runat="server" RemoteSort="true" OnRefreshData="PhenomenaGridStore_RefreshData"
                                        OnSubmitData="PhenomenaGridStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                    <ext:RecordField Name="Url" Type="String" />
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
                                        <DirectEventConfig IsUpload="true" />
                                        <SortInfo Field="Name" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Code" DataIndex="Code" Width="150" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="150" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="200" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="200" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                                <ext:GridCommand Icon="NoteDelete" CommandName="RemovePhenomenon" Text="" ToolTip-Text="Delete" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Fn="PhenomenaRowSelect" Buffer="250" />
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
                                    <Command OnEvent="DoDelete">
                                        <ExtraParams>
                                            <ext:Parameter Name="type" Value="params[0]" Mode="Raw" />
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <East Collapsible="true" Split="true" MarginsSummary="5 5 0 0">
                    <ext:Panel ID="pnlEast" runat="server" Title="Offerings" Layout="Fit" Width="320"
                        ClientIDMode="Static">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:Button ID="AddOffering" runat="server" Icon="Add" Text="Add Offering">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip3" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Handler="if(Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection()){#{OfferingGridStore}.reload();#{AvailableOfferingsWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a phenomenon.')}" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="PhenomenonOfferingGrid" runat="server" Border="false" Layout="FitLayout"
                                ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store1" runat="server" OnRefreshData="PhenomenonOfferingGrid_RefreshData">
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
                                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel5" runat="server">
                                    <Columns>
                                        <ext:Column Header="Code" DataIndex="Code" Width="100" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="100" />
                                        <ext:Column Header="Description" DataIndex="Description" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteDelete" CommandName="RemoveOffering" Text="" ToolTip-Text="Delete" />
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
                                </Listeners>
                                <DirectEvents>
                                    <Command OnEvent="DoDelete">
                                        <ExtraParams>
                                            <ext:Parameter Name="type" Value="params[0]" Mode="Raw" />
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </East>
                <South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
                    <ext:Panel ID="pnlSouth" runat="server" Title="Unit of Measurements" Height="200"
                        Layout="Fit" ClientIDMode="Static">
                        <TopBar>
                            <ext:Toolbar ID="SouthToolbar" runat="server">
                                <Items>
                                    <ext:Button ID="AddUnitButton" runat="server" Icon="Add" Text="Add Unit">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip2" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Handler="if(Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection()){#{Store3}.reload();#{AvailableUnitsWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a phenomenon.')}" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="PhenomenonUOMGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store4" runat="server" OnRefreshData="PhenomenonUOMGrid_RefreshData">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Unit" Type="String" />
                                                    <ext:RecordField Name="UnitSymbol" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                        <ext:Column Header="Unit" DataIndex="Unit" Width="200" />
                                        <ext:Column Header="Symbol" DataIndex="UnitSymbol" Width="200" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteDelete" CommandName="RemoveUOM" Text="" ToolTip-Text="Delete" />
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
                                </Listeners>
                                <DirectEvents>
                                    <Command OnEvent="DoDelete">
                                        <ExtraParams>
                                            <ext:Parameter Name="type" Value="params[0]" Mode="Raw" />
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="450" Height="400" Closable="true"
        Hidden="true" Collapsible="false" Title="Phenomenon Detail" Maximizable="false"
        Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="370" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Value" />
                            <ext:Parameter Name="blankText" Value="Code is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                runat="server" FieldLabel="Code" AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Name is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                runat="server" FieldLabel="Name" AnchorHorizontal="95%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server" FieldLabel="Url"
                                AnchorHorizontal="95%">
                                <ToolTips>
                                    <ext:ToolTip Html="Lookup latest SensorWeb ID" ID="rfUrlToolip" runat="server">
                                    </ext:ToolTip>
                                </ToolTips>
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Description is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server" FieldLabel="Description"
                                AnchorHorizontal="95%">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
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
    <ext:Window ID="AvailableUnitsWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Units" Width="620" Height="300" X="50" Y="50" Layout="FitLayout" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableUnit" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="UnitOfMeasureGrid" runat="server" Header="false" Border="false"
                ClientIDMode="Static">
                <Store>
                    <ext:Store ID="Store3" runat="server" OnRefreshData="UnitOfMeasureStore_RefreshData">
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <Reader>
                            <ext:JsonReader IDProperty="Id">
                                <Fields>
                                    <ext:RecordField Name="Id" Type="Auto" />
                                    <ext:RecordField Name="Code" Type="String" />
                                    <ext:RecordField Name="Unit" Type="String" />
                                    <ext:RecordField Name="UnitSymbol" Type="String" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <BaseParams>
                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                        <ext:Column Header="Unit" DataIndex="Unit" Width="200" />
                        <ext:Column Header="Symbol" DataIndex="UnitSymbol" Width="200" />
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="AcceptUOM" runat="server" Text="Save" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="AcceptUOM_Click">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="AvailableOfferingsWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Offerings" Width="620" Height="300" X="50" Y="50" 
        Layout="FitLayout" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableOffering" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="OfferingGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="OfferingGridStore" runat="server" OnRefreshData="OfferingGridStore_RefreshData">
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
                            <ext:Parameter Name="PhenomenonID" Value="Ext.getCmp('#{PhenomenaGrid}') && #{PhenomenaGrid}.getSelectionModel().hasSelection() ? #{PhenomenaGrid}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel4" runat="server">
                    <Columns>
                        <ext:Column Header="Code" DataIndex="Code" Width="150" />
                        <ext:Column Header="Name" DataIndex="Name" Width="150" />
                        <ext:Column Header="Description" DataIndex="Description" Width="200" />
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="AcceptOfferingButton" runat="server" Text="Save" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="AcceptOfferingButton_Click">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Items>
    </ext:Window>
</asp:Content>

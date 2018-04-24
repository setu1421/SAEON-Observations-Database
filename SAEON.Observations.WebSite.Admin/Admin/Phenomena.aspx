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
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Phenomenona" Height="350" Layout="FitLayout">
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
                                        <SortInfo Field="Name" Direction="ASC" />
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="500" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="200" />
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
                                            <ext:StringFilter DataIndex="Description" />
                                            <ext:StringFilter DataIndex="Url" />
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
                </North>
                <Center>
                    <ext:TabPanel ID="tpCenter" runat="server" TabPosition="Top" Border="false" ClientIDMode="Static">
                        <Items>
                            <ext:Panel ID="pnlOfferings" runat="server" Title="Offerings" ClientIDMode="Static" Layout="FitLayout">
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
                                            <ext:Store ID="PhenomenonOfferingGridStore" runat="server" OnRefreshData="PhenomenonOfferingGrid_RefreshData">
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
                                                <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                                <ext:Column Header="Name" DataIndex="Name" Width="300" />
                                                <ext:Column Header="Description" DataIndex="Description" Width="500" />
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
                            <ext:Panel ID="pnlUnitsOfMeasure" runat="server" Title="Units of Measure" ClientIDMode="Static" Layout="FitLayout">
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
                                            <ext:Store ID="PhenomenonUOMGridStore" runat="server" OnRefreshData="PhenomenonUOMGrid_RefreshData">
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
                                                <ext:Column Header="Unit" DataIndex="Unit" Width="300" />
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
                        </Items>
                    </ext:TabPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="450" Height="350" Closable="true"
        Hidden="true" Collapsible="false" Title="Phenomenon Detail" Maximizable="false" Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="370" ButtonAlign="Right" LabelAlign="Top" Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server" ClientIDMode="Static">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50" AllowBlank="false" BlankText="Code is a required" MsgTarget="Side"
                                runat="server" FieldLabel="Code" AnchorHorizontal="96%" ClientIDMode="Static">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true" AllowBlank="false" BlankText="Name is a required"
                                runat="server" FieldLabel="Name" AnchorHorizontal="96%" ClientIDMode="Static" MsgTarget="Side">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server" FieldLabel="Description"
                                AnchorHorizontal="96%">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server" FieldLabel="Url"
                                AnchorHorizontal="96%">
                                <ToolTips>
                                    <ext:ToolTip Html="Lookup latest SensorWeb ID" ID="rfUrlToolip" runat="server">
                                    </ext:ToolTip>
                                </ToolTips>
                            </ext:TextField>
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
    <ext:Window ID="AvailableOfferingsWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Offerings" Width="600" Height="600" Layout="FitLayout" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableOffering" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="OfferingGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="OfferingGridStore" runat="server" OnRefreshData="OfferingGridStore_RefreshData" RemoteSort="true" >
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
                        <SortInfo Field="Name" Direction="ASC" />
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
                <Plugins>
                    <ext:GridFilters runat="server" ID="GridFilters5">
                        <Filters>
                            <ext:StringFilter DataIndex="Code" />
                            <ext:StringFilter DataIndex="Name" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
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
    <ext:Window ID="AvailableUnitsWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Units" Width="600" Height="600" Layout="FitLayout" Hidden="true" ClientIDMode="Static">
        <Listeners>
            <Hide Fn="CloseAvailableUnit" />
        </Listeners>
        <Items>
            <ext:GridPanel ID="UnitOfMeasureGrid" runat="server" Header="false" Border="false" ClientIDMode="Static">
                <Store>
                    <ext:Store ID="UnitOfMeasureGridStore" runat="server" OnRefreshData="UnitOfMeasureStore_RefreshData">
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
                        <SortInfo Field="Unit" Direction="ASC" />
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
                <Plugins>
                    <ext:GridFilters runat="server" ID="GridFilters4">
                        <Filters>
                            <ext:StringFilter DataIndex="Code" />
                            <ext:StringFilter DataIndex="Unit" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
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
</asp:Content>

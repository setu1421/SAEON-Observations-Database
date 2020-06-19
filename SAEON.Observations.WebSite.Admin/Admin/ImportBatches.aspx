<%@ Page Title="Import Batches" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ImportBatches.aspx.cs" Inherits="Admin_ImportBatches" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../JS/ImportBatches.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ImportBatchesGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(ImportBatchesGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);
            ImportBatchesGrid.submitData(false, { isUpload: true });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ext:Store ID="StatusStore" runat="server">
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
    </ext:Store>
    <ext:Store ID="StatusReasonStore" runat="server">
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
    </ext:Store>
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Import Batches" Height="350" Layout="FitLayout">
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
                            <ext:GridPanel ID="ImportBatchesGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="ImportBatchesGridStore" runat="server" RemoteSort="true" OnRefreshData="ImportBatchesGridStore_RefreshData"
                                        OnSubmitData="ImportBatchesGridStore_Submit" ClientIDMode="Static">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="Int" />
                                                    <ext:RecordField Name="ImportDate" Type="Date" />
                                                    <ext:RecordField Name="Status" Type="Int" />
                                                    <ext:RecordField Name="DataSourceID" Type="String" />
                                                    <ext:RecordField Name="DataSourceName" Type="String" />
                                                    <ext:RecordField Name="StatusDescription" Type="String" />
                                                    <ext:RecordField Name="FileName" Type="String" />
                                                    <ext:RecordField Name="LogFileName" Type="String" />
                                                    <ext:RecordField Name="Issues" Type="String" />
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
                                        <%--<DirectEventConfig IsUpload="true" />--%>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Number" DataIndex="Code" Width="80" />
                                        <ext:DateColumn Header="Import Date" DataIndex="ImportDate" Width="125" Format="dd MMM yyyy HH:mm:ss" />
                                        <%--<ext:Column Header="Data Source ID" DataIndex="DataSourceID" Width="150" />--%>
                                        <ext:Column Header="Data Source Name" DataIndex="DataSourceName" Width="350" />
                                        <ext:Column Header="File Name" DataIndex="FileName" Width="350" />
                                        <ext:Column Header="Status" DataIndex="StatusDescription" Width="150" />
                                        <ext:Column Header="Issues" DataIndex="Issues" Width="350" />
                                        <ext:CommandColumn Width="75">
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
                                            <RowSelect Fn="ImportBatchRowSelect" Buffer="25" />
                                        </Listeners>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="Code" />
                                            <ext:DateFilter DataIndex="ImportDate" />
                                            <ext:StringFilter DataIndex="DataSourceID" />
                                            <ext:StringFilter DataIndex="DataSourceName" />
                                            <ext:StringFilter DataIndex="Description" />
                                            <ext:StringFilter DataIndex="FileName" />
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
                </North>
                <Center>
                    <ext:TabPanel ID="tpCenter" runat="server" TabPosition="Top" Border="false" ClientIDMode="Static">
                        <Items>
                            <ext:Panel ID="pnlObservations" runat="server" Title="Observations" ClientIDMode="Static" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="tbObservations" runat="server" ClientIDMode="Static">
                                        <Items>
                                            <ext:ToolbarTextItem Text="Status:" />
                                            <ext:ComboBox ID="cbStatus" runat="server" StoreID="StatusStore" MsgTarget="Side" DisplayField="Name" Width="200"
                                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                                ValueField="Id" DataIndex="Id" EmptyText="Select a status" ValueNotFoundText="Select a status"
                                                ClientIDMode="Static">
                                                <%--                                                <Listeners>
                                                    <Select Fn="EnableApply" />
                                                </Listeners>--%>
                                                <DirectEvents>
                                                    <Select OnEvent="EnableButtons" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:ToolbarTextItem Text="Reason:" />
                                            <ext:ComboBox ID="cbStatusReason" runat="server" StoreID="StatusReasonStore" MsgTarget="Side" DisplayField="Name" Width="200"
                                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local"
                                                ValueField="Id" DataIndex="Id" EmptyText="Select a reason" ValueNotFoundText="Select a reason"
                                                ClientIDMode="Static">
                                                <DirectEvents>
                                                    <Select OnEvent="EnableButtons" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:ToolbarSeparator Width="10" />
                                            <ext:Button ID="btnSetSelected" runat="server" Icon="ShieldAdd" Text="Set selected" ClientIDMode="Static" Disabled="true">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip2" runat="server" Html="Set selected" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="SetSelectedClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnClearSelected" runat="server" Icon="ShieldDelete" Text="Clear selected" ClientIDMode="Static" Disabled="true">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Clear selected" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="ClearSelectedClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarSeparator Width="10" />
                                            <ext:Button ID="btnSetAll" runat="server" Icon="ShieldAdd" Text="Set all" ClientIDMode="Static" Disabled="true">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip4" runat="server" Html="Set all" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="SetAllClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnClearAll" runat="server" Icon="ShieldDelete" Text="Clear all" ClientIDMode="Static" Disabled="true">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip7" runat="server" Html="Clear all" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="ClearAllClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarSeparator Width="10" />
                                            <ext:Button ID="btnSetWithout" runat="server" Icon="ShieldAdd" Text="Set without" ClientIDMode="Static" Disabled="true">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip3" runat="server" Html="Set without status" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="SetWithoutClick">
                                                        <ExtraParams>
                                                            <ext:Parameter
                                                                Name="count"
                                                                Value="ObservationsGrid.getStore().getCount()"
                                                                Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <%--                                            <ext:ToolbarSeparator Width="10" />
                                            <ext:Button ID="Button4" runat="server" Icon="ShieldAdd" Text="Test" ClientIDMode="Static">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip6" runat="server" Html="Test" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="SetTestClick" />
                                                </DirectEvents>
                                            </ext:Button>--%>
                                            <%--
                                            <ext:Button ID="btnAddObservation" runat="server" Icon="Add" Text="Add Observation">
                                                <ToolTips>
                                                    <ext:ToolTip ID="ToolTip5" runat="server" Html="Add" />
                                                </ToolTips>
                                                <DirectEvents>
                                                    <Click OnEvent="AddObservationClick" />
                                                </DirectEvents>
                                            </ext:Button>
                                            --%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="ObservationsGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="ObservationsGridStore" runat="server" OnRefreshData="ObservationsGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="SensorName" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonName" Type="Auto" />
                                                            <ext:RecordField Name="OfferingName" Type="Auto" />
                                                            <ext:RecordField Name="UnitOfMeasureUnit" Type="Auto" />
                                                            <ext:RecordField Name="ValueDate" Type="Date" />
                                                            <ext:RecordField Name="TextValue" Type="Auto" />
                                                            <ext:RecordField Name="RawValue" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="DataValue" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="Latitude" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="Longitude" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="Elevation" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="StatusName" Type="Auto" />
                                                            <ext:RecordField Name="StatusReasonName" Type="Auto" />
                                                            <ext:RecordField Name="Comment" Type="Auto" />
                                                            <ext:RecordField Name="CorrelationID" Type="Auto" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="ImportBatchID" Value="Ext.getCmp('#{ImportBatchesGrid}') && #{ImportBatchesGrid}.getSelectionModel().hasSelection() ? #{ImportBatchesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                    <ext:Parameter Name="limit" Value="250" Mode="Raw" />
                                                    <ext:Parameter Name="sort" Value="" />
                                                    <ext:Parameter Name="dir" Value="" />
                                                </BaseParams>
                                                <SortInfo Field="ValueDate" Direction="DESC" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel4" runat="server">
                                            <Columns>
                                                <ext:Column Header="Sensor" DataIndex="SensorName" Width="350" />
                                                <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="150" />
                                                <ext:Column Header="Offering" DataIndex="OfferingName" Width="150" />
                                                <ext:Column Header="Unit of Measure" DataIndex="UnitOfMeasureUnit" Width="150" />
                                                <ext:DateColumn Header="Date" DataIndex="ValueDate" Width="125" Format="dd MMM yyyy HH:mm:ss" />
                                                <ext:Column Header="Text value" DataIndex="TextValue" Width="75" />
                                                <ext:NumberColumn Header="Raw value" DataIndex="RawValue" Width="100" Format=",0.000000" Align="Right" />
                                                <ext:NumberColumn Header="Data value" DataIndex="DataValue" Width="100" Format=",0.000000" Align="Right" />
                                                <ext:NumberColumn Header="Latitude" DataIndex="Latitude" Width="100" Format="0.00000" Align="Right" />
                                                <ext:NumberColumn Header="Longitude" DataIndex="Longitude" Width="100" Format="0.00000" Align="Right" />
                                                <ext:NumberColumn Header="Elevation" DataIndex="Elevation" Width="100" Format="0.00" Align="Right" />
                                                <ext:Column Header="Status" DataIndex="StatusName" Width="150" />
                                                <ext:Column Header="Reason" DataIndex="StatusReasonName" Width="150" />
                                                <ext:Column Header="Correlation ID" DataIndex="CorrelationID" Width="150" />
                                                <ext:Column Header="Comment" DataIndex="Comment" Width="400" />
                                                <%--                                                <ext:CommandColumn Width="200">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                        <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="Delete" />
                                                    </Commands>
                                                </ext:CommandColumn>--%>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:CheckboxSelectionModel runat="server">
                                                <DirectEvents>
                                                    <SelectionChange OnEvent="EnableButtons" Buffer="100" />
                                                </DirectEvents>
                                            </ext:CheckboxSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFiltersObservations">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="SensorName" />
                                                    <ext:DateFilter DataIndex="ValueDate" />
                                                    <ext:NumericFilter DataIndex="RawValue" />
                                                    <ext:NumericFilter DataIndex="DataValue" />
                                                    <ext:StringFilter DataIndex="PhenomenonName" />
                                                    <ext:StringFilter DataIndex="OfferingName" />
                                                    <ext:StringFilter DataIndex="UnitOfMeasureUnit" />
                                                    <ext:StringFilter DataIndex="StatusName" />
                                                    <ext:StringFilter DataIndex="StatusReasonName" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbarObservations" runat="server" ClientIDMode="Static" PageSize="250" EmptyMsg="No data found" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel1" runat="server" Title="Summary" ClientIDMode="Static" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="SummaryGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="SummaryGridStore" runat="server" OnRefreshData="SummaryGridStore_RefreshData">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="Auto" />
                                                            <ext:RecordField Name="PhenomenonName" Type="Auto" />
                                                            <ext:RecordField Name="OfferingName" Type="Auto" />
                                                            <ext:RecordField Name="UnitOfMeasureUnit" Type="Auto" />
                                                            <ext:RecordField Name="SensorName" Type="Auto" />
                                                            <ext:RecordField Name="Count" Type="Int" />
                                                            <ext:RecordField Name="Minimum" Type="Float" />
                                                            <ext:RecordField Name="Maximum" Type="Float" />
                                                            <ext:RecordField Name="Average" Type="Float" />
                                                            <ext:RecordField Name="StandardDeviation" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="Variance" Type="Float" UseNull="true" />
                                                            <ext:RecordField Name="StartDate" Type="Date" />
                                                            <ext:RecordField Name="EndDate" Type="Date" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <BaseParams>
                                                    <ext:Parameter Name="ImportBatchID" Value="Ext.getCmp('#{ImportBatchesGrid}') && #{ImportBatchesGrid}.getSelectionModel().hasSelection() ? #{ImportBatchesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                    <ext:Parameter Name="limit" Value="250" Mode="Raw" />
                                                    <ext:Parameter Name="sort" Value="" />
                                                    <ext:Parameter Name="dir" Value="" />
                                                </BaseParams>
                                                <SortInfo Field="PhenomenonName" Direction="DESC" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel5" runat="server">
                                            <Columns>
                                                <ext:Column Header="Phenomenon" DataIndex="PhenomenonName" Width="150" />
                                                <ext:Column Header="Offering" DataIndex="OfferingName" Width="150" />
                                                <ext:Column Header="Unit of Measure" DataIndex="UnitOfMeasureUnit" Width="150" />
                                                <ext:Column Header="Sensor" DataIndex="SensorName" Width="350" />
                                                <ext:NumberColumn Header="Count" DataIndex="Count" Width="100" Format=",0" Align="Right" />
                                                <ext:NumberColumn Header="Minimum" DataIndex="Minimum" Width="100" Format=",0.000" Align="Right" />
                                                <ext:NumberColumn Header="Maximum" DataIndex="Maximum" Width="100" Format=",0.000" Align="Right" />
                                                <ext:NumberColumn Header="Average" DataIndex="Average" Width="100" Format=",0.000" Align="Right" />
                                                <ext:NumberColumn Header="Std. Deviation" DataIndex="StandardDeviation" Width="100" Format=",0.000000" Align="Right" />
                                                <ext:NumberColumn Header="Variance" DataIndex="Variance" Width="100" Format=",0.000000" Align="Right" />
                                                <ext:DateColumn Header="Start Date" DataIndex="StartDate" Width="125" Format="dd MMM yyyy HH:mm:ss" />
                                                <ext:DateColumn Header="End Date" DataIndex="EndDate" Width="125" Format="dd MMM yyyy HH:mm:ss" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters2">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="PhenomenonName" />
                                                    <ext:StringFilter DataIndex="OfferingName" />
                                                    <ext:StringFilter DataIndex="UnitOfMeasureUnit" />
                                                    <ext:StringFilter DataIndex="SensorName" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar3" runat="server" ClientIDMode="Static" PageSize="250" EmptyMsg="No data found" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel12" runat="server" Title="Data Log" ClientIDMode="Static" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="DataLogGrid" runat="server" Border="false" ClientIDMode="Static">
                                        <Store>
                                            <ext:Store ID="DataLogGridStore" runat="server" RemoteSort="true" OnRefreshData="DataLogGrid_RefreshData">
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
                                                    <ext:Parameter Name="ImportBatchID" Value="Ext.getCmp('#{ImportBatchesGrid}') && #{ImportBatchesGrid}.getSelectionModel().hasSelection() ? #{ImportBatchesGrid}.getSelectionModel().getSelected().id : -1"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                    <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                                    <ext:Parameter Name="sort" Value="" />
                                                    <ext:Parameter Name="dir" Value="" />
                                                </BaseParams>
                                                <SortInfo Field="ValueDate" Direction="ASC" />
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
                                                <ext:DateColumn Header="Import Date" DataIndex="ImportDate" Width="125" Format="dd MMM yyyy HH:mm:ss"
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
                                                <ext:Column Header="Sensor" DataIndex="SensorName" Width="350" />
                                                <ext:DateColumn Header="Date" DataIndex="ValueDate" Width="125" Format="dd MMM yyyy HH:mm:ss">
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
                                                <ext:Column Header="Raw Text" DataIndex="ValueText" Width="90" />
                                                <ext:Column Header="Raw Value" DataIndex="RawValue" Width="90">
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
                                                <ext:Column Header="Status" DataIndex="Status" Width="200" />
                                                <ext:Column Header="Data Transformation ID" DataIndex="DataSourceTransformationID"
                                                    Width="150" Hidden="true" />
                                                <ext:Column Header="Transformation" DataIndex="Transformation" Width="150" Hidden="true" />
                                                <ext:CommandColumn Width="75">

                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                                        <ext:GridCommand Icon="NoteDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                                        <ext:GridCommand Icon="Add" CommandName="MoveToObservation" Text="" ToolTip-Text="Move to observation" />
                                                    </Commands>
                                                    <PrepareToolbar Fn="prepareToolbarTransformation" />
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <LoadMask ShowMask="true" />
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFiltersDataLog">
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
                                            <Command Fn="onDataLogCommand" />
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
    <ext:Window ID="ImportWindow" runat="server" Title="Import Data File" Closable="true"
        Width="800" Height="500" Border="false" Collapsible="false" Maximizable="true"
        Hidden="true" ClientIDMode="Static">
        <%-- <Listeners>
            <Hide Fn="closepreview" />
        </Listeners>--%>
        <Listeners>
            <Hide Handler="#{DataFileUpload}.disable();" />
            <Show Handler="#{DataFileUpload}.enable();" />
        </Listeners>
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
                                MsgTarget="Side" BlankText="Data Source is required"
                                Editable="true" TypeAhead="true" ForceSelection="true" AllowBlank="false" SelectOnFocus="true" TriggerAction="All" Mode="Local">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();this.focus();" />
                                </Listeners>
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
                            <ext:FileUploadField ID="DataFileUpload" runat="server" EmptyText="Select a File" Enabled="false"
                                AllowBlank="false" FieldLabel="Data File" ButtonText="" Icon="Zoom" BlankText="input file is required"
                                ClientIDMode="Static">
                                <Listeners>
                                    <FileSelected Handler="#{DataFileUpload}.disable();" />
                                </Listeners>
                            </ext:FileUploadField>
                        </Items>
                        <Listeners>
                            <ClientValidation Handler="#{SaveButton}.setDisabled(!valid);#{LogStartButton}.setDisabled(!valid);#{TestUploadButton}.setDisabled(!valid);" />
                            <%--<ClientValidation Handler="#{SaveButton}.setDisabled(!valid);" />--%>
                        </Listeners>
                        <Buttons>
                            <ext:Button ID="TestCosmos" runat="server" Text="Test CosmosDB">
                                <DirectEvents>
                                    <Click OnEvent="TestCosmosDB" IsUpload="false" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="LogStartButton" runat="server" Text="Log Start" Icon="ApplicationLightning">
                                <%--                               <Listeners>
                                    <Click Handler="DirectCall.UploadLogging('LogStartClick'); #{DataFileUpload}.enable(); return true;" />
                                </Listeners>--%>
                                <DirectEvents>
                                    <Click OnEvent="LogStartClick" IsUpload="false" After="#{DataFileUpload}.enable();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="TestUploadButton" runat="server" Text="Test Upload" Icon="ApplicationLightning">
                                <DirectEvents>
                                    <Click OnEvent="TestUploadClick" IsUpload="true">
                                        <EventMask ShowMask="true" Msg="Testing upload..." RemoveMask="true" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>

                            <ext:Button ID="SaveButton" runat="server" Text="Import file" Icon="Accept" ClientIDMode="Static">
                                <DirectEvents>
                                    <Click OnEvent="UploadClick" IsUpload="true"
                                        Before="return #{BasicForm}.getForm().isValid();"
                                        After="#{DataFileUpload}.disable();return true;"
                                        Failure="DirectCall.UploadLogging('SaveClickFailure');
                                                 Ext.Msg.show({
                                                    title   : 'Error',
                                                    msg     : 'Error during uploading',
                                                    minWidth: 200,
                                                    modal   : true,
                                                    icon    : Ext.Msg.ERROR,
                                                    buttons : Ext.Msg.OK
                                                });">
                                        <EventMask ShowMask="true" Msg="Uploading and processing..." RemoveMask="true" />
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
                            <ext:DateField ID="ValueDate" runat="server" DataIndex="ValueDate" FieldLabel="Date Value" Format="dd MMM yyyy HH:mm:ss"
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

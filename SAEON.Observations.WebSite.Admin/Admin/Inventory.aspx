<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Inventory.aspx.cs" Inherits="Admin_Inventory" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <%--<script type="text/javascript" src="../JS/Sensor.js"></script>--%>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(InventoryGridFilters.buildQuery(InventoryGridFilters.getFilterData())));
            var viscolsNew = makenewJsonForExport(InventoryGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))

            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(InventoryGridFilters.store.sortInfo.field + "|" + InventoryGridFilters.store.sortInfo.direction);

            InventoryGrid.submitData();

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
                    <ext:Panel ID="Panel1" runat="server" Title="Inventory" Layout="FitLayout" Hidden="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
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
                            <ext:GridPanel ID="InventoryGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="InventoryGridStore" runat="server" RemoteSort="true" OnRefreshData="InventoryGridStore_RefreshData"
                                        OnSubmitData="InventoryGridStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" Type="Auto" />
                                                    <ext:RecordField Name="SiteCode" Type="String" />
                                                    <ext:RecordField Name="SiteName" Type="String" />
                                                    <ext:RecordField Name="StationCode" Type="String" />
                                                    <ext:RecordField Name="StationName" Type="String" />
                                                    <ext:RecordField Name="InstrumentCode" Type="String" />
                                                    <ext:RecordField Name="InstrumentName" Type="String" />
                                                    <ext:RecordField Name="SensorCode" Type="String" />
                                                    <ext:RecordField Name="SensorName" Type="String" />
                                                    <ext:RecordField Name="PhenomenonCode" Type="String" />
                                                    <ext:RecordField Name="PhenomenonName" Type="String" />
                                                    <ext:RecordField Name="OfferingCode" Type="String" />
                                                    <ext:RecordField Name="OfferingName" Type="String" />
                                                    <ext:RecordField Name="UnitOfMeasureCode" Type="String" />
                                                    <ext:RecordField Name="UnitOfMeasureUnit" Type="String" />
                                                    <ext:RecordField Name="Count" Type="Float" />
                                                    <ext:RecordField Name="StartDate" Type="Date" />
                                                    <ext:RecordField Name="EndDate" Type="Date" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="50" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="SiteName, StationName, InstrumentName, SensorName" Direction="ASC" />
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Site Code" DataIndex="SiteCode" Width="100" />
                                        <ext:Column Header="Site Name" DataIndex="SiteName" Width="200" />
                                        <ext:Column Header="Station Code" DataIndex="StationCode" Width="100" />
                                        <ext:Column Header="Station Name" DataIndex="StationName" Width="200" />
                                        <ext:Column Header="Instrument Code" DataIndex="InstrumentCode" Width="100" />
                                        <ext:Column Header="Instrument Name" DataIndex="InstrumentName" Width="200" />
                                        <ext:Column Header="Phenomenon Code" DataIndex="PhenomenonCode" Width="110" />
                                        <ext:Column Header="Phenomenon Name" DataIndex="PhenomenonName" Width="150" />
                                        <ext:Column Header="Offering Code" DataIndex="OfferingCode" Width="80" />
                                        <ext:Column Header="Offering Name" DataIndex="OfferingName" Width="100" />
                                        <ext:Column Header="Unit Code" DataIndex="UnitOfMeasureCode" Width="70" />
                                        <ext:Column Header="Unit Name" DataIndex="UnitOfMeasureUnit" Width="100" />
                                        <ext:NumberColumn Header="Count" DataIndex="Count" Format=",0" Align="Right" Width="60" />
                                        <ext:DateColumn Header="Start Date" DataIndex="StartDate" Format="dd MMM yyyy" Width="75" />
                                        <ext:DateColumn Header="End Date" DataIndex="EndDate" Format="dd MMM yyyy" Width="75" />
                                    </Columns>
                                </ColumnModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="InventoryGridFilters" ClientIDMode="Static">
                                        <Filters>
                                            <ext:StringFilter DataIndex="SiteCode" />
                                            <ext:StringFilter DataIndex="SiteName" />
                                            <ext:StringFilter DataIndex="StationCode" />
                                            <ext:StringFilter DataIndex="StationName" />
                                            <ext:StringFilter DataIndex="InstrumentCode" />
                                            <ext:StringFilter DataIndex="InstrumentName" />
                                            <ext:StringFilter DataIndex="SensorCode" />
                                            <ext:StringFilter DataIndex="SensorName" />
                                            <ext:StringFilter DataIndex="PhenomenonCode" />
                                            <ext:StringFilter DataIndex="PhenomenonName" />
                                            <ext:StringFilter DataIndex="OfferingCode" />
                                            <ext:StringFilter DataIndex="OfferingName" />
                                            <ext:StringFilter DataIndex="UnitOfMeasureCode" />
                                            <ext:StringFilter DataIndex="UnitOfMeasureUnit" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="InventoryGridSelectionModel" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="InventoryGridToolbar" runat="server" PageSize="50" EmptyMsg="No data found" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>

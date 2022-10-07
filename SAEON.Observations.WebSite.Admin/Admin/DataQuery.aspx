<%@ Page Title="Data Queries" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DataQuery.aspx.cs" Inherits="Admin_DataQuery" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../JS/DataQuery.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>
    <script type="text/javascript">
        var saveData = function (s, e) {
            GridData.setValue(Ext.encode(GridFilters1.buildQuery(GridFilters1.getFilterData())));
            VisCols.setValue(Ext.encode(ObservationsGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));

            ObservationsGrid.submitData(false);
            //VisCols.setValue(Ext.encode(ObservationsGrid.getRowsValues({ visibleOnly: true, excludeId: true })));
        };

        var submitValue = function (format) {
            if (FromFilter.getValue() == '' || ToFilter.getValue() == '') {
                //Ext.Msg.alert('Invalid Date Range', 'Select a valid Date Range');
                Ext.Msg.show(
                    {
                        icon: Ext.MessageBox.WARNING,
                        msg: 'Select a valid Date Range.',
                        buttons: Ext.Msg.OK,
                        title: 'Invalid Date Range'
                    });
            }
            else if (FilterTree.getCheckedNodes().length == 0) {
                Ext.Msg.show(
                    {
                        icon: Ext.MessageBox.WARNING,
                        msg: 'Select at least one aspect in the treepane',
                        buttons: Ext.Msg.OK,
                        title: 'Invalid Filter Criteria'
                    });
            }
            else {
                GridData.setValue(Ext.encode(GridFilters1.buildQuery(GridFilters1.getFilterData())));
                var viscolsNew = makenewJsonForExport(ObservationsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
                VisCols.setValue(viscolsNew);
                var hidCols = makenewJsonForExport(ObservationsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return this.isHidden(colIndex); }))
                HiddenCols.setValue(hidCols);
                FormatType.setValue(format);
                SortInfo.setValue(GridFilters1.store.sortInfo.field + "|" + GridFilters1.store.sortInfo.direction);
                ObservationsGrid.submitData(false);
            }
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="HiddenCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <West Collapsible="true" Split="true" MarginsSummary="5 0 5 5">
                    <ext:Panel ID="pnlEast" runat="server" Title="Data" Width="400" Layout="Fit">
                        <Items>
                            <ext:TreePanel ID="FilterTree" runat="server" Animate="false" AutoScroll="true" Icon="BookOpen"
                                ClientIDMode="Static" MonitorResize="true">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <%--                                                <ext:Button Text="Expand All" runat="server" ID="ExpandAllButton">
                                                    <Listeners>
                                                        <Click Handler="#{FilterTree}.expandAll();" />
                                                    </Listeners>
                                                </ext:Button>--%>
                                            <ext:Button Text="Collapse All" runat="server" ID="CollapseAllButton">
                                                <Listeners>
                                                    <Click Handler="#{FilterTree}.collapseAll();" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button Text="Refresh" runat="server" ID="btnTreeRefresh">
                                                <Listeners>
                                                    <Click Fn="CheckInputAndReload" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <%--                                <Loader>
                                    <ext:PageTreeLoader OnNodeLoad="NodeLoad" Timeout="900000"/>
                                </Loader>--%>
                                <Listeners>
                                    <Click Handler="#{FilterTreeBottomBar}.setStatus({text: 'Node Selected: <b>' + node.text + '</b>', clear: true});" />
                                    <ExpandNode Delay="30" Handler="#{FilterTreeBottomBar}.setStatus({text: 'Node Expanded: <b>' + node.text + '</b>', clear: true});" />
                                    <CollapseNode Handler="#{FilterTreeBottomBar}.setStatus({text: 'Node Collapsed: <b>' + node.text + '</b>', clear: true});" />
                                </Listeners>
                                <BottomBar>
                                    <ext:StatusBar ID="FilterTreeBottomBar" runat="server" ClientIDMode="Static" AutoClear="1000" />
                                </BottomBar>
                            </ext:TreePanel>
                        </Items>
                    </ext:Panel>
                </West>
                <Center MarginsSummary="5 5 5 0">
                    <ext:Panel ID="Panel1" runat="server" Title="Results" Layout="Fit">
                        <TopBar>
                            <ext:Toolbar ID="tbQueryData" runat="server">
                                <Items>
                                    <ext:ToolbarTextItem Text="From Date:" />
                                    <ext:ToolbarSpacer Width="10" />
                                    <ext:DateField ID="FromFilter" runat="server" Text="From" Vtype="daterange" ClientIDMode="Static" Format="dd MMM yyyy">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{ToFilter}" Mode="Value" />
                                        </CustomConfig>
                                    </ext:DateField>
                                    <ext:ToolbarSeparator Width="10" />
                                    <ext:ToolbarTextItem Text="To Date :" />
                                    <ext:ToolbarSpacer Width="10" />
                                    <ext:DateField ID="ToFilter" runat="server" Text="To" Vtype="daterange" ClientIDMode="Static" Format="dd MMM yyyy">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{FromFilter}" Mode="Value" />
                                        </CustomConfig>
                                    </ext:DateField>
                                    <ext:ToolbarSeparator Width="10" />
                                    <ext:Button ID="FilterGridButton" runat="server" Text="Filter" Icon="Find">
                                        <Listeners>
                                            <Click Fn="CheckInputAndReload" />
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
                            <ext:GridPanel ID="ObservationsGrid" runat="server" Border="false" Layout="FitLayout"
                                ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="ObservationsGridStore" runat="server" RemotePaging="true" RemoteSort="true" OnRefreshData="ObservationsGridStore_RefreshData"
                                        OnSubmitData="ObservationsGridStore_Submit" AutoLoad="false">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Site" Type="String" />
                                                    <ext:RecordField Name="Station" Type="String" />
                                                    <ext:RecordField Name="Instrument" Type="String" />
                                                    <ext:RecordField Name="Sensor" Type="String" />
                                                    <ext:RecordField Name="Phenomenon" Type="String" />
                                                    <ext:RecordField Name="Offering" Type="String" />
                                                    <ext:RecordField Name="Unit" Type="String" />
                                                    <ext:RecordField Name="UnitSymbol" Type="String" />
                                                    <ext:RecordField Name="Date" Type="Date" SortType="AsDate" />
                                                    <ext:RecordField Name="Value" Type="Float" UseNull="true" />
                                                    <ext:RecordField Name="Status" Type="String" />
                                                    <ext:RecordField Name="Reason" Type="String" />
                                                    <ext:RecordField Name="Comment" Type="String" />
                                                    <ext:RecordField Name="Elevation" Type="Float" UseNull="true" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="250" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="Date" Direction="DESC" />
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Site" DataIndex="Site" Width="100" Hideable="true" />
                                        <ext:Column Header="Station" DataIndex="Station" Width="100" />
                                        <ext:Column Header="Instrument" DataIndex="Instrument" Width="100"  Hideable="true"/>
                                        <ext:Column Header="Sensor" DataIndex="Sensor" Width="100"  Hideable="true"/>
                                        <ext:Column Header="Phenomenon" DataIndex="Phenomenon" Width="100" />
                                        <ext:Column Header="Offering" DataIndex="Offering" Width="100" />
                                        <ext:Column Header="Unit of Measure" DataIndex="Unit" Width="100" Hideable="true" Hidden="false" />
                                        <ext:Column Header="Symbol" DataIndex="UnitSymbol" Width="50" Hideable="true" Hidden="true" />
                                        <ext:DateColumn Header="Date" DataIndex="Date" Width="125" Format="dd MMM yyyy HH:mm" />
                                        <ext:NumberColumn Header="Value" DataIndex="Value" Width="100" Format=",0.000000" Align="Right" />
                                        <ext:Column Header="Status" DataIndex="Status" Width="150" Hideable="true" />
                                        <ext:Column Header="Reason" DataIndex="Reason" Width="150" Hideable="true" />
                                        <ext:NumberColumn Header="Elevation" DataIndex="Elevation" Width="75" Format="0.00" Align="Right" Hideable="true"/>
                                        <ext:Column Header="Comment" DataIndex="Comment" Width="150" Hideable="true" />
                                    </Columns>
                                </ColumnModel>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" ClientIDMode="Static">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Id" />
                                            <ext:StringFilter DataIndex="Site" />
                                            <ext:StringFilter DataIndex="Station" />
                                            <ext:StringFilter DataIndex="Phenomenon" />
                                            <ext:StringFilter DataIndex="Offering" />
                                            <ext:StringFilter DataIndex="Unit" />
                                            <ext:StringFilter DataIndex="UnitSymbol" />
                                            <ext:DateFilter DataIndex="Date" />
                                            <ext:NumericFilter DataIndex="Value" />
                                            <ext:StringFilter DataIndex="Instrument" />
                                            <ext:StringFilter DataIndex="Sensor" />
                                            <ext:StringFilter DataIndex="Status" />
                                            <ext:StringFilter DataIndex="Reason" />
                                            <ext:NumericFilter DataIndex="Elevation" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="250" EmptyMsg="No data found" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>

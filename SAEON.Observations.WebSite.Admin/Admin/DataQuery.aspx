<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataQuery.aspx.cs" Inherits="_DataQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="/JS/DataQuery.js"></script>
    <script type="text/javascript" src="/JS/generic.js"></script>
	<script type="text/javascript">
		var saveData = function (s, e)
		{
			GridData.setValue(Ext.encode(GridFilters1.buildQuery(GridFilters1.getFilterData())));
			VisCols.setValue(Ext.encode(ObservationsGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));

			ObservationsGrid.submitData(false);
			//VisCols.setValue(Ext.encode(ObservationsGrid.getRowsValues({ visibleOnly: true, excludeId: true })));
		};

		var submitValue = function (format)
		{
			if (FromFilter.getValue() == '' || ToFilter.getValue() == '')
			{
				//Ext.Msg.alert('Invalid Date Range', 'Select a valid Date Range');
				Ext.Msg.show(
				{
        			icon: Ext.MessageBox.WARNING,
        			msg: 'Select a valid Date Range.',
        			buttons: Ext.Msg.OK,
        			title: 'Invalid Date Range'
				});
			}
			else if (FilterTree.getCheckedNodes().length == 0)
			{
				Ext.Msg.show(
				{
        			icon: Ext.MessageBox.WARNING,
        			msg: 'Select at least one aspect in the treepane',
        			buttons: Ext.Msg.OK,
        			title: 'Invalid Filter Criteria'
				});
			}
			else
			{
				GridData.setValue(Ext.encode(GridFilters1.buildQuery(GridFilters1.getFilterData())));
				var viscolsNew = makenewJsonForExport(ObservationsGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
				VisCols.setValue(viscolsNew);
				FormatType.setValue(format);
				SortInfo.setValue(GridFilters1.store.sortInfo.field + "|" + GridFilters1.store.sortInfo.direction);

				ObservationsGrid.submitData(false);
			}

		};

	</script>
</head>
<body>
	<form id="form1" runat="server">
	<ext:ResourceManager ID="ResourceManager1" runat="server" />
	<ext:Hidden ID="GridData" runat="server" />
	<ext:Hidden ID="VisCols" runat="server" />
	<ext:Hidden ID="FormatType" runat="server" />
	<ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
	<ext:Viewport ID="ViewPort1" runat="server">
		<Items>
			<ext:BorderLayout ID="BorderLayout1" runat="server">
				<West Collapsible="true" Split="true" MarginsSummary="5 0 5 5">
					<ext:Panel ID="pnlEast" runat="server" Title="Data" Width="270" Layout="Fit">
						<Items>
							<ext:TreePanel ID="FilterTree" runat="server" Animate="false" AutoScroll="true" Icon="BookOpen"
								ClientIDMode="Static" MonitorResize="true">
								<TopBar>
									<ext:Toolbar ID="Toolbar1" runat="server">
										<Items>
											<ext:Button Text="Expand All" runat="server" ID="ExpandAllButton">
												<Listeners>
													<Click Handler="#{FilterTree}.expandAll();" />
												</Listeners>
											</ext:Button>
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
									<ext:DateField ID="FromFilter" runat="server" Text="From" Vtype="daterange" EndDateField="ToFilter"
										ClientIDMode="Static" />
									<ext:ToolbarSeparator Width="10" />
									<ext:ToolbarTextItem Text="To Date :" />
									<ext:ToolbarSpacer Width="10" />
									<ext:DateField ID="ToFilter" runat="server" Text="To" Vtype="daterange" ClientIDMode="Static">
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
									<ext:Store ID="Store2" runat="server" RemoteSort="true" RemotePaging="true" OnRefreshData="DQStore_RefreshData"
										OnSubmitData="DQStore_Submit" AutoLoad="false">
										<Proxy>
											<ext:PageProxy />
										</Proxy>
										<Reader>
											<ext:JsonReader IDProperty="Id">
												<Fields>
													<ext:RecordField Name="Id" Type="Auto" />
													<ext:RecordField Name="OrgName" Type="String" />
													<ext:RecordField Name="PsName" Type="String" />
													<ext:RecordField Name="StName" Type="String" />
													<ext:RecordField Name="SpName" Type="String" />
													<ext:RecordField Name="PhName" Type="String" />
													<ext:RecordField Name="OffName" Type="String" />
													<ext:RecordField Name="DataValue" Type="Float" UseNull="true" />
													<ext:RecordField Name="RawValue" Type="Float" UseNull="true"/>
													<ext:RecordField Name="UomUnit" Type="String" />
													<ext:RecordField Name="UomSymbol" Type="String" />
													<ext:RecordField Name="DschemaName" Type="String" />
													<ext:RecordField Name="ValueDate" Type="Date" SortType="AsDate"/>
													<ext:RecordField Name="UserName" Type="String" />
                                                    <ext:RecordField Name="Comment" Type="String" />
												</Fields>
											</ext:JsonReader>
										</Reader>
										<BaseParams>
											<ext:Parameter Name="start" Value="0" Mode="Raw" />
											<ext:Parameter Name="limit" Value="20" Mode="Raw" />
											<ext:Parameter Name="sort" Value="" />
											<ext:Parameter Name="dir" Value="" />
										</BaseParams>
										<SortInfo Field="ValueDate" Direction="DESC" />
										<DirectEventConfig IsUpload="true" />
									</ext:Store>
								</Store>
								<ColumnModel ID="ColumnModel1" runat="server">
									<Columns>
										<ext:Column Header="Organisation" DataIndex="OrgName" Width="100" Hideable="true"
											Hidden="true" />
										<ext:Column Header="ProjectSite" DataIndex="PsName" Width="100" Hideable="true" Hidden="true" />
										<ext:Column Header="Station" DataIndex="StName" Width="100" />
										<ext:Column Header="SensorProcedure" DataIndex="SpName" Width="100" />
										<ext:Column Header="Phenomenon" DataIndex="PhName" Width="100" />
										<ext:Column Header="Offering" DataIndex="OffName" Width="100" />
										<ext:Column Header="DataValue" DataIndex="DataValue" Width="100" />
										<ext:Column Header="RawValue" DataIndex="RawValue" Width="100" Hideable="true" Hidden="true" />
										<ext:Column Header="Unit of Measure" DataIndex="UomUnit" Width="100" Hideable="true"
											Hidden="true" />
										<ext:Column Header="Symbol" DataIndex="UomSymbol" Width="50" />
										<ext:Column Header="Schema Name" DataIndex="DschemaName" Width="100" Hideable="true"
											Hidden="true" />
										<ext:DateColumn Header="Date" DataIndex="ValueDate" Width="100" Format="yyyy/MM/dd HH:mm" />
                                        <ext:Column Header="Comment" DataIndex="Comment" Width="150" Hideable="true" />
										<ext:Column Header="User" DataIndex="UserName" Width="100" Hideable="true" Hidden="true" />
									</Columns>
								</ColumnModel>
								<LoadMask ShowMask="true" />
								<Plugins>
									<ext:GridFilters runat="server" ID="GridFilters1" ClientIDMode="Static">
										<Filters>
											<ext:StringFilter DataIndex="Id" />
											<ext:StringFilter DataIndex="PsName" />
											<ext:StringFilter DataIndex="StName" />
											<ext:StringFilter DataIndex="SpName" />
											<ext:StringFilter DataIndex="PhName" />
											<ext:StringFilter DataIndex="OffName" />
											<ext:NumericFilter DataIndex="DataValue" />
											<ext:NumericFilter DataIndex="RawValue" />
											<ext:StringFilter DataIndex="UomUnit" />
											<ext:StringFilter DataIndex="UomSymbol" />
											<ext:StringFilter DataIndex="DschemaName" />
											<ext:DateFilter DataIndex="ValueDate" />
											<ext:StringFilter DataIndex="UserName" />
										</Filters>
									</ext:GridFilters>
								</Plugins>
								<BottomBar>
									<ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="20" EmptyMsg="No data found" />
								</BottomBar>
							</ext:GridPanel>
						</Items>
					</ext:Panel>
				</Center>
			</ext:BorderLayout>
		</Items>
	</ext:Viewport>
	</form>
</body>
</html>

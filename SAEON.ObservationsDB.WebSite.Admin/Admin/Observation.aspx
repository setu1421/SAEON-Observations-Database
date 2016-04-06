<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Observation.aspx.cs" Inherits="_Observation"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Organisation.js"></script>
	<script type="text/javascript" src="../JS/generic.js"></script>

	<script type="text/javascript">
	    var submitValue = function (format) {
	    	GridData.setValue(Ext.encode(GridFilters1.buildQuery(GridFilters1.getFilterData())));
	    	var viscolsNew = makenewJsonForExport(ObservationGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
	    	//VisCols.setValue(Ext.encode(ObservationGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
	    	VisCols.setValue(viscolsNew);
	        FormatType.setValue(format);
	        SortInfo.setValue(GridFilters1.store.sortInfo.field + "|" + GridFilters1.store.sortInfo.direction);

	        ObservationGrid.submitData();
	    };
	</script>

</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
	<ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
	<ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
	<ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
	<ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Title="Organisations" Layout="FitLayout" Hidden="false"
                Icon="Find">
				<TopBar>
                    <ext:Toolbar ID="tOrganisations" runat="server">
                        <Items>
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
                    <ext:GridPanel ID="ObservationGrid" runat="server" Border="false" ClientIDMode="Static">
                        <Store>
                            <ext:Store ID="ObservationGridStore" runat="server" RemoteSort="true" OnRefreshData="ObservationStore_RefreshData" OnSubmitData="ObservationStore_Submit" 
							AutoLoad="true">
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
                                            <ext:RecordField Name="DataValue" Type="Float" UseNull="true"/>
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
                                <ext:Column Header="Station" DataIndex="StName" Width="150" />
                                <ext:Column Header="SensorProcedure" DataIndex="SpName" Width="150" />
                                <ext:Column Header="Phenomenon" DataIndex="PhName" Width="100" />
                                <ext:Column Header="Offering" DataIndex="OffName" Width="100" />
                                <ext:Column Header="DataValue" DataIndex="DataValue" Width="100" />
                                <ext:Column Header="RawValue" DataIndex="RawValue" Width="100" Hideable="true" Hidden="true" />
                                <ext:Column Header="Unit of Measure" DataIndex="UomUnit" Width="100" Hideable="true"
                                    Hidden="true" />
                                <ext:Column Header="Symbol" DataIndex="UomSymbol" Width="50" />
                                <ext:Column Header="Schema Name" DataIndex="DschemaName" Width="100" Hideable="true"
                                    Hidden="true" />
                                <ext:DateColumn Header="Date" DataIndex="ValueDate" Width="100" Format="yyyy/MM/dd HH:mm"/>
                                 <ext:Column Header="Comment" DataIndex="Comment" Width="150" Hideable="true" />
                                <ext:Column Header="User" DataIndex="UserName" Width="100" Hideable="true"/>
                            </Columns>
                        </ColumnModel>
                                 <%--<View>
									<ext:GroupingView ID="GroupingView1" HideGroupedColumn="false" runat="server" ForceFit="true"
										StartCollapsed="true" GroupTextTpl='<span id="Organisation-{[values.rs[0].data.OrgName]}"></span>{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
										EnableRowBody="true">
									</ext:GroupingView>
								</View>--%>
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
        </Items>
    </ext:Viewport>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Roles.aspx.cs" Inherits="_Roles"  MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="/JS/Roles.js"></script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
	<ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
		<Items>
			<ext:BorderLayout ID="BorderLayout1" runat="server">
				<Center MarginsSummary="5 5 0 5">
					<ext:Panel ID="Panel1" runat="server" Title="Roles" Layout="FitLayout" Hidden="false"
						Icon="Group">
						<TopBar>
							<ext:Toolbar ID="Toolbar1" runat="server">
								<Items>
									<ext:Button ID="Button1" runat="server" Icon="GroupAdd">
										<ToolTips>
											<ext:ToolTip ID="ToolTip1" runat="server" Html="Add Role" />
										</ToolTips>
										<DirectEvents>
											<Click OnEvent="NewRole" />
										</DirectEvents>
									</ext:Button>
								</Items>
							</ext:Toolbar>
						</TopBar>
						<Items>
							<ext:GridPanel ID="GridPanel1" runat="server" Border="false">
								<Store>
									<ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="Store1_RefreshData">
										<Proxy>
											<ext:PageProxy />
										</Proxy>
										<Reader>
											<ext:JsonReader IDProperty="RoleId">
												<Fields>
													<ext:RecordField Name="RoleId" Type="Auto" />
													<ext:RecordField Name="RoleName" Type="String" />
													<ext:RecordField Name="Description" Type="String" />
												</Fields>
											</ext:JsonReader>
										</Reader>
										<BaseParams>
											<ext:Parameter Name="start" Value="0" Mode="Raw" />
											<ext:Parameter Name="limit" Value="10" Mode="Raw" />
											<ext:Parameter Name="sort" Value="" />
											<ext:Parameter Name="dir" Value="" />
										</BaseParams>
										<SortInfo Field="RoleName" Direction="ASC" />
									</ext:Store>
								</Store>
								<ColumnModel ID="ColumnModel1" runat="server">
									<Columns>
										<ext:Column Header="Role Name" DataIndex="RoleName" Width="200" />
										<ext:Column Header="Description" DataIndex="Description" Width="200" />
										<ext:CommandColumn Width="50" Header="Options">
											<Commands>
												<ext:GridCommand Icon="GroupEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
												<ext:GridCommand Icon="GroupDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
											</Commands>
										</ext:CommandColumn>
									</Columns>
								</ColumnModel>
								<SelectionModel>
									<ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true">
										<Listeners>
											<RowSelect Handler="if (#{pnlSouth}.isVisible()) {#{Store4}.reload();}" Buffer="250" />
										</Listeners>
									</ext:RowSelectionModel>
								</SelectionModel>
								<LoadMask ShowMask="true" />
								<SaveMask ShowMask="true" />
								<Plugins>
									<ext:GridFilters runat="server" ID="GridFilters1">
										<Filters>
											<ext:StringFilter DataIndex="RoleId" />
											<ext:StringFilter DataIndex="RoleName" />
										</Filters>
									</ext:GridFilters>
								</Plugins>
								<BottomBar>
									<ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="10" EmptyMsg="No data found" />
								</BottomBar>
								<Listeners>
									<%--<CellClick Fn="cellClick" />--%>
									<Command Fn="DoDelete" />
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
				<South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
					<ext:Panel ID="pnlSouth" runat="server" Title="Modules" Height="200" Layout="Fit">
						<TopBar>
							<ext:Toolbar ID="SouthToolbar" runat="server">
								<Items>
									<ext:Button ID="AddModButton" runat="server" Icon="GroupAdd" Text="Add Module">
										<ToolTips>
											<ext:ToolTip ID="ToolTip3" runat="server" Html="Add" />
										</ToolTips>
										<Listeners>
											<Click Handler="if(Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection()){#{Store3}.reload();#{AvailableModsWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a Role.')}" />
										</Listeners>
									</ext:Button>
								</Items>
							</ext:Toolbar>
						</TopBar>
						<Items>
							<ext:GridPanel ID="ModsGrid" runat="server" Border="false">
								<Store>
									<ext:Store ID="Store4" runat="server" OnRefreshData="ModsGrid_RefreshData">
										<Proxy>
											<ext:PageProxy />
										</Proxy>
										<Reader>
											<ext:JsonReader IDProperty="Id">
												<Fields>
													<ext:RecordField Name="Id" Type="Auto" />
													<ext:RecordField Name="Name" Type="String" />
													<ext:RecordField Name="Description" Type="String" />
												</Fields>
											</ext:JsonReader>
										</Reader>
										<BaseParams>
											<ext:Parameter Name="RoleID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
												Mode="Raw" />
										</BaseParams>
									</ext:Store>
								</Store>
								<ColumnModel ID="ColumnModel2" runat="server">
									<Columns>
										<ext:Column Header="Module Name" DataIndex="Name" Width="200" />
										<ext:Column Header="Description" DataIndex="Description" Width="200" />
										<ext:CommandColumn Width="50">
											<Commands>
												<ext:GridCommand Icon="NoteDelete" CommandName="RemoveModules" Text="" ToolTip-Text="Delete" />
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
											<%--<ext:Parameter Name="Useridd" Value="record.id.sad()" Mode="Raw" />--%>
											<ext:Parameter Name="RoleID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
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
	<ext:Window ID="Window1" runat="server" Width="450" Height="400" Closable="true"
		Hidden="true" Collapsible="true" Title="Role Details" Maximizable="true" Layout="Fit">
		<Content>
			<ext:Hidden ID="tfID" runat="server">
			</ext:Hidden>
			<ext:FormPanel ID="FormPanel1" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
				Padding="5" Width="440" Height="370" ButtonAlign="Right" Layout="RowLayout">
				<Items>
					<ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
						LabelAlign="Top">
						<%--ColumnWidth=".5"--%>
						<Defaults>
							<ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
							<ext:Parameter Name="MsgTarget" Value="side" />
						</Defaults>
						<Items>
							<ext:TextField ID="tfRoleName" runat="server" IsRemoteValidation="false" Disabled="false"
								FieldLabel="Role Name" AnchorHorizontal="90%">
								<RemoteValidation OnValidation="ValidateField" />
							</ext:TextField>
						</Items>
					</ext:Panel>
					<ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form"
						LabelAlign="Top">
						<Items>
							<ext:TextArea ID="tfComment" runat="server" Height="50" FieldLabel="Comment" AnchorHorizontal="90%">
							</ext:TextArea>
						</Items>
					</ext:Panel>
					<ext:Label ID="lblInfo" runat="server" Text="" Hidden="true">
					</ext:Label>
				</Items>
				<Buttons>
					<ext:Button ID="btnSave" runat="server" Text="Save">
						<DirectEvents>
							<Click OnEvent="SaveRole" Method="POST" />
						</DirectEvents>
					</ext:Button>
				</Buttons>
				<BottomBar>
					<ext:StatusBar ID="StatusBar1" runat="server" Height="25" />
				</BottomBar>
				<Listeners>
					<ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? '' : 'Invalid data supplied', iconCls: valid ? 'icon-accept' : 'icon-exclamation'});#{btnSave}.setDisabled(!valid);" />
				</Listeners>
			</ext:FormPanel>
		</Content>
	</ext:Window>
	<ext:Window ID="AvailableModsWindow" runat="server" Collapsible="false" Maximizable="false"
		Title="Available Modules" Width="620" Height="300" X="50" Y="50" CenterOnLoad="false"
		Icon="GroupGear" Layout="Fit" Hidden="true">
		<Items>
			<ext:GridPanel ID="AModsGrid" runat="server" Header="false" Border="false">
				<Store>
					<ext:Store ID="Store3" runat="server" OnRefreshData="AMods_RefreshData">
						<Proxy>
							<ext:PageProxy />
						</Proxy>
						<Reader>
							<ext:JsonReader IDProperty="Id">
								<Fields>
									<ext:RecordField Name="Id" Type="Auto" />
									<ext:RecordField Name="Name" Type="String" />
									<ext:RecordField Name="Description" Type="String" />
								</Fields>
							</ext:JsonReader>
						</Reader>
						<BaseParams>
							<ext:Parameter Name="RoleID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
								Mode="Raw" />
						</BaseParams>
					</ext:Store>
				</Store>
				<ColumnModel ID="ColumnModel3" runat="server">
					<Columns>
						<ext:Column Header="Module Name" DataIndex="Name" Width="200" />
						<ext:Column Header="Description" DataIndex="Description" Width="200" />
					</Columns>
				</ColumnModel>
				<LoadMask ShowMask="true" />
				<SelectionModel>
					<ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
				</SelectionModel>
				<Buttons>
					<ext:Button ID="AcceptMod" runat="server" Text="Save" Icon="Accept">
						<DirectEvents>
							<Click OnEvent="AcceptMod_Click">
								<EventMask ShowMask="true" />
								<%--<ExtraParams>
									<ext:Parameter Name="RoleIDPara" Value="#{GridPanel1}.getSelectionModel().getSelected().id" Mode="Raw" ></ext:Parameter>
								</ExtraParams>--%>
							</Click>
						</DirectEvents>
					</ext:Button>
				</Buttons>
			</ext:GridPanel>
		</Items>
	</ext:Window>
</asp:Content>

		

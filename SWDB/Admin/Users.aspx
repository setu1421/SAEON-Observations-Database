<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    protected void Store1_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.GridPanel1.GetStore().DataSource = UserRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ShowDetails(object sender, DirectEventArgs e)
    {
        if (e != null)
        {
            lblInfo.Hidden = true;
            lblInfo.Text = "";
            tfPassword.Hidden = true;
            tfUserName.Disabled = true;
            string id = e.ExtraParams["id"];
            if (id != null)
            {
                MembershipUser usr = Membership.GetUser(new Guid(id));

                tfID.Text = id;
                tfUserName.Text = usr.UserName;
                tfEmail.Text = usr.Email;
                tfComment.Text = usr.Comment;
                tfCreateDate.Text = usr.CreationDate.ToString();

            }
        }

        this.Window1.Show();
    }

    protected void NewUser(object sender, DirectEventArgs e)
    {
        tfID.Text = "";
        tfUserName.Text = "";
        tfEmail.Text = "";
        tfComment.Text = "";
        tfCreateDate.Text = "";
        tfPassword.Text = "";
        tfPassword.Hidden = false;
        tfUserName.Disabled = false;

        ShowDetails(null, null);

    }

    protected void ChangePassword(object sender, DirectEventArgs e)
    {
        string temp = e.ExtraParams["Values"];
        string id = "";
        Dictionary<string, string>[] UserInfo = JSON.Deserialize<Dictionary<string, string>[]>(temp);
        //string id = UserInfo[0].Value;
        //tfPassUser = RowSelectionModel1.SelectedRows.items[0].RecordID;
        bool isDone = false;
        foreach (Dictionary<string, string> row in UserInfo)
        {
            foreach (KeyValuePair<string, string> keyValuePair in row)
            {
                if (!isDone)
                {
                    id = keyValuePair.Value;
                    isDone = true;
                }
            }
        }

        if (id != "")
        {
            tfPasswordHidden.Text = id;

            MembershipUser usr = Membership.GetUser(new Guid(id));
            tfPassUser.Text = usr.UserName;

            tfPassNew.Text = "";
            lblPassInfo.Text = "";
            this.Window2.Show();
        }
        else
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Invalid Selection",
                Message = "Please select the user who's password will be changed",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.WARNING
            });

            tfPasswordHidden.Text = "";
            tfPassUser.Text = "";
        }

    }


    protected void SavePass(object sender, DirectEventArgs e)
    {
        if (tfPassNew.Text.Length >= 6)
        {
            string id = tfPasswordHidden.Text;

            if (id != null)
            {
                try
                {
                    MembershipUser usr = Membership.GetUser(new Guid(id));
                    bool DidWork = usr.ChangePassword(usr.ResetPassword(), tfPassNew.Text);
                    //Membership.UpdateUser(usr);
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Password changed",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO
                    });

                    Window2.Hide();
                }
                catch (Exception ex1)
                {
                    lblPassInfo.Text = ex1.Message;
                    //lblPassInfo.Hidden = false;
                    //throw;
                }
                //Membership.UpdateUser(usr);

            }
        }
        else
        {
            lblPassInfo.Text = "Password should be more than 6 characters";
        }

    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];

        if (ActionType == "Delete")
        {
            //Membership.DeleteUser(recordID);
            MembershipUser usr = Membership.GetUser(new Guid(recordID));
            Membership.DeleteUser(usr.UserName);

            GridPanel1.DataBind();
        }

        else if (ActionType == "Edit")
        {
            ShowDetails(sender, e);
        }

        else if (ActionType == "RemoveRoles")
        {
            AspnetUsersInRoleCollection UiRCol = new AspnetUsersInRoleCollection().Where("UserId", e.ExtraParams["UserID"]).Where("RoleId", recordID).Load();
            if (UiRCol.Count == 1)
            {
                AspnetRole role = new AspnetRole(recordID);
                AspnetUser user = new AspnetUser(e.ExtraParams["UserID"]);

                Roles.RemoveUserFromRole(user.UserName, role.RoleName);
                //AspnetUsersInRole.Delete((UiRCol[0])
                Store4.DataBind();
            }
            else
            {

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Entry not found",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR
                });
            }

        }
    }

    protected void SaveUser(object sender, DirectEventArgs e)
    {
        if (tfID.Text != "")
        {
            string id = tfID.Text;

            if (id != null)
            {
                //MembershipUser usr = Membership.GetUser(id);
                MembershipUser usr = Membership.GetUser(new Guid(id));
                usr.Email = tfEmail.Text;
                usr.Comment = tfComment.Text;

                Membership.UpdateUser(usr);

                GridPanel1.DataBind();

                Window1.Hide();
            }
        }
        else  //new user
        {

            try
            {
                //MembershipCreateStatus status;
                //MembershipUser newUser = Membership.CreateUser(tfUserName.Text, tfPassword.Text, tfEmail.Text, "Name Of Company", "", true, out status);
                MembershipUser newUser = Membership.CreateUser(tfUserName.Text, tfPassword.Text, tfEmail.Text);

                if (newUser == null)
                {
                    //failed
                }
                else
                {
                    //success
                    newUser.Comment = tfComment.Text;
                    //newUser.IsApproved = true;
                    Membership.UpdateUser(newUser);
                    Window1.Hide();
                }
                GridPanel1.DataBind();

            }
            catch (Exception ex)
            {
                lblInfo.Text = ex.Message;
                lblInfo.Hidden = false;
                //throw;
            }
        }

    }

    ///////////////////////////////////////////////////////////

    protected void AcceptRole_Click(object sender, DirectEventArgs e)
    {
        StringBuilder result = new StringBuilder();


        RowSelectionModel sm = this.ARolesGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel userRow = this.GridPanel1.SelectionModel.Primary as RowSelectionModel;

        string UserID = userRow.SelectedRecordID;

        foreach (SelectedRow row in sm.SelectedRows)
        {
            AspnetRole role = new AspnetRole(row.RecordID);
            AspnetUser user = new AspnetUser(UserID);
            Roles.AddUserToRole(user.UserName, role.RoleName);


            role.Save();
        }

        Store4.DataBind();
        AvailableRolesWindow.Hide();
    }

    protected void RolesGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        if (e.Parameters["UserID"] != null && e.Parameters["UserID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["UserID"].ToString());

            AspnetRoleCollection roleCol = new Select()
                      .From(AspnetRole.Schema)
                      .InnerJoin(AspnetUsersInRole.RoleIdColumn, AspnetRole.RoleIdColumn)
                      .Where(AspnetUsersInRole.Columns.UserId).IsEqualTo(Id)
                      .ExecuteAsCollection<AspnetRoleCollection>();

            this.RolesGrid.GetStore().DataSource = roleCol;
            this.RolesGrid.GetStore().DataBind();
        }
    }


    protected void ARoles_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["UserID"] != null && e.Parameters["UserID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["UserID"].ToString());

            AspnetRoleCollection RoleavailCol = new Select()
                    .From(AspnetRole.Schema)
                    .Where(AspnetRole.RoleIdColumn).NotIn(new Select(new String[] { AspnetUsersInRole.Columns.RoleId })
                                                                    .From(AspnetUsersInRole.Schema)
                                                                    .Where(AspnetUsersInRole.UserIdColumn).IsEqualTo(Id))
                    .ExecuteAsCollection<AspnetRoleCollection>();

            Store3.DataSource = RoleavailCol.ToList();
            Store3.DataBind();

        }
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //		var employeeDetailsRender = function ()
        //		{
        //			return '<img class="imgEdit" ext:qtip="Edit" style="cursor:pointer;" src="images/vcard_edit.png" />';
        //		};

        //		var employeeDetailsDelete = function ()
        //		{
        //			return '<img class="imgEdit" ext:qtip="Delete" style="cursor:pointer;" src="images/delete.png" />';
        //		};

        var DoDelete = function (grid, rowIndex, columnIndex, e) {
            var 
                record = rowIndex.id,  // Get the Record
				columnId = rowIndex.id; // Get column id
            //			Ext.Msg.confirm('Confirm', 'Are you sure?', function (btn)
            //			{
            //				//console.log(this, arguments);
            //				if (btn == 'yes')
            //				{
            //					return true 
            //				}
            //				else
            //				{
            //					return false;
            //				}

            //			});

        }


        //		var cellClick = function (grid, rowIndex, columnIndex, e)
        //		{
        //			var t = e.getTarget(),
        //                record = grid.getStore().getAt(rowIndex),  // Get the Record
        //                columnId = grid.getColumnModel().getColumnId(columnIndex); // Get column id

        //			if (t.className == "imgEdit" && columnId == "Details")
        //			{
        //				//the ajax call is allowed
        //				return true;
        //			}

        //			//forbidden
        //			return false;
        //		};
	
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="5 5 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Users" Layout="FitLayout" Hidden="false">
                        <%--width="700" height="400"--%>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="UserAdd" Text="Add User">
                                        <%-- <Listeners>
                            <Click Handler="Ext.Msg.alert('Click','Click on Add');" />                                
                        </Listeners>--%>
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add user" />
                                        </ToolTips>
                                        <DirectEvents>
                                            <Click OnEvent="NewUser" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="Button2" runat="server" Icon="Key" Text="Change Password">
                                        <%-- <Listeners>
                            <Click Handler="Ext.Msg.alert('Click','Click on Add');" />                                
                        </Listeners>--%>
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip2" runat="server" Html="Change Password" />
                                        </ToolTips>
                                        <DirectEvents>
                                            <Click OnEvent="ChangePassword">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true}))"
                                                        Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <%--<ext:ToolbarSeparator/>                        
                    
                    <ext:Button ID="Button4" runat="server" EnableToggle="true" ToggleGroup="Group1" Icon="GroupAdd" Pressed="true" />
                    <ext:Button ID="Button5" runat="server" EnableToggle="true" ToggleGroup="Group1" Icon="GroupDelete" />--%>
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
                                            <ext:JsonReader IDProperty="UserId">
                                                <Fields>
                                                    <ext:RecordField Name="UserId" Type="Auto" />
                                                    <ext:RecordField Name="UserName" Type="String" />
                                                    <ext:RecordField Name="Email" Type="String" />
                                                    <ext:RecordField Name="CreateDate" Type="Date" />
                                                    <%--DateFormat="yyyy/MM/dd"--%>
                                                    <ext:RecordField Name="Comment" Type="String" />
                                                    <%--<ext:RecordField Name="StateProvince" Type="String" />
									<ext:RecordField Name="City" Type="String" />
									<ext:RecordField Name="PostalCode" Type="String" />
									<ext:RecordField Name="LastActivityDate" Type="Date" DateFormat="yyyy/MM/dd" />--%>
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="10" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="UserName" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <%--<ext:Column Header="AddressID" DataIndex="AddressID" />
						
						<ext:Column Header="UserId" DataIndex="UserId" />--%>
                                        <ext:Column Header="User Name" DataIndex="UserName" Width="200" />
                                        <ext:Column Header="Email" DataIndex="Email" Width="200" />
                                        <ext:DateColumn Header="Date Created" DataIndex="CreateDate" Width="100" Format="yyyy-MM-dd" />
                                        <%--<Renderer Handler="return '<b>' + record.data['LastName'] + '</b>,' + record.data['FirstName']" />  
						<ext:Column Header="StateProvince" DataIndex="StateProvince">
							<Renderer Format="UsMoney" />
						</ext:Column>                        
						<ext:Column Header="City" DataIndex="City" />
						<ext:Column Header="PostalCode" DataIndex="PostalCode" Align="Center">
							<Renderer Handler="return (value) ? 'Yes':'No';" />
						</ext:Column>
						<ext:DateColumn Header="ModifiedDate" DataIndex="ModifiedDate" Align="Center" Format="yyyy-MM-dd" />

						<ext:Column ColumnID="Details" 	Header="Details" Width="50" Align="Center" Fixed="true" MenuDisabled="true" Resizable="false">
							<Renderer Fn="employeeDetailsRender" />                    
						</ext:Column>
						<ext:Column ColumnID="Delete" 	Header="Delete" Width="50" Align="Center" Fixed="true" MenuDisabled="true" Resizable="false">
							<Renderer Fn="employeeDetailsDelete" />                    
						</ext:Column>--%>
                                        <ext:CommandColumn Width="50" Header="Options">
                                            <Commands>
                                                <ext:GridCommand Icon="UserDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                                <ext:GridCommand Icon="UserEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <%--<ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />--%>
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
                                            <%--<ext:NumericFilter DataIndex="AddressID" />--%>
                                            <ext:StringFilter DataIndex="UserId" />
                                            <ext:StringFilter DataIndex="UserName" />
                                            <ext:StringFilter DataIndex="Email" />
                                            <ext:DateFilter DataIndex="CreateDate" />
                                            <%--<ext:StringFilter DataIndex="StateProvince" />
							
							<ext:ListFilter DataIndex="City" Options="Austin,Montreal,Toronto,Phoenix,Mesa"/>
							<ext:StringFilter DataIndex="City" />
							<ext:StringFilter DataIndex="PostalCode" />
							<ext:DateFilter DataIndex="ModifiedDate">
								<DatePickerOptions runat="server" TodayText="Now" />
							</ext:DateFilter>--%>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="10" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <%--<CellClick Fn="cellClick" />--%>
                                    <Command Fn="DoDelete" />
                                    <%--<Command Fn="onCommand" />--%>
                                </Listeners>
                                <DirectEvents>
                                    <%--<CellClick 
						OnEvent="ShowDetails" 
						Failure="Ext.MessageBox.alert('Load failed', 'Error during ajax event!');">
						<EventMask ShowMask="true" Target="CustomTarget" CustomTarget="={#{GridPanel1}.body}" />
						<ExtraParams>
							<ext:Parameter Name="id" Value="params[0].getStore().getAt(params[1]).id" Mode="Raw" />
						</ExtraParams>
					</CellClick>--%>
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
                    <ext:Panel ID="pnlSouth" runat="server" Title="Roles" Height="200" Layout="Fit">
                        <TopBar>
                            <ext:Toolbar ID="SouthToolbar" runat="server">
                                <Items>
                                    <ext:Button ID="AddRoleButton" runat="server" Icon="GroupAdd" Text="Add Role">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip3" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Handler="if(Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection()){#{Store3}.reload();#{AvailableRolesWindow}.show()}else{Ext.Msg.alert('Invalid Selection','Select a User.')}" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel ID="RolesGrid" runat="server" Border="false">
                                <Store>
                                    <ext:Store ID="Store4" runat="server" OnRefreshData="RolesGrid_RefreshData">
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
                                            <ext:Parameter Name="UserID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column Header="Role Name" DataIndex="RoleName" Width="200" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="200" />
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteDelete" CommandName="RemoveRoles" Text="" ToolTip-Text="Delete" />
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
                                            <ext:Parameter Name="UserID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
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
        Hidden="true" Collapsible="true" Title="User Details" Maximizable="true" Layout="Fit">
        <Content>
            <ext:Hidden ID="tfID" runat="server">
            </ext:Hidden>
            <ext:FormPanel ID="FormPanel1" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                Padding="5" Width="440" Height="370" ButtonAlign="Right" Layout="RowLayout">
                <Items>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <%--<ext:Label ID="Label2" runat="server" Text="UserName">
						</ext:Label>--%>
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfUserName" runat="server" Disabled="true" FieldLabel="User Name"
                                AnchorHorizontal="90%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <%--<Defaults>
							<ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
							<ext:Parameter Name="MsgTarget" Value="side" />
						</Defaults>--%>
                        <Items>
                            <ext:TextField ID="tfPassword" runat="server" FieldLabel="Password" AnchorHorizontal="90%"
                                Hidden="true" InputType="Password">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <%--<Defaults>
							<ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
							<ext:Parameter Name="MsgTarget" Value="side" />
						</Defaults>--%>
                        <Items>
                            <%--<ext:Label ID="Label3" runat="server" Text="Date Created">
							</ext:Label>--%>
                            <ext:TextField ID="tfCreateDate" runat="server" Disabled="true" FieldLabel="Date Created"
                                AnchorHorizontal="90%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <%--<ext:Label ID="Label1" runat="server" Text="Email">
							</ext:Label>--%>
                            <ext:TextField ID="tfEmail" runat="server" FieldLabel="Email" Vtype="email" AnchorHorizontal="90%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <%--<Defaults>
							<ext:Parameter Name="AllowBlank" Value="true" Mode="Raw" />
							<ext:Parameter Name="MsgTarget" Value="side" />
						</Defaults>--%>
                        <Items>
                            <%--<ext:Label ID="Label4" runat="server" Text="Comment">
							</ext:Label>--%>
                            <ext:TextArea ID="tfComment" runat="server" Height="50" FieldLabel="Comment" AnchorHorizontal="90%">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Label ID="lblInfo" runat="server" Text="" Hidden="true">
                    </ext:Label>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSave" runat="server" Text="Save">
                        <%--<Listeners>
							<Click Handler="if (#{FormPanel1}.getForm().isValid()) {Ext.Msg.alert('Submit', 'Saved!');}else{Ext.Msg.show({icon: Ext.MessageBox.ERROR, msg: 'FormPanel is incorrect', buttons:Ext.Msg.OK});}" />
						</Listeners>--%>
                        <DirectEvents>
                            <Click OnEvent="SaveUser" Method="POST" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? '' : 'Invalid data supplied', iconCls: valid ? 'icon-accept' : 'icon-exclamation'});#{btnSave}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="Window2" runat="server" Width="450" Height="400" Closable="true"
        Hidden="true" Collapsible="true" Title="User Details" Maximizable="true" Layout="Fit">
        <Content>
            <ext:Hidden ID="tfPasswordHidden" runat="server">
            </ext:Hidden>
            <ext:FormPanel ID="FormPanel2" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                Padding="5" Width="440" Height="370" ButtonAlign="Right" Layout="RowLayout">
                <Items>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <Items>
                            <ext:TextField ID="tfPassUser" runat="server" FieldLabel="User Name" AnchorHorizontal="90%"
                                Disabled="true">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <%--ColumnWidth=".5"--%>
                        <%--<ext:Label ID="Label2" runat="server" Text="UserName">
						</ext:Label>--%>
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfPassNew" runat="server" FieldLabel="Password" AnchorHorizontal="90%"
                                InputType="Password">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Label ID="lblPassInfo" runat="server" Text="">
                    </ext:Label>
                </Items>
                <Buttons>
                    <ext:Button ID="BtnPassNew" runat="server" Text="Save">
                        <%--<Listeners>
							<Click Handler="if (#{FormPanel1}.getForm().isValid()) {Ext.Msg.alert('Submit', 'Saved!');}else{Ext.Msg.show({icon: Ext.MessageBox.ERROR, msg: 'FormPanel is incorrect', buttons:Ext.Msg.OK});}" />
						</Listeners>--%>
                        <DirectEvents>
                            <Click OnEvent="SavePass" Method="POST" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? '' : 'Invalid data supplied', iconCls: valid ? 'icon-accept' : 'icon-exclamation'});#{BtnPassNew}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="AvailableRolesWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Roles" Width="620" Height="300" X="50" Y="50" CenterOnLoad="false"
        Icon="GroupGear" Layout="Fit" Hidden="true">
        <Items>
            <ext:GridPanel ID="ARolesGrid" runat="server" Header="false" Border="false">
                <Store>
                    <ext:Store ID="Store3" runat="server" OnRefreshData="ARoles_RefreshData">
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
                            <ext:Parameter Name="UserID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
                                Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:Column Header="Role Name" DataIndex="RoleName" Width="200" />
                        <ext:Column Header="Description" DataIndex="Description" Width="200" />
                    </Columns>
                </ColumnModel>
                <LoadMask ShowMask="true" />
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                </SelectionModel>
                <Buttons>
                    <ext:Button ID="AcceptRole" runat="server" Text="Save" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="AcceptRole_Click">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="_User"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Users.js"></script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout runat="server">
                <North Collapsible="true" Split="true">
                    <ext:Panel ID="pnlNorth" runat="server" Title="Users" Height="600" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="UserAdd" Text="Add User">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add user" />
                                        </ToolTips>
                                        <DirectEvents>
                                            <Click OnEvent="NewUser" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="Button2" runat="server" Icon="Key" Text="Change Password">
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
                                                    <ext:RecordField Name="Comment" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="UserName" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="User Name" DataIndex="UserName" Width="200" />
                                        <ext:Column Header="Email" DataIndex="Email" Width="200" />
                                        <ext:DateColumn Header="Date Created" DataIndex="CreateDate" Width="100" Format="yyyy-MM-dd" />
                                        <ext:CommandColumn Width="50" Header="Options">
                                            <Commands>
                                                <ext:GridCommand Icon="UserDelete" CommandName="Delete" Text="" ToolTip-Text="Delete" />
                                                <ext:GridCommand Icon="UserEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Handler="if (#{pnlRoles}.isVisible()) {#{Store4}.reload();}" Buffer="250" />
                                        </Listeners>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <LoadMask ShowMask="true" />
                                <SaveMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1">
                                        <Filters>
                                            <ext:StringFilter DataIndex="UserId" />
                                            <ext:StringFilter DataIndex="UserName" />
                                            <ext:StringFilter DataIndex="Email" />
                                            <ext:DateFilter DataIndex="CreateDate" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
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
                            <ext:Panel ID="pnlRoles" runat="server" Title="Roles" ClientIDMode="Static" Layout="FitLayout">
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
                                                    <ext:Parameter Name="UserID" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
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
    <ext:Window ID="Window1" runat="server" Width="450" Height="400" Closable="true"
        Hidden="true" Collapsible="true" Title="User Details" Maximizable="true" Layout="Fit"
        ClientIDMode="Static">
        <Content>
            <ext:Hidden ID="tfID" runat="server">
            </ext:Hidden>
            <ext:FormPanel ID="FormPanel1" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                Padding="5" Width="440" Height="370" ButtonAlign="Right" Layout="RowLayout">
                <Items>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfUserName" runat="server" Disabled="true" FieldLabel="User Name"
                                AnchorHorizontal="90%" ClientIDMode="Static">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="tfPassword" runat="server" FieldLabel="Password" AnchorHorizontal="90%"
                                Hidden="true" InputType="Password" ClientIDMode="Static">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="tfCreateDate" runat="server" Disabled="true" FieldLabel="Date Created"
                                AnchorHorizontal="90%">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfEmail" runat="server" FieldLabel="Email" Vtype="email" AnchorHorizontal="90%">
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
                            <Click OnEvent="SaveUser" Method="POST" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? '' : 'Invalid data supplied', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});#{btnSave}.setDisabled(!valid);" />
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
                        <Items>
                            <ext:TextField ID="tfPassUser" runat="server" FieldLabel="User Name" AnchorHorizontal="90%"
                                Disabled="true">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
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
                        <DirectEvents>
                            <Click OnEvent="SavePass" Method="POST" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? '' : 'Invalid data supplied', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});#{BtnPassNew}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    <ext:Window ID="AvailableRolesWindow" runat="server" Collapsible="false" Maximizable="false"
        Title="Available Roles" Width="620" Height="300" X="50" Y="50"
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
</asp:Content>

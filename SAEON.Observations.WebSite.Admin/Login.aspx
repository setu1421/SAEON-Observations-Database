<%@ Page Language="C#" CodeFile="Login.aspx.cs" Inherits="_Login" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Window ID="Window1" runat="server" Closable="false" Resizable="false" Height="150"
        Icon="Lock" Title="Login" Draggable="false" Width="350" Modal="true" Padding="5"
        Layout="Form">
        <Items>
            <ext:TextField ID="txtUsername" runat="server" FieldLabel="Username" AllowBlank="false"
                BlankText="Your username is required." AnchorHorizontal="100%" />
            <ext:TextField ID="txtPassword" runat="server" InputType="Password" FieldLabel="Password"
                AllowBlank="false" BlankText="Your password is required." AnchorHorizontal="100%" />
            <ext:Checkbox ID="cbRememberMe" runat="server" FieldLabel="Remember me">
            </ext:Checkbox>
        </Items>
        <Buttons>
            <ext:Button ID="btnLogin" runat="server" Text="Login" Icon="Accept">
                <Listeners>
                    <Click Handler="
                            if (!#{txtUsername}.validate() || !#{txtPassword}.validate())
                            {
                                Ext.Msg.show(
                                {
                                    icon: Ext.MessageBox.ERROR, msg: 'Username and password is required', buttons: Ext.Msg.OK
                                });

                                return false;

                             }" />
                </Listeners>
                <DirectEvents>
                    <Click OnEvent="btnLogin_Click">
                        <EventMask ShowMask="true" Msg="Verifying..." MinDelay="500" />
                    </Click>
                </DirectEvents>
            </ext:Button>
            <%-- <ext:Button ID="btnCancel" runat="server" Text="Cancel" Icon="Decline">
                    <Listeners>
                        <Click Handler="#{Window1}.hide();#{lblMessage}.setText('LOGIN CANCELED')" />
                    </Listeners>
                </ext:Button>--%>
        </Buttons>
    </ext:Window>
</asp:Content>

<%@ Page Language="C#" CodeFile="ResetPassword.aspx.cs" Inherits="Admin_ResetPassword" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Window ID="Window1" runat="server" Closable="false" Resizable="false" Height="150"
        Icon="Lock" Title="Reset Password" Draggable="false" Width="350" Modal="true" Padding="5"
        Layout="Form">
        <Items>
            <ext:TextField ID="txtUsername" runat="server" FieldLabel="Username" AllowBlank="false"
                BlankText="Your username is required." AnchorHorizontal="100%" />
            <ext:TextField ID="txtPassword" runat="server" InputType="Password" FieldLabel="Password"
                AllowBlank="false" BlankText="Your password is required." AnchorHorizontal="100%" />
        </Items>
        <Buttons>
            <ext:Button ID="btnReset" runat="server" Text="Reset" Icon="Accept">
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
                    <Click OnEvent="btnResetPassword_Click">
                        <EventMask ShowMask="true" Msg="Resetting..." MinDelay="500" />
                    </Click>
                </DirectEvents>
            </ext:Button>
        </Buttons>
    </ext:Window>
</asp:Content>

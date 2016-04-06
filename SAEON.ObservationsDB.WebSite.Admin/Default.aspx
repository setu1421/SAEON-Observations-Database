<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <script type="text/javascript" src="JS/Default.js"></script>
    <ext:Viewport ID="Viewport1" runat="server" Layout="border">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Header="false" Region="North" Border="false" ButtonAlign="Right"
                Html="<div id='header' style='height:32px;'><div class='main-title'>Environmental Observations</div><div style='float:right;margin-right:10px;margin-top:5px;'><table style='width: auto;' id='ContentPlaceHolder1_LogoutButton' class='x-btn x-btn-text-icon' cellSpacing='0' onclick='Ext.net.DirectEvent.confirmRequest({control:this});'><tbody class='x-btn-small x-btn-icon-small-left'><tr><td class='x-btn-tl'><i>&nbsp;</i></td><td class='x-btn-tc'></td><td class='x-btn-tr'><i>&nbsp;</i></td></tr><tr><td class='x-btn-ml'><i>&nbsp;</i></td><td class='x-btn-mc'><em unselectable='on'><button id='ext-gen32' class='x-btn-text lock-open1' type='button'>Logout</button></em></td><td class='x-btn-mr'><i>&nbsp;</i></td></tr><tr><td class='x-btn-bl'><i>&nbsp;</i></td><td class='x-btn-bc'></td><td class='x-btn-br'><i>&nbsp;</i></td></tr></tbody></table></div></div>">
                 <Items>
                    <ext:Button ID="LogoutButton" runat="server" Text="Logout" Icon="LockOpen"  OnDirectClick="Logout" Style="display:none;">
                    </ext:Button>
                </Items>
            </ext:Panel>
           
            <ext:Panel ID="PanelW" runat="server" Title="Navigation" Region="West" Layout="accordion"
                Width="225" MinWidth="225" MaxWidth="400" Collapsible="true" Border="false" Split="false"
                ClientIDMode="Static">
                <Items>
                </Items>
            </ext:Panel>
            <ext:TabPanel ID="PanelC" runat="server" Region="Center" EnableTabScroll="true" ClientIDMode="Static">
                <Items>
                </Items>
                <Plugins>
                    <ext:TabCloseMenu ID="TabCloseMenu1" runat="server" />
                </Plugins>
            </ext:TabPanel>
        </Items>
    </ext:Viewport>
</asp:Content>

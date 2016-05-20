<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="GetKeys.aspx.cs" Inherits="Admin_GetKeys" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table style="width:100%;">
        <tr>
            <td>Key</td>
            <td>Value</td>
        </tr>
        <tr>
            <td>Decryption</td>
            <td><asp:Label ID="Decryption" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>DecryptionKey</td>
            <td><asp:Label ID="DecryptionKey" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>DecryptionKeyValue</td>
            <td><asp:Label ID="DecryptionKeyValue" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>Validation</td>
            <td><asp:Label ID="Validation" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>ValidationAlgorithm</td>
            <td><asp:Label ID="ValidationAlgorithm" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>ValidationKey</td>
            <td><asp:Label ID="ValidationKey" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>ValidationKeyValue</td>
            <td><asp:Label ID="ValidationKeyValue" runat="server" Text=""></asp:Label></td>
        </tr>
    </table>
</asp:Content>


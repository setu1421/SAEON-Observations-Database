<%@ Page Title="Import Setup" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ImportSetup.aspx.cs" Inherits="Admin_ImportSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:FormPanel ID="BasicForm" runat="server" Title="" Width="700px" LabelWidth="150" ClientIDMode="Static" MonitorValid="true">
                <Items>
                    <ext:FileUploadField ID="TemplateFile" runat="server" FieldLabel="Template Spreadsheet" AllowBlank="false" EmptyText="Select a Template Spreadsheet" Width="500px" Icon="Attach" />
                </Items>
                <Listeners>
                    <ClientValidation Handler="#{CreateButton}.setDisabled(!valid);" />
                </Listeners>
                <Buttons>
                    <ext:Button ID="CreateButton" runat="server" Text="Create">
                        <DirectEvents>
                            <Click
                                OnEvent="CreateClick"
                                Before="if (!#{BasicForm}.getForm().isValid()) { return false; } 
                                Ext.Msg.wait('Importing setup from template spreadsheet...', 'Information');"
                                Failure="Ext.Msg.show({ 
                                title   : 'Error', 
                                msg     : 'Error importing setup', 
                                minWidth: 200, 
                                modal   : true, 
                                icon    : Ext.Msg.ERROR, 
                                buttons : Ext.Msg.OK 
                            });">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button runat="server" Text="Reset">
                        <Listeners>
                            <Click Handler="#{BasicForm}.getForm().reset();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Viewport>
</asp:Content>


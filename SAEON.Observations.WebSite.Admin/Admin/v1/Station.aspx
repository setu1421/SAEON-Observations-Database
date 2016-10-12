<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Station.aspx.cs" Inherits="_Station"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="/JS/Station.js"></script>
    <script type="text/javascript" src="/JS/generic.js"></script>
    <script type="text/javascript">
        var submitValue = function (format) {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(StationGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(StationGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);

            StationGrid.submitData(false);
        };
    </script>
</asp:Content>
<asp:Content ID="Content" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <ext:Hidden ID="GridData" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="VisCols" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="FormatType" runat="server" ClientIDMode="Static" />
    <ext:Hidden ID="SortInfo" runat="server" ClientIDMode="Static" />
    <ext:Store ID="ProjectSiteStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Id">
                <Fields>
                    <ext:RecordField Name="Id" Type="String" />
                    <ext:RecordField Name="Name" Type="String" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="5 0 0 5">
                    <ext:Panel ID="Panel1" runat="server" Title="Stations" Layout="FitLayout" Hidden="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Icon="Add" Text="Add Station">
                                        <ToolTips>
                                            <ext:ToolTip ID="ToolTip1" runat="server" Html="Add" />
                                        </ToolTips>
                                        <Listeners>
                                            <Click Fn="New" />
                                        </Listeners>
                                    </ext:Button>
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
                            <ext:GridPanel ID="StationGrid" runat="server" Border="false" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="StationStore_RefreshData" OnSubmitData="StationStore_Submit">
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Code" Type="String" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                    <ext:RecordField Name="ProjectSiteID" Type="Auto" />
                                                    <ext:RecordField Name="ProjectSiteName" Type="String" />
                                                    <ext:RecordField Name="Description" Type="String" />
                                                    <ext:RecordField Name="Url" Type="String" />
                                                    <ext:RecordField Name="Latitude" Type="Auto" />
                                                    <ext:RecordField Name="Longitude" Type="Auto" />
                                                    <ext:RecordField Name="Elevation" Type="Auto" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                            <ext:Parameter Name="limit" Value="25" Mode="Raw" />
                                            <ext:Parameter Name="sort" Value="" />
                                            <ext:Parameter Name="dir" Value="" />
                                        </BaseParams>
                                        <SortInfo Field="Name" Direction="ASC" />
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Code" DataIndex="Code" Width="200" Groupable="false" />
                                        <ext:Column Header="Name" DataIndex="Name" Width="200" Groupable="false" />
                                        <ext:Column Header="Project / Site" DataIndex="ProjectSiteName" Width="100" />
                                        <ext:Column Header="Url" DataIndex="Url" Width="150" Groupable="false" />
                                        <ext:Column Header="Latitude" DataIndex="Latitude" Width="70" Groupable="false" />
                                        <ext:Column Header="Longitude" DataIndex="Longitude" Width="70" Groupable="false" />
                                        <ext:Column Header="Description" DataIndex="Description" Width="100" Groupable="false" />
                                        <ext:CommandColumn Width="50" Groupable="false">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                                </SelectionModel>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" HideGroupedColumn="false" runat="server" ForceFit="true"
                                        StartCollapsed="true" GroupTextTpl='<span id="Project / Site-{[values.rs[0].data.OrganisationName]}"></span>{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                                        EnableRowBody="true">
                                    </ext:GroupingView>
                                </View>
                                <LoadMask ShowMask="true" />
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1">
                                        <Filters>
                                            <ext:StringFilter DataIndex="ID" />
                                            <ext:StringFilter DataIndex="Code" />
                                            <ext:StringFilter DataIndex="Name" />
                                            <ext:StringFilter DataIndex="Description" />
                                            <ext:StringFilter DataIndex="Url" />
                                            <ext:NumericFilter DataIndex="Latitude" />
                                            <ext:NumericFilter DataIndex="Longitude" />
                                            <ext:NumericFilter DataIndex="Elevation" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                                </BottomBar>
                                <Listeners>
                                    <%--<Command Handler="#{DetailsFormPanel}.getForm().reset();#{DetailsFormPanel}.getForm().loadRecord(record);#{DetailsFormPanel}.clearInvalid();#{DetailWindow}.show()" />--%>
                                    <Command Fn="onCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <East Collapsible="true" Split="true" MarginsSummary="5 5 0 0">
                </East>
                <South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="700" Height="400" Closable="true"
        Hidden="true" Collapsible="false" Title="Station Detail" Maximizable="false"
        Layout="FitLayout" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                Padding="10" Width="490" ButtonAlign="Right" MonitorResize="true" Layout="FitLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Container ID="Container1" runat="server" Layout="Column" Height="100">
                        <Items>
                            <ext:Container ID="Container2" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfCode" AllowBlank="false" DataIndex="Code" IsRemoteValidation="true"
                                        MaxLength="50" runat="server" FieldLabel="Code" BlankText="Code is a required"
                                        MsgTarget="Side" AnchorHorizontal="93%" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbProjectSite" runat="server" StoreID="ProjectSiteStore" Editable="true"
                                        BlankText="Project / Site is required" MsgTarget="Side" DisplayField="Name" ValueField="Id"
                                        TypeAhead="true" Mode="Local" ForceSelection="true" TriggerAction="All" AllowBlank="false"
                                        DataIndex="ProjectSiteID" EmptyText="Select Project / site" SelectOnFocus="true"
                                        FieldLabel="Project / Site" AnchorHorizontal="93%" ClientIDMode="Static">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container3" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfName" AllowBlank="false" DataIndex="Name" IsRemoteValidation="true"
                                        MaxLength="150" runat="server" FieldLabel="Name" AnchorHorizontal="95%" MsgTarget="Side"
                                        BlankText="Name is a required" ClientIDMode="Static">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                    <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="150" runat="server" FieldLabel="Url"
                                        AnchorHorizontal="95%" MsgTarget="Side" ClientIDMode="Static">
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Panel4" runat="server" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Description is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" runat="server"
                                AllowBlank="false" BlankText="Description is required" MsgTarget="Side" FieldLabel="Description"
                                AnchorHorizontal="95%">
                            </ext:TextArea>
                        </Items>
                    </ext:Container>
                    <ext:Container ID="Container4" runat="server" Layout="Column" Height="50" Margins="10px 0px 0px 0px">
                        <Items>
                            <ext:Container ID="Container8" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLatitude" DataIndex="Latitude"
                                        MaxLength="150" runat="server" FieldLabel="Latitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container9" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="true" DecimalPrecision="6" ID="nfLongitude" DataIndex="Longitude"
                                        MaxLength="150" runat="server" FieldLabel="Longitude" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container10" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".33">
                                <Items>
                                    <ext:NumberField AllowDecimals="false" ID="nfElevation" DataIndex="Elevation" MaxLength="7"
                                        runat="server" FieldLabel="Elevation" AnchorHorizontal="90%">
                                    </ext:NumberField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSave" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="Save" Method="POST">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar1" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept1' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
</asp:Content>

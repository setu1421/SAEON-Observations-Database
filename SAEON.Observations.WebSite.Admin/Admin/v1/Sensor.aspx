<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sensor.aspx.cs" Inherits="_Sensor"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../JS/Sensor.js"></script>
    <script type="text/javascript" src="../JS/generic.js"></script>

    <script type="text/javascript">
        var submitValue = function (format)
        {
            GridData.setValue(Ext.encode(ContentPlaceHolder1_GridFilters1.buildQuery(ContentPlaceHolder1_GridFilters1.getFilterData())));
            //VisCols.setValue(Ext.encode(ContentPlaceHolder1_SensorGrid.getRowsValues({ visibleOnly: true, excludeId: true })[0]));
            var viscolsNew = makenewJsonForExport(ContentPlaceHolder1_SensorGrid.getColumnModel().getColumnsBy(function (column, colIndex) { return !this.isHidden(colIndex); }))
            VisCols.setValue(viscolsNew);
            FormatType.setValue(format);
            SortInfo.setValue(ContentPlaceHolder1_GridFilters1.store.sortInfo.field + "|" + ContentPlaceHolder1_GridFilters1.store.sortInfo.direction);

            ContentPlaceHolder1_SensorGrid.submitData(false);
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
            <ext:Panel ID="Panel1" runat="server" Title="Sensor Procedure" Layout="FitLayout"
                Hidden="false">
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Icon="Add" Text="Add Sensor Procedure">
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
                    <ext:GridPanel ID="SensorGrid" runat="server" Border="false">
                        <Store>
                            <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="SensorStore_RefreshData" OnSubmitData="SensorStore_Submit">
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>
                                <Reader>
                                    <ext:JsonReader IDProperty="Id">
                                        <Fields>
                                            <ext:RecordField Name="Id" Type="Auto" />
                                            <ext:RecordField Name="Code" Type="String" />
                                            <ext:RecordField Name="Name" Type="String" />
                                            <ext:RecordField Name="Description" Type="String" />
                                            <ext:RecordField Name="Url" Type="String" />
                                            <ext:RecordField Name="StationID" Type="String" />
                                            <ext:RecordField Name="PhenomenonID" Type="Auto" />
                                            <ext:RecordField Name="DataSourceID" Type="Auto" />
                                            <ext:RecordField Name="DataSourceName" Type="String" />
                                            <ext:RecordField Name="DataSchemaName" Type="String" />
                                            <ext:RecordField Name="UserId" Type="String" />
                                            <ext:RecordField Name="DataSchemaID" Type="Auto" />
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
                                <ext:Column Header="Code" DataIndex="Code" Width="100" />
                                <ext:Column Header="Name" DataIndex="Name" Width="100" />
                                <ext:Column Header="Description" DataIndex="Description" Width="200" />
                                <ext:Column Header="Source Name" DataIndex="DataSourceName" Width="200" />
                                <ext:Column Header="Schema Name" DataIndex="DataSchemaName" Width="200" />
                                <ext:CommandColumn Width="50">
                                    <Commands>
                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="" ToolTip-Text="Edit" />
                                    </Commands>
                                </ext:CommandColumn>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                        </SelectionModel>
                        <LoadMask ShowMask="true" />
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1">
                                <Filters>
                                    <ext:StringFilter DataIndex="ID" />
                                    <ext:StringFilter DataIndex="Code" />
                                    <ext:StringFilter DataIndex="Name" />
                                    <ext:StringFilter DataIndex="Description" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="25" EmptyMsg="No data found" />
                        </BottomBar>
                        <Listeners>
                            <Command Fn="onCommand" />
                        </Listeners>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    <ext:Window ID="DetailWindow" runat="server" Width="500" Height="500" Closable="true"
        Hidden="true" Collapsible="false" Title="Sensor Procedure Detail" Maximizable="false"
        Layout="Fit" ClientIDMode="Static">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="500" Height="500" ButtonAlign="Right"
                Layout="RowLayout" ClientIDMode="Static">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Container ID="Container1" runat="server" Layout="Column" Height="50">
                        <Items>
                            <ext:Container ID="Container2" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                        runat="server" FieldLabel="Code" AnchorHorizontal="90%" ClientIDMode="Static"
                                        MsgTarget="Side" AllowBlank="false" BlankText="Code is a required">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                            <ext:Container ID="Container5" runat="server" LabelAlign="Top" Layout="Form" ColumnWidth=".5">
                                <Items>
                                    <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                        runat="server" FieldLabel="Name" AnchorHorizontal="90%" ClientIDMode="Static"
                                        MsgTarget="Side" AllowBlank="false" BlankText="Name is a required">
                                        <RemoteValidation OnValidation="ValidateField" />
                                    </ext:TextField>
                                </Items>
                            </ext:Container>
                        </Items>
                    </ext:Container>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="250" IsRemoteValidation="false"
                                runat="server" FieldLabel="URL" AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="false"
                                BlankText="Url is a required">
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="150" runat="server"
                                FieldLabel="Description" AnchorHorizontal="95%" MsgTarget="Side" AllowBlank="false"
                                BlankText="Description is a required">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="sbStation" runat="server" DataIndex="StationID" DisplayField="Name"
                                ValueField="ID" FieldLabel="Station" EmptyText="Please select" AnchorHorizontal="95%"
                                MsgTarget="Side" AllowBlank="false" BlankText="Station is required">
                                <Store>
                                    <ext:Store ID="storeStation" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Name" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:SelectBox ID="sbPhenomenon" runat="server" DataIndex="PhenomenonID" DisplayField="Name"
                                ValueField="ID" FieldLabel="Phenomenon" EmptyText="Please select" AnchorHorizontal="95%"
                                MsgTarget="Side" AllowBlank="false" BlankText="Phenomenon is required">
                                <Store>
                                    <ext:Store ID="storePhenomenon" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Name" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:ComboBox ID="cbDataSource" runat="server" DataIndex="DataSourceID" DisplayField="Name"
                                ValueField="ID" FieldLabel="DataSource" EmptyText="Please select" AnchorHorizontal="95%"
                                MsgTarget="Side" AllowBlank="false" BlankText="DataSource is required" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="storeDataSource" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Name" />
                                                    <ext:RecordField Name="DataSchemaID" Type="Auto" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <Select Fn="SelectDataSource" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Items>
                            <ext:ComboBox ID="cbDataSchema" runat="server" Editable="true" ValueField="Id" DisplayField="Name"
                                TypeAhead="true" Mode="Local" AllowBlank="true" BlankText="Data Schema is required for the selected Data Source"
                                TriggerAction="All" DataIndex="DataSchemaID" EmptyText="Select Data Schema" SelectOnFocus="true" MsgTarget="Side"
                                FieldLabel="Data Schema" AnchorHorizontal="95%" ClientIDMode="Static">
                                <Store>
                                    <ext:Store ID="DataSchemaStore" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" Type="Auto" />
                                                    <ext:RecordField Name="Name" Type="String" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.clearValue();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
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

<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    protected void SensorStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SensorGrid.GetStore().DataSource = SensorRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        SensorCollection col = new SensorCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Sensor.Columns.Code;
            errorMessage = "The specified Sensor Procedure Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Sensor.Columns.Name;
            errorMessage = "The specified Sensor Procedure Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new SensorCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new SensorCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Offering.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

        if (col.Count > 0)
        {
            e.Success = false;
            e.ErrorMessage = errorMessage;
        }
        else
            e.Success = true;

    }

    protected void Save(object sender, DirectEventArgs e)
    {

        Sensor org = new Sensor();

        if (String.IsNullOrEmpty(tfID.Text))
            org.Id = Guid.NewGuid();
        else
            org = new Sensor(tfID.Text.Trim());

        org.Code = tfCode.Text.Trim();
        org.Name = tfName.Text.Trim();
        org.Description = tfDescription.Text.Trim();
        org.UserId = AuthHelper.GetLoggedInUserId;
        org.Url = tfUrl.Text.Trim();
        org.StationID = Guid.Parse(sbStation.SelectedItem.Value);
        org.PhenomenonID = Guid.Parse(sbPhenomenon.SelectedItem.Value);
        org.DataSourceID = Guid.Parse(sbDataSource.SelectedItem.Value);
        org.Save();

        SensorGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!X.IsAjaxRequest)
        {
            var store = sbStation.GetStore();
            SubSonic.SqlQuery q = new Select("ID", "Description").From(Station.Schema).Where("ID").IsNotNull();
            System.Data.DataSet ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();

            store = sbPhenomenon.GetStore();
            q = new Select("ID", "Description").From(Phenomenon.Schema).Where("ID").IsNotNull();
            ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();

            store = sbDataSource.GetStore();
            q = new Select("ID", "Description").From(DataSource.Schema).Where("ID").IsNotNull();
            ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();
        }

    }


    
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .icon-exclamation
        {
            padding-left: 25px !important;
            background: url(/icons/exclamation-png/ext.axd) no-repeat 3px 3px !important;
        }
        
        .icon-accept
        {
            padding-left: 25px !important;
            background: url(/icons/accept-png/ext.axd) no-repeat 3px 3px !important;
        }
    </style>
    <script type="text/javascript">
        function onCommand(e, record) {
            DetailsFormPanel.getForm().reset();
            DetailsFormPanel.getForm().loadRecord(record);
            DetailsFormPanel.getForm().clearInvalid();

            tfCode.rvConfig.remoteValidated = false;
            tfCode.rvConfig.remoteValid = false;

            tfName.rvConfig.remoteValidated = false;
            tfName.rvConfig.remoteValid = false;

            tfCode.markAsValid();
            tfName.markAsValid();

            DetailWindow.show()
        }

        function New() {

            DetailsFormPanel.getForm().reset();
            tfCode.rvConfig.remoteValidated = false;
            tfCode.rvConfig.remoteValid = false;

            tfName.rvConfig.remoteValidated = false;
            tfName.rvConfig.remoteValid = false;

            DetailWindow.show();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
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
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:GridPanel ID="SensorGrid" runat="server" Border="false">
                        <Store>
                            <ext:Store ID="Store2" runat="server" RemoteSort="true" OnRefreshData="SensorStore_RefreshData">
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
                                            <ext:RecordField Name="PhenomenonID" Type="String" />
                                            <ext:RecordField Name="DataSourceID" Type="String" />
                                            <ext:RecordField Name="UserId" Type="String" />
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
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Header="Code" DataIndex="Code" Width="200" />
                                <ext:Column Header="Name" DataIndex="Name" Width="200" />
                                <ext:Column Header="Description" DataIndex="Description" Width="200" />
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
    <ext:Window ID="DetailWindow" runat="server" Width="450" Height="500" Closable="true"
        Hidden="true" Collapsible="false" Title="Sensor Procedure Detail" Maximizable="false"
        Layout="Fit">
        <Content>
            <ext:FormPanel ID="DetailsFormPanel" runat="server" Title="" MonitorPoll="500" MonitorValid="true"
                MonitorResize="true" Padding="10" Width="440" Height="500" ButtonAlign="Right"
                Layout="RowLayout">
                <LoadMask ShowMask="true" />
                <Items>
                    <ext:Hidden ID="tfID" DataIndex="Id" runat="server">
                    </ext:Hidden>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Code is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfCode" DataIndex="Code" IsRemoteValidation="true" MaxLength="50"
                                runat="server" FieldLabel="Code" AnchorHorizontal="95%">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Name is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfName" DataIndex="Name" MaxLength="150" IsRemoteValidation="true"
                                runat="server" FieldLabel="Name" AnchorHorizontal="95%">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Name is a required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextField ID="tfUrl" DataIndex="Url" MaxLength="250" IsRemoteValidation="false"
                                runat="server" FieldLabel="URL" AnchorHorizontal="95%">
                                <RemoteValidation OnValidation="ValidateField" />
                            </ext:TextField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Description is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:TextArea ID="tfDescription" DataIndex="Description" MaxLength="150" runat="server"
                                FieldLabel="Description" AnchorHorizontal="95%">
                            </ext:TextArea>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Layout="Form"
                        LabelAlign="Top">
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Station is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:SelectBox ID="sbStation" runat="server" DataIndex="StationID" DisplayField="Description"
                                ValueField="ID" FieldLabel="Station" EmptyText="Please select">
                                <Store>
                                    <ext:Store ID="storeStation" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Description" />
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
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="Phenomenon is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:SelectBox ID="sbPhenomenon" runat="server" DataIndex="PhenomenonID" DisplayField="Description"
                                ValueField="ID" FieldLabel="Phenomenon" EmptyText="Please select">
                                <Store>
                                    <ext:Store ID="storePhenomenon" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Description" />
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
                        <Defaults>
                            <ext:Parameter Name="AllowBlank" Value="false" Mode="Raw" />
                            <ext:Parameter Name="blankText" Value="DataSource is required" Mode="Value" />
                            <ext:Parameter Name="MsgTarget" Value="side" />
                        </Defaults>
                        <Items>
                            <ext:SelectBox ID="sbDataSource" runat="server" DataIndex="DataSourceID" DisplayField="Description"
                                ValueField="ID" FieldLabel="DataSource" EmptyText="Please select">
                                <Store>
                                    <ext:Store ID="storeDataSource" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Description" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:SelectBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button ID="btnSave" runat="server" Text="Save" FormBind="true">
                        <DirectEvents>
                            <Click OnEvent="Save" Method="GET">
                                <EventMask ShowMask="true" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <BottomBar>
                    <ext:StatusBar ID="StatusBar1" runat="server" Height="25" />
                </BottomBar>
                <Listeners>
                    <ClientValidation Handler="this.getBottomToolbar().setStatus({text : valid ? 'Form is valid' : 'Form is invalid', iconCls: valid ? 'icon-accept' : 'icon-exclamation'});" />
                </Listeners>
            </ext:FormPanel>
        </Content>
    </ext:Window>
    </form>
</body>
</html>

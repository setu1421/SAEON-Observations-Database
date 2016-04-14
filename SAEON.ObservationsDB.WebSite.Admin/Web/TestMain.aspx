<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    public class Composer
    {
        public Composer(string name) { this.Name = name; }
        public string Name { get; set; }

        private List<Composition> compositions;
        public List<Composition> Compositions
        {
            get
            {
                if (this.compositions == null)
                {
                    this.compositions = new List<Composition>();
                }
                return this.compositions;
            }
        }
    }

    public class Composition
    {
        public Composition() { }

        public Composition(CompositionType type)
        {
            this.Type = type;
        }

        public CompositionType Type { get; set; }

        private List<Piece> pieces;
        public List<Piece> Pieces
        {
            get
            {
                if (this.pieces == null)
                {
                    this.pieces = new List<Piece>();
                }
                return this.pieces;
            }
        }
    }

    public class Piece
    {
        public Piece() { }

        public Piece(string title)
        {
            this.Title = title;
        }

        public string Title { get; set; }
    }

    public enum CompositionType
    {
        Concertos,
        Quartets,
        Sonatas,
        Symphonies
    }

    public List<Composer> GetData()
    {
        Composer beethoven = new Composer("Beethoven");

        Composition beethovenConcertos = new Composition(CompositionType.Concertos);
        Composition beethovenQuartets = new Composition(CompositionType.Quartets);
        Composition beethovenSonatas = new Composition(CompositionType.Sonatas);
        Composition beethovenSymphonies = new Composition(CompositionType.Symphonies);

        beethovenConcertos.Pieces.AddRange(new List<Piece> { 
            new Piece{ Title = "No. 1 - C" },
            new Piece{ Title = "No. 2 - B-Flat Major" },
            new Piece{ Title = "No. 3 - C Minor" },
            new Piece{ Title = "No. 4 - G Major" },
            new Piece{ Title = "No. 5 - E-Flat Major" }
        });

        beethovenQuartets.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Six String Quartets" },
            new Piece{ Title = "Three String Quartets" },
            new Piece{ Title = "Grosse Fugue for String Quartets" }
        });

        beethovenSonatas.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Sonata in A Minor" },
            new Piece{ Title = "sonata in F Major" }
        });

        beethovenSymphonies.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "No. 1 - C Major" },
            new Piece{ Title = "No. 2 - D Major" },
            new Piece{ Title = "No. 3 - E-Flat Major" },
            new Piece{ Title = "No. 4 - B-Flat Major" },
            new Piece{ Title = "No. 5 - C Minor" },
            new Piece{ Title = "No. 6 - F Major" },
            new Piece{ Title = "No. 7 - A Major" },
            new Piece{ Title = "No. 8 - F Major" },
            new Piece{ Title = "No. 9 - D Minor" }
        });

        beethoven.Compositions.AddRange(new List<Composition>{
            beethovenConcertos, 
            beethovenQuartets,
            beethovenSonatas,
            beethovenSymphonies 
        });


        Composer brahms = new Composer("Brahms");

        Composition brahmsConcertos = new Composition(CompositionType.Concertos);
        Composition brahmsQuartets = new Composition(CompositionType.Quartets);
        Composition brahmsSonatas = new Composition(CompositionType.Sonatas);
        Composition brahmsSymphonies = new Composition(CompositionType.Symphonies);

        brahmsConcertos.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Violin Concerto" },
            new Piece{ Title = "Double Concerto - A Minor" },
            new Piece{ Title = "Piano Concerto No. 1 - D Minor" },
            new Piece{ Title = "Piano Concerto No. 2 - B-Flat Major" }
        });

        brahmsQuartets.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Piano Quartet No. 1 - G Minor" },
            new Piece{ Title = "Piano Quartet No. 2 - A Major" },
            new Piece{ Title = "Piano Quartet No. 3 - C Minor" },
            new Piece{ Title = "Piano Quartet No. 3 - B-Flat Minor" }
        });

        brahmsSonatas.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Two Sonatas for Clarinet - F Minor" },
            new Piece{ Title = "Two Sonatas for Clarinet - E-Flat Major" }
        });

        brahmsSymphonies.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "No. 1 - C Minor" },
            new Piece{ Title = "No. 2 - D Minor" },
            new Piece{ Title = "No. 3 - F Major" },
            new Piece{ Title = "No. 4 - E Minor" }
        });

        brahms.Compositions.AddRange(new List<Composition>{
            brahmsConcertos, 
            brahmsQuartets,
            brahmsSonatas,
            brahmsSymphonies 
        });


        Composer mozart = new Composer("Mozart");

        Composition mozartConcertos = new Composition(CompositionType.Concertos);

        mozartConcertos.Pieces.AddRange(new List<Piece> {
            new Piece{ Title = "Piano Concerto No. 12" },
            new Piece{ Title = "Piano Concerto No. 17" },
            new Piece{ Title = "Clarinet Concerto" },
            new Piece{ Title = "Violin Concerto No. 5" },
            new Piece{ Title = "Violin Concerto No. 4" }
        });

        mozart.Compositions.Add(mozartConcertos);

        return new List<Composer> { beethoven, brahms, mozart };
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //// Build Panel for West Region
        //Ext.Net.Panel pnl = new Ext.Net.Panel();
        //pnl.ID = "Panel1";
        //pnl.Title = "Navigation";
        //pnl.Width = Unit.Pixel(175);

        //// Build TabPanel for Center Region
        //Ext.Net.Panel tab1 = new Ext.Net.Panel();
        //tab1.Title = "First Tab";
        //tab1.BodyStyle = "padding: 6px;";
        //tab1.Html = "Welcome message";

        //TabPanel tp = new TabPanel();
        //tp.ID = "TabPanelC";
        //tp.Region = Region.Center;

        //// Set first Tab to be the .ActiveTabIndex
        //tp.ActiveTabIndex = 0;

        //// Add Tabs to TabPanel
        //tp.Items.Add(tab1);
        ////tp.Items.Add(tab2);
        ////tp.Items.Add(tab3);

        //Viewport1.Items.Add(tp);

        //----------------------------------------------------------------------------
        //Ext.Net.Button btnExpand = new Ext.Net.Button();
        //btnExpand.Text = "Expand All";
        //btnExpand.Listeners.Click.Handler = tree.ClientID + ".expandAll();";

        //Ext.Net.Button btnCollapse = new Ext.Net.Button();
        //btnCollapse.Text = "Collapse All";
        //btnCollapse.Listeners.Click.Handler = tree.ClientID + ".collapseAll();";

        //Toolbar toolBar = new Toolbar();
        //toolBar.ID = "Toolbar1";
        //toolBar.Items.Add(btnExpand);
        //toolBar.Items.Add(btnCollapse);
        //tree.TopBar.Add(toolBar);

        //StatusBar statusBar = new StatusBar();
        //statusBar.AutoClear = 1000;
        //tree.BottomBar.Add(statusBar);

        //tree.Listeners.Click.Handler = statusBar.ClientID + ".setStatus({text: 'Node Selected: <b>' + node.text + '</b>', clear: true});";
        //tree.Listeners.ExpandNode.Handler = statusBar.ClientID + ".setStatus({text: 'Node Expanded: <b>' + node.text + '</b>', clear: true});";
        //tree.Listeners.ExpandNode.Delay = 30;
        //tree.Listeners.CollapseNode.Handler = statusBar.ClientID + ".setStatus({text: 'Node Collapsed: <b>' + node.text + '</b>', clear: true});";

        //Ext.Net.TreeNode root = new Ext.Net.TreeNode("Composers");
        //root.Expanded = true;
        //tree.Root.Add(root);

        //List<Composer> composers = this.GetData();

        //foreach (Composer composer in composers)
        //{
        //    Ext.Net.TreeNode composerNode = new Ext.Net.TreeNode(composer.Name, Icon.UserGray);
        //    root.Nodes.Add(composerNode);

        //    foreach (Composition composition in composer.Compositions)
        //    {
        //        Ext.Net.TreeNode compositionNode = new Ext.Net.TreeNode(composition.Type.ToString());
        //        composerNode.Nodes.Add(compositionNode);

        //        foreach (Piece piece in composition.Pieces)
        //        {
        //            Ext.Net.TreeNode pieceNode = new Ext.Net.TreeNode(piece.Title, Icon.Music);
        //            compositionNode.Nodes.Add(pieceNode);
        //        }
        //    }
        //}

        //----------------------------------------------------------------------------

        MakeUserTabFunction();



    }

    protected void RunDisplay(object sender, DirectEventArgs e)
    {
        string id = e.ExtraParams["id"];
        bool IsLeaf = false;

        if (e.ExtraParams["isLeaf"] == "0")
        {
            IsLeaf = true;
        }

        if (IsLeaf)
        {
            Ext.Net.Panel tab4 = new Ext.Net.Panel();
            tab4.Title = id;
            tab4.BodyStyle = "padding: 6px;";
            tab4.Html = id;
            tab4.Closable = true;
            tab4.AddTo("PanelC");
        }
    }

    //protected void MakeTab(object sender, DirectEventArgs e)
    //{
    //    Ext.Net.Panel tab4 = new Ext.Net.Panel();
    //    tab4.Title = "Another Tab 22";
    //    tab4.BodyStyle = "padding: 6px;";
    //    tab4.Html = "Another Tab 22";
    //    tab4.Closable = true;

    //    tab4.AddTo("TabPanelC");


    //}	

    protected void MakeUserTabFunction()
    {
        Ext.Net.Panel pnlUsers = new Ext.Net.Panel();
        pnlUsers.ID = "pnlUsers";
        pnlUsers.Title = "Users and Roles";
        pnlUsers.Height = Unit.Pixel(200);

        Ext.Net.Button btnUsers = new Ext.Net.Button();
        btnUsers.Text = "Users";

        btnUsers.DirectEvents.Click.Event += MakeTabUsers;

        /////////////////////////////////////////////////////////////
        //store
        /////////////////////////////////////////////////////////////

        AspnetUserCollection userCol = new AspnetUserCollection().OrderByDesc("UserName").Load();

        //GridPanel grid = new GridPanel();
        //grid.Height = Unit.Pixel(400);
        //foreach (RecordField fld in rdr.Fields)
        //{
        //    if (fld.Type == RecordFieldType.Date)
        //        grid.ColumnModel.Columns.Add(new DateColumn() { DataIndex = fld.Name, Header = fld.Name });
        //    else
        //        grid.ColumnModel.Columns.Add(new Column() { DataIndex = fld.Name, Header = fld.Name });
        //}
        //grid.Store.Add(store);

        //store.AutoLoadParams.Add(new Ext.Net.Parameter { Name = "start", Value = "0", Mode = ParameterMode.Raw });
        //store.AutoLoadParams.Add(new Ext.Net.Parameter { Name = "limit", Value = "10", Mode = ParameterMode.Raw });

        //PagingToolbar pager = new PagingToolbar() { HideRefresh = true, PageSize = 10 };
        //pager.StoreID = store.ID;
        //grid.BottomBar.Add(pager);

        //grid.SelectionModel.Add(new RowSelectionModel());
        //grid.AddTo(Panel1);





        //////////////////////////////////////////////////////////////
        pnlUsers.Items.Add(btnUsers);
        PanelW.Items.Add(pnlUsers);



        //PanelW.ActiveItem = "pnlUsers";		

    }


    protected void MakeTabUsers(object sender, DirectEventArgs e)
    {
        //string id = e.ExtraParams["id"];
        bool IsLeaf = true;

        if (IsLeaf)
        {
            Ext.Net.Panel tab4 = new Ext.Net.Panel();
            tab4.Title = "Users";
            tab4.BodyStyle = "padding: 6px;";
            tab4.Html = "";
            tab4.Closable = true;
            tab4.ID = "TestTab1";


            tab4.LoadContent(new LoadConfig("Users.aspx", LoadMode.IFrame, true));

            tab4.AddTo("PanelC");
            PanelC.SetActiveTab(tab4);
        }


    }

    protected void NodeClick(object sender, DirectEventArgs e)
    {
        string s = e.ExtraParams["selectedmodule"].ToString();

        Ext.Net.Panel newtab = new Ext.Net.Panel();

        newtab.BodyStyle = "padding: 6px;";
        newtab.Html = "";
        newtab.Closable = true;
        newtab.AutoLoad.ShowMask = true;

        string url = String.Empty;
        if (s == "orgs")
        {
            newtab.Title = "Organisations";
            newtab.ID = "OrgTab";
            url = "Admin/Organisations.aspx";
        }
        else if (s == "projects")
        {
            newtab.Title = "Projects/Sites";
            newtab.ID = "ProjectTab";
            url = "Admin/ProjectSites.aspx";

        }
        else if (s == "station")
        {
            newtab.Title = "Stations";
            newtab.ID = "StationTab";
            url = "Admin/Stations.aspx";

        }

        newtab.LoadContent(new LoadConfig(url, LoadMode.IFrame, true));

        newtab.AddTo("PanelC");
        PanelC.SetActiveTab(newtab);

    }
    
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Environmental Observations</title>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var employeeDetailsRender = function () {
            return '<img class="imgEdit" ext:qtip="Click to view/edit additional details" style="cursor:pointer;" src="images/vcard_edit.png" />';
        };

        var cellClick = function (grid, rowIndex, columnIndex, e) {
            var t = e.getTarget(),
                record = grid.getStore().getAt(rowIndex),  // Get the Record
                columnId = grid.getColumnModel().getColumnId(columnIndex); // Get column id

            if (t.className == "imgEdit" && columnId == "Details") {
                //the ajax call is allowed
                return true;
            }

            //forbidden
            return false;
        };

        var menuItemClick = function (item) {
            pnlCenter.body.update(String.format("Clicked: {0}", item.text)).highlight();
        };
    </script>
</head>
<body>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="border">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Header="false" Region="North" Border="false"
                Html="<div id='header' style='height:32px;'><div class='main-title'>Environmental Observations</div></div>">
            </ext:Panel>
            <ext:Panel ID="PanelW" runat="server" Title="West" Region="West" Layout="accordion"
                Width="225" MinWidth="225" MaxWidth="400" Collapsible="true" Border="false" Split="false">
                <Items>
               <%--     <ext:TreePanel ID="tree" runat="server" Height="450" Title="Site Map - Preload" Icon="ChartOrganisation">
                        <DirectEvents>
                            <Click OnEvent="RunDisplay" Failure="Ext.MessageBox.alert('Load failed', 'Error during ajax event!');">
                                <ExtraParams>
                                    <ext:Parameter Name="id" Value="params[0].text" Mode="Raw" />
                                    <ext:Parameter Name="isLeaf" Value="params[0].childNodes.length" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:TreePanel>--%>
                    <ext:TreePanel ID="MasterDataTree" runat="server" Height="450" Title="Master Data Management"
                        Icon="DatabaseGear" RootVisible="false" NodeID="root">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:Button ID="Button3" runat="server" Text="Expand All">
                                        <Listeners>
                                            <Click Handler="#{MasterDataTree}.expandAll();" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button4" runat="server" Text="Collapse All">
                                        <Listeners>
                                            <Click Handler="#{MasterDataTree}.collapseAll();" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Root>
                            <ext:TreeNode Text="Entities" Expanded="true" SingleClickExpand="true">
                                <Nodes>
                                    <ext:TreeNode Text="Organisations" NodeID="orgs" SingleClickExpand="true" Icon="BookAddresses">
                                    </ext:TreeNode>
                                    <ext:TreeNode Text="Projects/Sites" NodeID="projects" SingleClickExpand="true" Icon="BookGo">
                                    </ext:TreeNode>
                                    <ext:TreeNode Text="Stations" NodeID="station" SingleClickExpand="true" Icon="BookLink">
                                    </ext:TreeNode>
                                </Nodes>
                            </ext:TreeNode>
                        </Root>
                        <BottomBar>
                            <ext:StatusBar ID="StatusBar1" runat="server" AutoClear="1500" />
                        </BottomBar>
                        <Listeners>
                            <Click Handler="#{StatusBar1}.setStatus({text: 'Node Selected: <b>' + node.text + '<br />', clear: true});" />
                            <ExpandNode Handler="#{StatusBar1}.setStatus({text: 'Node Expanded: <b>' + node.text + '<br />', clear: true});"
                                Delay="30" />
                            <CollapseNode Handler="#{StatusBar1}.setStatus({text: 'Node Collapsed: <b>' + node.text + '<br />', clear: true});" />
                        </Listeners>
                        <DirectEvents>
                            <Click OnEvent="NodeClick">
                                <ExtraParams>
                                    <ext:Parameter Name="selectedmodule" Value="node.id" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:TreePanel>
                </Items>
            </ext:Panel>
            <ext:TabPanel ID="PanelC" runat="server" Region="Center">
                <Items>
                </Items>
            </ext:TabPanel>
        </Items>
    </ext:Viewport>
</body>
</html>

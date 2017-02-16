namespace Microsoft.Word4Tfs.WordAddIn
{
    partial class TeamRibbonView : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public TeamRibbonView()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamRibbonView));
            this.tabTeam = this.Factory.CreateRibbonTab();
            this.groupWorkItems = this.Factory.CreateRibbonGroup();
            this.buttonImport = this.Factory.CreateRibbonButton();
            this.buttonRefresh = this.Factory.CreateRibbonButton();
            this.groupDesign = this.Factory.CreateRibbonGroup();
            this.toggleButtonShowLayoutEditor = this.Factory.CreateRibbonToggleButton();
            this.groupSettings = this.Factory.CreateRibbonGroup();
            this.checkBoxShowBookmarks = this.Factory.CreateRibbonCheckBox();
            this.tabTeam.SuspendLayout();
            this.groupWorkItems.SuspendLayout();
            this.groupDesign.SuspendLayout();
            this.groupSettings.SuspendLayout();
            // 
            // tabTeam
            // 
            this.tabTeam.Groups.Add(this.groupWorkItems);
            this.tabTeam.Groups.Add(this.groupDesign);
            this.tabTeam.Groups.Add(this.groupSettings);
            this.tabTeam.Label = "Team";
            this.tabTeam.Name = "tabTeam";
            // 
            // groupWorkItems
            // 
            this.groupWorkItems.Items.Add(this.buttonImport);
            this.groupWorkItems.Items.Add(this.buttonRefresh);
            this.groupWorkItems.Label = "Work Items";
            this.groupWorkItems.Name = "groupWorkItems";
            // 
            // buttonImport
            // 
            this.buttonImport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonImport.Label = "Import";
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.ScreenTip = "Import";
            this.buttonImport.ShowImage = true;
            this.buttonImport.SuperTip = "Import work items from a Team Project. Disabled if the system template is missing" +
    " or invalid. Also disabled when editing a layout using the Layout Designer.";
            this.buttonImport.Tag = "Import";
            this.buttonImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Import_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonRefresh.Label = "Refresh";
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.ScreenTip = "Refresh";
            this.buttonRefresh.ShowImage = true;
            this.buttonRefresh.SuperTip = resources.GetString("buttonRefresh.SuperTip");
            this.buttonRefresh.Tag = "Refresh";
            this.buttonRefresh.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Refresh_Click);
            // 
            // groupDesign
            // 
            this.groupDesign.Items.Add(this.toggleButtonShowLayoutEditor);
            this.groupDesign.Label = "Design";
            this.groupDesign.Name = "groupDesign";
            // 
            // toggleButtonShowLayoutEditor
            // 
            this.toggleButtonShowLayoutEditor.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.toggleButtonShowLayoutEditor.Label = "Show Layout Designer";
            this.toggleButtonShowLayoutEditor.Name = "toggleButtonShowLayoutEditor";
            this.toggleButtonShowLayoutEditor.ScreenTip = "Show the Layout Designer in a New Window";
            this.toggleButtonShowLayoutEditor.ShowImage = true;
            this.toggleButtonShowLayoutEditor.Tag = "ShowDesigner";
            this.toggleButtonShowLayoutEditor.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ToggleButtonShowLayoutEditor_Click);
            // 
            // groupSettings
            // 
            this.groupSettings.Items.Add(this.checkBoxShowBookmarks);
            this.groupSettings.Label = "Settings";
            this.groupSettings.Name = "groupSettings";
            // 
            // checkBoxShowBookmarks
            // 
            this.checkBoxShowBookmarks.Label = "Show Work Item Bookmarks";
            this.checkBoxShowBookmarks.Name = "checkBoxShowBookmarks";
            this.checkBoxShowBookmarks.ScreenTip = "Show Bookmarks";
            this.checkBoxShowBookmarks.SuperTip = resources.GetString("checkBoxShowBookmarks.SuperTip");
            this.checkBoxShowBookmarks.Tag = "ShowBookmarks";
            this.checkBoxShowBookmarks.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CheckBoxShowBookmarks_Click);
            // 
            // TeamRibbonView
            // 
            this.Name = "TeamRibbonView";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tabTeam);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.TeamRibbon_Load);
            this.tabTeam.ResumeLayout(false);
            this.tabTeam.PerformLayout();
            this.groupWorkItems.ResumeLayout(false);
            this.groupWorkItems.PerformLayout();
            this.groupDesign.ResumeLayout(false);
            this.groupDesign.PerformLayout();
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabTeam;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupWorkItems;
        internal Office.Tools.Ribbon.RibbonButton buttonImport;
        internal Office.Tools.Ribbon.RibbonButton buttonRefresh;
        internal Office.Tools.Ribbon.RibbonGroup groupDesign;
        internal Office.Tools.Ribbon.RibbonToggleButton toggleButtonShowLayoutEditor;
        internal Office.Tools.Ribbon.RibbonGroup groupSettings;
        internal Office.Tools.Ribbon.RibbonCheckBox checkBoxShowBookmarks;
    }

    partial class ThisRibbonCollection
    {
        internal TeamRibbonView TeamRibbon
        {
            get { return this.GetRibbon<TeamRibbonView>(); }
        }
    }
}

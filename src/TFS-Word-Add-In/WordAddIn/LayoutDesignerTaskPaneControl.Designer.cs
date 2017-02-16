namespace Microsoft.Word4Tfs.WordAddIn
{
    partial class LayoutDesignerTaskPaneControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutDesignerTaskPaneControl));
            this.buttonSave = new System.Windows.Forms.Button();
            this.listViewFields = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelManageLayouts = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelEditLayout = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelConnect = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.layoutListControl = new Microsoft.Word4Tfs.WordAddIn.LayoutListControl();
            this.panelManageLayouts.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelEditLayout.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Tag = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // listViewFields
            // 
            resources.ApplyResources(this.listViewFields, "listViewFields");
            this.listViewFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewFields.Name = "listViewFields";
            this.listViewFields.ShowItemToolTips = true;
            this.listViewFields.Tag = "AddField";
            this.listViewFields.UseCompatibleStateImageBehavior = false;
            this.listViewFields.View = System.Windows.Forms.View.Details;
            this.listViewFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ListViewFields_ItemDrag);
            this.listViewFields.SizeChanged += new System.EventHandler(this.ListViewFields_SizeChanged);
            this.listViewFields.DoubleClick += new System.EventHandler(this.ListViewFields_DoubleClick);
            this.listViewFields.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListViewFields_KeyDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // panelManageLayouts
            // 
            this.panelManageLayouts.BackColor = System.Drawing.SystemColors.Control;
            this.panelManageLayouts.Controls.Add(this.linkLabel1);
            this.panelManageLayouts.Controls.Add(this.label9);
            this.panelManageLayouts.Controls.Add(this.label6);
            this.panelManageLayouts.Controls.Add(this.panel1);
            this.panelManageLayouts.Controls.Add(this.label2);
            this.panelManageLayouts.Controls.Add(this.layoutListControl);
            resources.ApplyResources(this.panelManageLayouts, "panelManageLayouts");
            this.panelManageLayouts.Name = "panelManageLayouts";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "AddNew";
            this.linkLabel1.Click += new System.EventHandler(this.ButtonAddNew_Click);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Controls.Add(this.label4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.SystemColors.Info;
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panelEditLayout
            // 
            this.panelEditLayout.BackColor = System.Drawing.SystemColors.Control;
            this.panelEditLayout.Controls.Add(this.linkLabelConnect);
            this.panelEditLayout.Controls.Add(this.label1);
            this.panelEditLayout.Controls.Add(this.label8);
            this.panelEditLayout.Controls.Add(this.label7);
            this.panelEditLayout.Controls.Add(this.panel2);
            this.panelEditLayout.Controls.Add(this.label3);
            this.panelEditLayout.Controls.Add(this.listViewFields);
            resources.ApplyResources(this.panelEditLayout, "panelEditLayout");
            this.panelEditLayout.Name = "panelEditLayout";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // linkLabelConnect
            // 
            resources.ApplyResources(this.linkLabelConnect, "linkLabelConnect");
            this.linkLabelConnect.Name = "linkLabelConnect";
            this.linkLabelConnect.TabStop = true;
            this.linkLabelConnect.Tag = "Connect";
            this.linkLabelConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel2.Controls.Add(this.label5);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.SystemColors.Info;
            this.label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelManageLayouts);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelEditLayout);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonSave, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxStatus, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // textBoxStatus
            // 
            resources.ApplyResources(this.textBoxStatus, "textBoxStatus");
            this.textBoxStatus.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxStatus.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxStatus.Name = "textBoxStatus";
            // 
            // layoutListControl
            // 
            this.layoutListControl.AllowDelete = false;
            this.layoutListControl.AllowRename = false;
            resources.ApplyResources(this.layoutListControl, "layoutListControl");
            this.layoutListControl.Name = "layoutListControl";
            this.layoutListControl.LayoutSelected += new System.EventHandler<Microsoft.Word4Tfs.Library.View.LayoutItemEventArgs>(this.LayoutListControl_LayoutSelected);
            this.layoutListControl.LayoutRename += new System.EventHandler<Microsoft.Word4Tfs.Library.View.RenameEventArgs>(this.LayoutListControl_LayoutRename);
            this.layoutListControl.LayoutDelete += new System.EventHandler<Microsoft.Word4Tfs.Library.View.LayoutItemEventArgs>(this.LayoutListControl_LayoutDelete);
            // 
            // LayoutDesignerTaskPaneControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LayoutDesignerTaskPaneControl";
            this.panelManageLayouts.ResumeLayout(false);
            this.panelManageLayouts.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelEditLayout.ResumeLayout(false);
            this.panelEditLayout.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private LayoutListControl layoutListControl;
        private System.Windows.Forms.ListView listViewFields;
        private System.Windows.Forms.Panel panelEditLayout;
        private System.Windows.Forms.Panel panelManageLayouts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.LinkLabel linkLabelConnect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStatus;


    }
}

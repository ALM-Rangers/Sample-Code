namespace Microsoft.WcfUnit.VSIntegration
{
    partial class SelectAssembliesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectAssembliesControl));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewAssemblies = new System.Windows.Forms.DataGridView();
            this.Assembly = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssemblies)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Tag = "";
            // 
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Tag = "AddAssembly";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dataGridViewAssemblies
            // 
            this.dataGridViewAssemblies.AllowUserToAddRows = false;
            this.dataGridViewAssemblies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAssemblies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Assembly});
            resources.ApplyResources(this.dataGridViewAssemblies, "dataGridViewAssemblies");
            this.dataGridViewAssemblies.Name = "dataGridViewAssemblies";
            this.dataGridViewAssemblies.ReadOnly = true;
            this.dataGridViewAssemblies.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.DataGridViewAssemblies_RowsRemoved);
            // 
            // Assembly
            // 
            resources.ApplyResources(this.Assembly, "Assembly");
            this.Assembly.Name = "Assembly";
            this.Assembly.ReadOnly = true;
            // 
            // openFileDialog
            // 
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            this.openFileDialog.Multiselect = true;
            // 
            // SelectAssembliesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewAssemblies);
            this.Controls.Add(this.textBoxDescription);
            this.Name = "SelectAssembliesControl";
            this.Tag = "SelectAssemblies";
            this.VisibleChanged += new System.EventHandler(this.SelectAssembliesControl_VisibleChanged);
            this.Load += new System.EventHandler(this.SelectAssembliesControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssemblies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewAssemblies;
        private System.Windows.Forms.DataGridViewTextBoxColumn Assembly;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

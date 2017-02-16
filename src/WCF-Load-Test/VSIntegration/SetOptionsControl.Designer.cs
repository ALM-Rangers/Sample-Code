namespace Microsoft.WcfUnit.VSIntegration
{
    partial class SetOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetOptionsControl));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.dataGridViewSoapActions = new System.Windows.Forms.DataGridView();
            this.SoapAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxTimers = new System.Windows.Forms.CheckBox();
            this.checkBoxIndividualTests = new System.Windows.Forms.CheckBox();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSoapActions)).BeginInit();
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
            // dataGridViewSoapActions
            // 
            this.dataGridViewSoapActions.AllowUserToAddRows = false;
            this.dataGridViewSoapActions.AllowUserToDeleteRows = false;
            this.dataGridViewSoapActions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSoapActions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SoapAction,
            this.ActionSelected});
            resources.ApplyResources(this.dataGridViewSoapActions, "dataGridViewSoapActions");
            this.dataGridViewSoapActions.Name = "dataGridViewSoapActions";
            this.dataGridViewSoapActions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSoapActions_CellEndEdit);
            // 
            // SoapAction
            // 
            resources.ApplyResources(this.SoapAction, "SoapAction");
            this.SoapAction.Name = "SoapAction";
            // 
            // ActionSelected
            // 
            resources.ApplyResources(this.ActionSelected, "ActionSelected");
            this.ActionSelected.Name = "ActionSelected";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxTimers
            // 
            resources.ApplyResources(this.checkBoxTimers, "checkBoxTimers");
            this.checkBoxTimers.Checked = true;
            this.checkBoxTimers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTimers.Name = "checkBoxTimers";
            this.checkBoxTimers.UseVisualStyleBackColor = true;
            this.checkBoxTimers.CheckedChanged += new System.EventHandler(this.CheckBoxTimers_CheckedChanged);
            // 
            // checkBoxIndividualTests
            // 
            resources.ApplyResources(this.checkBoxIndividualTests, "checkBoxIndividualTests");
            this.checkBoxIndividualTests.Name = "checkBoxIndividualTests";
            this.checkBoxIndividualTests.UseVisualStyleBackColor = true;
            this.checkBoxIndividualTests.CheckedChanged += new System.EventHandler(this.CheckBoxIndividualTests_CheckedChanged);
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Tag = "SelectAllSoapActions";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.ButtonSelectAll_Click);
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Tag = "ClearAllSoapActions";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.ButtonClearAll_Click);
            // 
            // SetOptionsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonClearAll);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.checkBoxIndividualTests);
            this.Controls.Add(this.checkBoxTimers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewSoapActions);
            this.Controls.Add(this.textBoxDescription);
            this.Name = "SetOptionsControl";
            this.Tag = "SetOptions";
            this.VisibleChanged += new System.EventHandler(this.SetOptionsControl_VisibleChanged);
            this.Load += new System.EventHandler(this.SetOptionsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSoapActions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.DataGridView dataGridViewSoapActions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxTimers;
        private System.Windows.Forms.CheckBox checkBoxIndividualTests;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoapAction;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ActionSelected;


    }
}

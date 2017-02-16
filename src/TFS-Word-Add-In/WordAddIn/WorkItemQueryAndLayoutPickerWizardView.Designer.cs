namespace Microsoft.Word4Tfs.WordAddIn
{
    partial class WorkItemQueryAndLayoutPickerWizardView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkItemQueryAndLayoutPickerWizardView));
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelPages = new System.Windows.Forms.Panel();
            this.pictureBoxTitleIcon = new System.Windows.Forms.PictureBox();
            this.panelLowerHalfBackground = new System.Windows.Forms.Panel();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitleIcon)).BeginInit();
            this.panelLowerHalfBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPrevious
            // 
            resources.ApplyResources(this.buttonPrevious, "buttonPrevious");
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Tag = "Previous";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // buttonNext
            // 
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Tag = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.ButtonNext_Click);
            // 
            // buttonFinish
            // 
            resources.ApplyResources(this.buttonFinish, "buttonFinish");
            this.buttonFinish.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Tag = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.ButtonFinish_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Tag = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // panelPages
            // 
            this.panelPages.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.panelPages, "panelPages");
            this.panelPages.Name = "panelPages";
            // 
            // pictureBoxTitleIcon
            // 
            resources.ApplyResources(this.pictureBoxTitleIcon, "pictureBoxTitleIcon");
            this.pictureBoxTitleIcon.Name = "pictureBoxTitleIcon";
            this.pictureBoxTitleIcon.TabStop = false;
            // 
            // panelLowerHalfBackground
            // 
            resources.ApplyResources(this.panelLowerHalfBackground, "panelLowerHalfBackground");
            this.panelLowerHalfBackground.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelLowerHalfBackground.Controls.Add(this.buttonCancel);
            this.panelLowerHalfBackground.Controls.Add(this.buttonFinish);
            this.panelLowerHalfBackground.Controls.Add(this.buttonNext);
            this.panelLowerHalfBackground.Controls.Add(this.buttonPrevious);
            this.panelLowerHalfBackground.Name = "panelLowerHalfBackground";
            // 
            // textBoxTitle
            // 
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.ReadOnly = true;
            // 
            // WorkItemQueryAndLayoutPickerWizardView
            // 
            this.AcceptButton = this.buttonFinish;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.buttonCancel;
            this.ControlBox = false;
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.pictureBoxTitleIcon);
            this.Controls.Add(this.panelPages);
            this.Controls.Add(this.panelLowerHalfBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WorkItemQueryAndLayoutPickerWizardView";
            this.Load += new System.EventHandler(this.WorkItemQueryAndLayoutPickerWizardView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitleIcon)).EndInit();
            this.panelLowerHalfBackground.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelPages;
        private System.Windows.Forms.PictureBox pictureBoxTitleIcon;
        private System.Windows.Forms.Panel panelLowerHalfBackground;
        private System.Windows.Forms.TextBox textBoxTitle;
    }
}
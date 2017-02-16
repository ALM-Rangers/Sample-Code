namespace Microsoft.WcfUnit.VSIntegration
{
    partial class WizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.linkWelcome = new System.Windows.Forms.LinkLabel();
            this.panelWizardArea = new System.Windows.Forms.Panel();
            this.panelNavigation = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.textBoxWizardPageSummary = new System.Windows.Forms.TextBox();
            this.linkLabelRunScenario = new System.Windows.Forms.LinkLabel();
            this.linkLabelSetOptions = new System.Windows.Forms.LinkLabel();
            this.linkLabelSelectAssemblies = new System.Windows.Forms.LinkLabel();
            this.panelNavigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkWelcome
            // 
            this.linkWelcome.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            this.linkWelcome.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkWelcome.LinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.linkWelcome, "linkWelcome");
            this.linkWelcome.Name = "linkWelcome";
            this.linkWelcome.TabStop = true;
            this.linkWelcome.Tag = "Welcome";
            this.linkWelcome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkWelcome_LinkClicked);
            // 
            // panelWizardArea
            // 
            this.panelWizardArea.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.panelWizardArea, "panelWizardArea");
            this.panelWizardArea.Name = "panelWizardArea";
            // 
            // panelNavigation
            // 
            this.panelNavigation.BackColor = System.Drawing.SystemColors.Control;
            this.panelNavigation.Controls.Add(this.labelVersion);
            this.panelNavigation.Controls.Add(this.buttonCancel);
            this.panelNavigation.Controls.Add(this.buttonFinish);
            this.panelNavigation.Controls.Add(this.buttonNext);
            this.panelNavigation.Controls.Add(this.buttonPrevious);
            resources.ApplyResources(this.panelNavigation, "panelNavigation");
            this.panelNavigation.Name = "panelNavigation";
            // 
            // labelVersion
            // 
            this.labelVersion.ForeColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Name = "labelVersion";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Tag = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonFinish
            // 
            resources.ApplyResources(this.buttonFinish, "buttonFinish");
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Tag = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.ButtonFinish_Click);
            // 
            // buttonNext
            // 
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Tag = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.ButtonNext_Click);
            // 
            // buttonPrevious
            // 
            resources.ApplyResources(this.buttonPrevious, "buttonPrevious");
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Tag = "Previous";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // textBoxWizardPageSummary
            // 
            this.textBoxWizardPageSummary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxWizardPageSummary, "textBoxWizardPageSummary");
            this.textBoxWizardPageSummary.Name = "textBoxWizardPageSummary";
            this.textBoxWizardPageSummary.TabStop = false;
            // 
            // linkLabelRunScenario
            // 
            this.linkLabelRunScenario.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            this.linkLabelRunScenario.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelRunScenario.LinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.linkLabelRunScenario, "linkLabelRunScenario");
            this.linkLabelRunScenario.Name = "linkLabelRunScenario";
            this.linkLabelRunScenario.TabStop = true;
            this.linkLabelRunScenario.Tag = "RunScenario";
            this.linkLabelRunScenario.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelRunScenario_LinkClicked);
            // 
            // linkLabelSetOptions
            // 
            this.linkLabelSetOptions.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.linkLabelSetOptions, "linkLabelSetOptions");
            this.linkLabelSetOptions.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelSetOptions.LinkColor = System.Drawing.SystemColors.ControlText;
            this.linkLabelSetOptions.Name = "linkLabelSetOptions";
            this.linkLabelSetOptions.TabStop = true;
            this.linkLabelSetOptions.Tag = "SetOptions";
            this.linkLabelSetOptions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelSetOptions_LinkClicked);
            // 
            // linkLabelSelectAssemblies
            // 
            this.linkLabelSelectAssemblies.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.linkLabelSelectAssemblies, "linkLabelSelectAssemblies");
            this.linkLabelSelectAssemblies.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelSelectAssemblies.LinkColor = System.Drawing.SystemColors.ControlText;
            this.linkLabelSelectAssemblies.Name = "linkLabelSelectAssemblies";
            this.linkLabelSelectAssemblies.TabStop = true;
            this.linkLabelSelectAssemblies.Tag = "SelectAssemblies";
            this.linkLabelSelectAssemblies.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SelectAssemblies_LinkClicked);
            // 
            // WizardForm
            // 
            this.AcceptButton = this.buttonFinish;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.linkLabelSelectAssemblies);
            this.Controls.Add(this.linkLabelSetOptions);
            this.Controls.Add(this.linkLabelRunScenario);
            this.Controls.Add(this.textBoxWizardPageSummary);
            this.Controls.Add(this.panelWizardArea);
            this.Controls.Add(this.linkWelcome);
            this.Controls.Add(this.panelNavigation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.ShowInTaskbar = false;
            this.panelNavigation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkWelcome;
        private System.Windows.Forms.Panel panelWizardArea;
        private System.Windows.Forms.Panel panelNavigation;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.TextBox textBoxWizardPageSummary;
        private System.Windows.Forms.LinkLabel linkLabelRunScenario;
        private System.Windows.Forms.LinkLabel linkLabelSetOptions;
        private System.Windows.Forms.LinkLabel linkLabelSelectAssemblies;
        private System.Windows.Forms.Label labelVersion;
    }
}
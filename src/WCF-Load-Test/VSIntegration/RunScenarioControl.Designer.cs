namespace Microsoft.WcfUnit.VSIntegration
{
    partial class RunScenarioControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunScenarioControl));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.textBoxExecutable = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.openExecutableFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBoxRunButtonDescription = new System.Windows.Forms.TextBox();
            this.radioButtonChooseExecutable = new System.Windows.Forms.RadioButton();
            this.radioButtonChooseTraceFile = new System.Windows.Forms.RadioButton();
            this.buttonBrowseTraceFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTraceFileName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonChooseFiddlerTextTrace = new System.Windows.Forms.RadioButton();
            this.radioButtonChooseServerTrace = new System.Windows.Forms.RadioButton();
            this.radioButtonChooseClientTrace = new System.Windows.Forms.RadioButton();
            this.openTraceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonParse = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.TabStop = false;
            // 
            // textBoxExecutable
            // 
            resources.ApplyResources(this.textBoxExecutable, "textBoxExecutable");
            this.textBoxExecutable.Name = "textBoxExecutable";
            this.textBoxExecutable.Tag = "ExecutableFileName";
            this.textBoxExecutable.TextChanged += new System.EventHandler(this.TextBoxExecutable_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonBrowse
            // 
            resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Tag = "BrowseExecutable";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // buttonRun
            // 
            resources.ApplyResources(this.buttonRun, "buttonRun");
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Tag = "RunExecutable";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.ButtonRun_Click);
            // 
            // openExecutableFileDialog
            // 
            resources.ApplyResources(this.openExecutableFileDialog, "openExecutableFileDialog");
            // 
            // textBoxRunButtonDescription
            // 
            this.textBoxRunButtonDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxRunButtonDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxRunButtonDescription, "textBoxRunButtonDescription");
            this.textBoxRunButtonDescription.Name = "textBoxRunButtonDescription";
            this.textBoxRunButtonDescription.TabStop = false;
            // 
            // radioButtonChooseExecutable
            // 
            resources.ApplyResources(this.radioButtonChooseExecutable, "radioButtonChooseExecutable");
            this.radioButtonChooseExecutable.Name = "radioButtonChooseExecutable";
            this.radioButtonChooseExecutable.TabStop = true;
            this.radioButtonChooseExecutable.Tag = "ChooseExecutable";
            this.radioButtonChooseExecutable.UseVisualStyleBackColor = true;
            this.radioButtonChooseExecutable.CheckedChanged += new System.EventHandler(this.RadioButtonChooseExecutable_CheckedChanged);
            // 
            // radioButtonChooseTraceFile
            // 
            resources.ApplyResources(this.radioButtonChooseTraceFile, "radioButtonChooseTraceFile");
            this.radioButtonChooseTraceFile.Name = "radioButtonChooseTraceFile";
            this.radioButtonChooseTraceFile.TabStop = true;
            this.radioButtonChooseTraceFile.Tag = "ChooseTraceFile";
            this.radioButtonChooseTraceFile.UseVisualStyleBackColor = true;
            this.radioButtonChooseTraceFile.CheckedChanged += new System.EventHandler(this.RadioButtonChooseTraceFile_CheckedChanged);
            // 
            // buttonBrowseTraceFile
            // 
            resources.ApplyResources(this.buttonBrowseTraceFile, "buttonBrowseTraceFile");
            this.buttonBrowseTraceFile.Name = "buttonBrowseTraceFile";
            this.buttonBrowseTraceFile.Tag = "BrowseTraceFile";
            this.buttonBrowseTraceFile.UseVisualStyleBackColor = true;
            this.buttonBrowseTraceFile.Click += new System.EventHandler(this.ButtonBrowseTraceFile_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxTraceFileName
            // 
            resources.ApplyResources(this.textBoxTraceFileName, "textBoxTraceFileName");
            this.textBoxTraceFileName.Name = "textBoxTraceFileName";
            this.textBoxTraceFileName.Tag = "TraceFileName";
            this.textBoxTraceFileName.TextChanged += new System.EventHandler(this.TextBoxTraceFileName_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonChooseFiddlerTextTrace);
            this.groupBox1.Controls.Add(this.radioButtonChooseServerTrace);
            this.groupBox1.Controls.Add(this.radioButtonChooseClientTrace);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonChooseFiddlerTextTrace
            // 
            resources.ApplyResources(this.radioButtonChooseFiddlerTextTrace, "radioButtonChooseFiddlerTextTrace");
            this.radioButtonChooseFiddlerTextTrace.Name = "radioButtonChooseFiddlerTextTrace";
            this.radioButtonChooseFiddlerTextTrace.TabStop = true;
            this.radioButtonChooseFiddlerTextTrace.Tag = "ChooseFiddlerTextTrace";
            this.radioButtonChooseFiddlerTextTrace.UseVisualStyleBackColor = true;
            this.radioButtonChooseFiddlerTextTrace.CheckedChanged += new System.EventHandler(this.RadioButtonChooseFiddlerTextTrace_CheckedChanged);
            // 
            // radioButtonChooseServerTrace
            // 
            resources.ApplyResources(this.radioButtonChooseServerTrace, "radioButtonChooseServerTrace");
            this.radioButtonChooseServerTrace.Name = "radioButtonChooseServerTrace";
            this.radioButtonChooseServerTrace.TabStop = true;
            this.radioButtonChooseServerTrace.Tag = "ChooseWcfServerTrace";
            this.radioButtonChooseServerTrace.UseVisualStyleBackColor = true;
            this.radioButtonChooseServerTrace.CheckedChanged += new System.EventHandler(this.RadioButtonChooseServerTrace_CheckedChanged);
            // 
            // radioButtonChooseClientTrace
            // 
            resources.ApplyResources(this.radioButtonChooseClientTrace, "radioButtonChooseClientTrace");
            this.radioButtonChooseClientTrace.Name = "radioButtonChooseClientTrace";
            this.radioButtonChooseClientTrace.TabStop = true;
            this.radioButtonChooseClientTrace.Tag = "ChooseWcfClientTrace";
            this.radioButtonChooseClientTrace.UseVisualStyleBackColor = true;
            this.radioButtonChooseClientTrace.CheckedChanged += new System.EventHandler(this.RadioButtonChooseClientTrace_CheckedChanged);
            // 
            // openTraceFileDialog
            // 
            resources.ApplyResources(this.openTraceFileDialog, "openTraceFileDialog");
            // 
            // buttonParse
            // 
            resources.ApplyResources(this.buttonParse, "buttonParse");
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Tag = "ParseTraceFile";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.ButtonParse_Click);
            // 
            // RunScenarioControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonParse);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonBrowseTraceFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTraceFileName);
            this.Controls.Add(this.radioButtonChooseTraceFile);
            this.Controls.Add(this.radioButtonChooseExecutable);
            this.Controls.Add(this.textBoxRunButtonDescription);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxExecutable);
            this.Controls.Add(this.textBoxDescription);
            this.Name = "RunScenarioControl";
            this.Tag = "RunScenario";
            this.Load += new System.EventHandler(this.RunScenarioControl_Load);
            this.VisibleChanged += new System.EventHandler(this.RunScenarioControl_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.OpenFileDialog openExecutableFileDialog;
        private System.Windows.Forms.TextBox textBoxRunButtonDescription;
        private System.Windows.Forms.TextBox textBoxExecutable;
        private System.Windows.Forms.RadioButton radioButtonChooseExecutable;
        private System.Windows.Forms.RadioButton radioButtonChooseTraceFile;
        private System.Windows.Forms.Button buttonBrowseTraceFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTraceFileName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonChooseClientTrace;
        private System.Windows.Forms.RadioButton radioButtonChooseServerTrace;
        private System.Windows.Forms.OpenFileDialog openTraceFileDialog;
        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.RadioButton radioButtonChooseFiddlerTextTrace;
    }
}

//---------------------------------------------------------------------
// <copyright file="Form1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Form1 type.</summary>
//---------------------------------------------------------------------

namespace WindowsApplication1
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.VSIntegration;

    /// <summary>
    /// The test form.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WizardForm f = (WizardForm)ItemWizard.DefaultFormFactory();
            ////new WizardForm();
            ////WizardController controller = new WizardController(f, new ScenarioRunManager());
            ////f.SetController(controller);
            ////controller.Initialize();
            f.WizardCompleting += new EventHandler<CancellableWizardEventArgs>(this.Form1_WizardCompleting);
            MessageBox.Show(f.ShowDialog().ToString());
        }

        private void Form1_WizardCompleting(object sender, CancellableWizardEventArgs e)
        {
            e.Cancel = MessageBox.Show("Cancel?", "Test", MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (!e.Cancel)
            {
                string mainFileNameTemp = Path.GetTempFileName();
                string stubFileNameTemp = Path.GetTempFileName();
                try
                {
                    TraceFileProcessor tfp = new TraceFileProcessor();
                    tfp.ProcessTraceFile("WizardTester", e.Data.TraceFile, null, e.Data.Configuration, mainFileNameTemp, stubFileNameTemp);
                    System.Diagnostics.Process.Start("notepad", mainFileNameTemp);
                    System.Diagnostics.Process.Start("notepad", stubFileNameTemp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    e.Cancel = true;
                }
                ////finally
                ////{
                ////    //File.Delete(mainFileNameTemp);
                ////    //File.Delete(stubFileNameTemp);
                ////}
            }
        }
    }
}
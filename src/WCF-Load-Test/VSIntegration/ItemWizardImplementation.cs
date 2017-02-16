//---------------------------------------------------------------------
// <copyright file="ItemWizardImplementation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ItemWizardImplementation type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;

    using Microsoft.VisualStudio.TemplateWizard;

    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Implementation of the IWizard interface.
    /// </summary>
    public class ItemWizardImplementation : IItemWizard
    {
        /// <summary>
        /// The factory for the form.
        /// </summary>
        private FormFactory formFactory;

        /// <summary>
        /// The object to use for display errors.
        /// </summary>
        private ErrorDisplay errorDisplay;

        /// <summary>
        /// Flag to detect first call.
        /// </summary>
        private bool firstCall = true;

        /// <summary>
        /// The collection of assemblies.
        /// </summary>
        private Collection<string> proxyAssemblies = new Collection<string>();

        /// <summary>
        /// The name of the scenario.
        /// </summary>
        private string scenarioName;

        /// <summary>
        /// The main file name.
        /// </summary>
        private string mainFileName;

        /// <summary>
        /// The stub file name.
        /// </summary>
        private string stubFileName;

        /// <summary>
        /// Temporary name for the main file.
        /// </summary>
        private string mainFileNameTemp;

        /// <summary>
        /// Temporary name for the stub file.
        /// </summary>
        private string stubFileNameTemp;

        /// <summary>
        /// Initialises a new instance of the <see cref="ItemWizardImplementation"/> class.
        /// </summary>
        /// <param name="formFactory">The factory used to create forms.</param>
        /// <param name="errorDisplay">The delegate to call when displaying an error.</param>
        public ItemWizardImplementation(FormFactory formFactory, ErrorDisplay errorDisplay)
        {
            this.formFactory = formFactory;
            this.errorDisplay = errorDisplay;
        }

        #region IItemWizard Members

        /// <summary>
        /// Gets the collection of paths to the proxy assemblies.
        /// </summary>
        /// <value>The collection of paths to the proxy assemblies.</value>
        public Collection<string> ProxyAssemblies
        {
            get
            {
                return this.proxyAssemblies;
            }
        }

        /// <summary>
        /// Called when the wizard is started.
        /// </summary>
        /// <param name="name">The name to be given to the file(s).</param>
        public void RunStarted(string name)
        {
            this.scenarioName = Utility.MakeSafeIdentifier(Path.GetFileNameWithoutExtension(name));
            if (string.IsNullOrEmpty(this.scenarioName))
            {
                this.scenarioName = "WCFTest";
            }

            this.firstCall = true;
        }

        /// <summary>
        /// Determines if the project item should be added.
        /// </summary>
        /// <remarks>
        /// This is called once per file in the item template (i.e. twice). The flag
        /// indicating the first call is reset after the item file name has been captured.
        /// </remarks>
        /// <param name="filePath">The file to be added.</param>
        /// <returns>True if the file can be added, false otherwise.</returns>
        public bool ShouldAddProjectItem(string filePath)
        {
            bool ans;
            if (this.firstCall)
            {
                WizardData wizardData = null;
                try
                {
                    using (IWizardForm wizard = this.formFactory())
                    {
                        wizard.WizardCompleting += new EventHandler<CancellableWizardEventArgs>(this.WizardCompletingHandler);
                        ans = wizard.RunWizard();
                        wizardData = wizard.WizardData;
                    }
                }
                catch (Exception ex)
                {
                    this.errorDisplay(ex.Message);
                    throw;
                }
                finally
                {
                    // Handle cancellation
                    if (wizardData != null && !string.IsNullOrEmpty(wizardData.TraceFile) && wizardData.TraceFileSource == TraceFileSource.Dynamic)
                    {
                        File.Delete(wizardData.TraceFile);
                    }
                }
            }
            else
            {
                ans = true;
            }

            if (!ans)
            {
                throw new WizardCancelledException();
            }

            return ans;
        }

        /// <summary>
        /// Called when finished generating the project item.
        /// </summary>
        /// <remarks>
        /// This is called once per file in the item template (i.e. twice). This is where the
        /// item file name is captured.
        /// </remarks>
        /// <param name="fileName">The name of the file being generated.</param>
        public void ProjectItemFinishedGenerating(string fileName)
        {
            if (this.firstCall)
            {
                this.mainFileName = fileName;
            }
            else
            {
                this.stubFileName = fileName;
            }

            this.firstCall = false;
        }

        /// <summary>
        /// Called just before the file is opened.
        /// </summary>
        public void BeforeOpeningFile()
        {
            File.Copy(this.mainFileNameTemp, this.mainFileName, true);
            File.Copy(this.stubFileNameTemp, this.stubFileName, true);
            File.Delete(this.mainFileNameTemp);
            File.Delete(this.stubFileNameTemp);
        }

        /// <summary>
        /// Called when the wizard has finished executing.
        /// </summary>
        public void RunFinished()
        {
        }

        #endregion

        /// <summary>
        /// Handles wizard completion. 
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "We really want to show a message for anything that may occur")]
        private void WizardCompletingHandler(object sender, CancellableWizardEventArgs e)
        {
            // Process the data and if it fails prevent the closing of the wizard
            this.mainFileNameTemp = Path.GetTempFileName();
            this.stubFileNameTemp = Path.GetTempFileName();
            try
            {
                this.proxyAssemblies = TraceFileProcessor.ProcessTraceFileInAppDomain(this.scenarioName, e.Data.TraceFile, null, e.Data.Configuration, this.mainFileNameTemp, this.stubFileNameTemp);
            }
            catch (Exception ex)
            {
                this.errorDisplay(ex.Message);
                File.Delete(this.mainFileNameTemp);
                File.Delete(this.stubFileNameTemp);
                e.Cancel = true;
            }
        }
    }
}

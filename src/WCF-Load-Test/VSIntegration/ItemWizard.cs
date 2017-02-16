//---------------------------------------------------------------------
// <copyright file="ItemWizard.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>Contains the ItemWizard type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TemplateWizard;

    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// This is the Visual Studio wizard that adds a WCF test to the project.
    /// </summary>
    public class ItemWizard : IWizard
    {
        /// <summary>
        /// The actual implementation to which the wizard calls are forwarded.
        /// </summary>
        private ItemWizardImplementation forward;

        /// <summary>
        /// The automation object to use.
        /// </summary>
        private EnvDTE._DTE visualStudioAutomationObject;

        /// <summary>
        /// Initialises a new instance of the <see cref="ItemWizard"/> class.
        /// </summary>
        public ItemWizard()
        {
            this.forward = new ItemWizardImplementation(new FormFactory(DefaultFormFactory), new ErrorDisplay(this.DisplayError));
        }

        /// <summary>
        /// Gets the currently active project.
        /// </summary>
        private EnvDTE.Project ActiveProject
        {
            get
            {
                EnvDTE.Project activeProject = null;
                object obj = this.visualStudioAutomationObject.ActiveSolutionProjects;
                Array a = obj as Array;
                if (a != null && a.Length > 0)
                {
                    // For adding new test, only one project should be selected
                    Debug.Assert(a.Length == 1, "Expect only one project to be selected");
                    object projObj = a.GetValue(0);
                    EnvDTE.Project proj = projObj as EnvDTE.Project;

                    if (proj != null)
                    {
                        activeProject = proj;
                    }
                    else
                    {
                        Debug.Fail("Active project is null or type-wrong.");
                    }
                }

                return activeProject;
            }
        }

        /// <summary>
        /// Builds the wizard form and its controller, initializing them both.
        /// </summary>
        /// <returns>The wizard form, initialized with an initialized controller.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is used by the caller")]
        public static IWizardForm DefaultFormFactory()
        {
            WizardForm form = new WizardForm();
            WizardController controller = new WizardController(form, new ScenarioRunManager(), new UILogic());
            form.SetController(controller);
            controller.Initialize();
            return form;
        }

        #region IWizard Members

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        /// <param name="automationObject">See Visual Studio SDK documentation for automationObject.</param>
        /// <param name="replacementsDictionary">See Visual Studio SDK documentation for replacementsDictionary.</param>
        /// <param name="runKind">See Visual Studio SDK documentation for runKind.</param>
        /// <param name="customParams">See Visual Studio SDK documentation for customParameters.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (automationObject == null)
            {
                throw new ArgumentNullException("automationObject");
            }

            if (replacementsDictionary == null)
            {
                throw new ArgumentNullException("replacementsDictionary");
            }

            this.visualStudioAutomationObject = automationObject as EnvDTE._DTE;
            Debug.Assert(this.visualStudioAutomationObject != null, "The automation object should not be null");
            this.forward.RunStarted(replacementsDictionary["$rootname$"]);
        }

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        /// <param name="filePath">See Visual Studio SDK documentation for filePath.</param>
        /// <returns>See Visual Studio SDK documentation for the return value.</returns>
        public bool ShouldAddProjectItem(string filePath)
        {
            return this.forward.ShouldAddProjectItem(filePath);
        }

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        /// <param name="projectItem">See Visual Studio SDK documentation.</param>
        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException("projectItem");
            }

            this.forward.ProjectItemFinishedGenerating(projectItem.get_FileNames(0));
        }

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        /// <param name="projectItem">See Visual Studio SDK documentation.</param>
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
            this.forward.BeforeOpeningFile();
        }

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        public void RunFinished()
        {
            VSLangProj80.VSProject2 vsProject = this.ActiveProject.Object as VSLangProj80.VSProject2;

            foreach (string path in this.forward.ProxyAssemblies)
            {
                AddAssemblyReference(vsProject, path);
            }

            this.forward.RunFinished();
        }

        /// <summary>
        /// See <see cref="IWizard"/>.
        /// </summary>
        /// <param name="project">See Visual Studio SDK documentation.</param>
        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Adds an assembly reference to a visual studio project.
        /// </summary>
        /// <param name="vsProject">The project to add the reference to.</param>
        /// <param name="path">The path to the assembly for which the reference is to be added.</param>
        private static void AddAssemblyReference(VSLangProj80.VSProject2 vsProject, string path)
        {
            bool isSelfReference = false;
            string referenceFileName = Path.GetFileName(path);

            // Decide if this is a self-reference
            foreach (EnvDTE.Configuration c in vsProject.Project.ConfigurationManager)
            {
                foreach (EnvDTE.OutputGroup og in c.OutputGroups)
                {
                    if (og.CanonicalName == "Built")
                    {
                        foreach (string str in (Array)og.FileNames)
                        {
                            if (str == referenceFileName)
                            {
                                isSelfReference = true;
                                break;
                            }
                        }
                    }

                    if (isSelfReference)
                    {
                        break;
                    }
                }

                if (isSelfReference)
                {
                    break;
                }
            }

            // Add reference, unless it is a self-reference
            if (!isSelfReference)
            {
                string referenceName = Path.GetFileNameWithoutExtension(path);
                VSLangProj.Reference reference = vsProject.References.Item(referenceName);
                if (reference == null)
                {
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Adding reference to {0}", path));
                    reference = vsProject.References.Add(path);
                }

                reference.CopyLocal = true;
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Reference to {0} was determined to be a self reference and has not been added as a reference to the test project", path));
            }
        }

        /// <summary>
        /// Displays an error to the user.
        /// </summary>
        /// <param name="errorMessage">The error message to display.</param>
        private void DisplayError(string errorMessage)
        {
            MessageBox.Show(errorMessage, ErrorMessages.ErrorMessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
        }
    }
}

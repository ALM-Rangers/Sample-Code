//---------------------------------------------------------------------
// <copyright file="FlowController.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowController type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Windows;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Microsoft.Word4Tfs.Library.Presenter;

    /// <summary>
    /// The overall flow controller.
    /// </summary>
    public class FlowController
    {
        /// <summary>
        /// Gets or sets the unity container to use.
        /// </summary>
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        [Dependency]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the team ribbon presenter.
        /// </summary>
        [Dependency]
        public ITeamRibbonPresenter TeamRibbonPresenter { get; set; }

        /// <summary>
        /// Gets or sets the team project document manager.
        /// </summary>
        [Dependency]
        public ITeamProjectDocumentManager Manager { get; set; }

        /// <summary>
        /// Gets or sets the object used to manipulate files.
        /// </summary>
        [Dependency]
        public IFile File { get; set; }

        /// <summary>
        /// Gets or sets the object used to manipulate directories.
        /// </summary>
        [Dependency]
        public IDirectory Directory { get; set; }

        /// <summary>
        /// Gets or sets the object used to manipulate the settings.
        /// </summary>
        [Dependency]
        public ISettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the presenter for the layout designer.
        /// </summary>
        [Dependency]
        public ILayoutDesignerPresenter LayoutDesignerPresenter { get; set; }

        /// <summary>
        /// Initialises the flow controller.
        /// </summary>
        public void Initialise()
        {
            this.Logger.Log(TraceEventType.Information, "Flow controller initialising");
            this.TeamRibbonPresenter.Initialise(this.HandleDocumentRebind);
            this.TeamRibbonPresenter.Import += new System.EventHandler(this.HandleTeamRibbonImport);
            this.TeamRibbonPresenter.Refresh += new System.EventHandler(this.HandleTeamRibbonRefresh);
            this.TeamRibbonPresenter.ShowLayoutDesigner += new EventHandler(this.HandleShowLayoutDesigner);
            this.TeamRibbonPresenter.HideLayoutDesigner += new EventHandler(this.HandleHideLayoutDesigner);
            this.SetupLayoutDesignerPresenterEvents();
            this.SetupSystemTemplate();
            IWordApplication app = this.UnityContainer.Resolve<IWordApplication>();
            app.Initialise();
            this.Manager.Initialise();
            this.TeamRibbonPresenter.LoadState();
        }

        /// <summary>
        /// Sets up the event handlers on the layout designer presenter.
        /// </summary>
        private void SetupLayoutDesignerPresenterEvents()
        {
            this.LayoutDesignerPresenter.Connect += new EventHandler(this.HandleLayoutDesignerPresenterConnect);
            this.LayoutDesignerPresenter.Close += new EventHandler(this.HandleLayoutDesignerPresenterClose);
        }

        /// <summary>
        /// Sets up the system template document so that Word can pick it up.
        /// </summary>
        private void SetupSystemTemplate()
        {
            IWordApplication app = this.UnityContainer.Resolve<IWordApplication>();
            string installedPath = Path.Combine(Utilities.ReadExecutingDirectory(), Constants.SystemTemplateName);
            string roamingProfileDir = app.UserTemplatesPath;
            string roamingProfilePath = Path.Combine(roamingProfileDir, Constants.SystemTemplateName);
            if (this.File.Exists(installedPath))
            {
                if (this.File.Exists(roamingProfilePath))
                {
                    if (this.File.GetLastWriteTimeUtc(installedPath) > this.File.GetLastWriteTimeUtc(roamingProfilePath))
                    {
                        Nullable<DateTime> ignoreUpgrade = this.Settings.IgnoreSystemTemplateUpgradeFor;
                        if (!ignoreUpgrade.HasValue || (ignoreUpgrade.HasValue && ignoreUpgrade.Value != this.File.GetLastWriteTimeUtc(installedPath)))
                        {
                            if (this.TeamRibbonPresenter.AskYesNoQuestion(FlowControllerResources.UpgradeSystemTemplateQuestion))
                            {
                                this.CopyTemplateToRoamingProfile(installedPath, roamingProfilePath);
                            }
                            else
                            {
                                this.Settings.IgnoreSystemTemplateUpgradeFor = this.File.GetLastWriteTimeUtc(installedPath);
                            }
                        }
                    }
                }
                else
                {
                    this.CopyTemplateToRoamingProfile(installedPath, roamingProfilePath);
                }
            }
        }

        /// <summary>
        /// Copies the template to the roaming profile.
        /// </summary>
        /// <param name="installedPath">Where to copy the template from.</param>
        /// <param name="roamingProfilePath">The location of the roaming version of the template.</param>
        private void CopyTemplateToRoamingProfile(string installedPath, string roamingProfilePath)
        {
            this.File.Copy(installedPath, roamingProfilePath, true);
            this.File.SetWritable(roamingProfilePath);
        }

        /// <summary>
        /// Handles the rebinding of the active document.
        /// </summary>
        /// <returns>The new uri for the document.</returns>
        private Uri HandleDocumentRebind()
        {
            Uri ans = null;
            ITeamProjectPickerPresenter projectPickerPresenter = projectPickerPresenter = this.Manager.ActiveContainer.Resolve<ITeamProjectPickerPresenter>();
            ans = projectPickerPresenter.ChooseTeamProjectCollection();
            return ans;
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbonPresenter.Import"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// Displays the team project picker.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not appropiate to dispose Task Parallel objects")]
        private void HandleTeamRibbonImport(object sender, System.EventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Flow controller HandleTeamRibbonImport called");
            Debug.Assert(this.Manager.ActiveContainer != null, "There should be an active container in the team project document manager");
            Debug.Assert(this.Manager.ActiveDocument != null, "There should be an active document in the team project document manager");
            ITeamProject teamProject = null;
            ITeamProjectPickerPresenter projectPickerPresenter = null;
            if (!this.Manager.ActiveDocument.IsInsertable)
            {
                this.TeamRibbonPresenter.DisplayError(FlowControllerResources.DocumentNotInsertable, string.Empty);
            }
            else if (!this.Manager.ActiveDocument.IsConnected)
            {
                projectPickerPresenter = this.Manager.ActiveContainer.Resolve<ITeamProjectPickerPresenter>();
                teamProject = projectPickerPresenter.ChooseTeamProject();
            }
            else
            {
                teamProject = this.Manager.ActiveDocument.TeamProject;
            }

            if (teamProject != null)
            {
                this.Manager.ActiveContainer.RegisterInstance<ITeamProject>(teamProject);
                IWorkItemQueryAndLayoutPickerWizardPresenter workItemAndLayoutPickerWizardPresenter = this.Manager.ActiveContainer.Resolve<IWorkItemQueryAndLayoutPickerWizardPresenter>();
                workItemAndLayoutPickerWizardPresenter.Initialise();
                workItemAndLayoutPickerWizardPresenter.Start();
                QueryAndLayoutInformation queryAndLayout = workItemAndLayoutPickerWizardPresenter.QueryAndLayout;
                if (queryAndLayout != null)
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    TaskScheduler scheduler = this.UnityContainer.Resolve<TaskScheduler>();

                    Task<WorkItemTree> runQueryTask = new Task<WorkItemTree>(
                        () =>
                            {
                                this.Logger.Log(TraceEventType.Information, "Starting query for work items");
                                WorkItemTree ans = teamProject.QueryRunner.QueryForWorkItems(queryAndLayout.Query, cancellationTokenSource.Token);
                                this.Logger.Log(TraceEventType.Information, "Finished querying for work items");
                                return ans;
                            },
                        cancellationTokenSource.Token);

                    Task queryErrorTask = runQueryTask.ContinueWith(
                        antecedent =>
                            {
                                this.Logger.Log(TraceEventType.Error, "Query execution failed: {0}", antecedent.Exception.InnerException.Message);
                            },
                        cancellationTokenSource.Token,
                        TaskContinuationOptions.OnlyOnFaulted,
                        scheduler);

                    Task queryDoneTask = runQueryTask.ContinueWith(
                        (antecedent) =>
                            {
                                this.Logger.Log(TraceEventType.Information, "Adding work items to document");
                                this.TeamRibbonPresenter.UpdateCancellableOperation(
                                    FlowControllerResources.AddingWorkItemsToDocument);

                                if (projectPickerPresenter != null)
                                {
                                    projectPickerPresenter.SaveTeamProject();
                                }

                                workItemAndLayoutPickerWizardPresenter.SaveQueryAndLayout();
                                this.Manager.ActiveDocument.SaveWorkItems(antecedent.Result, queryAndLayout.Layout.FieldNames.ToArray(), cancellationTokenSource.Token);
                                this.Manager.ActiveDocument.MapWorkItemsIntoDocument(queryAndLayout.Layout, queryAndLayout.Index, cancellationTokenSource.Token);
                                this.Logger.Log(TraceEventType.Information, "Finished adding work items to document");
                            },
                        cancellationTokenSource.Token,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        scheduler);

                    this.AddFinalErrorHandlingTask(scheduler, runQueryTask, queryErrorTask, queryDoneTask);

                    runQueryTask.Start(scheduler);
                    this.TeamRibbonPresenter.StartCancellableOperation(FlowControllerResources.StartQueryExecution, cancellationTokenSource);
                }
            }

            this.TeamRibbonPresenter.UpdateState();
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbonPresenter.Refresh"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// Reads the query in the document, executes it, and stores the results.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not appropiate to dispose Task Parallel objects")]
        private void HandleTeamRibbonRefresh(object sender, System.EventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Flow controller HandleTeamRibbonRefresh called");
            Debug.Assert(this.Manager.ActiveDocument != null, "There should be an active document in the team project document manager");
            ITeamProjectDocument doc = this.Manager.ActiveDocument;

            if (!this.Manager.ActiveDocument.IsInsertable)
            {
                this.TeamRibbonPresenter.DisplayError(FlowControllerResources.DocumentNotRefreshable, string.Empty);
            }
            else
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                TaskScheduler scheduler = this.UnityContainer.Resolve<TaskScheduler>();

                Task runRefreshTask = new Task(
                    () =>
                        {
                            this.Logger.Log(TraceEventType.Information, "Starting refresh");
                            try
                            {
                                IEnumerable<string> errors = doc.RefreshWorkItems(cancellationTokenSource.Token);
                                if (errors.Count() > 0)
                                {
                                    this.TeamRibbonPresenter.DisplayError(FlowControllerResources.CannotRefreshInvalidDocument, string.Join(Environment.NewLine, errors.ToArray()));
                                }
                            }
                            finally
                            {
                                this.Logger.Log(TraceEventType.Information, "End of asynch operation");
                            }
                        },
                    cancellationTokenSource.Token);

                this.AddFinalErrorHandlingTask(scheduler, runRefreshTask);

                runRefreshTask.Start(scheduler);
                this.TeamRibbonPresenter.StartCancellableOperation(FlowControllerResources.StartRefreshExecution, cancellationTokenSource);
            }
        }

        /// <summary>
        /// Adds a task to the antecedents that handles any errors from the antecedents.
        /// </summary>
        /// <param name="scheduler">The scheduler to be used.</param>
        /// <param name="antecedents">The antecedent tasks.</param>
        private void AddFinalErrorHandlingTask(TaskScheduler scheduler, params Task[] antecedents)
        {
            Task.Factory.ContinueWhenAll(
                antecedents,
                (tasks) =>
                {
                    this.Logger.Log(TraceEventType.Information, "End of asynch operation");
                    try
                    {
                        Exception[] exceptions = tasks.Where(t => t.Exception != null).Select(t => t.Exception.InnerException).Where(ex => ex.GetType() != typeof(TaskCanceledException)).ToArray();
                        if (exceptions.Length == 1)
                        {
                            this.TeamRibbonPresenter.DisplayError(Utilities.FormatException(exceptions), string.Empty);
                        }
                        else if (exceptions.Length > 1)
                        {
                            // Can't test this path as there should not be a situation where this actually occurs.
                            this.TeamRibbonPresenter.DisplayError(FlowControllerResources.OperationFailed, Utilities.FormatException(exceptions));
                        }
                    }
                    finally
                    {
                        this.TeamRibbonPresenter.EndCancellableOperation();
                        this.Logger.Log(TraceEventType.Information, "End of asynch operation task completed");
                    }
                },
                new CancellationToken(),
                TaskContinuationOptions.None,
                scheduler);
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbonPresenter.ShowLayoutDesigner"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// Displays the layout designer.
        /// </remarks>
        private void HandleShowLayoutDesigner(object sender, EventArgs e)
        {
            if (this.LayoutDesignerPresenter == null)
            {
                this.LayoutDesignerPresenter = this.UnityContainer.Resolve<ILayoutDesignerPresenter>();
                this.SetupLayoutDesignerPresenterEvents();
            }

            this.LayoutDesignerPresenter.Show();
            this.TeamRibbonPresenter.UpdateState();
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbonPresenter.HideLayoutDesigner"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// Displays the layout designer.
        /// </remarks>
        private void HandleHideLayoutDesigner(object sender, EventArgs e)
        {
            this.LayoutDesignerPresenter.Hide();
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerPresenter.Connect"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleLayoutDesignerPresenterConnect(object sender, EventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Flow controller HandleLayoutDesignerPresenterConnect called");
            ITeamProjectPickerPresenter projectPickerPresenter = this.Manager.ActiveContainer.Resolve<ITeamProjectPickerPresenter>();
            ITeamProject teamProject = projectPickerPresenter.ChooseTeamProject();
            if (teamProject != null)
            {
                projectPickerPresenter.SaveTeamProject();
            }
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerPresenter.Close"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleLayoutDesignerPresenterClose(object sender, EventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Flow controller HandleLayoutDesignerPresenterClose called");
            this.LayoutDesignerPresenter = null;
        }
    }
}

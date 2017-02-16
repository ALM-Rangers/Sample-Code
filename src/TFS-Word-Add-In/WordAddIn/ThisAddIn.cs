//---------------------------------------------------------------------
// <copyright file="ThisAddIn.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ThisAddIn type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Microsoft.Win32;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Windows;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The main class for the entry into the addin.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Caused by unity container setup and so not a real problem.")]
    public partial class ThisAddIn
    {
        /// <summary>
        /// The Unity container to be used.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The team ribbon.
        /// </summary>
        private TeamRibbonView teamRibbon;

        /// <summary>
        /// A value indicating whether the flow controller has been initialised yet.
        /// </summary>
        private bool flowControllerInitialised = false;

        /// <summary>
        /// Creates the ribbon object, initialised with its presenter.
        /// </summary>
        /// <returns>The array of ribbons to be added to Word.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The apparent excessive coupling comes from the fact that we are setting up Unity here")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The ribbon is passed out from this method and should not be disposed")]
        protected override Microsoft.Office.Tools.Ribbon.IRibbonExtension[] CreateRibbonObjects()
        {
            ILogger logger = new Logger();
            logger.Log(TraceEventType.Information, "Word4Tfs creating team ribbon");
            this.container = new UnityContainer();
            this.container.RegisterInstance<ThisAddIn>(this);
            this.container.RegisterInstance<ILogger>(logger);
            this.container.RegisterType<ILowLevelSettings, LowLevelSettings>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ISettings, Settings>();
            this.container.RegisterType<IFile, FileAbstraction>();
            this.container.RegisterType<IDirectory, DirectoryAbstraction>();
            this.container.RegisterInstance<TaskScheduler>(TaskScheduler.Default);
            this.container.RegisterType<ITeamRibbonView, TeamRibbonView>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ITeamRibbonPresenter, TeamRibbonPresenter>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ILayoutDesignerView, LayoutDesignerTaskPaneControl>();
            this.container.RegisterType<ILayoutDesignerPresenter, LayoutDesignerPresenter>();
            this.container.RegisterType<IMessageBoxView, RightToLeftAwareMessageBox>();
            this.container.RegisterType<ITeamProjectPickerView, TeamProjectPickerView>();
            this.container.RegisterType<ITeamProjectPickerPresenter, TeamProjectPickerPresenter>();

            this.container.RegisterType<IWorkItemQueryPickerWizardPageView, WorkItemQueryPickerWizardPageView>();
            this.container.RegisterType<ILayoutPickerWizardPageView, LayoutPickerWizardPageView>();
            this.container.RegisterType<IWorkItemQueryAndLayoutPickerWizardView, WorkItemQueryAndLayoutPickerWizardView>(new PerResolveLifetimeManager()); // Lifetime manager used to ensure view is shared by the same presenter and page presenters
            this.container.RegisterType<IWorkItemQueryPickerWizardPagePresenter, WorkItemQueryPickerWizardPagePresenter>();
            this.container.RegisterType<ILayoutPickerWizardPagePresenter, LayoutPickerWizardPagePresenter>();
            this.container.RegisterType<IWorkItemQueryAndLayoutPickerWizardPresenter, WorkItemQueryAndLayoutPickerWizardPresenter>();

            this.container.RegisterType<ITeamProjectDocumentManager, TeamProjectDocumentManager>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ITeamProjectTemplate, TeamProjectTemplate>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ITeamProjectDocument, TeamProjectDocument>(new HierarchicalLifetimeManager());
            this.container.RegisterType<ITeamProjectDocumentFormatter, TeamProjectDocumentFormatter>();
            this.container.RegisterType<ILayoutDefinitionFormatter, LayoutDefinitionFormatter>();
            this.container.RegisterType<ITeamProjectDocumentVerifier, TeamProjectDocumentVerifier>();
            this.container.RegisterType<IWordApplication, WordApplication>(new ContainerControlledLifetimeManager());
            this.container.RegisterInstance<IFactory>(new Factory(logger));

            this.teamRibbon = this.container.Resolve<TeamRibbonView>();
            this.teamRibbon.tabTeam.Visible = false;

            this.container.RegisterInstance<IWaitNotifier>(this.teamRibbon);

            return new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { this.teamRibbon };
        }

        /// <summary>
        /// Startup code.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            ILogger logger = this.container.Resolve<ILogger>();
            try
            {
                logger.Log(TraceEventType.Information, "Word4Tfs Add-in starting");

                this.UseDifferentCredentialsHackInitialize();

                this.container.RegisterInstance<Microsoft.Office.Interop.Word.Application>(Globals.ThisAddIn.Application);
                this.Application.DocumentChange += new Office.Interop.Word.ApplicationEvents4_DocumentChangeEventHandler(this.Application_DocumentChange);
            }
            catch (Exception ex)
            {
                Utilities.DisplayMessage(UIMessageType.Error, DialogueResources.MicrosoftWord, DialogueResources.StartupError, Microsoft.Word4Tfs.Library.Utilities.FormatException(ex));
            }
        }

        /// <summary>
        /// Handles the <see cref="Application.DocumentChange"/> event.
        /// </summary>
        /// <remarks>
        /// Initialises the flow controller only when the first non Protected View document is opened. Also hides the Team ribbon for Protected View documents.
        /// </remarks>
        private void Application_DocumentChange()
        {
            ILogger logger = this.container.Resolve<ILogger>();
            logger.Log(TraceEventType.Verbose, "Add in doc change, process id {0}", Process.GetCurrentProcess().Id);
            if (!this.flowControllerInitialised && !this.InProtectedView())
            {
                this.flowControllerInitialised = true;
                this.teamRibbon.tabTeam.Visible = true;
                FlowController flowController = this.container.Resolve<FlowController>();
                flowController.Initialise();
            }

            this.teamRibbon.tabTeam.Visible = !this.InProtectedView();
        }

        /// <summary>
        /// Checks to see if the current document is in protected view.
        /// </summary>
        /// <returns><c>True</c> if the current document is in protected view, <c>false</c> otherwise.</returns>
        private bool InProtectedView()
        {
            bool ans = true;

            // Cannot use the ActiveProtectedViewWindow property because it does not exist in Word 2007
            try
            {
                ans = this.Application.ActiveDocument == null;
            }
            catch (COMException)
            {
            }

            return ans;
        }

        /// <summary>
        /// Shutdown code.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            ILogger logger = this.container.Resolve<ILogger>();
            try
            {
                this.UseDifferentCredentialsHackShutdown();

                logger.Log(TraceEventType.Information, "Word4Tfs Add-in stopping");
            }
            catch (Exception ex)
            {
                Utilities.DisplayMessage(UIMessageType.Error, DialogueResources.MicrosoftWord, DialogueResources.ShutdownError, Microsoft.Word4Tfs.Library.Utilities.FormatException(ex));
            }
        }

        /// <summary>
        /// There is a bug in the TFS TeamProjectPicker class, and it won't allow the user to click "Use Different Credentials" without a call to
        /// an internal initialisation method.
        /// </summary>
        private void UseDifferentCredentialsHackInitialize()
        {
            ILogger logger = this.container.Resolve<ILogger>();

            Type windowsHostType = Type.GetType("Microsoft.TeamFoundation.Client.WindowsHost, Microsoft.TeamFoundation.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            if (windowsHostType != null)
            {
                MethodInfo initializeMethod = windowsHostType.GetMethod("Initialize", Type.EmptyTypes);
                if (initializeMethod != null)
                {
                    initializeMethod.Invoke(null, new object[0]);
                }
                else
                {
                    logger.Log(TraceEventType.Error, "Could not find the WindowsHost.Initialize method, cannot enable Use Different Credentials on the TeamProjectPicker");
                }
            }
            else
            {
                logger.Log(TraceEventType.Error, "Could not find the WindowsHost class, cannot enable Use Different Credentials on the TeamProjectPicker");
            }
        }

        /// <summary>
        /// Shuts down the WindowsHost for the use different credentials hack.
        /// </summary>
        private void UseDifferentCredentialsHackShutdown()
        {
            ILogger logger = this.container.Resolve<ILogger>();

            Type uiHostType = Type.GetType("Microsoft.TeamFoundation.Client.UIHost, Microsoft.TeamFoundation.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            if (uiHostType != null)
            {
                MethodInfo shutdownMethod = uiHostType.GetMethod("Shutdown", Type.EmptyTypes);
                if (shutdownMethod != null)
                {
                    shutdownMethod.Invoke(null, new object[0]);
                }
                else
                {
                    logger.Log(TraceEventType.Error, "Could not find the UIHost.Shutdown method, cannot cleanup Use Different Credentials on the TeamProjectPicker");
                }
            }
            else
            {
                logger.Log(TraceEventType.Error, "Could not find the UIHost class, cleanup enable Use Different Credentials on the TeamProjectPicker");
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}

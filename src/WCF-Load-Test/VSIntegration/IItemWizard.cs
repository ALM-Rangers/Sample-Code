//---------------------------------------------------------------------
// <copyright file="IItemWizard.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IItemWizard type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Used to get a wizard form to display.
    /// </summary>
    /// <returns>The form to display.</returns>
    public delegate IWizardForm FormFactory();

    /// <summary>
    /// Used to display errors to the user that cause the wizard to cancel.
    /// </summary>
    /// <param name="errorMessage">The message to display.</param>
    public delegate void ErrorDisplay(string errorMessage);

    /// <summary>
    /// Represents the wizard functions required for this item type.
    /// </summary>
    /// <remarks>
    /// This interface is similar to <see cref="IWizard"/> but without the <see cref="EnvDTE"/>
    /// dependencies that prevent classes using that interface being easily mocked for unit testing
    /// purposes. It also includes extra methods for error handling so that this can be tested for.
    /// </remarks>
    public interface IItemWizard
    {
        /// <summary>
        /// Gets the list of path to the assemblies containing proxies that were referenced in order to generate the code.
        /// </summary>
        /// <value>The list of paths to the assemblies containing proxies that were referenced in order to generate the code.</value>
        Collection<string> ProxyAssemblies
        {
            get;
        }

        /// <summary>
        /// Called when the wizard is started.
        /// </summary>
        /// <param name="name">The name to be given to the file(s).</param>
        void RunStarted(string name);

        /// <summary>
        /// Determines if the project item should be added.
        /// </summary>
        /// <remarks>
        /// This is called once per file in the item template (i.e. twice). The flag
        /// indicating the first call is reset after the item file name has been captured.
        /// </remarks>
        /// <param name="filePath">The file to be added.</param>
        /// <returns>True if the file can be added, false otherwise.</returns>
        bool ShouldAddProjectItem(string filePath);

        /// <summary>
        /// Called when finished generating the project item.
        /// </summary>
        /// <remarks>
        /// This is called once per file in the item template (i.e. twice). This is where the
        /// item file name is captured.
        /// </remarks>
        /// <param name="fileName">The name of the file being generated.</param>
        void ProjectItemFinishedGenerating(string fileName);

        /// <summary>
        /// Called just before the file is opened.
        /// </summary>
        void BeforeOpeningFile();

        /// <summary>
        /// Called when the wizard has finished executing.
        /// </summary>
        void RunFinished();
    }
}

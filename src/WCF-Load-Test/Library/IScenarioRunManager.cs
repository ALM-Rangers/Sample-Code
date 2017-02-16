//---------------------------------------------------------------------
// <copyright file="IScenarioRunManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IScenarioRunManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;

    /// <summary>
    /// Sets up the configuration file for capturing a suitable trace, runs the application
    /// under test and then restores the configuration at the end.
    /// </summary>
    public interface IScenarioRunManager : IDisposable
    {
        /// <summary>
        /// Initialises the object to work with the given executable.
        /// </summary>
        /// <param name="executableFileName">The executable to be managed.</param>
        void Initialize(string executableFileName);

        /// <summary>
        /// Sets up the executable's configuration file.
        /// </summary>
        /// <returns>The path to the trace file that will be generated.</returns>
        string SetupForTrace();

        /// <summary>
        /// Runs the program and gets the configuration information.
        /// </summary>
        /// <remarks>
        /// The assemblies found to add to the list to look for the proxy are limited to non-GAC assemblies.
        /// </remarks>
        /// <param name="configuration">The configuration that is to be updated with information from the trace.</param>
        void Run(WcfUnitConfiguration configuration);
        
        /// <summary>
        /// Restores the executable's configuration file to its original state.
        /// </summary>
        void RestoreOriginalConfiguration();
    }
}

//---------------------------------------------------------------------
// <copyright file="WizardData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WizardData type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    /// <summary>
    /// Value to indicate where a trace file has come from.
    /// </summary>
    public enum TraceFileSource
    {
        /// <summary>
        /// Indicates that the trace file was dynamically generated as a result of running the wizard.
        /// </summary>
        Dynamic,

        /// <summary>
        /// Indicates that the trace file was pre-captured.
        /// </summary>
        PreCaptured
    }

    /// <summary>
    /// Contains the data gathered by the wizard
    /// </summary>
    public sealed class WizardData
    {
        /// <summary>
        /// The configuration data.
        /// </summary>
        private WcfUnitConfiguration configuration;

        /// <summary>
        /// The name of the trace file.
        /// </summary>
        private string traceFile;

        /// <summary>
        /// The source of the trace file.
        /// </summary>
        private TraceFileSource traceFileSource;

        /// <summary>
        /// Gets or sets the configuration data.
        /// </summary>
        /// <value>The configuration data.</value>
        public WcfUnitConfiguration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        /// <summary>
        /// Gets or sets the name of the trace file.
        /// </summary>
        /// <value>The name of the trace file.</value>
        public string TraceFile
        {
            get { return this.traceFile; }
            set { this.traceFile = value; }
        }

        /// <summary>
        /// Gets or sets the source of the trace file.
        /// </summary>
        /// <value>The source of the trace file.</value>
        public TraceFileSource TraceFileSource
        {
            get
            {
                return this.traceFileSource;
            }

            set
            {
                this.traceFileSource = value;
            }
        }
    }
}

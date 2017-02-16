//---------------------------------------------------------------------
// <copyright file="ILayoutDefinitionFormatter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILayoutDefinitionFormatter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations for displaying and creating layout definitions.
    /// </summary>
    public interface ILayoutDefinitionFormatter
    {
        /// <summary>
        /// Displays a layout definition in a template.
        /// </summary>
        /// <param name="projectTemplate">The template containing the layout definition.</param>
        /// <param name="layoutName">The name of the layout to be displayed.</param>
        void DisplayDefinition(ITeamProjectTemplate projectTemplate, string layoutName);

        /// <summary>
        /// Saves a layout definition to the template
        /// </summary>
        /// <param name="projectTemplate">The template to save the definitions to.</param>
        /// <param name="layoutName">The name of the layout to be saved.</param>
        void SaveDefinition(ITeamProjectTemplate projectTemplate, string layoutName);

        /// <summary>
        /// Adds a prototype layout definition to the document, used when creating a new layout.
        /// </summary>
        void AddPrototypeDefinition();
    }
}

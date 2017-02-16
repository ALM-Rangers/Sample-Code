//---------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Constants type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Global constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The namespace for the XML used to store the unique handle in the Word document.
        /// </summary>
        public const string HandleInformationNamespace = "urn:www.microsoft.com:rangers:word4tfs:handleInformation";

        /// <summary>
        /// The namespace for the XML used to store project information in the Word document.
        /// </summary>
        public const string ProjectInformationNamespace = "urn:www.microsoft.com:rangers:word4tfs:projectInformation";

        /// <summary>
        /// The namespace for the XML used to store the query in the Word document.
        /// </summary>
        public const string QueryNamespace = "urn:www.microsoft.com:rangers:word4tfs:query";

        /// <summary>
        /// The namespace for the XML used to store the work items in the Word Document
        /// </summary>
        public const string WorkItemNamespace = "urn:www.microsoft.com:rangers:word4tfs:workItem";

        /// <summary>
        /// The namespace for the XML used to store the query work item association data in the Word Document
        /// </summary>
        public const string QueryWorkItemNamespace = "urn:www.microsoft.com:rangers:word4tfs:queryWorkItemAssociation";

        /// <summary>
        /// The name of the building block in a category which represents the default for the layout.
        /// </summary>
        public const string DefaultBuildingBlockName = "Default";

        /// <summary>
        /// The name of the building block in a category which represents the preview for the layout.
        /// </summary>
        public const string PreviewBuildingBlockName = "Preview";

        /// <summary>
        /// The regular expression pattern that identifies a reference name.
        /// </summary>
        public const string ReferenceNamePattern = @"[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z0-9_]+)+";

        /// <summary>
        /// The reference name for the work item ID field.
        /// </summary>
        public const string SystemIdFieldReferenceName = "System.Id";

        /// <summary>
        /// The reference name for the work item type field.
        /// </summary>
        public const string SystemWorkItemTypeFieldReferenceName = "System.WorkItemType";

        /// <summary>
        /// The name of the XML element in a work item that identifies a field
        /// </summary>
        public const string WorkItemFieldElementName = "Field";

        /// <summary>
        /// The name of the XML attribute in a work item that identifies a field name
        /// </summary>
        public const string WorkItemFieldNameAttributeName = "name";

        /// <summary>
        /// The name of the system template.
        /// </summary>
        public const string SystemTemplateName = "Word4TFSSystemTemplate.dotx";

        /// <summary>
        /// The name of the setting for template upgrades.
        /// </summary>
        public const string TemplateUpgradeSettingName = "IgnoreSystemTemplateUpgradeFor";

       /// <summary>
       /// The name of the setting that controls if bookmarks are to be shown.
       /// </summary>
        public const string ShowBookmarksSettingName = "ShowBookmarks";

        /// <summary>
        /// The name of the style used to mark out a work item in a layout when exporting the layouts to a document for editing. This style is defined in the system template.
        /// </summary>
        public const string WorkItemDefinitionStyleName = "TFS Work Item Definition";

        /// <summary>
        /// The name of the Normal style.
        /// </summary>
        public const string NormalStyleName = "Normal";

        /// <summary>
        /// The temporary name to be given to a layout when adding a new one.
        /// </summary>
        public const string PrototypeLayoutName = "New Layout";
    }
}

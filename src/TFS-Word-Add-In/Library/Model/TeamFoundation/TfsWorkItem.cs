//---------------------------------------------------------------------
// <copyright file="TfsWorkItem.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItem type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Provides access to the TFS <see cref="WorkItem"/> class.
    /// </summary>
    public class TfsWorkItem : ITfsWorkItem
    {
        /// <summary>
        /// The namespace for work items.
        /// </summary>
        private static XNamespace workItemNamespace = Constants.WorkItemNamespace;

        /// <summary>
        /// The TFS work item that this object wraps.
        /// </summary>
        private WorkItem workItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsWorkItem"/> class.
        /// </summary>
        /// <param name="workItem">The TFS work item that this object wraps.</param>
        public TfsWorkItem(WorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException("workItem");
            }

            this.workItem = workItem;
            this.FieldReferenceNames = (from Field field in workItem.Fields select field.ReferenceName).ToList();
        }

        /// <summary>
        /// Gets the ID of this work item.
        /// </summary>
        public int Id
        {
            get
            {
                return this.workItem.Id;
            }
        }

        /// <summary>
        /// Gets the name of the work item type.
        /// </summary>
        public string Type
        {
            get
            {
                return this.workItem.Type.Name;
            }
        }

        /// <summary>
        /// Gets the list of reference names of the fields in the work item.
        /// </summary>
        public IEnumerable<string> FieldReferenceNames { get; private set; }

        /// <summary>
        /// Gets or sets the value of a field in this work item specified by the field name.
        /// </summary>
        /// <param name="name">The name that is passed in <paramref name="name"/>could be either the field name or a reference name</param>
        /// <returns>The object that is contained in this field.</returns>
        public object this[string name]
        {
            get
            {
                if (this.workItem.Fields[name].FieldDefinition.FieldType == FieldType.History)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = this.workItem.Revisions.Count - 1; i >= 0; i--)
                    {
                        Revision revision = this.workItem.Revisions[i];
                        string history = (string)revision.Fields["History"].Value;
                        DateTime revisedDate = (DateTime)revision.Fields["Changed Date"].Value;
                        sb.Append(revisedDate + " " + (string.IsNullOrEmpty(history) ? "&lt;no comment&gt;" : history));
                        sb.Append("<br>");
                    }

                    return string.Concat("<html><head/><body>", sb.ToString(), "</body></html>");
                }
                else if (name == "Microsoft.VSTS.TCM.Steps")
                {
                    string testCase = this.workItem[name].ToString();
                    string formattedTestCase = Utilities.FormatTestCase(testCase);

                    return formattedTestCase;
                }
                else if (this.workItem.Fields[name].FieldDefinition.FieldType == FieldType.Html)
                {
                    return string.Concat("<html><head/><body>", this.workItem[name], "</body></html>");
                }
                else
                {
                    return this.workItem[name];
                }
            }
        }

        /// <summary>
        /// Serializes a work item to XML.
        /// </summary>
        /// <remarks>
        /// The work item id and type are always serialized, regardless of which other fields are to be serialized.
        /// </remarks>
        /// <param name="workItem">The work item to be serialized.</param>
        /// <param name="fieldNames">The names of fields to serialize.</param>
        /// <returns>The XElement that represents the work item.</returns>
        public static XElement Serialize(ITfsWorkItem workItem, params string[] fieldNames)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException("workItem");
            }

            fieldNames = fieldNames.Union(new string[] { Constants.SystemIdFieldReferenceName, Constants.SystemWorkItemTypeFieldReferenceName }).ToArray();

            XElement ans = new XElement(
                                        workItemNamespace + "WorkItem",
                                        fieldNames.Where(fieldName => workItem.FieldReferenceNames.Contains(fieldName)).Select(fieldName => SerializeField(workItem, fieldName)));
            return ans;
        }

        /// <summary>
        /// Serializes a field from a work item.
        /// </summary>
        /// <param name="workItem">The work item containing the field to be serialized.</param>
        /// <param name="fieldName">The name of the field to be serialized.</param>
        /// <returns>The serialized field.</returns>
        private static XElement SerializeField(ITfsWorkItem workItem, string fieldName)
        {
            XElement ans = new XElement(workItemNamespace + Constants.WorkItemFieldElementName, new XAttribute(Constants.WorkItemFieldNameAttributeName, fieldName), workItem[fieldName]);
            return ans;
        }
    }
}

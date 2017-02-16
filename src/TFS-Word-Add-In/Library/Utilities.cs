//---------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Utilities type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// General utility methods
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// The pattern used to check that a tag is a valid reference name.
        /// </summary>
        private static Regex tagValidationPattern = new Regex(Constants.ReferenceNamePattern);

        /// <summary>
        /// The pattern used to parse bookmark names.
        /// </summary>
        private static Regex bookmarkParsingPattern = new Regex(@"^TFS_WI_Q(?<queryIndex>(\d+))_W(?<workItemId>(\d+))$");

        /// <summary>
        /// The pattern used to parse xpath for work item id.
        /// </summary>
        private static Regex xpathParsingPattern = new Regex(@"^/wi:WorkItems/wi:WorkItem\[wi:Field\[@name='System.Id'\]=(?<workItemId>(\d+))]");

        /// <summary>
        /// Validates a content control tag.
        /// </summary>
        /// <remarks>
        /// A tag should only contain a Reference Name for a work item field.
        /// </remarks>
        /// <param name="tag">The tag to be validated.</param>
        /// <returns><c>True</c> if the tag is valid, <c>false</c> otherwise.</returns>
        public static bool IsValidTag(string tag)
        {
            return tagValidationPattern.IsMatch(tag);
        }

        /// <summary>
        /// Returns a function that will return the bookmark name for a <paramref name="queryIndex"/> and a <paramref name="workItemId"/>.
        /// </summary>
        /// <param name="queryIndex">The query index.</param>
        /// <param name="workItemId">The work item Id.</param>
        /// <returns>The bookmark name for the query and work item.</returns>
        public static string BookmarkNamingFunction(int queryIndex, int workItemId)
        {
            return QuerySpecificBookmarkNamingFunction(queryIndex)(workItemId);
        }

        /// <summary>
        /// Returns a function that will return the bookmark name for work items associated with <paramref name="queryIndex"/>.
        /// </summary>
        /// <param name="queryIndex">The query index the method needs to return a function for.</param>
        /// <returns>A function that will return the bookmark name for work items associated with <paramref name="queryIndex"/>.</returns>
        public static Func<int, string> QuerySpecificBookmarkNamingFunction(int queryIndex)
        {
            string bookmarkFormat = string.Format(CultureInfo.InvariantCulture, "TFS_WI_Q{0}_W{{0}}", queryIndex);
            return (int workItemId) => string.Format(CultureInfo.InvariantCulture, bookmarkFormat, workItemId);
        }

        /// <summary>
        /// Parses a bookmark name to extract the query index and work item id.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark.</param>
        /// <returns>Tuple with the query index and work item id, or <c>null</c> if not valid.</returns>
        public static Tuple<int, int> BookmarkParsingFunction(string bookmarkName)
        {
            Tuple<int, int> ans = null;
            Match m = bookmarkParsingPattern.Match(bookmarkName);
            if (m.Success)
            {
                int qid = int.Parse(m.Groups[1].Captures[0].Value, CultureInfo.InvariantCulture);
                int wid = int.Parse(m.Groups[2].Captures[0].Value, CultureInfo.InvariantCulture);
                ans = new Tuple<int, int>(qid, wid);
            }

            return ans;
        }

        /// <summary>
        /// Parses an xpath to get the work item id the xpath maps to.
        /// </summary>
        /// <param name="xpath">The xpath to be parsed.</param>
        /// <returns>The work item id, or <c>null</c> if not a valid work item xpath.</returns>
        public static Nullable<int> XpathParsingFunction(string xpath)
        {
            Nullable<int> ans = null;
            Match m = xpathParsingPattern.Match(xpath);
            if (m.Success)
            {
                ans = int.Parse(m.Groups[1].Captures[0].Value, CultureInfo.InvariantCulture);
            }

            return ans;
        }

        /// <summary>
        /// Formats a list of exceptions.
        /// </summary>
        /// <param name="exceptions">The exceptions to format.</param>
        /// <returns>The formatted exceptions.</returns>
        public static string FormatException(params Exception[] exceptions)
        {
            if (exceptions == null)
            {
                throw new ArgumentNullException("exceptions");
            }

            StringBuilder sb = new StringBuilder();
            int i = 1;
            bool moreThanOneException = exceptions.Length > 1;
            foreach (Exception ex in exceptions)
            {
                Exception current = ex;
                do
                {
                    bool isOutermostException = current == ex;
                    if (!isOutermostException)
                    {
                        sb.AppendLine();
                        sb.AppendFormat(CultureInfo.InvariantCulture, "Inner Exception: ");
                    }

                    if (moreThanOneException && isOutermostException)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}{2}", i, current.Message, Environment.NewLine);
                    }
                    else
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", current.Message, Environment.NewLine);
                    }

#if DEBUG
                    if (!string.IsNullOrEmpty(current.StackTrace))
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "Stack Trace:{1}{0}{1}", current.StackTrace, Environment.NewLine);
                    }
#endif
                    current = current.InnerException;
                }
                while (current != null);

                if (moreThanOneException && i < exceptions.Length)
                {
                    sb.AppendLine();
                }

                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the path of the directory where the executable is running from.
        /// </summary>
        /// <returns>The path of the directory where the executable is running from.</returns>
        public static string ReadExecutingDirectory()
        {
            string ans = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Remove(0, 6); // remove file:\ prefix
            return ans;
        }

        /// <summary>
        /// Formats a TFS test case work item.
        /// </summary>
        /// <param name="testCase">The test case work item string.</param>
        /// <returns>The formatted test case.</returns>
        public static string FormatTestCase(string testCase)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("<html><body>");
                XDocument testStepsDoc = XDocument.Parse(testCase);
                int i = 1;
                foreach (XElement step in testStepsDoc.Root.Elements("step"))
                {
                    // Pre 2012 code: string[] fields = step.Elements("parameterizedString").Select(xe => string.Join(" ", xe.Elements("text").Select(te => te.Value))).ToArray();
                    string[] fields = step.Elements("parameterizedString").Select(xe => xe.Value).ToArray();
                    sb.Append(i.ToString(CultureInfo.InvariantCulture) + " " + string.Join(" ", fields));
                    i++;
                }

                sb.Append("</body></html>");
            }
            catch (XmlException)
            {
            }

            return sb.ToString();
        }
    }
}

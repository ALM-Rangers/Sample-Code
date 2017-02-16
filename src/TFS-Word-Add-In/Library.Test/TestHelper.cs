//---------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TestHelper type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Xml;
    using global::System.Xml.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Provides miscellaneous test utility methods
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// The name of the project to use in tests.
        /// </summary>
        internal const string ProjectName = "Word4Tfs";

        /// <summary>
        /// A flat query to be used in tests.
        /// </summary>
        internal const string FlatQuery = "SELECT [System.Id], [Microsoft.VSTS.Common.StackRank], [Microsoft.VSTS.Scheduling.StoryPoints], [System.Title], [System.State], [System.IterationPath], [System.AreaPath], [System.WorkItemType] FROM WorkItems WHERE [System.TeamProject] = @project and [System.AreaPath] under @project and [System.WorkItemType] = 'User Story' and ([System.State] <> 'Closed' or [Microsoft.VSTS.Common.ClosedDate] > @today - 90) ORDER BY [Microsoft.VSTS.Common.StackRank]";

        /// <summary>
        /// A tree query to be used in tests.
        /// </summary>
        internal const string TreeQuery = "SELECT [System.Id], [Microsoft.VSTS.Common.StackRank], [Microsoft.VSTS.Scheduling.StoryPoints], [System.Title], [System.State], [System.IterationPath], [System.AreaPath], [System.WorkItemType] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = @project and [Source].[System.AreaPath] under @project and [Source].[System.WorkItemType] = 'User Story' and [Source].[System.State] <> 'Closed') and ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') and (Target.[System.WorkItemType] <> '') ORDER BY [Microsoft.VSTS.Common.StackRank] mode(Recursive)";

        /// <summary>
        /// A one-hop query to be used in tests.
        /// </summary>
        internal const string OneHopQuery = "SELECT [System.Id], [System.Links.LinkType], [Microsoft.VSTS.Common.StackRank], [Microsoft.VSTS.Scheduling.StoryPoints], [System.Title], [System.State], [System.IterationPath], [System.AreaPath], [System.WorkItemType] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = @project  AND  [Source].[System.AreaPath] UNDER @project  AND  [Source].[System.WorkItemType] = 'User Story'  AND  [Source].[System.State] <> 'Closed') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.WorkItemType] = 'User Story') ORDER BY [Microsoft.VSTS.Common.StackRank] mode(MustContain)";

        /// <summary>
        /// A query to be used in tests to get fields of each field type.
        /// </summary>
        internal const string AllFieldTypesQuery = "SELECT [System.Id], [System.Title], [System.Description], [System.CreatedDate], [System.History], [System.IterationPath], [Microsoft.VSTS.TCM.ReproSteps], [Microsoft.VSTS.Common.StackRank] FROM WorkItems";

        /// <summary>
        /// The work item type to use in most tests.
        /// </summary>
        internal const string StandardTestWorkItemType = "StandardTestWorkItemType";

        /// <summary>
        /// The test category for integration tests.
        /// </summary>
        internal const string TestCategoryIntegration = "Integration";

        /// <summary>
        /// The test category for unit tests.
        /// </summary>
        internal const string TestCategoryUnit = "Unit";

        /// <summary>
        /// The test category for spike tests.
        /// </summary>
        internal const string TestCategorySpike = "Spike";

        /// <summary>
        /// A valid WIQL query.
        /// </summary>
        internal const string ValidQuery = "Select System.Id, System.Title from Task";

        /// <summary>
        /// A valid WIQL query.
        /// </summary>
        internal const string ValidQuery2 = "Select System.Area, System.Description from Task";

        /// <summary>
        /// A valid Project Collection URI.
        /// </summary>
        internal const string ValidProjectCollectionUri = "urn:www.contoso.com-test";

        /// <summary>
        /// A valid Team Project name.
        /// </summary>
        internal const string ValidProjectName = "TestProject";

        /// <summary>
        /// A valid layout name.
        /// </summary>
        internal const string ValidLayoutName = "test layout";

        /// <summary>
        /// A valid layout name.
        /// </summary>
        internal const string ValidLayoutName2 = "test layout 2";

        /// <summary>
        /// The prefix to category names that indicates a category is being used as a layout
        /// </summary>
        internal const string CategoryPrefix = "TFS_";

        /// <summary>
        /// Used to generate unique sequential bookmark ranges.
        /// </summary>
        private static int lastBookmarkPosition = 1;

        /// <summary>
        /// Gets a valid Project Collection Id.
        /// </summary>
        private static Guid validProjectCollectionId = Guid.NewGuid();

        /// <summary>
        /// Gets a query definition for a valid WIQL query.
        /// </summary>
        internal static QueryDefinition ValidQueryDefinition
        {
            get
            {
                return new QueryDefinition("test", TestHelper.ValidQuery);
            }
        }

        /// <summary>
        /// Gets a query definition and layout for a valid WIQL query and layout.
        /// </summary>
        /// <remarks>
        /// The layout will result in the query not needing to be modified because the field list matches the field list of the query.
        /// </remarks>
        internal static QueryAndLayoutInformation ValidQueryDefinitionAndLayout
        {
            get
            {
                return new QueryAndLayoutInformation(new QueryDefinition("test", TestHelper.ValidQuery), CreateTestLayoutInformation(ValidQueryDefinitionAndLayoutFields));
            }
        }

        /// <summary>
        /// Gets an alternative query definition and layout for a valid WIQL query and layout.
        /// </summary>
        /// <remarks>
        /// The layout will result in the query not needing to be modified because the field list matches the field list of the query.
        /// </remarks>
        internal static QueryAndLayoutInformation ValidQueryDefinitionAndLayout2
        {
            get
            {
                return new QueryAndLayoutInformation(new QueryDefinition("test", TestHelper.ValidQuery2), CreateTestLayoutInformation(ValidLayoutName2, ValidQueryDefinitionAndLayoutFields2));
            }
        }

        /// <summary>
        /// Gets multiple query definitions and layouts for valid WIQL queries and layouts.
        /// </summary>
        /// <remarks>
        /// The layout will result in the query not needing to be modified because the field list matches the field list of the query.
        /// </remarks>
        internal static QueryAndLayoutInformation ValidQueryDefinitionAndLayoutMultiple
        {
            get
            {
                return new QueryAndLayoutInformation(new QueryDefinition("test", TestHelper.ValidQuery), CreateTestLayoutInformation(ValidQueryDefinitionAndLayoutFields));
            }
        }

        /// <summary>
        /// Gets the list of fields defined by <see cref="ValidQueryDefinitionAndLayout"/>.
        /// </summary>
        internal static string[] ValidQueryDefinitionAndLayoutFields
        {
            get
            {
                return new string[] { Constants.SystemIdFieldReferenceName, "System.Title" };
            }
        }

        /// <summary>
        /// Gets the list of fields defined by <see cref="ValidQueryDefinitionAndLayout2"/>.
        /// </summary>
        internal static string[] ValidQueryDefinitionAndLayoutFields2
        {
            get
            {
                return new string[] { "System.Area", "System.Description" };
            }
        }

        /// <summary>
        /// Gets the XML for for an empty query and layout section.
        /// </summary>
        internal static string EmptyQueryAndLayoutXml
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"<QueriesAndLayouts xmlns=""{0}"" />", Constants.QueryNamespace);
            }
        }

        /// <summary>
        /// Gets the XML for <see cref="ValidQuery"/> and <see cref="ValidLayoutName"/>.
        /// </summary>
        internal static string ValidQueryAndLayoutXml
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"<QueriesAndLayouts xmlns=""{0}""><QueryAndLayout><Query>{1}</Query><LayoutName>{2}</LayoutName></QueryAndLayout></QueriesAndLayouts>", Constants.QueryNamespace, ValidQuery, ValidLayoutName);
            }
        }

        /// <summary>
        /// Gets the XML for <see cref="ValidQuery"/> , <see cref="ValidLayoutName"/>, <see cref="ValidQuery2"/> and <see cref="ValidLayoutName2"/>.
        /// </summary>
        internal static string ValidQueryAndLayoutXmlMultiple
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"<QueriesAndLayouts xmlns=""{0}""><QueryAndLayout><Query>{1}</Query><LayoutName>{2}</LayoutName></QueryAndLayout><QueryAndLayout><Query>{3}</Query><LayoutName>{4}</LayoutName></QueryAndLayout></QueriesAndLayouts>", Constants.QueryNamespace, ValidQuery, ValidLayoutName, ValidQuery2, ValidLayoutName2);
            }
        }

        /// <summary>
        /// Gets the XML for a valid project.
        /// </summary>
        internal static string ValidProjectXml
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId=""{1}"" ProjectName=""{2}"" xmlns=""{3}"" />", ValidProjectCollectionUri, validProjectCollectionId, ValidProjectName, Constants.ProjectInformationNamespace);
            }
        }

        /// <summary>
        /// Gets a valid Project Collection Id.
        /// </summary>
        internal static Guid ValidProjectCollectionId
        {
            get { return validProjectCollectionId; }
        }

        /// <summary>
        /// Returns the valid XML for a collection of work items.
        /// </summary>
        /// <param name="workItems">Array of elements representing the XML for each work item in a collection.</param>
        /// <returns>The XML for a collection of work items.</returns>
        internal static string ValidWorkItemsXml(params XElement[] workItems)
        {
            XNamespace ns = Constants.WorkItemNamespace;
            XElement root = new XElement(ns + "WorkItems", workItems);
            return root.ToString();
        }

        /// <summary>
        /// Returns the valid XML for query and work item association data.
        /// </summary>
        /// <param name="queryWorkItemIds">The list of arrays of work item id, one array per query.</param>
        /// <returns>The XML that associated a set of queries and their work items.</returns>
        internal static string ValidQueryWorkItemsXml(params int[][] queryWorkItemIds)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "<Queries xmlns='{0}'>", Constants.QueryWorkItemNamespace);
            foreach (int[] ids in queryWorkItemIds)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "<Query>");
                sb.AppendFormat(CultureInfo.InvariantCulture, "<WorkItems>");
                foreach (int id in ids)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "<WorkItem Id='{0}'/>", id);
                }

                sb.AppendFormat(CultureInfo.InvariantCulture, "</WorkItems>");
                sb.AppendFormat(CultureInfo.InvariantCulture, "</Query>");
            }

            sb.AppendFormat(CultureInfo.InvariantCulture, "</Queries>");

            return sb.ToString();
        }

        /// <summary>
        /// Creates query and work item association data.
        /// </summary>
        /// <param name="queryWorkItemIds">The list of arrays of work item id, one array per query.</param>
        /// <returns>The query and work item association data.</returns>
        internal static QueryWorkItems[] CreateQueryWorkItems(params int[][] queryWorkItemIds)
        {
            int i = 0;
            return queryWorkItemIds.Select(qw => new QueryWorkItems(i++, qw)).ToArray();
        }

        /// <summary>
        /// Creates query and work item association data from work item trees.
        /// </summary>
        /// <param name="workItemTrees">The list of work item trees to use to create the data.</param>
        /// <returns>The query and work item association data.</returns>
        internal static QueryWorkItems[] CreateQueryWorkItems(params WorkItemTree[] workItemTrees)
        {
            int i = 0;
            return workItemTrees.Select(node => new QueryWorkItems(i++, node.DepthFirstNodes().ToArray())).ToArray();
        }

        /// <summary>
        /// Creates a mock work item.
        /// </summary>
        /// <param name="id">The id of the mock work item.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The mock work item.</returns>
        internal static ITfsWorkItem CreateMockWorkItem(int id, params Tuple<string, object>[] fieldsAndValues)
        {
            ITfsWorkItem mockItem = CreateMockWorkItem(StandardTestWorkItemType, id, fieldsAndValues);
            return mockItem;
        }

        /// <summary>
        /// Creates a mock work item.
        /// </summary>
        /// <param name="workItemType">The type of the work item.</param>
        /// <param name="id">The id of the mock work item.</param>
        /// <param name="title">The title of the work item.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The mock work item.</returns>
        internal static ITfsWorkItem CreateMockWorkItem(string workItemType, int id, string title, params Tuple<string, object>[] fieldsAndValues)
        {
            Tuple<string, object>[] initial = new Tuple<string, object>[] { new Tuple<string, object>("System.Title", title) };
            ITfsWorkItem mockItem = CreateMockWorkItem(workItemType, id, initial.Union(fieldsAndValues).ToArray());
            return mockItem;
        }

        /// <summary>
        /// Creates a mock work item.
        /// </summary>
        /// <param name="workItemType">The type of the work item.</param>
        /// <param name="id">The id of the mock work item.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The mock work item.</returns>
        internal static ITfsWorkItem CreateMockWorkItem(string workItemType, int id, params Tuple<string, object>[] fieldsAndValues)
        {
            List<string> fieldReferenceNames = new List<string>();

            Mock<ITfsWorkItem> mockItem = new Mock<ITfsWorkItem>();

            mockItem.Setup(item => item.Id).Returns(id);
            mockItem.Setup(item => item[Constants.SystemIdFieldReferenceName]).Returns(id);
            fieldReferenceNames.Add(Constants.SystemIdFieldReferenceName);

            mockItem.Setup(item => item.Type).Returns(workItemType);
            mockItem.Setup(item => item[Constants.SystemWorkItemTypeFieldReferenceName]).Returns(workItemType);
            fieldReferenceNames.Add(Constants.SystemWorkItemTypeFieldReferenceName);

            foreach (Tuple<string, object> fieldAndValue in fieldsAndValues)
            {
                mockItem.Setup(item => item[fieldAndValue.Item1]).Returns(fieldAndValue.Item2);
                fieldReferenceNames.Add(fieldAndValue.Item1);
            }

            mockItem.Setup(item => item.FieldReferenceNames).Returns(fieldReferenceNames);

            return mockItem.Object;
        }

        /// <summary>
        /// Creates and adds mock work items to a work item tree for a flat query.
        /// </summary>
        /// <param name="workItemTree">The tree to add the work items to.</param>
        /// <param name="workItemIds">The ids of the work items to add.</param>
        /// <returns>The mock items in the order they appear in the document.</returns>
        internal static ITfsWorkItem[] PopulateWorkItemTreeWithFlatQueryResults(WorkItemTree workItemTree, params int[] workItemIds)
        {
            ITfsWorkItem[] mockItems = new ITfsWorkItem[workItemIds.Length];
            for (int i = 0; i < workItemIds.Length; i++)
            {
                mockItems[i] = TestHelper.CreateMockWorkItem(workItemIds[i]);
                workItemTree.RootNodes.Add(new WorkItemTreeNode(mockItems[i], 0));
            }

            return mockItems;
        }

        /// <summary>
        /// Creates and adds mock work items to a work item tree for a flat query.
        /// </summary>
        /// <param name="workItemTree">The tree to add the work items to.</param>
        /// <param name="workItems">The work items to add.</param>
        internal static void PopulateWorkItemTreeWithFlatQueryResults(WorkItemTree workItemTree, params ITfsWorkItem[] workItems)
        {
            foreach (ITfsWorkItem workItem in workItems)
            {
                workItemTree.RootNodes.Add(new WorkItemTreeNode(workItem, 0));
            }
        }

        /// <summary>
        /// Creates and adds mock work items to a work item tree for a hierarchical query.
        /// </summary>
        /// <param name="workItems">The tree to add the work items to.</param>
        /// <param name="workItemIdPaths">The ids of the work items to add, expressed as a dotted hierarchy of ids.</param>
        /// <returns>The mock items in the order they appear in the document.</returns>
        internal static ITfsWorkItem[] PopulateWorkItemTreeWithHierarchicalQueryResults(WorkItemTree workItems, params string[] workItemIdPaths)
        {
            foreach (string workItemPath in workItemIdPaths)
            {
                int[] ids = workItemPath.Split('.').Select(s => int.Parse(s)).ToArray();
                WorkItemTreeNode parent = null;
                for (int level = 0; level < ids.Length; level++)
                {
                    int id = ids[level];
                    WorkItemTreeNode existingNode = workItems.DepthFirstNodes().Where(n => n.WorkItem.Id == id).SingleOrDefault();
                    if (existingNode == null)
                    {
                        existingNode = new WorkItemTreeNode(TestHelper.CreateMockWorkItem(id), level);
                        if (parent == null)
                        {
                            workItems.RootNodes.Add(existingNode);
                        }
                        else
                        {
                            parent.Children.Add(existingNode);
                        }
                    }
                    else
                    {
                        Assert.AreEqual<int>(level, existingNode.Level, "Bad test, inconsistent level for work item");
                    }

                    parent = existingNode;
                }
            }

            return workItems.DepthFirstNodes().Select(n => n.WorkItem).ToArray();
        }

        /// <summary>
        /// Creates a mock team project using default test collection URI and project name.
        /// </summary>
        /// <param name="fields">The work item fields that are defined in the mock team project.</param>
        /// <returns>The mock team project.</returns>
        internal static Mock<ITeamProject> CreateMockTeamProject(params string[] fields)
        {
            Mock<ITeamProject> teamProject = CreateMockTeamProject(new Uri(ValidProjectCollectionUri), validProjectCollectionId, ValidProjectName, fields);
            return teamProject;
        }

        /// <summary>
        /// Creates a mock team project.
        /// </summary>
        /// <param name="collectionUri">The URI for the team project collection.</param>
        /// <param name="collectionId">The id for the team project collection.</param>
        /// <param name="projectName">The name of the project within the team project collection.</param>
        /// <param name="fields">The work item fields that are defined in the mock team project.</param>
        /// <returns>The mock team project.</returns>
        internal static Mock<ITeamProject> CreateMockTeamProject(Uri collectionUri, Guid collectionId, string projectName, params string[] fields)
        {
            Mock<ITeamProject> mockTeamProject = new Mock<ITeamProject>();
            TeamProjectInformation tpi = new TeamProjectInformation(collectionUri, collectionId, projectName, null);
            mockTeamProject.Setup(project => project.TeamProjectInformation).Returns(tpi);
            mockTeamProject.Setup(p => p.FieldDefinitions).Returns(TestHelper.CreateMockFieldDefinitions(fields));
            return mockTeamProject;
        }

        /// <summary>
        /// Creates a mock and registers it in the Unity container as an instance.
        /// </summary>
        /// <typeparam name="T">The type of the mock to create.</typeparam>
        /// <param name="container">The container to add the new instance to.</param>
        /// <returns>The new mock.</returns>
        internal static Mock<T> CreateAndRegisterMock<T>(IUnityContainer container) where T : class
        {
            Mock<T> ans = new Mock<T>();
            container.RegisterInstance<T>(ans.Object);
            return ans;
        }

        /// <summary>
        /// Tests that the method under tests throws an <see cref="ArgumentNullException"/> and quotes the expected parameter name.
        /// </summary>
        /// <param name="methodUnderTest">The delegate to call that tests the method.</param>
        /// <param name="expectedParameterName">The expected name of the parameter to which the exception refers.</param>
        internal static void TestForArgumentNullException(Action methodUnderTest, string expectedParameterName)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual(expectedParameterName, ane.ParamName);
            }
        }

        /// <summary>
        /// Tests that the method under test throws an <see cref="InvalidOperationException"/> and quotes the expected error message.
        /// </summary>
        /// <param name="methodUnderTest">The delegate to call that tests the method.</param>
        /// <param name="expectedMessage">The expected error message.</param>
        internal static void TestForInvalidOperationException(Action methodUnderTest, string expectedMessage)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected InvalidOperationException");
            }
            catch (InvalidOperationException ioe)
            {
                Assert.AreEqual(expectedMessage, ioe.Message);
            }
        }

        /// <summary>
        /// Tests that the method under test throws an exception with an inner exception, the inner exception quoting a specific message.
        /// </summary>
        /// <typeparam name="Outer">The type of the expected outer exception.</typeparam>
        /// <typeparam name="Inner">The type of the expected inner exception.</typeparam>
        /// <param name="methodUnderTest">The delegate to call that tests the method.</param>
        /// <param name="expectedMessage">The expected error message on the inner exception.</param>
        internal static void TestForException<Outer, Inner>(Action methodUnderTest, string expectedMessage)
            where Outer : Exception
            where Inner : Exception
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected InvalidOperationException");
            }
            catch (Outer outer)
            {
                Assert.IsInstanceOfType(outer.InnerException, typeof(Inner), string.Format(CultureInfo.InvariantCulture, "Inner exception is not of type {0}", typeof(Inner).Name));
                Assert.AreEqual(expectedMessage, outer.InnerException.Message);
            }
        }

        /// <summary>
        /// Asserts that a particular XML node contains the expected text.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="xml">The XML to be checked.</param>
        /// <param name="xpath">The xpath which identified the data to be checked.</param>
        /// <param name="message">The message to display if the assert fails.</param>
        internal static void AssertXmlNodeContent(string expected, XElement xml, string xpath, string message)
        {
            XmlNode dataNode = GetXmlNodeFromXML(xml, xpath);
            Assert.IsNotNull(dataNode, message);
            Assert.AreEqual<string>(expected, dataNode.InnerText, message);
        }

        /// <summary>
        /// Asserts that a particular XML node is not present in some XML text.
        /// </summary>
        /// <param name="xml">The XML to be checked.</param>
        /// <param name="xpath">The xpath which identifies the node to be checked.</param>
        /// <param name="message">The message to display if the assert fails.</param>
        internal static void AssertXmlNodeMissing(XElement xml, string xpath, string message)
        {
            XmlNode dataNode = GetXmlNodeFromXML(xml, xpath);
            Assert.IsNull(dataNode, message);
        }

        /// <summary>
        /// Asserts that two XML strings are the same.
        /// </summary>
        /// <remarks>
        /// Ignores whitespace in the comparison.
        /// </remarks>
        /// <param name="expected">The expected XML string.</param>
        /// <param name="actual">The actual XML string.</param>
        /// <param name="message">The message to display if the assert fails.</param>
        internal static void AssertXmlEqual(string expected, string actual, string message)
        {
            XDocument actualDoc = XDocument.Parse(actual);
            XDocument expectedDoc = XDocument.Parse(expected);
            Assert.AreEqual<string>(expectedDoc.ToString(), actualDoc.ToString(), message);
        }

        /// <summary>
        /// Moq matcher for an unordered array of unique values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array</typeparam>
        /// <param name="values">The list of unique values</param>
        /// <returns>Matcher for unordered array of unique values.</returns>
        internal static T[] UnorderedUniqueArray<T>(params T[] values)
        {
            return Match<T[]>.Create(list => CheckUnorderedUniqueArrayMatch(values, list));
        }

        /// <summary>
        /// Moq matcher for an ordered array of values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array</typeparam>
        /// <param name="values">The list of ordered values</param>
        /// <returns>Matcher for ordered array of values.</returns>
        internal static IEnumerable<T> OrderedArray<T>(params T[] values)
        {
            return Match<IEnumerable<T>>.Create(list => CheckOrderedArrayMatch(values, list));
        }

        /// <summary>
        /// Moq matcher for an XML document in a particular namespace.
        /// </summary>
        /// <param name="ns">The namespace the document must match.</param>
        /// <returns>Matcher for XML documents in the namespace</returns>
        internal static string IsAnyXmlDocumentInNamespace(string ns)
        {
            return Match<string>.Create(s =>
                {
                    XDocument xdoc = XDocument.Parse(s);
                    return xdoc.Root.Name.Namespace == ns;
                });
        }

        /// <summary>
        /// Moq matcher for a <see cref="QueryDefinition"/>, matches by the text of the query
        /// </summary>
        /// <param name="queryText">The query text that must match.</param>
        /// <returns>Matcher for query matching the query text</returns>
        internal static QueryDefinition IsQueryText(string queryText)
        {
            return Match<QueryDefinition>.Create(qd => qd.QueryText == queryText);
        }

        /// <summary>
        /// Moq matcher for an empty array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array</typeparam>
        /// <returns>Array of unique values.</returns>
        internal static T[] EmptyArray<T>()
        {
            return Match<T[]>.Create(list => list.Length == 0);
        }

        /// <summary>
        /// Moq matcher for an <see cref="Exception"/>, matches by the exception message
        /// </summary>
        /// <param name="message">The exception message that must match.</param>
        /// <returns>Matcher for exception matching the exception message</returns>
        internal static Exception IsException(string message)
        {
            return Match<Exception>.Create(ex => ex.Message == message);
        }

        /// <summary>
        /// Moq matcher for a <see cref="string"/>, matches if the string starts with the chosen string
        /// </summary>
        /// <param name="message">The message that must match.</param>
        /// <returns>Matcher for message matching the start of the given message</returns>
        internal static string MessageStartsWith(string message)
        {
            return Match<string>.Create(s => s.StartsWith(message));
        }

        /// <summary>
        /// Moq matcher for an array of <see cref="Exception"/>, matches by the exception message
        /// </summary>
        /// <param name="message">The exception message that must match.</param>
        /// <returns>Matcher for exception matching the exception message</returns>
        internal static Exception[] IsExceptionArray(string message)
        {
            return Match<Exception[]>.Create(ex => ex[0].Message == message);
        }

        /// <summary>
        /// Creates a test layout information with building blocks with the given names.
        /// </summary>
        /// <param name="names">The names of the building blocks to create for the layout.</param>
        /// <returns>The test layout information.</returns>
        internal static LayoutInformation CreateTestLayoutInformation(params BuildingBlockName[] names)
        {
            return CreateTestLayoutInformation(ValidLayoutName, names);
        }

        /// <summary>
        /// Creates a test layout information with building blocks with the given names and sets up a mock word template to return them.
        /// </summary>
        /// <param name="layoutName">The name of the layout to create.</param>
        /// <param name="names">The names of the building blocks to create for the layout.</param>
        /// <returns>The test layout information.</returns>
        internal static LayoutInformation CreateTestLayoutInformation(string layoutName, params BuildingBlockName[] names)
        {
            return new LayoutInformation(layoutName, names, new string[0], null);
        }

        /// <summary>
        /// Creates a test layout to use.
        /// </summary>
        /// <param name="fields">Extra fields that the layout specifies should be included in the query.</param>
        /// <returns>The test layout.</returns>
        internal static LayoutInformation CreateTestLayoutInformation(string[] fields)
        {
            return CreateTestLayoutInformation(ValidLayoutName, fields);
        }

        /// <summary>
        /// Creates a test layout to use.
        /// </summary>
        /// <param name="name">The name of the layout.</param>
        /// <param name="fields">Extra fields that the layout specifies should be included in the query.</param>
        /// <returns>The test layout.</returns>
        internal static LayoutInformation CreateTestLayoutInformation(string name, string[] fields)
        {
            return new LayoutInformation(name, new BuildingBlockName[0], fields, null);
        }

        /// <summary>
        /// Creates a query and layout.
        /// </summary>
        /// <param name="queryText">The text of the query.</param>
        /// <param name="fields">The fields in the layout.</param>
        /// <returns>The query and layout.</returns>
        internal static QueryAndLayoutInformation CreateQueryAndLayout(string queryText, params string[] fields)
        {
            QueryAndLayoutInformation ans = new QueryAndLayoutInformation(new QueryDefinition("test", queryText), CreateTestLayoutInformation(fields));
            return ans;
        }

        /// <summary>
        /// Sets up mock team project template with building blocks for a layout.
        /// </summary>
        /// <param name="mockTeamProjectTemplate">The mock team project template to set up.</param>
        /// <param name="layout">The layout to which the building blocks belong.</param>
        /// <param name="buildingBlocks">The building blocks that should go into the template.</param>
        internal static void SetupMockTemplateWithBuildingBlocks(Mock<ITeamProjectTemplate> mockTeamProjectTemplate, LayoutInformation layout, params BuildingBlock[] buildingBlocks)
        {
            foreach (BuildingBlock bb in buildingBlocks)
            {
                mockTeamProjectTemplate.Setup(wt => wt.GetLayoutBuildingBlock(layout, new BuildingBlockName(bb))).Returns(bb);
            }
        }

        /// <summary>
        /// Checks two unordered arrays to make sure that the actual matches the contents of the expected array and that all actual values are unique.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="expected">The expected values.</param>
        /// <param name="actual">The actual values.</param>
        /// <returns><c>True</c> if the arrays match, <c>false</c> otherwise.</returns>
        internal static bool CheckUnorderedUniqueArrayMatch<T>(T[] expected, T[] actual)
        {
            bool ans = true;

            if (expected.Length != actual.Length)
            {
                ans = false;
            }
            else if (actual.Distinct().Count() != actual.Length)
            {
                ans = false;
            }
            else if (actual.Union(expected).Count() != actual.Length)
            {
                ans = false;
            }

            return ans;
        }

        /// <summary>
        /// Checks two ordered enumerables to make sure that the actual matches the contents of the expected.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="expected">The expected values.</param>
        /// <param name="actual">The actual values.</param>
        /// <returns><c>True</c> if the arrays match, <c>false</c> otherwise.</returns>
        internal static bool CheckOrderedArrayMatch<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            bool ans = true;

            if (expected.Count() != actual.Count())
            {
                ans = false;
            }

            IEnumerator<T> valuesEnumerator = expected.GetEnumerator();
            IEnumerator<T> listEnumerator = actual.GetEnumerator();

            Console.WriteLine("{0,-50} {1}", "Expected", "Actual");
            bool haveExpected;
            bool haveActual;
            do
            {
                haveExpected = valuesEnumerator.MoveNext();
                haveActual = listEnumerator.MoveNext();
                Console.WriteLine("{0,-50} {1}", haveExpected ? (object)valuesEnumerator.Current : (object)string.Empty, haveActual ? (object)listEnumerator.Current : (object)string.Empty);
                if (haveExpected && haveActual && !valuesEnumerator.Current.Equals(listEnumerator.Current))
                {
                    ans = false;
                }
            }
            while (haveExpected || haveActual);

            return ans;
         }

        /// <summary>
        /// Creates a mock field definition.
        /// </summary>
        /// <param name="name">The name of the mock field.</param>
        /// <returns>The mock field definition.</returns>
        internal static ITfsFieldDefinition CreateMockFieldDefinition(string name)
        {
            return CreateMockFieldDefinition(name, name);
        }

        /// <summary>
        /// Creates a mock field definition.
        /// </summary>
        /// <param name="name">The name of the mock field.</param>
        /// <param name="friendlyName">The friendly name of the mock field.</param>
        /// <returns>The mock field definition.</returns>
        internal static ITfsFieldDefinition CreateMockFieldDefinition(string name, string friendlyName)
        {
            return CreateMockFieldDefinition(name, friendlyName, FieldType.PlainText);
        }

        /// <summary>
        /// Creates a mock field definition.
        /// </summary>
        /// <param name="name">The name of the mock field.</param>
        /// <param name="friendlyName">The friendly name of the mock field.</param>
        /// <param name="fieldType">The type of the field.</param>
        /// <returns>The mock field definition.</returns>
        internal static ITfsFieldDefinition CreateMockFieldDefinition(string name, string friendlyName, FieldType fieldType)
        {
            Mock<ITfsFieldDefinition> ans = new Mock<ITfsFieldDefinition>();
            ans.Setup(fd => fd.ReferenceName).Returns(name);
            ans.Setup(fd => fd.FriendlyName).Returns(friendlyName);
            ans.Setup(fd => fd.FieldType).Returns(fieldType);
            return ans.Object;
        }

        /// <summary>
        /// Creates a mock field definitions.
        /// </summary>
        /// <param name="names">The name of the mock fields.</param>
        /// <returns>The mock field definitions.</returns>
        internal static ITfsFieldDefinition[] CreateMockFieldDefinitions(params string[] names)
        {
            ITfsFieldDefinition[] ans = new ITfsFieldDefinition[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                ans[i] = CreateMockFieldDefinition(names[i]);
            }

            return ans;
        }

        /// <summary>
        /// Asserts that a task was cancelled.
        /// </summary>
        /// <param name="t">The task to check.</param>
        internal static void AssertTaskCancelled(global::System.Threading.Tasks.Task t)
        {
            try
            {
                t.Wait();
                Assert.Fail("Was not cancelled.");
            }
            catch (AggregateException ae)
            {
                Assert.IsInstanceOfType(ae.InnerException, typeof(OperationCanceledException), "Was not cancelled");
            }
        }

        /// <summary>
        /// Dummy rebind callback.
        /// </summary>
        /// <returns>Dummy return value</returns>
        internal static Uri DummyRebindCallback()
        {
            return null;
        }

        /// <summary>
        /// Dummy bookmark naming function for query 0.
        /// </summary>
        /// <param name="workItemId">The id of the work item to be named.</param>
        /// <returns>The bookmark name</returns>
        internal static string DummyBookmarkNamingFuncQuery0(int workItemId)
        {
            return DummyBookmarkNamingFunc(0, workItemId);
        }

        /// <summary>
        /// Dummy bookmark naming function for query 1.
        /// </summary>
        /// <param name="workItemId">The id of the work item to be named.</param>
        /// <returns>The bookmark name</returns>
        internal static string DummyBookmarkNamingFuncQuery1(int workItemId)
        {
            return DummyBookmarkNamingFunc(1, workItemId);
        }

        /// <summary>
        /// Dummy bookmark naming function.
        /// </summary>
        /// <param name="queryIndex">The query index of the query.</param>
        /// <param name="workItemId">The id of the work item.</param>
        /// <returns>The bookmark name</returns>
        internal static string DummyBookmarkNamingFunc(int queryIndex, int workItemId)
        {
            return "Q " + queryIndex.ToString() + " W " + workItemId.ToString();
        }

        /// <summary>
        /// Dummy bookmark parsing function.
        /// </summary>
        /// <param name="bookmarkName">the bookmark to parse.</param>
        /// <returns>The query index and work item id, or null if not a work item bookmark.</returns>
        internal static Tuple<int, int> DummyBookmarkParsingFunc(string bookmarkName)
        {
            Tuple<int, int> ans = null;
            string[] parts = bookmarkName.Split(' ');
            if (parts.Length == 4)
            {
                ans = new Tuple<int, int>(int.Parse(parts[1]), int.Parse(parts[3]));
            }

            return ans;
        }

        /// <summary>
        /// Dummy xpath parsing function.
        /// </summary>
        /// <param name="xpath">the xpath to parse.</param>
        /// <returns>The work item id, or null if not a work item mapping.</returns>
        internal static Nullable<int> DummyXPathParsingFunc(string xpath)
        {
            Nullable<int> ans = null;

            int id;
            if (int.TryParse(xpath, out id))
            {
                ans = id;
            }

            return ans;
        }

        /// <summary>
        /// Creates a mock <see cref="Bookmark"/>.
        /// </summary>
        /// <param name="queryIndex">The query index the work item belongs to.</param>
        /// <param name="workItemId">The work item id of the work item.</param>
        /// <param name="contentControls">Any content controls contained inside the bookmark.</param>
        /// <returns>The mock bookmark</returns>
        internal static Bookmark CreateMockBookmark(int queryIndex, int workItemId, params ContentControl[] contentControls)
        {
            Bookmark ans = CreateMockBookmark(DummyBookmarkNamingFunc(queryIndex, workItemId), lastBookmarkPosition, lastBookmarkPosition + 9, contentControls);
            lastBookmarkPosition += 10;
            return ans;
        }

        /// <summary>
        /// Creates a mock <see cref="Bookmark"/>.
        /// </summary>
        /// <param name="queryIndex">The query index the work item belongs to.</param>
        /// <param name="workItemId">The work item id of the work item.</param>
        /// <param name="start">The start of the bookmark's range.</param>
        /// <param name="end">The end of the bookmark's range.</param>
        /// <param name="contentControls">Any content controls contained inside the bookmark.</param>
        /// <returns>The mock bookmark</returns>
        internal static Bookmark CreateMockBookmark(int queryIndex, int workItemId, int start, int end, params ContentControl[] contentControls)
        {
            return CreateMockBookmark(DummyBookmarkNamingFunc(queryIndex, workItemId), start, end, contentControls);
        }

        /// <summary>
        /// Creates a mock <see cref="Bookmark"/>.
        /// </summary>
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="contentControls">Any content controls contained inside the bookmark.</param>
        /// <returns>The mock bookmark</returns>
        internal static Bookmark CreateMockBookmark(string name, params ContentControl[] contentControls)
        {
            Bookmark ans = CreateMockBookmark(name, lastBookmarkPosition, lastBookmarkPosition + 9, contentControls);
            lastBookmarkPosition += 10;
            return ans;
        }

        /// <summary>
        /// Creates a mock <see cref="Bookmark"/>.
        /// </summary>
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="start">The start of the bookmark's range.</param>
        /// <param name="end">The end of the bookmark's range.</param>
        /// <param name="contentControls">Any content controls contained inside the bookmark.</param>
        /// <returns>The mock bookmark</returns>
        internal static Bookmark CreateMockBookmark(string name, int start, int end, params ContentControl[] contentControls)
        {
            Mock<Bookmark> bookmark = new Mock<Bookmark>();
            bookmark.Setup(bm => bm.Name).Returns(name);
            bookmark.Setup(bm => bm.Start).Returns(start);
            bookmark.Setup(bm => bm.End).Returns(end);
            bookmark.Setup(bm => bm.Range).Returns(CreateMockRange(start, end, contentControls).Object);
            return bookmark.Object;
        }

        /// <summary>
        /// Sets up a mock bookmark manager with a set of bookmarks.
        /// </summary>
        /// <param name="bookmarks">The bookmarks to put in the bookmark manager.</param>
        /// <returns>The mock bookmark manager.</returns>
        internal static IBookmarkManager CreateMockBookmarkManager(params Bookmark[] bookmarks)
        {
            IBookmarkManager ans = CreateMockBookmarkManager(CreateMockDocument(bookmarks));
            return ans;
        }

        /// <summary>
        /// Sets up a mock bookmark manager with a set of bookmarks.
        /// </summary>
        /// <param name="document">The mock document which the bookmark manager is to use.</param>
        /// <returns>The mock bookmark manager.</returns>
        internal static IBookmarkManager CreateMockBookmarkManager(Mock<Document> document)
        {
            IBookmarkManager ans = new BookmarkManager(document.Object);
            return ans;
        }

        /// <summary>
        /// Creates a mock <see cref="Document"/> with some bookmarks.
        /// </summary>
        /// <param name="bookmarks">The bookmarks to add to the mock document.</param>
        /// <returns>The mock <see cref="Document"/>.</returns>
        internal static Mock<Document> CreateMockDocument(params Bookmark[] bookmarks)
        {
            Mock<Document> mockDocument = new Mock<Document>();
            SetMockBookmarksForMockDocument(mockDocument, bookmarks);
            return mockDocument;
        }

        /// <summary>
        /// Adds mock bookmarks to a mock document.
        /// </summary>
        /// <param name="mockDocument">The mock document the bookmarks are to be set for.</param>
        /// <param name="bookmarks">The bookmarks to set.</param>
        internal static void SetMockBookmarksForMockDocument(Mock<Document> mockDocument, params Bookmark[] bookmarks)
        {
            Mock<Bookmarks> mockBookmarks = new Mock<Bookmarks>();
            mockBookmarks.Setup(collection => collection.GetEnumerator()).Returns(() => bookmarks.GetEnumerator()); // needs a new enumerator on each call to GetEnumerator, hence function is needed.
            mockDocument.Setup(d => d.Bookmarks).Returns(mockBookmarks.Object);
        }

        /// <summary>
        /// Creates a mock content control.
        /// </summary>
        /// <param name="from">The start of the content control.</param>
        /// <param name="to">The end of the content control.</param>
        /// <param name="isMapped">Indicates if the content control is mapped.</param>
        /// <param name="xpath">If mapped, the XPath for the mapping.</param>
        /// <returns>The mock content control.</returns>
        internal static ContentControl CreateMockContentControl(int from, int to, bool isMapped, string xpath)
        {
            Mock<ContentControl> ans = new Mock<ContentControl>();
            ans.Setup(cc => cc.ID).Returns(Guid.NewGuid().ToString());
            ans.Setup(cc => cc.Range).Returns(CreateMockRange(from, to).Object);
            Mock<XMLMapping> mockMapping = CreatetMockMapping(isMapped, xpath);
            ans.Setup(cc => cc.XMLMapping).Returns(mockMapping.Object);
            return ans.Object;
        }

        /// <summary>
        /// Creates a mock building block.
        /// </summary>
        /// <param name="name">The name of the building block to create.</param>
        /// <returns>The mock building block.</returns>
        internal static BuildingBlock CreateMockBuildingBlock(BuildingBlockName name)
        {
            Mock<BuildingBlock> buildingBlock = new Mock<BuildingBlock>();
            buildingBlock.Setup(bb => bb.Name).Returns(name.ToString());
            return buildingBlock.Object;
        }

        /// <summary>
        /// Creates a mock range.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="end">The end of the range.</param>
        /// <param name="contentControls">The content controls within the range.</param>
        /// <returns>The mock range.</returns>
        internal static Mock<Office.Interop.Word.Range> CreateMockRange(int start, int end, params ContentControl[] contentControls)
        {
            Mock<Office.Interop.Word.Range> mockRange = new Mock<Office.Interop.Word.Range>();
            mockRange.Setup(r => r.Start).Returns(start);
            mockRange.Setup(r => r.End).Returns(end);
            Mock<ContentControls> mockContentControls = new Mock<ContentControls>();
            mockContentControls.Setup(cc => cc.GetEnumerator()).Returns(contentControls.GetEnumerator());
            mockRange.Setup(r => r.ContentControls).Returns(mockContentControls.Object);
            return mockRange;
        }

        /// <summary>
        /// Creates a mock XMLMapping.
        /// </summary>
        /// <param name="isMapped">Indicates if the mapping is m</param>
        /// <param name="xpath">The xpath for the mapping.</param>
        /// <returns>The mock mapping.</returns>
        private static Mock<XMLMapping> CreatetMockMapping(bool isMapped, string xpath)
        {
            Mock<XMLMapping> mockMapping = new Mock<XMLMapping>();
            mockMapping.Setup(m => m.IsMapped).Returns(isMapped);
            mockMapping.Setup(m => m.XPath).Returns(xpath);
            return mockMapping;
        }

        /// <summary>
        /// Gets the node specified by an xpath from some XML
        /// </summary>
        /// <param name="xml">The XML to be searched.</param>
        /// <param name="xpath">The xpath to the node.</param>
        /// <returns>The node, <c>null</c> if not found.</returns>
        private static XmlNode GetXmlNodeFromXML(XElement xml, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml.ToString());
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("wi", Constants.WorkItemNamespace);
            XmlNode dataNode = xmlDoc.SelectSingleNode(xpath, namespaceManager);
            return dataNode;
        }
    }
}

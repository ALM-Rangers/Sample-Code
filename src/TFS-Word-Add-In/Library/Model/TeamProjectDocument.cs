//---------------------------------------------------------------------
// <copyright file="TeamProjectDocument.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocument type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Text.RegularExpressions;
    using global::System.Threading;
    using global::System.Xml;
    using global::System.Xml.Linq;
    using global::System.Xml.Schema;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// The model that represents the team project and the work items that the document is working with.
    /// </summary>
    /// <remarks>
    /// This class is stateful, so only a single instance of this class must exist for each open document.
    /// </remarks>
    public class TeamProjectDocument : ITeamProjectDocument
    {
        /// <summary>
        /// The schema for project information.
        /// </summary>
        private const string ProjectInformationSchema =
@"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' targetNamespace='urn:www.microsoft.com:rangers:word4tfs:projectInformation' xmlns:tns='urn:www.microsoft.com:rangers:word4tfs:projectInformation' elementFormDefault='qualified' attributeFormDefault='unqualified'>
    <xsd:simpleType name='ProjectName'>
      <xsd:restriction base='xsd:string'>
        <xsd:minLength value='1'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:simpleType name='guid'>
        <xsd:restriction base='xsd:string'>
          <xsd:pattern value='^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$'/>
        </xsd:restriction>
    </xsd:simpleType>
    <xsd:simpleType name='CollectionUri'>
      <xsd:restriction base='xsd:anyURI'>
        <xsd:minLength value='1'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:simpleType name='CollectionId'>
      <xsd:restriction base='xsd:anyURI'>
        <xsd:minLength value='1'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:element name='Project'>
      <xsd:complexType>
        <xsd:attribute name='CollectionUri' type='tns:CollectionUri' use='required'/>
        <xsd:attribute name='CollectionId' type='tns:guid' use='required'/>
        <xsd:attribute name='ProjectName' type='tns:ProjectName' use='required'/>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>";

        /// <summary>
        /// The schema for the queries and layouts.
        /// </summary>
        private const string QueryAndLayoutSchema =
@"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' targetNamespace='urn:www.microsoft.com:rangers:word4tfs:query' xmlns:tns='urn:www.microsoft.com:rangers:word4tfs:query' elementFormDefault='qualified' attributeFormDefault='unqualified'>
    <xsd:simpleType name='NonEmptyString'>
      <xsd:restriction base='xsd:string'>
        <xsd:minLength value='1'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:element name='QueriesAndLayouts'>
      <xsd:complexType>
        <xsd:sequence>
          <xsd:element name='QueryAndLayout' minOccurs='1' maxOccurs='unbounded'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='Query' type='tns:NonEmptyString'>
                </xsd:element>
                <xsd:element name='LayoutName' type='tns:NonEmptyString'>
                </xsd:element>
              </xsd:sequence>
            </xsd:complexType>
          </xsd:element>
        </xsd:sequence>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>";

        /// <summary>
        /// The schema for the work items.
        /// </summary>
        private const string WorkItemsSchema =
@"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' targetNamespace='urn:www.microsoft.com:rangers:word4tfs:workItem' xmlns:tns='urn:www.microsoft.com:rangers:word4tfs:workItem' elementFormDefault='qualified' attributeFormDefault='unqualified'>
    <xsd:simpleType name='NonEmptyString'>
      <xsd:restriction base='xsd:string'>
        <xsd:minLength value='1'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:simpleType name='ReferenceName'>
      <xsd:restriction base='xsd:string'>
        <xsd:pattern value='[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z0-9_]+)+'/>
      </xsd:restriction>
    </xsd:simpleType>
    <xsd:element name='WorkItems'>
      <xsd:complexType>
        <xsd:sequence>
          <xsd:element name='WorkItem' minOccurs='1' maxOccurs='unbounded'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='Field' minOccurs='1' maxOccurs='unbounded'>
                  <xsd:complexType>
                    <xsd:simpleContent>
                      <xsd:extension base='tns:NonEmptyString'>
                        <xsd:attribute name='name' type='tns:ReferenceName' use='required'/>
                      </xsd:extension>
                    </xsd:simpleContent>
                  </xsd:complexType>
                </xsd:element>
              </xsd:sequence>
            </xsd:complexType>
          </xsd:element>
        </xsd:sequence>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>";

        /// <summary>
        /// The schema for the queries and layouts.
        /// </summary>
        private const string QueryWorkItemsSchema =
@"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' targetNamespace='urn:www.microsoft.com:rangers:word4tfs:queryWorkItemAssociation' xmlns:tns='urn:www.microsoft.com:rangers:word4tfs:queryWorkItemAssociation' elementFormDefault='qualified' attributeFormDefault='unqualified'>
    <xsd:element name='Queries'>
      <xsd:complexType>
        <xsd:sequence>
          <xsd:element name='Query' minOccurs='0' maxOccurs='unbounded'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='WorkItems' minOccurs='1' maxOccurs='1'>
                    <xsd:complexType>
                      <xsd:sequence>
                        <xsd:element name='WorkItem' minOccurs='0' maxOccurs='unbounded'>
                            <xsd:complexType>
                              <xsd:attribute name='Id' type='xsd:int' use='required'/>
                            </xsd:complexType>
                        </xsd:element>
                      </xsd:sequence>
                    </xsd:complexType>
                </xsd:element>
              </xsd:sequence>
            </xsd:complexType>
          </xsd:element>
        </xsd:sequence>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>";

        /// <summary>
        /// The team project template that contains the layout information.
        /// </summary>
        private readonly ITeamProjectTemplate template;

        /// <summary>
        /// The formatter to be used to format work items in the document.
        /// </summary>
        private readonly ITeamProjectDocumentFormatter formatter;

        /// <summary>
        /// The layout definition formatter to be used to show and edit layout definitions in the document.
        /// </summary>
        private readonly ILayoutDefinitionFormatter layoutDefinitionFormatter;

        /// <summary>
        /// The verifier to be used to verify the structure of the document.
        /// </summary>
        private readonly ITeamProjectDocumentVerifier verifier;

        /// <summary>
        /// The factory used to create other objects.
        /// </summary>
        private readonly IFactory factory;

        /// <summary>
        /// The Word document to be used to store the project data.
        /// </summary>
        private IWordDocument wordDocument;

        /// <summary>
        /// The unity container.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// Track whether <see cref="Dispose"/> has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Indicates if <see cref="Load"/> has been called.
        /// </summary>
        private bool loaded;

        /// <summary>
        /// The team project represented by the document.
        /// </summary>
        private ITeamProject teamProject;

        /// <summary>
        /// The query and layout manager used by the document.
        /// </summary>
        private IQueryAndLayoutManager queryAndLayoutManager;

        /// <summary>
        /// Saves the results of each query.
        /// </summary>
        private List<QueryWorkItems> queryWorkItems;

        /// <summary>
        /// The work items that have been most recently saved in the document
        /// </summary>
        private WorkItemTree mostRecentlySavedWorkItems;

        /// <summary>
        /// The <see cref="XName"/> for the Project element.
        /// </summary>
        private XName projectXName;

        /// <summary>
        /// The <see cref="XName"/> for the CollectionUri attribute.
        /// </summary>
        private XName collectionUriXName;

        /// <summary>
        /// The <see cref="XName"/> for the CollectionId attribute.
        /// </summary>
        private XName collectionIdXName;

        /// <summary>
        /// The <see cref="XName"/> for the ProjectId attribute.
        /// </summary>
        private XName projectNameXName;

        /// <summary>
        /// The <see cref="XName"/> for the Query element.
        /// </summary>
        private XName queryXName;

        /// <summary>
        /// The <see cref="XName"/> for the QueriesAndLayouts element.
        /// </summary>
        private XName queriesAndLayoutsXName;

        /// <summary>
        /// The <see cref="XName"/> for the QueryAndLayout element.
        /// </summary>
        private XName queryAndLayoutXName;

        /// <summary>
        /// The <see cref="XName"/> for the LayoutName element.
        /// </summary>
        private XName layoutNameXName;

        /// <summary>
        /// The <see cref="XName"/> for the WorkItems element.
        /// </summary>
        private XName workItemsXName;

        /// <summary>
        /// The <see cref="XName"/> for the WorkItem element.
        /// </summary>
        private XName workItemXName;

        /// <summary>
        /// The <see cref="XName"/> for the Queries element for query and work item association.
        /// </summary>
        private XName queryWorkItemAssociationQueryXName;

        /// <summary>
        /// The <see cref="XName"/> for the WorkItem element for query and work item association.
        /// </summary>
        private XName queryWorkItemAssociationIdXName;

        /// <summary>
        /// The work item manager to be used for managing the work items.
        /// </summary>
        private WorkItemManager workItemManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectDocument"/> class.
        /// </summary>
        /// <param name="container">The unity container to use.</param>
        /// <param name="template">The team project template that contains the layout information.</param>
        /// <param name="formatter">The formatter to be used to format work items.</param>
        /// <param name="layoutDefinitionFormatter">The layout definition formatter to be used to show and edit layout definitions in the document.</param>
        /// <param name="verifier">The verifier to be used to verify the document structure.</param>
        /// <param name="factory">The factory used to create other objects.</param>
        /// <remarks>
        /// the code requires the <paramref name="wordDocument"/> parameter to have this exact name because of the way Unity does parameter overrides.
        /// </remarks>
        public TeamProjectDocument(IUnityContainer container, ITeamProjectTemplate template, ITeamProjectDocumentFormatter formatter, ILayoutDefinitionFormatter layoutDefinitionFormatter, ITeamProjectDocumentVerifier verifier, IFactory factory)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            if (layoutDefinitionFormatter == null)
            {
                throw new ArgumentNullException("layoutDefinitionFormatter");
            }

            if (verifier == null)
            {
                throw new ArgumentNullException("verifier");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this.container = container;
            this.loaded = false;
            this.template = template;
            this.formatter = formatter;
            this.layoutDefinitionFormatter = layoutDefinitionFormatter;
            this.verifier = verifier;
            this.factory = factory;

            XNamespace nsp = Constants.ProjectInformationNamespace;
            this.projectXName = nsp + "Project";
            this.collectionUriXName = "CollectionUri";
            this.collectionIdXName = "CollectionId";
            this.projectNameXName = "ProjectName";

            XNamespace nsq = Constants.QueryNamespace;
            this.queriesAndLayoutsXName = nsq + "QueriesAndLayouts";
            this.queryAndLayoutXName = nsq + "QueryAndLayout";
            this.queryXName = nsq + "Query";
            this.layoutNameXName = nsq + "LayoutName";

            XNamespace nsw = Constants.WorkItemNamespace;
            this.workItemsXName = nsw + "WorkItems";
            this.workItemXName = nsw + "WorkItem";

            XNamespace nsqw = Constants.QueryWorkItemNamespace;
            this.queryWorkItemAssociationQueryXName = nsqw + "Query";
            this.queryWorkItemAssociationIdXName = nsqw + "WorkItem";

            this.workItemManager = new WorkItemManager();
            this.queryWorkItems = new List<QueryWorkItems>();
            
            ILogger logger = this.container.Resolve<ILogger>();
            logger.Log(TraceEventType.Verbose, "Creating a new Team Project Document");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TeamProjectDocument"/> class.
        /// </summary>
        ~TeamProjectDocument()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when the document is closed.
        /// </summary>
        public event EventHandler Close;

        /// <summary>
        /// Raised when the document becomes connected.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Gets a value indicating whether the document is connected to a Team Project.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the document is insertable  at the current location.
        /// </summary>
        public bool IsInsertable
        {
            get { return this.wordDocument.IsInsertable(); }
        }

        /// <summary>
        /// Gets a value indicating whether the document is refreshable.
        /// </summary>
        public bool IsRefreshable
        {
            get
            {
                bool ans = this.IsConnected && this.ContainsQueries && this.LayoutsUsedArePresentInTemplate();
                return ans;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the document is temporary.
        /// </summary>
        /// <remarks>
        /// A temporary document will never be saved. It is used when hosting the layout designer. Setting this to true disables import and refresh.
        /// </remarks>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets a value indicating whether the document has changed since <see cref="MarkDocumentClean"/> was called.
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return this.wordDocument.HasChanged;
            }
        }

        /// <summary>
        /// Gets or sets the team project.
        /// </summary>
        public ITeamProject TeamProject
        {
            get
            {
                return this.teamProject;
            }

            set
            {
                this.teamProject = value;
                if (this.teamProject == null)
                {
                    this.queryAndLayoutManager = null;
                }
                else
                {
                    this.queryAndLayoutManager = this.factory.CreateQueryAndLayoutManager(this.teamProject.FieldDefinitions);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Word document that the Team Project Document is using.
        /// </summary>
        [Dependency]
        public IWordDocument WordDocument
        {
            get
            {
                return this.wordDocument;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.wordDocument = value;
                this.wordDocument.Close += new EventHandler(this.HandleClose);
            }
        }

        /// <summary>
        /// Gets the current list of queries and layouts after modification to account for all the other queries in the document.
        /// </summary>
        public IEnumerable<QueryAndLayoutInformation> QueriesAndLayouts
        {
            get
            {
                return this.queryAndLayoutManager.FinalQueriesAndLayouts;
            }
        }

        /// <summary>
        /// Gets the work items associated with each saved set of work items.
        /// </summary>
        public IEnumerable<QueryWorkItems> QueryWorkItems
        {
            get
            {
                return this.queryWorkItems;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document contains any queries.
        /// </summary>
        private bool ContainsQueries
        {
            get
            {
                bool ans = false;
                if (this.queryAndLayoutManager != null)
                {
                    ans = this.queryAndLayoutManager.OriginalQueriesAndLayouts.Count() > 0;
                }

                return ans;
            }
        }

        /// <summary>
        /// Loads the current state from the document.
        /// </summary>
        /// <param name="rebindCallback">The callback to be called if the document must be rebound because the collection cannot be found or the id does not match. The callback must return null to cancel the rebind.</param>
        /// <returns>List of load warnings, empty if there were no warnings. The document is still considered loaded if there are warnings.</returns>
        public IEnumerable<string> Load(Func<Uri> rebindCallback)
        {
            List<string> warnings = new List<string>();

            if (!this.loaded)
            {
                this.LoadProjectInformation(rebindCallback);
                if (this.teamProject != null)
                {
                    this.LoadQueryAndLayout(warnings);
                    this.LoadWorkItems();
                    this.LoadQueryWorkItems();
                }
            }

            this.loaded = true;

            return warnings;
        }

        /// <summary>
        /// Saves the team project in the document.
        /// </summary>
        public void SaveTeamProject()
        {
            if (this.teamProject == null)
            {
                throw new InvalidOperationException(ModelResources.TeamProjectNotSet);
            }

            XElement root = new XElement(
                                         this.projectXName,
                                         new XAttribute(this.collectionUriXName, this.teamProject.TeamProjectInformation.CollectionUri.AbsoluteUri),
                                         new XAttribute(this.collectionIdXName, this.teamProject.TeamProjectInformation.CollectionId),
                                         new XAttribute(this.projectNameXName, this.teamProject.TeamProjectInformation.ProjectName));

            this.wordDocument.DeleteXmlPart(Constants.ProjectInformationNamespace);
            this.wordDocument.AddXmlPart(root.ToString());
            if (!this.wordDocument.Handle.HasValue)
            {
                this.wordDocument.Handle = Guid.NewGuid();
            }

            this.IsConnected = true;
            if (this.Connected != null)
            {
                this.Connected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds the <paramref name="queryAndLayout"/> to the collection of queries and layouts in the document, but does not save the new query and layout.
        /// </summary>
        /// <param name="queryAndLayout">The query and layout to be added.</param>
        /// <returns>The query and layout with the query modified to include all the fields from all the layouts and exclude any fields not defined in the team project.</returns>
        public QueryAndLayoutInformation AddQueryAndLayout(QueryAndLayoutInformation queryAndLayout)
        {
            if (this.teamProject == null)
            {
                throw new InvalidOperationException(ModelResources.TeamProjectNotSet);
            }

            return this.queryAndLayoutManager.Add(queryAndLayout);
        }

        /// <summary>
        /// Saves the queries and layout names previously added to the document by <see cref="AddQueryAndLayout"/>.
        /// </summary>
        public void SaveQueriesAndLayouts()
        {
            IEnumerable<XElement> queriesAndLayouts = from ql in this.queryAndLayoutManager.OriginalQueriesAndLayouts
                                                      select new XElement(
                                                          this.queryAndLayoutXName,
                                                          new XElement(this.queryXName, ql.Query.QueryText),
                                                          new XElement(this.layoutNameXName, ql.Layout.Name));

            XElement root = new XElement(this.queriesAndLayoutsXName, queriesAndLayouts);
            this.wordDocument.DeleteXmlPart(Constants.QueryNamespace);
            this.wordDocument.AddXmlPart(root.ToString());
        }

        /// <summary>
        /// Saves a tree of work items in the document, merging them with any existing work items.
        /// </summary>
        /// <param name="workItems">The work items to be saved.</param>
        /// <param name="fields">The fields in the work item to be saved.</param>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        public void SaveWorkItems(WorkItemTree workItems, string[] fields, CancellationToken cancellationToken)
        {
            if (workItems == null)
            {
                throw new ArgumentNullException("workItems");
            }

            ITfsWorkItem[] workItemList = workItems.DepthFirstNodes().Select(node => node.WorkItem).ToArray();
            this.workItemManager.AddRange(workItemList);
            this.queryWorkItems.Add(new QueryWorkItems(this.queryWorkItems.Count, workItemList.Select(wi => wi.Id).ToArray()));
            this.SaveQueryWorkItems();
            this.SaveWorkItemsInWorkItemManager(fields, cancellationToken);
            this.mostRecentlySavedWorkItems = workItems;
        }

        /// <summary>
        /// Maps the saved work items into the document using the given layout.
        /// </summary>
        /// <param name="layout">The layout to be used to map the saved work items.</param>
        /// <param name="index">The index in the query and layout manager of the query that id being processed.</param>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="SaveWorkItems"/> has not been called to save the work items first.</exception>
        public void MapWorkItemsIntoDocument(LayoutInformation layout, int index, CancellationToken cancellationToken)
        {
            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            if (this.mostRecentlySavedWorkItems == null)
            {
                throw new InvalidOperationException(ModelResources.NoSavedWorkItems);
            }

            this.formatter.MapWorkItemsIntoDocument(this.mostRecentlySavedWorkItems, layout, Utilities.QuerySpecificBookmarkNamingFunction(index), cancellationToken);
        }

        /// <summary>
        /// Refreshes the work items in the document.
        /// </summary>
        /// <remarks>
        /// This call will update the rich text content controls which are not bound to the Custom XML Parts.
        /// </remarks>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        /// <returns>List of verification errors, empty if there were no errors.</returns>
        public IEnumerable<string> RefreshWorkItems(CancellationToken cancellationToken)
        {
            IEnumerable<string> ans;

            if (this.queryAndLayoutManager == null)
            {
                throw new InvalidOperationException(ModelResources.TeamProjectNotSet);
            }

            ans = this.verifier.VerifyDocument(this.queryWorkItems.ToArray(), Utilities.BookmarkNamingFunction, Utilities.BookmarkParsingFunction, Utilities.XpathParsingFunction);
            if (ans.Count() == 0)
            {
                this.workItemManager.Clear();
                List<QueryWorkItems> afterQueryWorkItems = new List<QueryWorkItems>();
                foreach (QueryAndLayoutInformation queryAndLayout in this.queryAndLayoutManager.FinalQueriesAndLayouts)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    QueryDefinition finalQuery = queryAndLayout.Query;
                    WorkItemTree workItems = this.teamProject.QueryRunner.QueryForWorkItems(finalQuery, cancellationToken);
                    afterQueryWorkItems.Add(new QueryWorkItems(afterQueryWorkItems.Count, workItems.DepthFirstNodes().ToArray()));
                    this.workItemManager.AddRange(workItems.DepthFirstNodes().Select(node => node.WorkItem).ToArray());
                }

                string[] fields = this.queryAndLayoutManager.AllLayoutFields.ToArray();
                FormatterRefreshData refreshData = new FormatterRefreshData
                                                       {
                                                           WorkItemManager = this.workItemManager,
                                                           QueryWorkItemsBefore = this.queryWorkItems.ToArray(),
                                                           QueryWorkItemsAfter = afterQueryWorkItems.ToArray(),
                                                           Layouts = this.queryAndLayoutManager.FinalQueriesAndLayouts.Select(qli => qli.Layout),
                                                           QueryIsFlat = this.queryAndLayoutManager.FinalQueriesAndLayouts.Select(qli => qli.Query.QueryType == QueryType.List)
                                                       };
                this.formatter.RefreshWorkItems(refreshData, (int qi, int id) => Utilities.QuerySpecificBookmarkNamingFunction(qi)(id), cancellationToken);
                this.queryWorkItems = afterQueryWorkItems;
                this.SaveWorkItemsInWorkItemManager(fields, cancellationToken);
                this.SaveQueryWorkItems();
            }

            return ans;
        }

        /// <summary>
        /// Inserts the named layout definition into the document so that the layout can be edited and then updated.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be displayed.</param>
        public void DisplayLayoutDefinition(string layoutName)
        {
            this.layoutDefinitionFormatter.DisplayDefinition(this.template, layoutName);
        }

        /// <summary>
        /// Saves a layout defined in the document into the template.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be saved.</param>
        public void SaveLayoutDefinition(string layoutName)
        {
            this.layoutDefinitionFormatter.SaveDefinition(this.template, layoutName);
        }

        /// <summary>
        /// Renames a layout.
        /// </summary>
        /// <param name="oldLayoutName">The name of the layout to be renamed.</param>
        /// <param name="newLayoutName">The new name for the layout.</param>
        public void RenameLayoutDefinition(string oldLayoutName, string newLayoutName)
        {
            this.template.DeleteLayout(oldLayoutName);
            this.layoutDefinitionFormatter.SaveDefinition(this.template, newLayoutName);
            this.template.Save();
        }

        /// <summary>
        /// Adds a prototype layout definition to the document, used when creating a new layout.
        /// </summary>
        public void AddPrototypeLayoutDefinition()
        {
            this.layoutDefinitionFormatter.AddPrototypeDefinition();
        }

        /// <summary>
        /// Marks the document as clean.
        /// </summary>
        public void MarkDocumentClean()
        {
            this.wordDocument.MarkDocumentClean();
        }

        /// <summary>
        /// Adds a field to the document.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        public void AddField(ITfsFieldDefinition field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            WdContentControlType controlType = WdContentControlType.wdContentControlText;
            if (field.FieldType == FieldType.Html || field.FieldType == FieldType.History)
            {
                controlType = WdContentControlType.wdContentControlRichText;
            }
            else if (field.FieldType == FieldType.DateTime)
            {
                controlType = WdContentControlType.wdContentControlDate;
            }

            this.wordDocument.AddContentControl(field.FriendlyName, field.ReferenceName, (int)controlType);
        }

        /// <summary>
        /// Disposes of the object and any resources it holds.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs actual dispose.
        /// </summary>
        /// <param name="disposing"><c>true</c> if <see cref="Dispose"/> has been called explicitly, <c>false</c> if disposal is being done by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    this.wordDocument.Dispose();
                    if (this.teamProject != null)
                    {
                        this.teamProject.Dispose();
                    }
                }

                // Dispose of unmanaged resources here.

                // Note disposing has been done.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Validates an XML document against a schema.
        /// </summary>
        /// <param name="document">The XML document to be validated.</param>
        /// <param name="schema">The schema the document is to be validated against.</param>
        /// <param name="expectedNamespace">The expected namespace of the document.</param>
        /// <returns><c>True</c> if the document is valid, <c>false</c> otherwise.</returns>
        private static bool ValidateXml(XDocument document, string schema, string expectedNamespace)
        {
            bool valid = true;
            XmlSchemaSet schemas = LoadSchema(expectedNamespace, schema);
            if (document.Root.Name.Namespace != expectedNamespace)
            {
                valid = false;
            }
            else
            {
                document.Validate(schemas, (o, e) => valid = false);
            }

            return valid;
        }

        /// <summary>
        /// Loads an XML schema and returns the schema set.
        /// </summary>
        /// <param name="targetNamespace">The target namespace for the schema</param>
        /// <param name="schema">A string containing the schema.</param>
        /// <returns>The schema set</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Have followed recommended pattern to resolve it, but analyser cannot see this.")]
        private static XmlSchemaSet LoadSchema(string targetNamespace, string schema)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            StringReader sr = null;
            try
            {
                sr = new StringReader(schema);
                using (XmlReader reader = XmlReader.Create(sr))
                {
                    schemas.Add(targetNamespace, reader);
                    sr = null;
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Dispose();
                }
            }

            return schemas;
        }

        /// <summary>
        /// Reads a work item field from the XML for a work item.
        /// </summary>
        /// <param name="workItemXml">The XML to read.</param>
        /// <param name="fieldName">The name of the field to read.</param>
        /// <returns>The value of the work item field.</returns>
        private static string GetWorkItemFieldFromXml(XElement workItemXml, string fieldName)
        {
            XNamespace ns = Constants.WorkItemNamespace;
            XElement element = workItemXml.Descendants(ns + Constants.WorkItemFieldElementName).Where(xml => xml.Attribute(Constants.WorkItemFieldNameAttributeName).Value == fieldName).Single();
            return element.Value;
        }

        /// <summary>
        /// Creates a <see cref="TfsWorkItemDisconnected"/> object for the work item represented by <paramref name="workItemXml"/>.
        /// </summary>
        /// <param name="workItemXml">The XML for the work item.</param>
        /// <returns>The work item.</returns>
        private static ITfsWorkItem CreateDisconnectedWorkItem(XElement workItemXml)
        {
            int id = int.Parse(GetWorkItemFieldFromXml(workItemXml, Constants.SystemIdFieldReferenceName), CultureInfo.InvariantCulture);
            string workItemType = GetWorkItemFieldFromXml(workItemXml, Constants.SystemWorkItemTypeFieldReferenceName);
            Tuple<string, object>[] fields = workItemXml.Elements().Select<XElement, Tuple<string, object>>(xml => new Tuple<string, object>(xml.Attribute(Constants.WorkItemFieldNameAttributeName).Value, xml.Value)).ToArray();
            return new TfsWorkItemDisconnected(id, workItemType, fields);
        }

        /// <summary>
        /// Loads and validates a document from an Custom XML Part.
        /// </summary>
        /// <param name="documentNamespace">The namespace used to get the Custom XML Part.</param>
        /// <param name="schema">The schema that the XML is expected to follow.</param>
        /// <returns>The loaded and validated document, or <c>null</c> if the document is not valid.</returns>
        private XDocument LoadAndValidateDocument(string documentNamespace, string schema)
        {
            ILogger logger = this.container.Resolve<ILogger>();
            XDocument ans = null;
            bool valid = false;
            string stringData = this.GetXmlPartData(documentNamespace);

            if (!string.IsNullOrEmpty(stringData))
            {
                valid = true;
                try
                {
                    ans = XDocument.Parse(stringData);
                }
                catch (XmlException xe)
                {
                    logger.Log(TraceEventType.Warning, "Document in namespace {0} failed to parse and so is not considered connected, the error is {1}", documentNamespace, xe.Message);

                    // Ignore, it just means invalid XML so we don't set the property.
                    valid = false;
                }

                if (valid)
                {
                    valid = ValidateXml(ans, schema, documentNamespace);
                    if (!valid)
                    {
                        logger.Log(TraceEventType.Warning, "Document in namespace {0} failed to validate and so is not considered connected", documentNamespace);
                    }
                }
            }

            if (!valid)
            {
                ans = null;
            }

            return ans;
        }

        /// <summary>
        /// Loads project information and validates it.
        /// </summary>
        /// <param name="rebindCallback">The callback to be called if the document must be rebound because the collection cannot be found or the id does not match.</param>
        private void LoadProjectInformation(Func<Uri> rebindCallback)
        {
            XDocument projectInformationDoc = this.LoadAndValidateDocument(Constants.ProjectInformationNamespace, ProjectInformationSchema);
            if (projectInformationDoc != null)
            {
                Uri uri = new Uri(projectInformationDoc.Descendants(this.projectXName).Single().Attribute(this.collectionUriXName).Value);
                string collectionId = projectInformationDoc.Descendants(this.projectXName).Single().Attribute(this.collectionIdXName).Value;
                string name = projectInformationDoc.Descendants(this.projectXName).Single().Attribute(this.projectNameXName).Value;
                bool rebound = false;
                bool done = false;
                do
                {
                    this.teamProject = this.factory.CreateTeamProject(uri, name, null);
                    if (this.teamProject.TeamProjectInformation.CollectionId != new Guid(collectionId))
                    {
                        uri = rebindCallback();
                        if (uri != null)
                        {
                            rebound = true;
                        }
                        else
                        {
                            done = true;
                        }
                    }
                    else
                    {
                        done = true;
                    }
                }
                while (!done);

                this.queryAndLayoutManager = this.factory.CreateQueryAndLayoutManager(this.teamProject.FieldDefinitions);

                if (rebound)
                {
                    this.SaveTeamProject();
                }

                if (uri != null)
                {
                    this.IsConnected = true;
                }
            }
            else
            {
                this.IsConnected = false;
            }
        }

        /// <summary>
        /// Loads query and layout and validates it.
        /// </summary>
        /// <param name="warnings">List of warnings to which a warning can be added.</param>
        private void LoadQueryAndLayout(List<string> warnings)
        {
            XDocument queryDoc = this.LoadAndValidateDocument(Constants.QueryNamespace, QueryAndLayoutSchema);
            if (queryDoc != null)
            {
                IEnumerable<QueryAndLayoutInformation> queriesAndLayouts = from ql in queryDoc.Descendants(this.queryAndLayoutXName)
                                                                           select new QueryAndLayoutInformation(new QueryDefinition("Saved", ql.Element(this.queryXName).Value), this.GetLayoutForCategory(ql.Element(this.layoutNameXName).Value, warnings));
                this.queryAndLayoutManager.AddRange(queriesAndLayouts.ToArray());
            }
        }

        /// <summary>
        /// Gets the layout with the given name.
        /// </summary>
        /// <param name="layoutName">The name of the layout to get.</param>
        /// <param name="warnings">List of warnings to which a warning can be added.</param>
        /// <returns>The layout with the given name.</returns>
        private LayoutInformation GetLayoutForCategory(string layoutName, List<string> warnings)
        {
            LayoutInformation ans = null;
            ans = this.template.Layouts.Where(li => li.Name == layoutName).SingleOrDefault();
            if (ans == null)
            {
                ans = new LayoutInformation(layoutName, new BuildingBlockName[0], new string[0], null);
                warnings.Add(string.Format(CultureInfo.CurrentCulture, ModelResources.LayoutMissing, layoutName));
            }

            return ans;
        }

        /// <summary>
        /// Loads work items and validates them.
        /// </summary>
        private void LoadWorkItems()
        {
            XDocument workItemDoc = this.LoadAndValidateDocument(Constants.WorkItemNamespace, WorkItemsSchema);
            if (workItemDoc != null)
            {
                IEnumerable<ITfsWorkItem> workItems = from wi in workItemDoc.Descendants(this.workItemXName)
                                                      select CreateDisconnectedWorkItem(wi);
                this.workItemManager.AddRange(workItems.ToArray());
            }
        }

        /// <summary>
        /// Loads the query and work item association data and validates it.
        /// </summary>
        private void LoadQueryWorkItems()
        {
            XDocument queryWorkItemDoc = this.LoadAndValidateDocument(Constants.QueryWorkItemNamespace, QueryWorkItemsSchema);
            if (queryWorkItemDoc != null)
            {
                int i = 0;
                IEnumerable<QueryWorkItems> qwis = queryWorkItemDoc.Descendants(this.queryWorkItemAssociationQueryXName).Select(qwi => this.LoadWorkItemsForQuery(i++, qwi));
                this.queryWorkItems.AddRange(qwis.ToArray());
            }
        }

        /// <summary>
        /// Loads the work items associated with a query.
        /// </summary>
        /// <param name="index">The query index.</param>
        /// <param name="xml">The XML for the work item ids for a particular query.</param>
        /// <returns>A <see cref="QueryWorkItems"/> object for the query and its associated work items.</returns>
        private QueryWorkItems LoadWorkItemsForQuery(int index, XElement xml)
        {
            QueryWorkItems ans = new QueryWorkItems(index, xml.Descendants(this.queryWorkItemAssociationIdXName).Select(e => int.Parse(e.Attribute("Id").Value, CultureInfo.InvariantCulture)).ToArray());
            return ans;
        }

        /// <summary>
        /// Saves the query and work item association data.
        /// </summary>
        private void SaveQueryWorkItems()
        {
            XNamespace ns = Constants.QueryWorkItemNamespace;
            XElement root = new XElement(
                ns + "Queries",
                this.queryWorkItems.Select(qwi => new XElement(ns + "Query", new XElement(ns + "WorkItems", qwi.WorkItemIds.Select(id => new XElement(ns + "WorkItem", new XAttribute("Id", id.ToString(CultureInfo.InvariantCulture))))))));

            this.wordDocument.DeleteXmlPart(Constants.QueryWorkItemNamespace);
            this.wordDocument.AddXmlPart(root.ToString());
        }

        /// <summary>
        /// Checks if the layouts currently used in the document are all present in the template.
        /// </summary>
        /// <returns><c>true</c> if all the layouts currently used in the document are all present in the template, <c>false</c> otherwise.</returns>
        private bool LayoutsUsedArePresentInTemplate()
        {
            bool ans = this.queryAndLayoutManager.FinalQueriesAndLayouts.All(qli => this.template.Layouts.Any(l => qli.Layout != null && l.Name == qli.Layout.Name));
            return ans;
        }

        /// <summary>
        /// Saves the work items currently in the work item manager.
        /// </summary>
        /// <param name="fields">The fields to be extracted from the work items.</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        private void SaveWorkItemsInWorkItemManager(string[] fields, CancellationToken cancellationToken)
        {
            List<XElement> workItemElements = new List<XElement>();
            foreach (ITfsWorkItem wi in this.workItemManager.WorkItems)
            {
                workItemElements.Add(TfsWorkItem.Serialize(wi, fields));
                cancellationToken.ThrowIfCancellationRequested();
            }

            XElement root = new XElement(this.workItemsXName, workItemElements);
            this.wordDocument.DeleteXmlPart(Constants.WorkItemNamespace);
            this.wordDocument.AddXmlPart(root.ToString());
        }

        /// <summary>
        /// Gets the data for a custom XML part.
        /// </summary>
        /// <param name="partNamespace">The namespace of the part to retrieve.</param>
        /// <returns>The XML data in the custom XML part, or <c>null</c> if the part does not exist.</returns>
        private string GetXmlPartData(string partNamespace)
        {
            string ans = null;
            CustomXMLPart part = this.wordDocument.GetXmlPart(partNamespace);
            if (part != null)
            {
                ans = part.XML;
            }

            return ans;
        }

        /// <summary>
        /// Handles the <see cref="IWordDocument.Close"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleClose(object sender, EventArgs e)
        {
            if (this.Close != null)
            {
                this.Close(this, EventArgs.Empty);
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="WorkItemQueryPickerWizardPageView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryPickerWizardPageView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// This the wizard page used to choose a query.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It takes a relatively long time to load the queries into the tree view, so this operation is performed asynchronously using a background worker. While the work item
    /// tree is being loaded a text box is overlaid in the same position as the tree view.
    /// </para>
    /// <para>
    /// The overlay text box is placed in the designer in a random place. The code sizes and positions it according to the position of the tree view.
    /// </para>
    /// </remarks>
    public partial class WorkItemQueryPickerWizardPageView : UserControl, IWorkItemQueryPickerWizardPageView
    {
        /// <summary>
        /// The image index for the closed folder icon.
        /// </summary>
        private const int ClosedFolderImageIndex = 3;

        /// <summary>
        /// The image index for the open folder icon.
        /// </summary>
        private const int OpenFolderImageIndex = 4;

        /// <summary>
        /// The image index for the list query icon.
        /// </summary>
        private const int ListQueryImageIndex = 0;

        /// <summary>
        /// The image index for the tree query icon.
        /// </summary>
        private const int TreeQueryImageIndex = 1;

        /// <summary>
        /// The image index for the one hop query icon.
        /// </summary>
        private const int OneHopQueryImageIndex = 2;

        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueryPickerWizardPageView"/> class.
        /// </summary>
        /// <param name="logger">The object to be used for logging.</param>
        public WorkItemQueryPickerWizardPageView(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised when the user selects a query.
        /// </summary>
        public event EventHandler<QueryItemEventArgs> QuerySelected;

        /// <summary>
        /// Gets the query that has been selected.
        /// </summary>
        /// <remarks>
        /// Only valid after <see cref="StartDialog"/> has returned <c>true</c>.
        /// </remarks>
        public QueryDefinition SelectedQuery { get; private set; }

        /// <summary>
        /// Loads the query hierarchy to display to the user.
        /// </summary>
        /// <param name="rootQueryFolder">The query hierarchy to be displayed.</param>
        public void LoadQueryHierarchy(QueryFolder rootQueryFolder)
        {
            if (rootQueryFolder == null)
            {
                throw new ArgumentNullException("rootQueryFolder");
            }

            this.treeView.Nodes.Clear();
            this.treeView.ImageList = new ImageList();
            this.treeView.ImageList.Images.Add(IconManager.GetImage(@"FlatList_11939.png"));
            this.treeView.ImageList.Images.Add(IconManager.GetImage(@"WorkItemTree_11580.png"));
            this.treeView.ImageList.Images.Add(IconManager.GetImage(@"DirectLinksQuery_11955.png"));
            this.treeView.ImageList.Images.Add(IconManager.GetImage(@"Folder_6222.png"));
            this.treeView.ImageList.Images.Add(IconManager.GetImage(@"Folder_6221.png"));
            this.treeView.ImageList.TransparentColor = Color.Magenta;
            this.treeView.Visible = false;

            this.textBoxWorkingOverlay.Size = this.treeView.Size;
            this.textBoxWorkingOverlay.Location = this.treeView.Location;
            this.textBoxWorkingOverlay.Anchor = this.treeView.Anchor;
            this.textBoxWorkingOverlay.Visible = true;

            this.backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.RunWorkerAsync(rootQueryFolder);
        }

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="message">The warning message to display.</param>
        public void DisplayWarning(string message)
        {
            this.labelWarning.Text = message;
        }

        /// <summary>
        /// Cancels asynchronous operations.
        /// </summary>
        public void Cancel()
        {
            this.backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Gets the query definition from a query item.
        /// </summary>
        /// <param name="queryItem">The query item to get the definition from.</param>
        /// <returns>The query definition for the query corresponding to the query item.</returns>
        private static QueryDefinition GetQueryDefinitionFromQueryItem(QueryItem queryItem)
        {
            return queryItem as QueryDefinition;
        }

        /// <summary>
        /// Determines if the node is at the root of the tree.
        /// </summary>
        /// <param name="treeNode">The node to check.</param>
        /// <returns><c>true</c> if the node is at the root, <c>false</c> otherwise.</returns>
        private static bool IsRootNode(TreeNode treeNode)
        {
            return treeNode.Parent == null;
        }

        /// <summary>
        /// Sets the image index for the given node. The image is used both when the node is selected and unselected.
        /// </summary>
        /// <param name="node">The node to set the index for.</param>
        /// <param name="imageIndex">The image index to be set.</param>
        private static void SetNodeImage(TreeNode node, int imageIndex)
        {
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = imageIndex;
        }

        /// <summary>
        /// Gets the image index for a given query definition.
        /// </summary>
        /// <param name="qd">The query definition to get the image index for.</param>
        /// <returns>The image index that represents the query type in the query definition.</returns>
        private static int GetQueryImageIndex(QueryDefinition qd)
        {
            int imageIndex;
            switch (qd.QueryType)
            {
                case QueryType.List:
                    {
                        imageIndex = ListQueryImageIndex;
                        break;
                    }

                case QueryType.OneHop:
                    {
                        imageIndex = OneHopQueryImageIndex;
                        break;
                    }

                case QueryType.Tree:
                    {
                        imageIndex = TreeQueryImageIndex;
                        break;
                    }

                default:
                    {
                        imageIndex = ListQueryImageIndex;
                        break;
                    }
            }

            return imageIndex;
        }

        /// <summary>
        /// Handles completion of the background worker.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.textBoxWorkingOverlay.Visible = false;
                this.treeView.Visible = true;
            }
        }

        /// <summary>
        /// Performs the actual work of populating the work item tree.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.logger.Log(TraceEventType.Verbose, "Work Item Query Picker starting background query to get query tree");
            QueryFolder rootQueryFolder = (QueryFolder)e.Argument;
            foreach (QueryItem queryItem in rootQueryFolder)
            {
                if (this.backgroundWorker.CancellationPending)
                {
                    break;
                }

                this.GetNodesForQueryFolder(queryItem as QueryFolder, this.treeView.Nodes, 0);
            }

            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            this.logger.Log(TraceEventType.Verbose, "Work Item Query Picker completed background query to get query tree");
        }

        /// <summary>
        /// Adds the nodes from a query folder to the tree view.
        /// </summary>
        /// <param name="queryFolder">The query folder.</param>
        /// <param name="nodes">The current set of tree nodes.</param>
        /// <param name="level">The level in the hierarchy, 0 is the root.</param>
        private void GetNodesForQueryFolder(QueryFolder queryFolder, TreeNodeCollection nodes, int level)
        {
            TreeNode folderNode = this.AddFolderToWorkItemTree(queryFolder, nodes, level);

            foreach (QueryItem queryItem in queryFolder)
            {
                if (this.backgroundWorker.CancellationPending)
                {
                    break;
                }

                QueryFolder subFolder = queryItem as QueryFolder;
                if (subFolder != null)
                {
                    this.GetNodesForQueryFolder(subFolder, folderNode.Nodes, level + 1);
                }
                else
                {
                    QueryDefinition qd = GetQueryDefinitionFromQueryItem(queryItem);
                    TreeNode itemNode = new TreeNode(queryItem.Name);
                    itemNode.Tag = qd;
                    SetNodeImage(itemNode, GetQueryImageIndex(qd));
                    this.AddNodeToWorkItemTree(folderNode, itemNode);
                }
            }
        }

        /// <summary>
        /// Adds a node to a tree.
        /// </summary>
        /// <remarks>
        /// Designed to also work when called from a background worker thread.
        /// </remarks>
        /// <param name="folderNode">The folder to which the node should be added.</param>
        /// <param name="itemNode">The item node to be added.</param>
        private void AddNodeToWorkItemTree(TreeNode folderNode, TreeNode itemNode)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate
                {
                    this.AddNodeToWorkItemTree(folderNode, itemNode);
                });
            }
            else
            {
                folderNode.Nodes.Add(itemNode);
            }
        }

        /// <summary>
        /// Adds a folder to the work item tree.
        /// </summary>
        /// <remarks>
        /// Designed to also work when called from a background worker thread.
        /// </remarks>
        /// <param name="queryFolder">The query folder to be added.</param>
        /// <param name="nodes">The set of nodes to which the new folder should be added.</param>
        /// <param name="level">The level in the tree of the node, 0 is the root.</param>
        /// <returns>The folder node that was added to the tree.</returns>
        private TreeNode AddFolderToWorkItemTree(QueryFolder queryFolder, TreeNodeCollection nodes, int level)
        {
            TreeNode folderNode = null;

            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate
                {
                    folderNode = this.AddFolderToWorkItemTree(queryFolder, nodes, level);
                });
            }
            else
            {
                folderNode = new TreeNode(queryFolder.Name);
                folderNode.Tag = queryFolder;
                nodes.Add(folderNode);
                if (level == 0)
                {
                    SetNodeImage(folderNode, ClosedFolderImageIndex);
                }
                else
                {
                    SetNodeImage(folderNode, ClosedFolderImageIndex);
                }
            }

            return folderNode;
        }

        /// <summary>
        /// Handles the <see cref="TreeView.AfterCollapse"/> event by toggling the folder image.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleAfterCollapse(object sender, TreeViewEventArgs e)
        {
            SetNodeImage(e.Node, ClosedFolderImageIndex);
        }

        /// <summary>
        /// Handles the <see cref="TreeView.AfterExpand"/> event by toggling the folder image.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleAfterExpand(object sender, TreeViewEventArgs e)
        {
            SetNodeImage(e.Node, OpenFolderImageIndex);
        }

        /// <summary>
        /// Handles the <see cref="TreeView.AfterSelect"/> event and raises a selection event for the query item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.QuerySelected != null)
            {
                this.QuerySelected(this, new QueryItemEventArgs(e.Node.Tag as QueryItem));
            }

            // SelectedQuery must be null if this is not a query definition (ie a folder) so that there is no selected query.
            this.SelectedQuery = e.Node.Tag as QueryDefinition;
        }

        /// <summary>
        /// Handles the <see cref="UserControl.VisibleChanged"/> event to set the focus on the tree view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleVisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.treeView.Focus();
            }
        }
    }
}

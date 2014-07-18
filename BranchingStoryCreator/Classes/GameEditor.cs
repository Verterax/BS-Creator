using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranchingStoryCreator
{
    public class GameEditor
    {

        #region Variables
        public StoryData storyData { get; set; }

        private StoryTree tree;
        private string resourcePath;

        #endregion

        #region Consts

        public const int MAX_NODES = 5;

        #endregion

        #region Load / Init

        public GameEditor(StoryTree tree, StoryData storyData, string resourcePath)
        {
            Init(tree, storyData, resourcePath);
        }

        private void Init(StoryTree tree, StoryData storyData, string resourcePath)
        {
            this.tree = tree;
            this.resourcePath = resourcePath;
            this.storyData = storyData;
        }

        #endregion

        #region Exposed Events

        /// <summary>
        /// Extended from the StoryTree class.
        /// </summary>
        public event ContextChangedEventHandler Before_SelectionChange
        {
            add { tree.Before_SelectionChanged += value; }
            remove { tree.Before_SelectionChanged -= value; }
        }
        /// <summary>
        /// Extended from the StoryTree class.
        /// </summary>
        public event ContextChangedEventHandler After_SelectionChange
        {
            add { tree.After_SelectionChanged += value; }
            remove { tree.After_SelectionChanged -= value; }
        }

        public event EventHandler NodeCount_Changed;

        #endregion

        #region Controller

        /// <summary>
        /// Adds an empty node to the parent node if possible.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>The node added.</returns>
        public DataNode AddNewNode(DataNode parent)
        {
            if (parent == null)
                return null;

            DataNode newNode = null;

            if (parent.Nodes.Count < MAX_NODES)
            {
                newNode = StoryTree.GetNewNode(tree);
                parent.Nodes.Add(newNode);
            }

            if (newNode != null)
                if (NodeCount_Changed != null)
                    NodeCount_Changed(this, EventArgs.Empty);

            return newNode;
        }

        public bool RemoveNode(DataNode parent, DataNode target)
        {
            if (parent == null ||
                target == null) 
                return false;

            bool success = StoryTree.RemoveNode(parent, target);

            if (success)
                if (NodeCount_Changed != null)
                    NodeCount_Changed(this, EventArgs.Empty);

            return success;
        }

        //Tries to select the DataNode passed in.
        public DataNode SelectNode(DataNode target)
        {
            DataNode node = tree.SelectedNode = target;
            return node;
        }
 
        #endregion

        #region View

        public DataNode GetSelectedNode()
        {
            return tree.SelectedNode;
        }

        public DataNode GetParentOfSelectedNode()
        {
            return tree.ParentOfSelected;
        }

        public void SyncTreeView(ref TreeView treeView)
        {
            StoryTree.SetTreeViewData(tree, ref treeView);
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the storyTree to xmlPath.
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns>Returns "" if no errors, or error message.</returns>
        public string SaveProject(string xmlPath = "")
        {
           // try
           // {
            if (xmlPath == "")
                xmlPath = GetDefaultSavePath();

                StoryProject project = new StoryProject(tree, storyData);
                StoryProject.Serialize(project, xmlPath);
           // }
           // catch (Exception ex)
           // {
          //      throw ex; //Throw Err for now.
               //return ex.Message;
          //  }

            return ""; //No Errors.
        }

        public string GetDefaultSavePath()
        {
            return Path.Combine(resourcePath,storyData.projectName + StoryProject.PROJECT_EXT);
        }

        #endregion

        #region Misc

        public static string[] GetSupportedImageExt()
        {
            string[] imgFormats = new string[]{
                ".bmp",
                ".jpg",
                ".gif",
                ".png"
            };

            return imgFormats;
        }

        #endregion

    }
}

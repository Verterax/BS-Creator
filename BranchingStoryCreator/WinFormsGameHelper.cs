using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BranchingStoryCreator.Web;

namespace BranchingStoryCreator.Forms
{
    public static class FormHelper
    {

        #region Winforms Tree Heleprs

        public static TreeNode GetNode(TreeView tree, string nodeID)
        {
            if (tree == null)
                return null;

            TreeNode foundNode = null;

            foreach (TreeNode node in tree.Nodes)
            {
                foundNode = GetNode(node, nodeID);

                if (foundNode != null)
                    break;
            }

            return foundNode;
        }

        public static TreeNode GetNode(TreeNode root, string nodeID)
        {
            if (root == null)
                return null;

            DataNode data = root.Tag as DataNode;
            TreeNode foundNode = null;

            if (data == null)
                return null;

            if (data.ID == nodeID)
                return root;

            foreach (TreeNode child in root.Nodes)
            {
                foundNode = GetNode(child, nodeID);

                if (foundNode != null)
                    break;
            }

            return foundNode;
        }


        #region Presentation

        public static TreeView GetNewTreeView(StoryTree tree)
        {
            if (tree == null)
                return null;

            TreeView treeView = new TreeView();
            treeView.TopNode = GetNewTreeNode(tree);

            return treeView;
        }

        /// <summary>
        /// Applies the structure of a StoryTree to a TreeView
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="treeView"></param>
        public static void SetTreeViewData(StoryTree tree, ref TreeView treeView)
        {
            TreeNode root = GetNewTreeNode(tree);
            treeView.Nodes.Clear();
            treeView.Nodes.Add(root);
            treeView.TopNode = root;
            treeView.Update();
        }

        /// <summary>
        /// Recursively constructs a TreeNode and it's children from a DataNode.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static TreeNode GetNewTreeNode(DataNode root)
        {
            TreeNode treeNode = new TreeNode();
            SetNodeText(ref treeNode, root, false);
            treeNode.Tag = root;

            foreach (DataNode child in root.Nodes)
                treeNode.Nodes.Add(GetNewTreeNode(child));

            return treeNode;

        }

        /// <summary>
        /// Applies the standard text formatting for a TreeNode from a DataNode.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isSelected"></param>
        public static void SetNodeText(ref TreeNode node, DataNode data, bool isSelected)
        {
            if (node != null)
            {
                if (isSelected)
                    node.Text = string.Format("█████ ( {0} ):  {1} █████", data.ID, data.ButtonText);
                else
                    node.Text = string.Format("( {0} ):  {1}", data.ID, data.ButtonText);
            }
            else
                throw new Exception("Cannot set node text. Node Data is null.");
        }


        #endregion

        #endregion



    }
}

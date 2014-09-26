using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Windows.Forms;
//using System.Drawing;
using System.Xml.Serialization;
using System.Xml;

namespace BranchingStoryCreator.Web
{        

        [Serializable]
        public class DataNode
        {
            public string ID { get; set; }
            public string ImgPath { get; set; }
            public string ButtonText { get; set; }
            public string Story { get; set; }
            public string AvailIf { get; set; }
            public string Script { get; set; }
            public List<DataNode> Nodes { get; set; }

            public DataNode() { Init(); }

            private void Init()
            {
                ID = "";
                ImgPath = "";
                ButtonText = "";
                Story = "";
                AvailIf = "";
                Script = "";
                Nodes = new List<DataNode>();
            }

            public static DataNode CopyNodeData(DataNode dataToCopy)
            {
                if (dataToCopy == null)
                    return null;

                DataNode copiedData = new DataNode();
                copiedData.ID = dataToCopy.ID;
                copiedData.ImgPath = dataToCopy.ImgPath;
                copiedData.ButtonText = dataToCopy.ButtonText;
                copiedData.Story = dataToCopy.Story;
                copiedData.AvailIf = dataToCopy.AvailIf;
                copiedData.Script = dataToCopy.Script;

                return copiedData;
            }
        }

        [Serializable]
        public class StoryTree : DataNode
        {

            #region Serializable Variables
            
            #endregion

            #region Non Serializable Variables

            /// <summary>
            /// Provides ability to select any node on the tree.
            /// </summary>
            //[XmlIgnore]
            //public string pos
            //{
            //    get { return SelectedNode.ID; }
            //    set
            //    {
            //        DataNode MoveTo = StoryTree.GetNode(this, value);

            //        if (MoveTo == null)
            //            throw new Exception(string.Format("Unable to select node ID: {0}", value));

            //        SelectedNode = MoveTo;

            //        if (GamePosition_Changing != null) //Pass the targetted node as the Event parameter.
            //            GamePosition_Changing(this, new ContextChangeEventArgs(MoveTo));                    
            //    }
            //}

            //[XmlIgnore]
            //public DataNode SelectedNode
            //{            
            //    get { return _selectedNode; }
            //    set
            //    {  //Only fire event when actual change occurs.
            //        if (_selectedNode != value)
            //        {
            //            if (Before_SelectionChanged != null)
            //                Before_SelectionChanged(this, new ContextChangeEventArgs(_selectedNode));

            //            _selectedNode = value;
            //            _parentOfSelected = StoryTree.GetParentOfSelected(this, value);

            //            if (After_SelectionChanged != null)
            //                After_SelectionChanged(this, new ContextChangeEventArgs(_selectedNode));

            //        }
            //    }
            //}
            //private DataNode _selectedNode;

            //[XmlIgnore]
            //public DataNode ParentOfSelected { 
            //    get { return _parentOfSelected; } 
            //    private set { _parentOfSelected = value; } }         
            //private DataNode _parentOfSelected;

            #endregion

            #region Exposed Events

            //public event ContextChangedEventHandler Before_SelectionChanged;
            //public event ContextChangedEventHandler After_SelectionChanged;
            //public event ContextChangedEventHandler GamePosition_Changing;

            #endregion

            #region Consts
            #endregion

            #region Load / Init

            public StoryTree() 
            { 
                Init(); 
            }
            private void Init() 
            { 
            }
          
            #endregion

            #region New Tree
            public static StoryTree GetNewStoryTree()
            {
                StoryTree tree = new StoryTree();
                tree.ID = "0"; //Root node.

                return tree;
            }
            #endregion

            #region New Node
            public static DataNode GetNewNode(StoryTree tree)
            {
                if (tree == null)
                    return null;

                DataNode node = new DataNode();
                node.ID = GetNewNodeID(tree);
                node.ImgPath = "";
                node.ButtonText = "";
                node.Story = "";
                node.AvailIf = "";
                node.Script = "";

                return node;
            }

            public static DataNode GetNewNode(StoryTree tree, DataNode withValues)
            {
                if (tree == null)
                    return null;

                DataNode node = new DataNode();
                node.ID = GetNewNodeID(tree);
                node.ImgPath = withValues.ImgPath;
                node.ButtonText = withValues.ButtonText;
                node.Story = withValues.Story;
                node.AvailIf = withValues.AvailIf;
                node.Script = withValues.Script;

                return node;
            }

            public static string GetNewNodeID(StoryTree tree)
            {

                int newNodeID = CountNodes(tree) + 1;
                string returnID = newNodeID.ToString();

                while (ContainsID(tree, returnID))
                {
                    newNodeID++;
                    returnID = newNodeID.ToString();
                }

                return returnID;
            }

            #endregion

            #region Node Removal

            /// <summary>
            /// Recursively removes a node from the tree.
            /// </summary>
            /// <param name="tree"></param>
            /// <param name="removeNode"></param>
            /// <returns></returns>
            public static bool RemoveNode(StoryTree tree, DataNode removeNode)
            {
                bool removed = false;

                if (tree == null ||
                    removeNode == null)
                    return false;

                foreach (DataNode node in tree.Nodes)
                {
                    if (node == removeNode)
                        return tree.Nodes.Remove(node);          

                    removed = RemoveNode(node, removeNode);

                    if (removed)
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Recursively removes a node from the node's children.
            /// </summary>
            /// <param name="root"></param>
            /// <param name="removeNode"></param>
            /// <returns></returns>
            public static bool RemoveNode(DataNode root, DataNode removeNode)
            {
                bool removed = false;

                if (root == null ||
                    removeNode == null)
                    return false;

                foreach (DataNode node in root.Nodes)
                {
                    if (node == removeNode)
                        return root.Nodes.Remove(node);  

                    removed = RemoveNode(node, removeNode);

                    if (removed)
                        return true;
                }

                return false;
            }

            #endregion

            #region Tree Traversal

            #region Story Tree

            /// <summary>
            /// Returns node if selection successful, null if not.
            /// </summary>
            /// <param name="tree"></param>
            /// <param name="nodeID"></param>
            /// <returns></returns>
            public static DataNode GetNode(StoryTree tree, string nodeID)
            {
                if (tree.ID == nodeID)
                    return tree;

                foreach (DataNode node in tree.Nodes)
                {
                    DataNode foundNode = GetNode(node, nodeID);

                    if (foundNode != null)
                        return foundNode;
                }

                 return null; //Node not found.
            }
            /// <summary>
            /// Returns true if found.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="gotoData"></param>
            /// <returns></returns>
            public static DataNode GetNode(DataNode parent, string nodeID)
            {
                if (parent != null)
                    if (parent.ID == nodeID)
                        return parent; //This is the node.

                //Keep searching down.
                foreach (DataNode node in parent.Nodes)
                {
                    DataNode found = GetNode(node, nodeID);

                    if (found != null)
                        return found; //Node found.
                }

                return null; //Node not found.
            }

            public static DataNode GetNode(StoryTree tree, EditRequest request)
            {
                return StoryTree.GetNode(tree, request.targetNode.ID);
            }

            /// <summary>
            /// Returns the parent DataNode of the selected DataNode
            /// </summary>
            /// <param name="tree"></param>
            /// <param name="selected"></param>
            /// <returns>The parent DataNode, or null</returns>
            public static DataNode GetParentOfSelected(DataNode root, DataNode selected)
            {
                if (root == null ||
                    selected == null)
                    return null;

                if (root == selected) //The root node of the tree is selected, it has no parent.
                    return null;

                DataNode found = null;

                foreach (DataNode child in root.Nodes)
                {
                    if (child == selected)
                        return root; //Parent found.

                    if (child.Nodes.Count > 0) //Search children's children.
                        found = GetParentOfSelected(child, selected);

                    if (found != null) //If parent found; return.
                        return found;
                }

                    return null; //Parent not found.
            }

            public static DataNode GetParentOfSelected( DataNode root, EditRequest request)
            {
                return GetParentOfSelected(root, request.targetNode);
            }

            #endregion

            #endregion

            #region Misc

            /// <summary>
            /// Recursively counts the number of NodeData children in the NodeData object.
            /// </summary>
            /// <param name="root"></param>
            /// <returns></returns>
            public static int CountNodes(DataNode root)
            {
                int count = root.Nodes.Count; //+1 for root node.

                foreach (DataNode node in root.Nodes)
                    count += CountNodes(node);

                return count;
            }

            /// <summary>
            /// Recursively checks the NodeData for a child object matching the passed ID string.
            /// </summary>
            /// <param name="root"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            public static bool ContainsID(DataNode root, string id)
            {
                bool contains = false;

                if (root.ID == id)
                    return true;

                foreach (DataNode node in root.Nodes)
                {
                    contains = ContainsID(node, id);

                    if (contains)
                        return true;
                }

                return false;
            }


            #endregion

            #region Clone Tree / Node

            public static StoryTree CloneTree(StoryTree treeToClone)
            {
                StoryTree newTree = new StoryTree();

                if (treeToClone.Nodes.Count < 1)
                    return newTree;
                else
                {
                    foreach (DataNode node in treeToClone.Nodes)
                    {
                        DataNode clonedNode = CloneNode(node);
                        newTree.Nodes.Add(clonedNode);
                    }
                }

                return newTree;
            }

            public static DataNode CloneNode(DataNode nodeToClone)
            {
                if (nodeToClone == null)
                    return null;

                DataNode clonedNode = DataNode.CopyNodeData(nodeToClone);

                foreach (DataNode childNode in nodeToClone.Nodes)
                {
                    DataNode clonedChild = CloneNode(childNode);
                    clonedNode.Nodes.Add(clonedChild);
                }

                return clonedNode;
            }

            #endregion

            #region Serializing

            public static bool SerializeTree(StoryTree storyTree, string saveTo)
            {
                if (storyTree == null)
                    return false;

                Type treeType = typeof(StoryTree);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.NewLineHandling = NewLineHandling.Entitize;

                XmlSerializer serializer = new XmlSerializer(treeType);
                using (XmlWriter writer = XmlWriter.Create(saveTo, settings))
                {
                    serializer.Serialize(writer, storyTree);
                }

                return true;
            }

            public static StoryTree DeserializeTree(string xmlPath)
            {
                if (!File.Exists(xmlPath))
                    return null;

                XmlSerializer deserializer = new XmlSerializer(typeof(StoryTree));
                StoryTree storyTree = null;

                using (FileStream stream = new FileStream(xmlPath, FileMode.Open))
                {
                    object obj = deserializer.Deserialize(stream);
                    storyTree = obj as StoryTree;
                }

                return storyTree;
            }

            #endregion
           
        }

        public class ContextChangeEventArgs : EventArgs
        {
            public DataNode DataNode;

            public ContextChangeEventArgs (DataNode node)
            {
                this.DataNode = node;
            }
        }

        public delegate void ContextChangedEventHandler(object sender, ContextChangeEventArgs e);
    
}

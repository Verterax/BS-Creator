using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MSScriptControl;

namespace BranchingStoryCreator
{ 
    public partial class Main : Form
    {

        #region Variables

        public Button[] Buttons { get; set; }
        //public TreeView MainTree { get; set; }

        public GameViewer viewerWindow { get; set; }
        public GameObject GameObj { get; set; }

        //Behind the scenes
        private int countDown; //For timer.
        private string tempStory; //For story value-key replace

        #endregion

        #region Constructors / Load / Init
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Init();
            AutoGoto();           
        }

        private void AutoGoto()
        {
            //string loadPath = @"C:\Users\Christopher\Documents\Visual Studio 2013\Projects\BranchingStoryCreator\BranchingStoryCreator\bin\Debug\squirrel.xml";
            //LoadTree(loadPath);
            //runToolStripMenuItem.PerformClick();
            //SendToBack();
        }

        public void Init()
        {
            AddKeyPressEvents();
            LoadProperties();

            //Instantiate Variables
            Buttons = new Button[5];

            //Add Button References to Array
            Buttons[0] = btn0;
            Buttons[1] = btn1;
            Buttons[2] = btn2;
            Buttons[3] = btn3;
            Buttons[4] = btn4;

            //Initialize Undo Redo
            UpdateUndoRedoMenu();

            ToggleInputFields(false);

            foreach (Button btn in Buttons)
                btn.Visible = false;

            countDown = 0;
            tsMessage.Text = "";
            timer.Start();

            //Auto load project?
            if (Properties.Settings.Default.autoLoadLastProject)
            {
                string lastProjectPath = Properties.Settings.Default.lastProjectPath;
                if (File.Exists(lastProjectPath))
                    LoadTree(lastProjectPath);
            }
                
        }

        #endregion

        #region User Interface

        #region Buttons

        private void imgBg_Click(object sender, EventArgs e)
        {
            if (txtImgPath.Enabled)
                ShowPictureDialog();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            TreeNode selected = treStory.SelectedNode;
            TreeNode parent = null;

            if (selected != null)
                parent = selected.Parent;

            if (parent != null)
            {
                treStory.SelectedNode = parent;
                UpdateViewerWindow();                
            }
        }

        private void btnTranspose_Click(object sender, EventArgs e)
        {
            tempStory = txtStory.Text;
            txtStory.Text = GameObj.dic.ReplaceKeysWithValues(txtStory.Text);
        }

        #endregion

        #region Menu Items

        #region File

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CreateNewProject();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = PromtToSave();

            if (result == DialogResult.Yes)
                SaveProject();

            if (result != DialogResult.Cancel)
                ShowLoadDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
            //ShowSaveDialog();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
                CloseCurrentProject();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = PromtToSave();

            if (result != DialogResult.Cancel)
                Application.Exit();
        }

        #endregion

        #region Edit

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        #endregion

        #region Open Project Folders

        private void bGImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectFolder = GameObj.GetBGImagePath();
            try
            {
                System.Diagnostics.Process.Start(projectFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectFolder = GameObj.GetItemImgPath();
            try
            {
                System.Diagnostics.Process.Start(projectFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void soundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectFolder = GameObj.GetSoundPath();
            try
            {
                System.Diagnostics.Process.Start(projectFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Preferences

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences pref = new Preferences();
        }

        #endregion

        #region Run

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameObj == null)
            {
                MessageBox.Show("Cannot run an empty game.");
                return;
            }


            if (viewerWindow != null)
                viewerWindow.Close();

            GameObj.StartFromBeginning();
            UpdateGameItemsListView();
            UpdateGameObjListView();
            
            viewerWindow = new GameViewer(GameObj);
            viewerWindow.Show();
        }


        #endregion

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataNode data = treStory.SelectedNode.Tag as DataNode;
            SetDataNodeValues(ref data);
            ShowMinorMessage("Changes updated.", 3000);
        }

        #endregion

        #region Context Menu

        /// <summary>
        /// Add Child Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildNode();
        }

        /// <summary>
        /// Add Sibling Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addSiblingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingNode();
        }

        /// <summary>
        /// Remove Selected Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        #region View

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treStory.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treStory.CollapseAll();
        }

        #endregion


        #endregion

        #endregion

        #region Events

        private void AddEvents()
        {
            GameObj.editor.NodeCount_Changed += UpdateTreeView;

            GameObj.editor.Before_SelectionChange += StoryTree_BeforeSelect;
            GameObj.editor.After_SelectionChange += StoryTree_AfterSelect;
          
            treStory.BeforeSelect += EditorTree_BeforeSelect;
            treStory.AfterSelect += EditorTree_AfterSelect;

            //Add wire-up nav button click events.
            foreach (Button btn in Buttons)
                btn.Click += btnNavButton_Clicked;
        }

        private void RemoveEvents()
        {
            GameObj.editor.Before_SelectionChange -= StoryTree_BeforeSelect;
            GameObj.editor.After_SelectionChange -= StoryTree_AfterSelect;

            treStory.BeforeSelect -= EditorTree_BeforeSelect;
            treStory.AfterSelect -= EditorTree_AfterSelect;

            foreach (Button btn in Buttons)
                btn.Click -= btnNavButton_Clicked;
        }


        #region Keyboard

        private void AddKeyPressEvents()
        {
            foreach (Control ctrl in this.Controls)
                ctrl.KeyDown += Key_Down;
        }

        /// <summary>
        /// The function called whenever there is a keypress in the main form.
        /// </summary>
        private void Key_Down(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                SaveProject();
        }

        #endregion

        #region Image

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (GameObj != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.All;
                else
                    e.Effect = DragDropEffects.None;
            }
            else
                e.Effect = DragDropEffects.None;

        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {

            if (GameObj == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] validImgExts = GameEditor.GetSupportedImageExt();
                string[] filePath = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string file in filePath)
                {
                    foreach (string ext in validImgExts)
                        if (Path.GetExtension(file) == ext)
                        {
                            string fileName = Path.GetFileName(file).Replace(' ', '_');
                            string imgCopyTo = GameObj.GetBGImagePath() + fileName;
                            try
                            {
                                //Copy if does not exist in resource folder.
                                if (!File.Exists(imgCopyTo))
                                    File.Copy(file, imgCopyTo);
                                else
                                {
                                    if (txtImgPath.Text != "")
                                    {
                                        DialogResult result = MessageBox.Show("Overwrite old file?", "Image exists.", MessageBoxButtons.YesNo);

                                        if (result == DialogResult.Yes)
                                            File.Copy(file, imgCopyTo, true);
                                        else
                                        {
                                            result = MessageBox.Show("Is this a new image?", "Generate unique filename?", MessageBoxButtons.YesNo);

                                            if (result == DialogResult.Yes)
                                            {
                                                string fExt = Path.GetExtension(imgCopyTo);
                                                string fName = Path.GetFileNameWithoutExtension(imgCopyTo) + GetRandChars(4) + fExt;
                                                string fDir = Path.GetDirectoryName(imgCopyTo);

                                                imgCopyTo = Path.Combine(fDir, fName + fExt);
                                                fileName = fName;
                                                File.Copy(file, imgCopyTo, true);
                                            }
                                        }
                                    }
                                }


                                txtImgPath.Text = fileName;
                                LoadImg(fileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    "Unable to copy " + Environment.NewLine +
                                    file + Environment.NewLine +
                                    " to " + Environment.NewLine +
                                    imgCopyTo + Environment.NewLine + Environment.NewLine +
                                    ex.Message);
                            }
                        }
                }
                e.Effect = DragDropEffects.None;
            }
        }


        #endregion

        #region Tree View


        private void StoryTree_BeforeSelect(object sender, ContextChangeEventArgs e)
        {
            //Save current values.
            DataNode data = e.DataNode;

            if (data != null)
            {
                SetDataNodeValues(ref data);
                TreeNode lastNode = StoryTree.GetNode(treStory, data.ID);

                if (lastNode != null)
                    StoryTree.SetNodeText(ref lastNode, data, false);
            }

            //Console.WriteLine("Story Before");
        }

        private void StoryTree_AfterSelect(object sender, ContextChangeEventArgs e)
        {
            //Bind current values.
            DataNode data = e.DataNode;
            TreeNode selected = null;

            if (data != null)
            {
                selected = FindInTreeView(treStory, data);
                treStory.SelectedNode = selected;
                BindDataNodeValues(data);
                ToggleInputFields(true);
                UpdateGameItemsListView();
                UpdateGameObjListView();
                UpdateButtonView();
            }

            if (data != null &&
                selected != null)
            {
                StoryTree.SetNodeText(ref selected, data, true);
            }

            //Console.WriteLine("Story After");
        }


        private void EditorTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            if (node != null)
            {
                DataNode data = node.Tag as DataNode;
            }

            //Console.WriteLine("Editor Before");
        }

        private void EditorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Ask StoryTree to select this node.
            TreeNode node = e.Node;

            if (node != null)
            {
                DataNode data = node.Tag as DataNode;
                GameObj.editor.SelectNode(data);

                if (treStory.DisplayRectangle.Contains(treStory.PointToClient(Cursor.Position)))
                    UpdateViewerWindow();
            }

            
        }



        #endregion

        #region Timer

        private void timer_Tick(object sender, EventArgs e)
        {
            if (countDown > 0)
            {
                countDown -= timer.Interval;

                if (countDown < 1)
                {
                    countDown = 0;
                    tsMessage.Text = "";
                }
            }
        }

        #endregion

        #region Buttons

        private void btnNavButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;
                GameButton data = btn.Tag as GameButton;
                string nodeID = data.NodeID;
                bool success = false;

                //Restart at start.
                if (nodeID == "0")
                {
                    treStory.SelectedNode = treStory.TopNode;
                    return;
                }

                success = GameObj.ButtonPress(nodeID, true); //Did the selection change?

                if (!success)
                    MessageBox.Show("Unable to proceed to Node ID " + nodeID);
                else
                {
                    UpdateGameObjListView();
                    UpdateGameItemsListView();
                }
                    
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            ExecuteScript();
        }

        private void btnTranspose_MouseDown(object sender, MouseEventArgs e)
        {
            tempStory = txtStory.Text;
            txtStory.Text = GameObj.dic.ReplaceKeysWithValues(txtStory.Text);
        }

        private void btnTranspose_MouseUp(object sender, MouseEventArgs e)
        {
            if (tempStory != "")
            {
                txtStory.Text = tempStory;
                tempStory = "";
            }
        }

        #endregion


        private void txtPreText_TextChanged(object sender, EventArgs e)
        {
            TreeNode node = treStory.SelectedNode;

            if (node != null)
            {
                DataNode data = (DataNode)node.Tag;           

                if (data != null)
                {
                    data.ButtonText = txtButtonText.Text;
                    StoryTree.SetNodeText(ref node, data, true);
                }
            }
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            TreeNode node = treStory.SelectedNode;

            if (node != null)
            {
                DataNode data = (DataNode)node.Tag;

                if (data != null)
                {
                    data.ID = txtID.Text;
                    StoryTree.SetNodeText(ref node, data, true);
                }
            }
        }

        private void chkRun_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.runScript = chkDisableScript.Checked;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Undo / Redo

        private bool Undo()
        {
            //if (UndoStack.Count > 0)
            //{
            //    SetRedoPoint();
            //    treStory.Nodes.Clear();

            //    TreeView popTree = UndoStack.Pop();

            //    treStory.Nodes.Clear();
            //    StoryTreeManager.TransferNodes(popTree, ref treStory);
            //    return true;
            //}
            //else
                return false;

        }

        private bool Redo()
        {
            //if (RedoStack.Count > 0)
            //{
            //    SetUndoPoint();
            //    treStory.Nodes.Clear();

            //    TreeView popTree = RedoStack.Pop();

            //    treStory.Nodes.Clear();
            //    StoryTreeManager.TransferNodes(popTree, ref treStory);
            //    return true;
            //}
            //else
                return false;
        }

        private void SetUndoPoint()
        {
            //TreeView saveState = StoryTreeManager.CloneTree(treStory);
            //UndoStack.Push(saveState);

            //UpdateUndoRedoMenu();
        }

        private void SetRedoPoint()
        {
            //TreeView saveState = StoryTreeManager.CloneTree(treStory);
            //RedoStack.Push(saveState);

            //UpdateUndoRedoMenu();
        }

        private void UpdateUndoRedoMenu()
        {
            //if (UndoStack.Count > 0)
            //    undoToolStripMenuItem.Enabled = true;
            //else
            //    undoToolStripMenuItem.Enabled = false;

            //if (RedoStack.Count > 0)
            //    redoToolStripMenuItem.Enabled = true;
            //else
            //    redoToolStripMenuItem.Enabled = false;

        }

        #endregion

        #region Editing

        #region Add / Remove Nodes

        public void UpdateTreeView(object sender, EventArgs e)
        {
            GameObj.editor.SyncTreeView(ref treStory);
        }

        private bool AddChildNode()
        {
            DataNode selected = treStory.SelectedNode.Tag as DataNode;
            DataNode newDataNode = GameObj.editor.AddNewNode(selected);     

            if (newDataNode == null)
            {
                MessageBox.Show("Unable to add more nodes.");
                return false;
            }
            else
            {
                //Select the newly added node.
                TreeNode treeNode = StoryTree.GetNode(treStory, newDataNode.ID);
                treStory.SelectedNode = treeNode;            
                return true;
            }
        }

        /// <summary>
        /// Adds a node to the selected node's parent.
        /// </summary>
        /// <returns></returns>
        private bool AddSiblingNode()
        {
            DataNode parent = GameObj.editor.GetParentOfSelectedNode();
            DataNode newDataNode = GameObj.editor.AddNewNode(parent);  //Add child to parent (sibling)

            if (newDataNode == null)
            {
                MessageBox.Show("Unable to add more nodes.");
                return false;
            }
            else
            {
                //Select the newly added node.
                TreeNode treeNode = StoryTree.GetNode(treStory, newDataNode.ID);
                treStory.SelectedNode = treeNode;
                return true;
            }
        }

        private bool RemoveSelectedNode()
        {
            DataNode selected = GameObj.editor.GetSelectedNode();
            DataNode parent = GameObj.editor.GetParentOfSelectedNode();

            if (selected == null ||
                parent == null)
                return false;

            bool success = GameObj.editor.RemoveNode(parent, selected);

            if (success)
            {
                TreeNode parentTreeNode = StoryTree.GetNode(treStory, parent.ID);
                treStory.SelectedNode = parentTreeNode;
            }
            else
                MessageBox.Show("Unable to remove node.");

            return success;
        }

        #endregion

        #region Save Node Values
        private void SetDataNodeValues(ref DataNode data)
        {
            if (data == null)
                throw new Exception("Unable to update Data Node.");

            data.ID = txtID.Text;
            data.ImgPath = txtImgPath.Text;
            data.ButtonText = txtButtonText.Text;
            data.Story = txtStory.Text;
            data.Script = txtScript.Text;
            data.AvailIf = txtAvailIf.Text;
        }

        /// <summary>
        /// Updates the selected data node with the values in the editor.
        /// </summary>
        private void SetSelectedDataNode()
        {
            if (treStory.SelectedNode == null)
                return;

            DataNode selectedData = GetSelectedDataNode();

            if (selectedData == null)
                return;

            SetDataNodeValues(ref selectedData);

        }

        private DataNode GetSelectedDataNode()
        {
            if (treStory.SelectedNode == null)
                return null;

            return treStory.SelectedNode.Tag as DataNode;
        }
        #endregion

        #region Scripting

        private void ExecuteScript()
        {
            try
            {
                GameObj.script.ExecuteStatementShorthand(txtScript.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(GameObj.script.GetError());
            }

            UpdateGameObjListView();
            UpdateGameItemsListView();

        }



        #endregion

        #endregion

        #region New / Save / Load / Close

        private void SaveProject()
        {

            if (GameObj == null)
            {
                MessageBox.Show("No project is open to save.");
                return;
            }


            //try
            //{
                SetSelectedDataNode();
                GameObj.editor.SaveProject();
                ShowMinorMessage("Project saved!", 5000);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Unable to save project. " + ex.Message);
            //}
        }

        private void CloseCurrentProject()
        {
            if (GameObj != null)
            {
                PromtToSave();
                RemoveEvents();
            }

            if (viewerWindow != null)
            {
                viewerWindow.Close();
                viewerWindow.Dispose();
                viewerWindow = null;
            }
          
            ToggleInputFields(false);
            treStory.Nodes.Clear();
            GameObj = null;
            
        }

        private void CreateNewProject()
        {
            CloseCurrentProject();

            string projectDirectory = GameObject.GetWindowsProjectFolder();
            string projectName = GetProjectName(projectDirectory);

            //Abort creation.
            if (projectName == "")
                return;

            GameObj = GameObject.CreateNewProject(projectName, projectDirectory);

            if (GameObj != null)
            {              
                GameObj.editor.SyncTreeView(ref treStory);
                ProjectInitialization(GameObj.ProjectTreePath());
                GameObj.editor.SaveProject(); //Save for first time to create xml file.
            }
            else
                MessageBox.Show("Unable to load Game Object.");
        }

        private static string GetProjectName(string projectsDir)
        {
            if (!Directory.Exists(projectsDir))
                return "";
          
            string prompt = "Please enter a name for the new game project. The name must be alpha-numeric, and shorter than 25 characters. Space will be converted to underscores.";
            string title = "Give the new project a name.";
            InputBox box = new InputBox(title, prompt);

            string projName = "";
            bool projectExists = true;
            DialogResult result = DialogResult.OK;

            while (projName == "" || projectExists)
            {
                result = box.ShowDialog();

                if (result == DialogResult.Cancel)
                    return "";

                projName = GameObject.GetProjectValidName(box.textResponse);

                if (projName != "")
                {
                    projectExists = Directory.Exists(projectsDir + "\\" + projName + "\\");

                    if (projectExists)
                        MessageBox.Show("The project folder for {0} already exists. Please choose a different name.", projName);
                }
           
            }

            return projName;
        }

        private void LoadTree(string filePath)
        {
            CloseCurrentProject();

            if (File.Exists(filePath))
            {
                GameObj = new GameObject(filePath);
                ShowMinorMessage("Story Loaded!", 5000);
                GameObj.editor.SyncTreeView(ref treStory);
                ProjectInitialization(filePath);
            }
            else
                ShowMinorMessage("File does not exist: " + filePath, 3000);

        }

        /// <summary>
        /// Call first.
        /// </summary>
        private void ProjectInitialization(string projectTreePath)
        {
            AddEvents();
            treStory.SelectedNode = treStory.TopNode;
            Properties.Settings.Default.lastProjectPath = projectTreePath;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Dialog Boxes

        private DialogResult PromtToSave()
        {
            DialogResult result = DialogResult.No;

            if (treStory.SelectedNode == null)
                return result;

            DataNode root = treStory.SelectedNode.Tag as DataNode;

            if (root == null)
                return result;

            if (StoryTree.CountNodes(root) > 1)
                result = YesOrNoQuestion("Would you like to save?", "Save or Die.");

            return result;
        }

        private DialogResult ShowLoadDialog()
        {
            OpenFileDialog loadDlg = new OpenFileDialog();

            loadDlg.DefaultExt = ".xml";
            loadDlg.InitialDirectory = GetProjectsDir();
            loadDlg.Title = "Load Branching Story";

            DialogResult result = loadDlg.ShowDialog();

            if (result != DialogResult.Cancel)
                LoadTree(loadDlg.FileName);

            return DialogResult;
        }


        private DialogResult ShowPictureDialog()
        {

            string imgFolder = GameObj.GetBGImagePath();
            string initialPath = imgFolder + txtImgPath.Text;

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Select Image File";
            dlg.Filter = "Image Files (*.bmp;*.jpg;*.gif;*.png)|*.bmp;*.jpg;*.gif;*.png|All files (*.*)|*.*";
            dlg.InitialDirectory = imgFolder;

            if (File.Exists(initialPath))
                dlg.FileName = initialPath;

            DialogResult result = dlg.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                string imgPath = dlg.FileName;
                string fileName = Path.GetFileName(imgPath);
                string finalPath = imgFolder + fileName;

                string pathA = Path.GetDirectoryName(imgPath);
                string pathB = Path.GetDirectoryName(imgFolder);

                if (pathA.ToLower() != pathB.ToLower())
                    File.Copy(imgPath, finalPath, true);

                Bitmap img = new Bitmap(finalPath);
                imgBg.Image = img;
                txtImgPath.Text = fileName;

                //Update node for new picture.
                DataNode data = treStory.SelectedNode.Tag as DataNode;

                SetDataNodeValues(ref data);
            }


            return result;
        }


        private DialogResult YesOrNoQuestion(string longQuestion, string shortQuestion)
        {
            return MessageBox.Show(longQuestion, shortQuestion, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        #endregion

        #region View

        private void ToggleInputFields(bool turnOn)
        {
            txtID.Enabled = turnOn;
            txtImgPath.Enabled = turnOn;
            txtButtonText.Enabled = turnOn;
            txtScript.Enabled = turnOn;
            txtStory.Enabled = turnOn;
            txtAvailIf.Enabled = turnOn;

            if (!turnOn)
            {
                txtID.Text = "";
                txtImgPath.Text = "";
                txtButtonText.Text = "";
                txtScript.Text = "";
                txtStory.Text = "";
                txtAvailIf.Text = "";

                lstGameObj.Items.Clear();
                lstItems.Items.Clear();
            }
        }

        private void UpdateGameObjListView()
        {
            List<string> listItems = GameObj.GetDicValues();
            lstGameObj.Items.Clear();

            ListView newList = new ListView();

            foreach (string item in listItems)
                lstGameObj.Items.Add(item);
        }

        private void UpdateGameItemsListView()
        {

            List<string> listItems = GameObj.GetBagValues();
            lstItems.Items.Clear();

            ListView newList = new ListView();

            foreach (string item in listItems)
                lstItems.Items.Add(item);
        }

        private void UpdateViewerWindow()
        {
            if (viewerWindow != null &&
                    viewerWindow.IsDisposed == false)
            {
                viewerWindow.ForceUpdate();
            }
        }

        private void ShowMinorMessage(string message, int milliseconds)
        {
            countDown = milliseconds;
            tsMessage.Text = message;
        }

        private void BindDataNodeValues(DataNode data)
        {
            if (data != null)
            {
                txtID.Text = data.ID;
                txtImgPath.Text = data.ImgPath;
                txtButtonText.Text = data.ButtonText;
                txtStory.Text = data.Story;
                txtScript.Text = data.Script;
                txtAvailIf.Text = data.AvailIf;

                LoadImg(data.ImgPath);
                return;
            }
        }

        private static TreeNode FindInTreeView(TreeView tree, DataNode data)
        {
            if (tree.SelectedNode.Tag == data)
                return tree.SelectedNode;

            if (data == null)
                return null;

            TreeNode found = null;
            foreach (TreeNode node in tree.Nodes)
            {
                found = FindInTreeView(node, data);

                if (found != null)
                    return found;
            }

            return null;
        }

        private static TreeNode FindInTreeView(TreeNode root, DataNode data)
        {
            if (root == null ||
                data == null)
                return null;

            if (root.Tag == null)
                return null;

            if ((DataNode)root.Tag == data)
                return root;

            TreeNode found = null;
            foreach (TreeNode child in root.Nodes)
            {
                found = FindInTreeView(child, data);

                if (found != null)
                    return found;
            }

            return null;
        }
        

        #region Image

        private void LoadImg(string imgPath)
        {
            if (imgPath == "")
            {
                imgBg.Image = null;
                return;
            }

            imgPath = Path.Combine(GameObj.GetBGImagePath(), imgPath);

            Bitmap img = null;
            if (File.Exists(imgPath))
            {
                img = new Bitmap(imgPath);
                imgBg.Image = img;
            }
            else
            {   //Image not found.
                string imgMissing = Path.Combine(GameObj.GetBGImagePath(), GameObject.BG_MISSING);
                if (File.Exists(imgMissing))
                {
                    img = new Bitmap(imgMissing);
                    imgBg.Image = img;
                }
            }

        }


        #endregion

        #region Buttons

        private void UpdateButtonView()
        {
            GameObj.InvalidatePresentation();
            List<GameButton> buttonData = GameObj.presentation.buttonData;           

            //Make visible what shows.
            int btnID = 0;
            foreach (GameButton data in buttonData)
            {
                Buttons[btnID].Visible = true;
                Buttons[btnID].Text = data.PreText;
                Buttons[btnID].Tag = data;
                btnID++;
            }

            //Hide the rest.
            for (; btnID < Buttons.Count(); btnID++)
                Buttons[btnID].Visible = false;

            //Add Return to start button. Wipe Bag and Dictionary.
            if (buttonData.Count == 0)
            {
                GameButton startButton = new GameButton("Return to Start", "0");
                Buttons[0].Visible = true;
                Buttons[0].Text = startButton.PreText;
                Buttons[0].Tag = startButton;
            }
        }

        #endregion


        #endregion

        #region Misc

        private string GetRandChars(int charCount)
        {
            
            Random rand = new Random();

            StringBuilder builder = new StringBuilder(charCount);

            for (int i = 0; i < charCount; i++)
            {
                char c = (char)((rand.Next() % 26) + 97);
                builder.Append(c);
            }

            return builder.ToString();

        }

        #endregion

        #region Preferences / Application Properties

        /// <summary>
        /// Apply saved properties to their controls and variables
        /// </summary>
        private void LoadProperties()
        {
            chkDisableScript.Checked = Properties.Settings.Default.runScript;
        }

        private string GetProjectsDir()
        {
            return GameObject.GetWindowsProjectFolder();
        }

        #endregion


        
    }
}

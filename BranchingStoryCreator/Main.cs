using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MSScriptControl;
using BranchingStoryCreator.Web;
using BranchingStoryCreator.Forms;

namespace BranchingStoryCreator
{ 
    public partial class Main : Form
    {

        #region Variables

        public Button[] Buttons { get; set; }
        public StoryTree treeSnapshot { get; set; }
        public GameViewer viewerWindow { get; set; }
        public Presentation currentPresentation { get; set; }

        public WinSound soundPlayer { get; set; }

        //Player and Game
        public string gameName;
        public string playerID = "0";

        //Behind the scenes
        private int countDown; //For timer.
        private string tempStory; //For story value-key replace

        private DataNode lastParentofSelected;
        private DataNode lastSelected;

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
            soundPlayer = new WinSound();

            //Instantiate Variables
            Buttons = new Button[5];

            //Add Button References to Array
            Buttons[0] = btn0;
            Buttons[1] = btn1;
            Buttons[2] = btn2;
            Buttons[3] = btn3;
            Buttons[4] = btn4;

            //Initialize Undo Redo
            //UpdateUndoRedoMenu();

            ToggleInputFields(false);

            foreach (Button btn in Buttons)
                btn.Visible = false;

            countDown = 0;
            tsMessage.Text = "";
            timer.Start();

            //Initialize GameServer
            string projDir = GetProjectsDir();
            string templateDir = Application.StartupPath;
            GameServer.Initialize(projDir, templateDir);

            //Auto load project?
            if (Properties.Settings.Default.autoLoadLastProject)
            {
                string lastProjectPath = Properties.Settings.Default.lastProjectPath;

                if (lastProjectPath == "")
                    lastProjectPath = GetDefaultProjectPath();

                if (!File.Exists(lastProjectPath))
                    lastProjectPath = GetDefaultProjectPath();

                if (File.Exists(lastProjectPath))
                {
                    LoadTree(lastProjectPath);                   
                }
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

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            if (treeSnapshot != null)
                treStory.CollapseAll();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (treeSnapshot != null)
                treStory.ExpandAll();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayGame();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopGame();
            currentPresentation = GameServer.GetPresentation(gameName, playerID, true);
            NavigateToPresentationNode();
        }

        private void btnTranspose_Click(object sender, EventArgs e)
        {
            //tempStory = txtStory.Text;
            //txtStory.Text = GameObj.dic.ReplaceKeysWithValues(txtStory.Text);
        }

        //Meta 
        private void btnSelectGameImg_Click(object sender, EventArgs e)
        {
            ShowPictureDialog(true);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                string gameImgPath = GameServer.GetBGImageDir(gameName) + txtGameImg.Text;

                if (File.Exists(gameImgPath))
                {
                    Bitmap bitmap = new Bitmap(gameImgPath);
                    imgBg.Image = bitmap;
                }
                else
                {
                    throw new Exception(string.Format("The file: {0} could not be found at: {1}", txtGameImg.Text, gameImgPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to show Game Image for: {0}. {1}", gameName, ex.Message));
            }

        }

        #endregion

        #region Checkboxes

        private void chkSoundEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSoundEnabled.Checked == false)
                soundPlayer.StopAll();
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
            //Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Redo();
        }

        #endregion

        #region Open Project Folders

        private void bGImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectFolder = GameServer.GetBGImageDir(gameName);
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
            string projectFolder = GameServer.GetItemImgDir(gameName);
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
            string projectFolder = GameServer.GetSoundDir(gameName);
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
            PlayGame();
        }


        #endregion

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataNode node = treStory.SelectedNode.Tag as DataNode;
            string errMsg = SetDataNodeValues(node);

            if (errMsg == "")
                ShowMinorMessage("Node successfully updated.", 2000);
            else
                MessageBox.Show("Unable to update node! " + errMsg);
        }

        #endregion

        #region Context Menu

        private void addNodeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AddChildNode();
        }

        private void addSiblingToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AddSiblingNode();
        }

        private void parentToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AddParentNode();
        }

        /// <summary>
        /// Remove Selected Node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        /// <summary>
        /// Remove the selected and all its children.
        /// </summary>
        /// <returns></returns>
        private void selectedAndChildrenStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveSelectedNodeAndChildren();
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
         
            treStory.BeforeSelect += EditorTree_BeforeSelect;
            treStory.AfterSelect += EditorTree_AfterSelect;

            //Add wire-up nav button click events.
            foreach (Button btn in Buttons)
                btn.Click += btnNavButton_Clicked;
        }

        private void RemoveEvents()
        {
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

            if (e.Alt && e.KeyCode == Keys.S)
                ToggleStoryViewFocus();
        }

        #endregion

        #region Image

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (treeSnapshot != null)
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

            if (treeSnapshot == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] validImgExts = StoryProject.GetSupportedImageExt();
                string[] filePath = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string file in filePath)
                {
                    foreach (string ext in validImgExts)
                        if (Path.GetExtension(file) == ext)
                        {
                            string fileName = Path.GetFileName(file).Replace(' ', '_');
                            string imgCopyTo = GameServer.GetBGImageDir(gameName) + fileName;
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

        private void SyncWithGameViewer(object sender, EventArgs e)
        {
            currentPresentation = GameServer.GetPresentation(gameName, playerID, false);
            NavigateToPresentationNode();
        }

        private void EditorTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //Get the selected node.
            if (treStory.SelectedNode == null)
                return;

            DataNode data = treStory.SelectedNode.Tag as DataNode;

            if (data != null)
            {
                SetDataNodeValues(data);
                TreeNode lastNode = FormHelper.GetNode(treStory, data.ID);

                if (lastNode != null)
                    FormHelper.SetNodeText(ref lastNode, data, false);
            }

            TreeNode node = e.Node;

            if (node != null)
            {
                //DataNode data = node.Tag as DataNode;
            }

            Console.WriteLine("Editor Before");
        }

        private void EditorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {

            DataNode data = e.Node.Tag as DataNode;
            TreeNode selected = null;

            if (data != null)
            {
                selected = FindInTreeView(treStory, data);
                if (selected == null)
                    return;

                EditResponse response = GameServer.MovePlayerToID(gameName, playerID, data.ID);

                if (response.errMsg != "")
                {
                    MessageBox.Show(string.Format(
                        "Unable to jump player to node: {0}. {1}", 
                        data.ID,
                        response.errMsg));
                }

                BindDataNodeValues(data);
                ToggleInputFields(true);
                UpdatePlayerDataView();
                if (currentPresentation == null ||
                    data.ID != currentPresentation.nodeID)
                {               
                    currentPresentation = GameServer.GetPresentation(gameName, playerID);
                }

                if (currentPresentation.inputPending)
                {
                    
                    InputBox box = new InputBox("Enter text.", currentPresentation.inputPrompt);
                    box.ShowDialog();

                    PresentationRequest request = new PresentationRequest(gameName, playerID);
                    request.textResponse = box.textResponse;
                    currentPresentation = GameServer.GetPresentation(request);
                    NavigateToPresentationNode();                    
                }


                UpdateButtonView();

                if (viewerWindow == null || !viewerWindow.Visible)
                    if (chkSoundEnabled.Checked)
                        soundPlayer.HandleSound(currentPresentation.sound);
            }

            if (data != null &&
                selected != null)
            {
                FormHelper.SetNodeText(ref selected, data, true);
            }


            //Ask Game Viewer to select this node.
            TreeNode node = e.Node;

            if (node != null)
            {
                //DataNode data = node.Tag as DataNode;
                //GameObj.editor.SelectNode(data);

                //if (treStory.DisplayRectangle.Contains(treStory.PointToClient(Cursor.Position)))
                //    UpdateViewerWindow();
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
                string nodeID = data.ButtonID;
                string previousNodeID = (currentPresentation != null) ? currentPresentation.nodeID : "";
                string btnID = btn.Name.Replace("btn", "");
     
                PresentationRequest request = new PresentationRequest(gameName, playerID, btnID);
                currentPresentation = GameServer.GetPresentation(request);

                string newNodeID = currentPresentation.nodeID;

                if (previousNodeID == newNodeID)
                    MessageBox.Show("Unable to proceed to Node ID " + nodeID);
                else
                {
                    UpdatePlayerDataView();
                    NavigateToPresentationNode();
                }
                    
            }
        }


        private void btnExecute_Click(object sender, EventArgs e)
        {
            //ExecuteScript();
        }

        private void btnTranspose_MouseDown(object sender, MouseEventArgs e)
        {
            tempStory = txtStory.Text;
            txtStory.Text = currentPresentation.story;
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
                    FormHelper.SetNodeText(ref node, data, true);
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
                    FormHelper.SetNodeText(ref node, data, true);
                }
            }
        }

        private void chkRun_CheckedChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.runScript = chkDisableScript.Checked;
           // Properties.Settings.Default.Save();
        }

        #endregion

        //#region Undo / Redo

        //private bool Undo()
        //{
        //    //if (UndoStack.Count > 0)
        //    //{
        //    //    SetRedoPoint();
        //    //    treStory.Nodes.Clear();

        //    //    TreeView popTree = UndoStack.Pop();

        //    //    treStory.Nodes.Clear();
        //    //    StoryTreeManager.TransferNodes(popTree, ref treStory);
        //    //    return true;
        //    //}
        //    //else
        //        return false;

        //}

        //private bool Redo()
        //{
        //    //if (RedoStack.Count > 0)
        //    //{
        //    //    SetUndoPoint();
        //    //    treStory.Nodes.Clear();

        //    //    TreeView popTree = RedoStack.Pop();

        //    //    treStory.Nodes.Clear();
        //    //    StoryTreeManager.TransferNodes(popTree, ref treStory);
        //    //    return true;
        //    //}
        //    //else
        //        return false;
        //}

        //private void SetUndoPoint()
        //{
        //    //TreeView saveState = StoryTreeManager.CloneTree(treStory);
        //    //UndoStack.Push(saveState);

        //    //UpdateUndoRedoMenu();
        //}

        //private void SetRedoPoint()
        //{
        //    //TreeView saveState = StoryTreeManager.CloneTree(treStory);
        //    //RedoStack.Push(saveState);

        //    //UpdateUndoRedoMenu();
        //}

        //private void UpdateUndoRedoMenu()
        //{
        //    //if (UndoStack.Count > 0)
        //    //    undoToolStripMenuItem.Enabled = true;
        //    //else
        //    //    undoToolStripMenuItem.Enabled = false;

        //    //if (RedoStack.Count > 0)
        //    //    redoToolStripMenuItem.Enabled = true;
        //    //else
        //    //    redoToolStripMenuItem.Enabled = false;

        //}

        //#endregion

        #region Editing

        private bool PerformEdit(RequestType reqType)
        {
            DataNode selected = treStory.SelectedNode.Tag as DataNode;
            EditResponse response = null;

            bool success = false;

            if (selected == null)
            {
                MessageBox.Show("The selected node is null");
                success = false;
            }
            else
            {
                //Refresh the tree view.
                response = GameServer.PostEditRequest(gameName, reqType, selected);

                if (response.errMsg == "")
                {
                    ShowMinorMessage(string.Format("Successful edit. Affected nodes: {0}",
                        response.nodesAffected), 3000);
                    success = true;
                }
                else
                {
                    MessageBox.Show(string.Format("Request Error: {0}", response.errMsg));
                    success = false;
                }
            }

            UpdateTreeView();
            SelectLastOrParentOr(response.affectedID);
            btnExpand.PerformClick();
            
            txtButtonText.Focus();

            return success;
        }

        #region Add / Remove Nodes

        public void UpdateTreeView()
        {
            treeSnapshot = GameServer.GetTree(gameName);           

            if (treeSnapshot != null)
            {
                FormHelper.SetTreeViewData(treeSnapshot, ref treStory);
            }
            else
            {
                MessageBox.Show("Unable to refresh the tree view. Tree Snapshot for "
                    + gameName + " could not be retrieved from the GameServer.");
            }
        }


        /// <summary>
        /// //Update DataNode in GameObject via GameServer.
        /// </summary>
        /// <param name="data"></param>
        private string SetDataNodeValues(DataNode node)
        {
            //Not using node data, just making sure a node is selected.
            if (node == null)
                return "Cannot update GameObject. No node is selected.";

            DataNode setNode = new DataNode();

            // Create a DataNode to send to the GameServer.
            setNode.ID = txtID.Text;
            setNode.ImgPath = txtImgPath.Text;
            setNode.ButtonText = txtButtonText.Text;
            setNode.Story = txtStory.Text;
            setNode.Script = txtScript.Text;
            setNode.AvailIf = txtAvailIf.Text;

            EditResponse response = GameServer.PostEditRequest(gameName, RequestType.Set, setNode);

            return response.errMsg;
        }

        /// <summary>
        /// Adds a node, and updates the tree view.
        /// </summary>
        /// <returns></returns>
        private bool AddChildNode()
        {
            return PerformEdit(RequestType.AddChild);
        }

        /// <summary>
        /// Adds a node to the selected node's parent.
        /// </summary>
        /// <returns></returns>
        private bool AddSiblingNode()
        {
            return PerformEdit(RequestType.AddSibling);
        }

        /// <summary>
        /// Squeeze a node in between the current selected node and its parent.
        /// </summary>
        /// <returns></returns>
        private bool AddParentNode()
        {
            return PerformEdit(RequestType.AddParent);
        }

        /// <summary>
        /// Remove only the selected node.
        /// </summary>
        /// <returns></returns>
        private bool RemoveSelectedNode()
        {
            return PerformEdit(RequestType.RemoveSelectedOnly);
        }

        /// <summary>
        /// Remove the selected node.
        /// </summary>
        /// <returns></returns>
        private bool RemoveSelectedNodeAndChildren()
        {
            return PerformEdit(RequestType.RemoveSelectedAndChildren);
        }


        #endregion

        #region Save Node Values

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

            SetDataNodeValues(selectedData);

        }

        private DataNode GetSelectedDataNode()
        {
            if (treStory.SelectedNode == null)
                return null;

            if (treStory.SelectedNode.Tag == null)
                return null;

            return treStory.SelectedNode.Tag as DataNode;
        }
        #endregion

        #region Scripting

        #endregion

        #endregion

        #region New / Save / Load / Close

        private void SaveProject()
        {
            if (treeSnapshot == null)
            {
                MessageBox.Show("No project is open to save.");
                return;
            }

            string author = txtAuthor.Text;
            string gameImg = txtGameImg.Text;
            string about = txtAbout.Text;


            SetSelectedDataNode();

            GameMeta meta = new GameMeta(gameName, author, about, gameImg);
            EditResponse response = GameServer.SaveProject(gameName, meta);

            if (response.errMsg == "")
            {
                ShowMinorMessage("Project saved!", 5000);
            }
            else
            {
                MessageBox.Show("Unable to save project. " + response.errMsg);
            }
        }

        private void CloseCurrentProject()
        {
            if (treeSnapshot != null)
            {
                //PromtToSave();
                RemoveEvents();
            }

            if (viewerWindow != null)
            {
                viewerWindow.Close();
                viewerWindow.Dispose();
                viewerWindow = null;
            }
          
            ToggleInputFields(false);
            tempStory = "";
            treStory.Nodes.Clear();
            imgBg.Image = null;
            treeSnapshot = null;

            if (gameName != null)
                GameServer.RemoveGame(gameName);

            gameName = "";
            
        }

        private void CreateNewProject()
        {
            CloseCurrentProject();

            string projectName = GetProjectName();

            //Abort creation.
            if (projectName == "")
                return;

            EditResponse response = GameServer.CreateNewProject(projectName);

            if (response.errMsg == "")
            {
                //Creation successful.
                gameName = response.gameName;
                DisplayProjectInEditor(projectName);
                string treePath = GameServer.GetTreePath(gameName);
                ProjectInitialization(treePath);
            }
            else
            {
                MessageBox.Show(string.Format("Unable to create new project. {0}", response.errMsg));
            }
        }

        private static string GetProjectName()
        {
         
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
                    break;
                }
           
            }

            return projName;
        }

        private void LoadTree(string filePath)
        {
            CloseCurrentProject();

            if (File.Exists(filePath))
            {

                //Load into GameServer
                EditResponse response = GameServer.AddProject(filePath);
                gameName = response.gameName;

                if (response.errMsg == "")
                {
                    //Attempt to display tree.                 
                    DisplayProjectInEditor(gameName);             
                }

                ShowMinorMessage("Story Loaded!", 5000);
                ProjectInitialization(filePath);
            }
            else
                ShowMinorMessage("File does not exist: " + filePath, 3000);

        }

        public void DisplayProjectInEditor(string gameName)
        {
            GameMeta meta = GameServer.GetGameMeta(gameName);
            gameName = meta.gameName;
            if (meta != null)
            {
                txtAbout.Text = meta.about;
                txtAuthor.Text = meta.author;
                txtGameImg.Text = meta.gameImg;
            }

            UpdateTreeView();
            ToggleInputFields(true);
        }

        /// <summary>
        /// Call first.
        /// </summary>
        private void ProjectInitialization(string projectTreePath)
        {
            AddEvents();
            //try
            //{
                currentPresentation = GameServer.GetPresentation(gameName, playerID, true);
                treStory.SelectedNode = treStory.TopNode;
                Properties.Settings.Default.lastProjectPath = projectTreePath;
                Properties.Settings.Default.Save();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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


        private DialogResult ShowPictureDialog(bool forMeta = false)
        {

            string imgFolder = GameServer.GetBGImageDir(gameName);
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

                if (!forMeta)
                { 
                txtImgPath.Text = fileName;

                //Update node for new picture.
                DataNode data = treStory.SelectedNode.Tag as DataNode;

                SetDataNodeValues(data);
                }
                else //for meta
                {
                    txtGameImg.Text = fileName;
                }
            }


            return result;
        }


        private DialogResult YesOrNoQuestion(string longQuestion, string shortQuestion)
        {
            return MessageBox.Show(longQuestion, shortQuestion, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        #endregion

        #region View

        private void PlayGame()
        {
            if (treeSnapshot == null)
            {
                MessageBox.Show("Cannot run an empty game.");
                return;
            }


            if (viewerWindow != null)
                viewerWindow.Close();

            if (viewerWindow == null || viewerWindow.IsDisposed)
            {
                viewerWindow = new GameViewer(gameName, playerID);
                viewerWindow.ContextChanging += new EventHandler(SyncWithGameViewer);
            }

            viewerWindow.Show();
        }

        private void StopGame()
        {
            soundPlayer.StopAll();
            currentPresentation = GameServer.GetPresentation(gameName, playerID, true);

            if (viewerWindow != null)
                if (viewerWindow.Visible)
                    viewerWindow.Close();

            NavigateToPresentationNode();
        }

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

        private void ToggleStoryViewFocus()
        {
            if (tabCtrl.SelectedIndex == 0)
                tabCtrl.SelectedIndex = 1;
            else
                tabCtrl.SelectedIndex = 0;
        }

        private void UpdatePlayerDataView()
        {
            PlayerData playerData = GameServer.GetPlayerData(gameName, playerID);

            //Update Variable List.
            lstGameObj.Items.Clear();
            List<string> dicPairs = playerData.dic.GetFormattedPairs();
            foreach (string item in dicPairs)
                lstGameObj.Items.Add(item);

            //Update Game Items List.
            lstItems.Items.Clear();
            List<string> itemPairs = playerData.items.GetFormattedValues();
            foreach (string item in itemPairs)
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
                SetLastAndCurrent();
                return;
            }
        }

        private static TreeNode FindInTreeView(TreeView tree, DataNode data)
        {
            // See if selected is what we're looking for first.
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode.Tag == data)
                    return tree.SelectedNode;
            }

            //If there's no other data, return null;
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

        private static TreeNode FindInTreeView(TreeView tree, string nodeID)
        {
            DataNode searchNode = new DataNode();
            searchNode.ID = nodeID;

            return FindInTreeView(tree, searchNode);
        }

        private static TreeNode FindInTreeView(TreeNode root, DataNode data)
        {
            if (root == null ||
                data == null)
                return null;

            if (root.Tag == null)
                return null;

            DataNode thisNode = root.Tag as DataNode;

            if (thisNode.ID == data.ID)
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

        private DataNode GetParentOfSelected()
        {
            TreeNode selected = treStory.SelectedNode;
            TreeNode parent = null;

            if (selected == null)
                return null;

            parent = selected.Parent;

            if (parent == null)
                return null;

            if (parent.Tag == null)
                return null;

            return selected.Parent.Tag as DataNode;
        }

        private void SetLastAndCurrent()
        {
            lastParentofSelected = GetParentOfSelected();
            lastSelected = GetSelectedDataNode();
        }

        private void NavigateToPresentationNode()
        {
            try
            {
                if (currentPresentation != null)
                {
                    if (currentPresentation.nodeID != null)
                    {
                        TreeNode targetNode = FormHelper.GetNode(treStory, currentPresentation.nodeID);

                        if (targetNode != null)
                            treStory.SelectedNode = targetNode;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to navigate to next node: " + ex.Message);
            }
        }

        private void SelectLastOrParentOr(string nodeID)
        {
            TreeNode node = null;

            if (nodeID != "")
            {
                node = FindInTreeView(treStory, nodeID);

                if (node != null)
                {
                    treStory.SelectedNode = node;
                    return;
                }
            }
            

            if (lastSelected != null)
            {
                node = FindInTreeView(treStory, lastSelected);

                if (node != null)
                {
                    treStory.SelectedNode = node;
                    return;
                }
            }

            if (lastParentofSelected != null)
            {
                node = FindInTreeView(treStory, lastParentofSelected);

                if (node != null)
                {
                    treStory.SelectedNode = node;
                }
            }
        }

        private void InvalidateEditorView(DataNode data = null, Presentation current = null)
        {
            if (current == null)
            {
                currentPresentation = GameServer.GetPresentation(gameName, playerID);
            }

            if (data == null)
            {
                if (treStory.SelectedNode != null)
                    data = treStory.SelectedNode.Tag as DataNode;
                else
                    return;
            }

            BindDataNodeValues(data);
            ToggleInputFields(true);
            UpdatePlayerDataView();
            if (currentPresentation == null ||
                data.ID != currentPresentation.nodeID)
                currentPresentation = currentPresentation = GameServer.GetPresentation(gameName, playerID);
            UpdateButtonView();

            if (viewerWindow == null || !viewerWindow.Visible)
                if (chkSoundEnabled.Checked)
                    soundPlayer.HandleSound(currentPresentation.sound);
        }
        

        #region Image

        private void LoadImg(string imgPath)
        {
            if (imgPath == "")
            {
                imgBg.Image = null;
                return;
            }

            imgPath = Path.Combine(GameServer.GetBGImageDir(gameName), imgPath);

            Bitmap img = null;
            if (File.Exists(imgPath))
            {
                img = new Bitmap(imgPath);
                imgBg.Image = img;
            }
            else
            {   //Image not found.
                string imgMissing = Path.Combine(GameServer.GetBGImageDir(gameName), GameObject.BG_MISSING);
                if (File.Exists(imgMissing))
                {
                    img = new Bitmap(imgMissing);
                    imgBg.Image = img;
                }
            }

        }


        #endregion

        #region Buttons

        //Used to show or hide buttons for the current presentation.
        private void UpdateButtonView()
        {
            //If P is null, hide all buttons. 
            DataNode node = GetSelectedDataNode();

            if (node == null)
            {
                foreach (Button btn in Buttons)
                    btn.Visible = false;

                return;
            }


            List<GameButton> buttonData = GameObject.GetGameButtons(node);

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
            for (; btnID < Buttons.Length; btnID++)
                Buttons[btnID].Visible = false;

            ////Add Return to start button. Wipe Bag and Dictionary.
            //if (buttonData.Count == 0)
            //{
            //    GameButton startButton = new GameButton("Return to Start", "");
            //    Buttons[0].Visible = true;
            //    Buttons[0].Text = startButton.PreText;
            //    Buttons[0].Tag = startButton;
            //}
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
            //chkDisableScript.Checked = Properties.Settings.Default.runScript;
        }

        private string GetProjectsDir()
        {
            return GameObject.GetWindowsProjectFolder();
        }

        private string GetDefaultProjectPath()
        {
            string path = Path.Combine(GetProjectsDir(), "Sonic_Game");
            path = Path.Combine(path, "Sonic_Game" + StoryProject.PROJECT_EXT);
            return path;
        }

        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
















        
    }
}

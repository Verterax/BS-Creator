using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSScriptControl;
//using System.Windows.Forms;
using System.IO;

namespace BranchingStoryCreator.Web
{
     
    public class GameObject
    {
        #region Variables

        //Store User Specific Game Data (Items, Vars, and Pos.
        public Dictionary<string, PlayerData> playerData { get; set; }
        public ExtraData extraData { get; set; }
        public StoryTree tree { get; private set; }
        public string gameDirectory { get; private set; }
        public bool isWebGame { get; private set; }
        public string gameName { get { return extraData.projectName.ToLower(); } }

        #endregion

        #region Consts

        public const string BG_IMG_FOLDER = @"Images\Bg\";
        public const string ITEM_IMG_FOLDER = @"Images\Items\";
        public const string SOUND_FOLDER = @"Sounds\";
        public const string SCRIPTS_FOLDER = @"Scripts\";

        public const string RESOURCES_FOLDER = @"Bessy\";


        public const string DEFAULTS_FOLDER = @"Defaults\";
        public const string BG_MISSING = "bg_missing.jpg";
        public const string ITEM_MISSING = "item_missing.png";
        public const string SOUND_MISSING = "sound_missing.mp3";
        public const string TREE_EXT = ".tree";

        public const int MAX_CHILDREN = 5;

        #endregion

        #region Load / Init / Constructor
        public GameObject(string xmlPath, string virtualPath = "") //Loads serialized project from file.
        {           
            gameDirectory = Path.GetDirectoryName(xmlPath);
            
            //try 
            //{
                StoryProject project = StoryProject.Deserialize(xmlPath);
                string resourcePath = Path.GetDirectoryName(xmlPath);
                Init(project.storyTree, project.storyData, resourcePath);
            //}
            //catch (Exception ex)
            //{ throw ex; }
           
        }

        public GameObject(StoryProject project, string gameDir, string virtualRoot = "")
        {
            Init(project.storyTree, project.storyData, gameDir, virtualRoot);
        }

        public GameObject(StoryTree tree, ExtraData storyData, string resourcePath, string virtualRoot = "")
        {
            Init(tree, storyData, resourcePath, virtualRoot);
        }

        private void Init(StoryTree tree, ExtraData storyData, string resourcePath, string virtualRoot = "")
        {
            this.tree = tree;
            this.extraData = storyData;

            
            if (virtualRoot == "") //Is local game.
            {
                if (resourcePath.Contains(TREE_EXT))
                    gameDirectory = Path.GetDirectoryName(resourcePath);
                else
                    gameDirectory = resourcePath;
                isWebGame = false;
            }
            else //Is web game.
            {
                gameDirectory = virtualRoot;
                isWebGame = true;
            }

            playerData = new Dictionary<string, PlayerData>();

            //Add Events.
            AddEvents();
        }

        #endregion

        #region Events

        private void AddEvents()
        {
            //tree.GamePosition_Changing += OnGamePosition_Changing;
        }

        private void RemoveEvents()
        {
            //tree.GamePosition_Changing += OnGamePosition_Changing;
        }

        #endregion

        #region Create New Project

        public static GameObject CreateNewProject(string projectName, string projTemplateDir, string newProjDir)
        {
            StoryTree newTree = StoryTree.GetNewStoryTree();
            ExtraData newData = ExtraData.GetNewStoryData();

            projectName = projectName.Replace(" ", "_");
            newData.projectName = projectName;

            EnsureFolders(newProjDir);
            EnsureFiles(newProjDir, projTemplateDir);
            GameObject newGameObj = new GameObject(newTree, newData, newProjDir);

            return newGameObj;
        }

        public static string GetProjectValidName(string candidateName)
        {
            const int MAX_NAME_LENGTH = 25;

            if (candidateName == "")
                return candidateName;

            candidateName = candidateName.Replace(" ", "_");

            StringBuilder builder = new StringBuilder();

            foreach (char c in candidateName)
                if (char.IsLetterOrDigit(c) || c == '_')
                    builder.Append(c);

            string finalString = builder.ToString().Trim();

            finalString = (finalString.Length < MAX_NAME_LENGTH) ?
                finalString : finalString.Substring(0, MAX_NAME_LENGTH - 1);

            return finalString;
        }
      

        #endregion

        #region Get Paths

        public static string GetWindowsProjectFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + RESOURCES_FOLDER;
        }

        public string GetProjectName()
        {
            return extraData.projectName;
        }

        public string GetBGImageDir()
        {
            string path = Path.Combine(gameDirectory, BG_IMG_FOLDER);

            if (isWebGame)
                return path.Replace("\\", "/");
            else
                return path;
        }

        public string GetSoundDir()
        {
            string path = Path.Combine(gameDirectory, SOUND_FOLDER);

            if (isWebGame)
                return path.Replace("\\", "/");
            else
                return path;
        }

        public string GetItemImgDir()
        {
            string path = Path.Combine(gameDirectory, ITEM_IMG_FOLDER);

            if (isWebGame)
                return path.Replace("\\", "/");
            else
                return path;
        }

        public string GetResourceFolder()
        {
            string path = gameDirectory;

            if (isWebGame)
                return path.Replace("\\", "/");
            else
                return path;
        }

        public string GetTreePath()
        {
            string treePath = Path.Combine(gameDirectory, extraData.projectName + StoryProject.PROJECT_EXT);
            return treePath;
        }

        #region Get File missing
        
        public string GetImageMissingPath()
        {
            return Path.Combine(gameDirectory, GameObject.BG_IMG_FOLDER, GameObject.BG_MISSING);
        }

        public string GetItemMissingPath()
        {
            return Path.Combine(gameDirectory, GameObject.ITEM_IMG_FOLDER, GameObject.ITEM_MISSING);
        }

        public string GetSoundMissingPath()
        {
            return Path.Combine(gameDirectory, GameObject.SOUND_FOLDER, GameObject.SOUND_MISSING);
        }
        
        #endregion


        /// <summary>
        /// Creates required project folders if not existing.
        /// </summary>
        /// <param name="projectFolder"></param>
        public static void EnsureFolders(string projectFolder)
        {
            string[] dirs = new string[]
            {
                projectFolder,
                Path.Combine(projectFolder, ITEM_IMG_FOLDER),
                Path.Combine(projectFolder, BG_IMG_FOLDER),
                Path.Combine(projectFolder, SOUND_FOLDER),
                Path.Combine(projectFolder, SCRIPTS_FOLDER)
            };

            foreach (string dir in dirs)
            {
                try 
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                   throw new Exception ("Unable to create required project folders. " + ex.Message);
                }
            }

            

        }

        public static void EnsureFiles(string projectFolder, string filesDir)
        {

            if (!Directory.Exists(projectFolder) ||
                !Directory.Exists(filesDir))
            {
                Console.WriteLine("Unable to create default files. The directory does not exist.");
                return;
            }

            string copyFrom = "";
            string copyTo = "";

            //Copy BG missing
            copyFrom = Path.Combine(filesDir, BG_IMG_FOLDER, BG_MISSING);
            copyTo = Path.Combine(projectFolder, BG_IMG_FOLDER + BG_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            //Copy Item missing.
            copyFrom = Path.Combine(filesDir, ITEM_IMG_FOLDER, ITEM_MISSING);
            copyTo = Path.Combine(projectFolder, ITEM_IMG_FOLDER + ITEM_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            //Copy Sound missing.
            copyFrom = Path.Combine(filesDir, SOUND_FOLDER, SOUND_MISSING);
            copyTo = Path.Combine(projectFolder, SOUND_FOLDER + SOUND_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            
            
        }

        #endregion
    
        #region Navigation

        ///// <summary>
        ///// Accepts a nodeID as input. Checks if movement to that node would be valid. If so, context will change.
        ///// </summary>
        ///// <param name="buttonNumber"></param>
        ///// <returns>True if context change was successful.</returns>
        //public bool ButtonPress(string nodeID, bool forceMove = false)
        //{

        //    DataNode selected = tree.SelectedNode;

        //    if (selected == null ||
        //        nodeID == null ||
        //        nodeID == "")
        //        return false;

        //    DataNode target = StoryTree.GetNode(tree, nodeID);

        //    if (target == null)
        //        return false;

        //    bool isLegalMove = IsLegalMove(tree, target, script);
        //    if (isLegalMove || forceMove) //If legal to move, select the new node in the tree.
        //    {
        //        isLegalMove = true; // If forced.
        //        tree.pos = nodeID;
        //    }

        //    return isLegalMove;
        //}

        public static bool IsLegalMove(DataNode selected, DataNode target, PlayerScript script)
        {
            if (selected == null ||
                target == null ||
                script == null)
                return false;

            if (selected.Nodes.Contains(target)) //If target is a child of selected.
                return bool.Parse(script.EvalShorthand(target.AvailIf)); // Return result of avail if evaluation.

            return false;
        }

        /// <summary>
        ///  Returns the final destination node. Executes intermediate scripts for each node.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="current"></param>
        /// <param name="previous"></param>
        /// <param name="script"></param>
        /// <returns>The final DataNode via any scripting re-directs.</returns>
        private static DataNode GetFinalDestination(int depth, StoryTree tree, DataNode current, PlayerData playerData, PlayerScript script)
        {
            string startPos = playerData.currentPos;
            if (current == null)
                current = StoryTree.GetNode(tree, playerData.currentPos);
          
            if (current == null) //Node must not be null.
                throw new ArgumentNullException("The tree contains no node named " + playerData.currentPos);

            if (depth == 100) //After 100 recurses, assume endless loop.
                throw new Exception("Script Navigation has timed out.");
            
            if (current.Script != "")
            {
                //Run this node's script.
                script.ExecuteStatementShorthand(current.Script);
                Console.WriteLine("Executing {0}", current.Script);
            }

            //If the selected node is still the current node, then is is the final destination.
            string endPos = playerData.currentPos;
            current = StoryTree.GetNode(tree, playerData.currentPos);
            if (startPos == endPos)
            {
                return current;
            }
            else
            {   //The node has changed, execute this node.
                depth++;
                current = GetFinalDestination(depth, tree, current, playerData, script);
            }

            
            return current;

        }

        public void StartFromBeginning()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Presentation

        //private void InvalidatePresentation(DataNode targetNode)
        //{
        //    //presentation = new PresentationObject(GetBGImagePath(), dic, bag, targetNode, script);
        //}

        //public void InvalidatePresentation()
        //{
        //    DataNode current = tree.SelectedNode;
        //    //presentation = new PresentationObject(GetBGImagePath(), dic, bag, current, script);
        //}

        public Presentation GetPresentation(PresentationRequest request)
        {
            Presentation response = null;
            string playerID = request.userID;
            string gameName = request.gameName;
            PlayerData thisPlayer = GetPlayerData(playerID);

            //Add option to return virtual rather than physical paths.
            string bgImgDir = GetBGImageDir();
            string itemImgDir = GetItemImgDir();
            string soundDir = GetSoundDir();
            DataNode target = null;
            bool isLegalMove = false;

            List<ScriptObject> scriptables = GetScriptables(thisPlayer);
            PlayerScript thisScript = new PlayerScript(scriptables);
            DataNode currentNode = StoryTree.GetNode(tree, thisPlayer.currentPos);


            if (request.hasTextResponse == "true" || request.buttonID == "none")
            {
                target = currentNode;
                isLegalMove = true; //Recieve text response. Stay on same node, update dictionary with value.
                thisPlayer.input.RecieveBox(request.textResponse, thisPlayer.dic);
                target = GetFinalDestination(0, tree, target, thisPlayer, thisScript);
            }
            else if (request.buttonID == "" && currentNode != null && currentNode.Nodes.Count > 0)
            {
                //Response is current node.              
                target = currentNode;               
                isLegalMove = true;                
            }
            else if (request.newGame || thisPlayer.currentPos == "" || currentNode == null || (request.buttonID== "0" && currentNode.Nodes.Count == 0))
            {
                 target = tree;                
                 thisPlayer.StartNewGame(tree);
                 request.newGame = true;
                 isLegalMove = true;               
            }
            else if (request.buttonID != "")
            {
                int btnID;
                string strBtnID = request.buttonID;
                bool isNum = int.TryParse(strBtnID, out btnID);

                if (isNum)
                {
                    if (currentNode.Nodes.Count > btnID)
                    {
                        target = currentNode.Nodes[int.Parse(request.buttonID)];
                        isLegalMove = IsLegalMove(currentNode, target, thisScript);

                        if (isLegalMove)
                        {
                            thisPlayer.currentPos = target.ID;
                            target = GetFinalDestination(0, tree, target, thisPlayer, thisScript);
                        }
                    }
                    else
                        isLegalMove = false; // Button does not exist.
                }
                else
                    isLegalMove = false; //Button is not a number.
            }

            if (isLegalMove && target != null)
            {
                if (target == tree) //Run script for root node.
                    target = GetFinalDestination(0, tree, target, thisPlayer, thisScript);

                response = new Presentation(
                    bgImgDir,
                    itemImgDir,
                    soundDir,
                    gameName,
                    thisPlayer,
                    target,
                    thisScript,
                    request.newGame);
            }
            else
            {
                //Not a legal move.
                response = new Presentation(gameName, GetRestartButton(), "Move not legal.");
            }


            return response;
        }

        public PlayerData GetPlayerData(string playerID)
        {
            PlayerData thisPlayer = null;

            //If this game has playerData, use it.
            if (playerData.ContainsKey(playerID))
            {
                thisPlayer = playerData[playerID];
            }
            else
            {
                thisPlayer = new PlayerData(playerID);
                playerData.Add(playerID, thisPlayer);
            }

            return thisPlayer;
        }

        public static List<GameButton> GetGameButtons(DataNode node)
        {
            List<GameButton> buttons = new List<GameButton>();

            foreach (DataNode child in node.Nodes)
            {
                GameButton btn = new GameButton(child.ButtonText, child.ID);
                buttons.Add(btn);
            }

            if (buttons.Count == 0)
            {
                GameButton restart = GetRestartButton();
                buttons.Add(restart);

            }
            return buttons;
        }

        public static GameButton GetRestartButton()
        {
            GameButton restartButton = new GameButton(" The End. Restart?", "");
            return restartButton;
        }

        private List<ScriptObject> GetScriptables(PlayerData playerData)
        {
            List<ScriptObject> scriptObjects = new List<ScriptObject>();

            PlayerVariables dic = playerData.dic;
            ItemBag bag = playerData.items;
            Input input = playerData.input;
            Sound sound = playerData.sound;
            Music music = playerData.music;
            LocWrapper treeWrapper = new LocWrapper(playerData);

            //Add objects that will be scriptable.
            scriptObjects.Add(new ScriptObject(PlayerScript.TREE, treeWrapper));
            scriptObjects.Add(new ScriptObject(PlayerScript.DIC, dic));
            scriptObjects.Add(new ScriptObject(PlayerScript.BAG, bag));
            scriptObjects.Add(new ScriptObject(PlayerScript.INPUT, input));
            scriptObjects.Add(new ScriptObject(PlayerScript.SOUND, sound));
            scriptObjects.Add(new ScriptObject(PlayerScript.MUSIC, music));

            return scriptObjects;
        }

        #endregion

        #region Tree Manipulation

        public EditResponse ReceiveEditRequest(EditRequest request)
        {
            EditResponse response = null;

            switch (request.requestType)
            {
                case RequestType.Set:
                    response = SetNodeData(request);
                    break;
                case RequestType.AddChild:
                    response = AddChildNode(request);
                    break;
                case RequestType.AddSibling:
                    response = AddSiblingNode(request);
                    break;
                case RequestType.AddParent:
                    response = AddParentNode(request);
                    break;
                case RequestType.RemoveSelectedOnly:
                    response = RemoveNodeOnly(request);
                    break;
                case RequestType.RemoveSelectedAndChildren:
                    response = RemoveNodeAndChildren(request);
                    break;
                case RequestType.Unknown:
                //Fall through to default Error.
                default:
                    response = new EditResponse(gameName);
                    response.errMsg = "No Edit Request Type was specified. Doing nothing.";
                    break;
            }

            return response;
        }

        private EditResponse SetNodeData(EditRequest request)
        {
            EditResponse response = new EditResponse(gameName);

            DataNode targetNode = StoryTree.GetNode(tree, request.targetNode.ID);
            DataNode fromNode = request.targetNode;

            if (fromNode == null)
            {
                response.errMsg = "Source node was null. Cannot update.";
                return response;
            }

            if (targetNode == null)
            {
                response.errMsg = string.Format("Unable to find node: {0}", request.targetNode);
                return response;
            }

            try
            {
                //Does not set targetNode.nodes. Does not affect child nodes.

                targetNode.AvailIf = fromNode.AvailIf;
                targetNode.ButtonText = fromNode.ButtonText;
                targetNode.ImgPath = fromNode.ImgPath;
                targetNode.Script = fromNode.Script;
                targetNode.Story = fromNode.Story;
                response.nodesAffected = 1;
            }
            catch (Exception ex)
            {
                response.errMsg = ex.Message;
            }


            return response;
        }

        private DataNode GetNewNode()
        {
            return StoryTree.GetNewNode(tree);
        }

        private EditResponse AddChildNode(EditRequest request)
        {
            DataNode parentNode = StoryTree.GetNode(tree, request);
            EditResponse response = new EditResponse(gameName);
            string parentID = request.targetNode.ID;

            if (parentNode == null)
            {
                response.errMsg = string.Format(
                    "The node having the ID of {0} was not found in game: {1}", 
                    parentID, gameName);

                return response;
            }
      
            int childCount = parentNode.Nodes.Count;

            if (childCount >= MAX_CHILDREN )
            {
                response.errMsg = string.Format(
                    "Cannot add more than {0} children to a node. Node {1} has {2} nodes.",
                    MAX_CHILDREN, parentID, childCount);

                return response;
            }

            DataNode newNode = StoryTree.GetNewNode(tree); 

            if (newNode == null)
            {
                response.errMsg = string.Format("Unable to create a new node for ID {0}", parentID);
                return response;
            }

            //Add the new node to the parent object residing in the game object.
            parentNode.Nodes.Add(newNode);
            response.affectedID = newNode.ID;

            return response;
            
        }

        private EditResponse AddSiblingNode(EditRequest request)
        {
            DataNode parentNode = StoryTree.GetParentOfSelected(tree, request);
            EditResponse response = new EditResponse(gameName);
            string parentID = request.targetNode.ID;

            if (parentNode == null)
            {
                response.errMsg = string.Format(
                    "The parent of the node having the ID of {0} was not found in game: {1}",
                    parentID, gameName);

                return response;
            }

            int childCount = parentNode.Nodes.Count;

            if (childCount >= MAX_CHILDREN)
            {
                response.errMsg = string.Format(
                    "Cannot add more than {0} children to a node. Node {1} has {2} nodes.",
                    MAX_CHILDREN, parentID, childCount);

                return response;
            }

            DataNode newNode = StoryTree.GetNewNode(tree);

            if (newNode == null)
            {
                response.errMsg = string.Format("Unable to create a new node for ID {0}", parentID);
                return response;
            }

            //Add the new node to the parent object residing in the game object.
            parentNode.Nodes.Add(newNode);
            response.affectedID = newNode.ID;

            return response;
        }

        private EditResponse AddParentNode(EditRequest request)
        {
            DataNode oldChild = StoryTree.GetNode(tree, request);
            DataNode oldParentNode = StoryTree.GetParentOfSelected(tree, request);            
            EditResponse response = new EditResponse(gameName);
            string parentID = request.targetNode.ID;

            if (oldParentNode == null)
            {
                response.errMsg = string.Format(
                    "The parent of the node having the ID of {0} was not found in game: {1}",
                    parentID, gameName);

                return response;
            }

            DataNode newMiddleMember = StoryTree.GetNewNode(tree);

            if (newMiddleMember == null)
            {
                response.errMsg = string.Format("Unable to create a new node for ID {0}", parentID);
                return response;
            }

            try
            {
                //Step 1: Remove old child from oldParent.
                oldParentNode.Nodes.Remove(oldChild);

                //Step 2: Add old child to newMiddle.
                newMiddleMember.Nodes.Add(oldChild);

                //Step 3: Add new middle to oldParent.
                oldParentNode.Nodes.Add(newMiddleMember);

                response.affectedID = newMiddleMember.ID;
                //Profit!
            }
            catch (Exception ex)
            {
                response.errMsg = string.Format(
                    "Unable to complete New Parent operation on node {0}. {1}",
                    request.targetNode.ID, ex.Message);

                return response;
            }

            return response;
        }

        private EditResponse RemoveNodeOnly(EditRequest request)
        {
            DataNode target = StoryTree.GetNode(tree, request);
            DataNode oldParentNode = StoryTree.GetParentOfSelected(tree, request);
            EditResponse response = new EditResponse(gameName);

            if (oldParentNode == null)
            {
                response.errMsg = string.Format(
                    "The parent of the node having the ID of {0} was not found in game: {1}. Unable to remove this node.",
                    request.targetNode.ID, gameName);

                return response;
            }

            if (target == null)
            {
                response.errMsg = string.Format(
                    "The node having the ID of {0} was not found in game: {1}. Unable to remove this node.",
                    request.targetNode.ID, gameName);

                return response;
            }

            int parentNodeCount = oldParentNode.Nodes.Count();
            int targetNodeCount = target.Nodes.Count();

            try
            {
                if (parentNodeCount + targetNodeCount <= MAX_CHILDREN)
                {
                    //Remove the target node from the old parent.
                    oldParentNode.Nodes.Remove(target);

                    //Add any of the target's children to the oldParent.
                    foreach (DataNode child in target.Nodes)
                        oldParentNode.Nodes.Add(child);
                }
            }
            catch (Exception ex)
            {
                response.errMsg = string.Format("Unable to remove node {0}.", ex.Message);
                return response;
            }
            
            return response;
        }

        private EditResponse RemoveNodeAndChildren(EditRequest request)
        {
            DataNode target = StoryTree.GetNode(tree, request);
            DataNode oldParentNode = StoryTree.GetParentOfSelected(tree, request);
            EditResponse response = new EditResponse(gameName);

            if (oldParentNode == null)
            {
                response.errMsg = string.Format(
                    "The parent of the node having the ID of {0} was not found in game: {1}. Unable to remove this node and its children.",
                    request.targetNode.ID, gameName);

                return response;
            }

            if (target == null)
            {
                response.errMsg = string.Format(
                    "The node having the ID of {0} was not found in game: {1}. Unable to remove this node and its children.",
                    request.targetNode.ID, gameName);

                return response;
            }

            try
            {
                //Try to remove the node, and it's children.
                oldParentNode.Nodes.Remove(target);
            }
            catch (Exception ex)
            {
                response.errMsg = string.Format(
                    "Unable to remove node {0) from node {1}. {2}", 
                    target.ID, oldParentNode.ID, ex.Message);

                return response;
            }

            return response;
        }

        #endregion

        #region Sound Interface

        public void ToggleSound(bool enableSound)
        {
            //sound.SoundEnabled = enableSound;
        }

        public void StopSound()
        {
            //sound.StopAll();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Add data items such as author and about to the game for serialization and deserialization.
        /// </summary>
        /// <param name="meta"></param>
        public void AddMeta(GameMeta meta)
        {
            extraData.about = meta.about;
            extraData.author = meta.author;
            extraData.gameImg = meta.gameImg;
            extraData.projectName = meta.gameName;
        }

        /// <summary>
        /// Returns a meta object about this game.
        /// </summary>
        /// <param name="fullPath">If true, returns the imgPath full, instead of just the file name and ext.</param>
        /// <returns></returns>
        public GameMeta GetMeta(bool fullPath = false)
        {
            string imgPath = (fullPath) ? GetBGImageDir() + extraData.gameImg : extraData.gameImg;

            GameMeta meta = new GameMeta(
                extraData.projectName,
                extraData.author,
                extraData.about,
                imgPath);

            return meta;
        }

        #endregion


    }

}

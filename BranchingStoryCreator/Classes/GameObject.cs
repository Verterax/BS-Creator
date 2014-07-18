using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSScriptControl;
using System.Windows.Forms;
using System.IO;

namespace BranchingStoryCreator
{
     
    public class GameObject
    {
        #region Variables

        public StoryData storyData { get; set; }

        public GameScript script { get; private set; }
        public GameDic dic { get; private set; }

        private ItemBag bag;
        private Sound sound;
        private StoryTree tree;
        private TreeWrapper treeWrapper;
        private List<ScriptObject> scriptObjects;

        public PresentationObject presentation { get; private set; }
        public GameEditor editor { get; private set; }

        public string ResourcePath { get; private set; }
        
        #endregion

        #region Consts

        public const string BG_IMG_FOLDER = @"Images\Bg\";
        public const string ITEM_IMG_FOLDER = @"Images\Items\";
        public const string SOUND_FOLDER = @"Sounds\";
        public const string SCRIPTS_FOLDER = @"Scripts\";

        public const string RESOURCES_FOLDER = @"BSCreator\";


        public const string DEFAULTS_FOLDER = @"Defaults\";
        public const string BG_MISSING = "bg_missing.jpg";
        public const string ITEM_MISSING = "item_missing.png";
        public const string SOUND_MISSING = "sound_missing.mp3";

        #endregion

        #region Load / Init / Constructor
        public GameObject(string xmlPath) //Loads serialized project from file.
        {
            ResourcePath = Path.GetDirectoryName(xmlPath);
            
            //try 
            //{
                StoryProject project = StoryProject.Deserialize(xmlPath);
                string resourcePath = Path.GetDirectoryName(xmlPath);
                Init(project.storyTree, project.storyData, resourcePath);
            //}
            //catch (Exception ex)
            //{ throw ex; }
           
        }

        public GameObject(StoryTree tree, StoryData storyData, string resourcePath)
        {
            Init(tree, storyData, resourcePath);
        }

        private void Init(StoryTree tree, StoryData storyData, string resourcePath)
        {
            this.tree = tree;
            this.storyData = storyData;
            ResourcePath = resourcePath;

            #region Scriptables

            dic = new GameDic();
            scriptObjects = new List<ScriptObject>();
            bag = new ItemBag(GetItemImgPath());
            sound = new Sound(GetSoundPath());
            treeWrapper = new TreeWrapper(tree);

            //Add objects that will be scriptable.
            scriptObjects.Add(new ScriptObject(GameScript.TREE, treeWrapper));                    
            scriptObjects.Add(new ScriptObject(GameScript.DIC, dic));         
            scriptObjects.Add(new ScriptObject(GameScript.BAG, bag));        
            scriptObjects.Add(new ScriptObject(GameScript.SOUND, sound));

            //Finally, initialize the script with all scriptable objects.
            script = new GameScript(scriptObjects);

            #endregion

            //non-scriptables
            presentation = new PresentationObject(GetBGImagePath(), dic, bag, tree.SelectedNode, script);
            editor = new GameEditor(tree, storyData, ResourcePath);

            //Add Events.
            AddEvents();
        }

        #endregion

        #region Events

        private void AddEvents()
        {
            tree.GamePosition_Changing += OnGamePosition_Changing;
        }

        private void RemoveEvents()
        {
            tree.GamePosition_Changing += OnGamePosition_Changing;
        }

        /// <summary>
        /// When Pos is changed, fire this event. The first first instance blocks, seeking the final node via recursive scripting, eventually returning the final destination and setting the tree.SelectedNode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGamePosition_Changing(object sender, ContextChangeEventArgs e)
        {
            int depth = 0;    
            DataNode current = e.DataNode;

            //Return if already looking forward.
            if (IsLookingForward ||
                current == null)
                return;

            IsLookingForward = true; //Lock this event until it returns.                     
            DataNode finalNode = GetFinalDestination(depth, current, tree, script);
            IsLookingForward = false; //Unlock this event now that GetFinalDesination has returned.

            if (finalNode == null)
                throw new Exception("Unable to navigate into Node: " + current.ID);
            else
            {
                //Update Presentation object.
                if (tree.SelectedNode == finalNode)
                {
                    InvalidatePresentation(finalNode);

                    if (Presentation_Changed != null)
                        Presentation_Changed(null, EventArgs.Empty);
                }
                else
                    throw new Exception("Selected node / final node mismatch");
            }

        }
        private bool IsLookingForward;

        #endregion

        #region Exposed Event

        public event EventHandler Presentation_Changed;

        #endregion 

        #region Create New Project

        public static GameObject CreateNewProject(string projectName, string projectsDirectory)
        {
            StoryTree newTree = StoryTree.GetNewStoryTree();
            StoryData newData = StoryData.GetNewStoryData();

            projectName = projectName.Replace(" ", "_");
            newData.projectName = projectName;

            string projectPath = projectsDirectory + projectName + "\\";
            string defaultFilesPath = Path.Combine(Application.StartupPath, DEFAULTS_FOLDER); 
            EnsureFolders(projectPath);
            EnsureFiles(projectPath, defaultFilesPath);

            GameObject newGameObj = new GameObject(newTree, newData, projectPath);

            return newGameObj;
        }

        public static string GetProjectValidName(string candidateName)
        {
            

            const int MAX_NAME_LENGTH = 25;

            if (candidateName == "")
                return candidateName;

            candidateName = candidateName.Replace(" ", "_");

            string finalString = new string(candidateName.Where(
                c => char.IsLetterOrDigit(c) || c == '_').ToArray());

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

        public string GetBGImagePath()
        {
            return Path.Combine(ResourcePath, BG_IMG_FOLDER);
        }

        public string GetSoundPath()
        {
            return Path.Combine(ResourcePath, SOUND_FOLDER);
        }

        public string GetItemImgPath()
        {
            return Path.Combine(ResourcePath, ITEM_IMG_FOLDER);
        }

        public string GetResourceFolder()
        {
            return ResourcePath;
        }

        public string ProjectTreePath()
        {
            return Path.Combine(ResourcePath, storyData.projectName + StoryProject.PROJECT_EXT);
        }

        /// <summary>
        /// Creates required project folders if not existing.
        /// </summary>
        /// <param name="projectFolder"></param>
        public static void EnsureFolders(string projectFolder)
        {
            string[] dirs = new string[]
            {
                projectFolder,
                projectFolder + ITEM_IMG_FOLDER,
                projectFolder + BG_IMG_FOLDER,
                projectFolder + SOUND_FOLDER,
                projectFolder + SCRIPTS_FOLDER
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
            copyFrom = Path.Combine(filesDir, BG_MISSING);
            copyTo = Path.Combine(projectFolder, BG_IMG_FOLDER + BG_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            //Copy Item missing.
            copyFrom = Path.Combine(filesDir, ITEM_MISSING);
            copyTo = Path.Combine(projectFolder, ITEM_IMG_FOLDER + ITEM_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            //Copy Sound missing.
            copyFrom = Path.Combine(filesDir, SOUND_MISSING);
            copyTo = Path.Combine(projectFolder, SOUND_FOLDER + SOUND_MISSING);
            try { File.Copy(copyFrom, copyTo); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            
            
        }

        #endregion
    
        #region Navigation

        /// <summary>
        /// Accepts a nodeID as input. Checks if movement to that node would be valid. If so, context will change.
        /// </summary>
        /// <param name="buttonNumber"></param>
        /// <returns>True if context change was successful.</returns>
        public bool ButtonPress(string nodeID, bool forceMove = false)
        {
            DataNode selected = tree.SelectedNode;  

            if (selected == null ||
                nodeID == null ||
                nodeID == "")
                return false;

            DataNode target = StoryTree.GetNode(tree, nodeID);

            if (target == null)
                return false;

            bool isLegalMove = IsLegalMove(tree, target, script);
            if (isLegalMove || forceMove) //If legal to move, select the new node in the tree.
            {
                isLegalMove = true; // If forced.
                tree.pos = nodeID;
            }

            return isLegalMove;
        }

        public static bool IsLegalMove(DataNode selected, DataNode target, GameScript script)
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
        /// <param name="tree"></param>
        /// <param name="script"></param>
        /// <returns>The final DataNode via any scripting re-directs.</returns>
        private static DataNode GetFinalDestination(int depth, DataNode current, StoryTree tree, GameScript script)
        {

            if (current == null ||
                tree == null ||
                script == null)
                throw new NullReferenceException();

            if (depth == 100)
                throw new Exception("Script Navigation has timed out.");
           
            //Run this node's script.
            script.ExecuteStatementShorthand(current.Script);

            if (current.Script != "")
                Console.WriteLine("Executing {0}", current.Script);

            //If the selected node is still the current node, then is is the final destination.
            if (tree.SelectedNode == current)
                return current;
            else
            {    //The node has changed, execute this node.
                depth++;
                current = GetFinalDestination(depth, tree.SelectedNode, tree, script);
            }

            return current;

        }

        public void StartFromBeginning()
        {
            dic.Clear();
            bag.Clear();
            //script.ExecuteStatementShorthand(tree.Script);
            tree.pos = tree.ID;           
        }

        #endregion

        #region Presentation

        private void InvalidatePresentation(DataNode targetNode)
        {
            presentation = new PresentationObject(GetBGImagePath(), dic, bag, targetNode, script);
        }

        public void InvalidatePresentation()
        {
            DataNode current = tree.SelectedNode;
            presentation = new PresentationObject(GetBGImagePath(), dic, bag, current, script);
        }

        #endregion

        #region Misc
        public List<string> GetDicValues()
        {
            return dic.GetFormattedPairs();
        }

        public List<string> GetBagValues()
        {
            return bag.GetFormattedValues();
        }

        #endregion





    }


}

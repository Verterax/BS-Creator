using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BranchingStoryCreator.Web
{
    public static class GameServer
    {
        #region Variables
        private static Dictionary<string, GameObject> _games { get; set; }
        private static Dictionary<string, GameObject> games {
            get {
                if (_games == null)
                    _games = new Dictionary<string, GameObject>();

                return _games;
            }
            set  {
                if (_games == null)
                    _games = new Dictionary<string, GameObject>();

                _games = value;
            }
        }
        private static string templateDir { get; set; }
        private static string gamesDir { get; set; }
        #endregion

        #region Load / Init

        public static string Initialize(string gamesDir, string templateDir)
        {
            GameServer.templateDir = templateDir;
            GameServer.gamesDir = gamesDir;

            return "";
        }


        #endregion

        #region Manage Games

        public static EditResponse AddProject(string gamePath, string virtualPath = "")
        {
            //Return an edit response with the status of the Load/Add.
            EditResponse response = new EditResponse("");
           
            try
            {
                StoryProject loadme = StoryProject.Deserialize(gamePath);
                GameObject newGame = new GameObject(loadme, gamePath, virtualPath);
                string gameName = newGame.gameName;
                response.gameName = gameName;

                //Add or overwrite game in collection.
                if (games.ContainsKey(gameName))
                    games[gameName] = newGame;
                else
                    games.Add(gameName, newGame); 
                                 
            }
            catch (Exception ex)
            {
                response.errMsg = string.Format("Unable to load project onto GameServer: {0}", ex.Message);
            }

            return response;
        }

        public static EditResponse AddAllProjects(string gamesDir, string virtualGamesDir = "")
        {
            EditResponse response = new EditResponse("All");

            DirectoryInfo gamesDirectory = new DirectoryInfo(gamesDir);

            foreach (DirectoryInfo gameDir in gamesDirectory.GetDirectories())
            {
                string treePath = "";
                string virtualDir = "";

                try
                {
                    treePath = gameDir.GetFiles("*" + GameObject.TREE_EXT, SearchOption.TopDirectoryOnly).FirstOrDefault().FullName;
                    virtualDir = Path.Combine(virtualGamesDir, gameDir.Name);

                    AddProject(treePath, virtualDir);
                    response.nodesAffected++;
                }
                catch (Exception ex)
                {
                    response.errMsg += string.Format("Error loading game: {0}. {1}", ex.Message +
                        Environment.NewLine + Environment.NewLine);
                }
            }

            return response;
        }

        public static EditResponse CreateNewProject(string projectName)
        {
            EditResponse response = new EditResponse(projectName);
            string gameDir = GameServer.GetGameDir(projectName);
            string templateDir = GameServer.GetProjectTemplateDir();

            //Return failure if this game already exists.
            if (Directory.Exists(gameDir))
            {
                response.errMsg = string.Format("The game: {1} could because a project by that name already exists. Try a different name.", projectName);
                return response;
            }

            //Create new Game.
            GameObject newGame = GameObject.CreateNewProject(projectName, templateDir, gameDir);
            
            //Add to games list.
            games.Add(projectName, newGame);

            //Save game for first time.
            GameServer.SaveProject(projectName, newGame.GetMeta());           
           
            //Set nodes affected to 1 to confirm success.
            response.nodesAffected = 1;

            return response;
        }

        public static EditResponse SaveProject(string gameName, GameMeta meta)
        {
            EditResponse response = new EditResponse(gameName);

            if (!games.ContainsKey(gameName))
            {
                response.errMsg = string.Format("The game: {0}, is not currently loaded onto the GameServer.", gameName);
                return response;
            }

            //Have server do the saving.
            try
            {
                GameObject game = games[gameName];

                //Add GameData (Meta Data)
                game.AddMeta(meta);


                StoryProject gameProj = new StoryProject(game.tree, game.extraData);
                string treePath = game.GetTreePath();
                StoryProject.Serialize(gameProj, treePath);
            }
            catch (Exception ex)
            {
                response.errMsg = string.Format("Unable to save game: {0}. {1}", gameName, ex.Message);
            }


            return response;
        }

        public static EditResponse RemoveGame(string gameName)
        {
            //Untested.
            EditResponse response = new EditResponse(gameName);

            if (games.ContainsKey(gameName))
            {
                games.Remove(gameName);
            }
            else
            {
                response.errMsg = string.Format("No such game ( {0} ) exists to remove from the GameServer", gameName);
            }

            return response;
        }

        #endregion

        #region GameServer Methods

        /// <summary>
        /// Set the default tempalte dir for new projects.
        /// </summary>
        /// <param name="newTemplateDir"></param>
        /// <returns>Returns "" if successful</returns>
        public static string SetTemplateDir(string newTemplateDir)
        {
            templateDir = newTemplateDir;
            return "";
        }

        public static string GetProjectTemplateDir()
        {
            return templateDir;
        }

        #endregion

        #region Get

        /// <summary>
        /// Interfaces MVC to the collection of Games
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Presentation GetPresentation(PresentationRequest request)
        {
            string errCode = ""; //temp presentation in case of errors.
            Presentation presentation = new Presentation(request.gameName, null, "");

            //Foresee errors.
            errCode = PresentationRequest.ValidateRequest(games, request);
            if (errCode != "")
            {
                presentation.errMsg = errCode;
                return presentation;
            }
            
            //Select Game.
            GameObject game = games[request.gameName];
            presentation = game.GetPresentation(request);

            return presentation;
        }
        public static Presentation GetPresentation(string gameName, string playerID, bool newGame = false)
        {
            PresentationRequest request = null;
            
            if (newGame) //Start a new game.
                request = new PresentationRequest(gameName, playerID, newGame);
            else
                request = new PresentationRequest(gameName, playerID, ""); //Empty button = re-display scene (ignores script.)
            
            return GetPresentation(request);
        }

        public static Presentation GetPresentation(string gameName, string playerID, string btnID)
        {
            PresentationRequest request = new PresentationRequest(gameName, playerID, btnID);
            return GetPresentation(request);
        }

        public static PlayerData GetPlayerData(string gameID, string playerID)
        {
            PlayerData playerData = null;

            //Return Player Data.
            if (games.ContainsKey(gameID))
            {
                if (games[gameID].playerData.ContainsKey(playerID))
                {   //Return data.
                    playerData = games[gameID].playerData[playerID];
                }
                else
                {   //Create new Player Data.
                    playerData = new PlayerData(playerID);
                    games[gameID].playerData.Add(playerID, playerData);
                }
            }
       
            return playerData;
        }

        public static StoryTree GetTree(string gameID)
        {
            return games[gameID].tree;
        }

        public static DataNode GetNode(string gameID, string nodeID)
        {
            throw new NotImplementedException();
        }

        public static GameMeta GetGameMeta(string gameID, bool fullPath = false)
        {
            if (games.ContainsKey(gameID))
            {
                return games[gameID].GetMeta(fullPath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of GameMeta objects for each game loaded onto the GameServer.
        /// </summary>
        /// <returns></returns>
        public static List<GameMeta> GetGameMetas(bool fullPath = false)
        {
            List<GameMeta> metas = new List<GameMeta>();

            foreach (string key in games.Keys)
            {
                GameMeta meta = GetGameMeta(key, fullPath);
                metas.Add(meta);
            }

            return metas;
        }

        public static string GetGameDir(string gameName)
        {
            if (games.ContainsKey(gameName))
            {
                return games[gameName].GetResourceFolder();
            }
            else //Game does not exist yet.
            {
                return Path.Combine(gamesDir, gameName);
            }
        }

        public static string GetItemImgDir(string gameName)
        {
            if (games.ContainsKey(gameName))
                return games[gameName].GetItemImgDir();
            else
                return Path.Combine(gamesDir,  gameName, GameObject.ITEM_IMG_FOLDER);
        }

        public static string GetBGImageDir(string gameName)
        {
            if (games.ContainsKey(gameName))
                return games[gameName].GetBGImageDir();
            else
                return Path.Combine(gamesDir,  gameName, GameObject.BG_IMG_FOLDER);
        }

        public static string GetSoundDir(string gameName)
        {
            if (games.ContainsKey(gameName))
                return games[gameName].GetSoundDir();
            else
                return Path.Combine(gamesDir, gameName, GameObject.SOUND_FOLDER);
        }

        public static string GetTreePath(string gameName)
        {
            if (games.ContainsKey(gameName))
                return games[gameName].GetTreePath();
            else
                return Path.Combine(gamesDir, gameName, GameObject.TREE_EXT);
        }

        #endregion

        #region Post

        public static EditResponse PostEditRequest(string gameName, EditRequest request)
        {
            EditResponse response = null;

            if (games.ContainsKey(gameName))
            {
                response = games[gameName].ReceiveEditRequest(request);
            }
            else
            {
                response = new EditResponse(gameName);
                response.errMsg = string.Format("The game: {0} is not currently loaded onto the GameServer.", gameName);
            }
            
            return response;
        }

        public static EditResponse PostEditRequest(string gameName, RequestType reqType, DataNode targetNode)
        {
            EditRequest request = new EditRequest(reqType, targetNode);
            return PostEditRequest(gameName, request);
        }

        #endregion

        #region Player Edit 

        public static EditResponse MovePlayerToID(string gameName, string playerID, string targetID)
        {
            EditResponse response = new EditResponse(gameName);
            PlayerData player = GetPlayerData(gameName, playerID);

            if (player == null)
            {
                response.errMsg = string.Format("Player with ID {0} not found.", playerID);
                return response;
            }

            player.currentPos = targetID;

            return response;
        }


        #endregion
    }
}

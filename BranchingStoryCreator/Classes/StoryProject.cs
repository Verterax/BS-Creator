using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace BranchingStoryCreator.Web
{
    [Serializable]
    public class StoryProject
    {
        #region Consts

        public static string PROJECT_EXT = ".tree";

        #endregion

        #region Serializable Variables
        public StoryTree storyTree { get; set; }
        public ExtraData storyData { get; set; }
        #endregion

        #region Load / Init

        public StoryProject()
        {
            //required parameterless constructor for serialization. Do no use.
        }

        public StoryProject(string xmlPath)
        {
            StoryProject loadedProject = StoryProject.Deserialize(xmlPath);
            Init(loadedProject.storyTree, loadedProject.storyData);                  
        }

        public StoryProject(StoryTree storyTree, ExtraData storyData)
        {
            Init(storyTree, storyData);
        }

        private void Init(StoryTree storyTree, ExtraData storyData)
        {
            this.storyTree = storyTree;
            this.storyData = storyData;
        }

        #endregion

        #region Serialization

        public static bool Serialize(StoryProject storyProject, string saveTo)
        {
            if (storyProject == null)
                return false;

            //Run preserialization on StoryProject
            storyProject.storyData.PreSerialize();

            Type type = typeof(StoryProject);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.NewLineHandling = NewLineHandling.Entitize;

            XmlSerializer serializer = new XmlSerializer(type);
            using (XmlWriter writer = XmlWriter.Create(saveTo, settings))
            {
                serializer.Serialize(writer, storyProject);
            }

            return true;
        }

        public static StoryProject Deserialize(string xmlPath)
        {
            if (!File.Exists(xmlPath))
                return null;

            XmlSerializer deserializer = new XmlSerializer(typeof(StoryProject));
            StoryProject storyProject = null;

            using (FileStream stream = new FileStream(xmlPath, FileMode.Open))
            {
                object obj = deserializer.Deserialize(stream);
                storyProject = obj as StoryProject;
            }

            return storyProject;
        }
        #endregion

        #region Supported Formats

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

    /// <summary>
    /// A dictionary class capable of expanding to include new variables, without breaking serialization.
    /// </summary>
    [Serializable]
    public class ExtraData
    {
        #region Key Constants

        public static string PROJECT_NAME = "projectname";
        public static string AUTHOR = "author";
        public static string ABOUT = "about";
        public static string GAME_IMG = "gameimg";

        #endregion

        #region Serializable Variables
        public List<StoryDataItem> storyData { get; set; }
        #endregion

        #region Non-Serializable Variables

        public string projectName 
        {
            get { return this[ExtraData.PROJECT_NAME]; }
            set { this[ExtraData.PROJECT_NAME] = value; }
        }

        public string author
        {
            get { return this[ExtraData.AUTHOR]; }
            set { this[ExtraData.AUTHOR] = value; }
        }

        public string about
        {
            get { return this[ExtraData.ABOUT]; }
            set { this[ExtraData.ABOUT] = value; }
        }

        public string gameImg
        {
            get { return this[ExtraData.GAME_IMG]; }
            set { this[ExtraData.GAME_IMG] = value; }
        }

        #endregion

        #region Private Variables
        private Dictionary<string, string> _projectDictionary { get; set; }
        #endregion

        #region Load / Init

        public ExtraData()
        {
            Init();
        }

        private void Init()
        {
            this.storyData = new List<StoryDataItem>();
        }

        public static ExtraData GetNewStoryData()
        {
            ExtraData newData = new ExtraData();
            return newData;
        }

        #endregion

        #region Dictionary Access

        public string this[string key]
        {
            get
            {
                EnsureDictionary();

                if (_projectDictionary.ContainsKey(key))
                    return _projectDictionary[key];
                else
                    return "";
            }
            set
            {
                EnsureDictionary();

                if (_projectDictionary.ContainsKey(key))
                    _projectDictionary[key] = value;
                else
                    _projectDictionary.Add(key, value);

            }

            
        }

        private void EnsureDictionary()
        {
            if (_projectDictionary == null)
                _projectDictionary = new Dictionary<string, string>();
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Run this before serializing to populate the List of StoryDataItems.
        /// </summary>
        public void PreSerialize()
        {
            this.storyData.Clear();

            foreach (string key in _projectDictionary.Keys)
            {
                StoryDataItem item = new StoryDataItem(key, this[key]);
                storyData.Add(item);
            }
        }

        #endregion

    }

    [Serializable]
    public class StoryDataItem
    {
        #region Serializable Variables
        public string key {get; set;}
        public string value {get; set;}
        #endregion

        #region Load / Init
        public StoryDataItem(string key, string value)
        {
            Init(key, value);
        }

        public StoryDataItem()
        {
            Init("","");
        }

        private void Init(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        #endregion

    }


}

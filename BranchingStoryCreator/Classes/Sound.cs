using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace BranchingStoryCreator
{
    [ComVisible(true)]
    public class Sound
    {
        #region Variables
        private WindowsMediaPlayer Player1 { get; set; }
        private WindowsMediaPlayer Player2 { get; set; }
        private WindowsMediaPlayer Player3 { get; set; }
        private WindowsMediaPlayer Player4 { get; set; }
        private List<WindowsMediaPlayer> players { get; set; }

        private bool _soundEnabled { get; set; }
        public bool SoundEnabled 
        {
            get { return _soundEnabled; }
            set 
            {
                _soundEnabled = value;

                if (!value) //Stop playing.
                    StopAll();               
            }
        }


        private string SoundDir;
        #endregion
        #region Consts
        public static string SOUND_MISSING = "sound_missing.mp3";
        #endregion
        #region Init / Constructor
        public Sound(string soundDir)
        {
            Init(soundDir);
        }

        private void Init(string soundDir)
        {
            players = new List<WindowsMediaPlayer>();
            Player1 = new WindowsMediaPlayer();
            Player2 = new WindowsMediaPlayer();
            Player3 = new WindowsMediaPlayer();
            Player4 = new WindowsMediaPlayer();

            InitPlayer(Player1);
            InitPlayer(Player2);
            InitPlayer(Player3);
            InitPlayer(Player4);

            SoundDir = soundDir;
            SoundEnabled = true;
        }

        #endregion


        public string this[string soundName]
        {
            get
            {
                if (!SoundEnabled)
                    return "";

                string filePath = SoundDir + soundName + ".mp3";

                if (!System.IO.File.Exists(filePath))
                {
                    filePath = SoundDir + SOUND_MISSING;

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePath = SoundDir + soundName + ".mp3";
                        MessageBox.Show("Unable to find file " + Path.GetFileName(filePath) + " or the missing sound file.");
                        return "";
                    }
                }

                Play(filePath);

                return "";
            }
        }

        private void InitPlayer(WindowsMediaPlayer player)
        {
            if (player == null)
                return;

            player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            player.MediaError += new _WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            players.Add(player);
        }

        private void UninitPlayer(WindowsMediaPlayer player)
        {
            player.PlayStateChange -= Player_PlayStateChange;
            player.MediaError -= Player_MediaError;
            player.close();    
        }

        private void Play(string filePath)
        {
            WindowsMediaPlayer targetPlayer = null;

            foreach(WindowsMediaPlayer player in players)
              if (player.URL == "")
              {
                  targetPlayer = player;
                  break;
              }

            if (targetPlayer == null)
                return;
            
            targetPlayer.URL = filePath;
            targetPlayer.controls.play();
        }

        public void StopAll()
        {
            foreach (WindowsMediaPlayer player in players)
                StopPlayer(player);
        }

        private void StopPlayer(WindowsMediaPlayer player)
        {
            if (player != null)
            {
                player.controls.stop();
                player.URL = "";
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPPlayState)NewState == WMPPlayState.wmppsStopped)
            {
                foreach (WindowsMediaPlayer player in players)
                    if (player.playState == WMPPlayState.wmppsStopped)
                        player.URL = "";                
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            MessageBox.Show("Unable to play " + pMediaObject.ToString());
           // Player1.close();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using WMPLib;
using BranchingStoryCreator.Web;

namespace BranchingStoryCreator.Forms
{
    public class WinSound
    {
        #region Variables
        private WindowsMediaPlayer Player1 { get; set; }
        private WindowsMediaPlayer Player2 { get; set; }
        private WindowsMediaPlayer Player3 { get; set; }
        private WindowsMediaPlayer Player4 { get; set; }
        private WindowsMediaPlayer Player5 { get; set; }
        private WindowsMediaPlayer MusicPlayer { get; set; }
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

        #endregion
        #region Init / Constructor
        public WinSound()
        {
            Init();
        }

        private void Init()
        {
            players = new List<WindowsMediaPlayer>();
            Player1 = new WindowsMediaPlayer();
            Player2 = new WindowsMediaPlayer();
            Player3 = new WindowsMediaPlayer();
            Player4 = new WindowsMediaPlayer();
            Player5 = new WindowsMediaPlayer();
            MusicPlayer = new WindowsMediaPlayer();

            InitPlayer(Player1);
            InitPlayer(Player2);
            InitPlayer(Player3);
            InitPlayer(Player4);
            InitPlayer(Player5);
            InitPlayer(MusicPlayer, true);

            SoundEnabled = true;
        }

        #endregion

        private void InitPlayer(WindowsMediaPlayer player, bool isMusic = false)
        {
            if (player == null)
                return;

            player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            player.MediaError += new _WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);

            if (!isMusic)
                players.Add(player);
        }

        private void UninitPlayer(WindowsMediaPlayer player, bool isMusic = false)
        {
            player.PlayStateChange -= Player_PlayStateChange;
            player.MediaError -= Player_MediaError;

            if (!isMusic)
                players.Remove(player);

            player.close();    
        }

        private void Play(string filePath, string SoundMissing)
        {

            if (!SoundEnabled || filePath == "")
                return;

            WindowsMediaPlayer targetPlayer = null;

            if (filePath != "")
                if (!File.Exists(filePath))
                {
                    if (File.Exists(SoundMissing))
                        filePath = SoundMissing;
                    else
                        return;
                }

            foreach(WindowsMediaPlayer player in players)
              if (player.playState == WMPPlayState.wmppsUndefined ||
                  player.playState == WMPPlayState.wmppsReady)
              {
                  targetPlayer = player;
                  break;
              }

            if (targetPlayer == null)
                return;
            
            targetPlayer.URL = filePath;
            targetPlayer.controls.play();
        }

        public void PlayMusic(string filePath, string SoundMissing)
        {
            if (!SoundEnabled || filePath == "")
                return;

            if (filePath != "")
                if (!File.Exists(filePath))
                {
                    if (File.Exists(SoundMissing))
                        filePath = SoundMissing;
                    else
                        return;
                }

            if (MusicPlayer.playState == WMPPlayState.wmppsPlaying)
                StopPlayer(MusicPlayer);

            MusicPlayer.URL = filePath;
            MusicPlayer.controls.play();

        }

        public void StopAll()
        {
            foreach (WindowsMediaPlayer player in players)
            {
                StopPlayer(player);
            }

            StopPlayer(MusicPlayer);
        }

        private void StopPlayer(WindowsMediaPlayer player)
        {
            if (player != null)
            {
                player.controls.stop();
                player.URL = "";
            }
        }

        public void HandleSound(PresentSound playThis)
        {
            if (playThis.stopAll)
                StopAll();

            string sndMissing = playThis.soundMissing;

            PlayMusic(playThis.musicFile,  sndMissing);
            Play(playThis.soundFile0, sndMissing);
            Play(playThis.soundFile1, sndMissing);
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

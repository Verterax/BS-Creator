using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace BranchingStoryCreator
{
    [ComVisible(true)]
    public class Sound
    {
        #region Variables
        private WMPLib.WindowsMediaPlayer Player;
        private string SoundDir;
        #endregion
        #region Init / Constructor
        public Sound(string soundDir)
        {
            Init(soundDir);
        }

        private void Init(string soundDir)
        {
            Player = null;
            SoundDir = soundDir;
        }

        #endregion

        public string this[string soundName]
        {
            get
            {
                string filePath = SoundDir + soundName + ".mp3";

                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show("Unable to find file " + Path.GetFileName(filePath));
                    return "";
                }

                Player = new WMPLib.WindowsMediaPlayer();
                Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
                Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);

                Player.URL = filePath;
                Player.controls.play();

                return "";
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                Player.close();
        }

        private void Player_MediaError(object pMediaObject)
        {
            MessageBox.Show("Unable to play " + pMediaObject.ToString());
            Player.close();
        }

    }
}

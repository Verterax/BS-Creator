using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;
using System.IO;
using BranchingStoryCreator.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace BranchingStoryCreator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {           
            //Application.EnableVisualStyles();
            EnsureFileStructure();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        //dlls
        //Interop_MSScriptControl
        //Interop_WMPLib
        //VerticalProgressBar

        //Sounds
        //sound_missing.mp3

        //images
        //bg_missing
        //item_missing

        static void EnsureFileStructure()
        {
            //copy all files to working path.
            string currentDir = Application.StartupPath;
            string thisFile = "";
            
            //Script DLL
            thisFile = Path.Combine(currentDir, "Interop.MSScriptControl.dll");
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, Properties.Resources.Interop_MSScriptControl);

            //Windows Media Player DLL
            thisFile = Path.Combine(currentDir, "Interop.WMPLib.dll");
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, Properties.Resources.Interop_WMPLib);

            //Vertical ProgressBar DLL
            thisFile = Path.Combine(currentDir, "VerticalProgressBar.dll");
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, Properties.Resources.VerticalProgressBar);

            //Background Image Missing jpg.
            thisFile = Path.Combine(currentDir, GameObject.BG_IMG_FOLDER, GameObject.BG_MISSING);
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, ImgToByte(Properties.Resources.bg_missing, ImageFormat.Jpeg));

            //Item Image Missing png
            thisFile = Path.Combine(currentDir, GameObject.ITEM_IMG_FOLDER, GameObject.ITEM_MISSING);
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, ImgToByte(Properties.Resources.item_missing, ImageFormat.Png));

            //Sound missing mp3
            thisFile = Path.Combine(currentDir, GameObject.SOUND_FOLDER, GameObject.SOUND_MISSING);
            if (!File.Exists(thisFile))
                CopyLocal(thisFile, Properties.Resources.sound_missing);
            
            //Create project directory in user documents folder
            EnsureProjectFolder();

            
        }

        static void EnsureProjectFolder()
        {
            string projectFolder = GameObject.GetWindowsProjectFolder();

            if (!Directory.Exists(projectFolder))
                Directory.CreateDirectory(projectFolder);

        }

        static byte[] ImgToByte(Image img, ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, format);
            return ms.ToArray();
        }

        static void CopyLocal(string toPath, byte[] fileBytes)
        {
            try
            {
                string dir = Path.GetDirectoryName(toPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (FileStream fileStream = new FileStream(toPath, FileMode.Create))
                {
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        fileStream.WriteByte(fileBytes[i]);
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to copy required file. {0}. {1). Shutting Bessy down. She's only suffering.", toPath, ex.Message),
                    "What? I don't even.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }



    }
}

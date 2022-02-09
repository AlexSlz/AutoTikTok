using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTikTok
{

    internal class Program
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            bool ffmpeg = File.Exists(Path.Combine(path,"ffmpeg.exe"));
            if (ffmpeg)
            {
                ConsoleManipulation console = new ConsoleManipulation();
                //Load Folsers Name
                Folders[] folders = (new Settings(path, "FoldersName")).LoadFolders();
                //CreateMain Folders
                foreach (Folders folder in folders)
                {
                    Directory.CreateDirectory(Path.Combine(path, "data", folder.Name));
                }
                VideoManager video = new VideoManager(Path.Combine(path, "data"), folders, new Settings(path, "Video"));
                Menu menu = new Menu(path, video);
                menu.Draw(true);
            }
            else
            {
                Console.WriteLine("Download FFMPEG!");
            }
        }
    }
}

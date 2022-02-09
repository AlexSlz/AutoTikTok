using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTikTok
{
    public class Folders
    {
        public string Name;
        public enum Types
        {
            Image,BackGround,outPut,CustomMainFolder
        }
        public Types Type;

        public Folders(string name, Types type)
        {
            Name = name;
            Type = type;
        }
    }

    public static class FolderEx
    {
        public static string selectFolder(this Folders[] f, Folders.Types _type)
        {
            return f.ToList().Find(x => x.Type == _type).Name;
        }
    }

    internal class FileManager
    {
        Random r = new Random();
        protected string path;
        protected Folders[] folders;
        public FileManager(string path, Folders[] folders)
        {
            this.path = path;
            this.folders = folders;


            SetUpFolder(folders.selectFolder(Folders.Types.Image));
            SetUpFolder(folders.selectFolder(Folders.Types.BackGround));
        }

        protected string SetUpFolder(string folderName)
        {
            int i = 0;
            Directory.CreateDirectory(Path.Combine(path, folderName, "temp"));
            List<string> allfiles = Directory.GetFiles(Path.Combine(path, folderName), "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in allfiles)
            {
                File.Move(file, Path.Combine(path, folderName, "temp", (++i + "." + Path.GetFileName(file).Split('.')[1])));
            }
            allfiles = Directory.GetFiles(Path.Combine(path, folderName, "temp"), "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in allfiles)
            {
                File.Move(file, Path.Combine(path, folderName, (i-- + "." + Path.GetFileName(file).Split('.')[1])));
            }
            Directory.Delete(Path.Combine(path, folderName, "temp"), true);
            return "OK";
        }

        public string GetRandomFile(Folders.Types types)
        {
            List<string> allfiles = Directory.GetFiles(Path.Combine(path, folders.selectFolder(types)), "*.*", SearchOption.AllDirectories).ToList();
            int randBgIndex = r.Next(allfiles.Count);
            return Path.GetFileName(allfiles[randBgIndex]);
        }
    }
}

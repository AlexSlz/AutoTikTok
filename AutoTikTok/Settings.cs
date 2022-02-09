using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoTikTok
{
    public class Settings
    {
        string path;
        string globalSection;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public Settings(string path, string section)
        {
            this.path = Path.Combine(path, "settings.ini");
            this.globalSection = section;
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? globalSection, Key, "", RetVal, 255, path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? globalSection, Key, Value, path);
        }
        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section ?? globalSection).Length > 0;
        }
        public void Delete(string Key = null, string Section = null)
        {
            Write(null, null, Section ?? globalSection);
        }
    }
    public static class SettingsEx
    {
        public static Folders[] LoadFolders(this Settings settings)
        {
            settings.WriteIfExist("ImageFolder", "Image");
            settings.WriteIfExist("BackGroundFolder", "bgVideo");
            settings.WriteIfExist("outPutVideoFolder", "outPutVideo");
            return new[] { new Folders(settings.Read("ImageFolder"), Folders.Types.Image), new Folders(settings.Read("BackGroundFolder"), Folders.Types.BackGround), new Folders(settings.Read("outPutVideoFolder"), Folders.Types.outPut) };
        }
        static void WriteIfExist(this Settings settings, string name, string value)
        {
            if (!settings.KeyExists(name))
                settings.Write(name, value);
        }
    }


}

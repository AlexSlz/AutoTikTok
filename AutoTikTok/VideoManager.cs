using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AutoTikTok
{
    internal class VideoManager
    {
        protected string path;
        protected Folders[] folders;
        private Random r = new Random();

        protected Settings settings;

        public VideoManager(string path, Folders[] folders, Settings settings)
        {
            this.path = path;
            this.folders = folders;
            this.settings = settings;
            if(!settings.KeyExists("CreateVideoDelayMin"))
                settings.Write("CreateVideoDelayMin", "60");
        }

        public string MakeRandVideo()
        {
            FileManager fileManager = new FileManager(path, folders);
            string videoName = $"video_{DateTime.Now.ToString("dd_MM_HH_mm")}.mp4";
            string imageName = fileManager.GetRandomFile(Folders.Types.Image);
            MakeBatFile(fileManager.GetRandomFile(Folders.Types.BackGround), imageName, videoName);
            ProcessStartInfo file = new ProcessStartInfo();
            file.FileName = Path.Combine(path, "temp.bat");
            file.UseShellExecute = false;

            Process proc = Process.Start(file);
            proc.WaitForExit();
            File.Delete(Path.Combine(path, folders.selectFolder(Folders.Types.Image), imageName));
            settings.Write("NextVideoCreateTime", DateTime.Now.AddMinutes(Int32.Parse(settings.Read("CreateVideoDelayMin"))).ToString());
            Console.Clear();
            return Path.Combine(path, folders.selectFolder(Folders.Types.outPut), videoName);
        }

        void MakeBatFile(string bgFileName, string imageFileName, string outPutFileName)
        {
            using (StreamWriter batFile = new StreamWriter(Path.Combine(path, "temp.bat"), false, Encoding.Default))
            {
                //ih * (9 / 16)
                batFile.WriteLine($"ffmpeg -i {Path.Combine(path, folders.selectFolder(Folders.Types.BackGround), bgFileName)} -vf \"crop=502:ih\" -crf 1 -c:a copy {Path.Combine(path, folders.selectFolder(Folders.Types.outPut), "tempInput.mp4")} -y");
                batFile.WriteLine($"ffmpeg -i {Path.Combine(path, folders.selectFolder(Folders.Types.outPut), "tempInput.mp4")} -i {Path.Combine(path, folders.selectFolder(Folders.Types.Image), imageFileName)} -filter_complex \"[1:v]scale = 502:(ih+502)/2.5[ovrl],[0:v][ovrl]overlay = (main_w - overlay_w) / 2:(main_h - overlay_h) / 2\" -codec:a copy {Path.Combine(path, folders.selectFolder(Folders.Types.outPut), outPutFileName)} -y");
                batFile.WriteLine($"del {Path.Combine(path, folders.selectFolder(Folders.Types.outPut), "tempInput.mp4")}");
                batFile.WriteLine("del %0");
            }
        }

    }
}

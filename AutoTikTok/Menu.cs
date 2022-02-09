using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTikTok
{
    class MenuItem
    {
        public ConsoleKey key;
        public string name;
        public Action action;

        public MenuItem(ConsoleKey key, string name, Action action)
        {
            this.key = key;
            this.name = name;
            this.action = action;
        }

    }

    internal class Menu
    {
        string path;
        VideoManager videoManager;
        MenuItem[] menuItems;
        Settings settings;
        Thread drawMenu;
        TIkTokApi api;
        public Menu(string path, VideoManager video)
        {
            this.path = path;
            api = new TIkTokApi(new Settings(path, "TikTokApi"), true);
            videoManager = video;
            menuItems = new[] { 
                new MenuItem(ConsoleKey.D1,"MakeVideo", new Action(()=> videoManager.MakeRandVideo())), 
                new MenuItem(ConsoleKey.D2, "MakeVideoAndUpload", new Action(() => api.UploadVideo(videoManager.MakeRandVideo()))),
                new MenuItem(ConsoleKey.D3, "Upload", new Action(() => api.UploadVideo(Path.Combine(path, Console.ReadLine()))))};
            this.settings = new Settings(path, "Menu");
        }

        public void Draw(bool s)
        {
            if (s)
            {
                drawMenu = new Thread(CycleDraw);
                drawMenu.Start();
                ReadMenuKey();
            }
            else
            {
                drawMenu.Abort();
            }
        }

        void CycleDraw()
        {
            Console.Clear();
            while (true)
            {
                DrawTime();
                foreach (var item in menuItems)
                {
                    Console.WriteLine($"[{item.key}] {item.name}");
                }
                Thread.Sleep(500);
            }
        }

        void DrawTime()
        {
            string msg = $"Time To next Video: {CalcTime()}";
            Console.SetCursorPosition(0, msg.Length - msg.Length);
            Console.Write(msg);
            Console.WriteLine();
        }

        string CalcTime()
        {
            TimeSpan res = TimeSpan.Zero;
            if (settings.KeyExists("NextVideoCreateTime", "Video"))
            {
                res = DateTime.Parse(settings.Read("NextVideoCreateTime", "Video")) - DateTime.Now;
                if (res <= TimeSpan.Zero) api.UploadVideo(videoManager.MakeRandVideo());
                return (res >= TimeSpan.Zero) ? res.ToString(@"hh\:mm\:ss") : "NOW";
            }
            else
            {
                return "FIRST TIME";
            }
        }

        void ReadMenuKey()
        {
            ConsoleKeyInfo res = Console.ReadKey();
            foreach(MenuItem item in menuItems)
            {
                if(item.key == res.Key)
                {
                    Draw(false);
                    Console.WriteLine();
                    item.action();
                    Draw(true);
                }
            }
            Console.Clear();
            ReadMenuKey();
        }

    }
}

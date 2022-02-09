using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTikTok
{
    internal class ConsoleManipulation
    {
        static NotifyIcon notificationIcon;
        static bool Visible = true;
        public ConsoleManipulation()
        {
            Thread notifyThread = new Thread(delegate () {
                notificationIcon = new NotifyIcon()
                {
                    Icon = new System.Drawing.Icon("e.ico"),
                    Text = Console.Title
                };
                notificationIcon.MouseClick += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    else
                    {
                        Visible = !Visible;
                        SetConsoleWindowVisibility(Visible);
                    }
                };
                notificationIcon.Visible = true;
                Application.Run();
            });
            notifyThread.Start();
        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace TizTaboo
{
    class Log
    {
        public static void Error(string msg)
        {
            File.AppendAllText(Application.StartupPath + "\\log", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " - " + msg + "\n");
        }
    }
}

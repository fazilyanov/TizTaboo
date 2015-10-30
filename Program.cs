﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TizTaboo {
    static class Program {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new MainForm();
            Application.Run();
        }
    }

    static class Data    {        public static faNotes NoteList { get; set; }
    }
}

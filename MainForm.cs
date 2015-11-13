using System;
using System.Data.SqlServerCe;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalHotKey;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace TizTaboo
{

    public partial class MainForm : Form
    {
        private readonly HotKeyManager _hotKeyManager;


        int i = 0;
        int si = 0;
        string _basepath = "";
        public int _height;
        public int _width;

        private void ShowForm()
        {
            this.Show();
            for (int y = 0; y < 100; y++)
            {
                if (this.Location.Y + y > 0)
                {
                    this.Location = new Point(0, 0);
                    break;
                }
                this.Location = new Point(0, this.Location.Y + y);
                Thread.Sleep(7);
            }
        }
        private void HideForm()
        {
            for (int y = 0; y < 100; y++)
            {
                if (this.Location.Y - y < -_height)
                {
                    this.Location = new Point(0, -_height);
                    break;
                }
                this.Location = new Point(0, this.Location.Y - y);
                Thread.Sleep(7);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            try
            {
                _hotKeyManager = new HotKeyManager();
                _hotKeyManager.KeyPressed += HotKeyManagerPressed;
                _hotKeyManager.Register(System.Windows.Input.Key.X, System.Windows.Input.ModifierKeys.Alt);
            }
            catch
            {
                MessageBox.Show("Не удалось зарегистрировать глобальные горячие клавишы! Возможно, приложение уже запущено.", "Ошибка");
                Environment.Exit(-1);
            }

            _basepath = Application.StartupPath + "\\" + "data.bin";

            if (File.Exists(_basepath))
            {
                int _n = int.Parse((Properties.Settings.Default.lastbackup ?? "0").ToString());
                _n = _n < 10 ? _n + 1 : 1;
                File.Copy(_basepath, _basepath + "_" + _n.ToString(), true);
                Properties.Settings.Default.lastbackup = _n.ToString();
                Properties.Settings.Default.Save();
                Data.NoteList = new faNotes(_basepath);
                Data.NoteList.Load();

            }
            else
            {
                var result = MessageBox.Show("База данных не найдена, создать новую?", "TizTaboo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Data.NoteList = new faNotes(_basepath);
                    Data.NoteList.Add(new faNote("Тест", "test", "https://vk.com", "", faType.URL));
                    if (!Data.NoteList.Save())
                    {
                        MessageBox.Show("Ошибка создания базы!");
                        Environment.Exit(-1);
                    }
                }
            }
        }

        void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            tbAlias.Clear();
            this.ShowForm();
            tbAlias.Focus();
            this.Activate();
            this.Focus();
            this.TopLevel = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;
            _width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.Size = new Size(_width, _height);
            this.Location = new Point(0, -_height);
        }

        private void Seek(string q)
        {
            try
            {
                q = q.Trim();
                pnl.Controls.Clear();
                if (q.Length > 0)
                {
                    List<faNote> result = Data.NoteList.Seek(q);
                    i = 0;
                    si = 0;
                    if (result.Count > 0)
                    {
                        foreach (faNote note in result)
                        {
                            Panel panel = new Panel();
                            panel.Name = "subpanel_" + i;
                            panel.Location = new Point(0, (i * 24) + 4);
                            panel.Size = new Size(this.Width, 20);
                            panel.BorderStyle = BorderStyle.None;
                            panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                            panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                            panel.Parent = pnl;
                            panel.Tag = note.Alias;

                            Label lbl = new Label();
                            lbl.Parent = panel;
                            lbl.Name = "label_" + i;
                            lbl.AutoSize = true;
                            lbl.Location = new Point(8, 2);
                            lbl.Text = note.Name;
                            lbl.Font = new Font(lbl.Font.FontFamily, 12);

                            lbl.Visible = true;

                            Label lbl2 = new Label();
                            lbl2.Parent = panel;
                            lbl2.Name = "label2_" + i;
                            lbl2.AutoSize = true;
                            lbl2.Location = new Point(lbl.Width + 4, 2);
                            lbl2.Text = " (" + note.Alias + ")";
                            lbl2.ForeColor = Color.Gray;
                            //lbl2.Font = new Font(lbl.Font, FontStyle.Bold);
                            lbl2.Visible = true;


                            panel.Controls.Add(lbl);
                            pnl.Controls.Add(panel);
                            i++;
                            //if (i == 13)
                            //    break;
                        }
                    }

                    string[] id = { "-1", "-2", "-3", "-4" };
                    string[] txt = { 
                                       ">>> Искать «" + tbAlias.Text + "» в Гугле",
                                       ">>> Перевести «" + tbAlias.Text + "» на английский" ,
                                       ">>> Перевести «" + tbAlias.Text + "» на русский",
                                       ">>> Выполнить «" + tbAlias.Text + "» "};

                    for (int ind = 0; ind < id.Length; ind++)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(this.Width, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.LawnGreen;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = id[ind];

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        lbl.Text = txt[ind];
                        lbl.Font = new Font(lbl.Font.FontFamily, 12);

                        lbl.Visible = true;
                        panel.Controls.Add(lbl);

                        pnl.Controls.Add(panel);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tbAlias_TextChanged(object sender, EventArgs e)
        {
            if (tbAlias.Text.Trim().Contains("`") || tbAlias.Text.Trim().Contains("ё"))
            {
                HideForm();
                SettForm newForm = new SettForm();
                tbAlias.Clear();
                newForm.ShowDialog();
                this.ShowForm();
            }
            else
                Seek(tbAlias.Text);
        }

        private bool Run(string alias, string query)
        {
            bool ret = true;
            try
            {
                switch (alias)
                {
                    case "-1":
                        Process.Start("https://www.google.ru/search?q=" + Uri.EscapeDataString(query));
                        break;
                    case "-2":
                        Process.Start("https://translate.google.ru/#ru/en/" + Uri.EscapeDataString(query));
                        break;
                    case "-3":
                        Process.Start("https://translate.google.ru/#en/ru/" + Uri.EscapeDataString(query));
                        break;
                    case "-4":
                        System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", query + "\n@pause");
                        Process.Start(Application.StartupPath + "\\run.bat");
                        break;
                    default:
                        {
                            faNote note = Data.NoteList.GetNodeByAlias(alias);
                            switch (note.Type)
                            {
                                case faType.None:
                                    break;
                                case faType.URL:
                                case faType.Windows:
                                    Process.Start(note.Command, note.Param);
                                    break;
                                case faType.Batch:
                                    System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", note.Command);
                                    Process.Start(Application.StartupPath + "\\run.bat");
                                    break;
                                case faType.MultiAlias:
                                    string[] cmd = note.Command.Split(';');
                                    foreach (string item in cmd)
                                    {
                                        faNote n = Data.NoteList.GetNodeByAlias(item);
                                        if (n != null)
                                            Process.Start(n.Command, n.Param);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            note.LastExec = DateTime.Now;
                            note.RunCount = note.RunCount > 9999 ? 0 : note.RunCount + 1;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        private void tbAlias_KeyDown(object sender, KeyEventArgs e)
        {
            // Color cl1 = System.Drawing.Color.Black;
            //Color cl2 = System.Drawing.Color.White;
            Color clr;
            if (e.KeyCode == Keys.Down && i > 0 && si < i - 1)
            {
                clr = pnl.Controls["subpanel_" + si].ForeColor;
                pnl.Controls["subpanel_" + si].ForeColor = pnl.Controls["subpanel_" + si].BackColor;
                pnl.Controls["subpanel_" + si].BackColor = clr;
                si++;
                clr = pnl.Controls["subpanel_" + si].ForeColor;
                pnl.Controls["subpanel_" + si].ForeColor = pnl.Controls["subpanel_" + si].BackColor;
                pnl.Controls["subpanel_" + si].BackColor = clr;
            }
            else if (e.KeyCode == Keys.Up && i > 0 && si > 0)
            {
                clr = pnl.Controls["subpanel_" + si].ForeColor;
                pnl.Controls["subpanel_" + si].ForeColor = pnl.Controls["subpanel_" + si].BackColor;
                pnl.Controls["subpanel_" + si].BackColor = clr;
                si--;
                clr = pnl.Controls["subpanel_" + si].ForeColor;
                pnl.Controls["subpanel_" + si].ForeColor = pnl.Controls["subpanel_" + si].BackColor;
                pnl.Controls["subpanel_" + si].BackColor = clr;
            }
            else if (e.KeyCode == Keys.Enter && i > 0)
            {
                this.HideForm();
                Run(pnl.Controls["subpanel_" + si].Tag.ToString(), tbAlias.Text.Trim());
            }
            else if (e.KeyCode == Keys.Escape)
                this.HideForm();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Data.NoteList.Save();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            this.HideForm();
        }

        private void tbAlias_Leave(object sender, EventArgs e)
        {
            this.HideForm();
        }
    }
}

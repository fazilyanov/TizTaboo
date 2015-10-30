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
        faNotes NoteList;

        int i = 0;
        int si = 0;

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            bool badfile = false;

            if (File.Exists(Properties.Settings.Default.basepath))
            {
                int _n = int.Parse((Properties.Settings.Default.lastbackup ?? "0").ToString());
                _n = _n < 10 ? _n + 1 : 1;
                File.Copy(Properties.Settings.Default.basepath, Properties.Settings.Default.basepath + "_" + _n.ToString(), true);
                Properties.Settings.Default.lastbackup = _n.ToString();
                NoteList = new faNotes(Properties.Settings.Default.basepath);
                badfile = !NoteList.Load();
            }
            else badfile = true;

            while (badfile)
            {
                var result = MessageBox.Show("База данных не найдена или имеет неправильный формат, создать новую? Нет - выбрать другой файл", "TizTaboo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.InitialDirectory = Application.StartupPath;
                    ofd.Filter = "bin files|*.bin|all files|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Properties.Settings.Default.basepath = ofd.FileName;
                        NoteList = new faNotes(ofd.FileName);
                        badfile = !NoteList.Load();
                    }
                }
                else if (result == DialogResult.Yes)
                {
                    Properties.Settings.Default.basepath = Application.StartupPath + "\\data.bin";
                    NoteList = new faNotes(Properties.Settings.Default.basepath);
                    NoteList.Add(new faNote("Тест", "test", "https://vk.com", faType.URL));
                    if (!NoteList.Save())
                    {
                        MessageBox.Show("Ошибка создания базы!");
                        return;
                    }
                    else badfile = false;

                }
            }


            Properties.Settings.Default.Save();

            _height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;
            _width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.Size = new Size(_width, _height);
            this.Location = new Point(0, -_height);
        }

        void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            tbAlias.Clear();
            this.ShowForm();
            this.Activate();
            this.Focus();
            tbAlias.Focus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }



        private void Seek(string q)
        {
            try
            {
                q = q.Trim();
                pnl.Controls.Clear();
                if (q.Length > 0)
                {
                    List<faNote> result = NoteList.Seek(q);
                    i = 0;
                    si = 0;
                    if (result.Count > 0)
                    {
                        foreach (faNote note in result)
                        {
                            Panel panel = new Panel();
                            panel.Name = "subpanel_" + i;
                            panel.Location = new Point(0, (i * 24) + 4);
                            panel.Size = new Size(750, 20);
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
                            lbl.Visible = true;

                            panel.Controls.Add(lbl);
                            pnl.Controls.Add(panel);
                            i++;
                            if (i == 13)
                                break;
                        }
                    }

                    if (i < 13)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(750, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = "-1";

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        lbl.Text = ">>> Искать «" + tbAlias.Text + "» в Гугле";
                        lbl.Visible = true;
                        panel.Controls.Add(lbl);

                        pnl.Controls.Add(panel);
                        i++;
                    }
                    if (i < 13)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(750, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = "-2";

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        lbl.Text = ">>> Перевести «" + tbAlias.Text + "» на английский";
                        lbl.Visible = true;
                        panel.Controls.Add(lbl);

                        pnl.Controls.Add(panel);
                        i++;
                    }
                    if (i < 13)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(750, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = "-3";

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        lbl.Text = ">>> Перевести «" + tbAlias.Text + "» на русский";
                        lbl.Visible = true;
                        panel.Controls.Add(lbl);

                        pnl.Controls.Add(panel);
                        i++;
                    }
                    if (i < 13)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(750, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = "-4";

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        lbl.Text = ">>> Выполнить «" + tbAlias.Text + "» ";
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
                            faNote note = NoteList.GetNodeByAlias(alias);
                            switch (note.Type)
                            {
                                case faType.None:
                                    break;
                                case faType.URL:
                                case faType.FileName:
                                    Process.Start(note.Command);
                                    break;
                                case faType.Batch:
                                    System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", note.Command);
                                    Process.Start(Application.StartupPath + "\\run.bat");
                                    break;
                                case faType.MultiAlias:
                                    break;
                                default:
                                    break;
                            }
                            note.LastExec = DateTime.Now;
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
            Color cl1 = System.Drawing.Color.Black;
            Color cl2 = System.Drawing.Color.White;
            if (e.KeyCode == Keys.Down && i > 0 && si < i - 1)
            {
                pnl.Controls["subpanel_" + si].ForeColor = cl2;
                pnl.Controls["subpanel_" + si].BackColor = cl1;
                si++;
                pnl.Controls["subpanel_" + si].ForeColor = cl1;
                pnl.Controls["subpanel_" + si].BackColor = cl2;
            }
            else if (e.KeyCode == Keys.Up && i > 0 && si > 0)
            {
                pnl.Controls["subpanel_" + si].ForeColor = cl2;
                pnl.Controls["subpanel_" + si].BackColor = cl1;
                si--;
                pnl.Controls["subpanel_" + si].ForeColor = cl1;
                pnl.Controls["subpanel_" + si].BackColor = cl2;
            }
            else if (e.KeyCode == Keys.Enter && i > 0)
            {
                if (Run(pnl.Controls["subpanel_" + si].Tag.ToString(), tbAlias.Text.Trim()))
                    this.HideForm();
            }
            else if (e.KeyCode == Keys.Escape)
                this.HideForm();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                NoteList.Save();
            else
            {
                HideForm();
                e.Cancel = true;
            }
        }
    }
}

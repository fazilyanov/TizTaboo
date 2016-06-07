using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TizTaboo
{
    public partial class MainForm : Form
    {
        public int _height;
        public int _width;
        private DateTime lastSaveTime = DateTime.Now;

        private readonly HotKeyManager _hotKeyManager;
        private bool _altF4Pressed;

        private string _basepath = "";
        private int i = 0;
        private int si = 0;

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
                    Data.NoteList.Add(new faNote("Тест", "test", "https://vk.com", "", faType.Ссылка, false, 0));
                    if (!Data.NoteList.Save())
                    {
                        MessageBox.Show("Ошибка создания базы!");
                        Environment.Exit(-1);
                    }
                }
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
                Thread.Sleep(1);
            }
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            tbAlias.Clear();
            this.ShowForm();
            this.TopLevel = true;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            this.HideForm();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_altF4Pressed)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                    e.Cancel = true;
                _altF4Pressed = false;
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;
            _width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 4;
            this.BackColor = tbAlias.BackColor = Color.FromArgb(1, 36, 86);
            this.Size = new Size(_width, _height);
            this.Location = new Point(0, -_height);
        }

        private bool Run(string alias, string query)
        {
            bool ret = true;
            try
            {
                faNote note = Data.NoteList.GetNodeByAlias(alias);
                if (note != null)
                {
                    if (note.Confirm && MessageBox.Show("Точно запустить?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return false;
                    }

                    switch (note.Type)
                    {
                        case faType.Ссылка:
                            Process.Start(note.Command, note.Param);
                            break;

                        case faType.Консоль:
                            System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", note.Command);
                            Process.Start(Application.StartupPath + "\\run.bat");
                            break;

                        case faType.Мульти:
                            string[] cmd = note.Command.Split(';');
                            foreach (string item in cmd)
                            {
                                faNote n = Data.NoteList.GetNodeByAlias(item);
                                if (n != null)
                                {
                                    Process.Start(n.Command, n.Param);
                                    n.LastExec = DateTime.Now;
                                    n.RunCount = n.RunCount > 99999 ? 0 : n.RunCount + 1;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    note.LastExec = DateTime.Now;
                    note.RunCount = note.RunCount > 99999 ? 0 : note.RunCount + 1;
                    if ((DateTime.Now - lastSaveTime).Hours > 3)
                    {
                        Data.NoteList.Save();
                        lastSaveTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        private void Seek(string q)
        {
            try
            {
                q = q.Trim();
                pnl.Controls.Clear();

                List<faNote> result = Data.NoteList.Seek(q);
                i = 0;
                si = 0;
                if (result.Count > 0)
                {
                    foreach (faNote note in result)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(20, (i * 24) + 20);
                        panel.Size = new Size(this.Width - 40, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? Color.FromArgb(1, 36, 86) : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : Color.FromArgb(1, 36, 86);
                        panel.Parent = pnl;
                        panel.Tag = note.Alias;

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(20, 4);
                        lbl.Text = "• " + note.Name;
                        lbl.Font = new Font(lbl.Font.FontFamily, 10);

                        lbl.Visible = true;

                        if (note.Alias.ToLower() != note.Name.ToLower())
                        {
                            Label lbl2 = new Label();
                            lbl2.Parent = panel;
                            lbl2.Name = "label2_" + i;
                            lbl2.AutoSize = true;
                            lbl2.Location = new Point(lbl.Width + 16, 4);
                            lbl2.Text = " (" + note.Alias + ")";
                            lbl2.ForeColor = Color.Gray;
                            lbl2.Visible = true;
                        }
                        panel.Controls.Add(lbl);
                        pnl.Controls.Add(panel);
                        i++;
                        if ((i * 24) + 60 > _height) break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowForm()
        {
            this.Show();
            this.Activate();
            this.Focus();
            tbAlias.Focus();
            Seek("");

            for (int y = 0; y < 100; y = y + 2)
            {
                if (this.Location.Y + y > 0)
                {
                    this.Location = new Point(0, 0);
                    break;
                }
                this.Location = new Point(0, this.Location.Y + y);
                Thread.Sleep(1);
            }
        }

        private void tbAlias_KeyDown(object sender, KeyEventArgs e)
        {
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
            else if (e.KeyCode == Keys.Tab && i > 0)
            {
                if (pnl.Controls["subpanel_" + si] != null)
                {
                    tbAlias.Text = pnl.Controls["subpanel_" + si].Tag.ToString() + " ";
                    tbAlias.SelectionStart = tbAlias.Text.Length - 1;
                    tbAlias.SelectionLength = 0;
                }
            }
            else if (e.KeyCode == Keys.Escape)
                this.HideForm();
        }

        private void tbAlias_Leave(object sender, EventArgs e)
        {
            this.HideForm();
        }

        private void tbAlias_TextChanged(object sender, EventArgs e)
        {
            string text = tbAlias.Text.Trim().ToLower();
            if (text.Contains("`") || text.Contains("ё"))
            {
                HideForm();
                SettForm newForm = new SettForm();
                tbAlias.Clear();
                newForm.ShowDialog();
                this.ShowForm();
            }
            else if (text == "exit" || text == "учше")
            {
                if (MessageBox.Show("Закрыть TizTaboo?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Seek(tbAlias.Text);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
                _altF4Pressed = true;
        }
    }
}
using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TizTaboo
{
    public partial class MainForm : Form
    {
        // Координаты окна
        public int height;

        public int width;
        public int topx;
        public int topy;

        /// <summary>
        /// Время последней записи данных в файл
        /// </summary>
        private DateTime lastSaveTime = DateTime.Now;

        /// <summary>
        ///  Регистрируем гк
        /// </summary>
        private readonly HotKeyManager hotKeyManager;

        private bool altF4Pressed;

        /// <summary>
        /// Путь до файла с данными приложения
        /// </summary>
        private string dataFilePath;

        //
        private int i = 0;

        private int si = 0;

        public MainForm()
        {
            InitializeComponent();
            try
            {
                // Подключаем горячие клавиши
                hotKeyManager = new HotKeyManager();
                hotKeyManager.KeyPressed += HotKeyManagerPressed;
                hotKeyManager.Register(System.Windows.Input.Key.X, System.Windows.Input.ModifierKeys.Alt);
            }
            catch
            {
                MessageBox.Show("Не удалось зарегистрировать глобальные горячие клавишы! Возможно, приложение уже запущено.", "Ошибка");
                Environment.Exit(-1);
            }

            dataFilePath = Application.StartupPath + "\\data.txt";

            if (File.Exists(dataFilePath))
            {
                Program.Links = new Links(dataFilePath);
                Program.Links.Load();
            }
            else
            {
                var result = MessageBox.Show("Файл данных не найден, создать новый?", "TizTaboo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Program.Links = new Links(dataFilePath);
                    Program.Links.Add(new Link() { Name = "Яндекс", Alias = "ya", Command = "https://ya.ru/", Type = LinkType.Ссылка });
                    if (!Program.Links.Save(false))
                    {
                        MessageBox.Show("Ошибка создания базы!");
                        Environment.Exit(-1);
                    }
                }
                else Environment.Exit(-1);
            }
        }

        /// <summary>
        /// При нажатии на гк для вызова окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            tbAlias.Clear();
            ShowForm();
            TopLevel = true;
        }

        /// <summary>
        /// Показываем форму
        /// </summary>
        private void ShowForm()
        {
            Show();
            Activate();
            Focus();
            tbAlias.Focus();
            Seek(string.Empty);
            Location = new Point(topx, topy);
        }

        /// <summary>
        /// Скрываем форму
        /// </summary>
        private void HideForm()
        {
            Location = new Point(0, -height);
        }

        /// <summary>
        /// Скрывваем окно при пропадании фокуса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            HideForm();
        }

        /// <summary>
        /// Не даем закрыть окно стандартными способами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (altF4Pressed)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                    e.Cancel = true;
                altF4Pressed = false;
            }
        }

        /// <summary>
        /// Загрузка формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            height = Screen.PrimaryScreen.Bounds.Height / 3;
            width = Screen.PrimaryScreen.Bounds.Width / 4;
            topx = Screen.PrimaryScreen.Bounds.Width / 2 - width / 2;
            topy = height;
            BackColor = tbAlias.BackColor = Color.FromArgb(1, 36, 86);
            Size = new Size(width, height);
            Location = new Point(topx, topy);
            ShowForm();
        }

        /// <summary>
        /// Отлавливаем нажатие клавиш Alt + F4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
                altF4Pressed = true;
        }

        /// <summary>
        /// Запускаем ссылку
        /// </summary>
        /// <param name="alias">Алиас ссылки</param>
        /// <returns></returns>
        private bool Run(string alias)
        {
            bool ret = true;
            try
            {
                // Ищем ссылку по алиасу
                Link link = Program.Links.GetByAlias(alias);
                if (link != null)
                {
                    if (link.Confirm && MessageBox.Show("Точно запустить?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return false;
                    }

                    switch (link.Type)
                    {
                        case LinkType.Ссылка:
                            Process.Start(link.Command, link.Param);
                            break;

                        case LinkType.Консоль:
                            File.WriteAllText(Application.StartupPath + "\\run.bat", link.Command);
                            Process.Start(Application.StartupPath + "\\run.bat");
                            break;

                        case LinkType.Мульти:
                            string[] cmd = link.Command.Split(';');
                            foreach (string item in cmd)
                            {
                                Link n = Program.Links.GetByAlias(item);
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
                    link.LastExec = DateTime.Now;
                    link.RunCount = link.RunCount > 99999 ? 0 : link.RunCount + 1;
                    // Записываем в файл не чаще, чем каждые 3 часа
                    if ((DateTime.Now - lastSaveTime).Hours > 3)
                    {
                        Program.Links.Save(false);
                        lastSaveTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                Log.Error("#201706021558: " + ex.Message);
                MessageBox.Show("Ошибка");
            }
            return ret;
        }

        /// <summary>
        /// Поиск по ссылкам
        /// </summary>
        /// <param name="q">Запрос</param>
        private void Seek(string q)
        {
            try
            {
                q = q.Trim();
                List<Link> foundLinks = Program.Links.Seek(q);
                i = 0;
                si = 0;
                if (foundLinks.Count > 0)
                {
                    // Расставляем полученные элементы
                    pnl.Controls.Clear();
                    foreach (Link link in foundLinks)
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(20, (i * 24) + 20);
                        panel.Size = new Size(this.Width - 40, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? Color.FromArgb(1, 36, 86) : Color.White;
                        panel.BackColor = (i == 0) ? Color.White : Color.FromArgb(1, 36, 86);
                        panel.Parent = pnl;
                        panel.Tag = link.Alias;

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(20, 4);
                        lbl.Text = "• " + link.Name;
                        lbl.Font = new Font(lbl.Font.FontFamily, 10);

                        lbl.Visible = true;

                        if (link.Alias.ToLower() != link.Name.ToLower())
                        {
                            Label lbl2 = new Label();
                            lbl2.Parent = panel;
                            lbl2.Name = "label2_" + i;
                            lbl2.AutoSize = true;
                            lbl2.Location = new Point(lbl.Width + 16, 4);
                            lbl2.Text = " (" + link.Alias + ")";
                            lbl2.ForeColor = Color.Gray;
                            lbl2.Visible = true;
                        }
                        panel.Controls.Add(lbl);
                        pnl.Controls.Add(panel);
                        i++;
                        if ((i * 24) + 60 > height) break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("#201706021601: " + ex.Message);
                MessageBox.Show("Ошибка");
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
                HideForm();
                Run(pnl.Controls["subpanel_" + si].Tag.ToString());
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
                HideForm();
        }

        private void tbAlias_Leave(object sender, EventArgs e)
        {
            HideForm();
        }

        private void tbAlias_TextChanged(object sender, EventArgs e)
        {
            string text = tbAlias.Text.Trim().ToLower();
            if (text.Contains("`") || text.Contains("ё"))
            {
                HideForm();
                LinksForm newForm = new LinksForm();
                tbAlias.Clear();
                newForm.ShowDialog();
                ShowForm();
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
    }
}
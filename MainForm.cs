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

namespace TizTaboo
{

    public partial class MainForm : Form
    {

        const string English = "qwertyuiop[]asdfghjkl;'zxcvbnm,.";
        public string Russian = "йцукенгшщзхъфывапролджэячсмитьбю";
        private readonly HotKeyManager _hotKeyManager;
        int i = 0;
        int si = 0;
        public MainForm()
        {
            InitializeComponent();
            try
            {
                _hotKeyManager = new HotKeyManager();
                _hotKeyManager.KeyPressed += HotKeyManagerPressed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            this.Show();
            //InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            tbAlias.Clear();
            //RefreshData("");
            this.Activate();
            this.Focus();
            tbAlias.Focus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.basepath))
            {
                Properties.Settings.Default.constr = String.Format("Data Source={0}", Properties.Settings.Default.basepath);
                _hotKeyManager.Register(System.Windows.Input.Key.X, System.Windows.Input.ModifierKeys.Alt);
                int _n = int.Parse((Properties.Settings.Default.lastbackup ?? "0").ToString());
                _n = _n < 10 ? _n + 1 : 1;

                File.Copy(Properties.Settings.Default.basepath, Properties.Settings.Default.basepath + "_" + _n.ToString(), true);
                Properties.Settings.Default.lastbackup = _n.ToString();
            }
            else
            {
                MessageBox.Show("База данных не найдена!");
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "sdf files|*.sdf|all files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.basepath = ofd.FileName;
                    Properties.Settings.Default.constr = String.Format("Data Source={0}", Properties.Settings.Default.basepath);

                }
            }
            Properties.Settings.Default.Save();
        }

        private string ConvertEngToRus(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = English.IndexOf(symbol)) != -1 ? Russian[index] : symbol);
            return result.ToString();
        }
        private string ConvertRusToEng(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = Russian.IndexOf(symbol)) != -1 ? English[index] : symbol);
            return result.ToString();
        }

        private void Seek(string q)
        {
            SqlCeConnection conn = new SqlCeConnection(Properties.Settings.Default.constr);
            try
            {
                q = q.Trim();
                pnl.Controls.Clear();
                if (q.Length > 0)
                {
                    string qr = ConvertEngToRus(q).Replace("'", "''");
                    string qe = ConvertRusToEng(q).Replace("'", "''");

                    SqlCeCommand cmd = new SqlCeCommand("", conn);
                    conn.Open();

                    cmd.CommandText =
                    "SELECT a.id, a.type, a.text, a.link, a.alias " +
                    "FROM Notes AS a " +
                    "WHERE (a.alias LIKE '%" + qr + "%') OR (a.alias LIKE '%" + qe + "%') " +
                    "OR (a.text LIKE '%" + qr + "%') OR (a.text LIKE '%" + qe + "%') " +
                    "ORDER BY a.[when] DESC, a.count DESC ";

                    SqlCeDataReader rdr = cmd.ExecuteReader();

                    i = 0;
                    si = 0;
                    while (rdr.Read())
                    {
                        Panel panel = new Panel();
                        panel.Name = "subpanel_" + i;
                        panel.Location = new Point(0, (i * 24) + 4);
                        panel.Size = new Size(750, 20);
                        panel.BorderStyle = BorderStyle.None;
                        panel.ForeColor = (i == 0) ? System.Drawing.Color.Black : System.Drawing.Color.White;
                        panel.BackColor = (i == 0) ? System.Drawing.Color.White : System.Drawing.Color.Black;
                        panel.Parent = pnl;
                        panel.Tag = rdr["id"].ToString();

                        Label lbl = new Label();
                        lbl.Parent = panel;
                        lbl.Name = "label_" + i;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(8, 2);
                        string[] aliases = rdr["alias"].ToString().Split(';');
                        lbl.Text = aliases[0];

                        string _buf = "";
                        if (rdr["text"].ToString().Length > 0)
                            _buf = "  (" + rdr["text"].ToString() + ")";
                        else
                            _buf = "  (" + rdr["link"].ToString() + ")";
                        //
                        if (_buf.Length > 85) _buf = _buf.Remove(84) + "..)";
                        lbl.Text += _buf;
                        lbl.Visible = true;
                        panel.Controls.Add(lbl);

                        pnl.Controls.Add(panel);
                        i++;
                        if (i == 13)
                            break;
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
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void tbAlias_TextChanged(object sender, EventArgs e)
        {
            if (tbAlias.Text.Trim().Contains("`") || tbAlias.Text.Trim().Contains("ё"))
            {
                SettForm newForm = new SettForm();
                tbAlias.Clear();
                this.Hide();
                newForm.ShowDialog();
                this.Show();
            }
            else
                Seek(tbAlias.Text);
        }


        private bool Run(string id, string alias)
        {
            bool ret = true;

            SqlCeConnection conn = new SqlCeConnection(Properties.Settings.Default.constr);
            try
            {
                switch (id)
                {
                    case "-1":
                        Process.Start("https://www.google.ru/search?q=" + Uri.EscapeDataString(alias));
                        break;
                    case "-2":
                        Process.Start("https://translate.google.ru/#ru/en/" + Uri.EscapeDataString(alias));
                        break;
                    case "-3":
                        Process.Start("https://translate.google.ru/#en/ru/" + Uri.EscapeDataString(alias));
                        break;
                    case "-4":
                        System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", alias+"\n@pause");
                        Process.Start(Application.StartupPath + "\\run.bat");
                        break;
                    default:
                        {
                            SqlCeCommand cmd = new SqlCeCommand("", conn);
                            conn.Open();

                            cmd.CommandText =
                                   "SELECT a.id, a.type, a.text, a.link, a.param, a.count " +
                                   "FROM Notes AS a " +
                                   "WHERE a.id = " + id;

                            SqlCeDataReader rdr = cmd.ExecuteReader();

                            rdr.Read();
                            switch (rdr["type"].ToString())
                            {
                                case "WebLink":
                                case "WinLink":
                                    Process.Start(rdr["link"].ToString(), rdr["param"].ToString());
                                    break;
                                case "cmd":
                                    System.IO.File.WriteAllText(Application.StartupPath + "\\run.bat", rdr["link"].ToString());
                                    Process.Start(Application.StartupPath + "\\run.bat");
                                    break;
                                default:
                                    break;
                            }
                            cmd.CommandText =
                                   "UPDATE Notes SET count = count + 1, [when] = GETDATE() WHERE id = " + id;
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
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
                    this.Hide();
            }
            else if (e.KeyCode == Keys.Escape)
                this.Hide();
        }
    }
}

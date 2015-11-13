using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace TizTaboo
{
    public partial class SettForm : Form
    {
        private bool addmode = true;
        private string curalias = "";
        private string curname = "";
        public SettForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            dgvAll.Rows.Clear();
            Data.NoteList.Items.Sort((a, b) => a.Name.CompareTo(b.Name));
            foreach (faNote note in Data.NoteList.Items)
                dgvAll.Rows.Add(note.Name, note.Alias, note.Type.ToString(), note.Command, note.Param, note.LastExec.ToString(),note.RunCount.ToString());
        }

        private void SettForm_Load(object sender, EventArgs e)
        {
            dgvAll.Columns.Add("name", "Имя");
            dgvAll.Columns.Add("alias", "Алиас");
            dgvAll.Columns.Add("type", "Тип");
            dgvAll.Columns.Add("command", "Путь | Ссылка");
            dgvAll.Columns.Add("param", "Параметр");
            dgvAll.Columns.Add("when", "Последний запуск");
            dgvAll.Columns.Add("count", "Запускалось");

            dgvAll.Columns["name"].Width = 160;
            dgvAll.Columns["alias"].Width = 160;
            dgvAll.Columns["type"].Width = 100;
            dgvAll.Columns["command"].Width = 400;
            dgvAll.Columns["param"].Width = 100;
            dgvAll.Columns["when"].Width = 150;
            dgvAll.Columns["count"].Width = 150;
            cbType.DataSource = Enum.GetValues(typeof(faType));
            LoadData();
            dgvAll.ClearSelection();
        }

        private void dgvAll_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                {
                    faType type = faType.None; ;
                    Enum.TryParse<faType>(dgvAll.Rows[e.RowIndex].Cells["type"].Value.ToString(), out type);
                    cbType.SelectedIndex = (int)type;
                    cbType.Enabled = true;

                    tbName.ReadOnly = false;
                    tbName.Text = curname = dgvAll.Rows[e.RowIndex].Cells["name"].Value.ToString().Trim();

                    tbAlias.ReadOnly = false;
                    tbAlias.Text = curalias = dgvAll.Rows[e.RowIndex].Cells["alias"].Value.ToString().Trim();

                    tbCommand.ReadOnly = false;
                    tbCommand.Text = dgvAll.Rows[e.RowIndex].Cells["command"].Value.ToString();

                    tbParam.ReadOnly = false;
                    tbParam.Text = dgvAll.Rows[e.RowIndex].Cells["param"].Value.ToString();

                    btnSave.Text = "Сохранить";
                    addmode = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            tbName.ReadOnly = false;
            tbName.Text = "";

            tbAlias.ReadOnly = false;
            tbAlias.Text = "";

            cbType.SelectedIndex = 0;
            cbType.Enabled = true;

            tbCommand.ReadOnly = false;
            tbCommand.Text = "";

            tbParam.ReadOnly = false;
            tbParam.Text = "";

            dgvAll.ClearSelection();
            btnSave.Text = "Добавить";
            addmode = true;

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (!addmode && MessageBox.Show("Удалить запись " + curname + "(" + curalias + ")" + "?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (Data.NoteList.DeleteNodeByAlias(tbAlias.Text)) LoadData();
                else MessageBox.Show("Не удалось удалить запись!");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!addmode && MessageBox.Show("Перезаписать " + curname + "(" + curalias + ")" + "?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            if ((tbName.Text = tbName.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет имени", "Ошибка");
                return;
            }

            if ((tbAlias.Text = tbAlias.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет алиаса", "Ошибка");
                return;
            }

            if ((tbCommand.Text = tbCommand.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет команды", "Ошибка");
                return;
            }

            faType type = faType.None; ;
            Enum.TryParse<faType>(cbType.SelectedValue.ToString(), out type);

            if (type == faType.None)
            {
                MessageBox.Show("Не выбран тип", "Ошибка");
                return;
            }

            if (addmode)
                if (Data.NoteList.GetNodeByAlias(tbAlias.Text) == null)
                    Data.NoteList.Add(new faNote(tbName.Text, tbAlias.Text, tbCommand.Text, tbParam.Text, type));
                else
                {
                    MessageBox.Show("C алиасом '" + tbAlias.Text + "' уже есть запись", "Ошибка");
                    return;
                }
            else
            {
                faNote note = Data.NoteList.GetNodeByAlias(curalias);

                if (note.Alias != tbAlias.Text && Data.NoteList.GetNodeByAlias(tbAlias.Text) != null)
                {
                    MessageBox.Show("C алиасом '" + curalias + "' уже есть запись", "Ошибка");
                    return;
                }
                note.Name = tbName.Text;
                note.Alias = tbAlias.Text;
                note.Command = tbCommand.Text;
                note.Param = tbParam.Text;
                note.Type = type;
            }

            Data.NoteList.Save();
            LoadData();
            btnNew_Click(null, null);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.NoteList.Add(new faNote("", "", "", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"Билайн", @"beeline", @"C:\Program Files (x86)\USB-модем Билайн\Huawei\USB-modem Beeline.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Android Studio", @"Android Studio", @"C:\Program Files\Android\Android Studio\bin\studio64.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Справочник", @"Справочник", @"C:\Program Files\Internet Explorer\iexplore.exe https://intranet.stg.ru/PhoneBook.aspx", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"nfkdata", @"nfkdata", @"\\nfk-dfs1.stg.lan\nfkdata\", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Sky-sp1 Webarchive Path", @"Sky-sp1 webarchive", @"\\sky-sp1\webarchiveedit$", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"ArchiveScanFilesSkySp1Path", @"ArchiveScanFilesSkySp1", @"\\SKY-SP-SQL1.STG.LAN\ArchiveScanFiles$", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"SQL BackUp Folder", @"SQL BackUp Folder", @"\\SKY-SP-SQL1\backup$", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Soft", @"Soft", @"\\stg.lan\nfkdata\Soft", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Локальный диск C:\", @"ссс", @"C:\", "", faType.URL));
            Data.NoteList.Add(new faNote(@"iTunes", @"iTunes", @"C:\Program Files (x86)\iTunes\iTunes.exe", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Outlook", @"outlook", @"C:\Program Files (x86)\Microsoft Office\Office15\outlook.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"SQL ManagementStudio", @"sss ", @"C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn\ManagementStudio\Ssms.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"SQL Server Profiler", @"SQL Server Profiler", @"C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn\PROFILER.EXE", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Visual Studio 2013", @"vs2013", @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Mozilla Firefox", @"Mozilla Firefox", @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Notepad++", @"Notepad++", @"C:\Program Files (x86)\Notepad++\notepad++.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Skype", @"Skype", @"C:\Program Files (x86)\Skype\Phone\Skype.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"TeamViewer", @"TeamViewer", @"C:\Program Files (x86)\TeamViewer\TeamViewer.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"uTorrent", @"utorrent", @"C:\Program Files (x86)\uTorrent\uTorrent.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"VirtAssist", @"VirtAssist", @"C:\Program Files (x86)\VirtAssist\VirtAssist.exe", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Internet Expolorer", @"ie8013", @"C:\Program Files\Internet Explorer\iexplore.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Mail.ru Cloud", @"Mail.ru Cloud", @"C:\Users\afazilyanov\AppData\Local\Mail.Ru\Cloud\Cloud.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Калькулятор", @"Калькулятор", @"C:\Windows\System32\calc.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Paint", @"paint", @"C:\Windows\system32\mspaint.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Планировщик заданий", @"task", @"C:\Windows\System32\Taskschd.msc", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Локальный диск D:\", @"ddd", @"D:\", "", faType.URL));
            Data.NoteList.Add(new faNote(@"ArchiveR5", @"ArchiveR5", @"D:\PROG\ArchiveR5\start.lnk", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Нси", @"NSIнси", @"D:\PROG\NSI\startnsi.lnk", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Total Commander", @"Total Commander", @"D:\PROG\Total_Commander_7.50_Final_PowerPack_0.99_Portable\TOTALBatch.EXE", "", faType.URL));
            Data.NoteList.Add(new faNote(@"PROJECT", @"PROJECT", @"D:\PROJECT", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Steam", @"steam", @"D:\STEAM\Steam.exe", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Локальный диск E:\", @"eee", @"E:\", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Корзина", @"trash", @"explorer ::{645FF040-5081-101B-9F08-00AA002F954E}", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"Локальный диск F:\", @"fff", @"F:\", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Spiralographomatic", @"Spiralographomatic", @"http://barrydahlberg.com/stuff/canvas/06/index.html", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Coub", @"coub", @"http://coub.com/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Android Site", @"Android", @"http://developer.alexanderklimov.ru/android/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Servicedesk", @"servicedesk", @"http://ds-helpiis1:9090/default.aspx?type_pages=soe&m=4", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Bootstrap", @"bootstrap", @"http://getbootstrap.com/components/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Glyphicons", @"glyphicons", @"http://glyphicons.com/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"JSbeautifier", @"JSbeautifier", @"http://jsbeautifier.org/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Localhost:7373", @"7373", @"http://localhost:7373/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Phpadmin", @"phpadmin", @"http://nfk-archive.nfk.argus.cis/phpmyadmin/index.php?db=reference&token=7203855728a2bd5fecbf2e126082dffd", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Reference", @"reference", @"http://nfk-archive.nfk.argus.cis/reference/reference.php", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Pikabu", @"pikabu", @"http://pikabu.ru/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Aliexpress", @"aliexpress", @"http://ru.aliexpress.com/ru_home.htm", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Портал СТГ", @"Portal", @"http://sky-sp1.stg.lan/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"sky-sp1.stg.lan:8013", @"8013", @"http://sky-sp1.stg.lan:8013/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"sky-sp1.stg.lan:8050", @"8050", @"http://sky-sp1.stg.lan:8050/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"SqlBeautifiersqlformat", @"SqlBeautifiersqlformat", @"http://sqlformat.org/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Производственный календарь", @"Производственный календарь", @"http://variant52.ru/kalendar/proizvodstvennyj-kalendar-rb-2015.htm", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Wotinfo", @"wotinfo", @"http://wotinfo.net/ru/efficiency?server=RU&playername=foxpro9", "", faType.URL));
            Data.NoteList.Add(new faNote(@"StartAndroid", @"StartAndroid", @"http://www.fandroid.info/videokurs-urokov-po-sozdaniyu-android-prilozheniya-reminder/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Курс Доллара", @"usd", @"http://www.forexpf.ru/chart/usdrub/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Погода", @"Погода", @"http://www.gismeteo.ru/city/daily/11960/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Кинопоиск", @"kinopoisk", @"http://www.kinopoisk.ru/user/1016545/go/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Last FM Profile", @"Last FM Profile", @"http://www.lastfm.ru/user/fazl-13", "", faType.URL));
            Data.NoteList.Add(new faNote(@"JqGrid Site", @"jqgrid", @"http://www.trirand.net/demo/aspnet/webforms/jqgrid/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Гугл диск", @"Drive", @"https://drive.google.com/drive/my-drive", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Feedly", @"Feedly", @"https://feedly.com/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Народная карта", @"Народная карта", @"https://n.maps.yandex.ru/", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Новости", @"news", @"https://news.google.ru", "", faType.URL));
            Data.NoteList.Add(new faNote(@"ВКонтакте", @"vk", @"https://vk.com/feed", "", faType.Windows));
            Data.NoteList.Add(new faNote(@"Поиск Google", @"gg", @"https://www.google.ru/webhp?hl=ru", "", faType.URL));
            Data.NoteList.Add(new faNote(@"YouTube", @"YouTube", @"https://www.youtube.com/feed/subscriptions", "", faType.URL));
            Data.NoteList.Add(new faNote(@"Удаленное подключение", @"Mstsc", @"Mstsc", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"Uninstall", @"Uninstall", @"rundll32.exe shell32.dll,Control_RunDLL appwiz.cpl", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"Спать", @"Shutdown", @"shutdown /h", "", faType.Batch));
            Data.NoteList.Add(new faNote(@"Servicedesk", @"servicedesk", @"start /DC:\Program Files\Internet Explorer\ iexplore.exe https://servicedesk.stg.ru/default.aspx?m=1", "", faType.Batch));

        }
    }
}

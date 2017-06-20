using System;
using System.Windows.Forms;

namespace TizTaboo
{
    public partial class CardForm : Form
    {
        private int openMode = 0;
        private string curAlias;

        public CardForm(int m, string a)
        {
            openMode = m;
            curAlias = a;
            InitializeComponent();
        }

        private void CardForm_Load(object sender, EventArgs e)
        {
            cbType.DataSource = Enum.GetValues(typeof(LinkType));
            switch (openMode)
            {
                // Новая запись
                case 1:
                    tbName.Text =
                    tbAlias.Text =
                    tbCommand.Text =
                    tbParam.Text = string.Empty;
                    chkbConfirm.Checked = tbRunCount.ReadOnly = false;
                    cbType.SelectedIndex = 0;
                    tbRunCount.Text = "0";
                    Text = "Новая запись";
                    break;
                // Редактирование
                case 2:
                    Link link = Program.Links.GetByAlias(curAlias);
                    cbType.SelectedIndex = (int)link.Type - 1;
                    tbName.Text = link.Name;
                    tbAlias.Text = link.Alias;
                    tbCommand.Text = link.Command;
                    tbParam.Text = link.Param;
                    tbRunCount.Value = link.RunCount;
                    chkbConfirm.Checked = link.Confirm;
                    Text = "Изменение записи";
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (openMode == 2 && MessageBox.Show($"Перезаписать {curAlias} ?", "Подтвердите действие", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            if ((tbAlias.Text = tbAlias.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Необходимо указать псевдоним", "Ошибка");
                tbAlias.Focus();
                return;
            }

            if ((tbCommand.Text = tbCommand.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Необходимо указать путь к файлу, URL или команду", "Ошибка");
                tbCommand.Focus();
                return;
            }

            LinkType type = LinkType.Ссылка;
            Enum.TryParse<LinkType>(cbType.SelectedValue.ToString(), out type);

            switch (openMode)
            {
                case 1:
                    if (Program.Links.GetByAlias(tbAlias.Text) == null)
                        Program.Links.Add(new Link()
                        {
                            Name = tbName.Text,
                            Alias = tbAlias.Text,
                            Command = tbCommand.Text,
                            Param = tbParam.Text,
                            Type = type,
                            Confirm = chkbConfirm.Checked,
                            RunCount = (ulong)tbRunCount.Value
                        });
                    else
                    {
                        MessageBox.Show($"C псевдонимом «{tbAlias.Text}» уже есть запись", "Ошибка");
                        tbAlias.Focus();
                        return;
                    }

                    break;

                case 2:
                    Link link = Program.Links.GetByAlias(curAlias);

                    if (link.Alias.Trim() != tbAlias.Text && Program.Links.GetByAlias(tbAlias.Text) != null)
                    {
                        MessageBox.Show($"C псевдонимом «{tbAlias.Text}» уже есть запись", "Ошибка");
                        tbAlias.Focus();
                        return;
                    }
                    link.Name = tbName.Text;
                    link.Alias = tbAlias.Text;
                    link.Command = tbCommand.Text;
                    link.Param = tbParam.Text;
                    link.Type = type;
                    link.Confirm = chkbConfirm.Checked;
                    link.RunCount = (ulong)tbRunCount.Value;
                    break;
            }

            Program.Links.LastEditDateTime = DateTime.Now;
            Program.Links.Save(Properties.Settings.Default.IsSync);
            this.Close();
        }
    }
}
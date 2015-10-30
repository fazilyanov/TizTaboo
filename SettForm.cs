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
        public SettForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            dgvAll.Rows.Clear();
            foreach (faNote note in Data.NoteList.Items)
                dgvAll.Rows.Add(note.Name, note.Alias, note.Type.ToString(), note.Command, note.LastExec.ToString());



        }

        private void SettForm_Load(object sender, EventArgs e)
        {

            dgvAll.Columns.Add("name", "Имя");
            dgvAll.Columns.Add("alias","Алиас");
            dgvAll.Columns.Add("type","Тип");
            dgvAll.Columns.Add("command","Путь | Ссылка");
            dgvAll.Columns.Add("when","Последний запуск");

            dgvAll.Columns["name"].Width = 160;
            dgvAll.Columns["alias"].Width = 160;
            dgvAll.Columns["type"].Width = 100;
            dgvAll.Columns["command"].Width = 200;
            dgvAll.Columns["when"].Width = 150;
            cbType.DataSource = Enum.GetValues(typeof(faType));
            LoadData();
        }

        private void dgvAll_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                {

                    cbType.SelectedIndex = cbType.Items.IndexOf(dgvAll.Rows[e.RowIndex].Cells["type"].Value.ToString());
                    cbType.Enabled = true;

                    tbName.ReadOnly = false;
                    tbName.Text = dgvAll.Rows[e.RowIndex].Cells["name"].Value.ToString().Trim();

                    tbAlias.ReadOnly = false;
                    tbAlias.Text = dgvAll.Rows[e.RowIndex].Cells["alias"].Value.ToString().Trim();

                    tbCommand.ReadOnly = false;
                    tbCommand.Text = dgvAll.Rows[e.RowIndex].Cells["command"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                tbName.Text = "";
                cbType.SelectedIndex = 0;
                cbType.Enabled = true;
                tbAlias.Text = "";
                tbAlias.ReadOnly = false;
                tbCommand.Text = "";
                tbCommand.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (tbName.Text != "" && MessageBox.Show("Удалить запись?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!Data.NoteList.DeleteNodeByAlias(tbAlias.Text)) MessageBox.Show("Не удалось удалить запись!");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbName.Text != "" && MessageBox.Show("Перезаписать текущие?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            if ((tbName.Text = tbName.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет имени");
                return;
            }

            if ((tbAlias.Text = tbAlias.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет алиаса");
                return;
            }

            if ((tbCommand.Text = tbCommand.Text.Trim()).Length == 0)
            {
                MessageBox.Show("Нет команды");
                return;
            }

            faType type;
            Enum.TryParse<faType>(cbType.SelectedValue.ToString(), out type);
            Data.NoteList.Add(new faNote(tbName.Text, tbAlias.Text, tbCommand.Text, type));

            Data.NoteList.Save();
            LoadData();
            btnNew_Click(null, null);

        }
    }
}

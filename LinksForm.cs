using System;
using System.Windows.Forms;

namespace TizTaboo
{
    public partial class LinksForm : Form
    {
        public LinksForm()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            CardForm newForm = new CardForm(1, string.Empty);
            newForm.ShowDialog();
            LoadData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 1)
            {
                CardForm newForm = new CardForm(2, dgv.SelectedRows[0].Cells["alias"].Value.ToString());
                newForm.ShowDialog();
                LoadData();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Удалить запи{(dgv.SelectedRows.Count == 1 ? "сь" : "си")}?", "Подтвердите действие", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dgv.SelectedRows)
                {
                    Program.Links.DeleteByAlias(item.Cells["alias"].Value.ToString());
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            int saveRow = 0;
            string sortedColumnName = string.Empty;
            int sortOrder = 0;
            if (dgv.SortedColumn != null)
            {
                sortedColumnName = dgv.SortedColumn.Name;
                sortOrder = (int)dgv.SortOrder;
                sortOrder = sortOrder > 0 ? sortOrder - 1 : 0;
            }

            int selectedRowIndex = 0;
            if (dgv.Rows.Count > 0)
            {
                selectedRowIndex = dgv.CurrentCell.RowIndex;
                saveRow = dgv.FirstDisplayedCell.RowIndex;
            }
            dgv.Rows.Clear();
            Program.Links.LinkList.Sort((a, b) => a.Name.CompareTo(b.Alias));
            foreach (Link link in Program.Links.LinkList)
                dgv.Rows.Add(link.Alias, link.Name, link.Type.ToString(), link.Command, link.Param, link.LastExec.ToString(), link.RunCount.ToString(), link.Confirm ? "Да" : "Нет");
            if (sortedColumnName.Length > 0)
            {
                dgv.Sort(dgv.Columns[sortedColumnName], (System.ComponentModel.ListSortDirection)sortOrder);
            }
            else
            {
                dgv.Sort(dgv.Columns["alias"], System.ComponentModel.ListSortDirection.Ascending);
            }
            if (saveRow != 0 && saveRow < dgv.Rows.Count)
            {
                dgv.FirstDisplayedScrollingRowIndex = saveRow;
            }
            dgv.ClearSelection();
            if (selectedRowIndex > 0) dgv.Rows[selectedRowIndex].Selected = true;
        }

        private void SettForm_Load(object sender, EventArgs e)
        {
            dgv.Columns.Add("alias", "Псевдоним");
            dgv.Columns.Add("name", "Имя / Описание");

            dgv.Columns.Add("type", "Тип");
            dgv.Columns.Add("command", "URL / Путь");
            dgv.Columns.Add("param", "Параметры");
            dgv.Columns.Add("when", "Последний запуск");
            dgv.Columns.Add("count", "Количество запусков");
            dgv.Columns.Add("confirm", "Подтверждать запуск");

            dgv.Columns["alias"].Width = 160;
            dgv.Columns["name"].Width = 200;
            dgv.Columns["type"].Width = 100;
            dgv.Columns["command"].Width = 400;
            dgv.Columns["param"].Width = 100;
            dgv.Columns["when"].Width = 150;
            dgv.Columns["count"].Width = 150;
            dgv.Columns["confirm"].Width = 150;

            LoadData();
            dgv.ClearSelection();
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = btnDel.Enabled = false;
            btnEdit.Enabled = dgv.SelectedRows.Count == 1;
            btnDel.Enabled = dgv.SelectedRows.Count > 0;
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEdit_Click(sender, null);
        }

        private void buttonPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
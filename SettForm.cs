﻿using System;
using System.Windows.Forms;

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

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (!addmode && MessageBox.Show("Удалить записи?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dgvAll.SelectedRows)
                {
                    Data.NoteList.DeleteNodeByAlias(item.Cells[1].Value.ToString());
                }
                LoadData();
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

            chkbConfirm.Checked = false;

            tbRunCount.ReadOnly = false;
            tbRunCount.Text = "0";

            dgvAll.ClearSelection();
            btnSave.Text = "Добавить";
            addmode = true;
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

            //
            int intRunCount = 0;
            int.TryParse(tbRunCount.Text.Trim(), out intRunCount);

            faType type = faType.Ссылка;
            Enum.TryParse<faType>(cbType.SelectedValue.ToString(), out type);

            if (addmode)
                if (Data.NoteList.GetNodeByAlias(tbAlias.Text) == null)
                    Data.NoteList.Add(new faNote(tbName.Text, tbAlias.Text, tbCommand.Text, tbParam.Text, type, chkbConfirm.Checked, intRunCount));
                else
                {
                    MessageBox.Show("C алиасом '" + tbAlias.Text + "' уже есть запись", "Ошибка");
                    return;
                }
            else
            {
                faNote note = Data.NoteList.GetNodeByAlias(curalias);

                if (note.Alias.Trim() != tbAlias.Text && Data.NoteList.GetNodeByAlias(tbAlias.Text) != null)
                {
                    MessageBox.Show("C алиасом '" + curalias + "' уже есть запись", "Ошибка");
                    return;
                }
                note.Name = tbName.Text;
                note.Alias = tbAlias.Text;
                note.Command = tbCommand.Text;
                note.Param = tbParam.Text;
                note.Type = type;
                note.Confirm = chkbConfirm.Checked;
                note.RunCount = intRunCount;
            }

            Data.NoteList.Save();
            LoadData();
            //btnNew_Click(null, null);
        }

        private void dgvAll_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                {
                    faType type = faType.Ссылка;
                    Enum.TryParse<faType>(dgvAll.Rows[e.RowIndex].Cells["type"].Value.ToString(), out type);
                    cbType.SelectedIndex = (int)type - 1;
                    cbType.Enabled = true;

                    tbName.ReadOnly = false;
                    tbName.Text = curname = dgvAll.Rows[e.RowIndex].Cells["name"].Value.ToString().Trim();

                    tbAlias.ReadOnly = false;
                    tbAlias.Text = curalias = dgvAll.Rows[e.RowIndex].Cells["alias"].Value.ToString().Trim();

                    tbCommand.ReadOnly = false;
                    tbCommand.Text = dgvAll.Rows[e.RowIndex].Cells["command"].Value.ToString();

                    tbParam.ReadOnly = false;
                    tbParam.Text = dgvAll.Rows[e.RowIndex].Cells["param"].Value.ToString();

                    tbRunCount.ReadOnly = false;
                    tbRunCount.Text = dgvAll.Rows[e.RowIndex].Cells["count"].Value.ToString();

                    chkbConfirm.Checked = dgvAll.Rows[e.RowIndex].Cells["confirm"].Value.ToString() == "Да" ? true : false;

                    btnSave.Text = "Сохранить";
                    addmode = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadData()
        {
            int saveRow = 0;
            int selectedRowIndex = 0;
            if (dgvAll.Rows.Count > 0)
            {
                selectedRowIndex = dgvAll.CurrentCell.RowIndex;
                saveRow = dgvAll.FirstDisplayedCell.RowIndex;
            }
            dgvAll.Rows.Clear();
            Data.NoteList.Items.Sort((a, b) => a.Name.CompareTo(b.Alias));
            foreach (faNote note in Data.NoteList.Items)
                dgvAll.Rows.Add(note.Name, note.Alias, note.Type.ToString(), note.Command, note.Param, note.LastExec.ToString(), note.RunCount.ToString(), note.Confirm ? "Да" : "Нет");
            dgvAll.Sort(dgvAll.Columns["alias"], System.ComponentModel.ListSortDirection.Ascending);

            dgvAll.ClearSelection();

            if (saveRow != 0 && saveRow < dgvAll.Rows.Count)
            {
                dgvAll.FirstDisplayedScrollingRowIndex = saveRow;
                dgvAll.Rows[selectedRowIndex].Selected = true;
            }
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
            dgvAll.Columns.Add("confirm", "Предупреждать");

            dgvAll.Columns["name"].Width = 160;
            dgvAll.Columns["alias"].Width = 160;
            dgvAll.Columns["type"].Width = 100;
            dgvAll.Columns["command"].Width = 400;
            dgvAll.Columns["param"].Width = 100;
            dgvAll.Columns["when"].Width = 150;
            dgvAll.Columns["count"].Width = 150;
            dgvAll.Columns["confirm"].Width = 150;

            cbType.DataSource = Enum.GetValues(typeof(faType));

            LoadData();
            dgvAll.ClearSelection();
        }
    }
}
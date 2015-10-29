using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace TizTaboo {
    public partial class SettForm : Form {
        public SettForm() {
            InitializeComponent();
        }

        private DataTable ReLoadData() {
            SqlCeConnection conn = new SqlCeConnection(Properties.Settings.Default.constr);
            try {

                SqlCeCommand cmd = new SqlCeCommand("", conn);
                conn.Open();
                cmd.CommandText = "SELECT a.id,a.alias,a.link, a.type, a.param,a.text,a.[when],a.[count] FROM Notes AS a Order by a.alias";
                DataTable dt = new DataTable();
                SqlCeDataAdapter sqlDataAdapter = new SqlCeDataAdapter(cmd);
                sqlDataAdapter.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                conn.Close();
                return null;
            }
        }

        private void SettForm_Load(object sender, EventArgs e) {
            dgvAll.DataSource = ReLoadData();
            dgvAll.Columns["id"].HeaderText = "ID";
            dgvAll.Columns["alias"].HeaderText = "Алиас";
            dgvAll.Columns["type"].HeaderText = "Тип";
            dgvAll.Columns["link"].HeaderText = "Путь | Ссылка";
            dgvAll.Columns["param"].HeaderText = "Параметры";
            dgvAll.Columns["text"].HeaderText = "Текст";
            dgvAll.Columns["when"].HeaderText = "Последний запуск";
            dgvAll.Columns["count"].HeaderText = "Количество";
            //
            dgvAll.Columns["id"].Width = 50;
            dgvAll.Columns["alias"].Width = 150;
            dgvAll.Columns["type"].Width = 100;
            dgvAll.Columns["link"].Width = 150;
            dgvAll.Columns["param"].Width = 150;
            dgvAll.Columns["text"].Width = 150;
            dgvAll.Columns["when"].Width = 150;
            dgvAll.Columns["count"].Width = 50;
        }

        private void dgvAll_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            try {
                if (e.RowIndex > -1)
                {
                    tbId.Text = dgvAll.Rows[e.RowIndex].Cells["id"].Value.ToString();
                    cbType.SelectedIndex = cbType.Items.IndexOf(dgvAll.Rows[e.RowIndex].Cells["type"].Value.ToString());
                    cbType.Enabled = true;
                    tbAlias.ReadOnly = false;

                    tbAlias.Lines = dgvAll.Rows[e.RowIndex].Cells["alias"].Value.ToString().Trim().Split(';');

                    tbLink.ReadOnly = false;
                    tbLink.Text = dgvAll.Rows[e.RowIndex].Cells["link"].Value.ToString();
                    tbParam.ReadOnly = false;
                    tbParam.Text = dgvAll.Rows[e.RowIndex].Cells["param"].Value.ToString();
                    tbText.ReadOnly = false;
                    tbText.Text = dgvAll.Rows[e.RowIndex].Cells["text"].Value.ToString();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);

            }
        }

        private void btnNew_Click(object sender, EventArgs e) {
            try {
                tbId.Text = "0";
                cbType.SelectedIndex = 0;
                cbType.Enabled = true;
                tbAlias.Text = "";
                tbAlias.ReadOnly = false;
                tbLink.Text = "";
                tbLink.ReadOnly = false;
                tbParam.Text = "";
                tbParam.ReadOnly = false;
                tbText.Text = "";
                tbText.ReadOnly = false;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnDel_Click(object sender, EventArgs e) {
            if (tbId.Text != "0" && MessageBox.Show("Удалить запись?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                SqlCeConnection conn = new SqlCeConnection(Properties.Settings.Default.constr);
                try {
                    SqlCeCommand cmd = new SqlCeCommand("", conn);
                    conn.Open();
                    cmd.CommandText = "Delete FROM Notes Where id = " + tbId.Text;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    conn.Close();
                }
                dgvAll.DataSource = ReLoadData();
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (tbId.Text != "0" && MessageBox.Show("Перезаписать текущие?", "Подтверди", MessageBoxButtons.YesNo) == DialogResult.No) {
                return;
            }
            SqlCeConnection conn = new SqlCeConnection(Properties.Settings.Default.constr);

            if((tbAlias.Text=tbAlias.Text.Trim()).Length==0){
                MessageBox.Show("Нет ни одного алиаса");
                return;
            }

            if ((tbLink.Text = tbLink.Text.Trim()).Length == 0) {
                MessageBox.Show("Нет ссылки или пути");
                return;
            }

            string _al = "";
            foreach (string str in tbAlias.Lines) {
                _al += str + ";";
            }
            //string _lnk = "";
            //if (cbType.Text == "cmd") {
            //    foreach (string str in tbLink.Lines) {
            //        _lnk += str + ";";
            //    }
            //}
            //else {
                
            //}
            string _lnk = tbLink.Text;
            try {
                SqlCeCommand cmd = new SqlCeCommand("", conn);
                conn.Open();
                if (tbId.Text == "0")
                    cmd.CommandText = "Insert into Notes (type,alias,link,param,text,[when],count) values ('" + cbType.Text + "','" + _al + "','" + _lnk + "','" + tbParam.Text + "','" + tbText.Text + "',GETDATE(),0);";
                else
                    cmd.CommandText = "Update Notes SET type='" + cbType.Text + "',alias='" + _al + "',link='" + _lnk + "',param='" + tbParam.Text + "',text='" + tbText.Text + "' Where id=" + tbId.Text;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
            dgvAll.DataSource = ReLoadData();
            btnNew_Click(null, null);

        }
    }
}

using System.Windows.Forms;
using System.Windows.Input;

namespace TizTaboo
{
    public partial class ParamsForm : Form
    {
        public string tmp = string.Empty;
        private Key wpfKey = 0;
        private bool kShift = false;
        private bool kControl = false;
        private bool kAlt = false;

        public ParamsForm()
        {
            InitializeComponent();
        }

        private void tbHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            tbHotKey.Text = tmp;
        }

        private void tbHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            kShift = kAlt = kControl = false;
            tmp = string.Empty;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.Modifiers == Keys.None)
            {
            }
            else
            {
                wpfKey = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);
                if ((int)wpfKey > 33 && (int)wpfKey < 44)
                {
                    tmp = wpfKey.ToString().Replace("D", string.Empty);
                }
                else
                    tmp = wpfKey.ToString();

                if (e.Modifiers != Keys.None)
                {
                    tmp = e.Modifiers.ToString() + " + " + tmp;
                    kShift = e.Shift;
                    kAlt = e.Alt;
                    kControl = e.Control;
                }
            }
            btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (tbHotKey.Text.Length == 0)
            {
                MessageBox.Show("Необходимо указать сочетания клавиш", "Ошибка");
                tbHotKey.Focus();
                return;
            }

            Properties.Settings.Default.hotKeyText = tbHotKey.Text;
            Properties.Settings.Default.hotKey = (int)wpfKey;
            Properties.Settings.Default.Control = kControl;
            Properties.Settings.Default.Shift = kShift;
            Properties.Settings.Default.Alt = kAlt;
            //
            Properties.Settings.Default.IsSync = chbSync.Checked;
            //
            Properties.Settings.Default.Save();
            btnSave.Enabled = false;
            Close();
        }

        private void ParamsForm_Load(object sender, System.EventArgs e)
        {
            tbHotKey.Text = Properties.Settings.Default.hotKeyText;
            chbSync.Checked = Properties.Settings.Default.IsSync;
        }

        private void chbSync_CheckedChanged(object sender, System.EventArgs e)
        {
            btnSave.Enabled = false;
        }
    }
}
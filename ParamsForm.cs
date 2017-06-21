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
            if (e.KeyCode==Keys.ShiftKey|| e.KeyCode == Keys.Menu)
            {
                tmp = string.Empty;
            }
            else
            {
                wpfKey = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);

                if ( e.Modifiers!= Keys.None)
                {
                    tmp = e.Modifiers.ToString() + " + ";
                    kShift = e.Shift;
                    kAlt = e.Alt;
                    kControl = e.Control;
                }
                tmp += e.KeyCode.ToString();
            }
            btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (tbHotKey.Text.Length==0)
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
            Properties.Settings.Default.Save();
            btnSave.Enabled = false;
        }

        private void ParamsForm_Load(object sender, System.EventArgs e)
        {
            tbHotKey.Text = Properties.Settings.Default.hotKeyText;
        }
    }
}
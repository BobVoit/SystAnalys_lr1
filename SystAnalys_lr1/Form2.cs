using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystAnalys_lr1
{
    public partial class CloseForm : Form
    {
        public CloseForm()
        {
            InitializeComponent();
        }

        // Закрыть форму диалогового окна.
        private void CloseApp_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Закрыть форму диалогового окна и перейти в меню.
        private void BackInMainMenu_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        // Закрыть форму диалогового окна и вернуться обратно на форму работы с графом.
        private void CancelClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

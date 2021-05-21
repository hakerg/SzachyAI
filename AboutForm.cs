using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class AboutForm : Form
    {
        MenuForm menuForm;

        public AboutForm(MenuForm menuForm)
        {
            this.menuForm = menuForm;
            InitializeComponent();
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            menuForm.Show();
        }
    }
}

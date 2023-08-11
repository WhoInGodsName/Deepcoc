using Deepcoc.Auth;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deepcoc
{
    public partial class Login : MaterialForm
    {
        public static api KeyAuthApp = new api(
            name: "Nemesis",
            ownerid: "JxxtbMr5Q0",
            secret: "1b35d2d380ed79523c35de34f3963cb650a53d55239a497e47ea8bbf0d7578cb",
            version: "1.0"
        );
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange900, Primary.Orange800, Primary.Red800, Accent.Orange700, TextShade.BLACK);

            KeyAuthApp.init();

            if (KeyAuthApp.checkblack())
            {
                // is blacklisted
            }
            else
            {
                // not blacklisted
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            KeyAuthApp.login(materialMaskedTextBox1.Text, materialMaskedTextBox2.Text);
            if (KeyAuthApp.response.success)
            {
                MessageBox.Show("Success");
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            KeyAuthApp.license(materialMaskedTextBox3.Text );
            if (KeyAuthApp.response.success)
            {
                //MessageBox.Show("Success");
                Form1 app = new Form1();
                app.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }
    }
}

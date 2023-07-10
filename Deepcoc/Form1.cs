using System.Diagnostics;
using System.Threading;

namespace Deepcoc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process game = Process.GetProcessesByName("FSD-Win64-Shipping").First();
            MemoryReader mem = new MemoryReader(game);
            var baseAddress = game.MainModule.BaseAddress;

            IntPtr Primaryaddress = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
            IntPtr Secondaryaddress = mem.ReadAddress(baseAddress, Offsets.SecondaryGun);

            //System.Diagnostics.Debug.WriteLine(address.ToString("X"));


            //System.Diagnostics.Debug.WriteLine(_currentAmmo.ToString("X"));

            Thread LA = new Thread(LockSecondaryAmmo);
            LA.Start();

            Thread LAP = new Thread(LockPrimaryAmmo);
            LAP.Start();

            void LockPrimaryAmmo()
            {
                while (true)
                {

                    if (checkBox3.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(Primaryaddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (checkBox4.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(Primaryaddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                }
            }

            void LockSecondaryAmmo()
            {
                while (true)
                {

                    if (checkBox1.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(Secondaryaddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (checkBox2.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(Secondaryaddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                }
            }


        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
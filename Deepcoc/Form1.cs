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

            IntPtr address = mem.ReadAddress(baseAddress, Offsets.SecondaryGun);

            System.Diagnostics.Debug.WriteLine(address.ToString("X"));

            int _currentAmmo = mem.ReadInt(address, Offsets.currentAmmo);
            System.Diagnostics.Debug.WriteLine(_currentAmmo.ToString("X"));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
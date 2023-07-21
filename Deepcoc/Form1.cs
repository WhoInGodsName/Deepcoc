using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using MaterialSkin;
using MaterialSkin.Controls;
using SoT_Helper.Services;

namespace Deepcoc
{

    public partial class Form1 : MaterialForm
    {

        public IntPtr primaryAddress = IntPtr.Zero;
        public IntPtr secondaryAddress = IntPtr.Zero;

        public IntPtr baseAddress = IntPtr.Zero;
        public Process game = null;

        public IntPtr yCoord = IntPtr.Zero;
        public IntPtr xCoord = IntPtr.Zero;
        public IntPtr zCoord = IntPtr.Zero;

        public float[] teleAddresses = new float[6];


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900, Primary.Orange800, Accent.Red700, TextShade.BLACK);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;

            game = Process.GetProcessesByName("FSD-Win64-Shipping").First();
            MemoryReader mem = new MemoryReader(game);
            baseAddress = game.MainModule.BaseAddress;
            ReloadAddresses();

            Thread LA = new Thread(LockAmmo);
            LA.Start();

            Thread SPD = new Thread(Speed);
            SPD.Start();

            Thread DM = new Thread(DownedMovement);
            DM.Start();

            Thread AR = new Thread(AutoReload);
            AR.Start();

            Thread Resources = new Thread(LockResources);
            Resources.Start();

            void LockResources()
            {


            }

            void LockAmmo()
            {
                while (true)
                {

                    //Primary gun
                    if (materialCheckbox8.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(primaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (materialCheckbox7.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(primaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }

                    //Secondary gun
                    if (materialCheckbox9.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(secondaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (materialCheckbox10.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(secondaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }

                    //Fourth gun
                    if (materialCheckbox13.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (materialCheckbox14.Checked)
                    {
                        IntPtr _fireRateAddy = mem.ReadAddress(baseAddress, Offsets.fireRate);
                        IntPtr _fireRate = mem.ReadAddress(_fireRateAddy, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                    //Thread.Sleep(25);
                }
            }

            void Speed()
            {
                while (true)
                {
                    IntPtr _xCoordAddress = mem.ReadAddress(baseAddress, Offsets.xCoord);
                    IntPtr _zCoordAddress = mem.ReadAddress(baseAddress, Offsets.zCoord);
                    int _speed = 25;
                    int _units = 59;
                    float _firstXVal = mem.ReadFloat(_xCoordAddress);
                    float _firstZVal = mem.ReadFloat(_zCoordAddress);
                    Thread.Sleep(25);
                    if (materialCheckbox16.Checked && (GetAsyncKeyState(Keys.W) < 0 || GetAsyncKeyState(Keys.S) < 0))
                    {
                        float _xCoordValue = mem.ReadFloat(_xCoordAddress);
                        if (_firstXVal - _xCoordValue > 0)
                        {
                            mem.WriteFloat(_xCoordAddress, _xCoordValue - _speed);
                        }
                        else
                        {
                            mem.WriteFloat(_xCoordAddress, _xCoordValue + _speed);
                        }

                    }
                    if (materialCheckbox16.Checked && (GetAsyncKeyState(Keys.A) < 0 || GetAsyncKeyState(Keys.D) < 0))
                    {
                        float _zCoordValue = mem.ReadFloat(_zCoordAddress);
                        if (_firstZVal - _zCoordValue > 0)
                        {
                            mem.WriteFloat(_zCoordAddress, _zCoordValue - _speed);
                        }
                        else
                        {
                            mem.WriteFloat(_zCoordAddress, _zCoordValue + _speed);
                        }
                    }

                    //Fly
                    if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue + _units);
                    }
                    else if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.LControlKey) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue - _units);
                    }
                }
            }

            void DownedMovement()
            {
                int _speed = 25;
                Thread.Sleep(25);
                while (true)
                {
                    if (materialCheckbox17.Checked && GetAsyncKeyState(Keys.W) < 0)
                    {
                        float _xCoordValue = mem.ReadFloat(xCoord);
                        mem.WriteFloat(xCoord, _xCoordValue - _speed);
                    }
                    if (materialCheckbox17.Checked && GetAsyncKeyState(Keys.S) < 0)
                    {
                        float _xCoordValue = mem.ReadFloat(xCoord);
                        mem.WriteFloat(xCoord, _xCoordValue + _speed);
                    }

                    if (materialCheckbox17.Checked && GetAsyncKeyState(Keys.A) < 0)
                    {
                        float _zCoordValue = mem.ReadFloat(zCoord);
                        mem.WriteFloat(zCoord, _zCoordValue + _speed);
                    }
                    if (materialCheckbox17.Checked && GetAsyncKeyState(Keys.D) < 0)
                    {
                        float _zCoordValue = mem.ReadFloat(zCoord);
                        mem.WriteFloat(zCoord, _zCoordValue - _speed);
                    }
                    Thread.Sleep(10);
                }
            }

        }

        private void AutoReload()
        {
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
                    ReloadAddresses();
                    System.Diagnostics.Debug.WriteLine("Reloaded");
                }
                catch
                {
                    listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Cant reload addresses");
                    System.Diagnostics.Debug.WriteLine("Cant reload");
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


        private void button1_Click(object sender, EventArgs e)
        {
            ReloadAddresses();
        }

        private void ReloadAddresses()
        {
            try
            {
                game = Process.GetProcessesByName("FSD-Win64-Shipping").First();
                MemoryReader mem = new MemoryReader(game);
                baseAddress = game.MainModule.BaseAddress;

                primaryAddress = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
                secondaryAddress = mem.ReadAddress(baseAddress, Offsets.SecondaryGun);
                yCoord = mem.ReadAddress(baseAddress, Offsets.yCoord);
                xCoord = mem.ReadAddress(baseAddress, Offsets.xCoord);
                zCoord = mem.ReadAddress(xCoord, 0x4);

                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Success: Reloaded addresses");
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Cant reload addresses");
                System.Diagnostics.Debug.WriteLine("Cant reload");
            }

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            ReloadAddresses();
            teleAddresses[0] = mem.ReadFloat(xCoord);
            teleAddresses[1] = mem.ReadFloat(yCoord);
            teleAddresses[2] = mem.ReadFloat(zCoord);

            materialMultiLineTextBox2.Text = "x: " + teleAddresses[0].ToString();
            materialMultiLineTextBox3.Text = "y: " + teleAddresses[1].ToString();
            materialMultiLineTextBox4.Text = "z: " + teleAddresses[2].ToString();

            foreach (var address in teleAddresses)
            {
                System.Diagnostics.Debug.WriteLine("in: " + address);
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            ReloadAddresses();
            teleAddresses[3] = mem.ReadFloat(xCoord);
            teleAddresses[4] = mem.ReadFloat(yCoord);
            teleAddresses[5] = mem.ReadFloat(zCoord);

            mem.WriteFloat(xCoord, teleAddresses[0] + 1);
            mem.WriteFloat(yCoord, teleAddresses[1] + 1);
            mem.WriteFloat(zCoord, teleAddresses[2] + 1);
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            mem.WriteFloat(xCoord, teleAddresses[3] + 1);
            mem.WriteFloat(yCoord, teleAddresses[4] + 1);
            mem.WriteFloat(zCoord, teleAddresses[5] + 1);
        }

        private void materialTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
            var infDepo = signatureScan.FindPattern("F3 0F 11 51 60 48", 0);
            System.Diagnostics.Debug.WriteLine("Thingy fuck: " + infDepo.ToString("X"));

            mem.WriteToCave(infDepo, new byte[] { 0xF3, 0x0F, 0x11, 0x51, 0x48 });

            var cave = mem.createCodeCave(1048);
            System.Diagnostics.Debug.WriteLine("cave: " + cave.ToString("X"));
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
            var infDepo = signatureScan.FindPattern("F3 0F 11 51 48", 0);
            mem.WriteToCave(infDepo, new byte[] { 0xF3, 0x0F, 0x11, 0x51, 0x60, 0x48 });

            mem.FreeCave(infDepo);
        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
            var infFlare = signatureScan.FindPattern("89 8F 18 02 00 00 48 85", 0);
            mem.WriteToCave(infFlare, new byte[] { 0x89, 0x8F, 0x18, 0x02, 0x00, 0x00 });

            System.Diagnostics.Debug.WriteLine("cave: ");
        }
    }
}
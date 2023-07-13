using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Deepcoc
{

    public partial class Form1 : Form
    {
        public IntPtr primaryAddress = IntPtr.Zero;
        public IntPtr secondaryAddress = IntPtr.Zero;

        public IntPtr baseAddress = IntPtr.Zero;
        public Process game = null;

        public IntPtr yCoord = IntPtr.Zero;
        public IntPtr xCoord = IntPtr.Zero;
        public IntPtr zCoord = IntPtr.Zero;

        public float[] teleAddresses = new float[3];

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            game = Process.GetProcessesByName("FSD-Win64-Shipping").First();
            MemoryReader mem = new MemoryReader(game);
            baseAddress = game.MainModule.BaseAddress;

            ReloadAddresses();

            //System.Diagnostics.Debug.WriteLine(address.ToString("X"));


            //System.Diagnostics.Debug.WriteLine(_currentAmmo.ToString("X"));

            Thread LA = new Thread(LockSecondaryAmmo);
            LA.Start();

            Thread LAP = new Thread(LockPrimaryAmmo);
            LAP.Start();

            Thread FLY = new Thread(Fly);
            FLY.Start();

            Thread SPD = new Thread(Speed);
            SPD.Start();

            Thread DM = new Thread(DownedMovement);
            DM.Start();

            void LockPrimaryAmmo()
            {
                while (true)
                {

                    if (checkBox3.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(primaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (checkBox4.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(primaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                    //Thread.Sleep(25);
                }
            }

            void LockSecondaryAmmo()
            {
                while (true)
                {

                    if (checkBox1.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(secondaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 30);
                    }
                    if (checkBox2.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(secondaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                    //Thread.Sleep(25);
                }
            }

            void Fly()
            {
                int _units = 5;
                while (true)
                {

                    if (checkBox5.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue + _units);
                    }
                    else if (checkBox5.Checked && GetAsyncKeyState(Keys.LControlKey) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue - _units);
                    }
                    Thread.Sleep(100);
                    _units += 7;
                }
            }

            void Speed()
            {
                while (true)
                {
                    IntPtr _xCoordAddress = mem.ReadAddress(baseAddress, Offsets.xCoord);
                    IntPtr _zCoordAddress = mem.ReadAddress(baseAddress, Offsets.zCoord);
                    int _speed = 25;
                    float _firstXVal = mem.ReadFloat(_xCoordAddress);
                    float _firstZVal = mem.ReadFloat(_zCoordAddress);
                    Thread.Sleep(25);
                    if (checkBox6.Checked && (GetAsyncKeyState(Keys.W) < 0 || GetAsyncKeyState(Keys.S) < 0))
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
                    if (checkBox6.Checked && (GetAsyncKeyState(Keys.A) < 0 || GetAsyncKeyState(Keys.D) < 0))
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
                }
            }

            void DownedMovement()
            {
                int _speed = 25;
                Thread.Sleep(25);
                while (true)
                {
                    if (checkBox7.Checked && GetAsyncKeyState(Keys.W) < 0)
                    {
                        float _xCoordValue = mem.ReadFloat(xCoord);
                        mem.WriteFloat(xCoord, _xCoordValue - _speed);
                    }
                    if (checkBox7.Checked && GetAsyncKeyState(Keys.S) < 0)
                    {
                        float _xCoordValue = mem.ReadFloat(xCoord);
                        mem.WriteFloat(xCoord, _xCoordValue + _speed);
                    }

                    if (checkBox7.Checked && GetAsyncKeyState(Keys.A) < 0)
                    {
                        float _zCoordValue = mem.ReadFloat(zCoord);
                        mem.WriteFloat(zCoord, _zCoordValue + _speed);
                    }
                    if (checkBox7.Checked && GetAsyncKeyState(Keys.D) < 0)
                    {
                        float _zCoordValue = mem.ReadFloat(zCoord);
                        mem.WriteFloat(zCoord, _zCoordValue - _speed);
                    }
                    Thread.Sleep(10);
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
            game = Process.GetProcessesByName("FSD-Win64-Shipping").First();
            MemoryReader mem = new MemoryReader(game);
            baseAddress = game.MainModule.BaseAddress;

            primaryAddress = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
            secondaryAddress = mem.ReadAddress(baseAddress, Offsets.SecondaryGun);
            yCoord = mem.ReadAddress(baseAddress, Offsets.yCoord);
            xCoord = mem.ReadAddress(baseAddress, Offsets.xCoord);
            zCoord = mem.ReadAddress(xCoord, 0x4);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            ReloadAddresses();
            teleAddresses[0] = mem.ReadFloat(xCoord);
            teleAddresses[1] = mem.ReadFloat(yCoord);
            teleAddresses[2] = mem.ReadFloat(zCoord);

            textBox3.Text = "x: " + teleAddresses[0].ToString();
            textBox4.Text = "y: " + teleAddresses[1].ToString();
            textBox5.Text = "z: " + teleAddresses[2].ToString();

            foreach (var address in teleAddresses)
            {
                System.Diagnostics.Debug.WriteLine("in: " + address);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            mem.WriteFloat(xCoord, teleAddresses[0] + 1);
            mem.WriteFloat(yCoord, teleAddresses[1] + 1);
            mem.WriteFloat(zCoord, teleAddresses[2] + 1);

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
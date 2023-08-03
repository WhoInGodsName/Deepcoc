using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using MaterialSkin;
using MaterialSkin.Controls;
using SoT_Helper.Services;
using System.Text;

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

        IntPtr infDepoAddress = IntPtr.Zero;
        IntPtr trampolineAddress = IntPtr.Zero;

        public float gravOffset = 0;

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

            Thread GO = new Thread(GravityOffset);
            GO.Start();

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

                    //Third gun
                    if (materialCheckbox11.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, Offsets.currentAmmo);
                        mem.WriteInt(_lockAmmo, 30);
                    }
                    if (materialCheckbox12.Checked)
                    {
                        IntPtr _fireRateAddy = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _fireRate = mem.ReadAddress(_fireRateAddy, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }

                    //Fourth gun
                    if (materialCheckbox13.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, Offsets.currentAmmo);
                        mem.WriteInt(_lockAmmo, 30);
                    }
                    if (materialCheckbox14.Checked)
                    {
                        IntPtr _fireRateAddy = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _fireRate = mem.ReadAddress(_fireRateAddy, Offsets.fireRate);
                        mem.WriteInt(_fireRate, 1);
                    }
                    Thread.Sleep(25);
                }
            }

            void Speed()
            {
                while (true)
                {
                    IntPtr _xCoordAddress = mem.ReadAddress(baseAddress, Offsets.xCoord);
                    IntPtr _zCoordAddress = mem.ReadAddress(baseAddress, Offsets.zCoord);
                    int _speed = materialSlider2.Value;
                    int _units = materialSlider3.Value;
                    float _firstXVal = mem.ReadFloat(_xCoordAddress);
                    float _firstZVal = mem.ReadFloat(_zCoordAddress);

                    var isGroundedAddress = mem.ReadAddress(baseAddress, Offsets.isGrounded);
                    var isGrounded = mem.ReadInt(isGroundedAddress);
                    Thread.Sleep(10);
                    var velY = mem.ReadAddress(baseAddress, Offsets.velocityY);
                    var velX = mem.ReadAddress(baseAddress, Offsets.velocityX);
                    var velZ = mem.ReadAddress(baseAddress, Offsets.velocityZ);

                    var velVector3 = mem.ReadVector3(velX, velY, velZ);

                    if (materialCheckbox16.Checked)
                    {
                        if (GetAsyncKeyState(Keys.W) < 0 || GetAsyncKeyState(Keys.A) < 0 || GetAsyncKeyState(Keys.S) < 0 || GetAsyncKeyState(Keys.D) < 0)
                        {
                            if (velVector3.X > 0)
                            {
                                mem.WriteFloat(velX, velVector3.X + _speed);
                            }
                            if (velVector3.X < 0)
                            {
                                mem.WriteFloat(velX, velVector3.X - _speed);
                            }

                            if (velVector3.Z > 0)
                            {
                                mem.WriteFloat(velZ, velVector3.Z + _speed);
                            }
                            if (velVector3.Z < 0)
                            {
                                mem.WriteFloat(velZ, velVector3.Z - _speed);
                            }
                        }
                        else
                        {
                            mem.WriteFloat(velX, 0);
                            mem.WriteFloat(velZ, 0);

                        }

                    }

                    //Fly
                    if (materialCheckbox15.Checked && isGrounded != 0)
                    {
                        mem.WriteFloat(velY, 0);
                    }

                    if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue + (_units));

                    }
                    else if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.LControlKey) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue - (_units));
                    }
                }
            }

            void GravityOffset()
            {
                while (true)
                {
                    var isGroundedAddress = mem.ReadAddress(baseAddress, Offsets.isGrounded);
                    var isGrounded = mem.ReadInt(isGroundedAddress);

                    if (isGrounded == 1)
                    {
                        gravOffset += 0.59f;
                        //System.Diagnostics.Debug.WriteLine("in");

                    }
                    else if (isGrounded == 0)
                    {
                        gravOffset = 0;
                        //System.Diagnostics.Debug.WriteLine("not in");
                    }
                    Thread.Sleep(10);
                }
            }

            void DownedMovement()
            {
                Thread.Sleep(25);
                while (true)
                {
                    int _speed = materialSlider2.Value;
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
                Thread.Sleep(20000);
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

        private void materialButton1_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            teleAddresses[0] = mem.ReadFloat(xCoord);
            teleAddresses[1] = mem.ReadFloat(yCoord);
            teleAddresses[2] = mem.ReadFloat(zCoord);

            materialMultiLineTextBox2.Text = "x: " + teleAddresses[0].ToString();
            materialMultiLineTextBox3.Text = "y: " + teleAddresses[1].ToString();
            materialMultiLineTextBox4.Text = "z: " + teleAddresses[2].ToString();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

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
            try
            {
                MemoryReader mem = new MemoryReader(game);
                SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
                var infDepo = signatureScan.FindPattern("F3 0F 11 51 60 48", 0);
                infDepoAddress = infDepo;

                Debug.WriteLine(infDepo.ToString("X"));
                IntPtr trampolineSourceAddr = mem.CreateCodeCave(infDepo + 0x15D6C00, 145);
                if (trampolineSourceAddr == IntPtr.Zero)
                {
                    trampolineSourceAddr = mem.CreateCodeCave(infDepo - 0x15D6C00, 145);
                }
                Debug.WriteLine(trampolineSourceAddr.ToString("X"));
                trampolineAddress = trampolineSourceAddr;


                // Calculate the address right after infDepo
                IntPtr infDepoNextAddr = infDepo + 5; // Assuming the instruction length is 6 bytes (adjust if needed)

                // Calculate the relative offset between infDepoNextAddr and trampolineSourceAddr
                var relativeOffset = mem.CalculateRelativeOffset(trampolineSourceAddr, infDepoNextAddr);

                // The jump back opcode for 64-bit processes
                byte[] jumpBackOpCode = new byte[]
                {
                    0xE9, // Relative jump opcode (jmp rel32)
                    (byte)(relativeOffset & 0xFF),
                    (byte)((relativeOffset >> 8) & 0xFF),
                    (byte)((relativeOffset >> 16) & 0xFF),
                    (byte)((relativeOffset >> 24) & 0xFF)
                };

                Debug.WriteLine(Encoding.UTF8.GetString(jumpBackOpCode));

                IntPtr pTrampoline = mem.CreateDetour(infDepo, trampolineSourceAddr, jumpBackOpCode);

                if (pTrampoline == IntPtr.Zero)
                {
                    listBox1.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss")} Error: Failed to create the detour.");
                    return;
                }

                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + $" Success: Infinite deposit has been enabled. {infDepo.ToString("X")} -> {trampolineSourceAddr.ToString("X")}");
            }
            catch
            {
                listBox1.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss")} Error: Infinite deposit has attempted to execute but failed.");
            }
        }
        private void materialButton5_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            mem.WriteToCave(infDepoAddress, new byte[] { 0xF3, 0x0F, 0x11, 0x51, 0x60, 0x48 });

            listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Success: Infinite deposite has been disabled.");

            mem.FreeCave(trampolineAddress);
        }

        private void materialButton7_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var ppAddress = mem.ReadAddress(baseAddress, Offsets.perkPoints);
            mem.WriteInt(ppAddress, 999999999);
        }

        private void materialButton8_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var scripAddress = mem.ReadAddress(baseAddress, Offsets.scrip);
            var scripValue = mem.ReadInt(scripAddress);

            mem.WriteInt(scripAddress, scripValue + 5);
        }

        private void materialButton9_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
            var infFlare = signatureScan.FindPattern("F3 0F ? ? ? ? ? ? F3 0F ? ? ? ? ? ? F3 0F ? ? 48 8B ? ? ? ? ? 48 8D", 0);
            mem.WriteToCave(infFlare, new byte[] { 0xF3, 0x0F, 0x11, 0x8E, 0xA8, 0x04, 0x00, 0x00 });
        }

        private void ChangeSize(float scale = 0, MaterialSlider slider = null)
        {
            MemoryReader mem = new MemoryReader(game);

            var charSizeAddress = mem.ReadAddress(baseAddress, Offsets.characterSizeY);
            var charSizeAddressX = mem.ReadAddress(baseAddress, Offsets.characterSizeX);
            var charSizeAddressZ = mem.ReadAddress(baseAddress, Offsets.characterSizeZ);

            if (slider != null)
            {
                mem.WriteFloat(charSizeAddress, slider.Value);
                mem.WriteFloat(charSizeAddressX, slider.Value);
                mem.WriteFloat(charSizeAddressZ, slider.Value);
            }
            else
            {
                mem.WriteFloat(charSizeAddress, scale);
                mem.WriteFloat(charSizeAddressX, scale);
                mem.WriteFloat(charSizeAddressZ, scale);
            }

        }

        private void Dance(byte danceMove)
        {
            var mem = new MemoryReader(game);
            var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);
            var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);
            var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);

            mem.WriteByte(isDancingAddress, 1);
            mem.WriteByte(inDanceRangeAddress, 1);
            mem.WriteByte(danceMoveAddress, 255);
            Thread.Sleep(100);

            switch (danceMove)
            {
                case 0:
                    mem.WriteByte(danceMoveAddress, 0);
                    break;
                case 1:
                    mem.WriteByte(danceMoveAddress, 1);
                    break;
                case 2:
                    mem.WriteByte(danceMoveAddress, 2);
                    break;
                case 3:
                    mem.WriteByte(danceMoveAddress, 3);
                    break;
                case 4:
                    mem.WriteByte(danceMoveAddress, 4);
                    break;
                case 5:
                    mem.WriteByte(danceMoveAddress, 5);
                    break;
                case 6:
                    mem.WriteByte(danceMoveAddress, 6);
                    break;
                case 7:
                    mem.WriteByte(danceMoveAddress, 7);
                    break;
                case 8:
                    mem.WriteByte(danceMoveAddress, 8);
                    break;
                case 9:
                    mem.WriteByte(danceMoveAddress, 9);
                    break;
                case 10:
                    mem.WriteByte(danceMoveAddress, 10);
                    break;
                case 11:
                    mem.WriteByte(inDanceRangeAddress, 0);
                    break;

            }

        }

        private void materialButton10_Click(object sender, EventArgs e)
        {
            ChangeSize(0, materialSlider1);
        }

        private void materialButton12_Click(object sender, EventArgs e)
        {
            ChangeSize(0.5f);
        }

        private void materialButton11_Click(object sender, EventArgs e)
        {
            ChangeSize(0.1f);
        }

        private void materialButton13_Click(object sender, EventArgs e)
        {
            ChangeSize(2f);
        }

        private void materialButton14_Click(object sender, EventArgs e)
        {
            ChangeSize(5f);
        }

        private void materialButton15_Click(object sender, EventArgs e)
        {
            ChangeSize(1f);
        }

        private void materialButton16_Click(object sender, EventArgs e)
        {
            ReloadAddresses();
        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var charSizeAddress = mem.ReadAddress(baseAddress, Offsets.characterSizeY);
                var charSizeAddressX = mem.ReadAddress(baseAddress, Offsets.characterSizeX);
                var charSizeAddressZ = mem.ReadAddress(baseAddress, Offsets.characterSizeZ);

                mem.WriteFloat(charSizeAddress, (float)Convert.ToDouble(materialMultiLineTextBox5.Text));
                mem.WriteFloat(charSizeAddressX, (float)Convert.ToDouble(materialMultiLineTextBox1.Text));
                mem.WriteFloat(charSizeAddressZ, (float)Convert.ToDouble(materialMultiLineTextBox6.Text));
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");

            }

        }

        private void materialButton17_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
            var freeShopping = signatureScan.FindPattern("89 83 ? ? ? ? 89 44 ? ? E8 ? ? ? ? 48 8B ? E8 ? ? ? ? 8B 83", 0);

            Debug.WriteLine(freeShopping.ToString("X"));

            IntPtr trampolineSourceAddr = (IntPtr)0x7FF7E93A0000;

            // Calculate the address right after infDepo
            IntPtr infDepoNextAddr = freeShopping + 5; // Assuming the instruction length is 6 bytes (adjust if needed)

            // Calculate the relative offset between infDepoNextAddr and trampolineSourceAddr
            var relativeOffset = mem.CalculateRelativeOffset(trampolineSourceAddr, infDepoNextAddr);

            // The jump back opcode for 64-bit processes
            byte[] jumpBackOpCode = new byte[]
            {
                    0xE9, // Relative jump opcode (jmp rel32)
                    (byte)(relativeOffset & 0xFF),
                    (byte)((relativeOffset >> 8) & 0xFF),
                    (byte)((relativeOffset >> 16) & 0xFF),
                    (byte)((relativeOffset >> 24) & 0xFF),
                    0x90
            };

            Debug.WriteLine(Encoding.UTF8.GetString(jumpBackOpCode));
            IntPtr pTrampoline = mem.CreateDetour(freeShopping, trampolineSourceAddr, jumpBackOpCode);

            if (pTrampoline == IntPtr.Zero)
            {
                listBox1.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss")} Error: Failed to create the detour.");
                return;
            }

            listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Success: Infinite deposit has been enabled.");
        }


        private void materialLabel16_Click(object sender, EventArgs e)
        {

        }

        private void materialButton18_Click_1(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var minThrowStrengthAddress = mem.ReadAddress(baseAddress, Offsets.throwMinForce);
                var maxThrowStrengthAddress = mem.ReadAddress(baseAddress, Offsets.throwMaxForce);

                Debug.WriteLine("meow: " + mem.ReadFloat(minThrowStrengthAddress));
                Debug.WriteLine(mem.ReadFloat(maxThrowStrengthAddress));

                mem.WriteFloat(minThrowStrengthAddress, (float)Convert.ToDouble(materialMultiLineTextBox7.Text));
                mem.WriteFloat(maxThrowStrengthAddress, (float)Convert.ToDouble(materialMultiLineTextBox7.Text));
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");
            }
        }

        private void materialButton19_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var fovAddress = mem.ReadAddress(baseAddress, Offsets.FOV);
                mem.WriteFloat(fovAddress, (float)Convert.ToDouble(materialMultiLineTextBox8.Text));

            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");
            }
        }

        private void materialButton20_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var runSpeedAddress = mem.ReadAddress(baseAddress, Offsets.runSpeed);
                var walkSpeedAddress = mem.ReadAddress(baseAddress, Offsets.walkSpeed);
                var decellerationAddress = mem.ReadAddress(baseAddress, Offsets.deceleration);

                mem.WriteFloat(runSpeedAddress, (float)Convert.ToDouble(materialMultiLineTextBox9.Text));
                mem.WriteFloat(walkSpeedAddress, (float)Convert.ToDouble(materialMultiLineTextBox9.Text));
                mem.WriteFloat(decellerationAddress, (float)Convert.ToDouble(materialMultiLineTextBox11.Text));
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");
            }
        }

        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            if (materialCheckbox1.Checked)
            {
                var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);
                var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);
                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);
                //mem.WriteByte(danceMoveAddress, 255);
                mem.WriteByte(isDancingAddress, 1);
                mem.WriteByte(inDanceRangeAddress, 1);
                Thread.Sleep(100);
                mem.WriteByte(danceMoveAddress, 255);
            }
            else
            {
                var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);
                var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);
                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);
                mem.WriteByte(isDancingAddress, 0);
                mem.WriteByte(inDanceRangeAddress, 0);
                mem.WriteByte(danceMoveAddress, 255);
            }

        }

        private void materialButton21_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var gravityScaleAddress = mem.ReadAddress(baseAddress, Offsets.gravityScale);

                mem.WriteFloat(gravityScaleAddress, (float)Convert.ToDouble(materialMultiLineTextBox10.Text));
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");
            }
        }

        private void materialButton22_Click(object sender, EventArgs e)
        {
            Dance(1);
        }

        private void materialButton23_Click(object sender, EventArgs e)
        {
            Dance(2);
        }

        private void materialButton24_Click(object sender, EventArgs e)
        {
            Dance(3);
        }

        private void materialButton25_Click(object sender, EventArgs e)
        {
            Dance(4);
        }

        private void materialButton26_Click(object sender, EventArgs e)
        {
            Dance(5);
        }

        private void materialButton27_Click(object sender, EventArgs e)
        {
            Dance(6);
        }

        private void materialButton28_Click(object sender, EventArgs e)
        {
            Dance(7);
        }

        private void materialButton29_Click(object sender, EventArgs e)
        {
            Dance(8);
        }

        private void materialButton30_Click(object sender, EventArgs e)
        {
            Dance(9);
        }

        private void materialButton31_Click(object sender, EventArgs e)
        {
            Dance(10);
        }

        private void materialButton32_Click(object sender, EventArgs e)
        {
            Dance(11);
        }
    }
}
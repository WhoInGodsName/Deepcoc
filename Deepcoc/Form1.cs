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
        int fireRate = 1;

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
            /*
            Thread DM = new Thread(DownedMovement);
            DM.Start();*/

            Thread AR = new Thread(AutoReload);
            AR.Start();

            Thread GO = new Thread(GravityOffset);
            GO.Start();

            Thread EDS = new Thread(DeathStare);
            EDS.Start();

            void LockAmmo()
            {
                while (true)
                {

                    try
                    {
                        if (materialMultiLineTextBox12.Text != "")
                        {
                            fireRate = Convert.ToInt32(materialMultiLineTextBox12.Text);
                            //listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + $" Success: firerate is set to {fireRate}.");
                        }
                    }
                    catch
                    {
                        listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid int for custom firerate.");
                    }
                    //Gunner specific
                    if (materialCheckbox18.Checked)
                    {
                        var overheatAddress = mem.ReadAddress(baseAddress, Offsets.primaryOverheat);
                        Thread.Sleep(5);
                        mem.WriteFloat(overheatAddress, 0);
                    }

                    //Primary gun
                    if (materialCheckbox8.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(primaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 1000);
                    }
                    if (materialCheckbox7.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(primaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, fireRate);
                    }

                    //Secondary gun
                    if (materialCheckbox9.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(secondaryAddress, Offsets.currentAmmo);
                        mem.WriteInt(_currentAmmo, 100);
                    }
                    if (materialCheckbox10.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(secondaryAddress, Offsets.fireRate);
                        mem.WriteInt(_fireRate, fireRate);
                    }

                    //Third gun
                    if (materialCheckbox11.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, Offsets.currentAmmo);
                        mem.WriteInt(_lockAmmo, 100);
                    }
                    if (materialCheckbox12.Checked)
                    {
                        IntPtr _fireRateAddy = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _fireRate = mem.ReadAddress(_fireRateAddy, Offsets.fireRate);
                        mem.WriteInt(_fireRate, fireRate);
                    }

                    //Fourth gun
                    if (materialCheckbox13.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, Offsets.currentAmmo);
                        mem.WriteInt(_lockAmmo, 100);
                    }
                    if (materialCheckbox14.Checked)
                    {
                        IntPtr _fireRateAddy = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _fireRate = mem.ReadAddress(_fireRateAddy, Offsets.fireRate);
                        mem.WriteInt(_fireRate, fireRate);
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
                    //Thread.Sleep(10);
                    var velY = mem.ReadAddress(baseAddress, Offsets.velocityY);
                    var velX = mem.ReadAddress(baseAddress, Offsets.velocityX);
                    var velZ = mem.ReadAddress(baseAddress, Offsets.velocityZ);

                    var velVector3 = mem.ReadVector3(velX, velY, velZ);

                    //Fly
                    if (materialCheckbox15.Checked && isGrounded != 0)
                    {
                        mem.WriteFloat(velY, 50);
                    }

                    if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue + (_units));
                        mem.WriteFloat(velY, 50);

                    }
                    else if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.LShiftKey) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(yCoord, _yCoordValue - (_units));
                    }

                    //Downed meme
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
                    Thread.Sleep(25);
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
            /*
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
            }*/

            void DeathStare()
            {
                while (true)
                {
                    if (materialCheckbox4.Checked)
                    {
                        try
                        {
                            var sightComponentAddress = mem.ReadAddress(baseAddress, Offsets.SightComponent);
                            var enemyPawnAddress = mem.ReadAddress(sightComponentAddress, Offsets.EnemyPawn);
                            var enemyMaxHealthAddress = mem.ReadAddress(baseAddress, Offsets.enemmMaxHealth);
                            var enemyDamage = mem.ReadAddress(baseAddress, Offsets.enemyDamage);
                            var enemyCourageAddress = mem.ReadAddress(baseAddress, Offsets.enemyCourage);
                            var enemyTimeDialationAddress = mem.ReadAddress(baseAddress, Offsets.enemyTimeScale);

                            //Debug.WriteLine(mem.ReadFloat(enemyMaxHealthAddress));
                            if (enemyMaxHealthAddress != IntPtr.Zero && mem.ReadFloat(enemyMaxHealthAddress) > 1)
                            {
                                mem.WriteFloat(enemyCourageAddress, 0);
                                mem.WriteFloat(enemyTimeDialationAddress, 0);
                                //mem.WriteFloat(enemyMaxHealthAddress, 0);
                                Thread.Sleep(20);
                                //mem.WriteFloat(enemyDamage, 10000);
                            }
                        }
                        catch
                        {

                        }
                        Thread.Sleep(20);
                    }

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
            
            //var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);

            

            if (danceMove < 11 && danceMove > 0)
            {

                var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);
                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);
                var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);

                mem.WriteByte(inDanceRangeAddress, 1);
                mem.WriteByte(isDancingAddress, 1);
                mem.WriteByte(danceMoveAddress, 255);
                Thread.Sleep(10);
                mem.WriteByte(danceMoveAddress, danceMove);
            }
            else if (danceMove == 11)
            {
                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);
                var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);

                mem.WriteByte(inDanceRangeAddress, 0);
            }
            Thread.Sleep(100);
        }

        private void materialButton10_Click_1(object sender, EventArgs e)
        {
            ChangeSize(0, materialSlider1);
        }
        private void materialButton6_Click_1(object sender, EventArgs e)
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

        private void materialButton11_Click_1(object sender, EventArgs e)
        {
            ChangeSize(0.1f);
        }

        private void materialButton12_Click_1(object sender, EventArgs e)
        {
            ChangeSize(0.5f);
        }

        private void materialButton15_Click_1(object sender, EventArgs e)
        {
            ChangeSize(1f);
        }

        private void materialButton13_Click_1(object sender, EventArgs e)
        {
            ChangeSize(2f);
        }

        private void materialButton14_Click_1(object sender, EventArgs e)
        {
            ChangeSize(5f);
        }

        private void materialButton16_Click(object sender, EventArgs e)
        {
            ReloadAddresses();
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

        private void materialButton18_Click(object sender, EventArgs e)
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

        private void materialButton19_Click_1(object sender, EventArgs e)
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

        private void materialButton20_Click_1(object sender, EventArgs e)
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

        private void materialSlider1_Click(object sender, EventArgs e)
        {

        }

        private void materialCheckbox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void materialCheckbox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void fullAutoPrimary_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var primaryFullAutoAddress = mem.ReadAddress(baseAddress, Offsets.primaryGunFullAuto);
                Debug.WriteLine(primaryFullAutoAddress.ToString("X"));
                if (fullAutoPrimary.Checked)
                {
                    mem.WriteByte(primaryFullAutoAddress, 1);
                }
                else
                {
                    mem.WriteByte(primaryFullAutoAddress, 0);
                }
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Illegal change.");
            }

        }

        private void materialCheckbox2_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            IntPtr secondaryGunAddress = mem.ReadAddress(baseAddress, Offsets.SecondayGunFullAuto);
            Thread.Sleep(10);
            IntPtr burstCountAddress = mem.ReadAddress(baseAddress, Offsets.SecondayGunBurst);

            Debug.WriteLine(secondaryGunAddress.ToString("X"));
            if (materialCheckbox2.Checked)
            {
                mem.WriteInt(burstCountAddress, 10);
                mem.WriteByte(secondaryGunAddress, 1);
            }
            else
            {
                mem.WriteByte(secondaryGunAddress, 0);
                mem.WriteInt(burstCountAddress, 0);
            }
        }

        public float priorMin = 0f;
        public float priorMax = 0f;
        private void materialCheckbox3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var recoilMinAddress = mem.ReadAddress(baseAddress, Offsets.SecondayGunRecoilMin);
                var recoilMaxAddress = mem.ReadAddress(baseAddress, Offsets.SecondayGunRecoilMax);



                if (materialCheckbox3.Checked)
                {
                    priorMin = mem.ReadFloat(recoilMinAddress);
                    priorMax = mem.ReadFloat(recoilMaxAddress);

                    mem.WriteFloat(recoilMinAddress, 0);
                    mem.WriteFloat(recoilMaxAddress, 0);
                }
                else
                {
                    mem.WriteFloat(recoilMinAddress, priorMin);
                    mem.WriteFloat(recoilMaxAddress, priorMax);
                }
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Illegal read/write for recoil.");
            }

        }

        private void materialCheckbox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckbox5_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var noClipAddress = mem.ReadAddress(baseAddress, Offsets.noClip);

            if (materialCheckbox5.Checked)
            {
                mem.WriteInt(noClipAddress, 0);
                materialCheckbox15.Checked = true;
            }
            else
            {
                mem.WriteInt(noClipAddress, 1);
            }

        }

        private void materialCheckbox6_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var flareAddress = mem.ReadAddress(baseAddress, Offsets.flares);

            if (materialCheckbox6.Checked)
            {
                mem.WriteInt(flareAddress, 99999999);
            }
            else
            {
                mem.WriteInt(flareAddress, 4);
            }
        }

        private void materialCheckbox16_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var flareAddress = mem.ReadAddress(baseAddress, Offsets.flareCooldown);

            if (materialCheckbox16.Checked)
            {
                mem.WriteFloat(flareAddress, 0);
            }
            else
            {
                mem.WriteFloat(flareAddress, 0.2f);
            }
        }

        private void materialCheckbox18_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckbox19_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            IntPtr secondaryGunAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunFullAuto);
            Thread.Sleep(10);
            IntPtr burstCountAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunBurst);

            Debug.WriteLine(secondaryGunAddress.ToString("X"));
            if (materialCheckbox19.Checked)
            {
                mem.WriteInt(burstCountAddress, 10);
                mem.WriteByte(secondaryGunAddress, 1);
            }
            else
            {
                mem.WriteByte(secondaryGunAddress, 0);
                mem.WriteInt(burstCountAddress, 0);
            }
        }

        private void materialCheckbox21_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialButton33_Click(object sender, EventArgs e)
        {
            materialCheckbox1.Checked = false;
            materialCheckbox2.Checked = false;
            materialCheckbox3.Checked = false;
            materialCheckbox4.Checked = false;
            materialCheckbox5.Checked = false;
            materialCheckbox6.Checked = false;
            materialCheckbox7.Checked = false;
            materialCheckbox8.Checked = false;
            materialCheckbox9.Checked = false;
            materialCheckbox10.Checked = false;
            materialCheckbox11.Checked = false;
            materialCheckbox12.Checked = false;
            materialCheckbox13.Checked = false;
            materialCheckbox14.Checked = false;
            materialCheckbox15.Checked = false;
            materialCheckbox16.Checked = false;
            materialCheckbox17.Checked = false;
            materialCheckbox18.Checked = false;
            materialCheckbox19.Checked = false;
            materialCheckbox20.Checked = false;

        }

        private void materialCheckbox20_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var recoilMinAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunRecoilMin);
                var recoilMaxAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunRecoilMax);



                if (materialCheckbox3.Checked)
                {
                    priorMin = mem.ReadFloat(recoilMinAddress);
                    priorMax = mem.ReadFloat(recoilMaxAddress);

                    mem.WriteFloat(recoilMinAddress, 0);
                    mem.WriteFloat(recoilMaxAddress, 0);
                }
                else
                {
                    mem.WriteFloat(recoilMinAddress, priorMin);
                    mem.WriteFloat(recoilMaxAddress, priorMax);
                }
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Illegal read/write for recoil.");
            }
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {
            Dance(1);
        }

        private void materialButton22_Click_1(object sender, EventArgs e)
        {
            Dance(1);
        }

        private void materialButton23_Click_1(object sender, EventArgs e)
        {
            Dance(2);
        }

        private void materialButton24_Click_1(object sender, EventArgs e)
        {
            Dance(3);
        }

        private void materialButton25_Click_1(object sender, EventArgs e)
        {
            Dance(4);
        }

        private void materialButton26_Click_1(object sender, EventArgs e)
        {
            Dance(5);
        }

        private void materialButton27_Click_1(object sender, EventArgs e)
        {
            Dance(6);
        }

        private void materialButton28_Click_1(object sender, EventArgs e)
        {
            Dance(7);
        }

        private void materialButton29_Click_1(object sender, EventArgs e)
        {
            Dance(8);
        }

        private void materialButton30_Click_1(object sender, EventArgs e)
        {
            Dance(9);
        }

        private void materialButton31_Click_1(object sender, EventArgs e)
        {
            Dance(10);
        }

        private void materialButton32_Click_1(object sender, EventArgs e)
        {
            Dance(11);
        }
    }
}
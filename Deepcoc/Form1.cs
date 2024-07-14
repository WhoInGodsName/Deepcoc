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
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange900, Primary.Orange800, Primary.Red800, Accent.Orange700, TextShade.BLACK);
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

            Thread AR = new Thread(AutoReload);
            AR.Start();

            Thread RE = new Thread(Resources);
            RE.Start();

            Thread EDS = new Thread(DeathStare);
            EDS.Start();

            void LockAmmo()
            {
                var ammoOffsetPrimary = Offsets.currentAmmo;
                var ammoOffsetSecondary = Offsets.currentAmmo;
                var ammoOffsetThird = Offsets.currentAmmo;
                while (true)
                {

                    try
                    {
                        if (materialMultiLineTextBox12.Text != "")
                        {
                            fireRate = Convert.ToInt32(materialMultiLineTextBox12.Text);
                        }
                    }
                    catch
                    {
                        listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid int for custom firerate.");
                    }

                    if (materialComboBox1.GetItemText(materialComboBox1.SelectedItem) == "Reserve")
                    {
                        ammoOffsetPrimary = Offsets.ammoCount;
                    }
                    else
                    {
                        ammoOffsetPrimary = Offsets.currentAmmo;
                    }

                    if (materialComboBox2.GetItemText(materialComboBox2.SelectedItem) == "Reserve")
                    {
                        ammoOffsetSecondary = Offsets.ammoCount;
                    }
                    else
                    {
                        ammoOffsetSecondary = Offsets.currentAmmo;
                    }

                    if (materialComboBox3.GetItemText(materialComboBox3.SelectedItem) == "Reserve")
                    {
                        ammoOffsetThird = Offsets.ammoCount;
                    }
                    else
                    {
                        ammoOffsetThird = Offsets.currentAmmo;
                    }

                    //Primary gun
                    if (materialCheckbox8.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(primaryAddress, ammoOffsetPrimary);
                        mem.WriteInt(_currentAmmo, 100);
                    }
                    if (materialCheckbox7.Checked)
                    {
                        var firstGunAddress = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
                        var fullAutoAddress = mem.ReadAddress(firstGunAddress, Offsets.fullAuto);
                        var cycleAddress = mem.ReadAddress(firstGunAddress, Offsets.cycleTimeLeft);
                        mem.WriteFloat(cycleAddress, 0);
                        mem.WriteFloat(fullAutoAddress, 1);
                    }
                    if (materialCheckbox22.Checked)
                    {
                        RecoilControl(Offsets.PrimaryGun);
                    }

                    //Secondary gun
                    if (materialCheckbox9.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(secondaryAddress, ammoOffsetSecondary);
                        mem.WriteInt(_currentAmmo, 100);
                    }
                    if (materialCheckbox10.Checked)
                    {
                        IntPtr _fireRate = mem.ReadAddress(secondaryAddress, Offsets.ammoCount);
                        mem.WriteFloat(_fireRate + Offsets.fireRate2, 20f);
                    }
                    if (materialCheckbox2.Checked)
                    {
                        var gun = mem.ReadAddress(baseAddress, Offsets.SecondaryGun);
                        var cycleTimeLeft = mem.ReadAddress(gun, Offsets.cycleTimeLeft);
                        var fullAutoAddress = mem.ReadAddress(gun, Offsets.fullAuto);
                        mem.WriteFloat(cycleTimeLeft, 0);
                        mem.WriteFloat(fullAutoAddress, 1);
                    }
                    if (materialCheckbox3.Checked)
                    {
                        RecoilControl(Offsets.SecondaryGun);
                    }

                    //Third gun
                    if (materialCheckbox11.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, ammoOffsetThird);
                        mem.WriteInt(_lockAmmo, 100);
                    }
                    if (materialCheckbox12.Checked)
                    {
                        IntPtr _gun = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        IntPtr _fireRate = mem.ReadAddress(_gun, Offsets.fireRate2);
                        mem.WriteFloat(_fireRate, 20f);
                    }
                    if (fullAutoPrimary.Checked)
                    {
                        var gun = mem.ReadAddress(baseAddress, Offsets.ThirdGun);
                        var cycleTimeLeft = mem.ReadAddress(gun, Offsets.cycleTimeLeft);
                        mem.WriteFloat(cycleTimeLeft, 0);
                    }
                    if (materialCheckbox20.Checked)
                    {
                        RecoilControl(Offsets.ThirdGun);
                    }

                    //Scout flare gun
                    if (materialCheckbox18.Checked)
                    {
                        IntPtr _currentAmmo = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _lockAmmo = mem.ReadAddress(_currentAmmo, Offsets.currentAmmo);
                        mem.WriteInt(_lockAmmo, 100);
                    }
                    if (materialCheckbox27.Checked)
                    {
                        IntPtr _gun = mem.ReadAddress(baseAddress, Offsets.FourthGun);
                        IntPtr _fireRate = mem.ReadAddress(_gun, Offsets.fireRate2);
                        mem.WriteInt(_fireRate, 20);
                    }
                    if (materialCheckbox14.Checked)
                    {
                        IntPtr fourthGunAddress = mem.ReadAddress(baseAddress, Offsets.ForthGunFullAuto);
                        IntPtr cycleTimeLeft = mem.ReadAddress(fourthGunAddress, Offsets.cycleTimeLeft);
                        mem.WriteFloat(cycleTimeLeft, 0);
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
                    IntPtr _jumpMaxAddress = mem.ReadAddress(baseAddress, Offsets.jumpMaxCount);
                    float yVal = mem.ReadFloat(yCoord);
                    int _speed = materialSlider2.Value;
                    float _firstXVal = mem.ReadFloat(_xCoordAddress);
                    float _firstZVal = mem.ReadFloat(_zCoordAddress);

                    var isGroundedAddress = mem.ReadAddress(baseAddress, Offsets.isGrounded);
                    var isGrounded = mem.ReadInt(isGroundedAddress);
                    var velY = mem.ReadAddress(baseAddress, Offsets.velocityY);
                    var velX = mem.ReadAddress(baseAddress, Offsets.velocityX);
                    var velZ = mem.ReadAddress(baseAddress, Offsets.velocityZ);

                    var velVector3 = mem.ReadVector3(velX, velY, velZ);

                    //Fly
                    if (materialCheckbox15.Checked && isGrounded != 0)
                    {
                        mem.WriteFloat(velY, 10);
                    }

                    if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(velY, 20 * materialSlider3.Value);

                    }
                    else if (materialCheckbox15.Checked && GetAsyncKeyState(Keys.LShiftKey) < 0)
                    {
                        float _yCoordValue = mem.ReadFloat(yCoord);
                        mem.WriteFloat(velY, -20 * materialSlider3.Value);

                    }

                    //Fly thats better for clients
                    if (materialCheckbox21.Checked && GetAsyncKeyState(Keys.Space) < 0)
                    {
                        mem.WriteInt(_jumpMaxAddress, 999999);
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
                    Thread.Sleep(5);
                }

            }

            //Function : Goes through all the resource slots within molly and updates the values to the line boxes.  If one of the values has been changed
            // by the user, it will update that resource slot within molly and the line box to match.
            void Resources()
            {
                MaterialSkin.Controls.MaterialMultiLineTextBox[] text_list = { materialMultiLineTextBox13, materialMultiLineTextBox14, materialMultiLineTextBox15, materialMultiLineTextBox16,
                                                                        materialMultiLineTextBox17, materialMultiLineTextBox18, materialMultiLineTextBox19, materialMultiLineTextBox20};
                float[] prevValues = new float[9];
                while (true)
                {
                    var mollyBase = mem.ReadAddress(baseAddress, Offsets.slot1Molly);


                    for (int i = 0; i < text_list.Length; i++)
                    {
                        Debug.WriteLine((i * 0x8));
                        var slotPosAddress = mem.ReadAddress(mollyBase, (i * 0x8));
                        var slotValueAddress = mem.ReadAddress(slotPosAddress, Offsets.slot);
                        var slotValue = mem.ReadFloat(slotValueAddress);
                        prevValues[i] = slotValue;
                        text_list[i].Text = slotValue.ToString();
                    }
                    Thread.Sleep(10000);

                    for (int j = 0; j < text_list.Length; j++)
                    {
                        var slotPosAddress = mem.ReadAddress(mollyBase, (j * 0x8));

                        var slotValueAddress = mem.ReadAddress(slotPosAddress, Offsets.slot);
                        var slotValue = mem.ReadFloat(slotValueAddress);
                        var slotCurrValue = mem.ReadAddress(slotPosAddress, Offsets.CurrentSlot);

                        if ((float)Convert.ToDouble(text_list[j].Text) != prevValues[j])
                        {

                            mem.WriteFloat(slotValueAddress, (float)Convert.ToDouble(text_list[j].Text));
                            mem.WriteFloat(slotCurrValue, (float)Convert.ToDouble(text_list[j].Text));
                            text_list[j].Text = mem.ReadFloat(slotValueAddress).ToString();
                        }
                    }
                }
            }

            //Function : Gets current entity being stared at and applies selected effects from the user to that entity.
            void DeathStare()
            {
                while (true)
                {
                    //Debug.WriteLine("in1");
                    if (materialCheckbox26.Checked)
                    {
                        try
                        {

                            var sightComponentAddress = mem.ReadAddress(baseAddress, Offsets.SightComponent);
                            var enemyTimeDialationAddress = mem.ReadAddress(baseAddress, Offsets.enemyTimeScale);
                            var meshAddress = mem.ReadAddress(sightComponentAddress, Offsets.targetMesh);
                            var meshValue = mem.ReadFloat(meshAddress);

                            if (sightComponentAddress != IntPtr.Zero)
                            {


                                var timeDialationAddress = mem.ReadAddress(sightComponentAddress, Offsets.timeDialation);
                                mem.WriteFloat(timeDialationAddress, materialSlider4.Value);

                                var scaleAddressX = mem.ReadAddress(meshAddress, Offsets.targetScaleX);
                                var scaleAddressY = mem.ReadAddress(meshAddress, Offsets.targetScaleY);
                                var scaleAddressZ = mem.ReadAddress(meshAddress, Offsets.targetScaleZ);
                                Debug.WriteLine(mem.ReadFloat(scaleAddressX) + " " + mem.ReadFloat(scaleAddressY) + " " + mem.ReadFloat(scaleAddressZ));
                                if (mem.ReadFloat(scaleAddressX) > 0.001 && scaleAddressY != IntPtr.Zero && scaleAddressZ != IntPtr.Zero)
                                {
                                    if (!materialMultiLineTextBox22.Text.Equals("0") && scaleAddressZ != IntPtr.Zero)
                                    {

                                        mem.WriteFloat(scaleAddressZ, (float)Convert.ToDouble(materialMultiLineTextBox22.Text));
                                    }
                                    if (!materialMultiLineTextBox23.Text.Equals("0") && scaleAddressY != IntPtr.Zero)
                                    {
                                        mem.WriteFloat(scaleAddressY, (float)Convert.ToDouble(materialMultiLineTextBox23.Text));
                                    }
                                    if (!materialMultiLineTextBox24.Text.Equals("0") && scaleAddressX != IntPtr.Zero)
                                    {
                                        mem.WriteFloat(scaleAddressX, (float)Convert.ToDouble(materialMultiLineTextBox24.Text));
                                    }
                                }

                            }


                        }
                        catch
                        {

                        }
                        Thread.Sleep(25);
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
                zCoord = IntPtr.Add(xCoord, 0x4);

                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Success: Reloaded addresses");
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Cant reload addresses");
                System.Diagnostics.Debug.WriteLine("Cant reload");
            }

        }

        //Teleporting
        private void materialButton1_Click_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            teleAddresses[0] = mem.ReadFloat(xCoord);
            teleAddresses[1] = mem.ReadFloat(yCoord);
            teleAddresses[2] = mem.ReadFloat(zCoord);

            materialMultiLineTextBox2.Text = "x: " + teleAddresses[0].ToString();
            materialMultiLineTextBox3.Text = "y: " + teleAddresses[1].ToString();
            materialMultiLineTextBox4.Text = "z: " + teleAddresses[2].ToString();
        }

        //Teleport
        private void materialButton2_Click_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            teleAddresses[3] = mem.ReadFloat(xCoord);
            teleAddresses[4] = mem.ReadFloat(yCoord);
            teleAddresses[5] = mem.ReadFloat(zCoord);

            mem.WriteFloat(xCoord, teleAddresses[0] + 1);
            mem.WriteFloat(yCoord, teleAddresses[1] + 1);
            mem.WriteFloat(zCoord, teleAddresses[2] + 1);
        }

        private void materialButton3_Click_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            mem.WriteFloat(xCoord, teleAddresses[3] + 1);
            mem.WriteFloat(yCoord, teleAddresses[4] + 1);
            mem.WriteFloat(zCoord, teleAddresses[5] + 1);
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                SignatureScan signatureScan = new SignatureScan(game, baseAddress, game.MainModule.ModuleMemorySize);
                var infDepo = signatureScan.FindPattern("F3 0F 11 51 60", 0);
                infDepoAddress = infDepo;

                Debug.WriteLine("infDepo: " + infDepo.ToString("X"));
                IntPtr trampolineSourceAddr = mem.CreateCodeCave(infDepo + 0x163F5B1, 145);
                if (trampolineSourceAddr == IntPtr.Zero)
                {
                    trampolineSourceAddr = mem.CreateCodeCave(infDepo - 0x163F5B1, 145);
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
                    listBox1.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss")} Error: Failed to create the detour. {infDepo.ToString("X")} -> {trampolineSourceAddr.ToString("X")}");
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

        //Max perk points
        private void materialButton7_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var ppAddress = mem.ReadAddress(baseAddress, Offsets.perkPoints);
            mem.WriteInt(ppAddress, 999999);
        }

        //Scrip
        private void materialButton8_Click(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var scripAddress = mem.ReadAddress(baseAddress, Offsets.scrip);
            var scripValue = mem.ReadInt(scripAddress);

            mem.WriteInt(scripAddress, scripValue + 5);
        }

        //Change char size
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

        float priorMin = 0;
        float priorMax = 0;
        private void RecoilControl(int[] gun)
        {

            try
            {
                MemoryReader mem = new MemoryReader(game);
                var recoilAddress = mem.ReadAddress(baseAddress, gun);
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + recoilAddress.ToString("X"));
                var recoilPitchMin = mem.ReadAddress(recoilAddress, Offsets.recoilPitchMin);
                var recoilPitchMax = mem.ReadAddress(recoilAddress, Offsets.recoilPitchMax);
                var recoilYawMin = mem.ReadAddress(recoilAddress, Offsets.recoilYawMin);
                var recoilYawMax = mem.ReadAddress(recoilAddress, Offsets.recoilYawMax);
                var recoilRollMin = mem.ReadAddress(recoilAddress, Offsets.recoilRollMin);
                var recoilRollMax = mem.ReadAddress(recoilAddress, Offsets.recoilRollMax);

                Debug.WriteLine("recoil min: " + recoilPitchMin.ToString("X"));
                priorMin = mem.ReadFloat(recoilPitchMin);
                priorMax = mem.ReadFloat(recoilPitchMax);

                mem.WriteFloat(recoilPitchMin, 0);
                mem.WriteFloat(recoilPitchMax, 0);
                mem.WriteFloat(recoilYawMin, 0);
                mem.WriteFloat(recoilYawMax, 0);
                mem.WriteFloat(recoilRollMin, 0);
                mem.WriteFloat(recoilRollMax, 0);
            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Illegal read/write for recoil.");
            }
        }

        private void Dance(byte danceMove)
        {
            var mem = new MemoryReader(game);

            //var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);



            if (danceMove < 11 && danceMove > 0)
            {


                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);
                mem.WriteByte(inDanceRangeAddress, 0);
                Thread.Sleep(10);

                var danceMoveAddress = mem.ReadAddress(baseAddress, Offsets.danceMove);
                var isDancingAddress = mem.ReadAddress(baseAddress, Offsets.isDancing);

                mem.WriteByte(inDanceRangeAddress, 1);
                mem.WriteByte(isDancingAddress, 0);
                mem.WriteByte(danceMoveAddress, 255);
                Thread.Sleep(10);
                mem.WriteByte(danceMoveAddress, danceMove);
            }
            else if (danceMove == 11)
            {
                var inDanceRangeAddress = mem.ReadAddress(baseAddress, Offsets.inDanceRange);

                mem.WriteByte(inDanceRangeAddress, 0);
            }
            Thread.Sleep(100);
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

        private void fullAutoPrimary_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var primaryFullAutoAddress = mem.ReadAddress(baseAddress, Offsets.primaryGunFullAuto);
                var gun = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
                var cycleTimeLeft = mem.ReadAddress(gun, Offsets.cycleTimeLeft);
                Thread.Sleep(10);
                IntPtr burstCountAddress = mem.ReadAddress(baseAddress, Offsets.primaryGunBurst);
                Debug.WriteLine(primaryFullAutoAddress.ToString("X"));
                if (fullAutoPrimary.Checked)
                {
                    mem.WriteByte(primaryFullAutoAddress, 1);
                    mem.WriteFloat(cycleTimeLeft, 0);
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
                mem.WriteByte(secondaryGunAddress, 1);
            }
            else
            {
                mem.WriteByte(secondaryGunAddress, 0);
            }
        }




        //No clip
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

        private void materialCheckbox19_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            IntPtr secondaryGunAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunFullAuto);
            Thread.Sleep(10);
            IntPtr burstCountAddress = mem.ReadAddress(baseAddress, Offsets.ThirdGunBurst);

            Debug.WriteLine(secondaryGunAddress.ToString("X"));
            if (materialCheckbox19.Checked)
            {
                mem.WriteByte(secondaryGunAddress, 1);
            }
            else
            {
                mem.WriteByte(secondaryGunAddress, 0);
                mem.WriteInt(burstCountAddress, 0);
            }
        }

        //Uncheck all
        private void materialButton33_Click(object sender, EventArgs e)
        {
            materialCheckbox2.Checked = false;
            materialCheckbox3.Checked = false;
            materialCheckbox4.Checked = false;
            materialCheckbox24.Checked = false;
            materialCheckbox5.Checked = false;
            materialCheckbox6.Checked = false;
            materialCheckbox7.Checked = false;
            materialCheckbox8.Checked = false;
            materialCheckbox9.Checked = false;
            materialCheckbox10.Checked = false;
            materialCheckbox11.Checked = false;
            materialCheckbox12.Checked = false;
            materialCheckbox15.Checked = false;
            materialCheckbox16.Checked = false;
            materialCheckbox17.Checked = false;
            materialCheckbox19.Checked = false;
            materialCheckbox20.Checked = false;
            materialCheckbox21.Checked = false;
            materialCheckbox22.Checked = false;
            materialCheckbox23.Checked = false;
            materialCheckbox26.Checked = false;
            materialCheckbox28.Checked = false;
            materialCheckbox1.Checked = false;
            materialCheckbox29.Checked = false;
            materialCheckbox30.Checked = false;
            materialCheckbox31.Checked = false;
            materialCheckbox32.Checked = false;
            materialCheckbox18.Checked = false;
            materialCheckbox27.Checked = false;
            materialCheckbox13.Checked = false;
            materialCheckbox14.Checked = false;
            fullAutoPrimary.Checked = false;
        }

        //All emote buttons
        private void materialLabel1_Click(object sender, EventArgs e) { Dance(1); }
        private void materialButton22_Click_1(object sender, EventArgs e) { Dance(1); }
        private void materialButton23_Click_1(object sender, EventArgs e) { Dance(2); }
        private void materialButton24_Click_1(object sender, EventArgs e) { Dance(3); }
        private void materialButton25_Click_1(object sender, EventArgs e) { Dance(4); }
        private void materialButton26_Click_1(object sender, EventArgs e) { Dance(5); }
        private void materialButton27_Click_1(object sender, EventArgs e) { Dance(6); }
        private void materialButton28_Click_1(object sender, EventArgs e) { Dance(7); }
        private void materialButton29_Click_1(object sender, EventArgs e) { Dance(8); }
        private void materialButton30_Click_1(object sender, EventArgs e) { Dance(9); }
        private void materialButton31_Click_1(object sender, EventArgs e) { Dance(10); }
        private void materialButton32_Click_1(object sender, EventArgs e) { Dance(11); }


        //Carve 'er up
        private void materialCheckbox23_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var weaponFireAddress = mem.ReadAddress(primaryAddress, Offsets.weaponFire);
            var canCarveAddress = mem.ReadAddress(weaponFireAddress, Offsets.bulletsCanCarve);
            var bulletPerCarveMin = mem.ReadAddress(weaponFireAddress, Offsets.bulletsPerCarveMin);
            var bulletPerCarveMax = mem.ReadAddress(weaponFireAddress, Offsets.bulletsPerCarveMax);
            var carveDiameterAddress = mem.ReadAddress(weaponFireAddress, Offsets.carveDiameter);

            if (materialCheckbox23.Checked)
            {
                mem.WriteInt(canCarveAddress, 1);
                mem.WriteInt(bulletPerCarveMin, 1);
                mem.WriteInt(bulletPerCarveMax, 1);
                mem.WriteFloat(carveDiameterAddress, (float)Convert.ToDouble(materialMultiLineTextBox21.Text));
            }
        }

        //God mode
        private void materialCheckbox24_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var charAddress = mem.ReadAddress(baseAddress, Offsets.character);
            var healthCompAddress = mem.ReadAddress(charAddress, Offsets.PlayerHealthComponent);
            var canTakeDamageAddress = mem.ReadAddress(healthCompAddress, Offsets.playerCanTakeDamage);
            if (materialCheckbox24.Checked)
            {
                mem.WriteInt(canTakeDamageAddress, 0);
            }
            else
            {
                mem.WriteInt(canTakeDamageAddress, 1);
            }
        }

        private void materialCheckbox4_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var dilationAddress = mem.ReadAddress(baseAddress, Offsets.characterTimeDialation);
            if (materialCheckbox4.Checked)
            {
                mem.WriteFloat(dilationAddress, materialSlider5.Value);
            }
            else
            {
                mem.WriteFloat(dilationAddress, 1);
            }
        }

        //No overheat : Gunner
        private void materialCheckbox1_CheckedChanged_2(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var primaryGun = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
            var manualCooldownAddress = mem.ReadAddress(primaryGun, Offsets.ManualCooldownDelay);
            var cooldownRateAddress = mem.ReadAddress(primaryGun, Offsets.coolDownRate);
            if (materialCheckbox1.Checked)
            {
                mem.WriteFloat(manualCooldownAddress, 0);
                mem.WriteFloat(cooldownRateAddress, 1000);
            }
            else
            {
                mem.WriteFloat(manualCooldownAddress, 0.3f);
                mem.WriteFloat(cooldownRateAddress, 4);
            }

        }

        private void materialCheckbox14_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            IntPtr fourthGunAddress = mem.ReadAddress(baseAddress, Offsets.ForthGunFullAuto);
            Thread.Sleep(10);
            IntPtr burstCountAddress = mem.ReadAddress(baseAddress, Offsets.FourthGunBurst);
            IntPtr cycleTimeLeft = mem.ReadAddress(fourthGunAddress, Offsets.cycleTimeLeft);

            Debug.WriteLine(fourthGunAddress.ToString("X"));
            if (materialCheckbox14.Checked)
            {
                mem.WriteInt(burstCountAddress, 10);
                mem.WriteByte(fourthGunAddress, 1);
            }
            else
            {
                mem.WriteByte(fourthGunAddress, 0);
                mem.WriteInt(burstCountAddress, 0);
            }
        }

        //Grenades
        private void materialCheckbox28_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);

            var inventory = mem.ReadAddress(baseAddress, Offsets.inventoryComponent);
            var grenadeItem = mem.ReadAddress(inventory, Offsets.grenadeItem);
            var grenadeCount = mem.ReadAddress(grenadeItem, Offsets.grenades);

            if (materialCheckbox28.Checked)
            {
                mem.WriteInt(grenadeCount, 999999);
            }
            else
            {
                mem.WriteInt(grenadeCount, 1);
            }
        }

        private void materialCheckbox29_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var primaryGun = mem.ReadAddress(baseAddress, Offsets.PrimaryGun);
            var pressureDrop = mem.ReadAddress(primaryGun, Offsets.pressureDrop);
            if (materialCheckbox29.Checked)
            {
                mem.WriteFloat(pressureDrop, 0f);
            }
            else
            {
                mem.WriteFloat(pressureDrop, 1f);

            }
        }

        private void materialCheckbox30_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var inventory = mem.ReadAddress(baseAddress, Offsets.inventoryComponent);
            var sentry = mem.ReadAddress(inventory, Offsets.recallableSentryGun);
            var maxSentry = mem.ReadAddress(sentry, Offsets.maxSentryCount);

            if (materialCheckbox30.Checked)
            {
                mem.WriteInt(maxSentry, 99999);
            }
            else
            {
                mem.WriteInt(maxSentry, 1);
            }
        }

        private void materialCheckbox31_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var inventory = mem.ReadAddress(baseAddress, Offsets.inventoryComponent);
            var sentry = mem.ReadAddress(inventory, Offsets.recallableSentryGun);
            var itemPlacer = mem.ReadAddress(sentry, Offsets.itemPlacer);
            var distance = mem.ReadAddress(itemPlacer, Offsets.placementDistance);

            if (materialCheckbox31.Checked)
            {
                mem.WriteFloat(distance, 999999f);
            }
            else
            {
                mem.WriteFloat(distance, 100f);
            }
        }

        private void materialCheckbox32_CheckedChanged_1(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var inventory = mem.ReadAddress(baseAddress, Offsets.inventoryComponent);
            var sentry = mem.ReadAddress(inventory, Offsets.recallableSentryGun);
            var ammoCapComp = mem.ReadAddress(sentry, Offsets.ammoCapacity);
            var ammoCount = mem.ReadAddress(ammoCapComp, Offsets.ammoCountSent);

            if (materialCheckbox32.Checked)
            {
                mem.WriteInt(ammoCount, 9999999);
            }
            else
            {
                mem.WriteInt(ammoCount, 300);
            }
        }


        private void materialCheckbox15_CheckedChanged(object sender, EventArgs e)
        {
            MemoryReader mem = new MemoryReader(game);
            var character = mem.ReadAddress(baseAddress, Offsets.character);
            var moveComponent = mem.ReadAddress(character, Offsets.characterMovementComponent);
            var brakingDecel = mem.ReadAddress(moveComponent, Offsets.BrakingDecelerationFalling);
            if (materialCheckbox15.Checked)
            {


                mem.WriteFloat(brakingDecel, 5000);
            }
            else
            {
                mem.WriteFloat(brakingDecel, 0);
            }
        }

        private void materialButton9_Click(object sender, EventArgs e)
        {
            try
            {
                MemoryReader mem = new MemoryReader(game);
                var runSpeedAddress = mem.ReadAddress(baseAddress, Offsets.runSpeed);
                var walkSpeedAddress = mem.ReadAddress(baseAddress, Offsets.walkSpeed);

                mem.WriteFloat(runSpeedAddress, (float)Convert.ToDouble(materialMultiLineTextBox9.Text));
                mem.WriteFloat(walkSpeedAddress, (float)Convert.ToDouble(materialMultiLineTextBox9.Text));

            }
            catch
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Error: Please enter a valid float.");
            }
        }

        private void materialCheckbox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckbox7_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
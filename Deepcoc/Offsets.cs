using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepcoc
{
    internal class Offsets
    {
        public static int[]
            SecondaryGun = { 0x06416250, 0x0, 0xA0, 0xAC8, 0xB0, 0x8, 0x684 },
            PrimaryGun = { 0x06416250, 0x0, 0xA0, 0x120, 0x8, 0x684 },
            yCoord = { 0x06416250, 0x0, 0x20, 0x130, 0x1D8},
            xCoord = { 0x06416250, 0x0, 0xA0, 0x290, 0x1D0 },
            zCoord = { 0x06416250 , 0x0, 0x20, 0x130, 0x1D0};

        public static int
            fireRate = 0xA2,
            currentAmmo = 0x4;
    }
}

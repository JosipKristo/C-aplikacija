using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SeminarskiRad
{
    class MotorDC
    {
        [DllImport("mtb.dll")]
        public static extern bool InitMotoBee();

        [DllImport("mtb.dll")]
        public static extern bool SetMotors(int on1, int speed1, int on2, int speed2, int on3, int speed3, int on4, int speed4, int servo);
    }
}

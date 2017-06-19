using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SeminarskiRad
{
    class SenzorZvuka
    {
        [DllImport("mtb.dll")]
        public static extern bool InitMotoBee();

        [DllImport("mtb.dll")]
        public static extern bool Digital_IO(ref int inputs, int outputs);

        public static int DohvatiInput()
        {
            int SenzorZvuka = 0;
            Digital_IO(ref SenzorZvuka, 0);
            return SenzorZvuka;
        }
    }
}

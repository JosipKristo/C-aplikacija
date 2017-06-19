using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SeminarskiRad
{
    class WASP
    {
        [DllImport("wsp.dll")]
        public static extern bool InitWasp();

        [DllImport("wsp.dll")]
        public static unsafe extern int ReadInputs(int* analog);

        [DllImport("wsp.dll")]
        public static extern void SetOutputs(int bits);


        public static int ProcitajInput()                    // Citanje inteziteta svijetlosti sa senzora
        {
            int[] vrijednosti = new int[4];
            unsafe
            {
                fixed (int* pokazivac = &vrijednosti[0])
                {
                    ReadInputs(pokazivac);
                }

            }

            return vrijednosti[0];

        }

    }
}

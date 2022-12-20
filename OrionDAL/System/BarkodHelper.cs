using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class BarkodHelper
    {
        public static string KontrolKoduEkleEAN8(string barkod)
        {
            if (barkod.Length != 7)
            {
                throw new ApplicationException("Barkod uzunluğu olması gerektiği gibi değil.\r\nSistem ayarlarındaki ülke kodunu kontrol ediniz.");
            }

            int toplam = (3 * int.Parse(barkod[0].ToString()))
                + (1 * int.Parse(barkod[1].ToString()))
                + (3 * int.Parse(barkod[2].ToString()))
                + (1 * int.Parse(barkod[3].ToString()))
                + (3 * int.Parse(barkod[4].ToString()))
                + (1 * int.Parse(barkod[5].ToString()))
                + (3 * int.Parse(barkod[6].ToString()))
                ;
            int modul = toplam % 10;
            modul = 10 - modul;
            if (modul == 10) modul = 0;

            barkod += modul;

            return barkod;
        }

        public static string KontrolKoduEkleEAN13(string barkod)
        {
            if (barkod.Length != 12)
            {
                throw new ApplicationException("Barkod uzunluğu olması gerektiği gibi değil.\r\nSistem ayarlarındaki ülke ve firma kodunu kontrol ediniz.");
            }

            int toplam = (1 * int.Parse(barkod[0].ToString()))
                + (3 * int.Parse(barkod[1].ToString()))
                + (1 * int.Parse(barkod[2].ToString()))
                + (3 * int.Parse(barkod[3].ToString()))
                + (1 * int.Parse(barkod[4].ToString()))
                + (3 * int.Parse(barkod[5].ToString()))
                + (1 * int.Parse(barkod[6].ToString()))
                + (3 * int.Parse(barkod[7].ToString()))
                + (1 * int.Parse(barkod[8].ToString()))
                + (3 * int.Parse(barkod[9].ToString()))
                + (1 * int.Parse(barkod[10].ToString()))
                + (3 * int.Parse(barkod[11].ToString()))
                ;
            int modul = toplam % 10;
            modul = 10 - modul;
            if (modul == 10) modul = 0;

            barkod += modul;

            return barkod;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitnessotomasyonu
{
    internal class Veri
    {
        public class Users
        {
            public string Uid { get; set; } 
            public string Uad { get; set; }
            public string Usoyad { get; set; }
            public int Uyas { get; set; }
            public string Utel { get; set; } 
            public string Usifre { get; set; } 
            public string Ucinsiyet { get; set; } 
            public string Uuyelik { get; set; } 
        }
        public static class giris
        {
            public static string GirisYapanAd { get; set; }
            public static string GirisYapanSoyad { get; set; }
            public static string GirisYapanYas { get; set; }
            public static string GirisYapanTelefon { get; set; }
            public static string GirisYapanSifre { get; set; }
            public static string GirisYapanCinsiyet { get; set; }
            public static string GirisYapanUyelik { get; set; }
        }
    }
}

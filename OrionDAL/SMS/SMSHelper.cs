using Newtonsoft.Json;
using OrionDAL;
using OrionDAL.OAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrionDAL.SMS
{
    /// <summary>
    /// sms entegrasyonu
    /// </summary>
    public class SMSHelper
    {
        public static IOrionSMSProvider GetProvider(string fullType)
        {
            var type = Type.GetType(fullType);
            if(type == null)
            {
                throw new ApplicationException("SMS dll'i bulunamadı! " + fullType);
            }

            IOrionSMSProvider provider = (IOrionSMSProvider)Activator.CreateInstance(type);
            return provider;
        }
    }

    public class SorguSonucu
    {
        public string campaign_id { get; set; }
        public string direction { get; set; }
        public string campaign_custom_id { get; set; }
        public string size { get; set; }
        public string international_multiplier { get; set; }
        public string credits { get; set; }
        public string status { get; set; }
        public string gsm_error { get; set; }
        public string sent_at { get; set; }
        public string done_at { get; set; }
    }

    public class Sonuc
    {
        public bool Hata { get; set; }
        public string BulkId { get; set; }
        public string SmsId { get; set; }
        public string To { get; set; }
        public SMSDurum Durum { get; set; }
        public string HataMesaji { get; set; }
        public int SMSSayisi { get; set; }
        public SorguSonucu SorguSonucu { get; set; }

        public Sonuc()
        {
            Hata = false;
            SorguSonucu = new SMS.SorguSonucu();
        }
    }

    public class Mesaj
    {
        public string Message { get; set; }
        public string To { get; set; }

        public Mesaj() { }

        public Mesaj(string message, string to)
        {
            this.Message = message;
            this.To = to;
        }
    }

    public class SMSHesapBilgisi
    {
        public string customerCode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string source_addr { get; set; }
        public string Url { get; set; }
    }

    public class HesapBakiyesi
    {
        public decimal Bakiye { get; set; }
        public string ParaBirimi { get; set; }
    }

    public class SMSRapor
    {
        public string BulkId { get; set; }
        public string SMSId { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime DoneAt { get; set; }
        public int SmsCount { get; set; }
        public string MccMnc { get; set; }
        public Price Price { get; set; }
        public SMSDurum Status { get; set; }
        public string Desciription { get; set; }
        public string CallbackData { get; set; }
    }

    public class Price
    {
        public decimal? PricePerMessage { get; set; }

        public decimal? PricePerLookup { get; set; }

       public string Currency { get; set; }
    }


    public enum SMSDurum
    {
        /// <summary>
        /// servise gönderilecek
        /// </summary>
        Gonderilecek = 0,

        /// <summary>
        /// servise gönderildi
        /// </summary>
        KabulEdildi = 1,

        Bekleyen = 2,

        /// <summary>
        /// Telefona iletildi
        /// </summary>

        Iletildi = 3,
        Iletilemedi = 4,
        SuresiDoldu = 5,
        Reddedildi = 6,
        HataliMesaj = 7,
        ServisHatasi = 8
    }
}
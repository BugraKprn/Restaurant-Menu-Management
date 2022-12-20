using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace System
{
    /// <summary>
    /// Verimor firması, sms entegrasyonu
    /// Kodun kaynağı: https://github.com/verimor/SMS-API/blob/master/sample_codes/C%23/WindowsFormsApplication1/Form1.cs
    /// </summary>
    public class VerimorSmsHelper
    {
        public static Sonuc SMSGonder(SmsIstegi istek)
        {
            string payload = JsonConvert.SerializeObject(istek);

            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";

            try
            {
                string campaign_id = wc.UploadString("http://sms.verimor.com.tr/v2/send.json", payload);
                return new Sonuc() {
                    Hata = false,
                    SmsId = campaign_id                    
                }; // Mesaj gönderildi, kampanya id
            }
            catch (WebException ex) // 400 hatalarında response body'de hatanın ne olduğunu yakalıyoruz
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) // 400 hataları
                {
                    var responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    return new Sonuc()
                    {
                        Hata = true,
                        HataMesaji = "Mesaj gönderilemedi, dönen hata: " + responseBody

                    };

                }
                else // diğer hatalar
                {
                    return new Sonuc()
                    {
                        Hata = true,
                        HataMesaji = "Mesaj gönderilemedi, dönen hata: " + ex.Status
                    };
                }
            }
            
        }

        public static Sonuc SmsSorgula(string smsId, string kullaniciAdi, string sifre)
        {
            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json";

            try
            {
                string result = wc.DownloadString("http://sms.verimor.com.tr/v2/status?id=" + smsId + "&username=" + kullaniciAdi + "&password=" + sifre);
                var sonuclar = JsonConvert.DeserializeObject<List<SorguSonucu>>(result);
                return new Sonuc() { SorguSonucu = sonuclar[0] }; // şu an tek sms sorgulanabildiği için tek cevap geldiğini varsayıyoruz.
            }
            catch (WebException ex) // 400 hatalarında response body'de hatanın ne olduğunu yakalıyoruz
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) // 400 hataları
                {
                    var responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    return new Sonuc()
                    {
                        Hata = true,
                        HataMesaji = "Mesaj gönderilemedi, dönen hata: " + responseBody
                    };
                }
                else // diğer hatalar
                {
                    return new Sonuc()
                    {
                        Hata = true,
                        HataMesaji = "Mesaj gönderilemedi, dönen hata: " + ex.Status
                    };
                }
            }
        }

        public class SorguSonucu
        {
            public string campaign_id { get; set; }
            public string direction { get; set; }
            public string campaign_custom_id { get; set; }
            public string message_id { get; set; }
            public string dest { get; set; }
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
            public string SmsId { get; set; }
            public string HataMesaji { get; set; }
            public SorguSonucu SorguSonucu { get; set; }
        }

        public class Mesaj
        {
            public string msg { get; set; }
            public string dest { get; set; }

            public Mesaj() { }

            public Mesaj(string msg, string dest)
            {
                this.msg = msg;
                this.dest = dest;
            }
        }

        public class SmsIstegi
        {
            public string username { get; set; }
            public string password { get; set; }
            public string source_addr { get; set; }
            public Mesaj[] messages { get; set; }
        }
    }
}
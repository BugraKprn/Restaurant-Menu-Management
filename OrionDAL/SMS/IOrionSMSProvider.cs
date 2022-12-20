using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrionDAL.SMS
{
    public interface IOrionSMSProvider
    {
        Sonuc SMSGonderBirNumaraBirMesaj(SMSHesapBilgisi smsHesapBilgisi, string to, string message);
       
        List<Sonuc> SMSGonderBirNumaraBirdenFazlaMesaj(SMSHesapBilgisi smsHesapBilgisi, string to, List<string> messageList);

        List<Sonuc> SMSGonderBirdenFazlaNumaraBirMesaj(SMSHesapBilgisi smsHesapBilgisi, List<string> toList, string message);

        List<Sonuc> SMSGonderBirdenFazlaNumaraBirdenFazlaMesaj(SMSHesapBilgisi smsHesapBilgisi, List<string> toList, List<string> messageList);

        HesapBakiyesi GetBakiyeBilgisi(SMSHesapBilgisi smsHesapBilgisi);

        SMSRapor GetIletimRaporu(SMSHesapBilgisi smsHesapBilgisi, string smsId);
    }
}

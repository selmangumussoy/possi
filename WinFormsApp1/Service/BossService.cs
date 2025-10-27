using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp1.Service
{
    public static class BossService
    {
        // Form1'de switch'in çağırdığı tek giriş noktası
        public static HubMessage HandleRequest(HubMessage request)
        {
            switch (request.MessageRequestCode)
            {
                case "GUNSONUOZET":
                    return HandleGunSonuOzet(request);

                case "SATISLAR":
                    return HandleSatislar(request);

                case "ACIKSATISLAR":
                    return HandleAcikSatislar(request);

                case "ONLINESIPARISOZET":
                    return HandleOnlineSiparisOzet(request);

                default:
                    return BuildErr(request, $"BossService: Desteklenmeyen kod: {request.MessageRequestCode}");
            }
        }

        // --- Handlers (şimdilik dummy, gerçek veriyle değiştir) ---

        private static HubMessage HandleGunSonuOzet(HubMessage request)
        {
            var prm  = request.GetMessageParams();
            var bas  = DateTime.Parse(prm.First(p => p.ParamName == "bastarih").ParamValue);
            var son  = DateTime.Parse(prm.First(p => p.ParamName == "sontarih").ParamValue);

            var ozet = new[]
            {
                new { Tarih = bas.ToString("yyyy-MM-dd"), Toplam = 1234.56m },
                new { Tarih = son.ToString("yyyy-MM-dd"), Toplam = 2345.67m }
            };

            return BuildRes(request, ozet);
        }

        private static HubMessage HandleSatislar(HubMessage request)
        {
            var prm = request.GetMessageParams();
            var bas = DateTime.Parse(prm.First(p => p.ParamName == "bastarih").ParamValue);
            var son = DateTime.Parse(prm.First(p => p.ParamName == "sontarih").ParamValue);

            var satislar = new
            {
                SatisDetaylari = new[]
                {
                    new { Urun = "Hamburger", Adet = 10, Tutar = 1200m },
                    new { Urun = "Pizza",     Adet =  7, Tutar = 1400m },
                },
                Dukkan = 1800m,
                Paket  = 500m,
                Online = 300m,
                Toplam = 2600m
            };

            return BuildRes(request, satislar);
        }

        private static HubMessage HandleAcikSatislar(HubMessage request)
        {
            var prm = request.GetMessageParams();
            var bas = DateTime.Parse(prm.First(p => p.ParamName == "bastarih").ParamValue);
            var son = DateTime.Parse(prm.First(p => p.ParamName == "sontarih").ParamValue);

            var acik = new[]
            {
                new { MasaNo = "A1", Tutar = 250m, Tarih = bas.ToString("yyyy-MM-dd") },
                new { MasaNo = "B3", Tutar = 300m, Tarih = son.ToString("yyyy-MM-dd") }
            };

            return BuildRes(request, acik);
        }

        private static HubMessage HandleOnlineSiparisOzet(HubMessage request)
        {
            var prm = request.GetMessageParams();
            var bas = DateTime.Parse(prm.First(p => p.ParamName == "bastarih").ParamValue);
            var son = DateTime.Parse(prm.First(p => p.ParamName == "sontarih").ParamValue);

            var ozet = new[]
            {
                new { Kanal = "Getir",   Siparis = 12, Tutar = 900m },
                new { Kanal = "Yemeksepeti", Siparis = 9, Tutar = 720m }
            };

            return BuildRes(request, ozet);
        }

        // --- Helpers ---

        private static HubMessage BuildRes(HubMessage req, object body)
        {
            return new HubMessage
            {
                MessageFromUser    = req.MessageToUser,
                MessageToUser      = req.MessageFromUser,
                MessageType        = "RES",
                MessageRequestCode = req.MessageRequestCode,
                MessageSubject     = req.MessageSubject,
                MessageParams      = req.MessageParams,
                MessageBody        = JsonConvert.SerializeObject(body)
            };
        }

        private static HubMessage BuildErr(HubMessage req, string error)
        {
            return new HubMessage
            {
                MessageFromUser    = req.MessageToUser,
                MessageToUser      = req.MessageFromUser,
                MessageType        = "ERR",
                MessageRequestCode = req.MessageRequestCode,
                MessageSubject     = "Boss hata",
                MessageParams      = req.MessageParams,
                MessageBody        = error
            };
        }
    }
}

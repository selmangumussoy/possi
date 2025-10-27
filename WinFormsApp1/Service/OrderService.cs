using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WinFormsApp1.Service
{
    public static class OrderService
    {
        // Form1'de switch'in çağırdığı tek giriş noktası
        public static HubMessage HandleRequest(HubMessage request)
        {
            switch (request.MessageRequestCode)
            {
                case "RESTAURANTMENU":
                    return HandleRestaurantMenu(request);

                case "WEBORDER":
                    return HandleWebOrder(request);

                default:
                    return BuildErr(request, $"OrderService: Desteklenmeyen kod: {request.MessageRequestCode}");
            }
        }

        // --- Handlers ---

        private static HubMessage HandleRestaurantMenu(HubMessage request)
        {
            // İstenirse parametre kullan
            var prm = request.GetMessageParams();
            string grup = prm.Find(p => p.ParamName == "menugrup")?.ParamValue ?? "Default";

            var menu = new List<object>
            {
                new { Id = 1, Grup = grup, Ad = "Hamburger", Fiyat = 120m },
                new { Id = 2, Grup = grup, Ad = "Pizza",      Fiyat = 200m },
                new { Id = 3, Grup = grup, Ad = "Salata",     Fiyat = 90m  }
            };

            return BuildRes(request, menu);
        }

        private static HubMessage HandleWebOrder(HubMessage request)
        {
            // Örnek: Body'den siparişi al
            var orderDynamic = request.GetBodyObject<dynamic>();
            long siparisId = DateTime.Now.Ticks; // DB insert sonrası dönen Id gibi

            var msg = new { SiparisId = siparisId, Mesaj = $"{siparisId} nolu siparişiniz alındı." };
            return BuildRes(request, msg);
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
                MessageSubject     = "Order hata",
                MessageParams      = req.MessageParams,
                MessageBody        = error
            };
        }
    }
}

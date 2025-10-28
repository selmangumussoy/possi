using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1.Service
{
    public static class BossService
    {
        // Request yönlendirme
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

        // Gün Sonu Özeti
        private static HubMessage HandleGunSonuOzet(HubMessage request)
        {
            var prm = request.GetMessageParams();
            
            // Web'den gelen: StartDate ve EndDate
            // Eski kod: bastarih ve sontarih (case insensitive)
            var basStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("StartDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("bastarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;
            
            var sonStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("EndDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("sontarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(basStr) || string.IsNullOrEmpty(sonStr))
            {
                return BuildErr(request, "StartDate ve EndDate parametreleri gereklidir.");
            }

            var bas = DateTime.Parse(basStr);
            var son = DateTime.Parse(sonStr);


            var result = new List<object>();

            try
            {
                using var conn = new SqlConnection(AppSession.DbConnectionString);
                conn.Open();

                string sql = @"
                    SELECT CAST(DeliveryTime AS DATE) AS Tarih, SUM(OrderTotal) AS Toplam
                    FROM ys_Orders
                    WHERE DeliveryTime BETWEEN @Bas AND @Son
                      AND StatusId = 12 -- Kapalı siparişler
                    GROUP BY CAST(DeliveryTime AS DATE)
                    ORDER BY Tarih;";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Bas", bas);
                cmd.Parameters.AddWithValue("@Son", son);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(new
                    {
                        Tarih = ((DateTime)rdr["Tarih"]).ToString("yyyy-MM-dd"),
                        Toplam = Convert.ToDecimal(rdr["Toplam"])
                    });
                }
            }
            catch (Exception ex)
            {
                return BuildErr(request, $"DB Hatası: {ex.Message}");
            }

            return BuildRes(request, result);
        }

        // Satışlar (Ürün bazlı)
        private static HubMessage HandleSatislar(HubMessage request)
        {
            var prm = request.GetMessageParams();
            
            var basStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("StartDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("bastarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;
            
            var sonStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("EndDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("sontarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(basStr) || string.IsNullOrEmpty(sonStr))
            {
                return BuildErr(request, "StartDate ve EndDate parametreleri gereklidir.");
            }

            var bas = DateTime.Parse(basStr);
            var son = DateTime.Parse(sonStr);


            var detayList = new List<object>();
            decimal toplam = 0;

            try
            {
                using var conn = new SqlConnection(AppSession.DbConnectionString);
                conn.Open();

                string sql = @"
                    SELECT op.Name AS Urun, SUM(op.Quantity) AS Adet, SUM(op.Price) AS Tutar
                    FROM ys_Orders o
                    INNER JOIN ys_OrderProducts op ON o.Id = op.OrderId
                    WHERE o.DeliveryTime BETWEEN @Bas AND @Son
                      AND o.StatusId = 12 -- Kapalı siparişler
                    GROUP BY op.Name
                    ORDER BY Tutar DESC;";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Bas", bas);
                cmd.Parameters.AddWithValue("@Son", son);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    detayList.Add(new
                    {
                        Urun = rdr["Urun"].ToString(),
                        Adet = Convert.ToInt32(rdr["Adet"]),
                        Tutar = Convert.ToDecimal(rdr["Tutar"])
                    });
                }

                // Toplam
                string sqlTotal = @"
                    SELECT SUM(OrderTotal) 
                    FROM ys_Orders
                    WHERE DeliveryTime BETWEEN @Bas AND @Son
                      AND StatusId = 12;";

                using var cmd2 = new SqlCommand(sqlTotal, conn);
                cmd2.Parameters.AddWithValue("@Bas", bas);
                cmd2.Parameters.AddWithValue("@Son", son);

                toplam = Convert.ToDecimal(cmd2.ExecuteScalar() ?? 0);
            }
            catch (Exception ex)
            {
                return BuildErr(request, $"DB Hatası: {ex.Message}");
            }

            var satislar = new
            {
                SatisDetaylari = detayList,
                Toplam = toplam
            };

            return BuildRes(request, satislar);
        }

        // Açık Satışlar
        private static HubMessage HandleAcikSatislar(HubMessage request)
        {
            var prm = request.GetMessageParams();
            
            var basStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("StartDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("bastarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;
            
            var sonStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("EndDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("sontarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(basStr) || string.IsNullOrEmpty(sonStr))
            {
                return BuildErr(request, "StartDate ve EndDate parametreleri gereklidir.");
            }

            var bas = DateTime.Parse(basStr);
            var son = DateTime.Parse(sonStr);


            var acik = new List<object>();

            try
            {
                using var conn = new SqlConnection(AppSession.DbConnectionString);
                conn.Open();

                string sql = @"
                    SELECT MasaKodu, OrderTotal, CAST(DeliveryTime AS DATE) AS Tarih
                    FROM ys_Orders
                    WHERE DeliveryTime BETWEEN @Bas AND @Son
                      AND StatusId = 4; -- Açık siparişler";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Bas", bas);
                cmd.Parameters.AddWithValue("@Son", son);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    acik.Add(new
                    {
                        MasaNo = rdr["MasaKodu"].ToString(),
                        Tutar = Convert.ToDecimal(rdr["OrderTotal"]),
                        Tarih = ((DateTime)rdr["Tarih"]).ToString("yyyy-MM-dd")
                    });
                }
            }
            catch (Exception ex)
            {
                return BuildErr(request, $"DB Hatası: {ex.Message}");
            }

            return BuildRes(request, acik);
        }

        // Online Sipariş Özeti
        private static HubMessage HandleOnlineSiparisOzet(HubMessage request)
        {
            var prm = request.GetMessageParams();
            
            var basStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("StartDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("bastarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;
            
            var sonStr = prm.FirstOrDefault(p => 
                p.ParamName.Equals("EndDate", StringComparison.OrdinalIgnoreCase) ||
                p.ParamName.Equals("sontarih", StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(basStr) || string.IsNullOrEmpty(sonStr))
            {
                return BuildErr(request, "StartDate ve EndDate parametreleri gereklidir.");
            }

            var bas = DateTime.Parse(basStr);
            var son = DateTime.Parse(sonStr);


            var ozet = new List<object>();

            try
            {
                using var conn = new SqlConnection(AppSession.DbConnectionString);
                conn.Open();

                string sql = @"
                    SELECT ProviderCode AS Kanal, COUNT(Id) AS Siparis, SUM(OrderTotal) AS Tutar
                    FROM ys_Orders
                    WHERE DeliveryTime BETWEEN @Bas AND @Son
                      AND StatusId = 12
                    GROUP BY ProviderCode;";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Bas", bas);
                cmd.Parameters.AddWithValue("@Son", son);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ozet.Add(new
                    {
                        Kanal = rdr["Kanal"].ToString(),
                        Siparis = Convert.ToInt32(rdr["Siparis"]),
                        Tutar = Convert.ToDecimal(rdr["Tutar"])
                    });
                }
            }
            catch (Exception ex)
            {
                return BuildErr(request, $"DB Hatası: {ex.Message}");
            }

            return BuildRes(request, ozet);
        }

        // Helper metotlar
        private static HubMessage BuildRes(HubMessage req, object body) =>
            new HubMessage
            {
                MessageFromUser = req.MessageToUser,
                MessageToUser = req.MessageFromUser,
                MessageType = "RES",
                MessageRequestCode = req.MessageRequestCode,
                MessageSubject = req.MessageSubject,
                MessageParams = req.MessageParams,
                MessageBody = JsonConvert.SerializeObject(body)
            };

        private static HubMessage BuildErr(HubMessage req, string error) =>
            new HubMessage
            {
                MessageFromUser = req.MessageToUser,
                MessageToUser = req.MessageFromUser,
                MessageType = "ERR",
                MessageRequestCode = req.MessageRequestCode,
                MessageSubject = "Boss hata",
                MessageParams = req.MessageParams,
                MessageBody = error
            };
    }
}

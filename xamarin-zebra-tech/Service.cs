using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xamarin_zebra_tech
{
    public static class Service
    {
        private static HttpClient GenerateHttpClient()
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            return httpClient;
        }

        public static async Task GetVehicleParkingStatus()
        {
            try
            {
                var messageBody = SecureHashCalculator.GetMessageBody("111942", "27", "Hourly", "20240327_66491450548", "CDJ4237", "2024-03-27T10:30:00.000", "2024-03-27T11:30:00.000", "0.6", "2024-03-27T10:30:00.000");
                var secureHashValue = SecureHashCalculator.CalculateSecureHash("111942", messageBody);
                var data = new
                {
                    secureHash = secureHashValue,
                    vendorID = "111942",
                    plateNo = "CDJ4237",
                    crc = SecureHashCalculator.CalculateCRCChecksum(secureHashValue, "111942", messageBody)
                };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                using (HttpClient client = GenerateHttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync("https://evocityapp.azurewebsites.net/vendor/viewActiveParking", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response: " + responseBody);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
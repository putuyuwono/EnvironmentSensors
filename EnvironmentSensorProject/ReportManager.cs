using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentSensorProject
{
    class ReportManager
    {
        public static async Task<String> PostAsync(String uri, String data)
        {
            String result = String.Empty;
            var httpClient = new HttpClient();
            try
            {                             
                var content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            httpClient.Dispose();
            return result;
        }
    }
}

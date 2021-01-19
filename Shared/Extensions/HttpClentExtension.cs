using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Shared.Extensions
{
    public static class HttpClentExtension
    {
        public static Task<HttpResponseMessage> PostFormDataAsync<T>(this HttpClient httpClient, string url,
            string token, T data)
        {
            try
            {
                var content = new MultipartFormDataContent();

                foreach (var prop in data.GetType().GetProperties())
                {
                    var value = prop.GetValue(data);
                    if (value is List<IFormFile>)
                    {
                        var files = value as List<IFormFile>;
                        foreach (var file in files.Where(file => file != null))
                        {
                            var streamContent = new StreamContent(file.OpenReadStream());
                            content.Add(streamContent, file.Name, file.FileName);
                            streamContent.Headers.ContentDisposition.FileNameStar = "";
                            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(JsonConvert.SerializeObject(value)), prop.Name);
                    }
                }

                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return httpClient.PostAsync(url, content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
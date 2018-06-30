﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestService;

namespace TestView
{
    public class ApiClient
    {
        private static HttpClient client = new HttpClient();
        public static string Role { get; set; }
        public static void Connect()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["IPAddress"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public static void Login(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "password", password )
                    };
            TokenResponse tokenResponse = null;
            try
            {
                tokenResponse = PostFormUrlEncoded<TokenResponse>("token", pairs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);
            Role = tokenResponse.UserRole;
        }

        public static U PostFormUrlEncoded<U>(string requestUrl, IEnumerable<KeyValuePair<string, string>> postData)
        {

            using (var content = new FormUrlEncodedContent(postData))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                HttpResponseMessage response = client.PostAsync(requestUrl, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<U>().Result;
                }
                else
                {
                    string error = response.Content.ReadAsStringAsync().Result;
                    var errorMessage = JsonConvert.DeserializeObject<HttpErrorMessage>(error);
                    throw new Exception(errorMessage.Error_description ?? "" + errorMessage.Message + "" +
                        errorMessage.MessageDetail ?? "" +
                        " " + errorMessage.ExceptionMessage ?? "");
                }
            }

        }
        private static async Task<HttpResponseMessage> GetRequest(string requestUrl)
        {
            return await client.GetAsync(requestUrl);
        }

        private static async Task<HttpResponseMessage> PostRequest<T>(string requestUrl, T model)
        {
            return await client.PostAsJsonAsync(requestUrl, model);
        }

        public static async Task<T> GetRequestData<T>(string requestUrl)
        {
            HttpResponseMessage response = Task.Run(() => GetRequest(requestUrl)).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else
            {
                string error = response.Content.ReadAsStringAsync().Result;
                throw new Exception(error);
            }
        }

        public static void PostRequestData<T>(string requestUrl, T model)
        {
            HttpResponseMessage response = Task.Run(() => PostRequest(requestUrl, model)).Result;
            if (!response.IsSuccessStatusCode)
            {
                string error = response.Content.ReadAsStringAsync().Result;
                var errorMessage = JsonConvert.DeserializeObject<HttpErrorMessage>(error);
                throw new Exception(errorMessage.Message + " " + (errorMessage.MessageDetail ?? "") +
                    " " + (errorMessage.ExceptionMessage ?? ""));
            }
        }

        public static async Task<U> PostRequestData<T, U>(string requestUrl, T model)
        {
            HttpResponseMessage response = Task.Run(() => PostRequest(requestUrl, model)).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<U>();
            }
            else
            {
                string error = response.Content.ReadAsStringAsync().Result;
                var errorMessage = JsonConvert.DeserializeObject<HttpErrorMessage>(error);
                throw new Exception(errorMessage.Message + " " + errorMessage.MessageDetail ?? "" +
                    " " + errorMessage.ExceptionMessage ?? "");
            }
        }

        public async static Task<HttpResponseMessage> PostRequest(string requestUrl)
        {
            return await client.PostAsync(requestUrl, null);
        }

        public async static void Logout()
        {
            HttpResponseMessage response = await PostRequest("api/account/logout");
        }
    }
}

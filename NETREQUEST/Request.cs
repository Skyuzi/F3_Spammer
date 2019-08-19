using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NETREQUEST
{
    public static class Request
    {
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

        public static string Get(CookieContainer cookies, string url)
        {
            var response = string.Empty;
            
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.KeepAlive = true;
            request.UserAgent = UserAgent;

            try
            {
                using (var resp = request.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    response = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch
            {
                response = "Bad";
            }

            return response;
        }
        
        public static string GetUserId(string html)
        {
            return Regex.Match(html, "userId:\"(.*?)\",").Groups[1].Value;
        }

        public static string Post(CookieContainer cookies, string url, string text, string proxy, int port)
        {
            string response = string.Empty;
            string requestText = $"text={text}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.KeepAlive = true;
            request.UserAgent = UserAgent;
            request.Accept = "application/json, text/plain, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = "https://f3.cool/skyuzi";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("x-app-version", "F3-Web/v1.9.3");

            var proxyServer = new WebProxy(proxy, port);
            request.Proxy = proxyServer;

            var bytes = Encoding.UTF8.GetBytes(requestText);
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var resp = request.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    response = sr.ReadToEnd();
                    sr.Close();
                }

            }
            catch { response = "bad"; }

            return response;
        }
    }
}
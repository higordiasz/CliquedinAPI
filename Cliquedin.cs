using System;
using System.Net;
using System.Net.Http;
using CliquedinAPI.Models.Conta;
using CliquedinAPI.Models.Retorno;

namespace CliquedinAPI
{
    public class Cliquedin
    {
        internal string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";
        internal HttpClient Client { get; set; }
        internal HttpClientHandler Handler { get; set; }
        internal CookieContainer Cookies { get; set; }
        internal string Token { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal Uri BasicUrl { get; set; }

        public Cliquedin(string username, string password)
        {
            this.BasicUrl = new Uri("https://cliquedin.app/api/");
            this.Cookies = new CookieContainer();
            this.Token = "";
            this.Handler = new HttpClientHandler
            {
                UseDefaultCredentials = false,
                UseCookies = true,
                CookieContainer = Cookies
            };
            this.Handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            this.Client = new HttpClient(handler: Handler, disposeHandler: true);
            this.Client.DefaultRequestHeaders.Add("UserAgent", UserAgent);
            Username = username;
            Password = password;
        }
    }
}

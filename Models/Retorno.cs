using Newtonsoft.Json;
using CliquedinAPI.Models.Conta;

namespace CliquedinAPI.Models.Retorno
{
    public class SendCadInstaCliquedin
    {
        [JsonProperty("conta")]
        public string Username { get; set; }
        [JsonProperty("site")]
        public int Site = 1;
        [JsonProperty("user_pkid")]
        public dynamic PKID { get; set; }
    }

    public class CliquedinRetorno
    {
        public int Status { get; set; }
        public string Response { get; set; }
        public dynamic Json { get; set; }
    }

    public class TaskRetorno
    {
        public int Status { get; set; }
        public string Response { get; set; }
        public dynamic Json { get; set; }
        public string Tipo { get; set; }
    }

    public class ContaRetorno
    {
        public int Status { get; set; }
        public string Response { get; set; }
        public dynamic Json { get; set; }
        public ContaCliquedin Conta { get; set; }
    }
}

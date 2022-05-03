using CliquedinAPI.Models.Retorno;
using CliquedinAPI.Models.Conta;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace CliquedinAPI.Controllers
{
    public static class Controller
    {
        public static async Task<CliquedinRetorno> Login(this Cliquedin cliquedin)
        {
            CliquedinRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (File.Exists($@"{dir}/Config/{cliquedin.Username.Split("@")[0]}.arka"))
                {
                    string token = File.ReadAllText($@"{dir}/Config/{cliquedin.Username.Split("@")[0]}.arka");
                    cliquedin.Token = token;
                    ret.Status = 1;
                    ret.Response = "Sucesso ao efetuar login na plataforma";
                    cliquedin.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    return ret;
                }
                else
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cliquedin.BasicUrl + $"users/auth?email={cliquedin.Username}&password={cliquedin.Password}");
                    cliquedin.Client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                    if (res.IsSuccessStatusCode)
                    {
                        string serializado = await res.Content.ReadAsStringAsync();
                        Console.WriteLine(serializado);
                        try
                        {
                            dynamic token = JsonConvert.DeserializeObject(serializado);
                            if (token.token != null)
                            {
                                cliquedin.Token = token.token;
                                ret.Status = 1;
                                ret.Response = "Sucesso ao efetuar login na plataforma";
                                ret.Json = token;
                                cliquedin.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.token}");
                                //File.WriteAllText($@"{dir}/Config/{Username.Split("@")[0]}.arka", token.token.ToString());
                                return ret;
                            }
                            else
                            {
                                ret.Status = 2;
                                ret.Response = "Não foi possivel realizar login na plataforma";
                                return ret;
                            }
                        }
                        catch
                        {
                            ret.Status = 2;
                            ret.Response = "Não foi possivel realizar login na plataforma";
                            return ret;
                        }
                    }
                    else
                    {
                        ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                        string serializado = await res.Content.ReadAsStringAsync();
                        Console.WriteLine(serializado);
                        ret.Status = 0;
                        return ret;
                    }
                }
            }
            catch (Exception err)
            {
                ret.Status = -1;
                ret.Response = "Error: " + err.Message;
            }
            return ret;
        }
    
        public static async Task<TaskRetorno> GetCommentTask(this Cliquedin cliquedin, string contaID)
        {
            TaskRetorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cliquedin.BasicUrl + $"postFollowers?profile={contaID}&type=3");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.id != null)
                        {
                            ret.Status = 1;
                            ret.Response = "Tarefa encontrada";
                            ret.Json = token;
                            ret.Tipo = "comentar";
                            return ret;
                        }
                        else
                        {
                            LogCliquedin("Retorno: " + serializado, "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                            if (serializado.IndexOf("\"error\"") <= -1)
                            {
                                ret.Status = 7;
                                ret.Response = "Não foi possivel encontrar tarefa";
                                return ret;
                            }
                            ret.Status = 2;
                            ret.Response = "Não foi possivel encontrar tarefa";
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin("Erro: " + err.Message + " || Retorno: " + serializado, "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                        ret.Status = 2;
                        ret.Response = "Não foi possivel encontrar tarefa";
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin("Retorno: " + await res.Content.ReadAsStringAsync(), "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Status = -1;
                ret.Response = "Error: " + err.Message;
            }
            return ret;
        }

        public static async Task<TaskRetorno> GetTask(this Cliquedin cliquedin, string contaID)
        {
            TaskRetorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.id != null)
                        {
                            ret.Status = 1;
                            ret.Response = "Tarefa encontrada";
                            ret.Json = token;
                            ret.Tipo = token.profileAction.ToString();
                            return ret;
                        }
                        else
                        {
                            LogCliquedin("Retorno: " + serializado, "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                            if (serializado.IndexOf("\"error\"") <= -1)
                            {
                                ret.Status = 7;
                                ret.Response = "Não foi possivel encontrar tarefa";
                                return ret;
                            }
                            ret.Status = 2;
                            ret.Response = "Não foi possivel encontrar tarefa";
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin("Erro: " + err.Message + " || Retorno: " + serializado, "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                        ret.Status = 2;
                        ret.Response = "Não foi possivel encontrar tarefa";
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin("Retorno: " + await res.Content.ReadAsStringAsync(), "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Status = -1;
                ret.Response = "Error: " + err.Message;
            }
            return ret;
        }

        public static async Task<CliquedinRetorno> ConfirmTask (this Cliquedin cliquedin, string idTask, string contaID)
        {
            CliquedinRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cliquedin.BasicUrl + $"postFollowers/accept/{idTask}?profile={contaID}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.message == "success")
                        {
                            ret.Status = 1;
                            ret.Response = "Tarefa confirmada com sucesso";
                            ret.Json = token;
                            return ret;
                        }
                        else
                        {
                            LogCliquedin("Retorno: " + serializado, "CONFIRMTASK", cliquedin.BasicUrl + $"postFollowers/accept/{idTask}?profile={contaID}");
                            ret.Status = 2;
                            ret.Response = "Não foi possivel confirmar a tarefa";
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin("Erro: " + err.Message + " || Retorno: " + serializado, "CONFIRMTASK", cliquedin.BasicUrl + $"postFollowers/accept/{idTask}?profile={contaID}");
                        ret.Status = 2;
                        ret.Response = "Não foi possivel confirmar a tarefa";
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin("Retorno: " + res.Content.ReadAsStringAsync().Result, "CONFIRMTASK", cliquedin.BasicUrl + $"postFollowers/accept/{idTask}?profile={contaID}");
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Response = "Error: " + err.Message;
                ret.Status = -1;
            }
            return ret;
        }

        public static async Task<CliquedinRetorno> JumpTask(this Cliquedin cliquedin, string idTask, string contaID)
        {
            CliquedinRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cliquedin.BasicUrl + $"postFollowers/next/{idTask}?profile={contaID}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.message == "success")
                        {
                            ret.Status = 1;
                            ret.Response = "Tarefa pulada com sucesso";
                            ret.Json = token;
                            return ret;
                        }
                        else
                        {
                            ret.Status = 2;
                            ret.Response = "Não foi possivel pular a tarefa";
                            return ret;
                        }
                    }
                    catch
                    {
                        ret.Status = 2;
                        ret.Response = "Não foi possivel pular a tarefa";
                        return ret;
                    }
                }
                else
                {
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Response = "Error: " + err.Message;
                ret.Status = -1;
            }
            return ret;
        }

        public static async Task<CliquedinRetorno> AccountBlock(this Cliquedin cliquedin, string username, int type)
        {
            CliquedinRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                HttpRequestMessage request = new(HttpMethod.Post, $"https://cliquedin.app/api/profiles/block/{username}?type={type}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.message == "success")
                        {
                            ret.Status = 1;
                            ret.Response = "Bloqueio Adicionado";
                            ret.Json = token;
                            return ret;
                        }
                        else
                        {
                            ret.Status = 2;
                            ret.Response = "Não foi possivel adicionar bloqueio no perfil";
                            return ret;
                        }
                    }
                    catch
                    {
                        ret.Status = 2;
                        ret.Response = "Não foi possivel adicionar bloqueio no perfil";
                        return ret;
                    }
                }
                else
                {
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Response = "Error: " + err.Message;
                ret.Status = -1;
            }
            return ret;
        }

        public static async Task<bool> RegisteAccount(this Cliquedin cliquedin, string username, string genero, string contaID)
        {
            bool ret = false;
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cliquedin.BasicUrl + $"profiles?profile_id={contaID}&name={username.ToLower()}&genre={genero}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.message == "success")
                        {
                            return true;
                        }
                        else
                        {
                            LogCliquedin(serializado, "CADASTRARCONTA", $"| URL: " + cliquedin.BasicUrl + $"profiles?profile_id={contaID}&name={username.ToLower()}&genre={genero}");
                            return false;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin($"Erro: {err.Message} || Retorno: {await res.Content.ReadAsStringAsync()}", "CADASTRARCONTA", $"| URL: " + cliquedin.BasicUrl + $"profiles?profile_id={contaID}&name={username.ToLower()}&genre={genero}");
                        return false;
                    }
                }
                else
                {
                    LogCliquedin(await res.Content.ReadAsStringAsync(), "CADASTRARCONTA", $"| URL: " + cliquedin.BasicUrl + $"profiles?profile_id={contaID}&name={username.ToLower()}&genre={genero}");
                    return false;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        public static async Task SendPrivateOrNotExistTask(this Cliquedin cliquedin, string idTask)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cliquedin.BasicUrl + $"postFollowers/cancel/{idTask}");
            HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                string serializado = await res.Content.ReadAsStringAsync();
                try
                {
                    dynamic token = JsonConvert.DeserializeObject(serializado);
                    if (token.message == "success")
                    {
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public static async Task<CliquedinRetorno> SendBlockTemp(this Cliquedin cliquedin, string contaID)
        {
            CliquedinRetorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cliquedin.BasicUrl + $"profiles/blockTemp?profile={contaID}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.message == "success")
                        {
                            ret.Status = 1;
                            ret.Response = "Conta enviada";
                            return ret;
                        }
                        else
                        {
                            ret.Status = 2;
                            ret.Response = "Não foi possivel enviar a requisição";
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin("Erro: " + err.Message + " || Retorno: " + serializado, "SendBlockTemp", cliquedin.BasicUrl + $"profiles/blockTemp?profile={contaID}");
                        ret.Status = 2;
                        ret.Response = "Não foi possivel enviar a requisição";
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin("Retorno: " + await res.Content.ReadAsStringAsync(), "gettask", cliquedin.BasicUrl + $"postFollowers?profile={contaID}");
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Status = -1;
                ret.Response = "Error: " + err.Message;
            }
            return ret;
        }

        public static async Task<CliquedinRetorno> GetAccountID(this Cliquedin cliquedin, string username)
        {
            CliquedinRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cliquedin.BasicUrl + $"profiles/{username.ToLower()}");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.Count > 0)
                        {
                            ret.Response = token[0].id.ToString();
                            ret.Status = 1;
                            return ret;
                        }
                        else
                        {
                            LogCliquedin($"Resposta: {serializado}", "CHECKACCOUNT", cliquedin.BasicUrl + $"profiles/{username.ToLower()}");
                            ret.Status = 0;
                            ret.Response = "Impossivel localizar a conta";
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin($"Erro: {err.Message} || Resposta: {serializado}", "CHECKACCOUNT", cliquedin.BasicUrl + $"profiles/{username.ToLower()}");
                        ret.Status = 0;
                        ret.Response = "Impossivel localizar a conta";
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin($"Resposta: {res.Content.ReadAsStringAsync().Result}", "CHECKACCOUNT", cliquedin.BasicUrl + $"profiles/{username.ToLower()}");
                    ret.Status = 0;
                    ret.Response = "Impossivel localizar a conta";
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Response = "Error: " + err.Message;
                ret.Status = -1;
            }
            return ret;
        }

        public static async Task<ContaRetorno> GetAccount(this Cliquedin cliquedin)
        {
            ContaRetorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://cliquedin.app/api/useAccounts");
                //Client.DefaultRequestHeaders.Remove("Accept");
                //Client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.Count > 0)
                        {
                            ret.Status = 1;
                            ret.Response = "Contas Carregadas";
                            ret.Json = token;
                            ret.Conta = new()
                            {
                                Password = token[0].password,
                                Username = token[0].profile,
                                Imap_ID = token[0].imap_server != null ? token[0].imap_server != "" ? $"{token[0].imap_server}" : "" : "",
                                Imap_Port = token[0].imap_port.ToString() != null ? token[0].imap_port.ToString() != "" ? $"{token[0].imap_port}" : "" : "",
                                Email = token[0].imap_username != null ? token[0].imap_username != "" ? $"{token[0].imap_username}" : "" : "",
                                Email_Password = token[0].imap_password != null ? token[0].imap_password != "" ? $"{token[0].imap_password}" : "" : "",
                                Imap_SSL = true,
                                ContaID = ""
                            };
                            ret.Conta.Verificar_Email = ret.Conta.Imap_ID != "" && ret.Conta.Imap_Port != "" && ret.Conta.Email != "" && ret.Conta.Email_Password != "";
                            return ret;
                        }
                        else
                        {
                            ret.Status = 2;
                            ret.Response = "Não foi possivel carregar as contas do instagram";
                            return ret;
                        }
                    }
                    catch
                    {
                        ret.Status = 2;
                        ret.Response = "Não foi possivel carregar as contas do instagram";
                        return ret;
                    }
                }
                else
                {
                    ret.Response = "Não foi possivel se comunicar com a API do Cliquedin no momento";
                    ret.Status = 0;
                    return ret;
                }
            }
            catch (Exception err)
            {
                ret.Response = "Error: " + err.Message;
                ret.Status = -1;
            }
            return ret;
        }

        public static async Task<List<string>> GetProxy(this Cliquedin cliquedin)
        {
            //https://cliquedin.app/api/postFollowers/proxy
            List<string> ret = new();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://cliquedin.app/api/postFollowers/proxy");
                HttpResponseMessage res = await cliquedin.Client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    string serializado = await res.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic token = JsonConvert.DeserializeObject(serializado);
                        if (token.ip != null)
                        {
                            ret.Add(token.ip);
                            ret.Add(token.port);
                            ret.Add(token.user);
                            ret.Add(token.password);
                            return ret;
                        }
                        else
                        {
                            LogCliquedin($"Resposta: {serializado}", "GETPROXY", "https://cliquedin.app/api/postFollowers/proxy");
                            return ret;
                        }
                    }
                    catch (Exception err)
                    {
                        LogCliquedin($"Erro: {err.Message} || Resposta: {serializado}", "GETPROXY", "https://cliquedin.app/api/postFollowers/proxy");
                        return ret;
                    }
                }
                else
                {
                    LogCliquedin($"Resposta: {res.Content.ReadAsStringAsync().Result}", "GETPROXY", "https://cliquedin.app/api/postFollowers/proxy");
                    return ret;
                }
            }
            catch (Exception err)
            {
                LogCliquedin($"Resposta: {err.Message}", "GETPROXY", "https://cliquedin.app/api/postFollowers/proxy");
                return ret;
            }
        }


        private static string  DateString()
        {
            var data = DateTime.Today;
            var dia = data.Day.ToString();
            var mes = data.Month.ToString();
            var ano = data.Year.ToString();
            return $"{dia}-{mes}-{ano}";
        }

        private static string  HorarioString()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        private static void LogCliquedin(string message, string function, string url)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (Directory.Exists($@"{dir}\logs"))
                {
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\retornocliquedin.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\retornocliquedin.txt");
                        var list = linhas.ToList();
                        list.Add($"{function.ToUpper()} {HorarioString()} {url} {message}");
                        //File.WriteAllLines($@"{dir}\logs\retornocliquedin.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"{function.ToUpper()} {HorarioString()} {url} {message}" };
                        //File.WriteAllLines($@"{dir}\logs\retornocliquedin.txt", linhas);
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory($@"{dir}\logs");
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\retornocliquedin.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\retornocliquedin.txt");
                        var list = linhas.ToList();
                        list.Add($"{function.ToUpper()} {HorarioString()} {url} {message}");
                        //File.WriteAllLines($@"{dir}\logs\retornocliquedin.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"{function.ToUpper()} {HorarioString()} {url} {message}" };
                        //File.WriteAllLines($@"{dir}\logs\retornocliquedin.txt", linhas);
                        return;
                    }
                }
            }
            catch { }
        }

    }
}

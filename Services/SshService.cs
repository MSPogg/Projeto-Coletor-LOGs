using Renci.SshNet;
using System.Text.RegularExpressions;
using LCollector.Entities;

namespace LCollector.Services
{
    class SshService
    {
        private readonly string _user;
        private readonly string _password;
        private readonly string _ipstelnet;

        public SshService(string user, string password, string ipstelnet = "")
        {
            _user = user;
            _password = password;
            _ipstelnet = ipstelnet;
        }

        public string ConnectAndGetPrompt(Router router)
        {
            string ipText = router.IP.ToString();

            using(var Client = new SshClient(ipText, _user, _password))
            {
                Client.Connect();

                using(var Stream = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024))
                {
                    Stream.WriteLine("");

                    string response = Stream.Expect(new Regex(@"[>#\$]\s?$"),TimeSpan.FromSeconds(5))!;
                    Client.Disconnect();
                    string prompt = response.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                    return prompt;
                }
            }
        }

        public void ConnectStelnet(Router router)
        {
            string ipText = router.IP.ToString();

            using(var Client = new SshClient(_ipstelnet, _user, _password))
            {
                Client.Connect();

                using(var Stream = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024))
                {
                    Stream.WriteLine($"stelnet -i loopback100 {ipText}");
                    Stream.WriteLine("y");
                    Stream.WriteLine("n");
                    Stream.WriteLine(_user);
                    Stream.WriteLine(_password);
                }
            }
        }
        public string ExecuteCommands(Router router, IReadOnlyList<string> commands)
        {
            string ipText = router.IP.ToString();

            using(var Client = new SshClient(ipText, _user, _password))
            {
                Client.Connect();

                using(var Stream = Client.CreateShellStream("xterm", 80, 24, 800, 600, 65536))
                {
                    Thread.Sleep(1000);
                    Stream.Read();

                    var logBuilder = new System.Text.StringBuilder();

                    TimeSpan commandTimeout = router.Vendor switch
                    {
                        Vendor.Huawei  => TimeSpan.FromMinutes(2),
                        Vendor.CiscoXE => TimeSpan.FromMinutes(2),
                        Vendor.CiscoXR => TimeSpan.FromMinutes(2),
                        Vendor.Alcatel => TimeSpan.FromMinutes(2),
                        _              => TimeSpan.FromMinutes(2)
                    };

                    foreach(var com in commands)
                    {
                        Stream.WriteLine(com); 
                        
                        string regexPattern = $@"{Regex.Escape(router.Hostname)}.*[>#\$]\s*$";

                        string? resposta = Stream.Expect(new Regex(regexPattern), commandTimeout);

                        if (!string.IsNullOrEmpty(resposta))
                        {
                            logBuilder.AppendLine(resposta);
                        }
                        else
                        {
                            logBuilder.AppendLine($"[AVISO] Timeout ao aguardar resposta do comando: {com}");
                            Console.WriteLine($"[TIMEOUT] O roteador {router.IP} travou no comando: {com}"); 
                        }
                    }

                    string log = logBuilder.ToString();
                    Client.Disconnect();
                    return log;
                }
            }
        }
    }
}
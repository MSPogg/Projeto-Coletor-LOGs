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

        public SshService(string user, string password)
        {
            _user = user;
            _password = password;
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

                using(var Stream = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024))
                {
                    Thread.Sleep(1000);
                    Stream.Read();

                    var logBuilder = new System.Text.StringBuilder();

                    int time = router.Vendor switch
                    {
                        Vendor.Huawei  => 700,
                        Vendor.CiscoXE => 700,
                        Vendor.CiscoXR => 700,
                        Vendor.Alcatel => 2000,
                        _              => throw new Exception("Fabricante desconhecido")
                    };

                    foreach(var com in commands)
                    {
                        Stream.WriteLine(com); 
                        Thread.Sleep(time);

                        string resposta = Stream.Read();
                        logBuilder.Append(resposta);
                    }

                    string log = logBuilder.ToString();

                    Client.Disconnect();
                    return log;
                }
            }
        }
    }
}
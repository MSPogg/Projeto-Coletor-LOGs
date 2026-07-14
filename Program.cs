using System.Net;
using DotNetEnv;
using LCollector.Services;
using LCollector.Entities;
using LCollector.Utils;
using LCollector.Data;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();
        string user = Env.GetString("SSH_USER");
        string password = Env.GetString("SSH_PASSWORD");
        var networkConnector = new SshService(user, password);

        var ListaIPs = FileReader.ReadFile("LCollector.txt");

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        await Parallel.ForEachAsync(ListaIPs, parallelOptions, async (Ip, cancellationToken) =>
        {
            bool isValid = IPAddress.TryParse(Ip.Ip, out IPAddress? ipConvertido);

            if(isValid)
            {
                var myRouter = new Router(ipConvertido!, "Desconhecido", Vendor.Unknown);
                
                try
                {
                    string newHost = networkConnector.ConnectAndGetPrompt(myRouter);

                    var resultadoParser = PromptParser.ParsePrompt(newHost);

                    myRouter.Hostname = resultadoParser.hostname;
                    myRouter.Vendor = resultadoParser.vendor;
                    
                    Console.WriteLine($"IP: {myRouter.IP} | Hostname: {myRouter.Hostname} | Vendor: {myRouter.Vendor}");

                    if(myRouter.Vendor == Vendor.Unknown)
                    {
                        Console.WriteLine($"[AVISO] {myRouter.IP} - Vendor Desconhecido.");
                        return;
                    }
                    
                    IReadOnlyList<string> commands = myRouter.Vendor switch
                    {
                        Vendor.Huawei  => Commands.HUAWEI_COMMANDS,
                        Vendor.CiscoXE => Commands.CISCOXE_COMMANDS,
                        Vendor.CiscoXR => Commands.CISCOXR_COMMANDS,
                        Vendor.Alcatel => Commands.ALCATEL_COMMANDS,
                        _              => new List<string> ()

                    };

                    if(commands.Count > 0)
                    {
                        string log = networkConnector.ExecuteCommands(myRouter, commands);

                        var fileSaver = new FileSaver();
                        fileSaver.SaveLog(myRouter, log);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Erro interno no loop do IP {myRouter.IP}: {ex.Message}");
                }
            }
        });
    }
}
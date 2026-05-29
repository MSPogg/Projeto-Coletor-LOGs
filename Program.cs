using System.Net;
using DotNetEnv;
using LCollector.Services;
using LCollector.Entities;
using LCollector.Utils;
using LCollector.Data;

class Program
{
    static void Main(string[] args)
    {
        Env.Load();
        string user = Env.GetString("SSH_USER");
        string password = Env.GetString("SSH_PASSWORD");
        var networkConnector = new SshService(user, password);

        var ListaIPs = FileReader.ReadFile("LCollector.txt");

        foreach(var Ip in ListaIPs)
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

                    if(myRouter.Vendor == Vendor.Huawei)
                    {
                        string logSalvo = networkConnector.ExecuteCommands(myRouter, Commands.HUAWEI_COMMANDS);

                        var fileSaver = new FileSaver();

                        fileSaver.SaveLog(myRouter, logSalvo);
                    }
                    if(myRouter.Vendor == Vendor.CiscoXE)
                    {
                        string logSalvo = networkConnector.ExecuteCommands(myRouter, Commands.CISCOXE_COMMANDS);

                        var fileSaver = new FileSaver();

                        fileSaver.SaveLog(myRouter, logSalvo);
                    }
                    if(myRouter.Vendor == Vendor.CiscoXR)
                    {
                        string logSalvo = networkConnector.ExecuteCommands(myRouter, Commands.CISCOXR_COMMANDS);

                        var fileSaver = new FileSaver();

                        fileSaver.SaveLog(myRouter, logSalvo);
                    }
                    if(myRouter.Vendor == Vendor.Alcatel)
                    {
                        string logSalvo = networkConnector.ExecuteCommands(myRouter, Commands.ALCATEL_COMMANDS);

                        var fileSaver = new FileSaver();

                        fileSaver.SaveLog(myRouter, logSalvo);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Erro interno no loop do IP {myRouter.IP}: {ex.Message}");
                }
            }
        }
    }
}
using System.Net;

namespace LCollector.Entities
{   
    public class Router
    {
        public IPAddress IP {get; set;}
        public string Hostname {get; set;}
        public Vendor Vendor {get; set;}

        public Router(IPAddress ip, string hostname, Vendor vendor)
        {
            this.IP = ip;
            this.Hostname = hostname;
            this.Vendor = vendor;
        }
    }
}
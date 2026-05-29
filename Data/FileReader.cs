using LCollector.DTOs;

namespace LCollector.Data
{
    class FileReader
    {
        static public List<RouterConfigDTO> ReadFile(string path)
        {
            var ListaIps = new List<RouterConfigDTO>();
            string[] Linhas = File.ReadAllLines(path);

            foreach(string Linha in Linhas)
            {
                ListaIps.Add(new RouterConfigDTO {Ip = Linha});
            }

            return ListaIps;
        }
    }
}
using LCollector.Entities;

namespace LCollector.Data
{
    public class FileSaver
    {
        public void SaveLog(Router router, string content)
        {
            string fileName = $"{router.Hostname}_Log.txt";

            Directory.CreateDirectory("Logs");
            string pathLog = Path.Combine("Logs", fileName);
            
            File.WriteAllText(pathLog, content);
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Lab3_OOP
{
    internal class JsonFunctions
    {
        public static void Serialize(string path, ObservableCollection<ScienceWorker> results)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {

                JsonSerializer.Serialize(fstream, results, options);
            }
        }

        //десеріалізація - прочитали json файл
        public static ObservableCollection<ScienceWorker> Deserialize(string path)
        {
            ObservableCollection<ScienceWorker> results = new ObservableCollection<ScienceWorker>();
            using (FileStream fstream = new FileStream(path, FileMode.Open))
            {
                var workers = JsonSerializer.Deserialize<List<ScienceWorker>>(fstream);

                foreach (ScienceWorker worker in workers)
                {
                    results.Add(worker);
                }

                return results;
            }
        }
    }
}

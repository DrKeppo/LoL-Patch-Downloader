using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace LoL_Downgrader
{
    class JsonManager
    {
        public Dictionary<string, string> Dictionary { get; private set; }

        public JsonManager()
        {
            Dictionary = new Dictionary<string, string>();
            return;
        }

        public static JsonManager Load(string path)
        {
            return JsonConvert.DeserializeObject<JsonManager>(File.ReadAllText(path));
        }
    }
}

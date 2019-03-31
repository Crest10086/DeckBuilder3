using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools
{
    public class IniConfig
    {
        private IniFile iniFile;
        private readonly string section;
        private Dictionary<string, string> configs = new Dictionary<string, string>();

        public string this[string key]
        {
            get => GetSetting(key);
            set => SetSetting(key, value);
        }

        public IniConfig(string fileName, string section = "config")
        {
            iniFile = new IniFile(fileName);
            this.section = section;
        }

        public string GetSetting(string key, string defValue = "")
        {
            if (!configs.ContainsKey(key))
            {
                configs[key] = iniFile.ReadString(section, key, defValue);
            }
            return configs[key];
        }

        public void SetSetting(string key, string value)
        {
            configs[key] = value;
            iniFile.WriteString(section, key, value);
        }

        public void Refresh() => configs.Clear();
    }
}

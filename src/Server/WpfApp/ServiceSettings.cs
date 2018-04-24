using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class ServiceSettings : SettingCache<ServiceStruct>
    {
        public override string Key { get; } = "service.json";

        public override string BasePath { get; set; }

        ServiceStruct defaultValue { get; set; }

        public ServiceSettings(string settingPath, string file,string path, int port )
        {
            BasePath = settingPath;
            defaultValue = new ServiceStruct
            {
                ServicePath = file,
                ServicePort = port,
                ServiceFile = path
            };
        }

        protected override ServiceStruct GetDefault()
        {
            return defaultValue;
        }
    }
}

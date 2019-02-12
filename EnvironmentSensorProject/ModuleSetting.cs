using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EnvironmentSensorProject
{
    public class ModuleSetting
    {
        private static String configFilePath = @"setting/config.json";

        public String serveraddress1;
        public String serveraddress2;
        public String sensorport;
        public String subaddress;
        public String location;

        public ModuleSetting()
        {

        }

        public static ModuleSetting Load()
        {
            ModuleSetting setting = null;
            try
            {
                String configContent = File.ReadAllText(configFilePath);
                setting = new JavaScriptSerializer().Deserialize<ModuleSetting>(configContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return setting;
        }
    }
}

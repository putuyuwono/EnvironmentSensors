using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EnvironmentSensorProject
{
    public class EnvironmentSensingReportNew
    {
        public String msg_no;
        public String msg_time;
        public String msg_type;
        public String sensor_type;
        public String sensor_id;
        public String data;
        public String sensor_location;

        public EnvironmentSensingReportNew()
        {

        }

        public String ToJSON()
        {
            String jsonFormatString = null;
            jsonFormatString = new JavaScriptSerializer().Serialize(this);
            return jsonFormatString;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentSensorProject
{
    public class SensingData
    {
        public enum SensorType { Temperature, Humidity, Motion, LightIntensity, Smoke, Flame, Unknown }

        public String SensorID;
        public SensorType Type;
        public double Value;

        public SensingData() { }
        public SensingData(String SID, SensorType type, double value)
        {
            this.SensorID = SID;
            this.Type = type;
            this.Value = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SensorID: " + SensorID + "\n");
            sb.Append("Type: " + Type + "\n");

            switch (Type)
            {
                case SensorType.Temperature:
                    sb.Append("Value: " + Value + " C\n");
                    break;
                case SensorType.Humidity:
                    sb.Append("Value: " + Value + " %\n");
                    break;
                case SensorType.Motion:
                    sb.Append("Value: " + Value + " \n");
                    break;
                case SensorType.LightIntensity:
                    sb.Append("Value: " + Value + " lux\n");
                    break;
                default:
                    sb.Append("Value: " + Value + "\n");
                    break;
            }
            return sb.ToString();
        }
    }
}

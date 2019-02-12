using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentSensorProject
{
    public class EnvironmentReader
    {
        public String DeviceID = String.Empty;
        public bool DeviceIsActive = false;
        public String Passcode = "555";
        public String PortName = String.Empty;
        public int BaudRate = 9600;

        public String Information = String.Empty;
        public SensingData Temperature = new SensingData("", SensingData.SensorType.Temperature, -1);
        public SensingData Humidity = new SensingData("", SensingData.SensorType.Humidity, -1);
        public SensingData LightIntensity = new SensingData("", SensingData.SensorType.LightIntensity, -1);
        public SensingData Motion = new SensingData("", SensingData.SensorType.Motion, -1);
        public SensingData Smoke = new SensingData("", SensingData.SensorType.Smoke, -1);
        public SensingData Flame = new SensingData("", SensingData.SensorType.Flame, -1);

        public EnvironmentReader()
        {

        }

        public EnvironmentReader(String portName)
        {
            this.PortName = portName;
        }

        public bool ParseData(String data)
        {
            bool success = false;
            String[] temp = data.Split(',');
            if (temp.Length == 2)
            {
                success = ParseSensorInfo(temp);
            }
            else
            {
                success = ParseSensorDataNew(temp);
                
            }
            return success;
        }

        private bool ParseSensorDataNew(String[] sensorData)
        {
            bool success = false;

            double oldValue;
            foreach (var item in sensorData)
            {
                string[] temp = item.Split(':');
                if (temp.Length == 2)
                {
                    string sensorID = temp[0];
                    string newValue = temp[1];
                    switch (sensorID)
                    {
                        case "DEV": DeviceID = temp[1]; break;
                        case "TEM":
                            double temperValue = Double.Parse(newValue);
                            oldValue = Temperature.Value;
                            if (IsSignigicantlyChanged(temperValue, oldValue, SensingData.SensorType.Temperature))
                            {
                                Temperature = new SensingData(sensorID, SensingData.SensorType.Temperature, temperValue);
                            }
                            break;
                        case "HUM":
                            double humidiValue = Double.Parse(newValue);
                            oldValue = Humidity.Value;
                            if (IsSignigicantlyChanged(humidiValue, oldValue, SensingData.SensorType.Humidity))
                            {
                                Humidity = new SensingData(sensorID, SensingData.SensorType.Humidity, humidiValue);
                            }
                            break;
                        case "MOT":
                            double motionValue = Double.Parse(newValue);
                            oldValue = Motion.Value;
                            if (IsSignigicantlyChanged(motionValue, oldValue, SensingData.SensorType.Motion))
                            {
                                Motion = new SensingData(sensorID, SensingData.SensorType.Motion, motionValue);
                            }
                            break;
                        case "LIG":
                            double lightValue = Double.Parse(newValue);
                            oldValue = LightIntensity.Value;
                            if (IsSignigicantlyChanged(lightValue, oldValue, SensingData.SensorType.LightIntensity))
                            {
                                LightIntensity = new SensingData(sensorID, SensingData.SensorType.LightIntensity, lightValue);
                            }
                            break;
                        case "SMK":
                            double smokeValue = Double.Parse(newValue);
                            oldValue = Smoke.Value;
                            if (smokeValue < 0)
                            {
                                smokeValue = Math.Abs(smokeValue);
                            }
                            if (IsSignigicantlyChanged(smokeValue, oldValue, SensingData.SensorType.Smoke))
                            {
                                Smoke = new SensingData(sensorID, SensingData.SensorType.Smoke, smokeValue);
                            }
                            break;
                        case "FLM":
                            double flameValue = Double.Parse(newValue);
                            oldValue = Flame.Value;
                            if (IsSignigicantlyChanged(flameValue, oldValue, SensingData.SensorType.Flame))
                            {
                                Flame = new SensingData(sensorID, SensingData.SensorType.Flame, flameValue);
                            }
                            break;
                        default:
                            break;
                    }
                }
                DeviceIsActive = true;
                success = true;
            }

            return success;
        }

        private bool ParseSensorData(String[] data)
        {
            bool success = false;
            String deviceInfo = data[0];
            String motionInfo = data[1];
            String temperInfo = data[2];
            String humidiInfo = data[3];
            String lightInfo = data[4];
            String smokeInfo = data[5];
            String flameInfo = data[6];

            double oldValue;
            try
            {
                this.DeviceID = deviceInfo.Split(':')[1];

                String[] motionData = motionInfo.Split(':');
                String motionSID = motionData[0];
                double motionValue = Double.Parse(motionData[1]);
                oldValue = Motion.Value;
                if (IsSignigicantlyChanged(motionValue, oldValue, SensingData.SensorType.Motion))
                {
                    Motion = new SensingData(motionSID, SensingData.SensorType.Motion, motionValue);
                }

                String[] temperData = temperInfo.Split(':');
                String temperSID = temperData[0];
                double temperValue = Double.Parse(temperData[1]);
                oldValue = Temperature.Value;
                if (IsSignigicantlyChanged(temperValue, oldValue, SensingData.SensorType.Temperature))
                {
                    Temperature = new SensingData(temperSID, SensingData.SensorType.Temperature, temperValue);
                }

                String[] humidiData = humidiInfo.Split(':');
                String humidiSID = humidiData[0];
                double humidiValue = Double.Parse(humidiData[1]);
                oldValue = Humidity.Value;
                if (IsSignigicantlyChanged(humidiValue, oldValue, SensingData.SensorType.Humidity))
                {
                    Humidity = new SensingData(humidiSID, SensingData.SensorType.Humidity, humidiValue);
                }

                String[] lightData = lightInfo.Split(':');
                String lightSID = lightData[0];
                double lightValue = Double.Parse(lightData[1]);
                oldValue = LightIntensity.Value;
                if (IsSignigicantlyChanged(lightValue, oldValue, SensingData.SensorType.LightIntensity))
                {
                    LightIntensity = new SensingData(lightSID, SensingData.SensorType.LightIntensity, lightValue);
                }

                String[] smokeData = smokeInfo.Split(':');
                String smokeSID = smokeData[0];
                double smokeValue = Double.Parse(smokeData[1]);
                oldValue = Smoke.Value;
                if (smokeValue < 0)
                {
                    smokeValue = Math.Abs(smokeValue);
                }
                if (IsSignigicantlyChanged(smokeValue, oldValue, SensingData.SensorType.Smoke))
                {
                    Smoke = new SensingData(smokeSID, SensingData.SensorType.Smoke, smokeValue);
                }

                String[] flameData = flameInfo.Split(':');
                String flameSID = flameData[0];
                double flameValue = Double.Parse(flameData[1]);
                oldValue = Flame.Value;
                if (IsSignigicantlyChanged(flameValue, oldValue, SensingData.SensorType.Flame))
                {
                    Flame = new SensingData(flameSID, SensingData.SensorType.Flame, flameValue);
                }

                this.DeviceIsActive = true;
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        private bool ParseSensorInfo(String[] temp)
        {
            bool success = false;
            try
            {
                Information = temp[1].Trim();
                switch (Information)
                {
                    case "INACTIVE": this.DeviceIsActive = false; break;
                    case "ACTIVE": this.DeviceIsActive = true; break;
                    default:
                        break;
                }
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        private bool IsSignigicantlyChanged(double value1, double value2, SensingData.SensorType type)
        {
            bool status = false;
            double temperatureDiff = 5;
            double humidityDiff = 5;
            double intensityDiff = 100;
            double motionDiff = 1;
            double smokeDiff = 1;
            double flameDiff = 1;
            double difference = Math.Abs(value1 - value2);

            switch (type)
            {
                case SensingData.SensorType.Temperature:
                    if (difference >= temperatureDiff) status = true;
                    break;
                case SensingData.SensorType.Humidity:
                    if (difference >= humidityDiff) status = true;
                    break;
                case SensingData.SensorType.Motion:
                    if (difference >= motionDiff) status = true;
                    break;
                case SensingData.SensorType.LightIntensity:
                    if (difference >= intensityDiff) status = true;
                    break;
                case SensingData.SensorType.Smoke:
                    if (difference >= smokeDiff) status = true;
                    break;
                case SensingData.SensorType.Flame:
                    if (difference >= flameDiff) status = true;
                    break;
                default:
                    break;
            }
            return status;
        }
    }
}

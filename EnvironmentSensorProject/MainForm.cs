using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvironmentSensorProject
{
    public partial class MainForm : Form
    {
        private String rawData;
        private EnvironmentReader environmentReader = null;

        private double temperature = -1, humidity = -1, brightness = -1, motion = -1, flame = -1, smoke = -1;

        private int messageCounter = 0;
        private String reportAddress1 = String.Empty;
        private String reportAddress2 = String.Empty;
        private String location = String.Empty;

        private String logFile = Application.StartupPath + "\\openscreen.log";
        private ModuleSetting setting;

        public MainForm()
        {
            InitializeComponent();

            this.CenterToScreen();

            InitSetting();
        }

        private void InitSetting()
        {
            try
            {
                setting = ModuleSetting.Load();

                this.reportAddress1 = setting.serveraddress1;
                this.reportAddress2 = setting.serveraddress2;
                this.location = setting.location;
                this.InitLoadSarialPorts(setting.sensorport);
                this.InvokeDevice();

                lblLoc.Text = "Location: " + location;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void InitLoadSarialPorts(String portName)
        {
            String[] ports = SerialPort.GetPortNames();
            foreach (String port in ports)
            {
                cbComPort.Items.Add(port);
            }

            if (cbComPort.Items.Count > 0 && portName != String.Empty)
            {
                cbComPort.SelectedIndex = this.FindItem(portName);
            }
        }

        private int FindItem(String portName)
        {
            int index = -1;
            for (int i = 0; i < cbComPort.Items.Count; i++)
            {
                if (cbComPort.Items[i].ToString() == portName)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void btInvoke_Click(object sender, EventArgs e)
        {
            this.InvokeDevice();
        }

        private void InvokeDevice()
        {
            if (serialPort.IsOpen)
            {
                String passcode = environmentReader.Passcode;
                byte[] buffer = new byte[passcode.Length];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = Convert.ToByte(passcode[i]);
                }
                serialPort.Write(buffer, 0, buffer.Length);
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                rawData = serialPort.ReadLine();
                this.Invoke(new EventHandler(ProcessData));
            }
            catch (Exception ex)
            {
            }
        }

        private void ProcessData(object sender, EventArgs e)
        {
            try
            {
                environmentReader.ParseData(rawData);
                this.DisplayData();
                //rtbSensorLog.AppendText(rawData);                               
            }
            catch (Exception ex)
            {
                //Incorrect Device
                lblStatus.Text = "Status: Device is unavailable.";
            }

        }

        private void DisplayData()
        {
            lblDeviceID.Text = "Device ID: " + environmentReader.DeviceID;
            lblTime.Text = "Time: " + DateTime.Now.ToString("yyyyMMddHHmmss");

            lblMotion.Text = "( " + environmentReader.Motion.Value + " )";
            lblTemp.Text = environmentReader.Temperature.Value + " °C";
            lblHumi.Text = environmentReader.Humidity.Value + " %";
            lblLight.Text = environmentReader.LightIntensity.Value + " LUX";
            lblSmoke.Text = environmentReader.Smoke.Value + " ppm";
            lblFlame.Text = "( " + environmentReader.Flame.Value + " )";

            TrackChanges();

            lblStatus.Text = "Status: " + environmentReader.Information;

            if (environmentReader.Information.Equals("ACTIVE"))
            {
                btInvoke.Text = "Stop";
            }
            else
            {
                btInvoke.Text = "Start";
            }
            
        }

        private void TrackChanges()
        {
            if (environmentReader != null)
            {
                //If the environment condition significantly change then send it to server.
                String msgNo = Utils.GenerateMsgNo(messageCounter++);
                String msgTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                String msgType = "sensor_report";

                if (IsDifferent(temperature, environmentReader.Temperature.Value, 5))
                {
                    temperature = environmentReader.Temperature.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "temperature", "celcius", temperature, "temperature_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "temp_sen", "3", temperature.ToString());
                }

                if (IsDifferent(humidity, environmentReader.Humidity.Value, 5))
                {
                    humidity = environmentReader.Humidity.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "humidity", "percentage", humidity, "humidity_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "hum_sen", "4", humidity.ToString());
                }

                if (IsDifferent(brightness, environmentReader.LightIntensity.Value, 10))
                {
                    brightness = environmentReader.LightIntensity.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "brightness", "lux", brightness, "brightness_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "illu_sen", "5", brightness.ToString());
                }

                if (IsDifferent(motion, environmentReader.Motion.Value, 1))
                {
                    motion = environmentReader.Motion.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "motion", "motion", motion, "motion_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "prox_sen", "8", motion.ToString());
                }

                if (IsDifferent(smoke, environmentReader.Smoke.Value, 1))
                {
                    smoke = environmentReader.Smoke.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "smoke", "ppm", smoke, "smoke_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "smoke_sen", "7", smoke.ToString());
                }

                if (IsDifferent(flame, environmentReader.Flame.Value, 1))
                {
                    flame = environmentReader.Flame.Value;
                    GenerateAndSendReport1(msgNo, msgTime, msgType, "flame", "flame", flame, "flame_sensor");
                    GenerateAndSendReport2(msgNo, msgTime, msgType, "flame_sen", "6", flame.ToString());
                }
            }
        }
        
        private bool IsDifferent(double oldValue, double newValue, double threshold)
        {
            bool status = false;

            double diff = Math.Abs(oldValue - newValue);
            if (diff >= threshold)
            {
                status = true;
            }

            return status;
        }
        
        private void GenerateAndSendReport1(String msgNo, String msgTime, String msgType, String item, String scale, double value, String sensorID)
        {
            EnvironmentSensingReport report1 = new EnvironmentSensingReport()
            {
                msg_no = msgNo,
                msg_time = msgTime,
                msg_type = msgType,
                sensor_id = sensorID,
                measure_item = item,
                measure_scale = scale,
                measure_value = value
            };

            String serverReport1 = "OpenScreenReport=" + report1.ToJSON();
            this.SendReport(reportAddress1, serverReport1);            
        }

        private void GenerateAndSendReport2(String msgNo, String msgTime, String msgType, String sen_type, String sensorID, String value)
        {
            EnvironmentSensingReportNew report2 = new EnvironmentSensingReportNew()
            {
                msg_no = msgNo,
                msg_time = msgTime,
                msg_type = msgType,
                sensor_type = sen_type,
                sensor_id = sensorID,
                data = value,
                sensor_location = this.location
            };

            String serverReport = "OpenScreenReport=" + report2.ToJSON();
            this.SendReport(reportAddress2, serverReport);
        }

        private async void SendReport(String server, String message)
        {
            String response = String.Empty;
            if (server == String.Empty)
            {
                response = "Report Server Address is null/empty";
            }
            else
            {
                try
                {
                    String sendLog = "ReportAddress: " + server + "\n" + message + "\n";
                    AppendReport(sendLog);
                    response = await ReportManager.PostAsync(server, message);
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }
            }
            String responseLog = "ReportAddress: " + server + "\nReponse: " + response + "\n";
            AppendReport(responseLog);
            WriteLog(message);
        }

        delegate void AppendTrackReport(String reportText);
        private void AppendReport(String reportText)
        {
            if (this.rtbSensorLog.InvokeRequired)
            {
                AppendTrackReport a = new AppendTrackReport(AppendReport);
                this.Invoke(a, new object[] { reportText });
            }
            else
            {
                this.rtbSensorLog.AppendText(reportText);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (environmentReader != null && environmentReader.DeviceIsActive)
            {
                btInvoke.PerformClick();
                serialPort.Close();
            }
        }

        private void WriteLog(String logMessage)
        {
            using (StreamWriter file = new StreamWriter(logFile, true))
            {
                file.WriteLine(logMessage);
            }
        }
        
        private void ShowError(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void cbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupSerialPort();
        }

        private void SetupSerialPort()
        {
            try
            {
                environmentReader = new EnvironmentReader(cbComPort.SelectedItem.ToString());
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.PortName = environmentReader.PortName;
                serialPort.BaudRate = environmentReader.BaudRate;
                serialPort.Open();
                serialPort.ReadTimeout = 200;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btClearLog_Click(object sender, EventArgs e)
        {
            rtbSensorLog.Text = "";
        }

    }
}

using C1202ComDemoBasic.Model;
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using System.Management;
using System.Timers;

namespace C1202ComDemoBasic.Services
{
    class C1202Com
    {
        // commands:
        string getAllFeatures = "?\r";

        // comport settings:
        int deviceBaudRate = 9600;
        int deviceBitsLength = 7;

        // patterns:
        string patternError = @"[1-3]?\?sERR[1-9]";
        string patternValue = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}$";
        string patternValueT = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}\s[<>=]$";
        string patternValueTW = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}\s[<>=]\s[<>=]$";

        string reply = "";

        public SerialPort comPort = null;
        public FeatureValue[] featureValues;
        mainViewCtrl _mainViewCtrl;
        int activeFeature = 0;
        

        public C1202Com(mainViewCtrl _mainViewCtrl)
        {
            this._mainViewCtrl = _mainViewCtrl;

            featureValues = new FeatureValue[3];
            featureValues[0] = new FeatureValue();
            featureValues[1] = new FeatureValue();
            featureValues[2] = new FeatureValue();
        }

        public bool getPort()
        {
            List<String> ComPortListLong;
            String [] ComPortListShort = SerialPort.GetPortNames();
            bool result = false;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                ComPortListLong = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                int i = 0;
                foreach (String s in ComPortListLong)
                {
                    if (s.Contains("USB Serial Port"))
                    {
                        comPort = new System.IO.Ports.SerialPort(portnames[i],
                            deviceBaudRate,
                            System.IO.Ports.Parity.Even,
                            deviceBitsLength,
                            System.IO.Ports.StopBits.Two
                            );
                        result = true;
                    }
                    i++;
                }
            }
            return result; // true = ok
        }

        public void getAllMeasurements()
        {
            try
            {
             if (comPort.IsOpen == false) comPort.Open();
                comPort.Write(getAllFeatures);
                portListen();
            }
            catch(Exception e)
            {
                Console.WriteLine("Can not write to com port");
            }
        }

        public void getFeature(int featureNo)
        {
            string getOneFeature = "M" + featureNo.ToString() + getAllFeatures;
            activeFeature = featureNo;
            try
            {
                if (comPort.IsOpen == false) comPort.Open();
                comPort.Write(getOneFeature);
                portListen();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can not write to com port");
            }
        }

        public void portListen()
        {
            try
            {
                if (comPort.IsOpen == false) comPort.Open();
                comPort.DataReceived += new SerialDataReceivedEventHandler(DataRecievedhandler);
                reply = "";
            }
            catch (Exception e)
            {
                Console.WriteLine("Can not open com port");
            }
        }

        private void DataRecievedhandler(object sender, SerialDataReceivedEventArgs e)
        {
            reply += comPort.ReadExisting();
            if (reply.EndsWith("\r"))
            {
                decomposeReply(reply);
                reply = "";
                _mainViewCtrl.updateValues(activeFeature);
                activeFeature = 0;
            }
        }

        private void decomposeReply(string inputStr)
        {
            Regex rgErr = new Regex(patternError);
            Regex rgValue = new Regex(patternValue);
            Regex rgValueT = new Regex(patternValueT);
            Regex rgValueTW = new Regex(patternValueTW);

            inputStr = inputStr.Remove(inputStr.Length - 1);
            string[] fractions = inputStr.Split(';');
            
            foreach (string fraction in fractions)
            {
                
                string[] parts = fraction.Split(' ');
                int x = 0;
                int mod = 0;
                try
                {
                    x = Int16.Parse(parts[0]) - 1;
                    if (fractions.Length > 1)
                    {
                        activeFeature = 0;
                    }
                    else
                    {
                        activeFeature = x + 1;
                    }
                }
                catch (Exception e)
                {
                    x = activeFeature - 1;
                    mod = -1;
                }
                
                featureValues[x].valueText = "-.-";
                featureValues[x].unit = "";
                featureValues[x].isToleranceEnabled = false;
                featureValues[x].isInTolerance = false;
                featureValues[x].isInWarning = false;

                if (rgErr.IsMatch(fraction) == true){
                    featureValues[x].valueText = "no value (" + parts[1 + mod] + ")";
                    featureValues[x].unit = "";
                    featureValues[x].isToleranceEnabled = false;
                    featureValues[x].isInTolerance = false;
                    featureValues[x].isInWarning = false;
                }
                else if (rgValue.IsMatch(fraction) == true)
                {
                    featureValues[x].valueText = parts[1 + mod];
                    featureValues[x].unit = parts[2 + mod];
                    featureValues[x].isToleranceEnabled = false;
                    featureValues[x].isInTolerance = true;
                    featureValues[x].isInWarning = true;
                }
                else if (rgValueT.IsMatch(fraction) == true)
                {
                    featureValues[x].valueText = parts[1 + mod];
                    featureValues[x].unit = parts[2 + mod];
                    featureValues[x].isToleranceEnabled = true;
                    if (parts[3 + mod].Equals("="))
                    {
                        featureValues[x].isInTolerance = true;
                        featureValues[x].isInWarning = true;
                    }
                }
                else if (rgValueTW.IsMatch(fraction) == true)
                {
                    featureValues[x].valueText = parts[1 + mod];
                    featureValues[x].unit = parts[2 + mod];
                    featureValues[x].isToleranceEnabled = true;
                    if (parts[3 + mod].Equals("="))
                    {
                        featureValues[x].isInTolerance = true;
                    }
                    if (parts[4 + mod].Equals("="))
                    {
                        featureValues[x].isInWarning = true;
                    }
                }
            }
        }
    }
}

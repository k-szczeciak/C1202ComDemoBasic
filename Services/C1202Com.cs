using C1202ComDemoBasic.Model;
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;

namespace C1202ComDemoBasic.Services
{
    class C1202Com
    {
        // commands:
        string getAllFeatures = "?\r";
        string patternError = @"[1-3]?\?sERR[1-9]";
        string patternValue = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}$";
        string patternValueT = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}\s[<>=]$";
        string patternValueTW = @"^[1-3]?\s?[+-]?[0-9]{1,3}.[0-9]{1,5}\s[a-z]{2,4}\s[<>=]\s[<>=]$";

        string reply = "";

        SerialPort comPort = null;
        //FeatureValue[] featureValues;

        mainViewCtrl _mainViewCtrl;
        int activeFeature = 0;


        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

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
                Console.WriteLine("Can not open com port");
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
                Console.WriteLine("Can not open com port");
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

        public C1202Com(SerialPort serialPort, mainViewCtrl _mainViewCtrl)
        {
            this._mainViewCtrl = _mainViewCtrl;
            this.comPort = serialPort;
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
                
                _mainViewCtrl.featureValues[x].valueText = "-.-";
                _mainViewCtrl.featureValues[x].unit = "";
                _mainViewCtrl.featureValues[x].isToleranceEnabled = false;
                _mainViewCtrl.featureValues[x].isInTolerance = false;
                _mainViewCtrl.featureValues[x].isInWarning = false;

                // qualification: err, value, value w. T, val w. TW
                if (rgErr.IsMatch(fraction) == true){
                    _mainViewCtrl.featureValues[x].valueText = "no value (" + parts[1 + mod] + ")";
                    _mainViewCtrl.featureValues[x].unit = "";
                    _mainViewCtrl.featureValues[x].isToleranceEnabled = false;
                    _mainViewCtrl.featureValues[x].isInTolerance = false;
                    _mainViewCtrl.featureValues[x].isInWarning = false;
                }
                else if (rgValue.IsMatch(fraction) == true)
                {
                    _mainViewCtrl.featureValues[x].valueText = parts[1 + mod];
                    _mainViewCtrl.featureValues[x].unit = parts[2 + mod];
                    _mainViewCtrl.featureValues[x].isToleranceEnabled = false;
                    _mainViewCtrl.featureValues[x].isInTolerance = true;
                    _mainViewCtrl.featureValues[x].isInWarning = true;
                }
                else if (rgValueT.IsMatch(fraction) == true)
                {
                    _mainViewCtrl.featureValues[x].valueText = parts[1 + mod];
                    _mainViewCtrl.featureValues[x].unit = parts[2 + mod];
                    _mainViewCtrl.featureValues[x].isToleranceEnabled = true;
                    if (parts[3 + mod].Equals("="))
                    {
                        _mainViewCtrl.featureValues[x].isInTolerance = true;
                        _mainViewCtrl.featureValues[x].isInWarning = true;
                    }
                }
                else if (rgValueTW.IsMatch(fraction) == true)
                {
                    _mainViewCtrl.featureValues[x].valueText = parts[1 + mod];
                    _mainViewCtrl.featureValues[x].unit = parts[2 + mod];
                    _mainViewCtrl.featureValues[x].isToleranceEnabled = true;
                    if (parts[3 + mod].Equals("="))
                    {
                        _mainViewCtrl.featureValues[x].isInTolerance = true;
                    }
                    if (parts[4 + mod].Equals("="))
                    {
                        _mainViewCtrl.featureValues[x].isInWarning = true;
                    }
                }
            }
        }
    }
}

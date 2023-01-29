using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using C1202ComDemoBasic.Core;
using C1202ComDemoBasic.Model;
using C1202ComDemoBasic.Services;

namespace C1202ComDemoBasic
{
    class mainViewCtrl : ObservableObject
    {

        string deviceComPort = "COM16";
        int deviceBaudRate = 9600;
        int deviceBitsLength = 7;
        C1202Com c1202com = null;
        public SerialPort deviceCom = null;

        private FeatureValue[] _featureValues;

        public FeatureValue[] featureValues
        {
            get { return _featureValues; }
            set {
                _featureValues = value;
            }
        }

        private String _featureValue1;

        public String featureValue1
        {
            get { return _featureValue1; }
            set {
                _featureValue1 = value;
                OnPropertyChanged();
            }
        }

        private String _featureValue2;

        public String featureValue2
        {
            get { return _featureValue2; }
            set
            {
                _featureValue2 = value;
                OnPropertyChanged();
            }
        }

        private String _featureValue3;

        public String featureValue3
        {
            get { return _featureValue3; }
            set
            {
                _featureValue3 = value;
                OnPropertyChanged();
            }
        }

        private string _backgrColor1;

        public string backgrColor1
        {
            get { return _backgrColor1; }
            set {
                _backgrColor1 = value;
                OnPropertyChanged();
            }
        }

        private string _backgrColor2;

        public string backgrColor2
        {
            get { return _backgrColor2; }
            set
            {
                _backgrColor2 = value;
                OnPropertyChanged();
            }
        }

        private string _backgrColor3;

        public string backgrColor3
        {
            get { return _backgrColor3; }
            set
            {
                _backgrColor3 = value;
                OnPropertyChanged();
            }
        }

        private List<dataDisplModel> valueList1;
        private List<dataDisplModel> valueList2;
        private List<dataDisplModel> valueList3;

        private dataDisplModel[] _displList1;

        public dataDisplModel[] displList1

        {
            get { return _displList1; }
            set {
                _displList1 = value;
                OnPropertyChanged();
            }
        }

        private dataDisplModel[] _displList2;

        public dataDisplModel[] displList2

        {
            get { return _displList2; }
            set
            {
                _displList2 = value;
                OnPropertyChanged();
            }
        }

        private dataDisplModel[] _displList3;

        public dataDisplModel[] displList3

        {
            get { return _displList3; }
            set
            {
                _displList3 = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BtnGetNewValue { get; set; }
        public RelayCommand BtnChangeView { get; set; }
        public RelayCommand BtnGetFeat1 { get; set; }
        public RelayCommand BtnGetFeat2 { get; set; }
        public RelayCommand BtnGetFeat3 { get; set; }
        public RelayCommand BtnClearData { get; set; }



        public mainViewCtrl()
        {
            deviceCom = new System.IO.Ports.SerialPort(deviceComPort,
                                deviceBaudRate,
                                System.IO.Ports.Parity.Even,
                                deviceBitsLength,
                                System.IO.Ports.StopBits.Two
                                );

            featureValues = new FeatureValue[3];
            featureValues[0] = new FeatureValue();
            featureValues[1] = new FeatureValue();
            featureValues[2] = new FeatureValue();

            //initial values:
            featureValue1 = "-.-";
            featureValue2 = "-.-";
            featureValue3 = "-.-";
            backgrColor1 = "Transparent";
            backgrColor2 = "Transparent";
            backgrColor3 = "Transparent";

            valueList1 = new List<dataDisplModel>();
            valueList2 = new List<dataDisplModel>();
            valueList3 = new List<dataDisplModel>();
            
            c1202com = new C1202Com(deviceCom, this);
            c1202com.portListen();

            BtnGetNewValue = new RelayCommand(o =>
            {
                c1202com.getAllMeasurements();
            });
            BtnGetFeat1 = new RelayCommand(o =>
            {
                c1202com.getFeature(1);
            });
            BtnGetFeat2 = new RelayCommand(o =>
            {
                c1202com.getFeature(2);
            });
            BtnGetFeat3 = new RelayCommand(o =>
            {
                c1202com.getFeature(3);
            });

            BtnChangeView = new RelayCommand(o =>
            {
                // change view between list and graph
            });
            BtnClearData = new RelayCommand(o =>
            {
                valueList1.Clear();
                valueList2.Clear();
                valueList3.Clear();
                displList1 = valueList1.ToArray();
                displList2 = valueList2.ToArray();
                displList3 = valueList3.ToArray();
            });
        }


        public void updateValues(int featureNo) // 0 - all, 1..3 feature No
        {
            if (featureNo == 0)
            {
                featureValue1 = evaluateOneFeat(0);
                featureValue2 = evaluateOneFeat(1);
                featureValue3 = evaluateOneFeat(2);
                backgrColor1 = evalOneFeatColor(0);
                backgrColor2 = evalOneFeatColor(1);
                backgrColor3 = evalOneFeatColor(2);
                valueList1.Add(new dataDisplModel(featureValue1, backgrColor1, ""));
                valueList2.Add(new dataDisplModel(featureValue2, backgrColor2, ""));
                valueList3.Add(new dataDisplModel(featureValue3, backgrColor3, ""));

                valueList1.Reverse();
                valueList2.Reverse();
                valueList3.Reverse();
                displList1 = valueList1.ToArray();
                displList2 = valueList2.ToArray();
                displList3 = valueList3.ToArray();
            }
            else if(featureNo == 1)
            {
                featureValue1 = evaluateOneFeat(0);
                backgrColor1 = evalOneFeatColor(0);
            }
            else if (featureNo == 2)
            {
                featureValue2 = evaluateOneFeat(1);
                backgrColor2 = evalOneFeatColor(1);
            }
            else if (featureNo == 2)
            {
                featureValue3 = evaluateOneFeat(2);
                backgrColor3 = evalOneFeatColor(2);
            }
        }

        private String evaluateOneFeat(int featureNo)
        {
            return featureValues[featureNo].valueText + " " + featureValues[featureNo].unit;
        }
        private String evalOneFeatColor(int featureNo)
        {
            String result = "Transparent";
            if (featureValues[featureNo].isToleranceEnabled == true)
            {
                result = "Green";
                if (featureValues[featureNo].isInWarning == false)
                {
                    result = "Yellow";
                }
                if (featureValues[featureNo].isInTolerance == false)
                {
                    result = "Red";
                }
            }
            return result;
        }

    }
}

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
        public C1202Com c1202Com = null;
        private static System.Timers.Timer aTimer;

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

        private string _BtnStartBackgrCol;
            
        public string BtnStartBackgrCol
        {
            get { return _BtnStartBackgrCol; }
            set {
                _BtnStartBackgrCol = value;
                OnPropertyChanged();
            }
        }

        private string _BtnStopBackgrCol;

        public string BtnStopBackgrCol
        {
            get { return _BtnStopBackgrCol; }
            set
            {
                _BtnStopBackgrCol = value;
                OnPropertyChanged();
            }
        }

        private string _sliderValue;

        public string sliderValue
        {
            get { return _sliderValue; }
            set {
                _sliderValue = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand BtnGetNewValue { get; set; }
        public RelayCommand BtnGetFeat1 { get; set; }
        public RelayCommand BtnGetFeat2 { get; set; }
        public RelayCommand BtnGetFeat3 { get; set; }
        public RelayCommand BtnClearData { get; set; }
        public RelayCommand BtnGetNewValCyclic { get; set; }
        public RelayCommand BtnStopCyclic { get; set; }

        public mainViewCtrl()
        {
            c1202Com = new C1202Com(this);
            c1202Com.getPort();
            c1202Com.portListen();

            //initial values:
            featureValue1 = "-.-";
            featureValue2 = "-.-";
            featureValue3 = "-.-";
            backgrColor1 = "Transparent";
            backgrColor2 = "Transparent";
            backgrColor3 = "Transparent";

            BtnStartBackgrCol = "LightGreen";
            BtnStopBackgrCol = "LightGray";

            sliderValue = "4";

            valueList1 = new List<dataDisplModel>();
            valueList2 = new List<dataDisplModel>();
            valueList3 = new List<dataDisplModel>();
            
            BtnGetNewValue = new RelayCommand(o =>
            {
                c1202Com.getAllMeasurements();
            });
            BtnGetFeat1 = new RelayCommand(o =>
            {
                c1202Com.getFeature(1);
            });
            BtnGetFeat2 = new RelayCommand(o =>
            {
                c1202Com.getFeature(2);
            });
            BtnGetFeat3 = new RelayCommand(o =>
            {
                c1202Com.getFeature(3);
            });

            BtnGetNewValCyclic = new RelayCommand(o =>
            {
                cyclicRequest();
                BtnStartBackgrCol = "LightGray";
                BtnStopBackgrCol = "LightSalmon";
            });
            BtnStopCyclic = new RelayCommand(o =>
            {
                stopRequest();
                BtnStartBackgrCol = "LightGreen";
                BtnStopBackgrCol = "LightGray";
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


        public void updateValues(int featureNo) // 0 - all features, 1..3 only one feature
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
            return c1202Com.featureValues[featureNo].valueText + " " + c1202Com.featureValues[featureNo].unit;
        }
        private String evalOneFeatColor(int featureNo)
        {
            String result = "Transparent";
            if (c1202Com.featureValues[featureNo].isToleranceEnabled == true)
            {
                result = "Green";
                if (c1202Com.featureValues[featureNo].isInWarning == false)
                {
                    result = "Yellow";
                }
                if (c1202Com.featureValues[featureNo].isInTolerance == false)
                {
                    result = "Red";
                }
            }
            return result;
        }
        public void cyclicRequest()
        {
            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval =  int.Parse(sliderValue) * 1000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }
        public void stopRequest()
        {
            try
            {
                aTimer.Stop();
            }catch (Exception e)
            {
                
            }
            
        }


            private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            c1202Com.getAllMeasurements();
        }


    }
}

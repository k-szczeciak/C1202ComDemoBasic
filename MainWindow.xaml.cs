using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows;
using C1202ComDemoBasic.Services;
using C1202ComDemoBasic.Model;
using C1202ComDemoBasic.Core;

namespace C1202ComDemoBasic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainViewCtrl testObj = (mainViewCtrl)Application.Current.MainWindow.DataContext;
            testObj.deviceCom.Close();
        }
    }
}

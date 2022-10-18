using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebServer.Models;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Program ServerThread = new Program();
            ServerThread.Start();
        }

        private void postJob_Click(object sender, RoutedEventArgs e)
        {

        }

        private void attachFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void viewJobs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

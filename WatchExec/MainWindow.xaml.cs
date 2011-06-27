using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WatchMan;
namespace WatchExec
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
        private List<WeWindow> windows = new List<WeWindow>();
        private WatchManager watchManager = new WatchManager(false);
        private void buttonCreateWE_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = "";
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Please select WE files";
            openFileDialog.Filter = "WatchExecutor settings|*.wes|All files|*.*";
            bool? filesSelected = openFileDialog.ShowDialog();
            if (filesSelected.HasValue && filesSelected.Value == true)
            {
                //this.Title = "OK";
                foreach (string filename in openFileDialog.FileNames)
                {
                    WeWindow wew = new WeWindow(filename, watchManager);
                    windows.Add(wew);
                    wew.Show();
                }
            }
            //else this.Title = "ne OK";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach(WeWindow w in windows)
            {
                try
                {
                    w.Close();
                }
                catch { }
            }
        }
    }
}

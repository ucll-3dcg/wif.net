using Microsoft.Win32;
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
using System.Windows.Shapes;
using WifViewer.ViewModels;

namespace WifViewer
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();

            this.DataContext = new ConfigurationViewModel();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            ((ConfigurationViewModel)DataContext).AcceptChanges();
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Executables|*.exe",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                ((ConfigurationViewModel)DataContext).RayTracerPath.Value = dlg.FileName;
            }
        }
    }
}

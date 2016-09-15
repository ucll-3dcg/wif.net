using Cells;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WifViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        private readonly DispatcherTimer timer;

        public MainWindow()
        {
            this.AnimationSpeed = Cell.Create(30);
            this.AnimationSpeed.ValueChanged += OnAnimationSpeedChanged;

            InitializeComponent();

            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(25), DispatcherPriority.ApplicationIdle, OnTimerTick, this.Dispatcher);
            timer.IsEnabled = true;

            this.DataContext = new ViewModel(this);
        }

        private void OnTimerTick(object sender, EventArgs args)
        {
            dynamic vm = DataContext;

            vm.Tick();
        }

        public string GetWifPath()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Wif Files|*.wif";
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public string GetExportPath()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "MP4 Files|*.mp4";
            dialog.CheckFileExists = false;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public Cell<int> AnimationSpeed { get; }

        private void OnAnimationSpeedChanged()
        {
            this.timer.Interval = TimeSpan.FromMilliseconds(AnimationSpeed.Value);
        }
    }
}

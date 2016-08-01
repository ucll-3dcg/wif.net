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
            InitializeComponent();

            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(25), DispatcherPriority.ApplicationIdle, OnTimerTick, this.Dispatcher);
            timer.IsEnabled = true;

            this.DataContext = new ViewModel(this);
            Open = new EnabledCommand(OnOpen);

            //this.frames = Cell.Create(WifLoader.Load(@"e:\temp\output\test.wif"));
            //this.CurrentImageIndex = Cell.Create(0);
            //this.CurrentImage = Cell.Derived(frames, CurrentImageIndex, (fs, i) => fs[i]);
            //this.FrameCount = Cell.Derived(frames, fs => fs.Count);
            //this.MaximumImageIndex = Cell.Derived(frames, fs => fs.Count - 1);
            //this.IsAnimating = Cell.Create(false);
            //this.DataContext = this;

            //this.IsAnimating.ValueChanged += OnIsAnimatingChanged;
        }

        public ICommand Open { get; }

        private void OnTimerTick(object sender, EventArgs args)
        {
            // CurrentImageIndex.Value = (CurrentImageIndex.Value + 1) % FrameCount.Value;
        }

        private void OnOpen()
        {
            
        }

        public string AskUserForFilename()
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

        //private void OnIsAnimatingChanged()
        //{
        //    this.timer.IsEnabled = IsAnimating.Value;
        //}

        //public Cell<int> FrameCount { get; }

        //public Cell<bool> IsAnimating { get; }

        //public Cell<WriteableBitmap> CurrentImage { get; }

        //public Cell<int> CurrentImageIndex { get; }

        //public Cell<int> MaximumImageIndex { get; }

        //private void UI_OnRefresh(object sender, RoutedEventArgs e)
        //{
        //    this.CurrentImageIndex.Value = 0;
        //    this.frames.Value = WifLoader.Load(@"e:\temp\output\test.wif");
        //}
    }
}

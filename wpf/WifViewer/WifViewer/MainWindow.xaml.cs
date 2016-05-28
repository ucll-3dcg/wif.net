using Cells;
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

namespace WifViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Cell<List<WriteableBitmap>> frames;

        public MainWindow()
        {
            InitializeComponent();

            this.frames = Cell.Create(WifLoader.Load(@"e:\temp\output\test.wif"));
            this.CurrentImageIndex = Cell.Create(0);
            this.CurrentImage = Cell.Derived(frames, CurrentImageIndex, (fs, i) => fs[i]);
            this.MaximumImageIndex = Cell.Derived(frames, fs => fs.Count - 1);
            this.DataContext = this;
        }

        public Cell<WriteableBitmap> CurrentImage { get; }

        public Cell<int> CurrentImageIndex { get; }

        public Cell<int> MaximumImageIndex { get; }
    }
}

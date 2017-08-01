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
using System.Windows.Shapes;
using System.Globalization;

namespace WifViewer
{
    /// <summary>
    /// Interaction logic for AnimationWindow.xaml
    /// </summary>
    public partial class AnimationWindow : Window
    {
        public AnimationWindow(AnimationViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;

            vm.MaximumFrameIndex.ValueChanged += () =>
            {
                if (vm.MaximumFrameIndex.Value == 0)
                {
                    resultTab.Visibility = Visibility.Visible;
                    tabControl.SelectedIndex = 0;
                }
            };         
        }        
    }
}

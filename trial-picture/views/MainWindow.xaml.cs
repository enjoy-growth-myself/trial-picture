using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using OpenCvSharp;

namespace trial_picture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = new MainViewModel();
        }
    }
}
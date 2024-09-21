using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using trial_picture.models;


namespace trial_picture
{
    /// <summary>
    /// PictureWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PictureWindow : System.Windows.Window
    {
        private VideoCapture _videoCapture;
        private Mat _mat;
        private bool _isRunning;

        public PictureWindow()
        {
            InitializeComponent();
            this.DataContext = new PictureViewModel();
            //StartCamera();
        }

        
       

        

        protected override void OnClosed(EventArgs e)
        {
            _isRunning = false;
            _videoCapture?.Release();
            _videoCapture?.Dispose();
            base.OnClosed(e);
        }
    }
}

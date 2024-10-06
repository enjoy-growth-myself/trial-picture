using OpenCvSharp;

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
        private PictureViewModel _pictureViewModel;

        public PictureWindow(int cameraNo)
        {
            InitializeComponent();
            _pictureViewModel = new PictureViewModel(cameraNo);
            this.DataContext = _pictureViewModel;
        }

        // Windowの閉じるイベントをViewModelに委譲
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ICommandのExecuteを呼び出し、処理を委譲
            _pictureViewModel.OnWindowClosing(e);

        }
    }
}

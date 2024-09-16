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
        private List<Picture> _pictures;
        private int _id = 0;

        public PictureWindow()
        {
            InitializeComponent();
            _pictures = new List<Picture>();
            StartCamera();
        }

        private async void StartCamera()
        {
            _videoCapture = new VideoCapture(0); // 0はデフォルトカメラを指します
            _videoCapture.Open(0); // カメラを開く

            if (!_videoCapture.IsOpened())
            {
                MessageBox.Show("カメラを開くことができませんでした");
                return;
            }

            _isRunning = true;

            // 別スレッドでカメラフレームを取得する
            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    using (var frame = new Mat())
                    {
                        _videoCapture.Read(frame);
                        if (frame.Empty())
                            continue;

                        _mat = frame.Clone();
                        // UIスレッドでImageコントロールにフレームを表示
                        Dispatcher.Invoke(() =>
                        {
                            pictureImage.Source = frame.ToBitmapSource();
                        });
                    }
                }
            });
        }

        private void getCaptureOnClick(object sender, RoutedEventArgs e)
        { 
            Mat mat = _mat.Clone();
            Picture currentPicture = new Picture { Id = _id, mat = mat };

            _pictures.Add(currentPicture);

            var image  = mat.ToBitmapSource();

            picturesList.Items.Add(image);

            _id += 1;

            Console.WriteLine("テスト");
        }

        public void saveCaptureOnClick(object sender, RoutedEventArgs e)
        {
            var selectedImage = picturesList.SelectedItem as BitmapSource;
            if (selectedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG ファイル (*.jpg)|*.jpg"; // PNGファイル形式を指定
                saveFileDialog.Title = "保存先を選択してください";
                saveFileDialog.FileName = "image.png";  // デフォルトのファイル名

                // ダイアログがOKで閉じられた場合のみ保存処理を行う
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // 画像を指定された場所に保存
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(selectedImage));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }

                    MessageBox.Show($"画像が保存されました: {filePath}");
                }

            }
            else
            {
                MessageBox.Show("保存する画像を選択してください");
            }
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

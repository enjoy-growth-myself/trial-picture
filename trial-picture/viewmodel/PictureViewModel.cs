using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using trial_picture.models;
using System.Windows.Input;
using System.Collections.ObjectModel;


namespace trial_picture
{
    public class PictureViewModel : BaseViewModel
    {
        public ICommand RelayCommand { get; }
        private VideoCapture _videoCapture;
        private Mat _mat;
        private bool _isRunning;
        private List<Picture> _pictures;
        private int _id = 0;
        private BitmapSource _currentImage;
        private ICommand _getCaptureOnClickCommand;
        private ICommand _saveCaptureOnClickCommnad;
        private BitmapSource _selectedImage;

        public ObservableCollection<BitmapSource> CaptureImages { get; set; }

        public PictureViewModel()
        {
            _pictures = new List<Picture>();
            CaptureImages = new ObservableCollection<BitmapSource>();
            getCaptureOnClickCommand = new RelayCommand(getCaptureOnClick);
            saveCaptureOnClickCommnad = new RelayCommand(saveCaptureOnClick);
            StartCamera();
        }

        public BitmapSource PictureImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();  // プロパティの変更を通知してUIを更新
            }
        }
       
        public ICommand getCaptureOnClickCommand
        {
            get { return _getCaptureOnClickCommand; }
            set
            {
                _getCaptureOnClickCommand = value;
                OnPropertyChanged();  // プロパティ変更を通知
            }
        }

        public ICommand saveCaptureOnClickCommnad
        {
            get { return _saveCaptureOnClickCommnad; }
            set
            {
                _saveCaptureOnClickCommnad = value;
                OnPropertyChanged();  // プロパティ変更を通知
            }
        }

        public BitmapSource SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged();  // 選択された画像をUIに通知
            }
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
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            PictureImage = frame.ToBitmapSource();
                        });
                    }
                }
            });
        }

        private void getCaptureOnClick(object parameter)
        {
            Mat mat = _mat.Clone();

            var image = mat.ToBitmapSource();

            CaptureImages.Add(image);

            Console.WriteLine("テスト");
        }

        public void saveCaptureOnClick(object parameter)
        {
            
            if (SelectedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG ファイル (*.jpg)|*.jpg"; // PNGファイル形式を指定
                saveFileDialog.Title = "保存先を選択してください";
                saveFileDialog.FileName = "image.jpg";  // デフォルトのファイル名

                // ダイアログがOKで閉じられた場合のみ保存処理を行う
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // 画像を指定された場所に保存
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(SelectedImage));

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
    }
}

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
        private int _cameraNo = 0;
        private BitmapSource _currentImage;
        private BitmapSource _selectedImage;
        private ICommand _getCaptureOnClickCommand;
        private ICommand _saveCaptureOnClickCommand;
        private ICommand _onClosedCommand;

        public ObservableCollection<BitmapSource> CaptureImages { get; set; }

        public PictureViewModel(int cameraNo)
        {
            _cameraNo = cameraNo;
            _pictures = new List<Picture>();
            CaptureImages = new ObservableCollection<BitmapSource>();
            getCaptureOnClickCommand = new RelayCommand(getCaptureOnClick);
            saveCaptureOnClickCommand = new RelayCommand(saveCaptureOnClick);
            onClosedCommand = new CloseCommand<System.Windows.Window>(OnClosed);
            startCamera();
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

        public BitmapSource SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged();  // 選択された画像をUIに通知
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

        public ICommand saveCaptureOnClickCommand
        {
            get { return _saveCaptureOnClickCommand; }
            set
            {
                _saveCaptureOnClickCommand = value;
                OnPropertyChanged();  // プロパティ変更を通知
            }
        }

        public ICommand onClosedCommand
        {
            get { return _onClosedCommand; }
            set
            {
                _onClosedCommand = value;
                OnPropertyChanged();  // プロパティ変更を通知
            }
        }


        private async void startCamera()
        {
            _videoCapture = new VideoCapture(); 
            _videoCapture.Open(_cameraNo); // カメラを開く

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
        }

        public void saveCaptureOnClick(object parameter)
        {
            
            if (SelectedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG ファイル (*.jpg)|*.jpg"; 
                saveFileDialog.Title = "保存先を選択してください";
                saveFileDialog.FileName = "image.jpg"; 

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
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

        private void OnClosed(System.Windows.Window window)
        {
            if(_isRunning && _videoCapture !=null && _videoCapture.IsOpened() && window != null)
            {
                _isRunning = false;
                _videoCapture.Release();
                _videoCapture.Dispose();
                window.Close();
            }
        }
    }
}

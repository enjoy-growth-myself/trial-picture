using OpenCvSharp;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace trial_picture
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand OpenPictureWindowCommand { get; }
        public ICommand ClosePictureWindowCommand { get; }
        public ObservableCollection<string> Cameras { get; set; }

        public string SelectedCamera { get; set; }


        public MainViewModel()
        {
            Cameras = new ObservableCollection<string>();
            // コマンドの初期化
            OpenPictureWindowCommand = new RelayCommand(OpenPictureWindow);
            LoadVideoDevices();
        }
        private void OpenPictureWindow(object parameter)
        {
            int cameraNo = 0;
            if (!string.IsNullOrEmpty(SelectedCamera))
            {
                var test = SelectedCamera;
                if (SelectedCamera.Contains("1"))
                {
                    cameraNo = 1;
                }

                PictureWindow secondWindow = new PictureWindow(cameraNo);
                secondWindow.Show();
            }
        }

        private void LoadVideoDevices()
        {
            try
            {
                //今回は内部カメラとスマホ接続のみを考慮
                for (int i = 0; i < 2; i++)
                {
                    using (var capture = new VideoCapture(i))
                    {
                        if (capture.IsOpened())
                        {
                            if(i == 0)
                            {
                                Cameras.Add($"PC内臓カメラ {0}");
                            }
                            if(i == 1)
                            {
                                Cameras.Add($"外部接続カメラ {1}");
                            }
                        }
                    }
                }

                if (Cameras.Count > 0)
                {
                    SelectedCamera = Cameras[0];
                    OnPropertyChanged(nameof(SelectedCamera));
                }
                else
                {
                    MessageBox.Show("カメラデバイスが見つかりませんでした。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("カメラデバイスの取得に失敗しました: " + ex.Message);
            }
        }
    }
}

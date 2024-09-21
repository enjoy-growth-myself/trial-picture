using System.Windows.Input;

namespace trial_picture
{
    public class MainViewModel
    {
        public ICommand OpenPictureWindowCommand { get; }

        public MainViewModel()
        {
            // コマンドの初期化
            OpenPictureWindowCommand = new RelayCommand(OpenPictureWindow);
        }
        private void OpenPictureWindow(object parameter)
        {
            Console.WriteLine("コンソールでの出力確認");
            PictureWindow secondWindow = new PictureWindow();
            secondWindow.Show();
        }
    }
}

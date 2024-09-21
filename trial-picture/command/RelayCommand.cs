using System.Windows.Input;

namespace trial_picture
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }
        // CanExecuteChanged イベントの実装
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // コマンドが常に実行可能な場合
        public bool CanExecute(object parameter)
        {
            return true; // 常に実行可能
        }

        // コマンドが実行された際の処理
        public void Execute(object parameter)
        {
            // 新しいウィンドウを開く処理
            _execute(parameter);
        }
    }
}

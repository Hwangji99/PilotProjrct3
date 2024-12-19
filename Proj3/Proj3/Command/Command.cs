using System.Windows.Input;

namespace Proj3.Command
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute; // 실제로 실행할 작업
        private readonly Func<object, bool>? _canExecute; // 실행 가능 여부를 결정하는 함수

        // 생성자: 실행할 작업과 실행 가능 여부를 설정
        public Command(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged; // CanExecute 변경 이벤트 (UI 갱신을 위해 사용)

        // 실행 가능 여부를 반환
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        // 실제로 작업을 실행
        public void Execute(object? parameter) => _execute(parameter);

        // CanExecuteChanged 이벤트를 수동으로 호출 (UI 갱신 필요 시)
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

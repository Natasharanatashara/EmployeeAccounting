// EmployeeExceptions.cs
namespace EmployeeManagementSystem
{
    /// <summary>
    /// Исключение при дублировании имени сотрудника.
    /// </summary>
    public sealed class EmployeeNameAlreadyExistsException : Exception
    {
        public EmployeeNameAlreadyExistsException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Исключение при отсутствии сотрудника.
    /// </summary>
    public sealed class NoSuchEmployeeException : Exception
    {
        public NoSuchEmployeeException(string message)
            : base(message)
        {
        }
    }
}

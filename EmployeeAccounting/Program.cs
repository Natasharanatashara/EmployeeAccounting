// Создайте консольное приложение, которое позволит управлять данными сотрудников.
// Программа должна обеспечивать функционал добавления, обновления, получения информации о работниках и расчета их зарплаты.

using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementSystem
{
    /// <summary>
    /// Интерфейс для управления сотрудниками.
    /// </summary>
    public interface IEmployeeManager<T> where T : Employee
    {
        /// <summary>
        /// Добавляет нового сотрудника.
        /// </summary>
        void Add(T employee);

        /// <summary>
        /// Получает сотрудника по имени.
        /// </summary>
        T Get(string name);

        /// <summary>
        /// Обновляет информацию о сотруднике.
        /// </summary>
        void Update(T employee);

        /// <summary>
        /// Получает список всех сотрудников.
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Удаляет сотрудника по идентификатору.
        /// </summary>
        void Remove(int id);
    }

    /// <summary>
    /// Методы для управления данными сотрудника
    /// </summary>
    public sealed class EmployeeManager : IEmployeeManager<Employee>
    {
        private readonly List<Employee> employees = new List<Employee>();
        private int nextId = 1;

        public void Add(Employee employee)
        {
            if (this.employees.Any(e => e.Name.Equals(employee.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new EmployeeNameAlreadyExistsException($"Сотрудник с именем '{employee.Name}' уже существует.");
            }

            employee.Id = this.nextId++;
            this.employees.Add(employee);
        }

        public Employee Get(string name)
        {
            return this.employees.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? throw new NoSuchEmployeeException($"Сотрудник с именем '{name}' не найден.");
        }

        public void Update(Employee employee)
        {
            var existing = this.GetById(employee.Id);

            if (this.employees.Any(e => e.Id != employee.Id && e.Name.Equals(employee.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new EmployeeNameAlreadyExistsException($"Сотрудник с именем '{employee.Name}' уже существует.");
            }

            existing.Name = employee.Name;
            existing.BaseSalary = employee.BaseSalary;
            existing.HoursWorked = employee.HoursWorked;
        }

        public IEnumerable<Employee> GetAll()
        {
            return this.employees;
        }

        public void Remove(int id)
        {
            var employee = this.GetById(id);
            this.employees.Remove(employee);
        }

        private Employee GetById(int id)
        {
            return this.employees.FirstOrDefault(e => e.Id == id)
                ?? throw new NoSuchEmployeeException($"Сотрудник с ID={id} не найден.");
        }
    }
  
    /// <summary>
    /// Главный класс программы.
    /// </summary>
    public static class Program
    {
        private const decimal MinHourlyRate = 100;
        private const int MaxWorkHours = 168;
        private static readonly EmployeeManager EmployeeManager = new EmployeeManager();

        public static void Main()
        {
            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        UpdateEmployee();
                        break;
                    case "3":
                        ViewEmployees();
                        break;
                    case "4":
                        CalculateSalary();
                        break;
                    case "5":
                        RemoveEmployee();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Повторите попытку.");
                        break;
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Добавить сотрудника");
            Console.WriteLine("2. Обновить информацию о сотруднике");
            Console.WriteLine("3. Просмотреть список сотрудников");
            Console.WriteLine("4. Рассчитать заработную плату сотрудника");
            Console.WriteLine("5. Удалить сотрудника");
            Console.WriteLine("0. Выход");
        }

        private static void AddEmployee()
        {
            try
            {
                Console.Write("Имя сотрудника: ");
                string name = Console.ReadLine().Trim();

                Console.WriteLine("Тип сотрудника:");
                Console.WriteLine("1. Полный рабочий день");
                Console.WriteLine("2. Частичная занятость");
                Console.Write("Выберите тип: ");
                string typeChoice = Console.ReadLine();

                decimal baseSalary = GetDecimalInput("Базовая ставка: ", MinHourlyRate);
                int hoursWorked = GetIntInput("Количество отработанных часов: ", 0, MaxWorkHours);

                Employee employee = typeChoice == "1"
                    ? new FullTimeEmployee(0, name, baseSalary, hoursWorked)
                    : new PartTimeEmployee(0, name, baseSalary, hoursWorked);

                EmployeeManager.Add(employee);
                Console.WriteLine($"Сотрудник '{name}' успешно добавлен!");
            }
            catch (EmployeeNameAlreadyExistsException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static void UpdateEmployee()
        {
            try
            {
                int id = GetIntInput("Введите ID сотрудника для обновления: ");
                var employee = EmployeeManager.GetAll().FirstOrDefault(e => e.Id == id);

                if (employee == null)
                {
                    Console.WriteLine("Сотрудника с таким ID не существует.");
                    return;
                }

                while (true)
                {
                    Console.WriteLine("\nЧто требуется изменить?");
                    Console.WriteLine("1. Имя сотрудника");
                    Console.WriteLine("2. Базовую ставку");
                    Console.WriteLine("3. Отработанные часы");
                    Console.WriteLine("4. Сохранить изменения и выйти");
                    Console.WriteLine("5. Выйти без сохранения");

                    string choice = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(choice))
                    {
                        Console.WriteLine("Необходимо выбрать один из вариантов.");
                        continue;
                    }

                    switch (choice)
                    {
                        case "1":
                            Console.Write("Введите новое имя сотрудника: ");
                            employee.Name = Console.ReadLine().Trim();
                            Console.WriteLine("Имя обновлено успешно.");
                            break;

                        case "2":
                            employee.BaseSalary = GetDecimalInput("Новая базовая ставка: ", MinHourlyRate);
                            break;

                        case "3":
                            employee.HoursWorked = GetIntInput("Новое количество отработанных часов: ", 0, MaxWorkHours);
                            break;

                        case "4":
                            EmployeeManager.Update(employee);
                            Console.WriteLine("Изменения сохранены.");
                            return;

                        case "5":
                            Console.WriteLine("Изменения не сохранены.");
                            return;

                        default:
                            Console.WriteLine("Недопустимый выбор. Выберите снова.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static void ViewEmployees()
        {
            var employees = EmployeeManager.GetAll();
            if (!employees.Any())
            {
                Console.WriteLine("Список сотрудников пуст.");
                return;
            }

            foreach (var emp in employees)
            {
                string type = emp is FullTimeEmployee ? "Полный день" : "Частичная занятость";
                Console.WriteLine($"{emp.Id}: {type} - Имя: {emp.Name}, Базовая ставка: {emp.BaseSalary}, Часы: {emp.HoursWorked}");
            }
        }

        private static void CalculateSalary()
        {
            try
            {
                int id = GetIntInput("Введите ID сотрудника для расчёта заработной платы: ");
                var emp = EmployeeManager.GetAll().FirstOrDefault(e => e.Id == id);

                if (emp == null)
                {
                    Console.WriteLine("Сотрудника с указанным ID не существует.");
                    return;
                }

                decimal salary = emp.CalculateSalary();
                Console.WriteLine($"Зарплата сотрудника {emp.Name} составляет {salary} рублей");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static void RemoveEmployee()
        {
            try
            {
                int id = GetIntInput("Введите ID сотрудника для удаления: ");
                EmployeeManager.Remove(id);
                Console.WriteLine($"Сотрудник с ID {id} удалён.");
            }
            catch (NoSuchEmployeeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static decimal GetDecimalInput(string prompt, decimal minValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal result) && result >= minValue)
                {
                    return result;
                }
                Console.WriteLine($"Некорректный ввод. Введите число не меньше {minValue}.");
            }
        }

        private static int GetIntInput(string prompt, int minValue, int maxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result) && result >= minValue && result <= maxValue)
                {
                    return result;
                }
                Console.WriteLine($"Некорректный ввод. Введите число от {minValue} до {maxValue}.");
            }
        }

        private static int GetIntInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("Некорректный ввод. Введите целое число.");
            }
        }
    }
}

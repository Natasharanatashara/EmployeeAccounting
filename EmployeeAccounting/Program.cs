// Создайте консольное приложение, которое позволит управлять данными сотрудников.
// Программа должна обеспечивать функционал добавления, обновления, получения информации о работниках и расчета их зарплаты.

using System;
using System.Collections.Generic;
using System.Xml.Linq;

class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Wages { get; set; }
    public int HoursWorked { get; set; }

    // Конструктор класса Employee
    public Employee(int id, string name, double wages, int hoursWorked)
    {
        this.Id = id;
        this.Name = name;
        this.Wages = wages;
        this.HoursWorked = hoursWorked;
    }
}


class Program
{
    static List<Employee> employees = new List<Employee>();
    static int nextId = 1;

    const double MIN_HOURLY_RATE = 100; // Минимальная почасовая ставка, рублей
    const int MAX_WORK_HOURS = 168;      // Максимально возможное количество часов в неделю

    
    // Метод вывода меню 
    static void ShowMenu()
    {
        Console.WriteLine("\nМеню:");
        Console.WriteLine("1. Добавить сотрудника");
        Console.WriteLine("2. Обновить информацию о сотруднике");
        Console.WriteLine("3. Просмотреть список сотрудников");
        Console.WriteLine("4. Рассчитать заработную плату сотрудника");
        Console.WriteLine("5. Удалить сотрудника");   // новый пункт меню
        Console.WriteLine("0. Выход");
    }

    // Главный цикл программы
    static void Main()
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



    // Конструкторы с исключениями
    public class EmployeeNameAlreadyExistsException : Exception   // Дубль сотрудника по имени
    {
        public EmployeeNameAlreadyExistsException(string message) : base(message) { }
    }
    public class EmployeeAlreadyExistsException : Exception     // Дубль ID
    {
        public EmployeeAlreadyExistsException(string message) : base(message) { }
    }

    public class InvalidWagesException : Exception    // Не найден сотрудник
    {
        public InvalidWagesException(string message) : base(message) { }
    }

    public class NegativeHoursException : Exception  // Исключение для отрицательных часов
    {
        public NegativeHoursException(string message) : base(message) { }
    }

    public class OverworkedHoursException : Exception  // Исключение для перебора часов
    {
        public OverworkedHoursException(string message) : base(message) { }
    }

    public class NoSuchEmployeeException : Exception   // Некорректно введенный сотрудник
    {
        public NoSuchEmployeeException(string message) : base(message) { }
    }


    // Проверка наличия сотрудников с таким же именем

    private static bool CheckIfEmployeeWithSameNameExists(string name)
    {
        return employees.Any(e => e.Name.ToLower() == name.ToLower());
    }


    // Проверка существования сотрудника с данным ID
    static void CheckIfEmployeeWithSameIdExists(int id)
    {
        if (employees.Exists(e => e.Id == id))
        {
            throw new EmployeeAlreadyExistsException($"Сотрудник с ID={id} уже существует.");
        }
    }

    // Нахождение сотрудника по ID
    static Employee FindEmployeeById(int id)
    {
        var foundEmp = employees.Find(e => e.Id == id);
        if (foundEmp == null)
        {
            throw new NoSuchEmployeeException($"Сотрудник с ID={id} не найден.");
        }
        return foundEmp;
    }

    // Ввод дробных чисел
    static bool TryGetDoubleInput(string prompt, out double result)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out result))
                return true;
            else
                Console.WriteLine("Некорректный ввод. Введите действительное число.");
        }
    }

    // Ввод целых чисел
    static bool TryGetIntInput(string prompt, out int result)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out result))
                return true;
            else
                Console.WriteLine("Некорректный ввод. Введите целое число.");
        }
    }


    // Метод добавления сотрудника
    static void AddEmployee()
    {
        Console.Write("Имя сотрудника: ");
        string name = Console.ReadLine().Trim();

        // Проверка существования сотрудника с таким же именем
        if (CheckIfEmployeeWithSameNameExists(name))
        {
            Console.WriteLine("Сотрудник с именем уже существует");
            return;
        }

        double wages;
        do
        {
            if (!TryGetDoubleInput("Почасовая ставка (рублей/час): ", out wages))
                continue;

            if (wages < MIN_HOURLY_RATE)
            {
                Console.WriteLine($"Минимальная почасовая ставка должна быть не менее {MIN_HOURLY_RATE}. Повторите ввод.");
            }
        } while (wages < MIN_HOURLY_RATE);

        int hoursWorked;
        do
        {
            if (!TryGetIntInput("Количество отработанных часов: ", out hoursWorked))
                continue;

            if (hoursWorked < 0)
            {
                Console.WriteLine("Отработанные часы не могут быть отрицательными. Повторите ввод.");
            }
            else if (hoursWorked > MAX_WORK_HOURS)
            {
                Console.WriteLine($"Максимальное количество часов превышает лимит ({MAX_WORK_HOURS}). Повторите ввод.");
            }
        } while (hoursWorked < 0 || hoursWorked > MAX_WORK_HOURS);

        try
        {
            CheckIfEmployeeWithSameIdExists(nextId); // проверка на уникальный ID
            Employee employee = new Employee(nextId++, name, wages, hoursWorked);
            employees.Add(employee);
            Console.WriteLine($"Сотрудник '{name}' успешно добавлен!");
        }
        catch (EmployeeAlreadyExistsException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (EmployeeNameAlreadyExistsException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



    // Метод изменения данных по сотруднику - case "3"

    static void UpdateEmployee()
    {
        Console.Write("Введите ID сотрудника для обновления: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный ввод ID.");
            return;
        }

        var empToUpdate = employees.Find(e => e.Id == id); // Поиск сотрудника по введенному ID

        if (empToUpdate == null)
        {
            Console.WriteLine("Сотрудника с таким ID не существует.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\nЧто требуется изменить?");
            Console.WriteLine("6. Имя сотрудника");
            Console.WriteLine("7. Почасовую ставку");
            Console.WriteLine("8. Отработанные часы");
            Console.WriteLine("9. Выход в главное меню");

            string choice = Console.ReadLine()?.Trim(); // Получаем выбор пользователя и очищаем пробелы

            if (string.IsNullOrEmpty(choice)) // Проверка на пустой ввод
            {
                Console.WriteLine("Необходимо выбрать один из вариантов.");
                continue;
            }

            switch (choice)
            {
                case "6": // Изменение имени сотрудника
                    Console.Write("Введите новое имя сотрудника: ");
                    string newName = Console.ReadLine().Trim();

                    if (CheckIfEmployeeWithSameNameExists(newName))
                    {
                        Console.WriteLine("Сотрудник с именем уже существует");
                        return;
                    }

                    if (newName != "")
                    {
                        empToUpdate.Name = newName;
                        Console.WriteLine("Имя обновлено успешно.");
                    }
                    else
                    {
                        Console.WriteLine("Имя не может быть пустым.");
                    }
                    break;

                case "7": // Изменение почасовой ставки
                    double newWageRate;
                    if (TryGetDoubleInput("Новая почасовая ставка (рублей/час): ", out newWageRate))
                    {
                        if (newWageRate >= MIN_HOURLY_RATE)
                            empToUpdate.Wages = newWageRate;
                        else
                            Console.WriteLine($"Минимальная почасовая ставка должна быть не менее {MIN_HOURLY_RATE}.");
                    }
                    break;

                case "8": // Изменение количества отработанных часов
                    int newHoursWorked;
                    if (TryGetIntInput("Новое количество отработанных часов: ", out newHoursWorked))
                    {
                        if (newHoursWorked >= 0 && newHoursWorked <= MAX_WORK_HOURS)
                            empToUpdate.HoursWorked = newHoursWorked;
                        else
                            Console.WriteLine($"Часы должны быть между 0 и {MAX_WORK_HOURS}.");
                    }
                    break;

                case "9":
                    Console.WriteLine("Возврат в главное меню...");
                    return; // Выходим из цикла и возвращаемся в главное меню

                default:
                    Console.WriteLine("Недопустимый выбор. Выберите снова.");
                    break;
            }
        }
    }



    // Метод просмотра списка сотрудников - case "3"
    static void ViewEmployees()
    {
        foreach (var emp in employees)
        {
            Console.WriteLine($"{emp.Id}: Имя - {emp.Name}, Почасовая ставка - {emp.Wages}, Отработано часов - {emp.HoursWorked}");
        }
    }

    // Метод расчета зарплаты - case "4"
    static void CalculateSalary()
    {
        int id;
        if (!TryGetIntInput("Введите ID сотрудника для расчёта заработной платы: ", out id)) return;

        var emp = employees.Find(e => e.Id == id);
        if (emp == null)
        {
            Console.WriteLine("Сотрудника с указанным ID не существует.");
            return;
        }

        double salary = emp.Wages * emp.HoursWorked;
        Console.WriteLine($"Зарплата сотрудника {emp.Name} составляет {salary} рублей");
    }


    // Метод удаления сотрудника - case "5"
    static void RemoveEmployee()
    {
        int id;
        if (!TryGetIntInput("Введите ID сотрудника для удаления: ", out id)) return;

        var emp = employees.Find(e => e.Id == id);
        if (emp == null)
        {
            Console.WriteLine("Сотрудника с указанным ID не существует.");
            return;
        }

        employees.Remove(emp);
        Console.WriteLine($"Сотрудник с ID {id} удалён.");
    }

 }


// Создайте консольное приложение, которое позволит управлять данными сотрудников.
// Программа должна обеспечивать функционал добавления, обновления, получения информации о работниках и расчета их зарплаты.

using System;
using System.Collections.Generic;

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

    // Метод вывода меню
    static void ShowMenu()
    {
        Console.WriteLine("Меню:");
        Console.WriteLine("1. Добавить сотрудника");
        Console.WriteLine("2. Обновить информацию о сотруднике");
        Console.WriteLine("3. Просмотреть список сотрудников");
        Console.WriteLine("4. Рассчитать заработную плату сотрудника");
        Console.WriteLine("5. Выход");
    }

    static void Main(string[] args)
    {
        while (true)
        {
            ShowMenu();
            Console.Write("Выберите пункт меню: ");
            switch (Console.ReadLine())
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
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неправильный выбор пункта меню. Попробуйте снова.");
                    break;
            }
        }
    }


    // Добавление нового сотрудника
    static void AddEmployee()
    {
        Console.Write("Имя сотрудника: ");
        string name = Console.ReadLine();

        Console.Write("Почасовая ставка (руб/час): ");
        if (!double.TryParse(Console.ReadLine(), out var wages))
        {
            Console.WriteLine("Ошибка ввода ставки.");
            return;
        }

        Console.Write("Количество отработанных часов: ");
        if (!int.TryParse(Console.ReadLine(), out var hoursWorked))
        {
            Console.WriteLine("Ошибка ввода количества часов.");
            return;
        }

        Employee employee = new Employee(nextId++, name, wages, hoursWorked);
        employees.Add(employee);
        Console.WriteLine($"Сотрудник '{name}' успешно добавлен!");
    }

    // Изменение данных сотрудника
    static void UpdateEmployee()
    {
        Console.Write("Введите ID сотрудника для обновления: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный ввод ID.");
            return;
        }

        var empToUpdate = employees.Find(e => e.Id == id);
        if (empToUpdate == null)
        {
            Console.WriteLine("Сотрудника с таким ID не существует.");
            return;
        }

        Console.Write("Обновленное имя сотрудника: ");
        string updatedName = Console.ReadLine();
        empToUpdate.Name = updatedName;

        Console.Write("Новая почасовая ставка (руб/час): ");
        if (!double.TryParse(Console.ReadLine(), out var updatedWages))
        {
            Console.WriteLine("Ошибка ввода новой ставки.");
            return;
        }
        empToUpdate.Wages = updatedWages;

        Console.Write("Новое количество отработанных часов: ");
        if (!int.TryParse(Console.ReadLine(), out var updatedHoursWorked))
        {
            Console.WriteLine("Ошибка ввода новых часов.");
            return;
        }
        empToUpdate.HoursWorked = updatedHoursWorked;

        Console.WriteLine("Данные обновлены успешно.");
    }

    // Просмотр списка сотрудников
    static void ViewEmployees()
    {
        foreach (var emp in employees)
            Console.WriteLine($"{emp.Id}. Имя: {emp.Name}, Почасовая ставка: ${emp.Wages}/ч, Отработано часов: {emp.HoursWorked}");
    }

    // Расчет зарплаты одного сотрудника
    static void CalculateSalary()
    {
        Console.Write("Введите ID сотрудника для расчета зарплаты: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный ввод ID.");
            return;
        }

        var empForCalc = employees.Find(e => e.Id == id);
        if (empForCalc == null)
        {
            Console.WriteLine("Сотрудника с таким ID не существует.");
            return;
        }

        double salary = empForCalc.Wages * empForCalc.HoursWorked;
        Console.WriteLine($"Зарплата сотрудника '{empForCalc.Name}': {salary} рублей");
    }


}




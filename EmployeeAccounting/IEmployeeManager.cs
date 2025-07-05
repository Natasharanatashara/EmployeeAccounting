using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Методы интерфейса для управления данными сотрудника
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
}

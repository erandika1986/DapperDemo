using System.Collections.Generic;

namespace DapperDemo
{
    public interface IEmployeeRepository
    {
        Employee Find(int id);
        Employee FindFullEmployee(int id);
        List<Employee> GetAll();
        Employee Add(Employee employee);
        void AddFullEmployee(Employee employee);
        Employee Update(Employee employee);
        void Remove(int id);       
    }
}

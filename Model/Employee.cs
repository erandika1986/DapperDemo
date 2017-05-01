using System.Collections.Generic;

namespace DapperDemo
{
    public class Employee
    {
        public Employee()
        {
            Departments = new List<Department>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public List<Department> Departments { get; set; }
    }
}

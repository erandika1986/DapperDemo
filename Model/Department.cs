using System.Collections.Generic;

namespace DapperDemo
{
    public class Department
    {
        public Department()
        {
            Employees = new List<Employee>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

using System.Collections.Generic;

namespace DapperDemo
{
    public class Employee
    {
        public Employee()
        {
            Addresses = new List<Address>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public List<Address> Addresses { get; set; }

        public bool IsNew
        {
            get
            {
                return this.Id == default(int);
            }
        }
    }
}

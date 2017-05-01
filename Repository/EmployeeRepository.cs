using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Transactions;

namespace DapperDemo
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private IDbConnection db;

        public EmployeeRepository()
        {
            IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDB"].ConnectionString);
        }

        public Employee Find(int id)
        {
            return this.db.Query<Employee>("SELECT * FROM Employee WHERE Id = @id", new { id }).SingleOrDefault();
        }

        public Employee FindFullEmployee(int id)
        {
            var sqlQuery =
                "SELECT * FROM Employee WHERE Id = @Id; " +
                "SELECT * FROM Address WHERE EmployeeId = @Id";

            using (var multipleResult = this.db.QueryMultiple(sqlQuery, new { Id = id }))
            {
                var employee = multipleResult.Read<Employee>().SingleOrDefault();
                var addresses = multipleResult.Read<Address>().ToList();

                if (employee != null)
                {
                    employee.Addresses.AddRange(addresses);
                }

                return employee;
            }
        }

        public List<Employee> GetAll()
        {
            return this.db.Query<Employee>("SELECT * FROM Employee").ToList();
        }

        public Employee Add(Employee employee)
        {
            var sqlQuery =
                "INSERT INTO Employee(FirstName,LastName,Email,DepartmentId)" +
                " VALUES(@FirstName,@LastName,@Email,@DepartmentId);" +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = this.db.Query<int>(sqlQuery, employee).Single();
            employee.Id = id;

            return employee;
        }

        public Employee Update(Employee employee)
        {
            var sqlUpdateQuery =
                "UPDATE Employee  SET "+
                "   FirstName    = @FirstName, "+
                "   LastName     = @LastName," +
                "   Email        = @Email,"+
                "   DepartmentId = @DepartmentId "+
                "WHERE Id=@Id";

            this.db.Execute(sqlUpdateQuery, employee);

            return employee;
        }

        public void Remove(int id)
        {
            db.Execute("DELETE Employee WHERE Id=@id", new { id });
        }

        public void AddFullEmployee(Employee employee)
        {
            using (var transactionScope = new TransactionScope())
            {
                if (employee.IsNew)
                {
                    this.Add(employee);
                }
                else
                {
                    this.Update(employee);
                }

                foreach (var addr in employee.Addresses.Where(a => !a.IsDeleted))
                {
                    addr.EmployeeId = employee.Id;

                    if (addr.IsNew)
                    {
                        this.Add(addr);
                    }
                    else
                    {
                        this.Update(addr);
                    }

                }

                foreach (var addr in employee.Addresses.Where(a => a.IsDeleted))
                {
                    this.db.Execute("DELETE FROM Address WHERE Id = @Id", new { addr.Id });
                }

                transactionScope.Complete();
            }
        }

        public Address Add(Address address)
        {
            var sql =
                "INSERT INTO Address (Street, Address1, Country, PostalCode, EmployeeId) VALUES(@Street, @Address1, @Country, @PostalCode, @EmployeeId); " +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = this.db.Query<int>(sql, address).Single();
            address.Id = id;
            return address;
        }

        public Address Update(Address address)
        {
            this.db.Execute("UPDATE Address " +
                "SET Street = @Street, " +
                "    Address1 = @Address1, " +
                "    Country = @Country, " +
                "    PostalCode = @PostalCode, " +
                "    EmployeeId = @EmployeeId " +
                "WHERE Id = @Id", address);
            return address;
        }
    }
}

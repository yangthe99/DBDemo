using System;
using System.Data.SQLite;
using Dapper;
using System.Linq;

namespace DBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=DBDemo.db;Version=3;";

            // Step 1: 建立資料庫連線
            using (var connection = new SQLiteConnection(connectionString))
            {
                // Open connection
                connection.Open();

                // Step 2: 建立表格
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    id INTEGER PRIMARY KEY,
                    name TEXT NOT NULL,
                    salary INTEGER NOT NULL,
                    managerId INTEGER
                );";

                connection.Execute(createTableQuery);

                //// Step 3: 插入資料
                //string insertQuery = @"
                //INSERT INTO Employees (id, name, salary, managerId) VALUES
                //(1, 'Joe', 70000, 3),
                //(2, 'Henry', 80000, 4),
                //(3, 'Sam', 60000, NULL),
                //(4, 'Max', 90000, NULL);";

                //connection.Execute(insertQuery);

                //Console.WriteLine("資料已成功插入");

                //使⽤C#搭配Dapper，Query出⾮管理職(managerId不等於null)的⼈員
                string queryString = "SELECT * FROM Employees WHERE managerId IS NOT NULL";
                var ManagerIdIsAvailable = connection.Query(queryString).ToList();
                foreach (var employee in ManagerIdIsAvailable)
                {
                    Console.WriteLine($"ID: {employee.id}, Name: {employee.name}, Salary: {employee.salary}, ManagerId: {employee.managerId}");
                }

                //使⽤C#搭配Dapper，Query出⾮管理職且薪資⾼於該主管⼈員
                //string goodSalaryString = @"
                //        SELECT name
                //        FROM Employees e
                //        LEFT JOIN Employees m ON m.id = e.managerId
                //        WHERE e.managerId IS NOT NULL AND m.salary > e.salary";
                //var goodSalary = connection.Query(goodSalaryString).ToList();
                //foreach (var employee in goodSalary)
                //{
                //    Console.WriteLine($"Name: {employee.name}");
                //}

            }
        }
    }
}

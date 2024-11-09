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
            // using 區塊會自動處理資源釋放。可以保證連線在程式執行完成後關閉並釋放資源。
            //如果不使用 using 區塊，仍需要呼叫 connection.Close() 來關閉連線。
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open connection
                connection.Open();

                // Step 2: 建立表格
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                    id int PRIMARY KEY,
                    name varchar(50) NOT NULL,
                    salary int NOT NULL,
                    managerId int);";

                connection.Execute(createTableQuery);
                Console.WriteLine("資料表已成功建立。");

                // Step 3: 插入資料
                string insertQuery = @"
                INSERT OR IGNORE INTO  Employees (id, name, salary, managerId) VALUES
                (1, 'Joe', 70000, 3),
                (2, 'Henry', 80000, 4),
                (3, 'Sam', 60000, NULL),
                (4, 'Max', 90000, NULL);";

                connection.Execute(insertQuery);
                Console.WriteLine("資料已成功插入，重複執行將被忽略。");

                //使⽤C#搭配Dapper，Query出⾮管理職(managerId不等於null)的⼈員
                string queryString = "SELECT * FROM Employees WHERE managerId IS NOT NULL";
                List<Employee> managerIdIsAvailable = connection.Query<Employee>(queryString).ToList();
                Console.WriteLine("有主管(managerId)的員工：");
                foreach (Employee employee in managerIdIsAvailable)
                {
                    Console.WriteLine($"員編: {employee.Id}, 名字: {employee.Name}, " +
                        $"薪水: {employee.Salary}, 主管編號: {employee.ManagerId}");
                }

                //使⽤C#搭配Dapper，Query出⾮管理職且薪資⾼於該主管⼈員
                string goodSalaryString = @"
                        SELECT e.name
                        FROM Employees e
                        LEFT JOIN Employees m ON e.managerId = m.id
                        WHERE e.managerId IS NOT NULL
                        AND e.salary > m.salary";
                List<Employee> goodSalary = connection.Query<Employee>(goodSalaryString).ToList();
                Console.WriteLine("薪水高於主管的員工：");
                foreach (Employee employee in goodSalary)
                {
                    Console.WriteLine($"名字: {employee.Name}");
                    // 查詢語句僅寫e.name，Employee的其餘屬性(Id, Salary, ManagerId)將會是預設值（0 或 null）。
                }
            }
        }
    }
}

// Execute()：主要用來執行INSERT、UPDATE、DELETE 或者 CREATE 等操作。
// Query()：執行 SELECT 查詢並返回資料，返回查詢的結果通常是 IEnumerable<> 或 List<>。
//          IEnumerable<> 用於延遲加載(需要數據時加載)，透過 foreach 遍歷集合。
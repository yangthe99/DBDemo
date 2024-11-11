using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DBDemo
{
    /// <summary>
    /// 員工資料建立與查詢
    /// </summary>
    public class EmployeeService : IDisposable
    // EmployeeService 繼承IDisposable
    // 並實作Dispose()，Dispose() 在 using 內會被自動呼叫。
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        private readonly SQLiteConnection _Connection;

        /// <summary>
        /// EmployeeService建構子，取得連線字串並開啟連線
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeService(string connectionString)
        {
            _Connection = new SQLiteConnection(connectionString);
            _Connection.Open(); //自動開啟資料庫連線
        }

        /// <summary>
        /// 建立資料表
        /// </summary>
        public void CreateTable()
        {
            try
            {
                // IF NOT EXISTS：資料表在不存在的情況下被建立
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                id int PRIMARY KEY,
                name varchar(50) NOT NULL,
                salary int NOT NULL,
                managerId int);";

                _Connection.Execute(createTableQuery);
                //Console.WriteLine("資料表已成功建立(資料表若存在則忽略)。");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
            }
        }
        /// <summary>
        /// 寫入員工資料
        /// </summary>
        public void InsertEmployees()
        {
            try
            {
                string insertQuery = @"
                INSERT OR IGNORE INTO  Employees (id, name, salary, managerId) VALUES
                (1, 'Joe', 70000, 3),
                (2, 'Henry', 80000, 4),
                (3, 'Sam', 60000, NULL),
                (4, 'Max', 90000, NULL);";

                _Connection.Execute(insertQuery);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
            }
        }
        /// <summary>
        /// 取得⾮管理職(managerId不等於null)的⼈員
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetEmployeesWithManager()
        {
            try
            {
                // @""預防SQL Injection
                string queryString = @"
                SELECT * FROM Employees 
                WHERE managerId IS NOT NULL";
                return _Connection.Query<Employee>(queryString).ToList();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
                return new List<Employee>(); // 發生錯誤，回傳空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
                return new List<Employee>(); // 發生未知錯誤，回傳空列表
            }
        }
        /// <summary>
        /// 取得⾮管理職且薪資⾼於該主管的⼈員
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetEmployeesWithGoodSalary()
        {
            try
            {
                string queryString = @"
                SELECT e.name
                FROM Employees e
                LEFT JOIN Employees m ON e.managerId = m.id
                WHERE e.managerId IS NOT NULL
                AND e.salary > m.salary";
                return _Connection.Query<Employee>(queryString).ToList();
                // e join m, e的主管ID是m的ID, 找出含有以下條件的e
                // 1: e表ID有主管的人
                // 2: e表ID的薪水比主管(m表ID)的薪水多

                // 查詢語句如果僅寫e.name，Employee的其餘屬性(Id, Salary, ManagerId)將會是預設值（0 或 null）。
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
                return new List<Employee>(); // 發生錯誤，回傳空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
                return new List<Employee>(); // 發生未知錯誤，回傳空列表
            }
        }
        /// <summary>
        /// 自動關閉連線
        /// </summary>
        public void Dispose()
        {
            _Connection.Close();  // 在Service結束時關閉連線
            _Connection.Dispose(); // 釋放資源
        }
    }
}
// Execute()：主要用來執行INSERT、UPDATE、DELETE 或者 CREATE 等操作。
// Query()：執行 SELECT 查詢並返回資料，Dapper 會返回一個 IEnumerable<Employee> 類型的集合。
//          IEnumerable<> 用於延遲加載(需要數據時加載)，透過 foreach 遍歷集合。
// .ToList()：將查詢結果轉換成 List<> 。
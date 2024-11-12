using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        /// 儲存Datatable用
        /// </summary>
        private DataTable _Employee;

        /// <summary>
        /// EmployeeService建構子，取得連線字串並開啟連線
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeService(string connectionString)
        {
            _Connection = new SQLiteConnection(connectionString);
            _Connection.Open(); //自動開啟資料庫連線
            _Employee = new DataTable();
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
        /// (測試用)取得所有員工
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllEmployee() {
            string queryString = @"
                SELECT * FROM Employees";
            var employees = _Connection.Query<Employee>(queryString).ToList();
            // 將 List<Employee> 轉換為 DataTable
            return ConvertToDataTable(employees); // 使用 ConvertToDataTable 方法將 List 轉換成 DataTable
        }
        /// <summary>
        /// 取得⾮管理職(managerId不等於null)的⼈員
        /// </summary>
        /// <returns></returns>
        public DataTable GetEmployeesWithManager()
        {
            try
            {
                // @""預防SQL Injection
                string queryString = @"
                SELECT * FROM Employees 
                WHERE managerId IS NOT NULL";

                var employees = _Connection.Query<Employee>(queryString).ToList();
                // 將 List<Employee> 轉換為 DataTable
                return ConvertToDataTable(employees); // 使用 ConvertToDataTable 方法將 List 轉換成 DataTable
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
                return new DataTable(); // 發生錯誤，回傳空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
                return new DataTable(); // 發生未知錯誤，回傳空列表
            }
        }
        /// <summary>
        /// 取得⾮管理職且薪資⾼於該主管的⼈員
        /// </summary>
        /// <returns></returns>
        public DataTable GetEmployeesWithGoodSalary()
        {
            try
            {
                string queryString = @"
                SELECT e.name
                FROM Employees e
                LEFT JOIN Employees m ON e.managerId = m.id
                WHERE e.managerId IS NOT NULL
                AND e.salary > m.salary";
                var employees = _Connection.Query<Employee>(queryString).ToList();
                // 將 List<Employee> 轉換為 DataTable
                return ConvertToDataTable(employees); // 使用 ConvertToDataTable 方法將 List 轉換成 DataTable

                // e join m, e的主管ID是m的ID, 找出含有以下條件的e
                // 1: e表ID有主管的人
                // 2: e表ID的薪水比主管(m表ID)的薪水多

                // 查詢語句如果僅寫e.name，Employee的其餘屬性(Id, Salary, ManagerId)將會是預設值（0 或 null）。
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"資料庫錯誤：{ex.Message}");
                return new DataTable(); // 發生錯誤，回傳空列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未知錯誤：{ex.Message}");
                return new DataTable(); // 發生未知錯誤，回傳空列表
            }
        }
        /// <summary>
        /// 自動關閉連線
        /// </summary>
        public void Dispose()
        {
            //_Connection.Close();  // 在Service結束時關閉連線
            _Connection.Dispose(); // 釋放資源
        }

        /// <summary>
        /// 將 List<Employee> 轉換為 DataTable
        /// </summary>
        /// <param name="employees">員工列表</param>
        /// <returns>DataTable</returns>
        public DataTable ConvertToDataTable(List<Employee> employees)
            // 手動映射：簡單直接，性能優越，適合已知類型結構的情況。
        {
            DataTable dataTable = new DataTable();


            // 手動添加列
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Salary", typeof(int));
            dataTable.Columns.Add("ManagerId", typeof(int));

            // 填充資料列
            foreach (var employee in employees)
            {
                var row = dataTable.NewRow();

                row["Id"] = employee.Id;
                row["Name"] = employee.Name;
                row["Salary"] = employee.Salary;
                // ManagerId有值的話取值，否則是空值。
                // DataTable 需要 object 類型來存儲欄位資料，資料型別與目標欄位型別不匹配（例如，int? 轉換為 object）時須轉型。
                row["ManagerId"] = employee.ManagerId.HasValue ? (object)employee.ManagerId.Value : DBNull.Value;
              
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        // 使用反射：最靈活，但會影響性能，適合結構動態的情況。
        #region
        //public DataTable ConvertToDataTable(List<Employee> employees)
        //{
        //    DataTable dataTable = new DataTable();

        //    // 檢查員工列表有資料，且至少有一筆資料
        //    if (employees != null && employees.Count > 0)
        //    {
        //        // typeof(Employee).GetProperties(): 使用反射（Reflection[註1]）來獲取 Employee 類型的所有屬性。
        //        // 會返回一個包含所有屬性的 properties 物件的陣列，包含像 Id、Name、ManagerId 等屬性。
        //        var properties = typeof(Employee).GetProperties();

        //        foreach (var property in properties)
        //        {
        //            // dataTable新增欄位(欄位名稱=屬性名稱, 欄位的資料類型=屬性資料的類型)。
        //            dataTable.Columns.Add(property.Name, property.PropertyType);
        //        }

        //        // 迭代 List<Employee>，並將每個員工的資料寫入到 DataTable 中
        //        // 每個員工物件將轉換為 DataTable 的一行（DataRow）
        //        foreach (var employee in employees)
        //        {
        //            // (為每個員工)創建一個新的資料列（DataRow）。
        //            // NewRow() 會創建一個空的資料列，資料列欄位結構與前面 DataTable 中定義的欄位結構相同。
        //            var row = dataTable.NewRow();

        //            // 將屬性對應的值寫入到row
        //            foreach (var property in properties)
        //            {
        //                // row的欄位名稱=employee對應名稱的值
        //                // ?? DBNull.Value: 用來處理可能為 null 的屬性。
        //                // 如果該屬性值為 null，將 DBNull.Value 賦給資料列對應的欄位，以表示「空值」。
        //                row[property.Name] = property.GetValue(employee) ?? DBNull.Value;
        //            }
        //            // 將寫入好資料的 row 加入到 DataTable
        //            dataTable.Rows.Add(row);
        //        }
        //    }
        //    return dataTable;
        //}
        #endregion
    }
}
// Execute()：主要用來執行INSERT、UPDATE、DELETE 或者 CREATE 等操作。
// Query()：執行 SELECT 查詢並返回資料，Dapper 會返回一個 IEnumerable<Employee> 類型的集合。
//          IEnumerable<> 用於延遲加載(需要數據時加載)，透過 foreach 遍歷集合。
// .ToList()：將查詢結果轉換成 List<> 。

// 註1: 反射 (typeof(Employee).GetProperties())：
// 動態獲取 Employee 類的屬性，使得方法能夠處理不同屬性的 Employee 類。
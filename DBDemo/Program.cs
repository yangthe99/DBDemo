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
            string connectionString = "Data Source=DBDemo.db;Version=3;"; //連線字串

            using (EmployeeService employeeService = new EmployeeService(connectionString))
            //在 using 區塊內，Dispose() 會被自動呼叫。
            {
                employeeService.CreateTable(); //建立資料表
                employeeService.InsertEmployees(); //寫入員工資料

                // Query出⾮管理職(managerId不等於null)的⼈員
                var managerIdIsAvailable = employeeService.GetEmployeesWithManager();
                Console.WriteLine("【有主管(managerId)的員工】");
                foreach (var employee in managerIdIsAvailable)
                {
                    Console.WriteLine($"員工編號: {employee.Id}, 名字: {employee.Name}," +
                                      $"薪水: {employee.Salary}, 主管編號: {employee.ManagerId}");
                }

                Console.WriteLine();

                // Query出⾮管理職且薪資⾼於該主管⼈員
                var employeesWithGoodSalary = employeeService.GetEmployeesWithGoodSalary();
                Console.WriteLine("【薪水高於主管的員工】");
                foreach (var employee in employeesWithGoodSalary)
                {
                    Console.WriteLine($"名字: {employee.Name}");
                }
            }
        }
    }
}
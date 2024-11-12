using System.Data;
using FastReport;
using FastReport.Export.PdfSimple;

namespace DBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string reportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyReport.frx");
            if (!File.Exists(reportFilePath))
            {
                Console.WriteLine("報表模板檔案未找到！");
                return;
            }
            try
            {

                // 載入報表檔案 (假設報表檔案是 MyReport.frx)
                Report report = new Report();
                report.Load(reportFilePath);

                // 假設這些是查詢出來的員工資料
                string connectionString = "Data Source=DBDemo.db;Version=3;"; // 連線字串
                using (EmployeeService employeeService = new EmployeeService(connectionString))
                {
                    // 測試用(AllEmployee)
                    #region
                    //DataTable empAll = employeeService.GetAllEmployee();
                    //Console.WriteLine($"總共有 {empAll.Rows.Count} 筆資料。");
                    //Console.WriteLine("【有主管(managerId)的員工】");
                    //foreach (DataRow employee in empAll.Rows)
                    //{
                    //    Console.WriteLine($"員工編號: {employee["id"]}, 名字: {employee["name"]}," +
                    //                      $"薪水: {employee["salary"]}, 主管編號: {employee["managerId"]}");
                    //}
                    //report.RegisterData(empAll, "empAll");
                    //report.GetDataSource("empAll").Enabled = true;
                    #endregion

                    DataTable employeesWithManagerTable = employeeService.GetEmployeesWithManager();
                    DataTable employeesWithGoodSalaryTable = employeeService.GetEmployeesWithGoodSalary();

                    // 檢查用
                    #region
                    Console.WriteLine($"總共有 {employeesWithManagerTable.Rows.Count} 筆資料。");
                    Console.WriteLine("【有主管(managerId)的員工】");
                    foreach (DataRow employee in employeesWithManagerTable.Rows)
                    {
                        Console.WriteLine($"員工編號: {employee["id"]}, 名字: {employee["name"]}," +
                                          $"薪水: {employee["salary"]}, 主管編號: {employee["managerId"]}");
                    }

                    Console.WriteLine($"總共有 {employeesWithGoodSalaryTable.Rows.Count} 筆資料。");
                    Console.WriteLine("【薪水高於主管的員工】");
                    foreach (DataRow employee in employeesWithGoodSalaryTable.Rows)
                    {
                        Console.WriteLine($"名字: {employee["name"]}");
                    }
                    #endregion

                    // 註冊資料源
                    report.RegisterData(employeesWithManagerTable, "employeesWithManager");
                    report.RegisterData(employeesWithGoodSalaryTable, "employeesWithGoodSalary");

                    // 啟用資料源
                    report.GetDataSource("employeesWithManager").Enabled = true;
                    report.GetDataSource("employeesWithGoodSalary").Enabled = true;
                }
                 ((DataBand)report.Report.FindObject("Data1")).DataSource = report.GetDataSource("employeesWithManager");
                 ((DataBand)report.Report.FindObject("Data2")).DataSource = report.GetDataSource("employeesWithGoodSalary");

                // 準備報表
                report.Prepare();

                // 設定 PDF 輸出路徑
                string outputDirectory = AppDomain.CurrentDomain.BaseDirectory; // 執行檔目錄
                string pdfFilePath = Path.Combine(outputDirectory, "EmployeeReport.pdf");

                // 創建 PDF 匯出器
                PDFSimpleExport pdfExport = new PDFSimpleExport();

                // 將報表導出為 PDF
                report.Export(pdfExport, pdfFilePath);

                // 顯示報表已成功生成
                Console.WriteLine($"報表已成功生成並保存在: {pdfFilePath}");

                // 釋放報表資源
                report.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }
    }
}
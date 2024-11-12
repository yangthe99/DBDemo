using System.Data;
using FastReport;
using FastReport.Export.PdfSimple;

namespace DBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // reportFilePath=應用程式執行的根目錄\MyReport.frx
            // Path.Combine：將多個路徑組合成一個單一的路徑
            string reportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyReport.frx");
           
            // 檢查檔案存在
            if (!File.Exists(reportFilePath))
            {
                Console.WriteLine("報表模板檔案未找到！");
                return;
            }

            try
            {
                Report report = new Report();
                report.Load(reportFilePath); // 載入報表檔案

                string connectionString = "Data Source=DBDemo.db;Version=3;"; // 連線字串
                using (EmployeeService employeeService = new EmployeeService(connectionString))
                {
                    employeeService.CreateTable(); //建立資料表
                    employeeService.InsertEmployees(); //寫入員工資料

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

                    // 取得有主管(managerId)的員工
                    DataTable employeesWithManagerTable = employeeService.GetEmployeesWithManager();
                    // 取得薪水高於主管的員工
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

                    // 註冊資料源RegisterData(資料源DataSource, 資料名稱DataName)
                    report.RegisterData(employeesWithManagerTable, "employeesWithManager");
                    report.RegisterData(employeesWithGoodSalaryTable, "employeesWithGoodSalary");

                    // 啟用資料源
                    // GetDataSource(資料名稱DataName).啟用狀態
                    report.GetDataSource("employeesWithManager").Enabled = true;
                    report.GetDataSource("employeesWithGoodSalary").Enabled = true;
                }

                // 資料帶與資料源的綁定
                // ((DataBand): 轉換為 DataBand 類型
                // report.Report: 表示報表的結構。例如資料帶、文字、圖片等都可以從 report.Report 來訪問。
                // FindObject("")): 查找報表中名為 "" 的物件
                // .DataSource: 資料源
                 ((DataBand)report.Report.FindObject("Data1")).DataSource = report.GetDataSource("employeesWithManager");
                 ((DataBand)report.Report.FindObject("Data2")).DataSource = report.GetDataSource("employeesWithGoodSalary");

                // 準備報表，處理報表中的資料綁定、格式設置等
                report.Prepare();

                string outputDirectory = @"C:\DBDemo"; // 設定 PDF 輸出路徑
                if (!Directory.Exists(outputDirectory)) // 檢查路徑資料夾是否存在
                    Directory.CreateDirectory(outputDirectory); // 資料夾不存在的話新增
                // C:\DBDemo\EmployeeReport.pdf
                string pdfFilePath = Path.Combine(outputDirectory, "EmployeeReport.pdf");

                // 創建 PDF 匯出器
                PDFSimpleExport pdfExport = new PDFSimpleExport();

                // 將報表內容匯出為 PDF，並保存到指定的 pdfFilePath 路徑
                report.Export(pdfExport, pdfFilePath); 
                Console.WriteLine($"報表已成功生成並保存在: {pdfFilePath}");

                report.Dispose();  // 釋放報表資源
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }
    }
}
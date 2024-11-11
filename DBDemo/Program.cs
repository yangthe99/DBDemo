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
                DataSet dataSet = new DataSet();

                #region
                //// 載入報表檔案 (假設報表檔案是 MyReport.frx)
                //Report report = new Report();
                //report.Load(reportFilePath);

                //// 假設這些是查詢出來的員工資料
                //string connectionString = "Data Source=DBDemo.db;Version=3;"; //連線字串
                //using (EmployeeService employeeService = new EmployeeService(connectionString))
                //{
                //    // 獲取有經理的員工資料
                //    var employeesWithManager = employeeService.GetEmployeesWithManager();

                //    // 獲取高薪的員工資料
                //    var employeesWithGoodSalary = employeeService.GetEmployeesWithGoodSalary();

                //    // 將資料轉換為 DataTable
                //    //DataTable employeesDataTable = employeesWithManager.CopyToDataTable();
                //    //DataTable goodSalaryDataTable = employeesWithGoodSalary.CopyToDataTable();

                //    // 註冊資料源到報表中
                //    report.RegisterData(employeesWithManager, "Employees"); // 註冊有經理的員工
                //    report.RegisterData(employeesWithGoodSalary, "EmployeesWithGoodSalary"); // 註冊高薪員工

                //    // 啟用資料源
                //    report.GetDataSource("Employees").Enabled = true;
                //    report.GetDataSource("EmployeesWithGoodSalary").Enabled = true;
                //}
                //report.Prepare();
                //// 設定 PDF 輸出路徑
                //string outputDirectory = AppDomain.CurrentDomain.BaseDirectory; // 執行檔目錄
                //string pdfFilePath = Path.Combine(outputDirectory, "EmployeeReport.pdf");

                //// 創建 PDF 匯出器
                //PDFSimpleExport pdfExport = new PDFSimpleExport();

                //// 將報表導出為 PDF
                //report.Export(pdfExport, pdfFilePath);

                //// 顯示報表已成功生成
                //Console.WriteLine($"報表已成功生成並保存在: {pdfFilePath}");

                //// 釋放報表資源
                //report.Dispose();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }
    }
}
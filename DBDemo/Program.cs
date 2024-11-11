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
                string connectionString = "Data Source=DBDemo.db;Version=3;"; //連線字串
                using (EmployeeService employeeService = new EmployeeService(connectionString))
                {
                    // 使用員工資料來註冊資料源，將資料傳遞給報表
                    var employeesWithManager = employeeService.GetEmployeesWithManager();
                    var employeesWithGoodSalary = employeeService.GetEmployeesWithGoodSalary();

                    // 假設您有兩個資料源可以傳遞給報表
                    report.RegisterData(employeesWithManager, "EmployeesWithManager");
                    report.RegisterData(employeesWithGoodSalary, "EmployeesWithGoodSalary");
                }

                // 設定 PDF 輸出路徑
                string outputDirectory = AppDomain.CurrentDomain.BaseDirectory; // 執行檔目錄
                string pdfFilePath = Path.Combine(outputDirectory, "EmployeeReport.pdf");

                // 創建 PDF 匯出器
                PDFSimpleExport pdfExport = new PDFSimpleExport();

                // 將報表導出為 PDF
                report.Export(pdfExport, pdfFilePath);

                // 顯示報表已成功生成
                Console.WriteLine($"報表已成功生成並保存在: {pdfFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }
    }
}
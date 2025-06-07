using OfficeOpenXml;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ 起動時にExcelファイルをダウンロードして読み込む処理
await DownloadAndReadExcelFile();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


// ==========================
// ✅ 起動時の処理（関数）
// ==========================
async Task DownloadAndReadExcelFile()
{
    string fileId = "1gjR2THGxqF1z69p0EQW39IsOGCbGLTgS"; // ←差し替えてください
    string url = $"https://drive.google.com/uc?export=download&id={fileId}";
    string localPath = Path.Combine("/tmp", "items.xlsx");

    using (var client = new HttpClient())
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(localPath, data);

            Console.WriteLine("✅ Excel ファイルのダウンロード成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Excel ダウンロード失敗: {ex.Message}");
            return;
        }
    }

    try
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(new FileInfo(localPath));
        var sheet = package.Workbook.Worksheets[0];
        string value = sheet.Cells[1, 1].Text;

        Console.WriteLine($"📄 Excel読み込み成功（A1セル）: {value}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Excel読み込み失敗: {ex.Message}");
    }
}

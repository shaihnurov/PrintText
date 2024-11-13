using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PrintText.MVVM.ViewModel;

public class PrintViewModel : ObservableObject
{
    private string _textTag = string.Empty;
    private string _textInput = string.Empty;

    public string TextTag
    {
        get => _textTag;
        private set => SetProperty(ref _textTag, value);
    }
    public string TextInput
    {
        get => _textInput;
        set
        {
            if (SetProperty(ref _textInput, value))
            {
                TextTag = _textInput;
            }
        }
    }
    
    public AsyncRelayCommand PrintCommand { get; }

    public PrintViewModel()
    {
        PrintCommand = new AsyncRelayCommand(PrintDocument);
    }

    private async Task PrintDocument()
    {
        string fileName = $"{TextTag}.pdf";
        using var document = new PdfDocument();
        
        document.Info.Title = TextTag;
        var page = document.AddPage();
        var graphics = XGraphics.FromPdfPage(page);

        var font = new XFont("Verdana", 12, XFontStyleEx.Regular);
        var boldFont = new XFont("Verdana", 12, XFontStyleEx.Bold);
        
        int startX = 50, startY = 50;
        int rowHeight = 15;
        int numColWidth = 70, lastNameColWidth = 200, firstNameColWidth = 250;
        
        DrawCenteredText(graphics, TextInput, boldFont, page.Width.Point, 20);

        int currentY = startY;
        DrawTableHeader(graphics, startX, currentY, rowHeight, numColWidth, lastNameColWidth, firstNameColWidth, boldFont);

        var tableData = new[]
        {
            new { Number = "1", LastName = "Иванов", FirstName = "Иван" },
            new { Number = "2", LastName = "Сидоров", FirstName = "Сидор" },
            new { Number = "3", LastName = "Петров", FirstName = "Петр" }
        };

        foreach (var rowData in tableData)
        {
            currentY += rowHeight;
            DrawTableRow(graphics, startX, currentY, rowHeight, numColWidth, lastNameColWidth, firstNameColWidth, font, rowData);
        }

        DrawTableBorders(graphics, startX, startY, rowHeight, numColWidth, lastNameColWidth, firstNameColWidth, tableData.Length + 1);

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        await document.SaveAsync(filePath);

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
    private static void DrawCenteredText(XGraphics graphics, string text, XFont font, double pageWidth, double posY)
    {
        graphics.DrawString(text, font, XBrushes.Black, new XRect(0, posY, pageWidth, 30), XStringFormats.TopCenter);
    }
    private static void DrawTableHeader(XGraphics graphics, int startX, int startY, int rowHeight, int numColWidth, int lastNameColWidth, int firstNameColWidth, XFont font)
    {
        var headerBrush = new XSolidBrush(XColor.FromArgb(200, 200, 200));
        
        graphics.DrawRectangle(headerBrush, startX, startY, numColWidth, rowHeight);
        graphics.DrawRectangle(headerBrush, startX + numColWidth, startY, lastNameColWidth, rowHeight);
        graphics.DrawRectangle(headerBrush, startX + numColWidth + lastNameColWidth, startY, firstNameColWidth, rowHeight);

        graphics.DrawString("Номер", font, XBrushes.Black, new XRect(startX + 10, startY, numColWidth, rowHeight), XStringFormats.CenterLeft);
        graphics.DrawString("Фамилия", font, XBrushes.Black, new XRect(startX + 10 + numColWidth, startY, lastNameColWidth, rowHeight), XStringFormats.CenterLeft);
        graphics.DrawString("Имя", font, XBrushes.Black, new XRect(startX + 10 + numColWidth + lastNameColWidth, startY, firstNameColWidth, rowHeight), XStringFormats.CenterLeft);
    }
    private static void DrawTableRow(XGraphics graphics, int startX, int currentY, int rowHeight, int numColWidth, int lastNameColWidth, int firstNameColWidth, XFont font, dynamic rowData)
    {
        graphics.DrawString(rowData.Number, font, XBrushes.Black, new XRect(startX + 10, currentY, numColWidth, rowHeight), XStringFormats.CenterLeft);
        graphics.DrawString(rowData.LastName, font, XBrushes.Black, new XRect(startX + 10 + numColWidth, currentY, lastNameColWidth, rowHeight), XStringFormats.CenterLeft);
        graphics.DrawString(rowData.FirstName, font, XBrushes.Black, new XRect(startX + 10 + numColWidth + lastNameColWidth, currentY, firstNameColWidth, rowHeight), XStringFormats.CenterLeft);

        graphics.DrawLine(XPens.Black, startX, currentY + rowHeight, startX + numColWidth + lastNameColWidth + firstNameColWidth, currentY + rowHeight);
    }
    private static void DrawTableBorders(XGraphics graphics, int startX, int startY, int rowHeight, int numColWidth, int lastNameColWidth, int firstNameColWidth, int rowsCount)
    {
        graphics.DrawLine(XPens.Black, startX, startY, startX, startY + rowsCount * rowHeight);
        graphics.DrawLine(XPens.Black, startX + numColWidth, startY, startX + numColWidth, startY + rowsCount * rowHeight);
        graphics.DrawLine(XPens.Black, startX + numColWidth + lastNameColWidth, startY, startX + numColWidth + lastNameColWidth, startY + rowsCount * rowHeight);
        graphics.DrawLine(XPens.Black, startX + numColWidth + lastNameColWidth + firstNameColWidth, startY, startX + numColWidth + lastNameColWidth + firstNameColWidth, startY + rowsCount * rowHeight);

        graphics.DrawLine(XPens.Black, startX, startY + rowHeight, startX + numColWidth + lastNameColWidth + firstNameColWidth, startY + rowHeight);
    }
}
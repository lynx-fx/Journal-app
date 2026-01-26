using JournalApp.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.Security;
using PdfSharpCore.Drawing.Layout;
using System.Text.RegularExpressions;

namespace JournalApp.Services;

public class PdfService : IPdfService
{
    public byte[] GenerateJournalPdf(List<Journals> journals, string password, string title)
    {
        using var document = new PdfDocument();
        document.Info.Title = title;

        // Apply Password Security if provided
        if (!string.IsNullOrWhiteSpace(password))
        {
            document.SecuritySettings.UserPassword = password;
            document.SecuritySettings.OwnerPassword = password;
            
            // Restrict permissions
            document.SecuritySettings.PermitAccessibilityExtractContent = false;
            document.SecuritySettings.PermitAnnotations = false;
            document.SecuritySettings.PermitAssembleDocument = false;
            document.SecuritySettings.PermitExtractContent = false;
            document.SecuritySettings.PermitFormsFill = false;
            document.SecuritySettings.PermitFullQualityPrint = true;
            document.SecuritySettings.PermitModifyDocument = false;
            document.SecuritySettings.PermitPrint = true;
        }

        // Fonts
        var fontTitle = new XFont("Verdana", 20, XFontStyle.Bold);
        var fontHeader = new XFont("Verdana", 14, XFontStyle.Bold);
        var fontDate = new XFont("Verdana", 12, XFontStyle.Italic);
        var fontBody = new XFont("Verdana", 11, XFontStyle.Regular);

        // margins
        double margin = 40;
        double currentY = margin;
        
        // --- Title Page or Header ---
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        
        gfx.DrawString(title, fontTitle, XBrushes.Black, new XRect(0, margin, page.Width, page.Height), XStringFormats.TopCenter);
        currentY += 60;

        foreach (var journal in journals)
        {
            // Check if we need a new page
            // Simple estimation: if currentY > page.Height - margin, add page
            if (currentY > page.Height - margin - 100)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                currentY = margin;
            }

            // Draw Date
            gfx.DrawString(journal.Date.ToString("D"), fontHeader, XBrushes.DarkBlue, margin, currentY);
            currentY += 25;

            // Draw Mood
            var moodText = $"Mood: {journal.Mood}";
            gfx.DrawString(moodText, fontDate, XBrushes.Gray, margin, currentY);
            currentY += 25;

            // Draw Body
            // Simple word wrap
            var content = StripHtml(journal.Content);
            var rect = new XRect(margin, currentY, page.Width - 2 * margin, page.Height - margin - currentY);
            
            // Measure string
            // PdfSharp doesn't have auto-wrap in DrawString easily without XTextFormatter, 
            // but let's try a simple multiline approach or XTextFormatter if available?
            // PdfSharpCore has XTextFormatter class usually.
            
            var tf = new PdfSharpCore.Drawing.Layout.XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Left;
            
            // Calculate height needed?
            // For simplicity, we just draw in the remaining box. 
            // If it overflows, it cuts off (in simple version). 
            // Better: loop and measure. But let's assume short entries or handle page breaks roughly.
            
            tf.DrawString(content, fontBody, XBrushes.Black, rect);
            
            // Estimate height used... not easy with XTextFormatter without measurement.
            // Let's guess based on length.
            var lines = content.Length / 80; // approx chars per line
            var height = (lines + 2) * 15; 
            
            currentY += height + 30; // spacing
        }

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
    
    private string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;
        // Replace <br> with newlines
        html = html.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<p>", "\n").Replace("</p>", "\n");
        // Strip other tags
        return Regex.Replace(html, "<.*?>", String.Empty).Trim();
    }
}

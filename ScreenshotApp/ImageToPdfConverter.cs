using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenshotApp
{
    public class ImageToPdfConverter
    {
        [Obsolete]
        public void ConvertImagesToPdf(List<string> imagePaths, string outputPdfPath)
        {
            if (imagePaths == null || imagePaths.Count == 0)
            {
                throw new ArgumentException("Image paths cannot be null or empty.");
            }

            using PdfDocument document = new();
            foreach (var imagePath in imagePaths)
            {
                if (File.Exists(imagePath))
                {
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XImage image = XImage.FromFile(imagePath);


                    page.Width = image.PixelWidth * 72 / image.HorizontalResolution;
                    page.Height = image.PixelHeight * 72 / image.VerticalResolution;

                    gfx.DrawImage(image, 0, 0, page.Width, page.Height);
                }
                else
                {
                    Console.WriteLine($"File not found: {imagePath}");
                }
            }

            document.Save(outputPdfPath);
            Console.WriteLine($"PDF saved at: {outputPdfPath}");
        }
    }
}

using ScreenshotApp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

class Program
{
    [Obsolete]
    static void Main(string[] args)
    {
        Console.WriteLine("Capturing 5 screenshots at 3-second intervals...");

        // List to store the paths of the captured images
        List<string> imagePaths = [];

        for (int i = 0; i < 6; i++)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string filename = $"screenshot_{timestamp}.png";

            // Append the working directory to the filename
            string fullPath = System.IO.Path.Combine(Environment.CurrentDirectory, filename);

            CaptureScreen(fullPath);
            Console.WriteLine($"Screenshot {i} saved at: {fullPath}");

            imagePaths.Add(fullPath);

            Thread.Sleep(500);
        }

        Console.WriteLine("Finished capturing screenshots.");


        // Convert the images to PDF
        string pdfFilename = $"screenshots_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        _ = System.IO.Path.Combine(Environment.CurrentDirectory, pdfFilename);

        ImageToPdfConverter converter = new();
        converter.ConvertImagesToPdf(imagePaths, pdfFilename);
    }

    static void CaptureScreen(string filename)
    {
        // Get the dimensions of the primary screen
        int screenX = GetSystemMetrics(0);
        int screenY = GetSystemMetrics(1);

        // Create a new bitmap to store the screenshot
        using Bitmap bmp = new(screenX, screenY);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            // Use the BitBlt API to capture the screen
            IntPtr hdcSrc = GetDC(IntPtr.Zero);
            IntPtr hdcDest = g.GetHdc();

            _ = BitBlt(hdcDest, 0, 0, screenX, screenY, hdcSrc, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

            g.ReleaseHdc(hdcDest);
            ReleaseDC(IntPtr.Zero, hdcSrc);
        }

        // Save the screenshot
        bmp.Save(filename, ImageFormat.Png);
    }

    // P/Invoke declarations
    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, CopyPixelOperation rop);

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
}

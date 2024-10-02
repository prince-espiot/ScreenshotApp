using ScreenshotApp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

class Program
{
    [Obsolete]
    static void Main(string[] args)
    {
        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Capture the entire screen");
        Console.WriteLine("2. Select an area to capture");
        Console.Write("Enter your choice (1 or 2): ");


        string choice = Console.ReadLine();

        

        //string pdfFilename = $"screenshots_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        //var fullPath = System.IO.Path.Combine(Environment.CurrentDirectory, pdfFilename);
        ImageToPdfConverter converter = new ImageToPdfConverter();
        switch (choice)
        {
            // List to store the paths of the captured images
            case "1":
                Console.WriteLine("Capturing 5 screenshots at 3-second intervals...");
                List<string> imagePaths = new List<string>(); // Initialize the list to hold image paths

                for (int i = 1; i <= 5; i++)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                    string filename = $"screenshot_{timestamp}.png";

                    // Append the working directory to the filename
                    string fullPath = System.IO.Path.Combine(Environment.CurrentDirectory, filename);

                    CaptureFullScreen(fullPath); // Capture the screenshot and save it to fullPath
                    Console.WriteLine($"Screenshot {i} done");

                    imagePaths.Add(fullPath); // Add the path to the list

                    Thread.Sleep(3000); // Sleep for 3 seconds
                }

                Console.WriteLine("Finished capturing screenshots.");
                Console.WriteLine("Screenshots saved at:");
                foreach (var path in imagePaths)
                {
                    Console.WriteLine(path);
                }

                // Assuming you have a converter instance of ImageToPdfConverter
                string pdfFilename = $"Screenshots_{DateTime.Now:yyyyMMdd_HHmmssfff}.pdf"; // Set PDF filename
                converter.ConvertImagesToPdf(imagePaths, pdfFilename); // Convert the images to PDF

                break;

            case "2":
                ScreenSelector selector = new ScreenSelector();
                selector.StartSelection();

                // Wait until the selection is complete
                Thread.Sleep(2000); // Adjust time as necessary to ensure selection is captured

                Rectangle selectedArea = selector.SelectionRectangle;

                // If no selection was made, exit the program
                if (selectedArea.Width == 0 || selectedArea.Height == 0)
                {
                    Console.WriteLine("No area selected. Exiting...");
                    return;
                }

                // Create a timestamped filename for the selected area screenshot
                string selectedFullPath = $"screenshot_selected_{DateTime.Now:yyyyMMdd_HHmmssfff}.png";
                string selectedImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, selectedFullPath);

                CaptureScreen(selectedImagePath, selectedArea); // Capture the specified area of the screen
                Console.WriteLine($"Screenshot saved at: {selectedImagePath}");
                break;

            default:
                Console.WriteLine("Invalid choice. Exiting...");
                break;
        }


    }

    static void CaptureFullScreen(string filename)
    {
        // Get the dimensions of the primary screen
        int screenX = GetSystemMetrics(0);
        int screenY = GetSystemMetrics(1);

        // Create a new bitmap to store the screenshot
        using (Bitmap bmp = new Bitmap(screenX, screenY))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Use the BitBlt API to capture the screen
                IntPtr hdcSrc = GetDC(IntPtr.Zero);
                IntPtr hdcDest = g.GetHdc();

                BitBlt(hdcDest, 0, 0, screenX, screenY, hdcSrc, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

                g.ReleaseHdc(hdcDest);
                ReleaseDC(IntPtr.Zero, hdcSrc);
            }

            // Save the screenshot
            bmp.Save(filename, ImageFormat.Png);
        }
    }

    static void CaptureScreen(string filename, Rectangle area)
    {
        // Create a new bitmap to store the screenshot
        using (Bitmap bmp = new Bitmap(area.Width, area.Height))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Use the BitBlt API to capture the screen
                IntPtr hdcSrc = GetDC(IntPtr.Zero);
                IntPtr hdcDest = g.GetHdc();

                BitBlt(hdcDest, 0, 0, area.Width, area.Height, hdcSrc, area.X, area.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

                g.ReleaseHdc(hdcDest);
                ReleaseDC(IntPtr.Zero, hdcSrc);
            }

            // Save the screenshot
            bmp.Save(filename, ImageFormat.Png);
        }
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

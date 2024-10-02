using System.Drawing;
using System.Runtime.InteropServices;

public class ScreenSelector
{
    private Point startPoint;
    private Rectangle selectionRectangle;
    private bool selecting;

    public Rectangle SelectionRectangle => selectionRectangle;

    public void StartSelection()
    {
        selecting = true;
        Console.WriteLine("Press any key to start selection. Click and drag to select the area, then release the mouse button to capture the screenshot.");

        // Wait for the user to press a key to begin
        Console.ReadKey(true);

        Task.Run(() => CaptureSelection());
    }

    private void CaptureSelection()
    {
        while (selecting)
        {
            // Check if the left mouse button is pressed
            if (GetAsyncKeyState(0x01) != 0) // 0x01 is the left mouse button
            {
                // Get the current mouse position
                var currentMousePosition = GetMousePosition();

                if (!selectionRectangle.IsEmpty)
                {
                    selectionRectangle = new Rectangle(
                        Math.Min(startPoint.X, currentMousePosition.X),
                        Math.Min(startPoint.Y, currentMousePosition.Y),
                        Math.Abs(startPoint.X - currentMousePosition.X),
                        Math.Abs(startPoint.Y - currentMousePosition.Y)
                    );

                    Console.SetCursorPosition(0, 1); // Move cursor down for visual feedback
                    Console.WriteLine($"Selecting area: {selectionRectangle}");
                }
                else
                {
                    // Start the selection
                    startPoint = currentMousePosition;
                }
            }
            else
            {
                // Mouse button released
                if (!selectionRectangle.IsEmpty)
                {
                    selecting = false;
                    Console.WriteLine($"Selected area: {selectionRectangle}");
                }
            }

            Thread.Sleep(10); // Prevent high CPU usage
        }
    }

    private Point GetMousePosition()
    {
        // Get mouse position
        GetCursorPos(out POINT point);
        return new Point(point.X, point.Y);
    }

    // Struct for mouse position
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    // P/Invoke for getting mouse position
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    // P/Invoke for checking key states
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}

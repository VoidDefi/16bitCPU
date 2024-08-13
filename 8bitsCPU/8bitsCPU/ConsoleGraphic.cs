using System.Runtime.InteropServices;

public class ConsoleGraphics : IDisposable
{
    // Import necessary WinAPI functions
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("gdi32.dll")]
    private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateSolidBrush(uint color);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll", EntryPoint = "FillRect", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    private static extern bool FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private IntPtr _hWnd;
    private IntPtr _hDC;
    private bool _disposed = false;
    public ConsoleGraphics()
    {
        try
        {
            _hWnd = GetConsoleWindow();
            if (_hWnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Unable to get console window handle.");
            }

            _hDC = GetDC(_hWnd);
            if (_hDC == IntPtr.Zero)
            {
                throw new InvalidOperationException("Unable to get device context.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ConsoleGraphics constructor: {ex.Message}");
            throw;
        }
    }

    public void DrawRectangle(int x, int y, int width, int height, uint color)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ConsoleGraphics));
        }

        IntPtr hBrush = CreateSolidBrush(color);
        if (hBrush == IntPtr.Zero)
        {
            throw new InvalidOperationException("Unable to create solid brush.");
        }

        try
        {
            RECT rect = new RECT
            {
                Left = x,
                Top = y,
                Right = x + width,
                Bottom = y + height
            };

            if (!FillRect(_hDC, ref rect, hBrush))
            {
                throw new InvalidOperationException("Failed to fill rectangle.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DrawRectangle: {ex.Message}");
        }
        finally
        {
            if (hBrush != IntPtr.Zero)
            {
                DeleteObject(hBrush);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_hDC != IntPtr.Zero)
                {
                    ReleaseDC(_hWnd, _hDC);
                    _hDC = IntPtr.Zero;
                }
            }

            _disposed = true;
        }
    }

    ~ConsoleGraphics()
    {
        Dispose(false);
    }
}
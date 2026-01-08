using Microsoft.Maui.Graphics;

namespace kando_desktop.Controls
{
    public class GridBackgroundDrawable : IDrawable
    {
        public bool IsDarkTheme { get; set; } = true;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            if (IsDarkTheme)
            {
                canvas.StrokeColor = Color.FromArgb("#0DFFFFFF");
            }
            else
            {
                canvas.StrokeColor = Color.FromArgb("#1A000000");
            }

            canvas.StrokeSize = 1;
            float gridSize = 40f;

            for (float x = 0; x < dirtyRect.Width; x += gridSize)
                canvas.DrawLine(x, 0, x, dirtyRect.Height);

            for (float y = 0; y < dirtyRect.Height; y += gridSize)
                canvas.DrawLine(0, y, dirtyRect.Width, y);

            canvas.RestoreState();
        }
    }
}
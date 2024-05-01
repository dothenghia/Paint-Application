
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace Line_
{
    public class MyLine : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Line"; // Name of the shape
        public string Icon => "Assets/line.png"; // Path to the icon
        public double Thickness { get; set; } = 1;
        public DoubleCollection StrokeDash { get; set; } = new DoubleCollection();
        public SolidColorBrush Brush { get; set; } = Brushes.Black;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color

        // Clone the object
        public object Clone()
        {
            return MemberwiseClone();
        }

        // Convert the object to a UIElement - Draw the shape
        public Canvas Convert()
        {
            // Calculate canvas position and size
            double canvasLeft = Math.Min(startPoint.X, endPoint.X);
            double canvasTop = Math.Min(startPoint.Y, endPoint.Y);
            double canvasWidth = Math.Abs(endPoint.X - startPoint.X);
            double canvasHeight = Math.Abs(endPoint.Y - startPoint.Y);

            // Set canvas position and size
            Canvas frameCanvas = new Canvas();
            Canvas.SetLeft(frameCanvas, canvasLeft);
            Canvas.SetTop(frameCanvas, canvasTop);
            frameCanvas.Width = canvasWidth;
            frameCanvas.Height = canvasHeight;

            // Create a new Line
            Line line = new Line()
            {
                X1 = startPoint.X - canvasLeft,
                Y1 = startPoint.Y - canvasTop,
                X2 = endPoint.X - canvasLeft,
                Y2 = endPoint.Y - canvasTop,
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(line);
            return frameCanvas;
        }

    }
}

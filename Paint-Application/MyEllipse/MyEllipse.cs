
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace Ellipse_
{
    public class MyEllipse : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Ellipse"; // Name of the shape
        public string Icon => "Assets/ellipse.png"; // Path to the icon

        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color
        public double Thickness { get; set; } = 3;
        public DoubleCollection StrokeDash { get; set; } = new DoubleCollection();
        public SolidColorBrush Brush { get; set; } = Brushes.Black;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }

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

            Ellipse ellipse = new Ellipse()
            {
                Width = canvasWidth,
                Height = canvasHeight,
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(ellipse);
            return frameCanvas;
        }

    }

}


using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace Triangle
{
    public class MyTriangle : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Triangle"; // Name of the shape
        public string Icon => "Assets/triangle.png"; // Path to the icon

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

            Polygon triangle = new Polygon();

            triangle.Points = new PointCollection()
            {
                new Point(startPoint.X - canvasLeft, endPoint.Y - canvasTop),
                new Point((startPoint.X - canvasLeft + endPoint.X - canvasLeft) / 2, startPoint.Y - canvasTop),
                new Point(endPoint.X - canvasLeft, endPoint.Y - canvasTop)
            };


            triangle.Fill = fillColor;
            triangle.Stroke = Brush;
            triangle.StrokeThickness = Thickness;
            triangle.StrokeDashArray = StrokeDash;

            frameCanvas.Children.Add(triangle);
            return frameCanvas;
        }

    }

}

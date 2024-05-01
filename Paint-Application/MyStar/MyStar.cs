
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace Star
{
    public class MyStar : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Star"; // Name of the shape
        public string Icon => "Assets/star.png"; // Path to the icon
        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color
        public double Thickness { get; set; } = 1;
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

            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;
            double radius = Math.Min(canvasWidth, canvasHeight) / 2;

            Point[] points = new Point[10];
            double angleIncrement = 2 * Math.PI / 10; // 5 points in total, each point separated by 2 * Math.PI / 5 radians
            double currentAngle = -Math.PI / 2; // Start from the top point of the star
            for (int i = 0; i < 10; i++)
            {
                double x = centerX + (radius * Math.Cos(currentAngle) * canvasWidth / canvasHeight);
                double y = centerY + (radius * Math.Sin(currentAngle) * canvasHeight / canvasWidth);
                points[i] = new Point(x, y);
                currentAngle += angleIncrement;
                x = centerX + (radius / 2.5 * Math.Cos(currentAngle) * canvasWidth / canvasHeight);
                y = centerY + (radius / 2.5 * Math.Sin(currentAngle) * canvasHeight / canvasWidth);
                points[++i] = new Point(x, y);
                currentAngle += angleIncrement;
            }

            // Create a Polygon with the calculated points
            Polygon starPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(starPolygon);
            return frameCanvas;
        }

    }

}

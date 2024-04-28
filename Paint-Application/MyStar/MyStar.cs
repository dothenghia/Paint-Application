
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace MyStar
{
    public class MyStar : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Star"; // Name of the shape
        public string Icon => "Assets/star.png"; // Path to the icon
        private Point startPoint; // Start point of the shape
        private Point endPoint; // End point of the shape

        private SolidColorBrush strokeColor = Brushes.Black; // Stroke color
        private SolidColorBrush fillColor = Brushes.White; // Fil color
        private double strokeThickness = 1; // Stroke thickness
        private DoubleCollection strokeDashArray = new DoubleCollection(); // Stroke dash array

        // ==================== Methods ====================
        public void SetStartPoint(Point point)
        {
            startPoint = point;
        }
        public void SetEndPoint(Point point)
        {
            endPoint = point;
        }
        public void SetStrokeColor(SolidColorBrush color)
        {
            strokeColor = color;
        }
        public void SetStrokeThickness(double thickness)
        {
            strokeThickness = thickness;
        }
        public void SetStrokeDashArray(DoubleCollection dashArray)
        {
            strokeDashArray = dashArray;
        }

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
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                StrokeDashArray = strokeDashArray,
                Fill = fillColor
            };

            frameCanvas.Children.Add(starPolygon);
            return frameCanvas;
        }



    }

}

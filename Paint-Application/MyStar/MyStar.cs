
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

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
        public UIElement Convert()
        {
            double width = endPoint.X - startPoint.X;
            double height = endPoint.Y - startPoint.Y;

            Point[] points = new Point[5];
            double cx = width / 2;
            double cy = height / 2;
            double theta = -Math.PI / 2;
            double dtheta = Math.PI * 0.8;

            for (int i = 0; i < 5; i++)
            {
                points[i].X = startPoint.X + cx + cx * Math.Cos(theta);
                points[i].Y = startPoint.Y + cy + cy * Math.Sin(theta);
                theta += dtheta;
            }

            // Create a Polygon with the calculated points
            Polygon starPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                StrokeDashArray = strokeDashArray
            };

            return starPolygon;
        }
    }

}

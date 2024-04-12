
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace MyTriangle
{
    public class MyTriangle : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Triangle"; // Name of the shape
        public string Icon => "Assets/triangle.png"; // Path to the icon
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
            Polygon triangle = new Polygon();

            triangle.Points = new PointCollection()
            {
                new Point(startPoint.X, endPoint.Y),
                new Point((startPoint.X + endPoint.X) / 2, startPoint.Y),
                new Point(endPoint.X, endPoint.Y)
            };

            triangle.Stroke = strokeColor;
            triangle.StrokeThickness = strokeThickness;
            triangle.StrokeDashArray = strokeDashArray;

            return triangle;
        }
    }

}

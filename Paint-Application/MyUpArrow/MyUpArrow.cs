
using MyShapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace MyUpArrow
{
    public class MyUpArrow : IShape
    {
        // ==================== Attributes ====================
        public string Name => "UpArrow"; // Name of the shape
        public string Icon => "Assets/up-arrow.png"; // Path to the icon
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
            Point[] points = new Point[]
            {
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.375, endPoint.Y),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.625, endPoint.Y),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.625, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.75, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.5, startPoint.Y),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.25, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25),
                new Point(startPoint.X + (endPoint.X - startPoint.X) * 0.375, startPoint.Y + (endPoint.Y - startPoint.Y) * 0.25)
            };

            // Create a Polygon with the calculated points
            Polygon upArrowPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                StrokeDashArray = strokeDashArray
            };

            return upArrowPolygon;
        }
    }
}
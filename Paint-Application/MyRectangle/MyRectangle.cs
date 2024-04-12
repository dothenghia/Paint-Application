
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace MyRectangle
{
    public class MyRectangle : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Rectangle"; // Name of the shape
        public string Icon => "Assets/rectangle.png"; // Path to the icon
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
            Rectangle rectangle = new Rectangle()
            {
                StrokeThickness = strokeThickness,
                Stroke = strokeColor,
                StrokeDashArray = strokeDashArray
            };
            if (startPoint.X < endPoint.X)
            {
                rectangle.Width = endPoint.X - startPoint.X;
                rectangle.SetValue(Canvas.LeftProperty, startPoint.X);
            }
            else
            {
                rectangle.Width = startPoint.X - endPoint.X;
                rectangle.SetValue(Canvas.LeftProperty, endPoint.X);
            }
            if (startPoint.Y < endPoint.Y)
            {
                rectangle.Height = endPoint.Y - startPoint.Y;
                rectangle.SetValue(Canvas.TopProperty, startPoint.Y);
            }
            else
            {
                rectangle.Height = startPoint.Y - endPoint.Y;
                rectangle.SetValue(Canvas.TopProperty, endPoint.Y);
            }
            return rectangle;
        }
    }

}

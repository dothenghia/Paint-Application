
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace MyEllipse
{
    public class MyEllipse : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Ellipse"; // Name of the shape
        public string Icon => "Assets/ellipse.png"; // Path to the icon
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
            Ellipse ellipse = new Ellipse()
            {
                StrokeThickness = strokeThickness,
                Stroke = strokeColor,
                StrokeDashArray = strokeDashArray
            };
            if (startPoint.X < endPoint.X)
            {
                ellipse.Width = endPoint.X - startPoint.X;
                ellipse.SetValue(Canvas.LeftProperty, startPoint.X);
            }
            else
            {
                ellipse.Width = startPoint.X - endPoint.X;
                ellipse.SetValue(Canvas.LeftProperty, endPoint.X);
            }
            if (startPoint.Y < endPoint.Y)
            {
                ellipse.Height = endPoint.Y - startPoint.Y;
                ellipse.SetValue(Canvas.TopProperty, startPoint.Y);
            }
            else
            {
                ellipse.Height = startPoint.Y - endPoint.Y;
                ellipse.SetValue(Canvas.TopProperty, endPoint.Y);
            }
            return ellipse;
        }
    }

}

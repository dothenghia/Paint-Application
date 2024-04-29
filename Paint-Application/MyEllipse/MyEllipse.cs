
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
        private SolidColorBrush fillColor = Brushes.White; // Fil color
        private double strokeThickness = 3; // Stroke thickness
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
        public void SetFillColor(SolidColorBrush color)
        {
            fillColor = color;
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

            Ellipse ellipse = new Ellipse()
            {
                Width = canvasWidth,
                Height = canvasHeight,
                StrokeThickness = strokeThickness,
                Stroke = strokeColor,
                StrokeDashArray = strokeDashArray,
                Fill = fillColor
            };

            frameCanvas.Children.Add(ellipse);
            return frameCanvas;
        }
    }

}

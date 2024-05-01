
using MyShapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace RightArrow
{
    public class MyRightArrow : IShape
    {
        // ==================== Attributes ====================
        public string Name => "RightArrow"; // Name of the shape
        public string Icon => "Assets/right-arrow.png"; // Path to the icon
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

            Point[] points = new Point[]
            {
                new Point(0, canvasHeight * 0.25),
                new Point(0, canvasHeight * 0.75),
                new Point(canvasWidth * 0.5, canvasHeight * 0.75),
                new Point(canvasWidth * 0.5, canvasHeight),
                new Point(canvasWidth, canvasHeight * 0.5),
                new Point(canvasWidth * 0.5, canvasHeight * 0),
                new Point(canvasWidth * 0.5, canvasHeight * 0.25)
            };

            // Create a Polygon with the calculated points
            Polygon rightArrowPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(rightArrowPolygon);
            return frameCanvas;
        }

    }
}

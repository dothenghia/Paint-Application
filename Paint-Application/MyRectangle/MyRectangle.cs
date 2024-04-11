
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace MyRectangle
{
    public class MyRectangle : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Rectangle"; // Name of the shape
        public string Icon => "Assets/rectangle.png"; // Path to the icon
        public Point startPoint; // Start point of the shape
        public Point endPoint; // End point of the shape


        // ==================== Methods ====================
        public void SetStartPoint(Point point)
        {
            startPoint = point;
        }
        public void SetEndPoint(Point point)
        {
            endPoint = point;
        }

        // Clone the object
        public object Clone()
        {
            return MemberwiseClone();
        }

        // Convert the object to a UIElement - Draw the shape
        public UIElement Convert()
        {
            return new Line()
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red)
            }; ;
        }
    }

}

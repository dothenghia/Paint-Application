
using System.Windows;
using System.Windows.Media;

namespace MyShapes
{
    public interface IShape : ICloneable
    {
        string Name { get; }
        string Icon { get; }

        void SetStartPoint(Point point);
        void SetEndPoint(Point point);

        void SetStrokeColor(SolidColorBrush color);
        void SetStrokeThickness(double thickness);
        void SetStrokeDashArray(DoubleCollection dashArray);
        
        UIElement Convert();
    }
}

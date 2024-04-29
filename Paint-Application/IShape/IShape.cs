
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyShapes
{
    public interface IShape : ICloneable
    {
        string Name { get; }
        string Icon { get; }
        //string Text { get; set; }

        void SetStartPoint(Point point);
        void SetEndPoint(Point point);

        void SetFillColor(SolidColorBrush color);
        void SetStrokeColor(SolidColorBrush color);
        void SetStrokeThickness(double thickness);
        void SetStrokeDashArray(DoubleCollection dashArray);

        Canvas Convert();
    }
}

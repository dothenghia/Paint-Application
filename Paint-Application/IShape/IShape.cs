
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
        public double Thickness { get; set; }
        public double Angle { get; set; }
        public DoubleCollection StrokeDash { get; set; }
        public SolidColorBrush Brush { get; set; }
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public SolidColorBrush fillColor { get; set; }
        Canvas Convert();
    }
}

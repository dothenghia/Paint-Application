
using System.Windows;

namespace MyShapes
{
    public interface IShape : ICloneable
    {
        string Name { get; }
        string Icon { get; }

        void SetStartPoint(Point point);
        void SetEndPoint(Point point);
        
        UIElement Convert();
    }
}

using MyShapes;

namespace MyClipboardControl
{
    public class ClipboardControl
    {
        public void Cut(List<IShape> drawnShapes, IShape selectShape, List<IShape> memory)
        {
            IShape temp = (IShape)selectShape.Clone();
            memory.Add(temp);
            int index = drawnShapes.IndexOf(selectShape);
            if (index >= 0)
            {
                drawnShapes.RemoveAt(index);
            }
        }

        public void Copy(IShape selectShape, List<IShape> memory)
        {
            IShape temp = (IShape)selectShape.Clone();
            memory.Add(temp);
        }

        public void Paste(List<IShape> drawnShapes, List<IShape> memory)
        {
            if(memory.Count > 0)
            {
                IShape temp = (IShape)memory[memory.Count - 1].Clone();
                drawnShapes.Add(temp);
            }
        }
    }

}

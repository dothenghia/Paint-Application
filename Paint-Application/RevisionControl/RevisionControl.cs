using MyShapes;
using System;

namespace MyRevisionControl
{
    public class RevisionControl
    {
        public void Undo(List<IShape>drawnShapes, Stack<IShape> buffer)
        {
            if (drawnShapes.Count == 0)
                return;
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;

            // Push last shape into buffer and remove it from final list, then re-draw canvas
            int lastIndex = drawnShapes.Count - 1;
            buffer.Push(drawnShapes[lastIndex]);
            drawnShapes.RemoveAt(lastIndex);
        }

        public void Redo(List<IShape> drawnShapes, Stack<IShape> buffer)
        {
            if (buffer.Count == 0)
                return;
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;

            // Pop the last shape from buffer and add it to final list, then re-draw canvas
            drawnShapes.Add(buffer.Pop());
        }
    }

}

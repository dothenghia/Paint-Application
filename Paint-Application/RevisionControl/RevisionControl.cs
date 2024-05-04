using MyShapes;
using System;

namespace MyRevisionControl
{
    public class RevisionControl
    {
        public void Undo(List<IShape>drawnShapes, Stack<IShape> buffer, List<int> position, Stack<int> positionBuffer)
        {
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;
            if(drawnShapes.Count != 0)
            {
                if (!drawnShapes[position[position.Count - 1]].undo())
                {
                    buffer.Push(drawnShapes[position[position.Count - 1]]);
                    drawnShapes.RemoveAt(position[position.Count - 1]);
                    for(int i = 0; i < position.Count; i++)
                    {
                        if(position[i] > position[position.Count - 1])
                        {
                            position[i] -= 1;
                        } else if(position[i] == position[position.Count - 1])
                        {
                            position[i] = drawnShapes.Count - 1;
                        }
                    }
                }
                positionBuffer.Push(position[position.Count - 1]);
                position.RemoveAt(position.Count - 1);
            }
        }

        public void Redo(List<IShape> drawnShapes, Stack<IShape> buffer, List<int> position, Stack<int> positionBuffer, int count)
        {
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;
            if (positionBuffer.Count > 0)
            {
                position.Add(positionBuffer.Pop());

                if (drawnShapes.Count - 1 < position[position.Count - 1])
                {
                    drawnShapes.Add(buffer.Pop());
                    drawnShapes[drawnShapes.Count - 1].actionBuffer.Peek().Type = IShape.ActionType.Create;
                    drawnShapes[drawnShapes.Count - 1].actionHistory.Add(drawnShapes[drawnShapes.Count - 1].actionBuffer.Pop());
                }
                else
                {
                    drawnShapes[position[position.Count - 1]].redo();
                }
            }
            
        }
    }

}

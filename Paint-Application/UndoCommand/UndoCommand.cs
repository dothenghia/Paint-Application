using ICommand;
using MyRevisionControl;
using MyShapes;

namespace MyUndoCommand
{
    public class UndoCommand : Command
    {
        private readonly RevisionControl revisionControl;
        private readonly List<IShape> drawnShapes;
        private readonly Stack<IShape> buffer;
        public UndoCommand(RevisionControl revisionControl, List<IShape> drawnShapes, Stack<IShape> buffer)
        {
            this.revisionControl = revisionControl;
            this.drawnShapes = drawnShapes;
            this.buffer = buffer;
        }

        public void Execute()
        {
            revisionControl.Undo(drawnShapes, buffer);
        }

        public void Undo()
        {
            revisionControl.Redo(drawnShapes, buffer);
        }
    }

}

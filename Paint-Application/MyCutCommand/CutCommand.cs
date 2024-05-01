using MyShapes;
using ICommand;
using System.Windows;
using MyClipboardControl;

namespace MyCutCommand
{
    public class CutCommand : Command
    {
        private readonly ClipboardControl clipboardControl;
        private readonly List<IShape> drawnShapes;
        private readonly IShape selectedShape;
        private readonly List<IShape> memory;
        private readonly bool selection;
        public CutCommand(ClipboardControl clipboardControl, List<IShape> drawnShapes, IShape selectedShape, List<IShape> memory, bool selection) {
            this.clipboardControl = clipboardControl;
            this.drawnShapes = drawnShapes;
            this.selectedShape = selectedShape;
            this.memory = memory;
            this.selection = selection;
        }

        public CutCommand(ClipboardControl clipboardControl, List<IShape> drawnShapes, List<IShape> memory, bool selection)
        {
            this.clipboardControl = clipboardControl;
            this.drawnShapes = drawnShapes;
            this.selectedShape = selectedShape;
            this.memory = memory;
            this.selection = selection;
        }

        public void Execute()
        {
            if(selection == false)
            {
                clipboardControl.Cut(drawnShapes, selectedShape, memory);
            }
            else
            {
                clipboardControl.Copy(selectedShape, memory);
            }
            
        }

        public void Undo()
        {
            clipboardControl.Paste(drawnShapes, memory);
        }
    }

}


using ICommand;

namespace MyToolbarCommand
{
    public class ToolBarCommand
    {
        //private readonly Command Cut;
        private readonly Command Undo;

        //public MyToolBarCommand(Command cut, Command undo)
        //{

        //    this.Cut = cut;
        //    this.Undo = undo;
        //}

        public ToolBarCommand(Command Undo)
        {
            this.Undo = Undo;
        }
        //public void Toolbar_Copy()
        //{
            
        //}
        //public void Toolbar_Cut()
        //{
        //    Cut.Execute();
        //}
        //public void Toolbar_Paste()
        //{
        //    Cut.Undo();
        //}

        public void Toolbar_Undo()
        {
            Undo.Execute();
        }
        public void Toolbar_Redo()
        {
            Undo.Undo();
        }
    }

}

using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint_Application
{
    public partial class MainWindow : Window
    {
        public Paint paint;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            paint = new Paint(Main_Canvas);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Open File"); }
        private void SaveButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Save File"); }
        private void UndoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Undo"); }
        private void RedoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Redo"); }
        private void ExitButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Exit"); }

        private void SelectButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Select"); }

        private void DrawLineButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Draw Line"); }
        private void DrawRectangleButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Draw Rectangle"); }
        private void DrawEllipseButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Draw Ellipse"); }
        private void DrawTriangleButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Draw Triangle"); }

        private void PaintColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                Debug.WriteLine("Color Changed");
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Size"); }
        private void StrokeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Stroke"); }
        private void LayersButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Layers"); }






    }
}
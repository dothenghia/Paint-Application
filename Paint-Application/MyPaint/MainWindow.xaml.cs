using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
using MyShapes;

namespace MyPaint
{
    public partial class MainWindow : Window
    {
        // ==================== Attributes ====================
        public Point startPoint; // Start point of the shape
        public Point endPoint; // End point of the shape

        List<IShape> prototypeShapes = new List<IShape>(); // Danh sách các hình vẽ có thể chọn từ giao diện (Sản phẩm mẫu)
        List<IShape> drawnShapes = new List<IShape>(); // Danh sách các hình đã vẽ trên canvas (dùng để vẽ lại khi MouseMove)
        IShape? currentShape = null; // Current||Preview Shape  - Hình vẽ hiện tại đang vẽ


        // ==================== Methods ====================
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Single configuration
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                // Get all types in the assembly (in the DLL)
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if ((type.IsClass) && (typeof(IShape).IsAssignableFrom(type)))
                    {
                        // Add the shape to the list of prototype shapes
                        prototypeShapes.Add((IShape)Activator.CreateInstance(type)!);
                    }
                }
            }

            RenderShapeButtons(prototypeShapes);
        }

        private void RenderShapeButtons(List<IShape> _prototypeShapes)
        {
            foreach (var shape in _prototypeShapes)
            {
                var button = new Button();
                button.Tag = shape;
                Style style = this.FindResource("FunctionalBarButtonImage_Style") as Style;
                button.Style = style;

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(shape.Icon, UriKind.Relative))
                };
                button.Content = image;
                button.Click += ShapeButton_Click;
                Shapes_StackPanel.Children.Add(button);
            }
        }



        // ==================== Tool Bar Handlers ====================
        private void OpenButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Open File"); }
        private void SaveButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Save File"); }
        private void UndoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Undo"); }
        private void RedoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Redo"); }
        private void ExitButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Exit"); }



        // ==================== Functional Bar Handlers ====================
        private void SelectButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Select"); }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            IShape item = (IShape)(sender as Button)!.Tag;
            Debug.WriteLine(item.Name);
        }

        private void PaintColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) { }

        private void SizeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Size"); }
        private void StrokeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Stroke"); }
        private void LayersButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Layers"); }



        // ==================== Main Canvas Handlers ====================
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e) { }
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e) { }
        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e) { }



    }
}
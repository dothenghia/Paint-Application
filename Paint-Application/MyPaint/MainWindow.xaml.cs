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
        IShape currentShape; // Current Shape  - Hình vẽ hiện tại đang vẽ
        bool isDrawing = false; // Is drawing

        // ==================== Methods ====================
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory; // Single configuration
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var fi in fis) {
                var assembly = Assembly.LoadFrom(fi.FullName); // Get all types in the assembly (in the DLL)
                var types = assembly.GetTypes();

                foreach (var type in types) {
                    if ((type.IsClass) && (typeof(IShape).IsAssignableFrom(type))) {
                        prototypeShapes.Add((IShape)Activator.CreateInstance(type)!); // Add the shape to the list of prototype shapes
                    }
                }
            }

            RenderShapeButtons(prototypeShapes);

            currentShape = prototypeShapes[0]; // Set the default shape
        }

        private void RenderShapeButtons(List<IShape> _prototypeShapes)
        {
            foreach (var shape in _prototypeShapes) {
                var button = new Button();
                button.Tag = shape;
                Style style = this.FindResource("FunctionalBarButtonImage_Style") as Style;
                button.Style = style;
                var image = new Image { Source = new BitmapImage(new Uri(shape.Icon, UriKind.Relative)) };
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
        // --- Select Tool "Select Area"
        private void SelectButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Select"); }

        // --- Select Shape Button
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            IShape item = (IShape)(sender as Button)!.Tag;
            currentShape = item;
        }

        // --- Select Color Stroke
        private void PaintColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) { }

        private void SizeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Size"); }
        private void StrokeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Stroke"); }
        private void LayersButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Layers"); }



        // ==================== Main Canvas Handlers ====================
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        { 
            startPoint = e.GetPosition(Main_Canvas);
            currentShape.SetStartPoint(startPoint);
            isDrawing = false;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                endPoint = e.GetPosition(Main_Canvas);

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    double width = Math.Abs(endPoint.X - startPoint.X);
                    double height = Math.Abs(endPoint.Y - startPoint.Y);
                    double edge = Math.Min(width, height);

                    endPoint.X = startPoint.X + edge * Math.Sign(endPoint.X - startPoint.X);
                    endPoint.Y = startPoint.Y + edge * Math.Sign(endPoint.Y - startPoint.Y);
                }

                currentShape.SetEndPoint(endPoint);

                // Remove the last shape (preview shape)
                if (isDrawing == true) { Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1); }

                // Then re-draw it
                Main_Canvas.Children.Add(currentShape.Convert());
                isDrawing = true;
            }
        }


        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IShape clone = (IShape)currentShape.Clone();
            drawnShapes.Add(clone);
        }



    }
}
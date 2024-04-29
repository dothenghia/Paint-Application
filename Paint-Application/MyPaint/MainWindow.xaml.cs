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
        Dictionary<string, DoubleCollection> dashCollections = new Dictionary<string, DoubleCollection>();

        List<IShape> prototypeShapes = new List<IShape>(); // Danh sách các hình vẽ có thể chọn từ giao diện (Sản phẩm mẫu)
        List<IShape> drawnShapes = new List<IShape>(); // Danh sách các hình đã vẽ trên canvas
        IShape currentShape; // Current Shape  - Hình vẽ hiện tại đang vẽ

        bool isDrawing = false; // Is Drawing - Tránh trường hợp xóa hình vẽ khi đang vẽ
        int selectingIndex = -1;
        bool isSelecting = false; // Is Selecting - Tránh trường họp MouseDown vào hình đã chọn


        // ==================== Methods ====================
        public MainWindow() {
            InitializeComponent();
            dashCollections["Solid"] = null;
            dashCollections["Dash"] = new DoubleCollection { 4, 4 };
            dashCollections["Dot"] = new DoubleCollection { 1, 2 };
            dashCollections["Dash Dot"] = new DoubleCollection { 4, 2, 1, 2 };
            dashCollections["Dash Dot Dot"] = new DoubleCollection { 4, 2, 1, 2, 1, 2 };
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

            RenderShapeButtons(prototypeShapes); // Render the shape buttons

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

        // --- Select Color Stroke & Fill
        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) 
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.SetStrokeColor(color); }
        }
        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.SetFillColor(color); }
        }
        // --- Select Stroke Thickness
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StrokeThickness_TextBlock.Text = Math.Ceiling(e.NewValue).ToString();
            foreach (IShape shape in prototypeShapes) { shape.SetStrokeThickness(e.NewValue); }
        }
        // --- Select Stroke Dash
        private void StrokeDashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StrokeDash_ComboBox.SelectedItem;
            string selectedTag = selectedItem.Tag.ToString();
            
            foreach (IShape shape in prototypeShapes) {
                if (selectedTag == "Solid") { shape.SetStrokeDashArray(null); }
                else { shape.SetStrokeDashArray(dashCollections[selectedTag]); }
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Size"); }
        private void StrokeButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Change Stroke"); }
        private void LayersButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Layers"); }



        // ==================== Main Canvas Handlers ====================
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == false)
            {
                Debug.WriteLine("MainCanvas - MouseDown");
                startPoint = e.GetPosition(Main_Canvas);
                currentShape.SetStartPoint(startPoint);
                isDrawing = false;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting == false && e.LeftButton == MouseButtonState.Pressed)
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
                Canvas shapeCanvas = currentShape.Convert();
                shapeCanvas.PreviewMouseDown += DrawnShape_PreviewMouseDown;

                Main_Canvas.Children.Add(shapeCanvas);
                isDrawing = true;
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == false)
            {
                IShape clone = (IShape)currentShape.Clone();
                drawnShapes.Add(clone);
            }
        }

        private void DrawnShape_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isSelecting = true;
            if (sender is Canvas shapeCanvas)
            {
                DrawSelectingShape(shapeCanvas);
            }
        }

        private void DrawSelectingShape(Canvas canvas)
        {
            // Xóa "Selecting_Rectangle" của shape có selectinIndex
            if (selectingIndex != -1)
            {
                Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];
                foreach (UIElement child in selectingCanvas.Children)
                {
                    if (child is Rectangle rectangle && rectangle.Name == "Selecting_Rectangle")
                    {
                        selectingCanvas.Children.Remove(rectangle);
                        break;
                    }
                }
            }

            // Tạo một Rectangle để vẽ viền cho hình đã chọn
            Rectangle borderRectangle = new Rectangle
            {
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Fill = Brushes.Transparent,
                Name = "Selecting_Rectangle"
            };

            canvas.Children.Add(borderRectangle);
            selectingIndex = Main_Canvas.Children.IndexOf(canvas);
        }

    }
}

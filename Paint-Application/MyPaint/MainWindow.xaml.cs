using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
using Microsoft.Win32;
using MyShapes;
using DownArrow;
using Ellipse_;
using LeftArrow;
using Line_;
using Rectangle_;
using RightArrow;
using Star;
using Triangle;
using UpArrow;
using ICommand;
using MyUndoCommand;
using MyRevisionControl;
using MyToolbarCommand;
using MyCutCommand;
using MyClipboardControl;

namespace MyPaint
{
    public partial class MainWindow : Window
    {
        // ==================== Attributes ====================
        public Point startPoint; // Start point of the shape
        public Point endPoint; // End point of the shape
        Dictionary<string, DoubleCollection> dashCollections = new Dictionary<string, DoubleCollection>(); // List of dash styles

        private Stack<IShape> _buffer = new Stack<IShape>();// Danh sách các hình vẽ được pop ra sau khi undo

        List<IShape> prototypeShapes = new List<IShape>(); // Danh sách các hình vẽ có thể chọn từ giao diện (Sản phẩm mẫu)
        List<IShape> drawnShapes = new List<IShape>(); // Danh sách các hình đã vẽ trên canvas
        IShape currentShape; // Current Shape  - Hình vẽ hiện tại đang vẽ
        IShape memoryShape;
        bool isSelectingArea = false;
        string choice;
        List<IShape> memory = new List<IShape> ();
        enum myMode
        {
            draw,
            select
        };

        bool isDrawing = false; // Is Drawing - Used to remove the last shape (preview shape) when drawing
        bool isDrawn = false; // Is Drawn - Used to check if the shape is drawn or not (handle MouseUp event is triggered)

        int selectingIndex = -1; // Index of the selecting single shape
        bool isSelecting = false; // Is Selecting - Used to check if the shape is selecting or not (avoid drawing new shape when selecting shape)
        Point dragStartPoint; // Start point when dragging the shape

        RevisionControl revisionControl = new RevisionControl();
        ClipboardControl clipboard = new ClipboardControl();

        // ==================== Methods ====================
        public MainWindow()
        {
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

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName); // Get all types in the assembly (in the DLL)
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if ((type.IsClass) && (typeof(IShape).IsAssignableFrom(type)))
                    {
                        prototypeShapes.Add((IShape)Activator.CreateInstance(type)!); // Add the shape to the list of prototype shapes
                    }
                }
            }

            RenderShapeButtons(prototypeShapes); // Render the shape buttons

            currentShape = prototypeShapes[0]; // Set the default shape
            choice = myMode.draw.ToString();
        }

        private void RenderShapeButtons(List<IShape> _prototypeShapes)
        {
            foreach (var shape in _prototypeShapes)
            {
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
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Children.Clear();
            drawnShapes.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "dat";        // Default file extension
            openFileDialog.Filter = "dat files (*.dat)|*.dat"; // Filter for .dat files
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                importCanvas(openFileDialog.FileName);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.DefaultExt = "dat"; // Default file extension
            saveFileDialog.Filter = "dat files (*.dat)|*.dat"; // Filter for .dat files

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                exportCanvas(fileName);
            }
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectingIndex > -1)
            {
                CutCommand cut = new CutCommand(clipboard, drawnShapes, drawnShapes[selectingIndex], memory, true);
                ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, _buffer));
                toolBarCommand.Toolbar_Copy();
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectingIndex > -1)
            {
                CutCommand cut = new CutCommand(clipboard, drawnShapes, drawnShapes[selectingIndex], memory, false);
                ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, _buffer));
                toolBarCommand.Toolbar_Cut();
                RedrawCanvas();
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            CutCommand cut = new CutCommand(clipboard, drawnShapes, memory, true);
            ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, _buffer));
            toolBarCommand.Toolbar_Paste();
            RedrawCanvas();
        }

        public void exportCanvas(string fileName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(drawnShapes.Count);
                    foreach (var shape in drawnShapes)
                    {
                        WriteShapeData(writer, shape);
                    }
                }
                MessageBox.Show("Shapes exported successfully.", "Export Shapes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting shapes: " + ex.Message, "Export Shapes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Method to write shape data
        private void WriteShapeData(BinaryWriter writer, IShape shape)
        {
            writer.Write(shape.Name);
            writer.Write(shape.Thickness);

            // Write DoubleCollection
            writer.Write(shape.StrokeDash.Count);
            foreach (double value in shape.StrokeDash)
            {
                writer.Write(value);
            }

            // Write SolidColorBrush color
            Color color = shape.Brush.Color;
            writer.Write(color.A);
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);

            Color background = shape.fillColor.Color;
            writer.Write(background.A);
            writer.Write(background.R);
            writer.Write(background.G);
            writer.Write(background.B);

            writer.Write(shape.startPoint.X);
            writer.Write(shape.startPoint.Y);
            writer.Write(shape.endPoint.X);
            writer.Write(shape.endPoint.Y);
        }
        public void importCanvas(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        // Read shape data
                        int numberofShape = reader.ReadInt32();
                        for (int i = 0; i < numberofShape; i++)
                        {
                            string name = reader.ReadString();
                            double thickness = reader.ReadDouble();
                            int strokeDashCount = reader.ReadInt32();
                            DoubleCollection strokeDash = new DoubleCollection();
                            for (int j = 0; j < strokeDashCount; j++)
                            {
                                strokeDash.Add(reader.ReadDouble());
                            }
                            byte a = reader.ReadByte();
                            byte r = reader.ReadByte();
                            byte g = reader.ReadByte();
                            byte b = reader.ReadByte();
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                            byte ba = reader.ReadByte();
                            byte br = reader.ReadByte();
                            byte bg = reader.ReadByte();
                            byte bb = reader.ReadByte();
                            SolidColorBrush background = new SolidColorBrush(Color.FromArgb(ba, br, bg, bb));
                            Point startPoint = new Point(reader.ReadDouble(), reader.ReadDouble());
                            Point endPoint = new Point(reader.ReadDouble(), reader.ReadDouble());

                            IShape shape;
                            // Create an instance of IShape using the read data
                            switch (name)
                            {
                                case "DownArrow":
                                    MyDownArrow myDownArrow = new MyDownArrow();
                                    myDownArrow.Thickness = thickness;
                                    myDownArrow.startPoint = startPoint;
                                    myDownArrow.endPoint = endPoint;
                                    myDownArrow.Brush = brush;
                                    myDownArrow.fillColor = background;
                                    myDownArrow.StrokeDash = strokeDash;
                                    shape = (IShape)myDownArrow;
                                    drawnShapes.Add(shape);
                                    break;
                                case "Ellipse":
                                    MyEllipse myEllipse = new MyEllipse();
                                    myEllipse.Thickness = thickness;
                                    myEllipse.startPoint = startPoint;
                                    myEllipse.endPoint = endPoint;
                                    myEllipse.Brush = brush;
                                    myEllipse.fillColor = background;
                                    myEllipse.StrokeDash = strokeDash;
                                    shape = (IShape)myEllipse;
                                    drawnShapes.Add(shape);
                                    break;
                                case "LeftArrow":
                                    MyLeftArrow myLeftArrow = new MyLeftArrow();
                                    myLeftArrow.Thickness = thickness;
                                    myLeftArrow.startPoint = startPoint;
                                    myLeftArrow.endPoint = endPoint;
                                    myLeftArrow.Brush = brush;
                                    myLeftArrow.StrokeDash = strokeDash;
                                    myLeftArrow.fillColor = background;
                                    shape = (IShape)myLeftArrow;
                                    drawnShapes.Add(shape);
                                    break;
                                case "Line":
                                    MyLine myLine = new MyLine();
                                    myLine.Thickness = thickness;
                                    myLine.startPoint = startPoint;
                                    myLine.endPoint = endPoint;
                                    myLine.Brush = brush;
                                    myLine.StrokeDash = strokeDash;
                                    myLine.fillColor = background;
                                    shape = (IShape)myLine;
                                    drawnShapes.Add(shape);
                                    break;
                                case "Rectangle":
                                    MyRectangle myRectangle = new MyRectangle();
                                    myRectangle.Thickness = thickness;
                                    myRectangle.startPoint = startPoint;
                                    myRectangle.endPoint = endPoint;
                                    myRectangle.Brush = brush;
                                    myRectangle.StrokeDash = strokeDash;
                                    myRectangle.fillColor = background;
                                    shape = (IShape)myRectangle;
                                    drawnShapes.Add(shape);
                                    break;
                                case "RightArrow":
                                    MyRightArrow myRightArrow = new MyRightArrow();
                                    myRightArrow.Thickness = thickness;
                                    myRightArrow.startPoint = startPoint;
                                    myRightArrow.endPoint = endPoint;
                                    myRightArrow.Brush = brush;
                                    myRightArrow.StrokeDash = strokeDash;
                                    myRightArrow.fillColor = background;
                                    shape = (IShape)myRightArrow;
                                    drawnShapes.Add(shape);
                                    break;
                                case "Star":
                                    MyStar myStar = new MyStar();
                                    myStar.Thickness = thickness;
                                    myStar.startPoint = startPoint;
                                    myStar.endPoint = endPoint;
                                    myStar.Brush = brush;
                                    myStar.StrokeDash = strokeDash;
                                    myStar.fillColor = background;
                                    shape = (IShape)myStar;
                                    drawnShapes.Add(shape);
                                    break;
                                case "Triangle":
                                    MyTriangle myTriangle = new MyTriangle();
                                    myTriangle.Thickness = thickness;
                                    myTriangle.startPoint = startPoint;
                                    myTriangle.endPoint = endPoint;
                                    myTriangle.Brush = brush;
                                    myTriangle.StrokeDash = strokeDash;
                                    myTriangle.fillColor = background;
                                    shape = (IShape)myTriangle;
                                    drawnShapes.Add(shape);
                                    break;
                                case "UpArrow":
                                    MyUpArrow myUpArrow = new MyUpArrow();
                                    myUpArrow.Thickness = thickness;
                                    myUpArrow.startPoint = startPoint;
                                    myUpArrow.endPoint = endPoint;
                                    myUpArrow.Brush = brush;
                                    myUpArrow.StrokeDash = strokeDash;
                                    myUpArrow.fillColor = background;
                                    shape = (IShape)myUpArrow;
                                    drawnShapes.Add(shape);
                                    break;
                                default:
                                    throw new ArgumentException("Unexpected value for name");
                            }
                        }
                    }
                }
                RedrawCanvas();
                MessageBox.Show("Shapes imported successfully.", "Import Shapes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error importing shapes: " + ex.Message, "Import Shapes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            Command control = new UndoCommand(revisionControl, drawnShapes, _buffer);
            ToolBarCommand toolBarCommand = new ToolBarCommand(control);
            toolBarCommand.Toolbar_Undo();
            RedrawCanvas();
        }
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            Command control = new UndoCommand(revisionControl, drawnShapes, _buffer);
            ToolBarCommand toolBarCommand = new ToolBarCommand(control);
            toolBarCommand.Toolbar_Redo();
            RedrawCanvas();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Exit"); }
        private void RedrawCanvas()
        {
            Main_Canvas.Children.Clear();
            Console.WriteLine(drawnShapes.Count);
            foreach (var shape in drawnShapes)
            {
                Canvas shapeCanvas = shape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;

                Main_Canvas.Children.Add(shapeCanvas);
            }
        }
        // ==================== Functional Bar Handlers ====================


        // --- Select Tool "Select Area"
        private void CopySelectedAreaToClipboard()
        {
            double left;
            double top;
            double width;
            double height;

            // Adjust the selected area
            left = Math.Max(0, Math.Min(startPoint.X, endPoint.X));
            top = Math.Max(0, Math.Min(startPoint.Y, endPoint.Y));
            width = Math.Abs(endPoint.X - startPoint.X);
            height = Math.Abs(endPoint.Y - startPoint.Y);

            drawnShapes.RemoveAt(drawnShapes.Count - 1);
            Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1);

            // Create a DrawingVisual for the selected area
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Draw the Main_Canvas content to the DrawingContext with the selected area
                drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, width, height)));
                VisualBrush visualBrush = new VisualBrush(Main_Canvas)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                };
                drawingContext.DrawRectangle(new VisualBrush(Main_Canvas), null, new Rect(-left, -top, Main_Canvas.ActualWidth, Main_Canvas.ActualHeight));
            }

            // Render the DrawingVisual to a RenderTargetBitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)width,
                (int)height,
                96,
                96,
                PixelFormats.Pbgra32);

            renderTargetBitmap.Render(drawingVisual);

            Clipboard.SetImage(renderTargetBitmap);
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (choice == myMode.select.ToString())
            {
                CopySelectedAreaToClipboard();
                choice = "";
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Select";
                }
            }
            else
            {
                choice = myMode.select.ToString();
                MyRectangle rect = new MyRectangle();
                currentShape = rect;
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Copy to Clipboard";
                }
            }
        }

        // --- Select Shape Button
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            choice = myMode.draw.ToString();
            IShape item = (IShape)(sender as Button)!.Tag;
            currentShape = item;
        }

        // --- Select Color Stroke & Fill
        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.Brush = color; }
        }
        
        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.fillColor = color; }
        }

        // --- Select Stroke Thickness
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StrokeThickness_TextBlock.Text = Math.Ceiling(e.NewValue).ToString();
            foreach (IShape shape in prototypeShapes) { shape.Thickness = e.NewValue; }
        }

        // --- Select Stroke Dash
        private void StrokeDashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StrokeDash_ComboBox.SelectedItem;
            string selectedTag = selectedItem.Tag.ToString();

            foreach (IShape shape in prototypeShapes)
            {
                if (selectedTag == "Solid") { shape.StrokeDash = null; }
                else { shape.StrokeDash = dashCollections[selectedTag]; }
            }
        }


        private void LayersButton_Click(object sender, RoutedEventArgs e) 
        { 
            foreach (IShape shape in drawnShapes)
            {
                Canvas shapeCanvas = shape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;
                Main_Canvas.Children.Add(shapeCanvas);
            }
        }

        // --- Erase Button => TEST CLEAR ALL SHAPES
        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Children.Clear();
            // drawnShapes.Clear();
        }



        // ==================== Main Canvas Handlers ====================
        
        // MainCanvas_PreviewMouseDown will be triggered before ShapeCanvas_PreviewMouseDown
        // Used to remove the selecting shape when clicking outside the shape
        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isSelecting = false;
            RemoveSelectingShape();
            selectingIndex = -1;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == false)
            {
                startPoint = e.GetPosition(Main_Canvas);
                currentShape.startPoint = startPoint;
                isDrawing = false;
                isDrawn = false;
                isSelectingArea = false;
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

                currentShape.endPoint = endPoint;

                // Remove the last shape (preview shape)
                if (isDrawing == true || isSelectingArea == true)
                {
                    Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1);
                    drawnShapes.RemoveAt(drawnShapes.Count - 1);
                }
                drawnShapes.Add((IShape)currentShape.Clone());
                if (_buffer.Count > 0)
                {
                    _buffer.Clear();
                }
                // Then re-draw it
                Canvas shapeCanvas = currentShape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;

                Main_Canvas.Children.Add(shapeCanvas);
                if (choice == myMode.select.ToString())
                {
                    isSelectingArea = true;
                    currentShape.StrokeDash = dashCollections["Dot"];
                    currentShape.Thickness = 3;
                    currentShape.fillColor = new SolidColorBrush(Colors.Transparent);
                }
                else if (choice == myMode.draw.ToString())
                {
                    isDrawing = true;
                }
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing == true && isDrawn == false)
            {
                IShape clone = (IShape)currentShape.Clone();
                isDrawn = true;
            }
        }


        private void ShapeCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas shapeCanvas)
            {
                isSelecting = true;

                DrawSelectingShape(shapeCanvas);
                
                selectingIndex = Main_Canvas.Children.IndexOf(shapeCanvas);

                dragStartPoint = e.GetPosition(Main_Canvas);
            }
        }

        private void ShapeCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting == true && e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas shapeCanvas = Main_Canvas.Children[selectingIndex] as Canvas;
                shapeCanvas.Cursor = Cursors.SizeAll;
                Point currentPoint = e.GetPosition(Main_Canvas);
                double offsetX = currentPoint.X - dragStartPoint.X;
                double offsetY = currentPoint.Y - dragStartPoint.Y;

                // Move the shape
                Canvas.SetLeft(shapeCanvas, Canvas.GetLeft(shapeCanvas) + offsetX);
                Canvas.SetTop(shapeCanvas, Canvas.GetTop(shapeCanvas) + offsetY);

                // Update the start and end points of the shape
                dragStartPoint = currentPoint;
            }
        }

        private void ShapeCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == true)
            {
                if (sender is Canvas shapeCanvas)
                {
                    drawnShapes[selectingIndex].startPoint = new Point(Canvas.GetLeft(shapeCanvas), Canvas.GetTop(shapeCanvas));
                    drawnShapes[selectingIndex].endPoint = new Point(Canvas.GetLeft(shapeCanvas) + shapeCanvas.ActualWidth, Canvas.GetTop(shapeCanvas) + shapeCanvas.ActualHeight);
                }
            }
            // For Line, Rectangle because they are hit the cursor => Do not catch the MainCanvas_MouseUp event
            else if (isDrawing == true && isDrawn == false)
            {
                IShape clone = (IShape)currentShape.Clone();
                isDrawn = true;
            }
        }



        private void RemoveSelectingShape()
        {
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
        }

        private void DrawSelectingShape(Canvas canvas)
        {
            Rectangle selectingRectangle = new Rectangle
            {
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Fill = Brushes.Transparent,
                Name = "Selecting_Rectangle"
            };
            
            canvas.Children.Add(selectingRectangle);
        }

    }
}

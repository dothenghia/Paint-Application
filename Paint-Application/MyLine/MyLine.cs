
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Line_
{
    public class MyLine : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Line"; // Name of the shape
        public string Icon => "Assets/line.png"; // Path to the icon
        public RichTextBox richTextBox { get; set; }
        public double Thickness { get; set; } = 3;
        public double Angle { get; set; } = 0;
        public DoubleCollection StrokeDash { get; set; } = new DoubleCollection();
        public SolidColorBrush Brush { get; set; } = Brushes.Black;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color

        // Clone the object
        public object Clone()
        {
            IShape clonedShape = (IShape)MemberwiseClone();

            // Clone the RichTextBox if it exists
            if (richTextBox != null)
            {
                // Create a RichTextBox
                clonedShape.richTextBox = new System.Windows.Controls.RichTextBox()
                {
                    Width = Math.Abs(endPoint.X - startPoint.X),
                    Height = Math.Abs(endPoint.Y - startPoint.Y),
                    Background = Brushes.Transparent,
                    Foreground = richTextBox.Foreground,
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                string text = textRange.Text;

                string fontFamily = richTextBox.FontFamily.ToString();
                double fontSize = richTextBox.FontSize;

                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Center;

                clonedShape.richTextBox.Document.Blocks.Add(paragraph);
                clonedShape.richTextBox.FontFamily = new FontFamily(fontFamily);
                clonedShape.richTextBox.FontSize = fontSize;
                clonedShape.richTextBox.Padding = new Thickness(0, 0, 0, 0);
                TextRange textRangeCloned = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                textRangeCloned.Text = text.Replace("\r\n", ""); ;

                foreach (var child in richTextBox.Document.Blocks.ToList())
                {
                    if (child is Paragraph)
                    {
                        TextRange t = new TextRange(child.ContentStart, child.ContentEnd);
                        if (t.GetPropertyValue(TextElement.BackgroundProperty) is SolidColorBrush backgroundTextColor)
                        {
                            textRangeCloned.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundTextColor);
                        }
                    }
                }
            }

            return clonedShape;
        }

        // Convert the object to a UIElement - Draw the shape
        public Canvas Convert()
        {
            // Calculate canvas position and size
            double canvasLeft = Math.Min(startPoint.X, endPoint.X);
            double canvasTop = Math.Min(startPoint.Y, endPoint.Y);
            double canvasWidth = Math.Abs(endPoint.X - startPoint.X);
            double canvasHeight = Math.Abs(endPoint.Y - startPoint.Y);

            // Set canvas position and size
            Canvas frameCanvas = new Canvas();
            Canvas.SetLeft(frameCanvas, canvasLeft);
            Canvas.SetTop(frameCanvas, canvasTop);
            frameCanvas.Width = canvasWidth;
            frameCanvas.Height = canvasHeight;

            // Create a new Line
            Line line = new Line()
            {
                X1 = startPoint.X - canvasLeft,
                Y1 = startPoint.Y - canvasTop,
                X2 = endPoint.X - canvasLeft,
                Y2 = endPoint.Y - canvasTop,
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(line);


            // Remove the RichTextBox from its current parent before adding it to frameCanvas
            if (richTextBox != null && richTextBox.Parent != null)
            {
                var parent = richTextBox.Parent as Panel;
                parent.Children.Remove(richTextBox);
            }

            if (richTextBox != null)
            {
                frameCanvas.Children.Add(richTextBox);
            }

            RotateTransform rotateTransform = new RotateTransform(Angle);
            frameCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            frameCanvas.RenderTransform = rotateTransform;
            return frameCanvas;
        }

    }
}

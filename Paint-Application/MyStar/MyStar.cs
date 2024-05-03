
using MyShapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Star
{
    public class MyStar : IShape
    {
        // ==================== Attributes ====================
        public string Name => "Star"; // Name of the shape
        public string Icon => "Assets/star.png"; // Path to the icon
        public RichTextBox richTextBox { get; set; }
        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color
        public double Thickness { get; set; } = 3;
        public double Angle { get; set; } = 0;
        public DoubleCollection StrokeDash { get; set; } = new DoubleCollection();
        public SolidColorBrush Brush { get; set; } = Brushes.Black;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }

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

            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;
            double radius = Math.Min(canvasWidth, canvasHeight) / 2;

            Point[] points = new Point[10];
            double angleIncrement = 2 * Math.PI / 10; // 5 points in total, each point separated by 2 * Math.PI / 5 radians
            double currentAngle = -Math.PI / 2; // Start from the top point of the star
            for (int i = 0; i < 10; i++)
            {
                double x = centerX + (radius * Math.Cos(currentAngle) * canvasWidth / canvasHeight);
                double y = centerY + (radius * Math.Sin(currentAngle) * canvasHeight / canvasWidth);
                points[i] = new Point(x, y);
                currentAngle += angleIncrement;
                x = centerX + (radius / 2.5 * Math.Cos(currentAngle) * canvasWidth / canvasHeight);
                y = centerY + (radius / 2.5 * Math.Sin(currentAngle) * canvasHeight / canvasWidth);
                points[++i] = new Point(x, y);
                currentAngle += angleIncrement;
            }

            // Create a Polygon with the calculated points
            Polygon starPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(starPolygon);


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

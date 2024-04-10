using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Paint
    {
        // ==================== Attributes ====================
        public enum MyShape
        {
            Line, Ellipse, Rectangle, Text, Triangle
        }

        public Canvas mainCanvas; // Main canvas
        public MyShape currentShape; // Current shape to draw
        public Point startPoint; // Start point of the shape
        public Point endPoint; // End point of the shape

        public Shape? previewShape; // Preview shape

        public SolidColorBrush strokeColor = Brushes.Black; // Stroke color
        public double strokeThickness = 2; // Stroke thickness
        public DoubleCollection? strokeDash = null; // Stroke dash

        public bool isShiftPressed = false; // Shift key pressed


        // ==================== Methods ====================
        public Paint(Canvas mainCanvas)
        {
            this.mainCanvas = mainCanvas;
            this.previewShape = null;
        }

        public void StartShape()
        {
            previewShape = null;

            switch (currentShape)
            {
                case MyShape.Line:
                    DrawLine(startPoint, startPoint);
                    break;

                default:
                    Debug.WriteLine("Please choose shape !");
                    break;
            }
        }

        public void UpdateShape()
        {
            switch (currentShape)
            {
                case MyShape.Line:
                    DrawLine(startPoint, endPoint);
                    break;

                default:
                    Debug.WriteLine("Please choose shape !");
                    break;
            }
        }

        public void EndShape()
        {
            switch (currentShape)
            {
                case MyShape.Line:
                    DrawLine(startPoint, endPoint);
                    break;

                default:
                    return;
            }
        }


        private void DrawLine(Point start, Point end)
        {
            if (previewShape != null)
                mainCanvas.Children.Remove(previewShape);

            Line newLine = new Line()
            {
                Stroke = strokeColor,
                StrokeDashArray = strokeDash,
                StrokeThickness = strokeThickness,
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
            };
            mainCanvas.Children.Add(newLine);
            previewShape = newLine;
        }
    }
}

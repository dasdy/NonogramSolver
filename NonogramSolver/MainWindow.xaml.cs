using System;
using System.Collections.Generic;
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

namespace NonogramSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var content = (StackPanel)this.Content;

            var canvas = (Canvas)content.Children[0];

            var squareLength = 30;
            var rows = 10;
            var columns = 20;

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            SolidColorBrush emptyCell = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            emptyCell.Color = Color.FromArgb(255, 0, 0, 255);

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            SolidColorBrush filledCell = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            filledCell.Color = Color.FromArgb(255, 255, 255, 255);


            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            SolidColorBrush undefinedCell = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            undefinedCell.Color = Color.FromArgb(255, 255, 255, 0);

            var nonogram = new Solver.Nonogram(rows, columns);

            var rand = new Random();
            var a = (Array)(new int[] { 5 });
            var enumValues = (int[])Enum.GetValues(typeof(Solver.CellState));
            foreach(var r in nonogram.Cells)
            {
                foreach(var cell in r)
                {
                    cell.State = (Solver.CellState)enumValues[rand.Next(enumValues.Length)];
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    // Create a red Ellipse.
                    var rectangle = new Rectangle();

                    switch (nonogram.Cells[j][i].State) {
                        case Solver.CellState.Undefined:
                            rectangle.Fill = undefinedCell;
                            break;
                        case Solver.CellState.Filled:
                            rectangle.Fill = filledCell;
                            break;
                        case Solver.CellState.Empty:
                            rectangle.Fill = emptyCell;
                            break;
                        default:
                            throw new ArgumentException("wtf: " + nonogram.Cells[i][j].State);

                }
                    rectangle.StrokeThickness = 2;
                    rectangle.Stroke = Brushes.Black;

                    // Set the width and height of the Ellipse.
                    rectangle.Width = squareLength;
                    rectangle.Height = squareLength;

                    Canvas.SetLeft(rectangle, j * squareLength);
                    Canvas.SetTop(rectangle, i * squareLength);

                    // Add the Ellipse to the StackPanel.
                    canvas.Children.Add(rectangle);
                }
            }
        }
    }
}

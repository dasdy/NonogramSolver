using NonogramSolver.Solver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NonogramSolver
{
    public class NonogramView : Canvas, INotifyPropertyChanged
    {
        
        private Solver.Nonogram nonogram;
        public const int SquareLength = 20;
        private readonly SolidColorBrush emptyCell = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private readonly SolidColorBrush filledCell = new SolidColorBrush(Color.FromArgb(120,0,0,0));
        private readonly SolidColorBrush undefinedCell = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));

        public event PropertyChangedEventHandler PropertyChanged;

        public Solver.Nonogram Nonogram
        {
            get
            {
                return nonogram;
            }
            set
            {
                nonogram = value;
                RefreshCanvas();
            }
        }

        private Rectangle GetRectangle()
        {
            var rectangle = new Rectangle();
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = Brushes.Black;
            rectangle.Width = SquareLength;
            rectangle.Height = SquareLength;
            return rectangle;
        }

        private void RefreshCanvas()
        {
            Children.Clear();
            var horizontalOffset = SquareLength * nonogram.RowDescriptors.Max(desc => desc.BlockSizes.Count);
            var verticalOffset = SquareLength * nonogram.ColumnDescriptors.Max(desc => desc.BlockSizes.Count);
            int rows = nonogram.Height;
            int columns = nonogram.Width;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    var rectangle = GetRectangle();
                    int inner_j = j;
                    int inner_i = i;
                    rectangle.MouseDown += (e, o) =>
                    {
                        Cell cell = nonogram.Cells[inner_i][inner_j];
                        RotateState(cell);
                        rectangle.Fill = ChooseBrush(emptyCell, filledCell, undefinedCell, cell.State);
                        NotifyCellChanged();
                    };
                    CellState state = nonogram.Cells[i][j].State;

                    rectangle.Fill = ChooseBrush(emptyCell, filledCell, undefinedCell, state);

                    Canvas.SetLeft(rectangle, horizontalOffset + j * SquareLength);
                    Canvas.SetTop(rectangle, verticalOffset + i * SquareLength);

                    Children.Add(rectangle);
                }
            }
            const int horizontalTextOffset = 5;
            for (int i = 0; i < rows; i++)
            {
                var rowDescriptor = nonogram.RowDescriptors[i];

                int originalOffset = horizontalOffset - rowDescriptor.BlockSizes.Count * SquareLength;

                for (int j = 0; j < rowDescriptor.BlockSizes.Count; j++)
                {
                    var rectangle = GetRectangle();
                    rectangle.StrokeThickness = 1;

                    rectangle.Width = SquareLength - .5;
                    rectangle.Height = SquareLength - .5;

                    Canvas.SetLeft(rectangle, originalOffset + j * SquareLength);
                    Canvas.SetTop(rectangle, verticalOffset + i * SquareLength);

                    Children.Add(rectangle);

                    var text = new TextBlock();
                    text.Foreground = Brushes.Black;
                    text.Text = rowDescriptor.BlockSizes[j].ToString();
                    Canvas.SetTop(text, verticalOffset + i * SquareLength);
                    Canvas.SetLeft(text, horizontalTextOffset + originalOffset + j * SquareLength);
                    Children.Add(text);
                }
            }
            for (int i = 0; i < columns; i++)
            {
                var rowDescriptor = nonogram.ColumnDescriptors[i];

                int originalOffset = verticalOffset - rowDescriptor.BlockSizes.Count * SquareLength;

                for (int j = 0; j < rowDescriptor.BlockSizes.Count; j++)
                {
                    var rectangle = GetRectangle();
                    rectangle.StrokeThickness = 1;
                    rectangle.Stroke = Brushes.Black;

                    // Set the width and height of the Ellipse.
                    rectangle.Width = SquareLength - .5;
                    rectangle.Height = SquareLength - .5;

                    Canvas.SetLeft(rectangle, horizontalOffset + i * SquareLength);
                    Canvas.SetTop(rectangle, originalOffset + j * SquareLength);

                    Children.Add(rectangle);

                    var text = new TextBlock();
                    text.Foreground = Brushes.Black;
                    text.Text = rowDescriptor.BlockSizes[j].ToString();
                    Canvas.SetTop(text, originalOffset + j * SquareLength);
                    Canvas.SetLeft(text, horizontalTextOffset + horizontalOffset + i * SquareLength);
                    Children.Add(text);
                }
            }

            Width = SquareLength * columns + horizontalOffset;
            Height = SquareLength * rows + verticalOffset;
        }

        private void NotifyCellChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("some_cell"));
        }

        private void RotateState(Cell cell)
        {
            switch (cell.State)
            {
                case CellState.Empty:
                    cell.State = CellState.Filled;
                    break;
                case CellState.Filled:
                    cell.State = CellState.Undefined;
                    break;
                case CellState.Undefined:
                    cell.State = CellState.Empty;
                    break;
                default:
                    throw new ArgumentException("wtf: " + cell.State);
            }
        }

        private static SolidColorBrush ChooseBrush(SolidColorBrush emptyCell, SolidColorBrush filledCell, SolidColorBrush undefinedCell, CellState state)
        {
            SolidColorBrush brush;
            switch (state)
            {
                case Solver.CellState.Undefined:
                    brush = undefinedCell;
                    break;
                case Solver.CellState.Filled:
                    brush = filledCell;
                    break;
                case Solver.CellState.Empty:
                    brush = emptyCell;
                    break;
                default:
                    throw new ArgumentException("wtf: " + state);

            }

            return brush;
        }

        
    }
}

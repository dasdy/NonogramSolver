using NonogramSolver.Solver;
using System;
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
            var rows = 10;
            var columns = 20;

            Solver.Nonogram nonogram = genNonogram(rows, columns);

            DrawNonogramField(rows, columns, nonogram);
        }

        private void DrawNonogramField(int rows, int columns, Solver.Nonogram nonogram)
        {
            var content = (StackPanel)this.Content;
            var canvas = (Canvas)content.Children[0];
            var squareLength = 30;
            var horizontalOffset = squareLength * nonogram.RowDescriptors.Max(desc => desc.BlockSizes.Count);
            var verticalOffset = squareLength * nonogram.ColumnDescriptors.Max(desc => desc.BlockSizes.Count);

            SolidColorBrush emptyCell = new SolidColorBrush();
            emptyCell.Color = Color.FromArgb(255, 255, 255, 255);
            SolidColorBrush filledCell = new SolidColorBrush();
            filledCell.Color = Color.FromArgb(120, 0, 0, 0);
            SolidColorBrush undefinedCell = new SolidColorBrush();
            undefinedCell.Color = Color.FromArgb(255, 255, 255, 0);


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    // Create a red Ellipse.
                    var rectangle = new Rectangle();
                    int inner_j = j;
                    int inner_i = i;
                    rectangle.MouseDown += (e, o) =>
                    {
                        Cell cell = nonogram.Cells[inner_i][inner_j];
                        RotateState(cell);
                        rectangle.Fill = ChooseBrush(emptyCell, filledCell, undefinedCell, cell.State);
                    };
                    CellState state = nonogram.Cells[i][j].State;

                    rectangle.Fill = ChooseBrush(emptyCell, filledCell, undefinedCell, state);
                    rectangle.StrokeThickness = 2;
                    rectangle.Stroke = Brushes.Black;

                    // Set the width and height of the Ellipse.
                    rectangle.Width = squareLength;
                    rectangle.Height = squareLength;

                    Canvas.SetLeft(rectangle, horizontalOffset + j * squareLength);
                    Canvas.SetTop(rectangle, verticalOffset + i * squareLength);

                    // Add the Ellipse to the StackPanel.
                    canvas.Children.Add(rectangle);
                }
            }

            const int horizontalTextOffset = 5;
            for (int i = 0; i < rows; i++)
            {
                var rowDescriptor = nonogram.RowDescriptors[i];

                int originalOffset = horizontalOffset - rowDescriptor.BlockSizes.Count * squareLength;

                for (int j = 0; j < rowDescriptor.BlockSizes.Count; j++)
                {
                    var rectangle = new Rectangle();
                    rectangle.StrokeThickness = 1;
                    rectangle.Stroke = Brushes.Black;

                    // Set the width and height of the Ellipse.
                    rectangle.Width = squareLength - .5;
                    rectangle.Height = squareLength - .5;

                    Canvas.SetLeft(rectangle, originalOffset + j * squareLength);
                    Canvas.SetTop(rectangle, verticalOffset + i * squareLength);

                    // Add the Ellipse to the StackPanel.
                    canvas.Children.Add(rectangle);

                    var text = new TextBlock();
                    text.Foreground = Brushes.Black;
                    text.Text = rowDescriptor.BlockSizes[j].ToString();
                    Canvas.SetTop(text, verticalOffset + i * squareLength);
                    Canvas.SetLeft(text, horizontalTextOffset + originalOffset + j * squareLength);
                    canvas.Children.Add(text);
                }
            }
            for (int i = 0; i < columns; i++)
            {
                var rowDescriptor = nonogram.ColumnDescriptors[i];

                int originalOffset = verticalOffset - rowDescriptor.BlockSizes.Count * squareLength;

                for (int j = 0; j < rowDescriptor.BlockSizes.Count; j++)
                {
                    var rectangle = new Rectangle();
                    rectangle.StrokeThickness = 1;
                    rectangle.Stroke = Brushes.Black;

                    // Set the width and height of the Ellipse.
                    rectangle.Width = squareLength - .5;
                    rectangle.Height = squareLength - .5;

                    Canvas.SetLeft(rectangle, horizontalOffset + i * squareLength);
                    Canvas.SetTop(rectangle, originalOffset + j * squareLength);

                    // Add the Ellipse to the StackPanel.
                    canvas.Children.Add(rectangle);

                    var text = new TextBlock();
                    text.Foreground = Brushes.Black;
                    text.Text = rowDescriptor.BlockSizes[j].ToString();
                    Canvas.SetTop(text, originalOffset + j * squareLength);
                    Canvas.SetLeft(text, horizontalTextOffset + horizontalOffset + i * squareLength);
                    canvas.Children.Add(text);
                }
            }
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

        private static Nonogram genNonogram(int rows, int columns)
        {
            var nonogram = new Nonogram(columns, rows);

            var rand = new Random();
            var a = (Array)(new int[] { 5 });


            foreach (var r in nonogram.Cells)
            {
                foreach (var cell in r)
                {
                    cell.State = rand.NextDouble() > 0.5
                        ? Solver.CellState.Filled
                        : Solver.CellState.Empty;
                }
            }

            var rowsList = new List<RowDescriptor>(rows);
            for (int i = 0; i < rows; i++)
            {
                var row = nonogram.getRow(i);
                Solver.RowDescriptor rowDescriptor = MakeRowDescriptorFor(row);
                rowsList.Add(rowDescriptor);
            }
            nonogram.RowDescriptors = rowsList;

            var columnList = new List<Solver.RowDescriptor>(rows);
            for (int i = 0; i < columns; i++)
            {
                var column = nonogram.getColumn(i);
                Solver.RowDescriptor rowDescriptor = MakeRowDescriptorFor(column);
                columnList.Add(rowDescriptor);
            }
            nonogram.ColumnDescriptors = columnList;
            return nonogram;
        }

        private static Solver.RowDescriptor MakeRowDescriptorFor(Solver.Cell[] row)
        {

            var rowDescriptor = new Solver.RowDescriptor();
            int cellCounter = 0;
            foreach (var cell in row)
            {
                if (cell.State == Solver.CellState.Filled)
                {
                    cellCounter++;
                }
                else if (cell.State == Solver.CellState.Empty
                   && cellCounter > 0)
                {
                    rowDescriptor.BlockSizes.Add(cellCounter);
                    cellCounter = 0;
                }
            }
            if (row[row.Length - 1].State == Solver.CellState.Filled)
            {
                rowDescriptor.BlockSizes.Add(cellCounter);
            }
            Trace.WriteLine("for row: " + RowToString(row) + ": " + String.Join(",", rowDescriptor.BlockSizes));
            return rowDescriptor;
        }

        private static string RowToString(IEnumerable<Cell> row)
        {
            return new string(row.Select(CharForCell).ToArray());
        }

        private static char CharForCell(Cell state)
        {
            switch (state.State)
            {
                case CellState.Empty:
                    return '.';
                case CellState.Filled:
                    return 'X';
                case CellState.Undefined:
                    return ' ';
                default:
                    throw new ArgumentException("state value: " + state);
            }
        }
    }
}

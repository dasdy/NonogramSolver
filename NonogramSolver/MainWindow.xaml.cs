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

            var content = (StackPanel)this.Content;
            var stackPanel = ((StackPanel)content.Children[0]);
            var nonogramControl = (NonogramView)stackPanel.Children[0];
            nonogramControl.Nonogram = nonogram;

            var emptiedNonogram = CloneNonogram(nonogram);
            TrySolve(emptiedNonogram);

            var emptiedControl = (NonogramView)stackPanel.Children[1];
            emptiedControl.Nonogram = emptiedNonogram;
            
            
            
            emptiedControl.PropertyChanged += (sender, args) =>
            {
                UpdateTextBox(((NonogramView)sender).Nonogram);
            };

        }

        private void TrySolve(Nonogram n)
        {
            var solver = new Solver.Solver();
            for(int i = 0; i < n.Height; i++)
            {
                var descriptor = n.RowDescriptors[i];
                var row = n.getRow(i);
                solver.FillInvariantCells(row, descriptor);
            }
            for (int i = 0; i < n.Width; i++)
            {
                var descriptor = n.ColumnDescriptors[i];
                var col = n.getColumn(i);
                solver.FillInvariantCells(col, descriptor);
            }
        }

        private Nonogram CloneNonogram(Nonogram nonogram)
        {
            var result = new Nonogram(nonogram.Width, nonogram.Height);
            result.RowDescriptors = nonogram.RowDescriptors.ToList();
            result.ColumnDescriptors = nonogram.ColumnDescriptors.ToList();
            return result;
        }

        private void UpdateTextBox(Nonogram n)
        {
            var content = (StackPanel)this.Content;
            var blk = (TextBlock)content.Children[1];
            var solver = new Solver.Solver();
            for(int i = 0; i < n.Height; i++)
            {
                var row = n.getRow(i);
                var rowDesc = n.RowDescriptors[i];
                if(solver.GetRowStatus(row, rowDesc) != RowStatus.FilledCorrectly)
                {
                    blk.Text = "ERRORS FOUND BITHC";
                    return;
                }
            }

            for (int i = 0; i < n.Width; i++)
            {
                var row = n.getColumn(i);
                var rowDesc = n.ColumnDescriptors[i];
                if (solver.GetRowStatus(row, rowDesc) != RowStatus.FilledCorrectly)
                {
                    blk.Text = "ERRORS FOUND BITHC";
                    return;
                }
            }

            blk.Text = "Correct!";
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
            
            return rowDescriptor;
        }

        
    }
}

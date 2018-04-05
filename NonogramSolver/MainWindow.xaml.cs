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
        private readonly Nonogram _solvedNonogram;
        public MainWindow()
        {
            InitializeComponent();
            var rows = 10;
            var columns = 20;

            _solvedNonogram = Solver.NonogramFactory.MakeNonogram(rows, columns);

            var content = (StackPanel)this.Content;
            var stackPanel = ((StackPanel)content.Children[0]);
            var nonogramControl = (NonogramView)stackPanel.Children[0];
            nonogramControl.Nonogram = _solvedNonogram;

            var emptiedNonogram = CloneNonogram(_solvedNonogram);
            var solver = new Solver.Solver();
            solver.Solve(emptiedNonogram);

            var emptiedControl = (NonogramView)stackPanel.Children[1];
            emptiedControl.Nonogram = emptiedNonogram;



            emptiedControl.PropertyChanged += (sender, args) =>
            {
                UpdateTextBox(((NonogramView)sender).Nonogram);
            };
            UpdateTextBox(emptiedNonogram);
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
            for (int i = 0; i < n.Height; i++)
            {
                for (int j = 0; j < n.Width; j++)
                {
                    var expected = _solvedNonogram.Cells[i][j].State;
                    var real = n.Cells[i][j].State;
                    var solvingStatus = (solver.GetRowStatus(n.getRow(i), n.RowDescriptors[i]) == RowStatus.ContainsErrors
                            || solver.GetRowStatus(n.getColumn(j), n.ColumnDescriptors[j]) == RowStatus.ContainsErrors);
                    if (real != CellState.Undefined
                        && expected != real
                        && solvingStatus)
                    {
                        blk.Text = $"found error at: row {i}, col {j}";
                        return;
                    }
                }
            }
            blk.Text = "no errors hooray";
        }
    }
}

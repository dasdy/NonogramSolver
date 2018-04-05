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

            var stackPanel = ((WrapPanel)this.Content);
            var nonogramControl = (NonogramView)stackPanel.Children[0];
            nonogramControl.Nonogram = _solvedNonogram;

            var emptiedNonogram = _solvedNonogram.Clone() as Nonogram;
            emptiedNonogram.Clear();
            var solver = new Solver.Solver();
            solver.Solve(emptiedNonogram);

            var emptiedControl = (NonogramView)stackPanel.Children[1];
            emptiedControl.Nonogram = emptiedNonogram;


            var solvedFullyControl = (NonogramView)stackPanel.Children[2];
            Nonogram solvedFullyNonogram = emptiedNonogram.Clone() as Nonogram;
            solvedFullyControl.Nonogram = solvedFullyNonogram;

            solver.RecursivelySolve(solvedFullyNonogram);
            solvedFullyControl.Nonogram = solvedFullyNonogram;



            var acSolvedControl = (NonogramView)stackPanel.Children[3];
            Nonogram acSolvedNonogram = emptiedNonogram.Clone() as Nonogram;
            solvedFullyControl.Nonogram = solvedFullyNonogram;

            Task.Run(() =>
            {
                Task.Delay(2000);
                var acSolver = new ArcConsistencySolver();
                acSolver.Solve(acSolvedNonogram);
            }).ContinueWith((tsk) =>
            {
                acSolvedControl.Nonogram = acSolvedNonogram;
                UpdateTextBox(acSolvedNonogram);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            acSolvedControl.PropertyChanged += (sender, args) =>
            {
                UpdateTextBox(((NonogramView)sender).Nonogram);
            };
            UpdateTextBox(acSolvedNonogram);
        }

        private void UpdateTextBox(Nonogram n)
        {
            var content = (WrapPanel)this.Content;
            var blk = (TextBlock)content.Children[content.Children.Count - 1];
            var solver = new Solver.Solver();
            for (int i = 0; i < n.Height; i++)
            {
                for (int j = 0; j < n.Width; j++)
                {
                    var expected = _solvedNonogram.Cells[i][j].State;
                    var real = n.Cells[i][j].State;
                    if (solver.GetRowStatus(n.getRow(i), n.RowDescriptors[i]) == RowStatus.ContainsErrors)
                    {
                        blk.Text = $"found error at: row {i}";
                        return;
                    }
                    
                    if (solver.GetRowStatus(n.getColumn(j), n.ColumnDescriptors[j]) == RowStatus.ContainsErrors)
                    {
                        blk.Text = $"found error at: col {j}";
                        return;
                    }
                }
            }
            blk.Text = "no errors hooray";
        }
    }
}

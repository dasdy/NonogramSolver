using NonogramSolver.Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NonogramSolver.Tests
{
    public class RandomSolverTests
    {

        private class Solvers : IEnumerable<object[]>
        {
            private List<ISolver> data = new List<ISolver>()
            {
                new ContradictionSolver(),
                new ArcConsistencySolver(),
                new RecursiveSolver(),
            };
            public IEnumerator<object[]> GetEnumerator() => data.Select(x => new object[] { x }).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private const int tries = 20;
        [Theory]
        [ClassData(typeof(Solvers))]
        public void RandomSolvingTests(ISolver solver)
        {
            for (int run = 0; run < tries; run++)
            {
                var nonogram = NonogramFactory.MakeNonogram(10, 10);
                nonogram.Clear();
                solver.Solve(nonogram);

                for (int i = 0; i < nonogram.Width; i++)
                {
                    var row = nonogram.getColumn(i);
                    Assert.NotEqual(RowStatus.ContainsErrors, Utils.GetRowStatus(row, nonogram.ColumnDescriptors[i]));
                }

                for (int i = 0; i < nonogram.Height; i++)
                {
                    var row = nonogram.getRow(i);
                    Assert.NotEqual(RowStatus.ContainsErrors, Utils.GetRowStatus(row, nonogram.RowDescriptors[i]));
                }
            }
        }
    }
}

using NonogramSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace NonogramSolver.Tests
{
    public class NonogramTests
    {
        private const int RowCount = 6;
        private const int ColumnCount = 5;

        [Fact]
        public void nonogram_creates_correct_field()
        {
            var n = new Nonogram(ColumnCount, RowCount);

            Assert.Equal(RowCount, n.Cells.Count);
            Assert.All<List<Cell>>(n.Cells, row =>
            {

                Assert.Equal(ColumnCount, row.Count);
                Assert.All<Cell>(row, c =>
                {
                    Assert.NotNull(c);
                    Assert.Equal(CellState.Undefined, c.State);
                });

            });

        }

        [Fact]
        public void getRow_returns_row()
        {
            var n = new Nonogram(ColumnCount, RowCount);
            for (int i = 0; i < RowCount; i++)
            {
                var row = n.getRow(i);
                for(int j = 0; j < ColumnCount; j++)
                {
                    Assert.Same(n.Cells[i][j], row[j]);
                }
            }
        }

        [Fact]
        public void getColumn_returns_row()
        {
            var n = new Nonogram(ColumnCount, RowCount);
            for (int i = 0; i < ColumnCount; i++)
            {
                var row = n.getColumn(i);
                for (int j = 0; j < RowCount; j++)
                {
                    Assert.Same(n.Cells[j][i], row[j]);
                }
            }
        }
    }
}

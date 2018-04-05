using System;
using System.Collections.Generic;

namespace NonogramSolver.Solver
{
    public class NonogramFactory
    {

        public static Nonogram MakeNonogram(int rows, int columns)
        {
            var nonogram = new Nonogram(columns, rows);

            var rand = new Random();
            var a = (Array)(new int[] { 5 });


            foreach (var r in nonogram.Cells)
            {
                foreach (var cell in r)
                {
                    cell.State = rand.NextDouble() > 0.5
                        ? CellState.Filled
                        : CellState.Empty;
                }
            }

            var rowsList = new List<RowDescriptor>(rows);
            for (int i = 0; i < rows; i++)
            {
                var row = nonogram.getRow(i);
                RowDescriptor rowDescriptor = MakeRowDescriptorFor(row);
                rowsList.Add(rowDescriptor);
            }
            nonogram.RowDescriptors = rowsList;

            var columnList = new List<RowDescriptor>(rows);
            for (int i = 0; i < columns; i++)
            {
                var column = nonogram.getColumn(i);
                RowDescriptor rowDescriptor = MakeRowDescriptorFor(column);
                columnList.Add(rowDescriptor);
            }
            nonogram.ColumnDescriptors = columnList;
            return nonogram;
        }

        private static RowDescriptor MakeRowDescriptorFor(Cell[] row)
        {

            var rowDescriptor = new RowDescriptor();
            int cellCounter = 0;
            foreach (var cell in row)
            {
                if (cell.State == CellState.Filled)
                {
                    cellCounter++;
                }
                else if (cell.State == CellState.Empty
                   && cellCounter > 0)
                {
                    rowDescriptor.BlockSizes.Add(cellCounter);
                    cellCounter = 0;
                }
            }
            if (row[row.Length - 1].State == CellState.Filled)
            {
                rowDescriptor.BlockSizes.Add(cellCounter);
            }

            return rowDescriptor;
        }
    }
}

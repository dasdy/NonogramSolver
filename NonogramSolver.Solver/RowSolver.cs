using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class RowSolver
    {
        public RowStatus GetRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {
            
            if (row.All(cell => cell.State != CellState.Undefined))
            {
                return CheckFilledRowStatus(row, rowDescriptor);
            } else
            {
                return CheckPartialRowStatus(row, rowDescriptor);
            }
        }

        private RowStatus CheckPartialRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {
            throw new NotImplementedException();
        }

        private static RowStatus CheckFilledRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {
            int rowIndex = 0;
            int blockIndex = 0;
            for (blockIndex = 0; blockIndex < rowDescriptor.BlockSizes.Count; blockIndex++)
            {
                int blockSize = rowDescriptor.BlockSizes[blockIndex];
                //skip empty cells
                while (rowIndex < row.Count && row[rowIndex].State == CellState.Empty)
                {
                    rowIndex++;
                }
                int foundBlockSize = 0;
                while (rowIndex < row.Count && row[rowIndex].State == CellState.Filled)
                {
                    rowIndex++;
                    foundBlockSize++;
                }
                if (foundBlockSize != blockSize)
                {
                    return RowStatus.ContainsErrors;
                }
                rowIndex++;
            }
            return RowStatus.FilledCorrectly;
        }

        /// <summary>
        /// fill cells in row that are guaranteed to be filled. for example,
        /// in row length 5, blocks 2,2 would guarantee full row to be filled
        /// </summary>
        /// <param name="row">row that should be filled if possible(will be mutated)</param>
        public void FillInvariantCells(IList<Cell> row, RowDescriptor rowDescriptor)
        {
            var rowLength = row.Count;
            //blocks that should be filled in the row with their empty neighbors
            var busyBlocks = rowDescriptor.BlockSizes.Count;
            foreach (var i in rowDescriptor.BlockSizes)
            {
                busyBlocks += i;
            }
            busyBlocks--;
            if (busyBlocks >= rowLength / 2)
            {
                //cells that are undefined from each side of the block
                int cutAmount = row.Count - busyBlocks;

                int position = 0;
                foreach (var blockLength in rowDescriptor.BlockSizes)
                {
                    position += cutAmount;
                    if (blockLength > cutAmount)
                    {
                        //cells that should be filled from this block
                        var blockPartToFill = blockLength - cutAmount;
                        for (int i = 0; i < blockPartToFill; i++)
                        {
                            row[position + i].State = CellState.Filled;
                        }
                        position += blockPartToFill;
                        if (cutAmount == 0 && position < row.Count)
                        {
                            row[position].State = CellState.Empty;
                        }
                        position++;
                    }
                }
            }
        }
    }
}

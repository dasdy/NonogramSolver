using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class Utils
    {
        public static List<List<List<CellState>>> PossibleStatesForRows(int rowLength, IEnumerable<RowDescriptor> rowDescriptors)
        {
            return rowDescriptors.Select(desc => Utils.MakePossibleStates(rowLength, desc).Select(x => x.ToList()).ToList()).ToList();
        }

        public static IEnumerable<IEnumerable<CellState>> MakePossibleStates(int rowLength, RowDescriptor rowDescriptor)
        {
            int filledCells = rowDescriptor.BlockSizes.Sum();
            var blocks = rowDescriptor.BlockSizes.Select(blockSize => Enumerable.Repeat(CellState.Filled, blockSize));
            return GeneratePermutations(blocks, rowLength - filledCells + 1);
        }
        public static RowStatus GetRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {

            if (row.All(cell => cell.State != CellState.Undefined))
            {
                return CheckFilledRowStatus(row.Select(x => x.State).ToList(), rowDescriptor);
            }
            else
            {
                return CheckPartialRowStatus(row.Select(x => x.State).ToList(), rowDescriptor);
            }
        }


        public static RowStatus GetRowStatus(IList<CellState> row, RowDescriptor rowDescriptor)
        {

            if (row.All(cell => cell != CellState.Undefined))
            {
                return CheckFilledRowStatus(row, rowDescriptor);
            }
            else
            {
                return CheckPartialRowStatus(row, rowDescriptor);
            }
        }

        // checks only until first undefined is found
        private static RowStatus CheckPartialRowStatus(IList<CellState> row, RowDescriptor rowDescriptor)
        {
            int rowIndex = 0;
            int blockIndex = 0;
            for (blockIndex = 0; blockIndex < rowDescriptor.BlockSizes.Count; blockIndex++)
            {
                int blockSize = rowDescriptor.BlockSizes[blockIndex];
                //skip empty cells
                while (rowIndex < row.Count && row[rowIndex] == CellState.Empty)
                {
                    rowIndex++;
                }
                if (rowIndex < row.Count && row[rowIndex] == CellState.Undefined)
                {
                    break;
                }
                int foundBlockSize = 0;
                while (rowIndex < row.Count && row[rowIndex] == CellState.Filled)
                {
                    rowIndex++;
                    foundBlockSize++;
                }
                if (foundBlockSize != blockSize)
                {
                    //maybe unfinished block?
                    if (rowIndex < row.Count && row[rowIndex] == CellState.Undefined
                        && foundBlockSize < blockSize)
                    {
                        break;
                    }
                    return RowStatus.ContainsErrors;
                }
                rowIndex++;
            }
            
            return RowStatus.FilledPartially;
        }

        private static RowStatus CheckFilledRowStatus(IList<CellState> row, RowDescriptor rowDescriptor)
        {
            int rowIndex = 0;
            int blockIndex = 0;
            for (blockIndex = 0; blockIndex < rowDescriptor.BlockSizes.Count; blockIndex++)
            {
                int blockSize = rowDescriptor.BlockSizes[blockIndex];
                //skip empty cells
                while (rowIndex < row.Count && row[rowIndex] == CellState.Empty)
                {
                    rowIndex++;
                }
                int foundBlockSize = 0;
                while (rowIndex < row.Count && row[rowIndex] == CellState.Filled)
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

        private static IEnumerable<IEnumerable<CellState>> GeneratePermutations(IEnumerable<IEnumerable<CellState>> blocks, int zerosAmount, int startCounter = 0)
        {
            if (blocks.Count() == 0)
            {
                if (zerosAmount > 0)
                {
                    yield return Enumerable.Repeat(CellState.Empty, zerosAmount - 1);
                }
                yield break;
            }
            for (int x = startCounter; x < zerosAmount - blocks.Count() + startCounter + 1; x++)
            {
                var tail = blocks.Skip(1);
                foreach (var generatedTail in GeneratePermutations(tail, zerosAmount - x, 1))
                {
                    var newState = Enumerable.Repeat(CellState.Empty, x).ToList();
                    newState.AddRange(blocks.ElementAt(0));
                    newState.AddRange(generatedTail);
                    yield return newState;
                }
            }
        }

    }
}

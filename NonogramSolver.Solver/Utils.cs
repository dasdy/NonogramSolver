using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    class Utils
    {

        public static IEnumerable<IEnumerable<CellState>> MakePossibleStates(int rowLength, RowDescriptor rowDescriptor)
        {
            int filledCells = rowDescriptor.BlockSizes.Sum();
            var blocks = rowDescriptor.BlockSizes.Select(blockSize => Enumerable.Repeat(CellState.Filled, blockSize));
            return GeneratePermutations(blocks, rowLength - filledCells + 1);
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

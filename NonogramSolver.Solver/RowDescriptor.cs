using System.Collections.Generic;

namespace NonogramSolver.Solver
{
    public class RowDescriptor
    {
        public IList<int> BlockSizes { get; set; }

        public override string ToString()
        {
            return $"{string.Join(", ", BlockSizes)}";
        }
    }
}
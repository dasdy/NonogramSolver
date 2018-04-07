using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public interface ISolver
    {
        void Solve(Nonogram n);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class RecursiveSolver
    {
        public void Solve(Nonogram n)
        {
            var rowsPossibleStates = Utils.PossibleStatesForRows(n.Width, n.RowDescriptors);
            var colsPossibleStates = Utils.PossibleStatesForRows(n.Height, n.ColumnDescriptors);
            var initWorkingCopy = new WorkingCopy()
            {
                AssignedRows = Enumerable.Repeat<List<CellState>>(null, n.Height).ToList()
            };
            var solution = FindSolution(initWorkingCopy, n, rowsPossibleStates);
            if (solution == null)
            {
                throw new Exception("wtf??????????/");
            }
            for(int i = 0; i < n.Height; i++)
            {
                for(int j = 0; j < n.Width; j++)
                {
                    n.Cells[i][j].State = solution[i][j];
                }
            }
        }

        private bool containsErrors(WorkingCopy assignments, Nonogram n)
        {
            for (int i = 0; i < n.Width; i++)
            {

                var createdColumn = assignments.AssignedRows.Select(x => x == null ? CellState.Undefined : x[i]).ToList();

                if (Utils.GetRowStatus(createdColumn, n.ColumnDescriptors[i]) == RowStatus.ContainsErrors)
                {
                    return true;
                }
            }
            return false;
        }

        private List<List<CellState>> FindSolution(WorkingCopy assignments, Nonogram n, List<List<List<CellState>>> cellStates)
        {
            int nullIndex = assignments.AssignedRows.FindIndex(x => x == null);
            if (nullIndex < 0)
            {
                if(!containsErrors(assignments, n))
                {
                    return assignments.AssignedRows;
                } else
                {
                    return null;
                }
            }
            var rowVariants = cellStates[nullIndex];
            var workingCopy = assignments.Clone() as WorkingCopy;
            foreach(var value in rowVariants)
            {
                workingCopy.AssignedRows[nullIndex] = value;
                if (containsErrors(assignments, n))
                    continue;
                var resultsSearch = FindSolution(workingCopy, n, cellStates);
                if (resultsSearch != null)
                {
                    return resultsSearch;
                }
            }
            return null;
        }

        private class WorkingCopy: ICloneable
        {
            public List<List<CellState>> AssignedRows;

            public object Clone()
            {
                return new WorkingCopy()
                {
                    AssignedRows = new List<List<CellState>>(this.AssignedRows)
                };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class Solver
    {
        public RowStatus GetRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {

            if (row.All(cell => cell.State != CellState.Undefined))
            {
                return CheckFilledRowStatus(row, rowDescriptor);
            }
            else
            {
                return CheckPartialRowStatus(row, rowDescriptor);
            }
        }

        private RowStatus CheckPartialRowStatus(IList<Cell> row, RowDescriptor rowDescriptor)
        {
            return RowStatus.FilledPartially;
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


        public void RecursivelySolve(Nonogram n)
        {
            var rowsPossibleStates = n.RowDescriptors.Select(desc => Utils.MakePossibleStates(n.Width, desc).Select(x => x.ToList()).ToList()).ToList();
            var colsPossibleStates = n.ColumnDescriptors.Select(desc => Utils.MakePossibleStates(n.Height, desc).Select(x => x.ToList()).ToList()).ToList();
            List<List<CellState>> rowSolvingCandidates = null;
            List<List<CellState>> colSolvingCandidates = null;

            bool fullySolved = false;
            while (rowsPossibleStates.All(rowStates => rowStates.Count > 0)
                && colsPossibleStates.All(columnStates => columnStates.Count > 0)
                && !fullySolved)
            {
                rowSolvingCandidates = rowsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
                colSolvingCandidates = colsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
                bool changeFound = true;
                while (changeFound)
                {
                    changeFound = false;
                    for (int i = 0; i < rowSolvingCandidates.Count; i++)
                    {
                        var rowCandidate = rowSolvingCandidates[i];

                        int rowsCandidatesRemoved = RemoveConflictingStates(rowCandidate, i, colsPossibleStates);
                        if (rowsCandidatesRemoved > 0)
                        {
                            changeFound = true;
                        }
                    }
                    if (colsPossibleStates.Any(columnStates => columnStates.Count == 0))
                    {
                        break;
                    }
                    colSolvingCandidates = colsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
                    for (int i = 0; i < colSolvingCandidates.Count; i++)
                    {
                        var rowCandidate = colSolvingCandidates[i];

                        int rowsCandidatesRemoved = RemoveConflictingStates(rowCandidate, i, rowsPossibleStates);
                        if (rowsCandidatesRemoved > 0)
                        {
                            changeFound = true;
                        }
                    }
                    if (rowsPossibleStates.Any(rowStates => rowStates.Count == 0))
                    {
                        break;
                    }
                    rowSolvingCandidates = rowsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
                    
                }
                fullySolved = rowSolvingCandidates.All(rowState => rowState.All(cell => cell != CellState.Undefined)) &&
                    colSolvingCandidates.All(rowState => rowState.All(cell => cell != CellState.Undefined));
                if (!fullySolved)
                {
                    //find row with several candidates left and remove all others
                    foreach(var colStates in colsPossibleStates)
                    {
                        if (colStates.Count > 1)
                        {
                            colStates.RemoveRange(1, colStates.Count - 1);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < n.Height; i++)
            {
                var row = n.getRow(i);
                var commonCells = rowSolvingCandidates[i];
                for (int j = 0; j < commonCells.Count; j++)
                {
                    row[j].State = commonCells[j];
                }

            }
            for (int i = 0; i < n.Width; i++)
            {
                var col = n.getColumn(i);
                var commonCells = colSolvingCandidates[i];
                for (int j = 0; j < commonCells.Count; j++)
                {
                    col[j].State = commonCells[j];
                }
            }
        }

        public int RemoveConflictingStates(IEnumerable<CellState> rowState, int rowNumber, List<List<List<CellState>>> columnsStates)
        {
            int removed = 0;
            for (int j = 0; j < columnsStates.Count; j++)
            {
                if (rowState.ElementAt(j) != CellState.Undefined)
                {
                    removed += columnsStates[j].RemoveAll(state => state[rowNumber] != rowState.ElementAt(j));
                }
            }
            return removed;
        }

        public IEnumerable<CellState> FindCommonCells(IEnumerable<IEnumerable<CellState>> rowStates)
        {

            var res = Enumerable.Repeat((int)(CellState.Filled | CellState.Empty),
                rowStates.First().Count()).ToList();
            foreach (var rowState in rowStates)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    var rowStateCellValue = rowState.ElementAt(i);

                    res[i] = res[i] & (int)rowStateCellValue;
                }
            }
            return res.Select(x => (CellState)x);
        }

        public void Solve(Nonogram n)
        {
            var rowsPossibleStates = n.RowDescriptors.Select(desc => Utils.MakePossibleStates(n.Width, desc).Select(x => x.ToList()).ToList()).ToList();
            var colsPossibleStates = n.ColumnDescriptors.Select(desc => Utils.MakePossibleStates(n.Height, desc).Select(x => x.ToList()).ToList()).ToList();
            var rowSolvingCandidates = rowsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
            var colSolvingCandidates = colsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();

            bool changeFound = true;
            while (changeFound)
            {
                changeFound = false;
                for (int i = 0; i < rowSolvingCandidates.Count; i++)
                {
                    var rowCandidate = rowSolvingCandidates[i];

                    int rowsCandidatesRemoved = RemoveConflictingStates(rowCandidate, i, colsPossibleStates);
                    if (rowsCandidatesRemoved > 0)
                    {
                        changeFound = true;
                    }
                }
                colSolvingCandidates = colsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
                for (int i = 0; i < colSolvingCandidates.Count; i++)
                {
                    var rowCandidate = colSolvingCandidates[i];

                    int rowsCandidatesRemoved = RemoveConflictingStates(rowCandidate, i, rowsPossibleStates);
                    if (rowsCandidatesRemoved > 0)
                    {
                        changeFound = true;
                    }
                }
                rowSolvingCandidates = rowsPossibleStates.Select(rowStates => FindCommonCells(rowStates).ToList()).ToList();
            }

            for (int i = 0; i < n.Height; i++)
            {
                var row = n.getRow(i);
                var commonCells = rowSolvingCandidates[i];
                for (int j = 0; j < commonCells.Count; j++)
                {
                    row[j].State = commonCells[j];
                }

            }
            for (int i = 0; i < n.Width; i++)
            {
                var col = n.getColumn(i);
                var commonCells = colSolvingCandidates[i];
                for (int j = 0; j < commonCells.Count; j++)
                {
                    col[j].State = commonCells[j];
                }
            }
        }
    }
}

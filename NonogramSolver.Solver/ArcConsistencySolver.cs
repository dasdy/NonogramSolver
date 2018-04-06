using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class ArcConsistencySolver
    {
        public void Solve(Nonogram n)
        {
            var rowVars = n.RowDescriptors.Select((desc, i) =>
                new Variable()
                {
                    PossibleStates = Utils.MakePossibleStates(n.Width, desc).Select(x => x.ToList()).ToList(),
                    Index = i,
                    ColType = ColumnType.Row
                }
            ).ToList();
            var colVars = n.ColumnDescriptors.Select((desc, i) => new Variable()
            {
                PossibleStates = Utils.MakePossibleStates(n.Height, desc).Select(x => x.ToList()).ToList(),
                Index = i,
                ColType = ColumnType.Column
            }).ToList();

            var allVars = rowVars.Concat(colVars).ToList();

            var workList = new Queue<Constraint>();
            foreach (var a in allVars)
            {
                foreach (var b in allVars)
                {
                    if (a != b && FormConstraint(a, b))
                    {
                        workList.Enqueue(new Constraint(a, b));
                    }
                }
            }

            while (workList.Any())
            {
                var item = workList.Dequeue();
                if (ArcReduce(item.x, item.y))
                {
                    if (!item.x.PossibleStates.Any())
                    {
                        throw new Exception($"no items left in: {item.x.ColType} {item.x.Index}");
                    }
                    foreach (var v in allVars)
                    {

                        if (item.x != v 
                            && item.y != v 
                            && FormConstraint(item.x, v))
                        {
                            workList.Enqueue(new Constraint(v, item.x));
                        }
                    }
                }
            }

            foreach (var v in allVars)
            {
                if (v.ColType == ColumnType.Row)
                {
                    
                    if (v.PossibleStates.Count > 1)
                    {
                        var solver = new Solver();
                        var state = solver.FindCommonCells(v.PossibleStates).ToList();
                        for (int i = 0; i < state.Count; i++)
                        {
                            n.Cells[v.Index][i].State = state[i];
                        }
                    }
                    else
                    {
                        var state = v.PossibleStates.First();
                        for (int i = 0; i < state.Count; i++)
                        {
                            n.Cells[v.Index][i].State = state[i];
                        }
                    }
                }
            }
        }

        private bool ArcReduce(Variable x, Variable y)
        {
            var changed = false;

            x.PossibleStates = x.PossibleStates.Where(vx =>
            {
                var satisfiesY = y.PossibleStates.Any(vy => SatisfyConstraint(x, y, vx, vy));
                if (!satisfiesY)
                {
                    changed = true;
                }
                return satisfiesY;
            }).ToList();
                
            return changed;
        }

        private class Constraint
        {
            public Constraint(Variable x, Variable y)
            {
                this.x = x;
                this.y = y;
            }
            public Variable x { get; }
            public Variable y { get; }
        }

        private bool SatisfyConstraint(Variable a, Variable b, List<CellState> va, List<CellState> vb)
        {
            return va[b.Index] == vb[a.Index];
        }

        // check if any values in a and b's domains contradict each other
        private bool FormConstraint(Variable a, Variable b)
        {
            if (a.ColType == b.ColType)
            {
                return false;
            }
            foreach (var va in a.PossibleStates)
            {
                foreach (var vb in b.PossibleStates)
                {
                    if (!SatisfyConstraint(a, b, va, vb))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private class Variable
        {
            public List<List<CellState>> PossibleStates { get; set; }
            public int Index { get; set; }
            public ColumnType ColType { get; set; }
        }
        private enum ColumnType { Column, Row };
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NonogramSolver.Solver
{
    public class ArcConsistencySolver:ISolver
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
            var rand = new Random();
            int oneRun = 0;
            while (allVars.Any(v => v.PossibleStates.Count > 1) && oneRun < 4)
            {
                oneRun++;
                TrySolve(allVars);
                var sortedByStatesCount = allVars
                    .OrderBy(x => x.PossibleStates.Count)
                    .Where(x => x.PossibleStates.Count > 1)
                    .ToList();

                
                if (!sortedByStatesCount.Any())
                {
                    break;
                }
                var unsolvedVar = sortedByStatesCount.FirstOrDefault();
                var states = new List<List<CellState>>(unsolvedVar.PossibleStates);
                var allVarsClone = allVars.Select(v => v.Clone() as Variable).ToList();
                var unsolvedVarClone = allVarsClone.Find(x => x.ColType == unsolvedVar.ColType 
                                                                && x.Index == unsolvedVar.Index);
                var possibleStatesLeft = new List<List<CellState>>();
                foreach (var st in states)
                {
                    unsolvedVarClone.PossibleStates = new List<List<CellState>> { st };
                    TrySolve(allVarsClone);
                    if (allVarsClone.All(v => v.PossibleStates.Count == 1))
                    {
                        allVars = allVarsClone;
                        break;
                    }
                    else if (allVarsClone.Any(v => !v.PossibleStates.Any()))
                    {
                        continue;
                    }
                    else
                    {
                        possibleStatesLeft.Add(st);
                    }
                }
                if (unsolvedVar.PossibleStates.Count != possibleStatesLeft.Count
                    && allVars.Any(v => v.PossibleStates.Count != 1))
                {
                    unsolvedVarClone.PossibleStates = possibleStatesLeft;
                    break;
                }
            }

            foreach (var v in allVars)
            {
                if (v.ColType == ColumnType.Row)
                {
                    if (v.PossibleStates.Count == 1)
                    {
                        var state = v.PossibleStates.First();
                        for (int i = 0; i < state.Count; i++)
                        {
                            n.Cells[v.Index][i].State = state[i];
                        }
                    } else
                    {
                        var solver = new ContradictionSolver();
                        var state = solver.FindCommonCells(v.PossibleStates).ToList();
                        for (int i = 0; i < state.Count; i++)
                        {
                            n.Cells[v.Index][i].State = state[i];
                        }
                        Trace.WriteLine($"at: {v.ColType} {v.Index}, possible states: {v.PossibleStates.Count}");
                    }
                }
            }
        }

        private void TrySolve(List<Variable> allVars)
        {
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

        private class Variable : ICloneable
        {
            public List<List<CellState>> PossibleStates { get; set; }
            public int Index { get; set; }
            public ColumnType ColType { get; set; }
            public object Clone()
            {
                return new Variable()
                {
                    Index = this.Index,
                    ColType = this.ColType,
                    PossibleStates = new List<List<CellState>>(this.PossibleStates)
                };
            }
        }
        private enum ColumnType { Column, Row };
    }
}

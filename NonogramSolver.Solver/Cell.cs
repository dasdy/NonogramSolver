namespace NonogramSolver.Solver
{
    public class Cell
    {
        public CellState State { get; set; } = CellState.Undefined;


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otherCell = obj as Cell;
            return this.State == otherCell.State;
        }


        public override int GetHashCode()
        {
            return this.State.GetHashCode();
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}

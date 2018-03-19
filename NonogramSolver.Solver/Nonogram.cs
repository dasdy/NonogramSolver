using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramSolver.Solver
{
    public class Nonogram
    {
        public Nonogram(int width, int height)
        {
            this.Cells = new List<List<Cell>>(height);
            this.Width = width;
            this.Height = height;
            for(int i = 0; i < Height; i++)
            {
                var cellArr = new List<Cell>(width);
                for(int j = 0; j < Width; j++)
                {
                    cellArr.Add(new Cell());
                }
                this.Cells.Add(cellArr);
            }
        }

        public IList<RowDescriptor> ColumnDescriptors { get; set; }
        public IList<RowDescriptor> RowDescriptors { get; set; }

        public List<List<Cell>> Cells { get; private set; }
        public int Width { get; }
        public int Height { get; }

        public Cell[] getRow(int rowIndex)
        {
            return Cells[rowIndex].ToArray();
        }

        public Cell[] getColumn(int columnIndex)
        {
            return Cells.Select(row => row[columnIndex]).ToArray();
        }
    }
}

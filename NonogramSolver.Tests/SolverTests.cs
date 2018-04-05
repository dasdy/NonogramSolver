using NonogramSolver.Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static NonogramSolver.Solver.Cell;

namespace NonogramSolver.Tests
{
    public class SolverTests
    {
        private IList<Cell> MakeEmptyRow(int length)
        {
            var result = new List<Cell>(length);
            for (int i = 0; i < length; i++)
            {
                result.Add(new Cell() { State = CellState.Undefined });
            }
            return result;
        }

        private IList<Cell> MakeList(params CellState[] cellStates)
        {
            var result = new List<Cell>(cellStates.Length);
            for (int i = 0; i < cellStates.Length; i++)
            {
                result.Add(new Cell() { State = cellStates[i] });
            }
            return result;
        }

        private RowDescriptor MakeDescriptor(params int[] ranges)
        {

            return new RowDescriptor
            {
                BlockSizes = ranges.ToList()
            };
        }

        [Theory]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Filled, CellState.Filled, CellState.Filled }, RowStatus.FilledCorrectly)]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Empty, CellState.Filled, CellState.Filled }, RowStatus.ContainsErrors)]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Empty, CellState.Filled, CellState.Empty }, RowStatus.ContainsErrors)]
        [InlineData(new int[] { 1, 1 }, new CellState[] { CellState.Filled, CellState.Empty, CellState.Filled }, RowStatus.FilledCorrectly)]
        [InlineData(new int[] { 1, 1 }, new CellState[] { CellState.Filled, CellState.Filled, CellState.Filled }, RowStatus.ContainsErrors)]
        public void calculate_row_status(int[] ranges, CellState[] states, RowStatus resultStatus)
        {
            var row = MakeList(states);
            var solver = new Solver.Solver();
            var descriptor = MakeDescriptor(ranges);

            Assert.Equal(resultStatus, solver.GetRowStatus(row, descriptor));
        }

        private char CharForCell(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    return '.';
                case CellState.Filled:
                    return 'X';
                case CellState.Undefined:
                    return ' ';
                default:
                    throw new ArgumentException("state value: " + state);
            }
        }

        private CellState CellStateForChar(char ch)
        {
            switch (ch)
            {
                case '.':
                    return CellState.Empty;
                case 'X':
                    return CellState.Filled;
                case ' ':
                    return CellState.Undefined;
                default:
                    throw new ArgumentException("char: " + ch);
            }
        }

        private string RowToString(IEnumerable<CellState> row)
        {
            return new string(row.Select(CharForCell).ToArray());
        }

        [Theory]
        [InlineData(3, new int[] { 1 }, new string[] { "X..", ".X.", "..X" })]
        [InlineData(4, new int[] { 2 }, new string[] { "XX..", ".XX.", "..XX" })]
        [InlineData(7, new int[] { 1, 2 }, new string[] { "X.XX...", "X..XX..", "X...XX.", "X....XX",
            ".X.XX..", ".X..XX.", ".X...XX",
            "..X.XX.", "..X..XX",
            "...X.XX"})]
        public void generate_permutations(int rowSize, int[] ranges, string[] expected)
        {
            var solver = new Solver.Solver();
            var descriptor = MakeDescriptor(ranges);
            var result = solver.MakePossibleStates(rowSize, descriptor);
            var stringResult = result.Select(RowToString).ToArray();
            Array.Sort(stringResult);
            Array.Sort(expected);
            Debug.WriteLine(String.Join(",", stringResult));
            Assert.Equal(expected, stringResult);
        }

        [Theory]
        [InlineData(new string[] { "XXX..", ".XXX.", "..XXX" }, "  X  ")]
        public void find_common_cells(string[] state_values, string result)
        {
            var cellStates = state_values.Select(x => x.Select(CellStateForChar));
            var solver = new Solver.Solver();
            var commonRow = solver.FindCommonCells(cellStates);
            Assert.Equal(result, RowToString(commonRow));
        }

        [Theory]

        [ClassData(typeof(RemoveConflictingStatesGenerator))]
        public void remove_conflicting_states(string state, string[][] rowVariants, string[][] expected, int expectedRemoved)
        {
            var cellStates = rowVariants.Select(x => x.Select(y => y.Select(CellStateForChar).ToList()).ToList()).ToList();
            var solver = new Solver.Solver();
            var commonState = state.Select(CellStateForChar);
            var expectedStates = expected.Select(x => x.Select(y => y.Select(CellStateForChar)));
            var result = solver.RemoveConflictingStates(commonState, 2, cellStates);

            Assert.Equal(expectedRemoved, result);
            Assert.Equal(expectedStates, cellStates);
        }

        /// <summary>
        /// shorthand func to avoid loads of new string[][] { new string[] {}}...
        /// </summary>
        /// <returns>same thing passed to arguments</returns>
        private static T[] Arr<T>(params T[] ts)
        {
            return ts;
        }

        private class RemoveConflictingStatesGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>()
            {
                new object[] {"X", Arr<string[]>(Arr( "XXX..", ".XXX.", "X....")),
                                   Arr<string[]>(Arr( "XXX..", ".XXX.")),
                              1},
                new object[] {". ", Arr(Arr( "XXX..", ".XXX.", "X...."),
                                        Arr("XXXXX", ".....")),
                                   Arr(Arr( "X...." ),
                                       Arr("XXXXX", ".....")),
                              2}
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}

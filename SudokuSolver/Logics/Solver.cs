using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SudokuSolver.Logics
{
    public class Solver
    {
        public int[][] Solve(int[][] sudoku)
        {
            int[][] solvedSudoku = SudokuSolverStatic.SudokuSolver.Solve(sudoku);
            return solvedSudoku;
        }

        public int[][] Create(int[][] sudoku)
        {
            int[][] created = SudokuSolverStatic.SudokuSolver.Create(sudoku);
            return created;
        }
    }
}
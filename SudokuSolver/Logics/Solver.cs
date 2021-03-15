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
            /*
            int[][] test = new int[][]{
                new int[]{0,0,0,0,0,0},
                new int[]{0,0, 0, 0 ,0,0},
                new int[]{0,0, 0, 0 ,0,0},
                new int[]{0,0, 0, 0 ,0,0},
                new int[]{0,0, 0, 0 ,0,0},
                new int[]{0,0, 0, 0 ,0,0}
            };
            int[][] solvedSudoku = SudokuSolverStatic.SudokuSolver.Solve(test);
            */
            /**/int[][] solvedSudoku = SudokuSolverStatic.SudokuSolver.Solve(sudoku);
            return solvedSudoku;
        }

        public int[][] Create(int[][] sudoku)
        {
            //int[][] created = SudokuSolverStatic.SudokuSolver.Create();
            return sudoku;
        }
    }
}
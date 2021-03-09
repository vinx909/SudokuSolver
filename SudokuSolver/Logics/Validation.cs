using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SudokuSolver.Logics
{
    public class Validation
    {
        public bool validate(int[][] sudoku)
        {
            for (int i = 0; i < sudoku.Length; i++)
            {
                List<int> numbers = new List<int>();
                for (int j = 0; j < sudoku.Length; j++)
                {
                    if (!numbers.Exists(c => c == sudoku[i][j]))
                    {
                        numbers.Add(sudoku[i][j]);
                    }
                    else { return false; }
                }
            }

            for (int i = 0; i < sudoku.Length; i++)
            {
                List<int> numbers = new List<int>();
                for (int j = 0; j < sudoku.Length; j++)
                {
                    if (!numbers.Exists(c => c == sudoku[j][i]))
                    {
                        numbers.Add(sudoku[j][i]);
                    }
                    else { return false; }
                }
            }


            return true;
        }
    }
}
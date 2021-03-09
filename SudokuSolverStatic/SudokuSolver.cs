using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    public class SudokuSolver
    {
        private const int minNumber = 1;
        private const int maxNumber = 9;

        private const string exceptionWrongFieldLength = "the sudoku field must be 9 by 9";

        public static int[][] Solve(int[][] field)
        {
            if(field.GetLength(0)!=9 || field.GetLength(1) != 9)
            {
                throw new Exception(exceptionWrongFieldLength);
            }
            SudokuField[,] sudoku = FieldIntToField(field);
            return null;
        }
        private static SudokuField[,] FieldIntToField(int[][] field)
        {
            SudokuField[,] sudoku = new SudokuField[maxNumber,maxNumber];
            for(int x = 0; x < field.GetLength(0); x++)
            {
                for(int y=0; y < field.GetLength(1); y++)
                {
                    sudoku[x, y] = new SudokuField(minNumber,maxNumber);
                }
            }
            return sudoku;
        }
    }
}

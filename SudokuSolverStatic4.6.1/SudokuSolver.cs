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
        private const int squareWidth = 3;
        private const int squareHeight = 3;

        private const string exceptionWrongFieldLength = "the sudoku field must be 9 by 9";

        private SudokuField[,] sudoku;
        private readonly List<int> numbersToExclude;
        private readonly List<SudokuField> fieldsToActOn;

        public static int[,] Solve(int[,] field)
        {
            if (field.GetLength(0) != maxNumber || field.GetLength(1) != maxNumber)
            {
                throw new Exception(exceptionWrongFieldLength);
            }
            SudokuSolver solver = new SudokuSolver(field);
            Solve(solver);
            return solver.FieldFieldToInt();
        }
        public static int[][] Solve(int[][] field)
        {
            if (field.Length != maxNumber || field[0].Length != maxNumber)
            {
                throw new Exception(exceptionWrongFieldLength);
            }
            SudokuSolver solver = new SudokuSolver(field);
            Solve(solver);
            return solver.FieldFieldToIntJagged();
        }

        public static void Solve(SudokuSolver solver)
        {
            bool finished = false;

            int solveState = 1;
            bool addition = false;

            while(finished == false)
            {
                switch (solveState)
                {
                    case 0:
                        if (addition == true)
                        {
                            finished = solver.CheckIfFinished();
                            if (finished == true)
                            {
                                return;
                            }
                            else
                            {
                                solveState = 1;
                                break;
                            }
                        }
                        else
                        {
                            solveState++;
                            break; ;
                        }

                    case 1:
                        addition = solver.HorizontalTest();
                        goto case 0;

                    case 2:
                        addition = solver.VerticalTest();
                        goto case 0;
                    case 3:
                        addition = solver.SquareTest();
                        goto case 0;
                    case 4:
                        finished = true;
                        break;
                }
            }
           
            return;
        }

        private SudokuSolver(int[,] field) : this()
        {
            FieldIntToField(field);
        }
        private SudokuSolver(int[][] field):this()
        {
            FieldIntToField(field);
        }
        private SudokuSolver()
        {
            numbersToExclude = new List<int>();
            fieldsToActOn = new List<SudokuField>();
        }

        private bool HorizontalTest(bool guess = false)
        {
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                numbersToExclude.Clear();
                fieldsToActOn.Clear();
                for (int y = 0; y < sudoku.GetLength(0); y++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if(number != null)
                    {
                        numbersToExclude.Add((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool addition = ExcludeNumbersToExludeFromFieldsToActOn(guess);
                if (addition == true)
                {
                    return true;
                }
            }
            return false;
        }
        private bool VerticalTest(bool guess = false)
        {
            for (int y = 0; y < sudoku.GetLength(0); y++)
            {
                numbersToExclude.Clear();
                fieldsToActOn.Clear();
                for (int x = 0; x < sudoku.GetLength(0); x++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        numbersToExclude.Add((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool addition = ExcludeNumbersToExludeFromFieldsToActOn(guess);
                if(addition == true)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SquareTest(bool guess = false)
        {
            for(int xpart1 = 0; xpart1 * squareWidth < sudoku.GetLength(0); xpart1++)
            {
                for (int ypart1 = 0; ypart1 * squareHeight < sudoku.GetLength(1); ypart1++)
                {
                    numbersToExclude.Clear();
                    fieldsToActOn.Clear();
                    for (int xpart2 = 0; xpart2 < squareWidth; xpart2++)
                    {
                        for (int ypart2 = 0; ypart2 < squareHeight; ypart2++)
                        {
                            SudokuField field = sudoku[xpart1 * squareWidth + xpart2, ypart1 * squareHeight + ypart2];
                            int? number = field.GetNumber();
                            if (number != null)
                            {
                                numbersToExclude.Add((int)number);
                            }
                            else
                            {
                                fieldsToActOn.Add(field);
                            }
                        }
                    }
                    bool addition = ExcludeNumbersToExludeFromFieldsToActOn(guess);
                    if (addition == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void FieldIntToField(int[,] field)
        {
            sudoku = new SudokuField[maxNumber, maxNumber];
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(0); y++)
                {
                    if (field[x,y] == 0)
                    {
                        sudoku[x, y] = new SudokuField(minNumber, maxNumber);
                    }
                    else
                    {
                        sudoku[x, y] = new SudokuField(minNumber, maxNumber, field[x,y]);
                    }
                }
            }
        }
        private void FieldIntToField(int[][] field)
        {
            sudoku = new SudokuField[maxNumber, maxNumber];
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[0].Length; y++)
                {
                    if(field[x][y] == 0)
                    {
                        sudoku[x, y] = new SudokuField(minNumber, maxNumber);
                    }
                    else
                    {
                        sudoku[x, y] = new SudokuField(minNumber, maxNumber, field[x][y]);
                    }
                }
            }
        }
        private int[,] FieldFieldToInt()
        {
            int[,] field = new int[sudoku.GetLength(0), sudoku.GetLength(1)];
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        field[x,y] = (int)number;
                    }
                    else
                    {
                        field[x,y] = 0;
                    }
                }
            }
            return field;
        }
        private int[][] FieldFieldToIntJagged()
        {
            int[][] field = new int[sudoku.GetLength(0)][];
            for(int x = 0; x < sudoku.GetLength(0); x++)
            {
                field[x] = new int[sudoku.GetLength(1)];
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        field[x][y] = (int)number;
                    }
                    else
                    {
                        field[x][y] = 0;
                    }
                }
            }
            return field;
        }

        private bool ExcludeNumbersToExludeFromFieldsToActOn(bool guess = false)
        {
            foreach (SudokuField field in fieldsToActOn)
            {
                bool addition = false;
                if (guess == false)
                {
                    addition = field.SetNumbers(numbersToExclude, SudokuField.Certainty.CanNotBe);
                }
                else if (guess == true)
                {
                    addition = field.SetNumbers(numbersToExclude, SudokuField.Certainty.CanNotBeOnGuess);
                }
                if (addition == true)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckIfFinished()
        {
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                for (int y = 0; y < sudoku.GetLength(0); y++)
                {
                    if (sudoku[x, y].GetNumber() == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

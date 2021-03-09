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
        private const int guessStartNumber = -1;

        private int guessx;
        private int guessy;
        private int guessNumber;

        private const string exceptionWrongFieldLength = "the sudoku field must be 9 by 9";

        private SudokuField[,] sudoku;
        private readonly List<int> numbersToWorkWith;
        private readonly List<SudokuField> fieldsToActOn;

        public static int[,] Solve(int[,] field, bool willGuess = true)
        {
            if (field.GetLength(0) != maxNumber || field.GetLength(1) != maxNumber)
            {
                throw new Exception(exceptionWrongFieldLength);
            }
            SudokuSolver solver = new SudokuSolver(field);
            Solve(solver, willGuess);
            return solver.FieldFieldToInt();
        }
        public static int[][] Solve(int[][] field, bool willGuess = true)
        {
            if (field.Length != maxNumber || field[0].Length != maxNumber)
            {
                throw new Exception(exceptionWrongFieldLength);
            }
            SudokuSolver solver = new SudokuSolver(field);
            Solve(solver, willGuess);
            return solver.FieldFieldToIntJagged();
        }

        public static void Solve(SudokuSolver solver, bool willGuess = true)
        {
            bool finished = false;
            bool guess = false;

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
                                goto case 1;
                            }
                        }
                        else
                        {
                            solveState++;
                            break; ;
                        }

                    case 1:
                        addition = solver.HorizontalExcluder(guess);
                        goto case 0;

                    case 2:
                        addition = solver.VerticalExcluder(guess);
                        goto case 0;
                    case 3:
                        addition = solver.SquareExcluder(guess);
                        goto case 0;
                    case 4:
                        addition = solver.HorizontalTest(guess);
                        goto case 0;
                    case 5:
                        addition = solver.VerticalTest(guess);
                        goto case 0;
                    case 6:
                        addition = solver.SquareTest(guess);
                        goto case 0;
                    case 7:
                        if (willGuess == true)
                        {
                            guess = true;
                            guess = !solver.Guess();
                            solveState = 0;
                            goto case 0;
                        }
                        else
                        {
                            goto case 8;
                        }
                    case 8:
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
        private SudokuSolver(int[][] field) :this()
        {
            FieldIntToField(field);
        }
        private SudokuSolver()
        {
            numbersToWorkWith = new List<int>();
            fieldsToActOn = new List<SudokuField>();
            guessx = guessStartNumber;
            guessy = guessStartNumber;
            guessNumber = guessStartNumber;
        }

        private bool HorizontalExcluder(bool guess = false)
        {
            bool addition = false;
            for (int y = 0; y < sudoku.GetLength(0); y++)
            {
                numbersToWorkWith.Clear();
                fieldsToActOn.Clear();
                for (int x = 0; x < sudoku.GetLength(0); x++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        numbersToWorkWith.Add((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool additionTest = ExcludeNumbersFromFieldsToActOn(guess);
                if (additionTest == true && addition != true)
                {
                    addition = true;
                }
            }
            return addition;
        }
        private bool VerticalExcluder(bool guess = false)
        {
            bool addition = false;
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                numbersToWorkWith.Clear();
                fieldsToActOn.Clear();
                for (int y = 0; y < sudoku.GetLength(0); y++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if(number != null)
                    {
                        numbersToWorkWith.Add((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool additionTest = ExcludeNumbersFromFieldsToActOn(guess);
                if (additionTest == true && addition != true)
                {
                    addition = true;
                }
            }
            return addition;
        }
        private bool SquareExcluder(bool guess = false)
        {
            bool addition = false;
            for (int xpart1 = 0; xpart1 * squareWidth < sudoku.GetLength(0); xpart1++)
            {
                for (int ypart1 = 0; ypart1 * squareHeight < sudoku.GetLength(1); ypart1++)
                {
                    numbersToWorkWith.Clear();
                    fieldsToActOn.Clear();
                    for (int xpart2 = 0; xpart2 < squareWidth; xpart2++)
                    {
                        for (int ypart2 = 0; ypart2 < squareHeight; ypart2++)
                        {
                            SudokuField field = sudoku[xpart1 * squareWidth + xpart2, ypart1 * squareHeight + ypart2];
                            int? number = field.GetNumber();
                            if (number != null)
                            {
                                numbersToWorkWith.Add((int)number);
                            }
                            else
                            {
                                fieldsToActOn.Add(field);
                            }
                        }
                    }
                    bool additionTest = ExcludeNumbersFromFieldsToActOn(guess);
                    if (additionTest == true && addition != true)
                    {
                        addition = true;
                    }
                }
            }
            return addition;
        }

        private bool HorizontalTest(bool guess = false)
        {
            for (int y = 0; y < sudoku.GetLength(0); y++)
            {
                numbersToWorkWith.Clear();
                fieldsToActOn.Clear();

                for (int i = minNumber; i <= maxNumber; i++)
                {
                    numbersToWorkWith.Add(i);
                }

                for (int x = 0; x < sudoku.GetLength(0); x++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        numbersToWorkWith.Remove((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool addition = CheckIfNumberHasToIntoOneOfFieldsToActOn(guess);
                if (addition == true)
                {
                    return true;
                }
            }
            return false;
        }
        private bool VerticalTest(bool guess = false)
        {
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                numbersToWorkWith.Clear();
                fieldsToActOn.Clear();

                for (int i = minNumber; i<=maxNumber; i++)
                {
                    numbersToWorkWith.Add(i);
                }

                for (int y = 0; y < sudoku.GetLength(0); y++)
                {
                    int? number = sudoku[x, y].GetNumber();
                    if (number != null)
                    {
                        numbersToWorkWith.Remove((int)number);
                    }
                    else
                    {
                        fieldsToActOn.Add(sudoku[x, y]);
                    }
                }
                bool addition = CheckIfNumberHasToIntoOneOfFieldsToActOn(guess);
                if (addition == true)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SquareTest(bool guess = false)
        {
            for (int xpart1 = 0; xpart1 * squareWidth < sudoku.GetLength(0); xpart1++)
            {
                for (int ypart1 = 0; ypart1 * squareHeight < sudoku.GetLength(1); ypart1++)
                {
                    numbersToWorkWith.Clear();
                    fieldsToActOn.Clear();

                    for (int i = minNumber; i <= maxNumber; i++)
                    {
                        numbersToWorkWith.Add(i);
                    }

                    for (int xpart2 = 0; xpart2 < squareWidth; xpart2++)
                    {
                        for (int ypart2 = 0; ypart2 < squareHeight; ypart2++)
                        {
                            SudokuField field = sudoku[xpart1 * squareWidth + xpart2, ypart1 * squareHeight + ypart2];
                            int? number = field.GetNumber();
                            if (number != null)
                            {
                                numbersToWorkWith.Remove((int)number);
                            }
                            else
                            {
                                fieldsToActOn.Add(field);
                            }
                        }
                    }
                    bool addition = CheckIfNumberHasToIntoOneOfFieldsToActOn(guess);
                    if (addition == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private bool Guess()
        {
            bool first = true;
            bool addition = false;
            if (guessNumber != guessStartNumber)
            {
                addition = sudoku[guessx, guessy].SetNumber(guessNumber, SudokuField.Certainty.CanNotBe);
                if(addition == true)
                {
                    first = false;
                }
            }
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    sudoku[x, y].RemoveGuesses();
                    if (sudoku[x, y].GetNumber() == null)
                    {
                        if (first == true)
                        {
                            IEnumerable<int> options = sudoku[x, y].GetNumbers();
                            guessNumber = options.FirstOrDefault();
                            guessx = x;
                            guessy = y;
                            sudoku[x, y].SetNumber(guessNumber, SudokuField.Certainty.Guess);
                            first = false;
                        }
                    }
                }
            }
            return addition;
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

        private bool ExcludeNumbersFromFieldsToActOn(bool guess = false)
        {
            bool addition = false;
            foreach (SudokuField field in fieldsToActOn)
            {
                bool additionTest = false;
                if (guess == false)
                {
                    additionTest = field.SetNumbers(numbersToWorkWith, SudokuField.Certainty.CanNotBe);
                }
                else if (guess == true)
                {
                    additionTest = field.SetNumbers(numbersToWorkWith, SudokuField.Certainty.CanNotBeOnGuess);
                    
                }
                if (additionTest == true)
                {
                    addition = true;
                }
            }
            return addition;
        }
        private bool CheckIfNumberHasToIntoOneOfFieldsToActOn(bool guess = false)
        {
            bool addition = false;
            foreach (int numberMissing in numbersToWorkWith)
            {
                SudokuField correctField = null;
                foreach (SudokuField field in fieldsToActOn)
                {
                    IEnumerable<int> fieldOption = field.GetNumbers();
                    foreach (int number in fieldOption)
                    {
                        if(numberMissing == number)
                        {
                            if(correctField == null)
                            {
                                correctField = field;
                                break;
                            }
                            else
                            {
                                goto skipToNextNumber;
                            }
                        }
                    }
                }
                if (correctField != null)
                {
                    if(guess == false)
                    {
                        correctField.SetNumber(numberMissing, SudokuField.Certainty.FiguredOut, false);
                    }
                    else if(guess == true)
                    {
                        correctField.SetNumber(numberMissing, SudokuField.Certainty.FiguredOutOnGuess, false);
                    }
                    addition = true;
                }
                skipToNextNumber:;
            }
            return addition;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    public class SudokuSolver
    {
        static SudokuBoard board;

        public static int[,] Solve(int[,] sudoku)
        {
            SudokuBoard board = new SudokuBoard(sudoku);
            board = Solve(board);
            sudoku = board.GetAsMultidimentionalArray();
            return sudoku;
        }
        public static int[][] Solve(int[][] sudoku)
        {
            SudokuBoard board = new SudokuBoard(sudoku);
            /*
            if(SudokuSolver.board == null)
            {
                SudokuSolver.board = board;
            }
            */
            /**/board = Solve(board);
            //board = Solve();
            sudoku = board.GetAsJaggedArray();
            return sudoku;
        }
        public static SudokuBoard Solve(SudokuBoard board, bool willGuess = true)
        {
            int solveState = 1;
            int highestSolveState = 0;
            int numberOfLoops = 0;

            while (true)
            {
                switch (solveState)
                {
                    case 0:
                        numberOfLoops++;
                        if (board.Changed == true)
                        {
                            highestSolveState = Math.Max(solveState, highestSolveState);
                            if (board.Finished == true)
                            {
                                return board;
                            }
                            board.Changed = false;
                            board.ChangedExcluding = false;
                            if (solveState == 1)
                            {
                                solveState++;
                            }
                            else
                            {
                                solveState = 1;
                            }
                            break;
                        }
                        else if (solveState > 3 && board.ChangedExcluding == true)
                        {
                            if (solveState == 4)
                            {
                                board.ChangedExcluding = false;
                                solveState++;
                            }
                            else
                            {
                                solveState = 4;
                            }
                            break;
                        }
                        else
                        {
                            solveState++;
                            break;
                        }

                    case 1:
                        board.SimpleHorisontalExclusion();
                        goto case 0;

                    case 2:
                        board.SimpleVerticalExclusion();
                        goto case 0;
                    case 3:
                        board.SimpleSquareExclusion();
                        goto case 0;
                    case 4:
                        board.ChangedExcluding = false;
                        board.HorizontalExclusionOnOnlyPositionsThatFit();
                        goto case 0;
                    case 5:
                        board.VerticalExclusionOnOnlyPositionsThatFit();
                        goto case 0;
                    case 6:
                        board.SquareExclusionOnOnlyPositionsThatFit();
                        goto case 0;
                    case 7:
                        if (willGuess)
                        {
                            board.MakeGuess();
                            goto case 0;
                        }
                        else
                        {
                            goto case 8;
                        }
                    case 8:
                        return board;
                }
            }
        }
        /*
        public static void Create(SudokuSolver solver)
        {
            bool solveable = false;
            Random random = new Random();
            List<int[]> indexes = solver.GetIndexes();
            int perfectRandomOptionIndex = 1;
            for( int i = 1; i<= 1 + maxNumber - minNumber; i++)
            {
                perfectRandomOptionIndex *= i;
            }
            while(solveable == false)
            {
                Solve(solver, false);
                int randomIndex = random.Next(indexes.Count());
                solver.SetValueAtIndex(indexes[randomIndex][0], indexes[randomIndex][1], random.Next(perfectRandomOptionIndex));
                indexes.Remove(indexes[randomIndex]);
                solveable = solver.CheckIfFinished();
                solver.RemoveAllButSet();
            }
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

        private List<int[]> GetIndexes()
        {
            List<int[]> indexes = new List<int[]>();
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    indexes.Add(new int[]{ x, y});
                }
            }
            return indexes;
        }
        private void SetValueAtIndex(int x, int y, int optionIndex = 0)
        {
            List<int> options = (List<int>)sudoku[x, y].GetNumbers();
            if (options.Count() > 0)
            {
                int option = options[optionIndex % options.Count()];
                sudoku[x, y].SetNumber(options.FirstOrDefault(), SudokuField.Certainty.Set);
            }
        }
        private void RemoveAllButSet()
        {
            foreach(SudokuField field in sudoku)
            {
                field.RemoveAllBut();
            }
        }

        private void FieldWidthHeight(int width, int height)
        {
            sudoku = new SudokuField[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sudoku[x, y] = new SudokuField(minNumber, maxNumber);
                }
            }
        }
        private void FieldIntToField(int[,] field)
        {
            sudoku = new SudokuField[maxNumber, maxNumber];
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
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
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    if (sudoku[x, y].GetNumber() == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        */
    }
}

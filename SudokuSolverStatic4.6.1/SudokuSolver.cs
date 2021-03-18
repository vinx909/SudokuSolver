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
            board = Solve(board);
            sudoku = board.GetAsJaggedArray();
            return sudoku;
        }
        private static SudokuBoard Solve(SudokuBoard board, bool willGuess = true)
        {
            int solveState = 1;

            while (true)
            {
                switch (solveState)
                {
                    case 0:
                        if (board.Changed == true)
                        {
                            if (board.Finished == true)
                            {
                                if (board.Guess == true)
                                {
                                    if (board.SimpleTest() == false)
                                    {
                                        goto case 7;
                                    }
                                }
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

        public static int[][] Create(int[][] example)
        {
            int[,] sudoku = Create();
            int[][] toReturn = new int[sudoku.GetLength(0)][];
            for (int x = 0; x < sudoku.GetLength(0); x++)
            {
                toReturn[x] = new int[sudoku.GetLength(1)];
                for (int y = 0; y < sudoku.GetLength(1); y++)
                {
                    toReturn[x][y] = sudoku[x, y];
                }
            }

            return toReturn;
        }
        public static int[,] Create(int[,] example)
        {
            return Create();
        }
        public static int[,] Create(SudokuBoard.BoardProperty boardProperty = SudokuBoard.BoardProperty.NineByNine)
        {
            Random random = new Random();

            SudokuBoard solution = new SudokuBoard(boardProperty);
            solution.SetField();
            solution = Solve(solution);

            int[,] sudoku = solution.GetAsMultidimentionalArray();
            List<int[]> fields = new List<int[]>();
            for(int x = 0; x < sudoku.GetLength(0); x++)
            {
                for (int y = 0; y < sudoku.GetLength(0); y++)
                {
                    fields.Add(new int[] { x, y });
                }
            }

            while(fields.Count() != 0)
            {
                int[,] newSudokuAttempt = CreateCopyOfArray(sudoku);
                int[] randomField = fields[random.Next(fields.Count())];
                newSudokuAttempt[randomField[0], randomField[1]] = 0;

                solution.SetField(newSudokuAttempt);
                solution = Solve(solution, false);

                if (solution.Finished)
                {
                    sudoku = newSudokuAttempt;
                }
                fields.Remove(randomField);
            }

            return sudoku;
        }

        private static int[,] CreateCopyOfArray(int[,] toCopy)
        {
            int[,] copy = new int[toCopy.GetLength(0), toCopy.GetLength(1)];
            for (int x = 0; x < copy.GetLength(0); x++)
            {
                for (int y = 0; y < copy.GetLength(1); y++)
                {
                    copy[x, y] = toCopy[x, y];
                }
            }
            return copy;
        }
    }
}

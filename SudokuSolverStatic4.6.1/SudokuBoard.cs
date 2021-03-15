using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    public class SudokuBoard
    {
        public enum BoardPropperty
        {
            NineByNine,
            SixBySix,
            FourByFour,
            TwoByTwo
        }
        private static Dictionary<BoardPropperty, BoardProppertySet> boardPropperties = new Dictionary<BoardPropperty, BoardProppertySet>
        {
            { BoardPropperty.NineByNine, new BoardProppertySet { BoardWidth = 9, BoardHeight = 9, SquareWidth = 3, SquareHeight = 3, MinNumber = 1, MaxNumber = 9 } },
            { BoardPropperty.SixBySix, new BoardProppertySet { BoardWidth = 6, BoardHeight = 6, SquareWidth = 3, SquareHeight = 2, MinNumber = 1, MaxNumber = 6 } },
            { BoardPropperty.FourByFour, new BoardProppertySet { BoardWidth = 4, BoardHeight = 4, SquareWidth = 2, SquareHeight = 2, MinNumber = 1, MaxNumber = 4 } },
            { BoardPropperty.TwoByTwo, new BoardProppertySet { BoardWidth = 2, BoardHeight = 2, SquareWidth = 2, SquareHeight = 1, MinNumber = 1, MaxNumber = 2 } }
        };

        private readonly Func<int, int, SudokuField> defaultCreatereateSudokuField = (int width, int height) =>
        {
            return new SudokuField(width, height);
        };
        private readonly Func<int, int, int, SudokuField> defaultCreatereateSudokuFieldWithSetNumber = (int width, int height, int setNumber) =>
        {
            return new SudokuField(width, height, setNumber);
        };

        private Func<int, int, SudokuField> createreateSudokuField;
        private Func<int, int, int, SudokuField> createreateSudokuFieldWithSetNumber;

        private readonly BoardPropperty boardPropperty;
        private SudokuField[,] board;

        private List<int> numbersToWorkWith;
        private List<SudokuField> fieldsToWorkWith;

        public bool Changed { get; set; }
        public bool Guess { get; set; }


        public SudokuBoard(int[,] board, BoardPropperty boardPropperty = BoardPropperty.NineByNine) :this(boardPropperty)
        {
            SetField(board);
        }
        public SudokuBoard(int[][] board, BoardPropperty boardPropperty = BoardPropperty.NineByNine) :this(boardPropperty)
        {
            SetField(board);
        }
        public SudokuBoard(BoardPropperty boardPropperty = BoardPropperty.NineByNine)
        {
            this.boardPropperty = boardPropperty;
            createreateSudokuField = defaultCreatereateSudokuField;
            createreateSudokuFieldWithSetNumber = defaultCreatereateSudokuFieldWithSetNumber;
            numbersToWorkWith = new List<int>();
            fieldsToWorkWith = new List<SudokuField>();
            Changed = false;
            Guess = false;
        }

        public void SetField()
        {
            board = BoardWidthHeight();
        }
        public void SetField(int[,] board)
        {
            this.board = BoardIntToField(board);
        }
        public void SetField(int[][] board)
        {
            this.board = BoardIntToField(board);
        }


        public void SetCreateFieldFunctions(Func<int, int, SudokuField> create, Func<int, int, int, SudokuField> createWithSetNumber)
        {
            createreateSudokuField = create;
            createreateSudokuFieldWithSetNumber = createWithSetNumber;
        }


        public void SimpleHorizontalExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    numbersToWorkWith.Clear();
                    fieldsToWorkWith.Clear();
                    for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbersToWorkWith.Add((int)number);
                        }
                        fieldsToWorkWith.Add(board[x, y]);
                    }
                    ExcludeNumbersFromFields(numbersToWorkWith, fieldsToWorkWith);
                }
            }
        }
        public void SimpleVerticalExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                for (int x = 0; x < boardPropperties[boardPropperty].BoardHeight; x++)
                {
                    numbersToWorkWith.Clear();
                    fieldsToWorkWith.Clear();
                    for (int y = 0; y < boardPropperties[boardPropperty].BoardWidth; y++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbersToWorkWith.Add((int)number);
                        }
                        fieldsToWorkWith.Add(board[x, y]);
                    }
                    ExcludeNumbersFromFields(numbersToWorkWith, fieldsToWorkWith);
                }
            }
        }
        public void SimpleSquareExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                for(int xSquare = 0; xSquare < boardPropperties[boardPropperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardPropperties[boardPropperty].NumberOfVerticalSquares; ySquare++)
                    {
                        numbersToWorkWith.Clear();
                        fieldsToWorkWith.Clear();
                        for (int xInternal = 0; xInternal < boardPropperties[boardPropperty].SquareWidth; xInternal++)
                        {
                            for (int yInternal = 0; yInternal < boardPropperties[boardPropperty].SquareHeight; yInternal++)
                            {
                                int x = xSquare * boardPropperties[boardPropperty].SquareWidth + xInternal;
                                int y = ySquare * boardPropperties[boardPropperty].SquareHeight + yInternal;
                                int? number = board[x, y].GetNumber();
                                if (number != null)
                                {
                                    numbersToWorkWith.Add((int)number);
                                }
                                fieldsToWorkWith.Add(board[x, y]);
                            }
                        }
                        ExcludeNumbersFromFields(numbersToWorkWith, fieldsToWorkWith);
                    }
                }
            }
        }


        private void ExcludeNumbersFromFields(IEnumerable<int> numbers, IEnumerable<SudokuField> fields)
        {
            SudokuField.Certainty toSetTo = SudokuField.Certainty.CanNotBe;
            if(Guess == true)
            {
                toSetTo = SudokuField.Certainty.CanNotBeOnGuess;
            }

            foreach(SudokuField field in fields)
            {
                bool newChange = field.SetNumbers(numbers, toSetTo);
                Changed = Changed || newChange;
            }
        }

        private SudokuField[,] BoardWidthHeight()
        {
            SudokuField[,]  board = new SudokuField[boardPropperties[boardPropperty].BoardWidth, boardPropperties[boardPropperty].BoardHeight];
            for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
            {
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    board[x, y] = createreateSudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber);
                }
            }
            return board;
        }
        private SudokuField[,] BoardIntToField(int[,] field)
        {
            SudokuField[,] board = new SudokuField[boardPropperties[boardPropperty].BoardWidth, boardPropperties[boardPropperty].BoardHeight];
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (field[x, y] == 0)
                    {
                        board[x, y] = new SudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = new SudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber, field[x, y]);
                    }
                }
            }
            return board;
        }
        private SudokuField[,] BoardIntToField(int[][] field)
        {
            SudokuField[,] board = new SudokuField[boardPropperties[boardPropperty].BoardWidth, boardPropperties[boardPropperty].BoardHeight];
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[0].Length; y++)
                {
                    if (field[x][y] == 0)
                    {
                        board[x, y] = new SudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = new SudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber, field[x][y]);
                    }
                }
            }
            return board;
        }
        public int[,] GetAsMultidimentionalArray()
        {
            int[,] field = new int[board.GetLength(0), board.GetLength(1)];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    int? number = board[x, y].GetNumber();
                    if (number != null)
                    {
                        field[x, y] = (int)number;
                    }
                    else
                    {
                        field[x, y] = 0;
                    }
                }
            }
            return field;
        }
        public int[][] GetAsJaggedArray()
        {
            int[][] field = new int[board.GetLength(0)][];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                field[x] = new int[board.GetLength(1)];
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    int? number = board[x, y].GetNumber();
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

        
        internal class BoardProppertySet
        {
            public int BoardWidth { get; set; }
            public int BoardHeight { get; set; }
            public int SquareWidth { get; set; }
            public int SquareHeight { get; set; }
            public int NumberOfHorizontalSquares
            {
                get
                {
                    return BoardWidth / SquareWidth;
                }
            }
            public int NumberOfVerticalSquares
            {
                get
                {
                    return BoardHeight / SquareHeight;
                }
            }
            public int MinNumber { get; set; }
            public int MaxNumber { get; set; }
        }
    }
}

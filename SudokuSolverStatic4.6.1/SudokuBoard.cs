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

        private const string exceptionNoEmptySpotToGuessFor = "trying to fill an empty stop when no empty spot can be found";
        private const string exceptionNoGuessToGuessIn = "trying to make a different guess when there is no guess";
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
        private List<GuessDetails> guesses;

        public bool Changed { get; set; }
        public bool ChangedExcluding { get; set; }
        public bool Guess { get; set; }
        public bool Finished
        {
            get
            {
                foreach(SudokuField field in board)
                {
                    if(field.GetNumber() == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        public SudokuBoard(int[,] board, BoardPropperty boardPropperty = BoardPropperty.NineByNine) : this(boardPropperty)
        {
            SetField(board);
        }
        public SudokuBoard(int[][] board, BoardPropperty boardPropperty = BoardPropperty.NineByNine) : this(boardPropperty)
        {
            SetField(board);
        }
        public SudokuBoard(BoardPropperty boardPropperty = BoardPropperty.NineByNine)
        {
            this.boardPropperty = boardPropperty;
            createreateSudokuField = defaultCreatereateSudokuField;
            createreateSudokuFieldWithSetNumber = defaultCreatereateSudokuFieldWithSetNumber;
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


        public void SimpleHorisontalExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                List<SudokuField> fields = new List<SudokuField>();
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    numbers.Clear();
                    fields.Clear();
                    for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbers.Add((int)number);
                        }
                        fields.Add(board[x, y]);
                    }
                    ExcludeNumbersFromFields(numbers, fields);
                }
            }
        }
        public void SimpleVerticalExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                List<SudokuField> fields = new List<SudokuField>();
                for (int x = 0; x < boardPropperties[boardPropperty].BoardHeight; x++)
                {
                    numbers.Clear();
                    fields.Clear();
                    for (int y = 0; y < boardPropperties[boardPropperty].BoardWidth; y++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbers.Add((int)number);
                        }
                        fields.Add(board[x, y]);
                    }
                    ExcludeNumbersFromFields(numbers, fields);
                }
            }
        }
        public void SimpleSquareExclusion()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                List<SudokuField> fields = new List<SudokuField>();
                for (int xSquare = 0; xSquare < boardPropperties[boardPropperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardPropperties[boardPropperty].NumberOfVerticalSquares; ySquare++)
                    {
                        numbers.Clear();
                        fields.Clear();
                        for (int xInternal = 0; xInternal < boardPropperties[boardPropperty].SquareWidth; xInternal++)
                        {
                            for (int yInternal = 0; yInternal < boardPropperties[boardPropperty].SquareHeight; yInternal++)
                            {
                                int x = xSquare * boardPropperties[boardPropperty].SquareWidth + xInternal;
                                int y = ySquare * boardPropperties[boardPropperty].SquareHeight + yInternal;
                                int? number = board[x, y].GetNumber();
                                if (number != null)
                                {
                                    numbers.Add((int)number);
                                }
                                fields.Add(board[x, y]);
                            }
                        }
                        ExcludeNumbersFromFields(numbers, fields);
                    }
                }
            }
        }

        public void HorizontalExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<SudokuField> fields = new List<SudokuField>();
                for(int y = 0; y < board.GetLength(1); y++)
                {
                    fields.Clear();
                    for(int x = 0; x < board.GetLength(0); x++)
                    {
                        fields.Add(board[x, y]);
                    }
                    FindWhichOfNumbersItMustBeByOnlyFieldWhereNumbersFits(fields);
                }
            }
        }
        public void VerticalExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<SudokuField> fields = new List<SudokuField>();
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    fields.Clear();
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        fields.Add(board[x, y]);
                    }
                    FindWhichOfNumbersItMustBeByOnlyFieldWhereNumbersFits(fields);
                }
            }
        }
        public void SquareExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<SudokuField> fields = new List<SudokuField>();
                for(int xSquare = 0; xSquare < boardPropperties[boardPropperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardPropperties[boardPropperty].NumberOfVerticalSquares; ySquare++)
                    {
                        fields.Clear();
                        for(int xInSquare = 0; xInSquare < boardPropperties[boardPropperty].SquareWidth; xInSquare++)
                        {
                            for(int yInSquare = 0; yInSquare < boardPropperties[boardPropperty].SquareHeight; yInSquare++)
                            {
                                int x = xSquare * boardPropperties[boardPropperty].SquareWidth + xInSquare;
                                int y = ySquare * boardPropperties[boardPropperty].SquareHeight + yInSquare;
                                fields.Add(board[x, y]);
                            }
                        }
                        FindWhichOfNumbersItMustBeByOnlyFieldWhereNumbersFits(fields);
                    }
                }
            }
        }

        public void MakeGuess()
        {
            if(guesses == null)
            {
                guesses = new List<GuessDetails>();
            }
            bool goBackOneGuess = false;
            if(SimpleHorisontalTest() && SimpleVerticalTest() && SimpleSquareTest() && !Finished)
            {
                goBackOneGuess = MakeGuessFillEmpty();
            }
            else
            {
                goBackOneGuess = MakeGuessTryDifferentGuess();
            }
            if(goBackOneGuess == true)
            {
                MakeGuessGoBackOneGuess();
            }
            MakeGuessApplyAllGuesses();
            Changed = true;
        } 
        private bool MakeGuessFillEmpty()
        {
            for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
            {
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    if (board[x,y].GetNumber() == null)
                    {
                        guesses.Add(new GuessDetails { X = x, Y = y });
                        return MakeGuessTryDifferentGuess(true);
                    }
                }
            }
            throw new Exception(exceptionNoEmptySpotToGuessFor);
        }
        private bool MakeGuessTryDifferentGuess(bool firstGuess = false)
        {
            MakeGuessRemoveGuessExclusionsAndSets();
            int guessIndex = guesses.Count() - 1;
            if (guessIndex < 0)
            {
                throw new Exception(exceptionNoGuessToGuessIn);
            }
            GuessDetails guess = guesses[guessIndex];
            if(firstGuess == false)
            {
                guess.AddFailedGuess(guess.Guess);
            }
            IEnumerable<int> guessOptions = guess.getGuessOptions(board[guess.X, guess.Y]);
            if(guessOptions.Count() == 0)
            {
                return true;
            }
            guess.Guess = guessOptions.FirstOrDefault();
            return false;
        }
        private void MakeGuessGoBackOneGuess()
        {
            int guessIndex = guesses.Count() - 1;
            if (guessIndex < 0)
            {
                throw new Exception(exceptionNoGuessToGuessIn);
            }
            guesses.Remove(guesses[guessIndex]);
            bool removeAnotherGuess = MakeGuessTryDifferentGuess();
            if(removeAnotherGuess == true)
            {
                MakeGuessGoBackOneGuess();
            }
        }
        private void MakeGuessRemoveGuessExclusionsAndSets()
        {
            for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
            {
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    board[x, y].RemoveGuesses();
                }
            }
        }
        private void MakeGuessApplyAllGuesses()
        {
            foreach(GuessDetails guess in guesses)
            {
                board[guess.X, guess.Y].SetNumber(guess.Guess, SudokuField.Certainty.Guess);
            }
        }

        public bool SimpleHorisontalTest()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                for (int y = 0; y < boardPropperties[boardPropperty].BoardHeight; y++)
                {
                    numbers.Clear();
                    for (int x = 0; x < boardPropperties[boardPropperty].BoardWidth; x++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbers.Add((int)number);
                        }
                    }
                    bool noNumberMoreThenOnce = NoNumberMoreThenOnce(numbers);
                    if (noNumberMoreThenOnce == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool SimpleVerticalTest()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                for (int x = 0; x < boardPropperties[boardPropperty].BoardHeight; x++)
                {
                    numbers.Clear();
                    for (int y = 0; y < boardPropperties[boardPropperty].BoardWidth; y++)
                    {
                        int? number = board[x, y].GetNumber();
                        if (number != null)
                        {
                            numbers.Add((int)number);
                        }
                    }
                    bool noNumberMoreThenOnce = NoNumberMoreThenOnce(numbers);
                    if (noNumberMoreThenOnce == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool SimpleSquareTest()
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                List<int> numbers = new List<int>();
                List<SudokuField> fields = new List<SudokuField>();
                for (int xSquare = 0; xSquare < boardPropperties[boardPropperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardPropperties[boardPropperty].NumberOfVerticalSquares; ySquare++)
                    {
                        numbers.Clear();
                        fields.Clear();
                        for (int xInternal = 0; xInternal < boardPropperties[boardPropperty].SquareWidth; xInternal++)
                        {
                            for (int yInternal = 0; yInternal < boardPropperties[boardPropperty].SquareHeight; yInternal++)
                            {
                                int x = xSquare * boardPropperties[boardPropperty].SquareWidth + xInternal;
                                int y = ySquare * boardPropperties[boardPropperty].SquareHeight + yInternal;
                                int? number = board[x, y].GetNumber();
                                if (number != null)
                                {
                                    numbers.Add((int)number);
                                }
                                fields.Add(board[x, y]);
                            }
                        }
                        bool noNumberMoreThenOnce = NoNumberMoreThenOnce(numbers);
                        if(noNumberMoreThenOnce == false)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        private void ExcludeNumbersFromFields(IEnumerable<int> numbers, IEnumerable<SudokuField> fields)
        {
            SudokuField.Certainty toSetTo = SudokuField.Certainty.CanNotBe;
            if (Guess == true)
            {
                toSetTo = SudokuField.Certainty.CanNotBeOnGuess;
            }

            foreach (SudokuField field in fields)
            {
                SudokuField.ChangeType newChange = field.SetNumbers(numbers, toSetTo);
                ChangeController(newChange);
            }
        }
        private void FindWhichOfNumbersItMustBeByOnlyFieldWhereNumbersFits(List<SudokuField> fields)
        {
            if (boardPropperty == BoardPropperty.NineByNine || boardPropperty == BoardPropperty.SixBySix || boardPropperty == BoardPropperty.FourByFour || boardPropperty == BoardPropperty.TwoByTwo)
            {
                SudokuField.Certainty toSetTo = SudokuField.Certainty.CanNotBe;
                if(Guess == true)
                {
                    toSetTo = SudokuField.Certainty.CanNotBeOnGuess;
                }

                List<int> numbersNeeded = new List<int>();
                for(int i = boardPropperties[boardPropperty].MinNumber; i <= boardPropperties[boardPropperty].MaxNumber; i++)
                {
                    numbersNeeded.Add(i);
                }
                for (int i = 0; i < fields.Count; i++)
                {
                    int? number = fields[i].GetNumber();
                    if (number != null)
                    {
                        numbersNeeded.Remove((int)number);
                        fields.Remove(fields[i]);
                        i--;
                    }
                }
                foreach (SudokuField field in fields)
                {
                    List<int> numbersField = (List<int>)field.GetNumbers();
                    numbersField.RemoveAll(number => !numbersNeeded.Contains(number));
                    foreach (SudokuField fieldToCheckAgainst in fields)
                    {
                        if (field != fieldToCheckAgainst)
                        {
                            IEnumerable<int> numbersCoveredByOtherField = fieldToCheckAgainst.GetNumbers();
                            numbersField.RemoveAll(number => numbersCoveredByOtherField.Contains(number));
                            if (numbersField.Count() == 0)
                            {
                                break;
                            }
                        }
                    }
                    if(numbersField.Count() >= 1)
                    {
                        List<int> numbersToExclude = (List<int>)field.GetNumbers();
                        numbersToExclude.RemoveAll(number => numbersField.Contains(number));
                        SudokuField.ChangeType newChange = field.SetNumbers(numbersToExclude, toSetTo);
                        ChangeController(newChange);
                    }
                }
            }
        }
        private bool NoNumberMoreThenOnce(List<int> numbers)
        {
            for (int indexOne = 0; indexOne < numbers.Count(); indexOne++)
            {
                for (int indexTwo = indexOne + 1; indexTwo < numbers.Count(); indexTwo++)
                {
                    if (numbers[indexOne] == numbers[indexTwo])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void ChangeController(SudokuField.ChangeType change)
        {
            if(change == SudokuField.ChangeType.Excluding)
            {
                ChangedExcluding = true;
            }
            else if(change == SudokuField.ChangeType.FiguredOut)
            {
                Changed = true;
            }
        }

        private SudokuField[,] BoardWidthHeight()
        {
            SudokuField[,] board = new SudokuField[boardPropperties[boardPropperty].BoardWidth, boardPropperties[boardPropperty].BoardHeight];
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
                        board[x, y] = createreateSudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = createreateSudokuFieldWithSetNumber(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber, field[x, y]);
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
                        board[x, y] = createreateSudokuField(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = createreateSudokuFieldWithSetNumber(boardPropperties[boardPropperty].MinNumber, boardPropperties[boardPropperty].MaxNumber, field[x][y]);
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
            int[][] field = new int[9][];
            for(int i = 0; i < 9; i++)
            {
                field[i] = new int[9];
            }

            //int[][] field = new int[board.GetLength(0)][];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                //field[x] = new int[board.GetLength(1)];
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
        internal class GuessDetails
        {
            private List<int> failedGuesses;
            internal int X { get; set; }
            internal int Y { get; set; }
            internal int Guess { get; set; }
            internal GuessDetails()
            {
                failedGuesses = new List<int>();
            }
            internal void AddFailedGuess(int guess)
            {
                failedGuesses.Add(guess);
            }
            internal IEnumerable<int> getGuessOptions(SudokuField field)
            {
                List<int> options = (List<int>)field.GetNumbers();
                options.RemoveAll(number => failedGuesses.Contains(number));
                return options;
            }
        }
    }
}

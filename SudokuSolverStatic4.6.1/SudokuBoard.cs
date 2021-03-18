using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    public class SudokuBoard
    {
        public enum BoardProperty
        {
            NineByNine,
            SixBySix,
            FourByFour,
            TwoByTwo,
            BinaryEightByEight
        }

        private const string exceptionNoEmptySpotToGuessFor = "trying to fill an empty stop when no empty spot can be found";
        private const string exceptionNoGuessToGuessIn = "trying to make a different guess when there is no guess";
        private static readonly Dictionary<BoardProperty, BoardPropertySet> boardProperties = new Dictionary<BoardProperty, BoardPropertySet>
        {
            { BoardProperty.NineByNine, new BoardPropertySet { BoardWidth = 9, BoardHeight = 9, SquareWidth = 3, SquareHeight = 3, MinNumber = 1, MaxNumber = 9 } },
            { BoardProperty.SixBySix, new BoardPropertySet { BoardWidth = 6, BoardHeight = 6, SquareWidth = 3, SquareHeight = 2, MinNumber = 1, MaxNumber = 6 } },
            { BoardProperty.FourByFour, new BoardPropertySet { BoardWidth = 4, BoardHeight = 4, SquareWidth = 2, SquareHeight = 2, MinNumber = 1, MaxNumber = 4 } },
            { BoardProperty.TwoByTwo, new BoardPropertySet { BoardWidth = 2, BoardHeight = 2, SquareWidth = 2, SquareHeight = 1, MinNumber = 1, MaxNumber = 2 } },
            { BoardProperty.BinaryEightByEight, new BoardPropertySet { BoardWidth = 8, BoardHeight = 8, SquareWidth = 0, SquareHeight = 0,  MinNumber = 1, MaxNumber = 2 } }
        };
        private static readonly BoardProperty[] boardPropertiesClassic = new BoardProperty[] {
            BoardProperty.TwoByTwo,
            BoardProperty.FourByFour,
            BoardProperty.SixBySix,
            BoardProperty.NineByNine};
        private static readonly BoardProperty[] boardPropertiesBinary = new BoardProperty[]
        {
            BoardProperty.BinaryEightByEight
        };

        private readonly Func<int, int, SudokuField> defaultCreateSudokuField = (int width, int height) =>
        {
            return new SudokuField(width, height);
        };
        private readonly Func<int, int, int, SudokuField> defaultCreateSudokuFieldWithSetNumber = (int width, int height, int setNumber) =>
        {
            return new SudokuField(width, height, setNumber);
        };

        private Func<int, int, SudokuField> createSudokuField;
        private Func<int, int, int, SudokuField> createSudokuFieldWithSetNumber;

        private readonly BoardProperty boardProperty;
        private SudokuField[,] board;

        private List<GuessDetails> guesses;
        private Random randomForGuesses;
        private bool guessesApplied;

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


        public SudokuBoard(int[,] board, BoardProperty boardProperty = BoardProperty.NineByNine) : this(boardProperty)
        {
            SetField(board);
        }
        public SudokuBoard(int[][] board, BoardProperty boardProperty = BoardProperty.NineByNine) : this(boardProperty)
        {
            SetField(board);
        }
        public SudokuBoard(BoardProperty boardProperty = BoardProperty.NineByNine)
        {
            this.boardProperty = boardProperty;
            createSudokuField = defaultCreateSudokuField;
            createSudokuFieldWithSetNumber = defaultCreateSudokuFieldWithSetNumber;
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
            createSudokuField = create;
            createSudokuFieldWithSetNumber = createWithSetNumber;
        }

        public void SimpleHorisontalExclusion()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
                {
                    fields.Clear();
                    for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
                    {
                        fields.Add(board[x, y]);
                    }
                    SimpleExclusion(fields);
                }
            }
        }
        public void SimpleVerticalExclusion()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for (int x = 0; x < boardProperties[boardProperty].BoardHeight; x++)
                {
                    fields.Clear();
                    for (int y = 0; y < boardProperties[boardProperty].BoardWidth; y++)
                    {
                        fields.Add(board[x, y]);
                    }
                    SimpleExclusion(fields);
                }
            }
        }
        public void SimpleSquareExclusion()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for (int xSquare = 0; xSquare < boardProperties[boardProperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardProperties[boardProperty].NumberOfVerticalSquares; ySquare++)
                    {
                        fields.Clear();
                        for (int xInternal = 0; xInternal < boardProperties[boardProperty].SquareWidth; xInternal++)
                        {
                            for (int yInternal = 0; yInternal < boardProperties[boardProperty].SquareHeight; yInternal++)
                            {
                                int x = xSquare * boardProperties[boardProperty].SquareWidth + xInternal;
                                int y = ySquare * boardProperties[boardProperty].SquareHeight + yInternal;
                                fields.Add(board[x, y]);
                            }
                        }
                        SimpleExclusion(fields);
                    }
                }
            }
        }

        public void HorizontalExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for(int y = 0; y < board.GetLength(1); y++)
                {
                    fields.Clear();
                    for(int x = 0; x < board.GetLength(0); x++)
                    {
                        fields.Add(board[x, y]);
                    }
                    ExclusionOnOnlyPositionsThatFit(fields);
                }
            }
        }
        public void VerticalExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    fields.Clear();
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        fields.Add(board[x, y]);
                    }
                    ExclusionOnOnlyPositionsThatFit(fields);
                }
            }
        }
        public void SquareExclusionOnOnlyPositionsThatFit()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<SudokuField> fields = new List<SudokuField>();
                for(int xSquare = 0; xSquare < boardProperties[boardProperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardProperties[boardProperty].NumberOfVerticalSquares; ySquare++)
                    {
                        fields.Clear();
                        for(int xInSquare = 0; xInSquare < boardProperties[boardProperty].SquareWidth; xInSquare++)
                        {
                            for(int yInSquare = 0; yInSquare < boardProperties[boardProperty].SquareHeight; yInSquare++)
                            {
                                int x = xSquare * boardProperties[boardProperty].SquareWidth + xInSquare;
                                int y = ySquare * boardProperties[boardProperty].SquareHeight + yInSquare;
                                fields.Add(board[x, y]);
                            }
                        }
                        ExclusionOnOnlyPositionsThatFit(fields);
                    }
                }
            }
        }

        public void MakeGuess()
        {
            if(guesses == null)
            {
                guesses = new List<GuessDetails>();
                randomForGuesses = new Random();
                Guess = true;
            }
            bool goBackOneGuess = false;
            if(SimpleTest() && !Finished)
            {
                goBackOneGuess = MakeGuessFillEmpty();
            }
            if(SimpleTest() == false)
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
            for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
            {
                for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
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
            List<int> guessOptions = guess.getGuessOptions(board[guess.X, guess.Y]);
            if(guessOptions.Count() == 0)
            {
                return true;
            }
            guess.Guess = guessOptions[randomForGuesses.Next(guessOptions.Count())];
            MakeGuessApplyAllGuesses();
            if(SimpleTest() == false)
            {
                MakeGuessTryDifferentGuess();
            }
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
            if(guessesApplied == true)
            {
                guessesApplied = false;
                for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
                {
                    for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
                    {
                        board[x, y].RemoveGuesses();
                    }
                }
            }
        }
        private void MakeGuessApplyAllGuesses()
        {
            if(guessesApplied == false)
            {
                guessesApplied = true;
                foreach(GuessDetails guess in guesses)
                {
                    board[guess.X, guess.Y].SetNumber(guess.Guess, SudokuField.Certainty.Guess);
                }
            }
        }

        public void BinarySolve()
        {
            if(boardPropertiesBinary.Contains(boardProperty))
            {
                bool changeMade;
                do
                {
                    changeMade = false;
                    for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
                    {
                        for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
                        {
                            bool newchange = BinarySolve(x, y);
                            changeMade = changeMade || newchange;
                        }
                    }
                    Changed = Changed || changeMade;
                }
                while (changeMade == true);
            }
        }
        private bool BinarySolve(int x, int y)
        {
            if(board[x,y].GetNumber() != null)
            {
                return false;
            }

            SudokuField.Certainty toSetTo = SudokuField.Certainty.FiguredOut;
            if (Guess == true)
            {
                toSetTo = SudokuField.Certainty.FiguredOutOnGuess;
            }

            int? number1;
            int? number2;

            if (x - 1 >= 0 && x + 1 < boardProperties[boardProperty].BoardWidth)
            {
                number1 = board[x - 1, y].GetNumber();
                number2 = board[x + 1, y].GetNumber();
                bool attempt = BinarySolveTryToSetWithTwoNumbers(x, y, number1, number2, toSetTo);
                if (attempt == true)
                {
                    return true;
                }
            }
            if (y - 1 >= 0 && y + 1 < boardProperties[boardProperty].BoardHeight)
            {
                number1 = board[x, y - 1].GetNumber();
                number2 = board[x, y + 1].GetNumber();
                bool attempt = BinarySolveTryToSetWithTwoNumbers(x, y, number1, number2, toSetTo);
                if(attempt == true)
                {
                    return true;
                }
            }
            int[] positiveAndNegative = new int[]{ -1, 1 };
            foreach(int positiveNegative in positiveAndNegative)
            {
                if(x + 2 * positiveNegative >= 0 && x + 2 * positiveNegative < boardProperties[boardProperty].BoardWidth)
                {
                    number1 = board[x + 2 * positiveNegative, y].GetNumber();
                    number2 = board[x + positiveNegative, y].GetNumber();
                    bool attempt = BinarySolveTryToSetWithTwoNumbers(x, y, number1, number2, toSetTo);
                    if (attempt == true)
                    {
                        return true;
                    }
                }
                if (y + 2 * positiveNegative >= 0 && y + 2 * positiveNegative < boardProperties[boardProperty].BoardHeight)
                {
                    number1 = board[x, y + 2 * positiveNegative].GetNumber();
                    number2 = board[x, y + positiveNegative].GetNumber();
                    bool attempt = BinarySolveTryToSetWithTwoNumbers(x, y, number1, number2, toSetTo);
                    if (attempt == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool BinarySolveTryToSetWithTwoNumbers(int x, int y, int? number1, int? number2, SudokuField.Certainty certainty)
        {
            if (number1 != null && number2 != null && number1 == number2)
            {
                board[x, y].SetNumber(BinaryOtherNumber((int)number1), certainty);
                return true;
            }
            return false;
        }
        private int BinaryOtherNumber(int number)
        {
            if (boardProperties[boardProperty].MaxNumber == number)
            {
                return boardProperties[boardProperty].MinNumber;
            }
            else if(boardProperties[boardProperty].MinNumber == number)
            {
                return boardProperties[boardProperty].MaxNumber;
            }
            else
            {
                throw new Exception();
            }
        }

        public bool SimpleTest()
        {
            return SimpleHorisontalTest() && SimpleVerticalTest() && SimpleSquareTest();
        }
        public bool SimpleHorisontalTest()
        {
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<int> numbers = new List<int>();
                for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
                {
                    numbers.Clear();
                    for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
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
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<int> numbers = new List<int>();
                for (int x = 0; x < boardProperties[boardProperty].BoardHeight; x++)
                {
                    numbers.Clear();
                    for (int y = 0; y < boardProperties[boardProperty].BoardWidth; y++)
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
            if (boardPropertiesClassic.Contains(boardProperty))
            {
                List<int> numbers = new List<int>();
                List<SudokuField> fields = new List<SudokuField>();
                for (int xSquare = 0; xSquare < boardProperties[boardProperty].NumberOfHorizontalSquares; xSquare++)
                {
                    for (int ySquare = 0; ySquare < boardProperties[boardProperty].NumberOfVerticalSquares; ySquare++)
                    {
                        numbers.Clear();
                        fields.Clear();
                        for (int xInternal = 0; xInternal < boardProperties[boardProperty].SquareWidth; xInternal++)
                        {
                            for (int yInternal = 0; yInternal < boardProperties[boardProperty].SquareHeight; yInternal++)
                            {
                                int x = xSquare * boardProperties[boardProperty].SquareWidth + xInternal;
                                int y = ySquare * boardProperties[boardProperty].SquareHeight + yInternal;
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

        private void SimpleExclusion(IEnumerable<SudokuField> fields)
        {
            SudokuField.Certainty toSetTo = SudokuField.Certainty.CanNotBe;
            if (Guess == true)
            {
                toSetTo = SudokuField.Certainty.CanNotBeOnGuess;
            }

            List<int> filledInNumbers = new List<int>();
            foreach(SudokuField field in fields)
            {
                int? number = field.GetNumber();
                if(number != null)
                {
                    filledInNumbers.Add((int)number);
                }
            }

            foreach (SudokuField field in fields)
            {
                SudokuField.ChangeType newChange = field.SetNumbers(filledInNumbers, toSetTo);
                ChangeController(newChange);
            }
        }
        private void ExclusionOnOnlyPositionsThatFit(List<SudokuField> fields)
        {
            if (boardProperty == BoardProperty.NineByNine || boardProperty == BoardProperty.SixBySix || boardProperty == BoardProperty.FourByFour || boardProperty == BoardProperty.TwoByTwo)
            {
                SudokuField.Certainty toSetTo = SudokuField.Certainty.CanNotBe;
                if(Guess == true)
                {
                    toSetTo = SudokuField.Certainty.CanNotBeOnGuess;
                }

                List<int> numbersNeeded = new List<int>();
                for(int i = boardProperties[boardProperty].MinNumber; i <= boardProperties[boardProperty].MaxNumber; i++)
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
            SudokuField[,] board = new SudokuField[boardProperties[boardProperty].BoardWidth, boardProperties[boardProperty].BoardHeight];
            for (int x = 0; x < boardProperties[boardProperty].BoardWidth; x++)
            {
                for (int y = 0; y < boardProperties[boardProperty].BoardHeight; y++)
                {
                    board[x, y] = createSudokuField(boardProperties[boardProperty].MinNumber, boardProperties[boardProperty].MaxNumber);
                }
            }
            return board;
        }
        private SudokuField[,] BoardIntToField(int[,] field)
        {
            SudokuField[,] board = new SudokuField[boardProperties[boardProperty].BoardWidth, boardProperties[boardProperty].BoardHeight];
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (field[x, y] == 0)
                    {
                        board[x, y] = createSudokuField(boardProperties[boardProperty].MinNumber, boardProperties[boardProperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = createSudokuFieldWithSetNumber(boardProperties[boardProperty].MinNumber, boardProperties[boardProperty].MaxNumber, field[x, y]);
                    }
                }
            }
            return board;
        }
        private SudokuField[,] BoardIntToField(int[][] field)
        {
            SudokuField[,] board = new SudokuField[boardProperties[boardProperty].BoardWidth, boardProperties[boardProperty].BoardHeight];
            for (int x = 0; x < field.Length; x++)
            {
                for (int y = 0; y < field[0].Length; y++)
                {
                    if (field[x][y] == 0)
                    {
                        board[x, y] = createSudokuField(boardProperties[boardProperty].MinNumber, boardProperties[boardProperty].MaxNumber);
                    }
                    else
                    {
                        board[x, y] = createSudokuFieldWithSetNumber(boardProperties[boardProperty].MinNumber, boardProperties[boardProperty].MaxNumber, field[x][y]);
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


        internal class BoardPropertySet
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
            internal List<int> getGuessOptions(SudokuField field)
            {
                List<int> options = (List<int>)field.GetNumbers();
                options.RemoveAll(number => failedGuesses.Contains(number));
                return options;
            }
        }
    }
}

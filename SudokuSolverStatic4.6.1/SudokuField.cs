using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    internal class SudokuField
    {
        internal enum Certainty
        {
            Set,
            FiguredOut,
            FiguredOutOnGuess,
            Guess,
            CanBe,
            CanNotBe,
            CanNotBeOnGuess
        }

        private const Certainty defaultCertaintySetNumber = Certainty.FiguredOut;
        private const Certainty defaultCertaintySetNumbers = Certainty.CanNotBe;
        private const string exceptionNotMoreThenOneOfCertainty = "more then one number can not hold the certainty ";
        private const string exceptionIsSet = "this field is already set and should not have it's possible numbers read in all likelyhood";
        List<Option> Options;

        private SudokuField()
        {
            Options = new List<Option>();
        }
        internal SudokuField(int minNumber, int maxNumber) : this()
        {
            for (int i = minNumber; i <= maxNumber; i++)
            {
                Options.Add(new Option() { Number = i, Certainty = Certainty.CanBe });
            }
        }
        internal SudokuField(int minNumber, int maxNumber, int setNumber) : this()
        {
            for (int i = minNumber; i <= maxNumber; i++)
            {
                if (i == setNumber)
                {
                    Options.Add(new Option() { Number = i, Certainty = Certainty.Set });
                }
                else
                {
                    Options.Add(new Option() { Number = i, Certainty = Certainty.CanNotBe });
                }
            }
        }

        internal int? GetNumber()
        {
            foreach (Option option in Options)
            {
                if (CertaintyIsOfSingleInstance(option.Certainty))
                {
                    return option.Number;
                }
            }
            return null;
        }
        internal IEnumerable<int> GetNumbers()
        {
            List<int> numbers = new List<int>();
            foreach (Option option in Options)
            {
                if (option.Certainty == Certainty.CanBe)
                {
                    numbers.Add(option.Number);
                }
            }
            return numbers;
        }

        internal bool SetNumber(int number, Certainty certainty = defaultCertaintySetNumber, bool test = true)
        {
            if (CertaintyIsOfSingleInstance(certainty))
                foreach (Option option in Options)
                {
                    if (option.Number == number)
                    {
                        option.Certainty = certainty;
                    }
                    else if (certainty == Certainty.Set || certainty == Certainty.FiguredOut)
                    {
                        option.Certainty = Certainty.CanNotBe;
                    }
                    else if (certainty == Certainty.FiguredOutOnGuess)
                    {
                        option.Certainty = Certainty.CanNotBeOnGuess;
                    }
                }
            if (test == true)
            {
                return TryFigureOut();
            }
            else
            {
                return false;
            }
        }

        internal bool SetNumbers(IEnumerable<int> numbers, Certainty certainty = defaultCertaintySetNumbers)
        {
            if (numbers.Count() > 1)
            {
                if (CertaintyIsOfSingleInstance(certainty))
                {
                    throw new Exception(exceptionNotMoreThenOneOfCertainty + certainty.ToString());
                }
            }
            foreach (int number in numbers)
            {
                SetNumber(number, certainty);
            }
            return TryFigureOut();
        }

        internal void RemoveGuesses()
        {
            foreach (Option option in Options)
            {
                if (option.Certainty == Certainty.CanNotBeOnGuess || option.Certainty == Certainty.FiguredOutOnGuess)
                {
                    option.Certainty = Certainty.CanBe;
                }
            }
        }

        private bool TryFigureOut()
        {
            if (GetNumber() == null)
            {
                int? number = null;
                bool guess = false;
                foreach (Option option in Options)
                {
                    if (option.Certainty == Certainty.CanBe)
                    {
                        if (number == null)
                        {
                            number = option.Number;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (guess == false)
                    {
                        if (option.Certainty == Certainty.CanNotBeOnGuess)
                        {
                            guess = true;
                        }
                    }
                }
                if (number != null)
                {
                    Certainty certainty = Certainty.FiguredOut;
                    if (guess == true)
                    {
                        certainty = Certainty.FiguredOutOnGuess;
                    }
                    SetNumber((int)number, certainty, false);
                    return true;
                }
            }
            return false;
        }
        private bool CertaintyIsOfSingleInstance(Certainty certainty)
        {
            if (certainty == Certainty.Set || certainty == Certainty.FiguredOut || certainty == Certainty.FiguredOutOnGuess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private class Option
        {
            public int Number { get; set; }
            public Certainty Certainty { get; set; }
        }
    }
}

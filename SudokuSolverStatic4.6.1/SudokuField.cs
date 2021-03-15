using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverStatic
{
    public class SudokuField
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
        internal enum ChangeType
        {
            None,
            Excluding,
            FiguredOut
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
        internal int? GetSetNumber()
        {
            foreach (Option option in Options)
            {
                if (option.Certainty == Certainty.Set)
                {
                    return option.Number;
                }
                else if (CertaintyIsOfSingleInstance(option.Certainty))
                {
                    break;
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

        internal ChangeType SetNumber(int number, Certainty certainty = defaultCertaintySetNumber, bool test = true)
        {
            ChangeType returnValue = ChangeType.None;
            foreach (Option option in Options)
            {
                if (option.Number == number)
                {
                    if(option.Certainty == certainty)
                    {
                        return ChangeType.None;
                    }
                    else if (CertaintyIsOfSingleInstance(certainty))
                    {
                        returnValue = ChangeType.FiguredOut;
                    }
                    else
                    {
                        returnValue = ChangeType.Excluding;
                    }
                    if (certainty == Certainty.CanNotBe)
                    {
                        if (option.Certainty == Certainty.CanBe || option.Certainty == Certainty.CanNotBeOnGuess)
                        {
                            option.Certainty = certainty;
                        }
                    }
                    else if (certainty == Certainty.CanNotBeOnGuess)
                    {
                        if (option.Certainty == Certainty.CanBe)
                        {
                            option.Certainty = certainty;
                        }
                    }
                    else
                    {
                        option.Certainty = certainty;
                    }
                }
                else if (certainty == Certainty.Set || certainty == Certainty.FiguredOut)
                {
                    option.Certainty = Certainty.CanNotBe;
                }
                else if ((certainty == Certainty.FiguredOutOnGuess || certainty == Certainty.Guess) && option.Certainty == Certainty.CanBe)
                {
                    option.Certainty = Certainty.CanNotBeOnGuess;
                }
            }
            if (test == true || CertaintyIsOfSingleInstance(certainty))
            {
                bool figuredOut = TryFigureOut();
                if(figuredOut == true)
                {
                    return ChangeType.FiguredOut;
                }
            }
            return returnValue;
        }
        internal ChangeType SetNumbers(IEnumerable<int> numbers, Certainty certainty = defaultCertaintySetNumbers)
        {
            ChangeType returnValue = ChangeType.None;
            if (numbers.Count() > 1)
            {
                if (CertaintyIsOfSingleInstance(certainty))
                {
                    throw new Exception(exceptionNotMoreThenOneOfCertainty + certainty.ToString());
                }
            }
            foreach (int number in numbers)
            {
                ChangeType newChange = SetNumber(number, certainty, false);
                if(newChange != ChangeType.None)
                {
                    if(newChange != returnValue)
                    {
                        returnValue = newChange;
                    }
                }
            }
            bool figuredOut = TryFigureOut();
            if (figuredOut == true)
            {
                return ChangeType.FiguredOut;
            }
            return returnValue;
        }

        internal void RemoveGuesses()
        {
            foreach (Option option in Options)
            {
                if (option.Certainty == Certainty.Guess || option.Certainty == Certainty.CanNotBeOnGuess || option.Certainty == Certainty.FiguredOutOnGuess)
                {
                    option.Certainty = Certainty.CanBe;
                }
            }
        }
        internal void RemoveAllBut(Certainty certainty = Certainty.Set)
        {
            foreach (Option option in Options)
            {
                if (option.Certainty != certainty)
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
            if (certainty == Certainty.Set || certainty == Certainty.FiguredOut || certainty == Certainty.FiguredOutOnGuess || certainty == Certainty.Guess)
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolverStatic;
using System;

namespace SudokuSolverStaticTest
{
    [TestClass]
    public class SudokuBoardTest
    {
        SudokuBoard systemUnderTest;

        [TestMethod]
        public void SimpleHorizontalExclusionTest()
        {
            int[,] start =
            {
                {1,0 },
                {0,0 }
            };
            int[,] end =
            {
                {1,0 },
                {2,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.TwoByTwo);

            systemUnderTest.SimpleHorisontalExclusion();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void SimpleVerticalExclusionTest()
        {
            int[,] start =
            {
                {1,0 },
                {0,0 }
            };
            int[,] end =
            {
                {1,2 },
                {0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.TwoByTwo);

            systemUnderTest.SimpleVerticalExclusion();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void SimpleSquareExclusionTest()
        {
            int[,] start =
            {
                {1,0 },
                {0,0 }
            };
            int[,] end =
            {
                {1,0 },
                {2,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.TwoByTwo);

            systemUnderTest.SimpleSquareExclusion();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }

        [TestMethod]
        public void HorisontalExclusionOnOnlyPositionsThatFit()
        {
            int[,] start =
            {
                {1,0,0,0},
                {0,4,0,0},
                {3,0,0,0},
                {0,0,0,0}
            };
            int[,] end =
            {
                {1,3,0,0},
                {0,4,0,0},
                {3,0,0,0},
                {4,0,0,0}
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.FourByFour);

            systemUnderTest.SimpleSquareExclusion();
            systemUnderTest.HorizontalExclusionOnOnlyPositionsThatFit();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void VerticalExclusionOnOnlyPositionsThatFit()
        {
            int[,] start =
            {
                {1,0,3,0},
                {0,4,0,0},
                {0,0,0,0},
                {0,0,0,0}
            };
            int[,] end =
            {
                {1,0,3,4},
                {3,4,0,0},
                {0,0,0,0},
                {0,0,0,0}
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.FourByFour);

            systemUnderTest.SimpleSquareExclusion();
            systemUnderTest.VerticalExclusionOnOnlyPositionsThatFit();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void SquareExclusionOnOnlyPositionsThatFit()
        {
            int[,] start =
            {
                {0,0,1,0},
                {0,0,0,0},
                {1,0,0,0},
                {0,0,0,0}
            };
            int[,] end =
            {
                {0,0,1,0},
                {0,1,0,0},
                {1,0,0,0},
                {0,0,0,1}
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.FourByFour);

            systemUnderTest.SimpleHorisontalExclusion();
            systemUnderTest.SimpleVerticalExclusion();
            systemUnderTest.SquareExclusionOnOnlyPositionsThatFit();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }

        [TestMethod]
        public void GuessMakeFirstGuessTest()
        {
            int[,] start =
            {
                {0,0 },
                {0,0 }
            };
            int[,] endOne =
            {
                {1,0 },
                {0,0 }
            };
            int[,] endTwo =
            {
                {2,0 },
                {0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.TwoByTwo);

            systemUnderTest.MakeGuess();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, endOne) || EqualTwoDimentionalArray(outcome, endTwo));
        }
        [TestMethod]
        public void GuessMakeSecondGuessTest()
        {
            int[,] start =
            {
                {0,0,0,0 },
                {0,0,0,0 },
                {0,0,0,0 },
                {0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.FourByFour);

            systemUnderTest.MakeGuess();
            systemUnderTest.MakeGuess();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            bool testSuccessful = false;
            int[,] correctEnd = CreateCopyOfArray(start);
            correctEnd[0, 0] = outcome[0, 0];
            for (int i = 1; i <= 4; i++)
            {
                if (i != outcome[0, 0])
                {
                    correctEnd[0, 1] = i;
                    testSuccessful = EqualTwoDimentionalArray(outcome, correctEnd);
                    if (testSuccessful == true)
                    {
                        break;
                    }
                }
            }
            Assert.IsTrue(testSuccessful);
        }

        [TestMethod]
        public void BinarySolveHorizontalTestOne()
        {
            int[,] start =
            {
                {1,1,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {1,1,2,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void BinarySolveHorizontalTestTwo()
        {
            int[,] start =
            {
                {0,0,0,0,0,0,1,1 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {0,0,0,0,0,2,1,1 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void BinarySolveHorizontalTestThree()
        {
            int[,] start =
            {
                {1,0,1,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {1,2,1,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void BinarySolveVerticalTestOne()
        {
            int[,] start =
            {
                {1,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {1,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {2,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void BinarySolveVerticalTestTwo()
        {
            int[,] start =
            {
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {2,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }
        [TestMethod]
        public void BinarySolveVerticalTestThree()
        {
            int[,] start =
            {
                {1,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            int[,] end =
            {
                {1,0,0,0,0,0,0,0 },
                {2,0,0,0,0,0,0,0 },
                {1,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 }
            };
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardProperty.BinaryEightByEight);

            systemUnderTest.BinarySolve();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
        }

        [TestMethod]
        public void SimpleHorizontalTestTestSuccess()
        {
            int[,] Input =
            {
                {1,0 },
                {2,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsTrue(systemUnderTest.SimpleHorisontalTest());
        }
        [TestMethod]
        public void SimpleHorizontalTestTestFailure()
        {
            int[,] Input =
            {
                {1,0 },
                {1,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsFalse(systemUnderTest.SimpleHorisontalTest());
        }
        [TestMethod]
        public void SimpleVerticalTestTestSuccess()
        {
            int[,] Input =
            {
                {1,2 },
                {0,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsTrue(systemUnderTest.SimpleVerticalTest());
        }
        [TestMethod]
        public void SimpleVerticalTestTestFailure()
        {
            int[,] Input =
            {
                {1,1 },
                {0,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsFalse(systemUnderTest.SimpleVerticalTest());
        }
        [TestMethod]
        public void SimpleSquareTestTestSuccess()
        {
            int[,] Input =
            {
                {1,0 },
                {2,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsTrue(systemUnderTest.SimpleSquareTest());
        }
        [TestMethod]
        public void SimpleSquareTestTestFailure()
        {
            int[,] Input =
            {
                {1,0 },
                {1,0 }
            };
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardProperty.TwoByTwo);

            Assert.IsFalse(systemUnderTest.SimpleSquareTest());
        }

        private bool EqualTwoDimentionalArray(int[,] arrayOne, int[,] arrayTwo)
        {
            if(arrayOne.GetLength(0) != arrayTwo.GetLength(0) || arrayOne.GetLength(1) != arrayTwo.GetLength(1))
            {
                return false;
            }
            for (int i = 0; i < arrayOne.GetLength(0); i++)
            {
                for (int o = 0; o < arrayOne.GetLength(1); o++)
                {
                    if (arrayOne[i, o] != arrayTwo[i, o])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private int[,] CreateCopyOfArray(int[,] toCopy)
        {
            int[,] copy = new int[toCopy.GetLength(0), toCopy.GetLength(1)];
            for(int x = 0; x < copy.GetLength(0); x++)
            {
                for (int y = 0; y < copy.GetLength(1); y++)
                {
                    copy[x,y] = toCopy[x,y];
                }
            }
            return copy;
        }
    }
}

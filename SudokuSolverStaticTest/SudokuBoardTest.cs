using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolverStatic;
using System;

namespace SudokuSolverStaticTest
{
    [TestClass]
    public class SudokuBoardTest
    {
        SudokuBoard systemUnderTest;

        //in arrays the first number is the x value and the second is the y. because of this how they appear here they look over the y = -x line flipped.
        
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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.FourByFour);

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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.FourByFour);

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
            systemUnderTest = new SudokuBoard(start, SudokuBoard.BoardPropperty.FourByFour);

            systemUnderTest.SimpleHorisontalExclusion();
            systemUnderTest.SimpleVerticalExclusion();
            systemUnderTest.SquareExclusionOnOnlyPositionsThatFit();
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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
            systemUnderTest = new SudokuBoard(Input, SudokuBoard.BoardPropperty.TwoByTwo);

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
    }
}

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
        public void HorizontalExclusionTest()
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

            systemUnderTest.SimpleHorizontalExclusion();
            int[,] outcome = systemUnderTest.GetAsMultidimentionalArray();

            Assert.IsTrue(EqualTwoDimentionalArray(outcome, end));
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

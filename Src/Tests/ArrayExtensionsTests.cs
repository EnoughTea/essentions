using NUnit.Framework;


namespace Essentions.Tests
{
    [TestFixture]
    public class ArrayExtensionsTests
    {
        [Test]
        public void TestConcatenation()
        {
            int[] alpha = { 1, 2, 3 };
            int[] omega = { 4, 5, 6 };

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, alpha.Concat(omega));
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 1, 2, 3 }, omega.Concat(alpha));
        }

        [Test]
        public void TestEmptyConcatenation()
        {
            int[] alpha = { 1, 2, 3 };
            int[] empty = new int[0];
            int[] expected = { 1, 2, 3 };

            int[] result = alpha.Concat(empty);
            CollectionAssert.AreEqual(expected, result);

            result = empty.Concat(alpha);
            CollectionAssert.AreEqual(expected, result);

            result = empty.Concat(empty);
            CollectionAssert.AreEqual(new int[0], result);

            result = alpha.Concat(null);
            CollectionAssert.AreEqual(alpha, result);
        }

        [Test]
        public void TestFilling2D()
        {
            int width = 32;
            int height = 16;

            int[,] expected = new int[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    expected[i, j] = i * j;

            int[,] result = new int[width, height];
            result.Fill((x, y, old) => x * y);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFilling3D()
        {
            int width = 32;
            int height = 16;
            int depth = 8;

            int[,,] expected = new int[width, height, depth];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        expected[i, j, k] = i * j * k;

            int[,,] result = new int[width, height, depth];
            result.Fill((x, y, z, old) => x * y * z);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestJaggedSlice()
        {
            int[] alpha = new[] { 1, 2, 3, 4, 5, 6 };
            int[][] result = alpha.JaggedSlice(4);
            int[] expected1 = new[] { 1, 2, 3, 4 };
            int[] expected2 = new[] { 5, 6 };

            CollectionAssert.AreEqual(expected1, result[0]);
            CollectionAssert.AreEqual(expected2, result[1]);

            result = alpha.JaggedSlice(8);
            CollectionAssert.AreEqual(alpha, result[0]);
        }


        [Test]
        public void Test2DToJagged()
        {
            int width = 8;
            int height = 4;
            int[,] alpha = new int[width, height];

            for (int i = alpha.GetLowerBound(0); i <= alpha.GetUpperBound(0); i++)
                for (int j = alpha.GetLowerBound(1); j <= alpha.GetUpperBound(1); j++)
                    alpha[i, j] = i;

            int[][] result = alpha.ToJaggedArray();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    Assert.AreEqual(result[i][j], i);
        }

        [Test]
        public void Test3DToJagged()
        {
            int width = 8;
            int height = 4;
            int depth = 2;

            int[,,] alpha = new int[width, height, depth];

            for (int i = alpha.GetLowerBound(0); i <= alpha.GetUpperBound(0); i++)
                for (int j = alpha.GetLowerBound(1); j <= alpha.GetUpperBound(1); j++)
                    for (int k = alpha.GetLowerBound(2); k <= alpha.GetUpperBound(2); k++)
                        alpha[i, j, k] = i;

            int[][][] result = alpha.ToJaggedArray();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        Assert.AreEqual(result[i][j][k], i);
        }
    }
}
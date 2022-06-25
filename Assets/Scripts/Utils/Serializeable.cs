using System;

namespace Serializeable
{
    [Serializable]
    public class IntArray
    {
        private int[] data;

        public IntArray(int size)
        {
            data = new int[size];
        }

        public static implicit operator int[](IntArray arr) => arr.data;
    }

    [Serializable]
    public class IntMatrix
    {
        private IntArray[] data;

        public IntMatrix(int xSize, int ySize)
        {
            data = new IntArray[xSize];
            for(int i = 0; i < xSize; i++)
                data[i] = new IntArray(ySize);
        }

        public int GetLength(int rank)
        {
            switch(rank)
            {
                case 0:
                    return data.Length;
                case 1:
                    return ((int[])data[0]).Length;
                default:
                    throw new ArgumentException($"Rank {rank} does not exist on matrix.");
            }
        }

        public int[] this[int x] => data[x];
    }
}
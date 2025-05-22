using UnityEngine;

public static class RandomIntArrayGeneration
{
    public static int[] Generate(int lenght)
    {
        int[] array = new int[lenght];
        for (int i = 0; i < lenght; i++)
        {
            array[i] = i;
        }
        return Shuffle(array);
    }

    public static int[] Shuffle(int[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return array;
    }
}

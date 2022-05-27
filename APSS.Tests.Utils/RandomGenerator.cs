using System.Text;

namespace APSS.Tests.Utils;

public static class RandomGenerator
{
    private static readonly Random Rand = new();

    private const string LowercaseAlphabet = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "1234567890";
    private const string Symbols = "`~!@#$%^&*()-_=+\"\\/?.>,<";

    /// <summary>
    /// Generates a random string value
    /// </summary>
    /// <param name="length">The length of the string</param>
    /// <param name="opts">The options used to build the pool</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="RandomStringOptions.None"/> is used
    /// </exception>
    public static string NextString(
        int length,
        RandomStringOptions opts = RandomStringOptions.Alpha | RandomStringOptions.Mixedcase)
    {
        var poolBuilder = new StringBuilder();

        if (opts.HasFlag(RandomStringOptions.Alpha))
        {
            if (!opts.HasFlag(RandomStringOptions.Lowercase) &&
                !opts.HasFlag(RandomStringOptions.Uppercase))
            {
                poolBuilder.Append(LowercaseAlphabet);
            }
            else
            {
                if (opts.HasFlag(RandomStringOptions.Lowercase))
                    poolBuilder.Append(LowercaseAlphabet);

                if (opts.HasFlag(RandomStringOptions.Uppercase))
                    poolBuilder.Append(UppercaseAlphabet);
            }
        }

        if (opts.HasFlag(RandomStringOptions.Numeric))
            poolBuilder.Append(Numbers);

        if (opts.HasFlag(RandomStringOptions.Symbol))
            poolBuilder.Append(Symbols);

        var pool = poolBuilder.ToString();

        return new string(Enumerable
            .Range(0, length)
            .Select(i => pool[Rand.Next(pool.Length - 1)])
            .ToArray());
    }

    /// <summary>
    /// Generates a random 32-bit integer
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static int NextInt(int min = int.MinValue, int max = int.MaxValue)
        => Rand.Next(min, max);

    /// <summary>
    /// Generates a random 64-bit integer
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static long NextLong(long min = long.MinValue, long max = long.MaxValue)
        => Rand.NextInt64(min, max);

    /// <summary>
    /// Generates a random 64-bit floating point number
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static double NextDouble(double min = double.MinValue, double max = double.MaxValue)
        => Rand.NextDouble() * (max - min) + min;
}

/// <summary>
/// An enum to describe the options that can be used for string generation
/// </summary>
[Flags]
public enum RandomStringOptions : int
{
    Alpha = 1,
    Numeric = 2,
    Symbol = 4,
    AlphaNumeric = Alpha | Numeric,
    Lowercase = 8,
    Uppercase = 16,
    Mixedcase = Lowercase | Uppercase,
    Mixed = Alpha | Numeric | Lowercase | Uppercase | Symbol,
};

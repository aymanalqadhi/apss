using System.Text;

namespace APSS.Tests.Utils;

public static class RandomGenerator
{
    private static readonly Random _rnd = new();

    private const string _lowercaseAlphabet = "abcdefghijklmnopqrstuvwxyz";
    private const string _uppercaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string _numbers = "1234567890";
    private const string _symbols = "`~!@#$%^&*()-_=+\"\\/?.>,<";

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
                poolBuilder.Append(_lowercaseAlphabet);
            }
            else
            {
                if (opts.HasFlag(RandomStringOptions.Lowercase))
                    poolBuilder.Append(_lowercaseAlphabet);

                if (opts.HasFlag(RandomStringOptions.Uppercase))
                    poolBuilder.Append(_uppercaseAlphabet);
            }
        }

        if (opts.HasFlag(RandomStringOptions.Numeric))
            poolBuilder.Append(_numbers);

        if (opts.HasFlag(RandomStringOptions.Symbol))
            poolBuilder.Append(_symbols);

        var pool = poolBuilder.ToString();

        return new string(Enumerable
            .Range(0, length)
            .Select(i => pool[_rnd.Next(pool.Length - 1)])
            .ToArray());
    }

    /// <summary>
    /// Generates a random 32-bit integer
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static int NextInt(int min = int.MinValue, int max = int.MaxValue)
        => _rnd.Next(min, max);

    /// <summary>
    /// Generates a random 64-bit integer
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static long NextLong(long min = long.MinValue, long max = long.MaxValue)
        => _rnd.NextInt64(min, max);

    /// <summary>
    /// Generates a random 64-bit floating point number
    /// </summary>
    /// <param name="min">The minimum boundary</param>
    /// <param name="max">The maximum boundary</param>
    /// <returns>The generated value</returns>
    public static double NextDouble(double min = double.MinValue, double max = double.MaxValue)
        => _rnd.NextDouble() * (max - min) + min;

    /// <summary>
    /// Generates a random boolean value
    /// </summary>
    /// <returns>The generated value</returns>
    public static bool NextBool()
        => NextInt(0, 1) == 1;
}

/// <summary>
/// An enum to describe the options that can be used for string generation
/// </summary>
[Flags]
public enum RandomStringOptions : int
{
    Alpha = 1 << 0,
    Numeric = 1 << 1,
    Symbol = 1 << 3,
    AlphaNumeric = Alpha | Numeric,
    Lowercase = 1 << 4,
    Uppercase = 1 << 5,
    Mixedcase = Lowercase | Uppercase,
    Mixed = Alpha | Numeric | Lowercase | Uppercase | Symbol,
};
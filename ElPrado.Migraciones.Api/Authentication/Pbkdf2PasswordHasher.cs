using System.Security.Cryptography;

namespace ElPrado.Migraciones.Api.Authentication;

public static class Pbkdf2PasswordHasher
{
    private const int SaltLength = 16;
    private const int HashLength = 32;
    private const string Version = "pbkdf2-sha512-v1";

    public static string Hash(string secret, int iterations = 210000)
    {
        ValidateIterations(iterations);
        ArgumentException.ThrowIfNullOrEmpty(secret);
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations, HashAlgorithmName.SHA512, HashLength);
        return $"{Version}${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string secret, string encodedHash)
    {
        if (string.IsNullOrEmpty(secret) || string.IsNullOrWhiteSpace(encodedHash)) return false;
        var fields = encodedHash.Split('$');
        if (fields.Length != 4 || fields[0] != Version || !int.TryParse(fields[1], out var iterations)) return false;
        try
        {
            ValidateIterations(iterations);
            var salt = Convert.FromBase64String(fields[2]);
            var expected = Convert.FromBase64String(fields[3]);
            if (salt.Length != SaltLength || expected.Length != HashLength) return false;
            var actual = Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations, HashAlgorithmName.SHA512, HashLength);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
        catch (ArgumentException) { return false; }
    }

    public static bool IsValidHash(string encodedHash)
    {
        var fields = encodedHash?.Split('$');
        if (fields is null || fields.Length != 4 || fields[0] != Version || !int.TryParse(fields[1], out var iterations)) return false;
        try
        {
            ValidateIterations(iterations);
            return Convert.FromBase64String(fields[2]).Length == SaltLength && Convert.FromBase64String(fields[3]).Length == HashLength;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException) { return false; }
    }

    private static void ValidateIterations(int iterations)
    {
        if (iterations is < 210000 or > 1000000) throw new ArgumentOutOfRangeException(nameof(iterations));
    }
}

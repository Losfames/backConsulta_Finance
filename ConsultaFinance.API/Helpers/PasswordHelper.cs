using System.Security.Cryptography;

namespace ConsultaFinance.API.Helpers;

public static class PasswordHelper
{
    private const string HashAlgorithm = "PBKDF2";
    private const int Iterations = 150_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const char Delimiter = '$';

    public static string HashPassword(string password)
    {
        if (password is null)
            throw new ArgumentNullException(nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return string.Join(Delimiter, HashAlgorithm, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (password is null)
            throw new ArgumentNullException(nameof(password));

        if (!TryParseHash(hashedPassword, out var iterations, out var salt, out var hash))
            return false;

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hash.Length);
        return CryptographicOperations.FixedTimeEquals(computedHash, hash);
    }

    public static bool IsHashedPassword(string hashedPassword)
        => TryParseHash(hashedPassword, out _, out _, out _);

    private static bool TryParseHash(string hashedPassword, out int iterations, out byte[] salt, out byte[] hash)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        hash = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 4 || parts[0] != HashAlgorithm)
            return false;

        if (!int.TryParse(parts[1], out iterations))
            return false;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            hash = Convert.FromBase64String(parts[3]);
            return salt.Length >= SaltSize && hash.Length >= 16;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

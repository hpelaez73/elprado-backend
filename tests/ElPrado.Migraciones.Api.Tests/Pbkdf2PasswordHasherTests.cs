using ElPrado.Migraciones.Api.Authentication;
using Xunit;

namespace ElPrado.Migraciones.Api.Tests;

public sealed class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void Hash_verifies_only_the_original_secret() =>
        Assert.True(Pbkdf2PasswordHasher.Verify("secret", Pbkdf2PasswordHasher.Hash("secret")));

    [Fact]
    public void Invalid_hash_is_rejected() => Assert.False(Pbkdf2PasswordHasher.Verify("secret", "invalid"));
}

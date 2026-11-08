using ElPrado.Migraciones.Api.Authentication;

Console.Error.Write("Credencial: ");
var characters = new List<char>();
for (ConsoleKeyInfo key; (key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter;)
{
    if (key.Key == ConsoleKey.Backspace)
    {
        if (characters.Count > 0) characters.RemoveAt(characters.Count - 1);
    }
    else if (!char.IsControl(key.KeyChar)) characters.Add(key.KeyChar);
}
Console.Error.WriteLine();
var buffer = characters.ToArray();
var secret = new string(buffer);
try { Console.Out.WriteLine(Pbkdf2PasswordHasher.Hash(secret)); }
finally { Array.Clear(buffer, 0, buffer.Length); }

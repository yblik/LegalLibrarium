using System;

public static class AlphaNum2
{
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // Convert integer → "B2"
    public static string ToAlphaNum(int number)
    {
        if (number < 0 || number >= 260)
            throw new ArgumentOutOfRangeException(nameof(number), "Must be between 0 and 259.");

        int letterIndex = number / 10;
        int digit = number % 10;

        char letter = Letters[letterIndex];
        char digitChar = (char)('0' + digit);

        return $"{letter}{digitChar}";
    }

    // Convert "B2" → integer
    public static int FromAlphaNum(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
            throw new ArgumentException("Code must be exactly 2 characters like 'B2'.");

        char letter = code[0];
        char digitChar = code[1];

        int letterIndex = Letters.IndexOf(letter);
        if (letterIndex < 0)
            throw new ArgumentException("Invalid letter in code.");

        if (!char.IsDigit(digitChar))
            throw new ArgumentException("Second character must be a digit 0–9.");

        int digit = digitChar - '0';

        return (letterIndex * 10) + digit;
    }
}
//examples 
//AlphaNum2.ToAlphaNum(12);   // "B2"
//AlphaNum2.ToAlphaNum(39);   // "D9"
//AlphaNum2.ToAlphaNum(45);   // "E5"

//AlphaNum2.FromAlphaNum("B2"); // 12
//AlphaNum2.FromAlphaNum("D9"); // 39
//AlphaNum2.FromAlphaNum("E5"); // 45
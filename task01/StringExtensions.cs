using System;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        int left = 0;
        int right = input.Length - 1;
        bool hasValidChars = false;

        while (left <= right)
        {
            char leftChar = input[left];
            char rightChar = input[right];

            if (char.IsPunctuation(leftChar) || char.IsWhiteSpace(leftChar))
            {
                left++;
                continue;
            }

            if (char.IsPunctuation(rightChar) || char.IsWhiteSpace(rightChar))
            {
                right--;
                continue;
            }

            hasValidChars = true;

            if (char.ToLowerInvariant(leftChar) != char.ToLowerInvariant(rightChar))
            {
                return false;
            }

            left++;
            right--;
        }

        return hasValidChars;
    }
}

using System;
using System.Text;

namespace TodoApi.Utility
{
    public static class Generator
    {
        public static string? GenerateSimpleToken(params object[]? args)
        {
            StringBuilder result = new();
            Random gen = new();
            for (int i = 0; i < 20; ++i)
                result.Append(((char)gen.Next(0, 255)));
            return result.ToString();
        }
    }
}
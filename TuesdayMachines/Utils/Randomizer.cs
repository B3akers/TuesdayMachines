﻿using System.Security.Cryptography;

namespace TuesdayMachines.Utils
{
    public static class Randomizer
    {
        public static int Next()
        {
            return BitConverter.ToInt32(RandomNumberGenerator.GetBytes(4));
        }

        public static int Next(int maxValue)
        {
            return RandomNumberGenerator.GetInt32(maxValue);
        }

        public static byte[] GetBytes(int value)
        {
            return RandomNumberGenerator.GetBytes(value);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_-";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Next(s.Length)]).ToArray());
        }

        public static string RandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz(~!@#$%^&*_-+=`|(){}[]:;<>,.?/";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Next(s.Length)]).ToArray());
        }

        public static string RandomReadableString(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Next(s.Length)]).ToArray());
        }
    }
}

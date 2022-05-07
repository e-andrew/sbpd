﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Cerberus.core
{ 
    public class CryptographyModule
    {
        private static readonly Random Random = new Random();
        private static int Generate(int a, int b)
        {
            return Random.Next(a, b);
        }
        public static string GenerateSalt(int size)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            while (size > 0)
            {
                int category = Generate(0, 3);
                if (category == 0)
                {
                    stringBuilder.Append((char) Generate(48, 58));
                } else if (category == 1)
                {
                    stringBuilder.Append((char) Generate(64, 91));
                }
                else
                {
                    stringBuilder.Append((char) Generate(97, 123));
                }
                
                size -= 1;
            }
            
            return stringBuilder.ToString();
        }

        public static string HashPassword(string password, string salt)
        {
            SHA512 hasher = SHA512.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.ASCII.GetBytes(password + salt))).Replace("-", "");
        }
    }
}
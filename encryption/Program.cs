using System;
using System.Collections;
using System.Numerics;

namespace CerberusEncrypted
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            Cryptography cryptography = new Cryptography();
            ArrayList keys = cryptography.RSA(BigInteger.Parse("845840720572075245454"), 5);
            BigInteger message = BigInteger.Parse("999999349730734049999999949494933");
            BigInteger encryptedMessage = cryptography.Encrypt(message, (BigInteger) keys[0], (BigInteger) keys[2]);
            BigInteger decryptedMessage = cryptography.Decrypt(encryptedMessage, (BigInteger) keys[1], (BigInteger) keys[2]);
            Console.WriteLine("=== Encryption scheme RSA ===");
            Console.WriteLine($" Ko = {keys[0]}");
            Console.WriteLine($" Kc = {keys[1]}");
            Console.WriteLine($" P = {keys[2]}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"Encrypted Message: {encryptedMessage}");
            Console.WriteLine($"Decrypted Message: {decryptedMessage}");
            Console.WriteLine($"Is Same: {BigInteger.Compare(message, decryptedMessage) == 0}");
            
            cryptography.ElectronicCodeBook("input_ecb.txt", "close_ecb.txt", true);
            cryptography.ElectronicCodeBook("close_ecb.txt", "open_ecb.txt", false);
            
            cryptography.CipherBlockChaining("input_cbc.txt", "close_cbc.txt", true);
            cryptography.CipherBlockChaining("close_cbc.txt", "open_cbc.txt", false);

            Console.ReadLine();
        }
    }
}
using System;
using System.Collections;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace CerberusEncrypted
{
    public class Cryptography
    {
        private DES alg;

        public Cryptography()
        {
            alg = DES.Create();
            alg.BlockSize = 64;
            alg.KeySize = 64;
            alg.FeedbackSize = 64;
        }
        private BigInteger Fdp(BigInteger value, BigInteger exponent, BigInteger divisor)
        {
            byte[] octets = exponent.ToByteArray();
            BigInteger remainder = BigInteger.One;
            
            for (int i = octets.Length - 1; i > -1; i--)
            {
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 128) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 64) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 32) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 16) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 8) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 4) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 2) == 0 ? BigInteger.One : value), divisor);
                remainder = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Multiply(remainder, remainder), (octets[i] & 1) == 0 ? BigInteger.One : value), divisor);
            }
            return remainder;
        }
        private bool FermatSmallTheorem(BigInteger p)
        {
            return BigInteger.Compare(Fdp(new BigInteger(2), BigInteger.Subtract(p, BigInteger.One), p), BigInteger.One) == 0;
        }

        private ArrayList LargePrimeGenerator(BigInteger pMax, int a)
        {
            ArrayList primes = new ArrayList();
            BigInteger two = new BigInteger(2);
            var K = (int) Math.Ceiling(BigInteger.Log(BigInteger.Divide(BigInteger.Add(pMax, BigInteger.One), two), a));
          
            BigInteger main = BigInteger.Multiply(two, BigInteger.Pow(new BigInteger(a), K)); 
            BigInteger p1 = BigInteger.Add(main, BigInteger.One);
            BigInteger p2 = BigInteger.Subtract(main, BigInteger.One);
            
            while (primes.Count < 2)
            {
                if (FermatSmallTheorem(p1))
                {
                    primes.Add(p1);
                }

                if (FermatSmallTheorem(p2))
                {
                    primes.Add(p2);
                }
                
                p1 = BigInteger.Add(p1, BigInteger.One);
                p2 = BigInteger.Subtract(p2, BigInteger.One);
            }
            
            return new ArrayList(primes);
        }

        private BigInteger NSD(BigInteger a, BigInteger b)
        {
            return BigInteger.Compare(b, BigInteger.Zero) == 0 ? a : NSD(b, BigInteger.Remainder(a, b));
        }

        public ArrayList RSA(BigInteger pMax, int a)
        {
            ArrayList primes = LargePrimeGenerator(pMax, a);
            BigInteger p = (BigInteger) primes[0];
            BigInteger q = (BigInteger) primes[1];
            BigInteger P = BigInteger.Multiply(p, q);
            BigInteger F = BigInteger.Multiply(BigInteger.Subtract(p, BigInteger.One),
                BigInteger.Subtract(q, BigInteger.One));
            
            BigInteger Ko = new BigInteger((int) Math.Ceiling(BigInteger.Log(P, 2)));
            while (BigInteger.Compare(Ko, p) == 0 || BigInteger.Compare(Ko, q) == 0 || BigInteger.Compare(NSD(Ko, F), BigInteger.One) != 0)
            {
                Ko = BigInteger.Add(Ko, BigInteger.One);
            }

            int z = 1;
            while (BigInteger.Compare(BigInteger.Remainder(BigInteger.Add(BigInteger.Multiply(new BigInteger(z), F), BigInteger.One), Ko), BigInteger.Zero) != 0)
            {
                z += 1;
            }

            BigInteger Kc = BigInteger.Divide(BigInteger.Add(BigInteger.Multiply(new BigInteger(z), F), BigInteger.One), Ko);
            return new ArrayList {Ko, Kc, P};
        }
        
        public BigInteger Encrypt(BigInteger S, BigInteger Ko, BigInteger P)
        {
            return Fdp(S, Ko, P);
        }
        
        public BigInteger Decrypt(BigInteger E, BigInteger Kc, BigInteger P)
        {
            return Fdp(E, Kc, P);
        }
        private static void EncryptBetweenFiles(DES alg, byte[] key, byte[] initialVector, String inputFilename, String outputFilename) 
        { 
            try
            {
                FileStream iStream = File.Open(inputFilename, FileMode.OpenOrCreate);
                FileStream oStream = File.Open(outputFilename,FileMode.OpenOrCreate);
                
                StreamReader sReader = new StreamReader(iStream);
                CryptoStream cStream = new CryptoStream(oStream,
                    alg.CreateEncryptor(key,initialVector),
                    CryptoStreamMode.Write);
                
                StreamWriter sWriter = new StreamWriter(cStream);

                while (true)
                {
                    String line = sReader.ReadLine();
                    if (line != null)
                    {
                        sWriter.WriteLine(line);
                    }
                    else
                    {
                        break;
                    }
                }
                
                sWriter.Close();
                cStream.Close();
                sReader.Close();
                oStream.Close();
                iStream.Close();
            }
            catch(CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            }
            catch(UnauthorizedAccessException  e)
            {
                Console.WriteLine("A file error occurred: {0}", e.Message);
            }
        }
        private static void DecryptBetweenFiles(DES alg, byte[] key, byte[] initialVector, String inputFilename, String outputFilename)
        {
            try
            {
                FileStream iStream = File.Open(inputFilename, FileMode.OpenOrCreate);
                FileStream oStream = File.Open(outputFilename,FileMode.OpenOrCreate);
                
                CryptoStream cStream = new CryptoStream(iStream,
                    alg.CreateDecryptor(key,initialVector),
                    CryptoStreamMode.Read);
                
                StreamReader sReader = new StreamReader(cStream);
                StreamWriter sWriter = new StreamWriter(oStream);
                
                while (true)
                {
                    String line = sReader.ReadLine();
                    if (line != null)
                    {
                        sWriter.WriteLine(line);
                    }
                    else
                    {
                        break;
                    }
                }

                sWriter.Close();
                sReader.Close();
                cStream.Close();
                oStream.Close();
                iStream.Close();
            }
            catch(CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            }
            catch(UnauthorizedAccessException  e)
            {
                Console.WriteLine("A file error occurred: {0}", e.Message);
            } 
        }
        public void ElectronicCodeBook(String inputFilename, String outputFilename, bool isEncrypt)
        {
            try
            {
                alg.Mode = CipherMode.ECB;

                if (isEncrypt)
                {
                    EncryptBetweenFiles(alg, alg.Key, alg.IV, inputFilename, outputFilename);
                }
                else
                {
                    DecryptBetweenFiles(alg, alg.Key, alg.IV, inputFilename, outputFilename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void CipherBlockChaining(String inputFilename, String outputFilename, bool isEncrypt)
        {
            alg.Mode = CipherMode.CBC;
            if (isEncrypt)
            {
                EncryptBetweenFiles(alg, alg.Key, alg.IV, inputFilename, outputFilename);
            }
            else
            {
                DecryptBetweenFiles(alg, alg.Key, alg.IV, inputFilename, outputFilename);
            }
        }
    }
}
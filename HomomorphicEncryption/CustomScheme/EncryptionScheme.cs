// -----------------------------------------------------------------------
//  <copyright company="Microsoft Corporation">
//       Copyright (C) Microsoft Corporation. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;

namespace HomomorphicEncryption.CustomScheme
{
    /// <summary>
    /// Encryption scheme.
    /// </summary>
    public class EncryptionScheme
    {
        private static readonly Random _random = new Random();
        private static readonly int keyLength = 8;

        private static readonly double epsilon = 0.15;

        public double[] Encrypt(int bit, int[] key)
        {
            ValidateKey(key);

            var cipher = new double[keyLength];
            double dot = 0;

            while (true)
            {
                for (var i = 0; i < keyLength - 1; i++)
                {
                    // Generate between -1 and 1;
                    var next = (_random.NextDouble() * 2) - 1;
                    cipher[i] = next;

                    dot += next * key[i];
                }

                if (dot < 0 || dot > 1)
                {
                    dot = 0;
                    continue;
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                var rand = (_random.NextDouble() * 2) - 1;

                var dotProduct = dot + rand * key[keyLength - 1];

                if (Math.Abs(bit - dotProduct % 2) < epsilon)
                {
                    cipher[keyLength - 1] = rand;
                    break;
                }
            }

            return cipher;
        }

        public void Encrypt(string data, int[] key)
        {
            var bitArrays = data.Select(c => new BitArray(BitConverter.GetBytes(c)));

            // Each character should be two bytes = 16 bits.
            if (bitArrays.Any(a => a.Length != 16)) throw new ArgumentException("Unexpected bit array length");

            foreach (var bitArray in bitArrays)
            {
                for (var i = 0; i < bitArray.Length; i++)
                {
                    var bit = Convert.ToInt32(bitArray.Get(i));
                    var encrypted = Encrypt(bit, key);
                }
            }

            // TODO: Complete.
        }

        public double[] AddCiphertext(double[] cipher1, double[] cipher2)
        {
            var sum = new double[cipher1.Length];

            for (var i = 0; i < cipher1.Length; i++)
            {
                sum[i] = (cipher1[i] + cipher1[2]);
            }

            return sum;
        }

        public double[] MultiplyCiphertext(double[] cipher1, double[] cipher2)
        {
            var product = new double[cipher1.Length];

            for (var i = 0; i < cipher1.Length; i++)
            {
                product[i] = (cipher1[i] * cipher1[2]) % 2;
            }

            return product;
        }

        public int Decrypt(double[] cipher, int[] key)
        {
            double dot = 0;

            for (var i = 0; i < cipher.Length; i++)
            {
                dot += cipher[i] * key[i];
            }

            return MathMod((int)Math.Round(dot), 2);
        }

        private void ValidateKey(int[] key)
        {
            if (key.Length != keyLength || !key.All(i => i == 0 || i == 1))
            {
                throw new ArgumentException("KeyInvalid");
            }
        }

        static int MathMod(int a, int b)
        {
            return (Math.Abs(a * b) + a) % b;
        }

    }
}

// -----------------------------------------------------------------------
//  <copyright company="Microsoft Corporation">
//       Copyright (C) Microsoft Corporation. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using HomomorphicEncryption.CustomScheme;

namespace HomomorphicEncryption
{
    /// <summary>
    /// 
    /// </summary>
    public class EncryptionScheme2Test
    {
        public static void CustomEncryptionTest()
        {
            var secret = 10061;
            var scheme = new EncryptionScheme2();

            Console.WriteLine("###################### Encryption/Decryption Test ######################");

            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine("---------------");
                var bitToEncrypt = i % 2;

                var ctx = scheme.Encrypt(bitToEncrypt, secret);
                var decrypted = scheme.Decrypt(ctx, secret);

                Console.WriteLine($"Bit\t{bitToEncrypt}");
                Console.WriteLine($"Cipher\t{ctx}");
                Console.WriteLine($"Decrypt\t{decrypted}");

                if (decrypted != bitToEncrypt)
                {
                    throw new InvalidOperationException("Decryption failed.");
                }

                Console.WriteLine();
            }

            Console.WriteLine("###################### Encrypted Addition Test 1 (1 + 1 = 0) ######################");

            for (var i = 0; i < 5; i++)
            {
                var bit1 = scheme.Encrypt(1, secret);
                var bit2 = scheme.Encrypt(1, secret);

                var sum = scheme.AddCiphertext(bit1, bit2);

                var decrypted = scheme.Decrypt(sum, secret);

                Console.WriteLine("---------------");

                Console.WriteLine($"Cipher 1\t{string.Join(',', bit1)}");
                Console.WriteLine($"Cipher 2\t{string.Join(',', bit2)}");
                Console.WriteLine($"Cipher sum\t{string.Join(',', sum)}");
                Console.WriteLine($"Decrypt\t{decrypted}");

                if (decrypted != 0)
                {
                    throw new InvalidOperationException("Encrypted addition test failed");
                }

                Console.WriteLine();
            }

            Console.WriteLine("###################### Encrypted Addition Test 2 (1 + 0 = 1) ######################");

            for (var i = 0; i < 5; i++)
            {
                var bit1 = scheme.Encrypt(1, secret);
                var bit2 = scheme.Encrypt(0, secret);

                var sum = scheme.AddCiphertext(bit1, bit2);

                var decrypted = scheme.Decrypt(sum, secret);

                Console.WriteLine("---------------");

                Console.WriteLine($"Cipher 1\t{string.Join(',', bit1)}");
                Console.WriteLine($"Cipher 2\t{string.Join(',', bit2)}");
                Console.WriteLine($"Cipher sum\t{string.Join(',', sum)}");
                Console.WriteLine($"Decrypt\t{decrypted}");

                if (decrypted != 1)
                {
                    throw new InvalidOperationException("Encrypted addition test 2 failed");
                }

                Console.WriteLine();
            }

            Console.WriteLine("###################### Encrypted Multiplication Test 1 (1 * 0 = 0) ######################");

            for (var i = 0; i < 5; i++)
            {
                var bit1 = scheme.Encrypt(1, secret);
                var bit2 = scheme.Encrypt(0, secret);

                var product = scheme.MultiplyCiphertext(bit1, bit2);

                var decrypted = scheme.Decrypt(product, secret);

                Console.WriteLine("---------------");

                Console.WriteLine($"Cipher 1\t{string.Join(',', bit1)}");
                Console.WriteLine($"Cipher 2\t{string.Join(',', bit2)}");
                Console.WriteLine($"Cipher prod\t{string.Join(',', product)}");
                Console.WriteLine($"Decrypt\t{decrypted}");

                if (decrypted != 0)
                {
                    throw new InvalidOperationException("Encrypted multiplication test 1 failed");
                }

                Console.WriteLine();
            }

            Console.WriteLine("###################### Encrypted Multiplication Test 2 (1 * 1 = 1) ######################");

            for (var i = 0; i < 5; i++)
            {
                var bit1 = scheme.Encrypt(1, secret);
                var bit2 = scheme.Encrypt(1, secret);

                var product = scheme.MultiplyCiphertext(bit1, bit2);

                var decrypted = scheme.Decrypt(product, secret);

                Console.WriteLine("---------------");

                Console.WriteLine($"Cipher 1\t{string.Join(',', bit1)}");
                Console.WriteLine($"Cipher 2\t{string.Join(',', bit2)}");
                Console.WriteLine($"Cipher prod\t{string.Join(',', product)}");
                Console.WriteLine($"Decrypt\t{decrypted}");

                if (decrypted != 1)
                {
                    throw new InvalidOperationException("Encrypted multiplication test 2 failed");
                }

                Console.WriteLine();
            }

            Console.WriteLine("###################### Encrypted Multiplication Noise Extension Test ######################");

            var done = false;

            // Failed at around 900 comps
            for (var computations = 1; computations < 1000; computations++)
            {
                var ct = scheme.Encrypt(1, secret);

                if (done)
                {
                    break;
                }

                for (var i = 0; i < computations; i++)
                {
                    ct = scheme.AddCiphertext(ct, scheme.Encrypt(1, secret));

                    if (ct < 0) throw new OverflowException($"Computations {i}");

                    // Adding a sequence of ones.
                    if (scheme.Decrypt(ct, secret) != (i % 2 == 0 ? 0 : 1))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Error too high at {i} computations");
                        Console.ResetColor();
                        done = true;
                        break;
                    }
                }
            }
        }
    }
}

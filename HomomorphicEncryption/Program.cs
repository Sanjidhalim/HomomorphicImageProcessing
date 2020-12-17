using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using HomomorphicEncryption.Client;
using HomomorphicEncryption.CustomScheme;
using ImageProcessor;

namespace HomomorphicEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            // Implementation challenges - batch size of image too big
            // using vectors in CKKS.
            // lack of branching.
            Console.WriteLine("Hello World!");

            CustomEncryptionTest();
        }

        private static void CustomEncryptionTest()
        {
            var secret = new int[] { 1, 1, 0, 1, 0, 0, 1, 1 };
            var scheme = new EncryptionScheme();

            Console.WriteLine("###################### Encryption/Decryption Test ######################");

            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine("---------------");
                var bitToEncrypt = i % 2;

                var ctx = scheme.Encrypt(bitToEncrypt, secret);
                var decrypted = scheme.Decrypt(ctx, secret);

                Console.WriteLine($"Bit\t{bitToEncrypt}");
                Console.WriteLine($"Cipher\t{string.Join(',', ctx)}");
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
        }

        private static void HomomorphicEncryptionTest()
        {
            var client = new ImageProcessorClient();

            client.ConvertImageToGrayScale(
                @"C:\Users\sahalim\Downloads\adventure-time.png",
                @"C:\Users\sahalim\Downloads\adventure-time-homomorphic-grayscale.png");
        }
    }
}

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
            Console.WriteLine("Hello World!");

            EncryptionScheme2Test.CustomEncryptionTest();
            // HomomorphicEncryptionTest();
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

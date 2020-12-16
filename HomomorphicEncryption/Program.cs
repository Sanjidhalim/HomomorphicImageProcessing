using System;
using HomomorphicEncryption.Client;
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

            var client = new ImageProcessorClient();

            client.ConvertImageToGrayScale(
                @"C:\Users\sahalim\Downloads\adventure-time.png",
                @"C:\Users\sahalim\Downloads\adventure-time-homomorphic-grayscale.png");

            // ImageTest();

            // new CKKS().Encrypt();

            //int x = 10;
            //int c = -2;
            //new BFVBasics2().ExampleBFVBasics(x,c);
            //var val = x * x + c;
            //Console.WriteLine("Expected: "+ val.ToString("X"));
            //new BFVBasics().Run(259, 28);
        }

        private static void ImageTest()
        {
            var image = ImageIo.ReadImage(@"C:\Users\sahalim\Downloads\adventure-time.png");
            var grayscale = PlaintextImageProcessor.DetectEdge(image);
            ImageIo.SaveImage(@"C:\Users\sahalim\Downloads\adventure-time-x-y.png", grayscale);
        }
    }
}

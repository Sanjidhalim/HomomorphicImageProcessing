using System;
using System.Linq;
using HomomorphicEncryption.Common;
using HomomorphicEncryption.Server;
using ImageProcessor;
using Microsoft.Research.SEAL;

namespace HomomorphicEncryption.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageProcessorClient
    {
        private readonly int PolyModulusDegree = 8192;

        private EncryptionParameters parms;

        private KeyGenerator keygen;
        private SecretKey secretKey;
        private SEALContext context;
        private Encryptor encryptor;
        private Decryptor decryptor;
        private Evaluator evaluator;
        private CKKSEncoder encoder;

        private ImageProcessorServer _imageProcessorServer;

        public ImageProcessorClient()
        {
            parms = new EncryptionParameters(SchemeType.CKKS);
            parms.PolyModulusDegree = (ulong)PolyModulusDegree;
            parms.CoeffModulus = CoeffModulus.Create((ulong)PolyModulusDegree, new int[] { 60, 40, 40, 60 });

            double scale = Math.Pow(2.0, 40);

            context = new SEALContext(parms);

            keygen = new KeyGenerator(context);
            secretKey = keygen.SecretKey;
            keygen.CreatePublicKey(out PublicKey publicKey);
            
            // keygen.CreateRelinKeys(out RelinKeys relinKeys);

            encryptor = new Encryptor(context, publicKey);
            evaluator = new Evaluator(context);
            decryptor = new Decryptor(context, secretKey);

            encoder = new CKKSEncoder(context);

            var v1 = Enumerable.Range(0, 2000).Select(e => (double)200);
            var v2 = Enumerable.Range(0, 4000).Select(e => (double)200);
            var v3 = Enumerable.Range(0, 4096).Select(e => (double)200);

            var pd1 = new Plaintext();
            var pd2 = new Plaintext();
            var pd3 = new Plaintext();

            //encoder.Encode(v1, scale, pd1);
            //encoder.Encode(v1, scale, pd2);
            //encoder.Encode(v1, scale, pd3);

            // Pass in encode and evaluator to server.
            _imageProcessorServer = new ImageProcessorServer(evaluator, encoder, scale);

        }

        public void ConvertImageToGrayScale(string src, string dst)
        {
            double scale = Math.Pow(2.0, 40);

            var img = ImageIo.ReadImage(src);
            var encryptedImage = EncryptedImage.FromBmp(img, encryptor, encoder, scale, PolyModulusDegree / 2);

            // Call server. Server just has the encoder and evaluator.
            var encryptedGrayScaleImage = _imageProcessorServer.ConvertToGrayScale(encryptedImage);

            var decryptedGrayScaleImage = encryptedGrayScaleImage.DecryptFromVector(decryptor, encoder);

            var dataP = PlaintextImageProcessor.ConvertToGrayScale(img);

            ImageIo.SaveImage(dst, decryptedGrayScaleImage);
        }
    }
}

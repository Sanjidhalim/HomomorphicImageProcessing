using System;
using System.Collections.Generic;
using HomomorphicEncryption.Common;
using Microsoft.Research.SEAL;

namespace HomomorphicEncryption.Server
{
    /// <summary>
    /// Image processor.
    /// </summary>
    public class ImageProcessorServer
    {
        private readonly Evaluator _evaluator;

        private readonly Plaintext RMultiplePtx = new Plaintext();
        private readonly Plaintext GMultiplePtx = new Plaintext();
        private readonly Plaintext BMultiplePtx = new Plaintext();

        public ImageProcessorServer(
            Evaluator evaluator,
            CKKSEncoder encoder,
            double scale)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));

            encoder.Encode(0.3, scale, RMultiplePtx);
            encoder.Encode(0.59, scale, GMultiplePtx);
            encoder.Encode(0.11, scale, BMultiplePtx);
        }

        /// <summary>
        /// Converts an image to grayscale.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <returns></returns>
        public EncryptedImage ConvertToGrayScale(
            EncryptedImage image)
        {
            var rCtx = MultiplyPlain(image._rValueVector, RMultiplePtx);
            var gCtx = MultiplyPlain(image._gValueVector, GMultiplePtx);
            var bCtx = MultiplyPlain(image._bValueVector, BMultiplePtx);

            var grayCtx = AddMany(rCtx, gCtx, bCtx);

            // WARNING: NULL values can cause error on decryption.
            return new EncryptedImage(
                image._width,
                image._height,
                image._aValues,
                null,
                null,
                null,
                grayCtx,
                grayCtx,
                grayCtx);

            // int grayScale = (int)(((actualColor.R * 0.3) + (actualColor.G * 0.59) + (actualColor.B * 0.11)));
        }

        private List<Ciphertext> AddMany(List<Ciphertext> ct1, List<Ciphertext> ct2, List<Ciphertext> ct3)
        {
            var ret = new List<Ciphertext>();

            for (var i = 0; i < ct1.Count; i++)
            {
                var output = new Ciphertext();
                _evaluator.AddMany(new List<Ciphertext> { ct1[i], ct2[i], ct3[i] }, output);

                ret.Add(output);
            }

            return ret;
        }

        private List<Ciphertext> MultiplyPlain(List<Ciphertext> ctxs, Plaintext ptx)
        {
            var ret = new List<Ciphertext>();

            foreach (var ctxt in ctxs)
            {
                var output = new Ciphertext();
                _evaluator.MultiplyPlain(ctxt, ptx, output);
                ret.Add(output);
            }

            return ret;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Research.SEAL;

namespace HomomorphicEncryption.Common
{
    /// <summary>
    /// Encrypted image.
    /// </summary>
    public class EncryptedImage : IDisposable
    {
        public readonly int _width;
        public readonly int _height;

        /// <summary>
        /// Keep in clear. Needed to recreate image but
        /// not an RGB val and no computation is done over this.
        /// </summary>
        public readonly int[] _aValues;

        public readonly Ciphertext[] _rValues;
        public readonly Ciphertext[] _gValues;
        public readonly Ciphertext[] _bValues;

        public readonly List<Ciphertext> _rValueVector;
        public readonly List<Ciphertext> _gValueVector;
        public readonly List<Ciphertext> _bValueVector;

        public EncryptedImage(
            int width,
            int height,
            int[] aValues,
            Ciphertext[] rValues,
            Ciphertext[] gValues,
            Ciphertext[] bValues,
            List<Ciphertext> rValueVector,
            List<Ciphertext> gValueVector,
            List<Ciphertext> bValueVector)
        {
            _width = width;
            _height = height;
            _aValues = aValues;
            _rValues = rValues;
            _gValues = gValues;
            _bValues = bValues;
            _rValueVector = rValueVector;
            _gValueVector = gValueVector;
            _bValueVector = bValueVector;
        }

        public Bitmap DecryptFromVector(
            Decryptor decryptor,
            CKKSEncoder encoder)
        {
            var bmp = new Bitmap(_width, _height);

            var rVals = _rValueVector.DecryptData(decryptor, encoder);
            var gVals = _gValueVector.DecryptData(decryptor, encoder);
            var bVals = _bValueVector.DecryptData(decryptor, encoder);

            var data = new List<int>();

            var ind = 0;
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = 0; j < bmp.Height; j++)
                {
                    var color = Color.FromArgb(
                        _aValues[ind],
                        rVals[ind],
                        gVals[ind],
                        bVals[ind]);

                    data.Add(rVals[ind]);

                    bmp.SetPixel(i, j, color);

                    ind++;
                }
            }

            return bmp;
        }

        public void Dispose()
        {
            Dispose(_rValues);
            Dispose(_gValues);
            Dispose(_bValues);
        }

        private void Dispose(Ciphertext[] array)
        {
            foreach (var arr in array)
            {
                arr.Dispose();
            }
        }

        public static EncryptedImage FromBmp(
            Bitmap bmp,
            Encryptor encryptor,
            CKKSEncoder encoder,
            double scale,
            int slots)
        {
            var count = bmp.Height * bmp.Width;

            var aVals = new int[count];
            var rVals = new Ciphertext[count];
            var gVals = new Ciphertext[count];
            var bVals = new Ciphertext[count];

            var rValPlain = new byte[count];
            var gValPlain = new byte[count];
            var bValPlain = new byte[count];

            var ind = 0;
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = 0; j < bmp.Height; j++)
                {
                    var pixel = bmp.GetPixel(i, j);

                    aVals[ind] = pixel.A;

                    rValPlain[ind] = pixel.R;
                    gValPlain[ind] = pixel.G;
                    bValPlain[ind] = pixel.B;

                    // Add when implementing edge detector.
                    // rVals[ind] = pixel.R.Encrypt(encoder, encryptor, scale);
                    // gVals[ind] = pixel.G.Encrypt(encoder, encryptor, scale);
                    // bVals[ind] = pixel.B.Encrypt(encoder, encryptor, scale);

                    ind++;
                }
            }

            return new EncryptedImage(
                bmp.Width,
                bmp.Height,
                aVals,
                rVals,
                gVals,
                bVals,
                rValPlain.Encrypt(encoder, encryptor, scale, slots),
                gValPlain.Encrypt(encoder, encryptor, scale, slots),
                bValPlain.Encrypt(encoder, encryptor, scale, slots));
        }
    }
}

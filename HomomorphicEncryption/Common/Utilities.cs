using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Research.SEAL;

namespace HomomorphicEncryption.Common
{
    /// <summary>
    /// Conversion utilities.
    /// </summary>
    public static class Utilities
    {
        public static List<Ciphertext> Encrypt(
            this byte[] data,
            CKKSEncoder encoder,
            Encryptor encryptor,
            double scale,
            int slots)
        {
            return ComputeInBatches(
                data,
                slots,
                bytes =>
                {
                    using var pt = new Plaintext();
                    encoder.Encode(
                        bytes.Select(d => Convert.ToDouble(d)),
                        scale,
                        pt);

                    var ct = new Ciphertext();

                    encryptor.Encrypt(pt, ct);

                    return ct;
                });
        }

        public static Ciphertext Encrypt(
            this byte data,
            CKKSEncoder encoder,
            Encryptor encryptor,
            double scale)
        {
            using var pt = new Plaintext();
            encoder.Encode(Convert.ToDouble(data), scale, pt);

            var ct = new Ciphertext();
            encryptor.Encrypt(pt, ct);

            return ct;
        }

        public static List<int> DecryptData(
            this List<Ciphertext> ctxs,
            Decryptor decryptor,
            CKKSEncoder encoder)
        {
            var result = new List<int>();

            foreach (var ctx in ctxs)
            {
                using var ptx = new Plaintext();
                decryptor.Decrypt(ctx, ptx);

                var data = new List<double>();

                encoder.Decode(ptx, data);

                var integers = data.Select(d => (int)d);

                result.AddRange(integers);
            }

            return result;
        }

        public static List<Ciphertext> ComputeInBatches(
            byte[] input,
            int batchSize,
            Func<IEnumerable<byte>, Ciphertext> computation)
        {
            var result = new List<Ciphertext>();

            int processed = 0;

            while (processed < input.Length)
            {
                var left = input.Length - processed;
                var take = Math.Min(left, batchSize);

                var next = input.Skip(processed).Take(take);

                result.Add(computation(next));

                processed += take;
            }

            return result;
        }
    }
}

// -----------------------------------------------------------------------
//  <copyright company="Microsoft Corporation">
//       Copyright (C) Microsoft Corporation. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security.Cryptography;

namespace HomomorphicEncryption.CustomScheme
{
    /// <summary>
    /// 
    /// </summary>
    public class EncryptionScheme2
    {
        // NOT CRYPTOGRAPHICALLY SECURE.
        private static Random _random = new Random();

        public long Encrypt(int bit, int key)
        {
            var R = _random.Next(key, key * 2);
            var r = _random.Next(0, 10);

            return (R * key) + (r * 2) + bit;
        }

        public long Decrypt(long cipher, int key) => (cipher % key) % 2;

        public long AddCiphertext(long cipher1, long cipher2) => cipher1 + cipher2;

        public long MultiplyCiphertext(long cipher1, long cipher2) => cipher1 * cipher2;
    }
}

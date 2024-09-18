using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Gym_Reception_Management_System.Utils.Password
{
    public static class SymmetricEncryption
    {
        private static readonly string EncryptionKey = "7b417f03ec499a0cf803586aa2e1489b";

        public static string Encrypt(string plainText)
        {
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        private static string GenerateRandomKey(int keyLength)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[keyLength / 8];
                randomNumberGenerator.GetBytes(randomBytes);

                return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            }
        }
    }
}
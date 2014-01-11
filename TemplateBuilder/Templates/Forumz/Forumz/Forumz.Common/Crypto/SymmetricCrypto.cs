using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Forumz.Common.Crypto
{
    public class SymmetricCrypto : ISymmetricCrypto
    {
        private const int BLOCK_BIT_SIZE = 128;
        private const int KEY_BIT_SIZE = 256;
        private const int SALT_BIT_SIZE = 64;
        private const int ITERATIONS = 10000;

        private static byte[] m_CryptoKey;
        private static byte[] m_AuthKey;

        private static byte[] CryptKey
        {
            get
            {
                if (m_CryptoKey != null)
                    return m_CryptoKey;

                m_CryptoKey = Encoding.UTF8.GetBytes(@"*JS&$UQPL%J74UA&");
                return m_CryptoKey;
            }
        }

        private static byte[] AuthKey
        {
            get
            {
                if (m_AuthKey != null)
                    return m_AuthKey;

                m_AuthKey = Encoding.UTF8.GetBytes(@"");
                return m_AuthKey;
            }
        }

        public string Encrypt(string value, string seed)
        {
            return Encrypt(value, seed, null);
        }

        public string Decrypt(string value, string seed)
        {
            return Decrypt(value, seed, 0);
        }

        public string Encrypt(string value)
        {
            return Encrypt(value, CryptKey, AuthKey);
        }

        public string Decrypt(string value)
        {
            return Decrypt(value, CryptKey, AuthKey);
        }

        public string Encrypt(string value, byte[] cryptKey, byte[] authKey)
        {
            return Encrypt(value, cryptKey, authKey, null);
        }

        public string Decrypt(string value, byte[] cryptKey, byte[] authKey)
        {
            return Decrypt(value, cryptKey, authKey, 0);
        }

        private string Decrypt(string encryptedMessage, string password, int nonSecretPayloadLength)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                return null;

            var cryptSalt = new byte[SALT_BIT_SIZE / 8];
            var authSalt = new byte[SALT_BIT_SIZE / 8];
            var message = Convert.FromBase64String(encryptedMessage);

            Array.Copy(message, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
            Array.Copy(message, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

            byte[] cryptKey;
            byte[] authKey;

            using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, ITERATIONS))
            {
                cryptKey = generator.GetBytes(KEY_BIT_SIZE / 8);
            }

            using (var generator = new Rfc2898DeriveBytes(password, authSalt, ITERATIONS))
            {
                authKey = generator.GetBytes(KEY_BIT_SIZE / 8);
            }

            return Decrypt(encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
        }

        private string Encrypt(string secretMessage, string password, byte[] nonSecretPayload)
        {
            nonSecretPayload = nonSecretPayload ?? new byte[] { };
            var payload = new byte[((SALT_BIT_SIZE / 8) * 2) + nonSecretPayload.Length];

            Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);

            var payloadIndex = nonSecretPayload.Length;

            byte[] cryptKey;
            byte[] authKey;

            using (var generator = new Rfc2898DeriveBytes(password, SALT_BIT_SIZE / 8, ITERATIONS))
            {
                var salt = generator.Salt;

                cryptKey = generator.GetBytes(KEY_BIT_SIZE / 8);

                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
                payloadIndex += salt.Length;
            }

            using (var generator = new Rfc2898DeriveBytes(password, SALT_BIT_SIZE / 8, ITERATIONS))
            {
                var salt = generator.Salt;

                authKey = generator.GetBytes(KEY_BIT_SIZE / 8);

                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
            }

            return Encrypt(secretMessage, cryptKey, authKey, payload);
        }

        private string Decrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayLoadLength)
        {
            var message = Convert.FromBase64String(encryptedMessage);

            using (var hmac = new HMACSHA256(authKey))
            {
                var sentTag = new byte[hmac.HashSize / 8];

                var calcTag = hmac.ComputeHash(message, 0, message.Length - sentTag.Length);
                const int ivLength = (BLOCK_BIT_SIZE / 8);

                if (encryptedMessage.Length < sentTag.Length + nonSecretPayLoadLength + ivLength)
                    return null;

                Array.Copy(message, message.Length - sentTag.Length, sentTag, 0, sentTag.Length);

                var compare = 0;
                for (var i = 0; i < sentTag.Length; i++)
                    compare |= sentTag[i] ^ calcTag[i];

                if (compare != 0)
                    return null;

                using (var aes = new AesManaged
                                 {
                                     KeySize = KEY_BIT_SIZE,
                                     BlockSize = BLOCK_BIT_SIZE,
                                     Mode = CipherMode.CBC,
                                     Padding = PaddingMode.PKCS7
                                 })
                {
                    var iv = new byte[ivLength];
                    Array.Copy(message, nonSecretPayLoadLength, iv, 0, iv.Length);

                    try
                    {
                        using (var decryptor = aes.CreateDecryptor(cryptKey, iv))
                        {
                            using (var plainTextStream = new MemoryStream())
                            {
                                using (var decryptorStream = new CryptoStream(plainTextStream, decryptor, CryptoStreamMode.Write))
                                {
                                    using (var binaryWriter = new BinaryWriter(decryptorStream))
                                    {
                                        binaryWriter.Write(message, nonSecretPayLoadLength + iv.Length, message.Length - nonSecretPayLoadLength - iv.Length - sentTag.Length);
                                    }

                                    return Encoding.UTF8.GetString(plainTextStream.ToArray());
                                }
                            }
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                    catch (CryptographicException)
                    {
                        return null;
                    }
                }
            }
        }

        private string Encrypt(string secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload)
        {
            nonSecretPayload = nonSecretPayload ?? new byte[] { };
            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
                             {
                                 KeySize = KEY_BIT_SIZE,
                                 BlockSize = BLOCK_BIT_SIZE,
                                 Mode = CipherMode.CBC,
                                 Padding = PaddingMode.PKCS7
                             })
            {
                aes.GenerateIV();
                iv = aes.IV;
                using (var encryptor = aes.CreateEncryptor(cryptKey, iv))
                {
                    using (var cipherStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(cipherStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var binaryWriter = new BinaryWriter(cryptoStream))
                            {
                                binaryWriter.Write(Encoding.UTF8.GetBytes(secretMessage));
                            }
                        }
                        cipherText = cipherStream.ToArray();
                    }
                }
            }

            using (var hmac = new HMACSHA256(authKey))
            {
                using (var encryptedStream = new MemoryStream())
                {
                    using (var binaryWriter = new BinaryWriter(encryptedStream))
                    {
                        binaryWriter.Write(nonSecretPayload);
                        binaryWriter.Write(iv);
                        binaryWriter.Write(cipherText);
                        binaryWriter.Flush();

                        var tag = hmac.ComputeHash(encryptedStream.ToArray());
                        binaryWriter.Write(tag);
                    }
                    return Convert.ToBase64String(encryptedStream.ToArray());
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Crypto
{
    public class AESCrypto
    {
        public string CreateKey(string plainKey)
        {
            if (string.IsNullOrEmpty(plainKey) ||
                plainKey.Length != 32)
                throw new ArgumentException("키가 비어있거나 32자가 아닙니다.");

            var bytes = Encoding.UTF8.GetBytes(plainKey);

            for(int head = 0, tail = plainKey.Length-1; head < plainKey.Length / 2; head++, tail--)
            { 
                byte tmp = bytes[head];
                bytes[head] = bytes[tail];
                bytes[tail] = tmp;
            }

            return Convert.ToBase64String(bytes);
        } 

        private  string CreateIV(string key)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Convert.FromBase64String(key));

                return Convert.ToBase64String(hash);
            }
        }

        public string Encrypt(string key, string data)
        {
            var iv = CreateIV(key);

            using (var aes = CreateAES(key, iv))
            using (var enc = aes.CreateEncryptor(aes.Key, aes.IV))
            { 
                var plainBytes = Encoding.UTF8.GetBytes(data);

                var encBytes = enc.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encBytes);
            } 
        }

        public string Decrypt(string key, string data)
        {
            var iv = CreateIV(key);

            using (var aes = CreateAES(key, iv))
            using (var enc = aes.CreateDecryptor(aes.Key, aes.IV))
            { 
                var plainBytes = Convert.FromBase64String(data);

                var decBytes = enc.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Encoding.UTF8.GetString(decBytes);
            }
        }

        private RijndaelManaged CreateAES(string key, string iv)
        {
            return new RijndaelManaged()
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = Convert.FromBase64String(key),
                IV = Convert.FromBase64String(iv),
            };
        }
    }
}

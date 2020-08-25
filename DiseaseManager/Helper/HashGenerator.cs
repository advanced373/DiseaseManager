using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace DiseaseManager.Helper
{
    class HashGenerator
    {
        #region Members
        private static int iterations=10000;
        private static int hashSize = 24;
        private static int saltSize = 24;

        private static HashGenerator instance = null;
        #endregion
        #region Methods
        public static byte[] generateHash(string inPassword)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltSize];
            provider.GetBytes(salt);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(inPassword, salt);
            int newSize = saltSize + hashSize;
            byte[] saltPlusHash = new byte[newSize];
            System.Buffer.BlockCopy(salt, 0, saltPlusHash,0,saltSize);
            System.Buffer.BlockCopy(pbkdf2.GetBytes(hashSize), 0, saltPlusHash, saltSize, hashSize);
            return saltPlusHash;
        }
        public static bool VerifyPasswordHash(string password1,byte[] hash)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltSize];
            System.Buffer.BlockCopy(hash, 0, salt, 0, saltSize);
            byte[] password2 = new byte[hashSize];
            System.Buffer.BlockCopy(hash, saltSize, password2, 0, hashSize);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password1, salt);
            return pbkdf2.GetBytes(hashSize).SequenceEqual(password2);
        }
        #endregion
        #region Properties
        public static HashGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HashGenerator();
                }
                return instance;
            }
        }
        #endregion
    }
}

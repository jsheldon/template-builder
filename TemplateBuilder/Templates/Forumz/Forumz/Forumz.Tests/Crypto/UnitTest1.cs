using Forumz.Common.Crypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forumz.Tests.Crypto
{
    [TestClass]
    public class When_Encrypting_And_Decrypting_Data
    {
        [TestMethod]
        public void Given_Password_Symmetric_Encryptor_Should_Decrypt()
        {
            const string plainText = "Hello World";
            const string encryptPassword = "Random Password String For Encryption";

            var symmetricCrypto = new SymmetricCrypto();
            var cipherText = symmetricCrypto.Encrypt(plainText, encryptPassword);
            var returnValue = symmetricCrypto.Decrypt(cipherText, encryptPassword);

            Assert.AreEqual(plainText, returnValue);
        }

        [TestMethod]
        public void Given_The_Wrong_Password_Symmetric_Encryptor_Should_Fail_To_Decrypt()
        {
            const string plainText = "Hello World";
            const string encryptPassword = "Random Password String For Encryption";

            var symmetricCrypto = new SymmetricCrypto();
            var cipherText = symmetricCrypto.Encrypt(plainText, encryptPassword);
            var returnValue = symmetricCrypto.Decrypt(cipherText, "J!@(DJLASJDKJD@*JUD(*@D@D@DOI@JDOIJ");

            Assert.IsNull(returnValue);
        }
    }
}
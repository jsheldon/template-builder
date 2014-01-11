namespace Forumz.Common.Crypto
{
    public interface ISymmetricCrypto
    {
        string Encrypt(string value, string seed);
        string Decrypt(string value, string seed);
    }
}
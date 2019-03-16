using Sodium;

namespace jaytwo.ejson.Internal
{
    internal interface IBoxedMessageCrypto
    {
        string Decrypt(BoxedMessage boxedMessage, byte[] privateKey);
        BoxedMessage Encrypt(string nessage, byte[] privateKey);
    }
}

using System;

namespace jaytwo.ejson.Internal
{
    internal static class IBoxedMessageCryptoExtensions
    {
        public static string Decrypt(this IBoxedMessageCrypto boxedMessageCrypto, string boxedMessageAsString, string privateKey)
        {
            var privateKeyBytes = HexConverter.HexToBinary(privateKey);
            return boxedMessageCrypto.Decrypt(boxedMessageAsString, privateKeyBytes);
        }

        public static string Decrypt(this IBoxedMessageCrypto boxedMessageCrypto, string boxedMessageAsString, byte[] privateKey)
        {
            var boxedMessage = BoxedMessage.Create(boxedMessageAsString);
            return boxedMessageCrypto.Decrypt(boxedMessage, privateKey);
        }

        public static string Decrypt(this IBoxedMessageCrypto boxedMessageCrypto, BoxedMessage boxedMessage, string privateKey)
        {
            var privateKeyBytes = HexConverter.HexToBinary(privateKey);
            return boxedMessageCrypto.Decrypt(boxedMessage, privateKeyBytes);
        }

        public static BoxedMessage Encrypt(this IBoxedMessageCrypto boxedMessageCrypto, string nessage, string publicKey)
        {
            var publicKeyBytes = HexConverter.HexToBinary(publicKey);
            return boxedMessageCrypto.Encrypt(nessage, publicKeyBytes);
        }
    }
}

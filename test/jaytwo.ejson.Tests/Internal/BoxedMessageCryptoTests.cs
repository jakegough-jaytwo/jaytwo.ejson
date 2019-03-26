using System;
using System.Text;
using jaytwo.ejson.Internal;
using jaytwo.ejson.Internal.Sodium;
using Moq;
using Xunit;

namespace jaytwo.ejson.Tests.Internal
{
    public class BoxedMessageCryptoTests
    {
        [Fact]
        public void Decrypt_works_with_values_from_original_shopify_ejson()
        {
            // arrange
            var privateKey = "a749f5b3003787d633ee61dda991f973a34528496babdf6c22fddd50a8959067";
            var secretText = "EJ[1:KNN7o/ZQEVNz+u4Fi6xlp4c9oqll840ney9iM1Wo3Xw=:PwqUnJ+zoUDqjh60UQpar5t0Z1qMizCu:PSDeMTcyetIp0ue9gRR4Nk/Rg4FS]";
            var expectedDecrypted = "hello";

            var crypto = new BoxedMessageCrypto();

            // act
            var actualDecrypted = crypto.Decrypt(secretText, privateKey);

            // assert
            Assert.Equal(expectedDecrypted, actualDecrypted);
        }

        [Fact]
        public void Encrypt_and_Decrypt_works_as_expected()
        {
            // arrange
            var plain = "hello";
            var crypto = new BoxedMessageCrypto();
            var keyPair = new PublicKeyBoxWrapper().GenerateKeyPair();

            // act
            var encrypted = crypto.Encrypt(plain, keyPair.PublicKey);
            var decrypted = crypto.Decrypt(encrypted, keyPair.SecretKey);

            // assert
            Assert.Equal(decrypted, plain);
        }
    }
}

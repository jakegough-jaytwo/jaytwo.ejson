using System;
using System.Text;
using jaytwo.ejson.Internal;
using jaytwo.ejson.Internal.Sodium;
using Moq;
using Sodium;
using Xunit;

namespace jaytwo.ejson.Tests.Internal
{
    public class BoxedMessageCryptoTests
    {
        [Fact]
        public void Decrypt_works_as_expected()
        {
            // arrange
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";
            var secretText = "EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]";
            var expectedDecrypted = "hello";

            var crypto = new BoxedMessageCrypto();

            // act
            var actualDecrypted = crypto.Decrypt(secretText, privateKey);

            // assert
            Assert.Equal(expectedDecrypted, actualDecrypted);
        }

        [Fact]
        public void Encrypt_works_as_expected()
        {
            // arrange
            var privateKey = Utilities.HexToBinary("5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6");

            var mockPublicKeyBox = new Mock<PublicKeyBoxWrapper> { CallBase = true };
            mockPublicKeyBox
                .Setup(x => x.GenerateNonce())
                .Returns(Convert.FromBase64String("ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL"));

            mockPublicKeyBox
                .Setup(x => x.GenerateRandomPublicKey())
                .Returns(Convert.FromBase64String("EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo="));

            var boxedMessageCrypto = new BoxedMessageCrypto(Encoding.UTF8, mockPublicKeyBox.Object);


            // act
            var encryptedBoxMessage = boxedMessageCrypto.Encrypt("hello", privateKey);

            // assert
            Assert.Equal("EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]", encryptedBoxMessage.ToString());
        }

        [Fact]
        public void Encrypt_and_Decrypt_works_as_expected()
        {
            // arrange
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";
            var plain = "hello";

            var crypto = new BoxedMessageCrypto();

            // act
            var encrypted = crypto.Encrypt(plain, privateKey);
            var decrypted = crypto.Decrypt(encrypted, privateKey);

            // assert
            Assert.Equal(decrypted, plain);
        }
    }
}

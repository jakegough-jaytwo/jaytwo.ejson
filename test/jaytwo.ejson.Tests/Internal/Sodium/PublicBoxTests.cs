using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using jaytwo.ejson.Internal.Sodium;
using Xunit;

namespace jaytwo.ejson.Tests.Internal.Sodium
{
    public class PublicBoxTests
    {
        public static byte[] GetRandomBytes(int length)
        {
            var result = new byte[length];
            var random = RandomNumberGenerator.Create();
            random.GetBytes(result);
            return result;
        }

        public static byte[] HexToBinary(string hex) => Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();

        public static string BinaryToHex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();

        [Fact]
        public void Decrypt()
        {
            // arrange
            var publicBox = new PublicKeyBoxWrapper();
            var privateKey = HexToBinary("5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6");
            var publicKey = HexToBinary("749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117");

            var ephemeralPublicKey = Convert.FromBase64String("EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=");
            var nonce = Convert.FromBase64String("ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL");
            var encryptedMmessage = Convert.FromBase64String("OR5Owty2fMOPTownI2/xngsWISvD");
            var expectedDecryptedMessage = Encoding.UTF8.GetBytes("hello");

            // act
            var decryptedMessage = publicBox.Open(encryptedMmessage, nonce, privateKey, ephemeralPublicKey);

            // assert
            Assert.Equal(expectedDecryptedMessage, decryptedMessage);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(168)]
        [InlineData(542)]
        [InlineData(15573)]
        [InlineData(1001024)]
        public void FullCircle(int dataSize)
        {
            // arrange
            var publicBox = new PublicKeyBoxWrapper();
            var keyPair = publicBox.GenerateKeyPair();
            var ephemeralKeypair = publicBox.GenerateKeyPair();
            var nonce = publicBox.GenerateNonce();

            var expectedDecryptedMessage = GetRandomBytes(dataSize);

            // act
            var encryptedMmessage = publicBox.Create(expectedDecryptedMessage, nonce, ephemeralKeypair.SecretKey, keyPair.PublicKey);
            var decryptedMessage = publicBox.Open(encryptedMmessage, nonce, keyPair.SecretKey, ephemeralKeypair.PublicKey);

            // assert
            Assert.Equal(expectedDecryptedMessage, decryptedMessage);
        }
    }
}

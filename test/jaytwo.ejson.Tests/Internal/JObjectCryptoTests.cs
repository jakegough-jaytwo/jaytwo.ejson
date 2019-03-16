using System;
using System.Collections.Generic;
using jaytwo.ejson.Internal;
using Xunit;

namespace jaytwo.ejson.Tests.Internal
{
    public class JObjectCryptoTests
    {
        [Fact]
        public void Decrypt_works_as_expected()
        {
            // arrange
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";

            var ejson = @"{
  ""_public_key"": ""749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117"",
  ""passwords"": {
    ""database_password"": ""EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]""
  }
}";

            var expectedDecrypted = @"{
  ""_public_key"": ""749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117"",
  ""passwords"": {
    ""database_password"": ""hello""
  }
}";

            var crypto = new JObjectCrypto();

            // act
            var actualDecrypted = crypto.DecryptJson(ejson, privateKey);

            // assert
            Assert.Equal(JObjectTools.NormalizeJson(expectedDecrypted), JObjectTools.NormalizeJson(actualDecrypted));
        }


        [Fact]
        public void Encrypt_and_Decrypt_works_as_expected()
        {
            // arrange
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";

            var json = @"{
  ""passwords"": {
    ""database_password"": ""hello""
  }
}";

            var crypto = new JObjectCrypto();

            // act
            var encrypted = crypto.EncryptJson(json, privateKey);
            var decrypted = crypto.DecryptJson(encrypted, privateKey);

            // assert
            Assert.Equal(JObjectTools.NormalizeJson(decrypted), JObjectTools.NormalizeJson(json));
        }
    }
}

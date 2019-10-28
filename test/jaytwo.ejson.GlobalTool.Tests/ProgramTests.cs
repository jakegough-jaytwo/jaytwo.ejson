using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Moq;
using Xunit;

namespace jaytwo.ejson.GlobalTool.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void keygen_to_stdout()
        {
            // arrange

            // act
            var stdOut = RunProgram("keygen");

            // assert
            Assert.StartsWith("Public Key:", stdOut);
            Assert.Contains("Private Key:", stdOut);
        }

        [Fact]
        public void keygen_save_to_keydir()
        {
            // arrange
            var tempFolder = GetTempDir();

            // act
            var publicKey = RunProgram($"--keydir={tempFolder}", "keygen", "-w").Trim();

            // assert
            var privateKeyPath = Path.Combine(tempFolder, publicKey);
            Assert.True(File.Exists(privateKeyPath));

            var contents = File.ReadAllText(privateKeyPath).Trim();
            Assert.NotEmpty(contents);

            // cleanup
            File.Delete(privateKeyPath);
        }

        [Fact]
        public void decrypt()
        {
            // arrange
            var tempFolder = GetTempDir();
            var publicKey = "749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117";
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";
            var privateKeyPath = Path.Combine(tempFolder, publicKey);
            var encryptedFilePath = Path.Combine(tempFolder, $"{nameof(decrypt)}.ejson");

            var encryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]""
  }}
}}";

            var expectedDecryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""hello""
  }}
}}";

            File.WriteAllText(privateKeyPath, privateKey);
            File.WriteAllText(encryptedFilePath, encryptedJson);

            // act
            var decryptedJson = RunProgram($"--keydir={tempFolder}", "decrypt", encryptedFilePath);

            // assert
            Assert.Equal(Regex.Replace(expectedDecryptedJson, "\\s", string.Empty), Regex.Replace(decryptedJson, "\\s", string.Empty));

            // cleanup
            File.Delete(encryptedFilePath);
            File.Delete(privateKeyPath);
        }

        [Fact]
        public void encrypt_without_private_key_can_be_decrypted()
        {
            // arrange
            var tempFolder = GetTempDir();
            var publicKey = "749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117";
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";
            var privateKeyPath = Path.Combine(tempFolder, publicKey);
            var encryptedFilePath = Path.Combine(tempFolder, $"{nameof(encrypt_without_private_key_can_be_decrypted)}.ejson");

            var partiallyEncryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]""
  }},
  ""secret_web_key"": ""world""
}}";

            var expectedDecryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""hello""
  }},
  ""secret_web_key"": ""world""
}}";

            File.WriteAllText(privateKeyPath, privateKey);
            File.WriteAllText(encryptedFilePath, partiallyEncryptedJson);

            // act
            var stdOut = RunProgram("encrypt", encryptedFilePath);

            // assert
            Assert.NotEmpty(stdOut.Trim());

            var encryptedJson = File.ReadAllText(encryptedFilePath);
            Assert.Contains("_public_key", encryptedJson);
            Assert.Contains("database_password", encryptedJson);
            Assert.Contains("secret_web_key", encryptedJson);
            Assert.DoesNotContain("hello", encryptedJson);
            Assert.DoesNotContain("world", encryptedJson);

            var decryptedJson = RunProgram($"--keydir={tempFolder}", "decrypt", encryptedFilePath);
            Assert.Contains("_public_key", decryptedJson);
            Assert.Contains("hello", decryptedJson);
            Assert.Contains("world", decryptedJson);
            Assert.Equal(Regex.Replace(expectedDecryptedJson, "\\s", string.Empty), Regex.Replace(decryptedJson, "\\s", string.Empty));

            // cleanup
            File.Delete(encryptedFilePath);
            File.Delete(privateKeyPath);
        }

        [Fact]
        public void encrypt_multiple_files_with_glob()
        {
            // arrange
            var tempFolder = GetTempDir();
            var publicKey = "749d901c694890ee91b7d2c366c2d59dc7b6b8a386d0a5be73431e622b91d117";
            var privateKey = "5f349a6bf95d692830a8930aa657b5d553073e589564925f153c018b5c27c8b6";
            var privateKeyPath = Path.Combine(tempFolder, publicKey);
            var encryptedFilePath1 = Path.Combine(tempFolder, $"{nameof(encrypt_multiple_files_with_glob)}1.ejson");
            var encryptedFilePath2 = Path.Combine(tempFolder, $"{nameof(encrypt_multiple_files_with_glob)}2.ejson");
            var glob = Path.Combine(tempFolder, $"{nameof(encrypt_multiple_files_with_glob)}*.ejson");

            var partiallyEncryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""EJ[1:EtX9E9y07M9ppTIeiLgdMysdWgmWNPvdNRBMtHKuDQo=:ZQqM6wGxUUiRy2kXYVinPKEFIyMRubvL:OR5Owty2fMOPTownI2/xngsWISvD]""
  }},
  ""secret_web_key"": ""world""
}}";

            var expectedDecryptedJson =
$@"{{
  ""_public_key"": ""{publicKey}"",
  ""passwords"": {{
    ""database_password"": ""hello""
  }},
  ""secret_web_key"": ""world""
}}";

            File.WriteAllText(privateKeyPath, privateKey);
            File.WriteAllText(encryptedFilePath1, partiallyEncryptedJson);
            File.WriteAllText(encryptedFilePath2, partiallyEncryptedJson);

            // act
            var stdOut = RunProgram("encrypt", glob);

            // assert
            Assert.NotEmpty(stdOut.Trim());

            var encryptedJson1 = File.ReadAllText(encryptedFilePath1);
            Assert.Contains("_public_key", encryptedJson1);
            Assert.Contains("database_password", encryptedJson1);
            Assert.Contains("secret_web_key", encryptedJson1);
            Assert.DoesNotContain("hello", encryptedJson1);
            Assert.DoesNotContain("world", encryptedJson1);

            var encryptedJson2 = File.ReadAllText(encryptedFilePath2);
            Assert.Contains("_public_key", encryptedJson2);
            Assert.Contains("database_password", encryptedJson2);
            Assert.Contains("secret_web_key", encryptedJson2);
            Assert.DoesNotContain("hello", encryptedJson2);
            Assert.DoesNotContain("world", encryptedJson2);

            var decryptedJson1 = RunProgram($"--keydir={tempFolder}", "decrypt", encryptedFilePath1);
            Assert.Contains("_public_key", decryptedJson1);
            Assert.Contains("hello", decryptedJson1);
            Assert.Contains("world", decryptedJson1);
            Assert.Equal(Regex.Replace(expectedDecryptedJson, "\\s", string.Empty), Regex.Replace(decryptedJson1, "\\s", string.Empty));

            var decryptedJson2 = RunProgram($"--keydir={tempFolder}", "decrypt", encryptedFilePath2);
            Assert.Contains("_public_key", decryptedJson2);
            Assert.Contains("hello", decryptedJson2);
            Assert.Contains("world", decryptedJson2);
            Assert.Equal(Regex.Replace(expectedDecryptedJson, "\\s", string.Empty), Regex.Replace(decryptedJson2, "\\s", string.Empty));

            // cleanup
            File.Delete(encryptedFilePath1);
            File.Delete(encryptedFilePath2);
            File.Delete(privateKeyPath);
        }

        private static string GetTempDir()
        {
            var assemblyName = typeof(ProgramTests).GetType().Assembly.GetName().Name;
            var tempFolder = Path.Combine(Path.GetTempPath(), assemblyName);

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            return tempFolder;
        }

        private static string RunProgram(params string[] args)
        {
            var outputBuilder = new StringBuilder();
            using (var output = new StringWriter(outputBuilder))
            {
                var program = new Program(null, output, null);
                var exitCode = program.Run(args);

                Assert.Equal(0, exitCode);
            }

            return outputBuilder.ToString();
        }
    }
}

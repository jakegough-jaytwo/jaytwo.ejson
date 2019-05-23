namespace jaytwo.ejson.Crypto
{
    internal class KeyPair
    {
        public KeyPair()
        {
        }

        public KeyPair(byte[] publicKey, byte[] secretKey)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
        }

        public byte[] PublicKey { get; set; }

        public byte[] SecretKey { get; set; }
    }
}

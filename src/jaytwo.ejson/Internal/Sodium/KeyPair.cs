namespace jaytwo.ejson.Crypto
{
    internal class KeyPair
    {
        public byte[] PublicKey { get; set; }
        public byte[] SecretKey { get; set; }

        public KeyPair()
        {
        }

        public KeyPair(byte[] publicKey, byte[] secretKey)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
        }
    }
}

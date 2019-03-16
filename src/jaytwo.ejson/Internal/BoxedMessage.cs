using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace jaytwo.ejson.Internal
{
    internal class BoxedMessage
    {
        private static readonly Regex _regex = new Regex("EJ[[](?<V>[^:]+)[:](?<P>[^:]+)[:](?<N>[^:]+)[:](?<M>[^:]+)[]]", RegexOptions.Compiled);

        /*        
        from: https://shopify.github.io/ejson/ejson.5.html

        SECRET SCHEMA
        When a value is encrypted, it will be replaced by a relatively long string of the form "EJ[V:P:N:M]". The fields are:

        V (decimal-as-string int)
        Schema Version, hard-coded to "1" for now

        P (base64-encoded 32-byte array)
        Public key of an ephemeral keypair used to encrypt this key

        N (base64-encoded 24-byte array)
        Nonce used to encrypt this key

        M (base64-encoded variable-length array)
        Raw ciphertext
         */

        public static bool TryCreate(string secret, out BoxedMessage result)
        {
            try
            {
                result = Create(secret);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool IsBoxedMessage(string value)
        {
            return _regex.IsMatch(value);
        }

        public static BoxedMessage Create(string boxedMessageAsString)
        {

            var match = _regex.Match(boxedMessageAsString);

            if (!match.Success)
            {
                throw new ArgumentException("Cannot parse secret.  Must be inthe form 'EJ[V: P:N: M]'.", nameof(boxedMessageAsString));
            }

            var result = new BoxedMessage();
            result.SchemaVersion = match.Groups["V"].Value;
            result.PublicKeyBase64 = match.Groups["P"].Value;
            result.NonceBase64 = match.Groups["N"].Value;
            result.EncryptedMessageBase64 = match.Groups["M"].Value;

            return result;
        }

        /// <summary>
        /// Schema Version
        /// </summary>
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Public key of an ephemeral keypair used to encrypt this key
        /// </summary>
        public string PublicKeyBase64 { get; set; }

        /// <summary>
        /// Nonce used to encrypt this key
        /// </summary>
        public string NonceBase64 { get; set; }

        /// <summary>
        /// Raw ciphertext
        /// </summary>
        public string EncryptedMessageBase64 { get; set; }

        public override string ToString()
        {
            return $"EJ[{SchemaVersion}:{PublicKeyBase64}:{NonceBase64}:{EncryptedMessageBase64}]";
        }
    }
}

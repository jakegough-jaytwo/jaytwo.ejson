I don't know where it came from, but when I was looking for C# implementations of NaCl functions, pretty 
much everybody has these exact same `TweetNaCl` files copied and pasted into their repositories.  I wish 
I could give credit to the original source, but my cursory search didn't make that obvious and I didn't 
want to spend too much time on it.  Though the NaCl functions were everywhere, tests were a bit harder to
find, so test coverage is a little light on the _borrowed_ functions.

Since secuirty is hard and I wanted transparency, I didn't want to touch these files at all when I brought 
them into this library.  The only things I changed were the namespaces, perhaps some `using` declarations, 
and commmented out `curve25519xsalsa20poly1305.crypto_box_keypair` because it wasn't compatible with all 
the target frameworks I wanted to support.  No problem though, the it was just the `Random` object that 
wasn't available in earlier versions of .NET Core.  The functionaltiy is replicated in the 
`PublicKeyBoxWrapper` object.

From https://tweetnacl.cr.yp.to/:

> TweetNaCl is the world's first auditable high-security cryptographic library. TweetNaCl fits into just 100 
> tweets while supporting all 25 of the C NaCl functions used by applications. TweetNaCl is a self-contained 
> public-domain C library, so it can easily be integrated into applications.

See:
* https://github.com/search?q=tweetnacl+filename%3APublicBox.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Acurve25519.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Acurve25519xsalsa20poly1305.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Ahsalsa20.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Apoly1305.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Asalsa20.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Averify_16.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Axsalsa20.cs&type=Code
* https://github.com/search?q=tweetnacl+filename%3Axsalsa20poly1305.cs&type=Code

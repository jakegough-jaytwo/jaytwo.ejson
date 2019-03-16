﻿using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Sodium;

namespace jaytwo.ejson.Internal
{
    internal interface IJObjectCrypto
    {
        void Encrypt(JObject jObject, byte[] privateKey);
        void Decrypt(JObject jObject, byte[] privateKey);
    }
}

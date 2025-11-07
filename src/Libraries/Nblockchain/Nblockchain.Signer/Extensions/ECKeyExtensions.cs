using Nethereum.Signer.Crypto;
using System;

namespace Nblockchain.Signer
{
    public static class ECKeyExtensions
    {
        public static byte[] GetPubKeyNoPrefix(this ECKey eCKey, bool isCompressed)
        {
            var pubKey = eCKey.GetPubKey(isCompressed);
            var arr = new byte[pubKey.Length - 1];
            Array.Copy(pubKey, 1, arr, 0, arr.Length);
            return arr;
        }
    }
}

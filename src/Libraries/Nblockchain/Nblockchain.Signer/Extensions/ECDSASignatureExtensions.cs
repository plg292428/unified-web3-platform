using Nethereum.Signer.Crypto;
using Nethereum.Util;
using Org.BouncyCastle.Math;
using System;

namespace Nblockchain.Signer
{
    public static class ECDSASignatureExtensions
    {
        public static byte[] ToByteArray(this ECDSASignature eCDSASignature)
        {
            return ByteUtil.Merge(BigIntegerToBytes(eCDSASignature.R, 32), BigIntegerToBytes(eCDSASignature.S, 32), eCDSASignature.V);
        }

        private static byte[] BigIntegerToBytes(BigInteger b, int numBytes)
        {
            var bytes = new byte[numBytes];
            var biBytes = b.ToByteArray();
            var start = (biBytes.Length == numBytes + 1) ? 1 : 0;
            var length = Math.Min(biBytes.Length, numBytes);
            Array.Copy(biBytes, start, bytes, numBytes - length, length);
            return bytes;
        }
    }
}

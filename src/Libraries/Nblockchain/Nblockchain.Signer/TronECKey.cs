using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Linq;

namespace Nblockchain.Signer
{
    public class TronECKey
    {
        private static readonly SecureRandom _secureRandom = new SecureRandom();

        private readonly ECKey _ecKey;
        private readonly TronNetwork _network = TronNetwork.MainNet;

        private byte[]? _publicKey = null;
        private byte[]? _publicKeyCompressed = null;
        private byte[]? _publicKeyNoPrefix = null;
        private byte[]? _publicKeyNoPrefixCompressed = null;
        private byte[]? _privateKey = null;
        private string? _privateKeyHex = null;
        private string? _tronAddress = null;

        public TronECKey(string privateKey, TronNetwork network = TronNetwork.MainNet)
        {
            _ecKey = new ECKey(privateKey.HexToByteArray(), true);
            _network = network;
        }

        public TronECKey(byte[] vch, bool isPrivate, TronNetwork network = TronNetwork.MainNet)
        {
            _ecKey = new ECKey(vch, isPrivate);
            _network = network;
        }

        internal TronECKey(ECKey ecKey, TronNetwork network = TronNetwork.MainNet)
        {
            _ecKey = ecKey;
            _network = network;
        }

        public static TronECKey GenerateKey(byte[]? seed = null, TronNetwork network = TronNetwork.MainNet)
        {
            var secureRandom = _secureRandom;
            if (seed is not null)
            {
                secureRandom = new SecureRandom();
                secureRandom.SetSeed(seed);
            }

            var gen = new ECKeyPairGenerator("EC");
            var keyGenParam = new KeyGenerationParameters(secureRandom, 256);
            gen.Init(keyGenParam);
            var keyPair = gen.GenerateKeyPair();
            var privateBytes = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArray();
            if (privateBytes.Length != 32)
                return GenerateKey(network);
            return new TronECKey(privateBytes, true, network);
        }

        public static TronECKey GenerateKey(TronNetwork network = TronNetwork.MainNet)
        {
            var gen = new ECKeyPairGenerator("EC");
            var keyGenParam = new KeyGenerationParameters(_secureRandom, 256);
            gen.Init(keyGenParam);
            var keyPair = gen.GenerateKeyPair();
            var privateBytes = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArray();
            if (privateBytes.Length != 32)
                return GenerateKey(network);
            return new TronECKey(privateBytes, true, network);
        }

        public byte[] GetPrivateKeyAsBytes()
        {
            if (_privateKey is null)
            {
                _privateKey = _ecKey.PrivateKey.D.ToByteArrayUnsigned();
            }
            return _privateKey;
        }

        public string GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(_privateKeyHex))
            {
                _privateKeyHex = _ecKey.PrivateKey.D.ToByteArrayUnsigned().ToHex();
            }
            return _privateKeyHex;
        }

        public byte[] GetPubKey()
        {
            return _ecKey.GetPubKey(false);
        }

        public byte[] GetPubKey(bool compresseed = false)
        {
            if (!compresseed)
            {
                if (_publicKey is null)
                {
                    _publicKey = _ecKey.GetPubKey(false);
                }
                return _publicKey;
            }
            else
            {
                if (_publicKeyCompressed is null)
                {
                    _publicKeyCompressed = _ecKey.GetPubKey(true);
                }
                return _publicKeyCompressed;

            }
        }

        internal byte GetPublicAddressPrefix()
        {
            if (_network == TronNetwork.MainNet)
            {
                return 0x41;
            }
            else
            {
                return 0xa0;
            }

        }

        public byte[] GetPubKeyNoPrefix(bool compressed = false)
        {
            if (!compressed)
            {
                if (_publicKeyNoPrefix is null)
                {
                    var pubKey = _ecKey.GetPubKey(false);
                    var arr = new byte[pubKey.Length - 1];
                    //remove the prefix
                    Array.Copy(pubKey, 1, arr, 0, arr.Length);
                    _publicKeyNoPrefix = arr;
                }
                return _publicKeyNoPrefix;
            }
            else
            {
                if (_publicKeyNoPrefixCompressed is null)
                {
                    var pubKey = _ecKey.GetPubKey(true);
                    var arr = new byte[pubKey.Length - 1];
                    //remove the prefix
                    Array.Copy(pubKey, 1, arr, 0, arr.Length);
                    _publicKeyNoPrefixCompressed = arr;
                }
                return _publicKeyNoPrefixCompressed;

            }
        }

        public string GetPublicAddress()
        {
            if (!string.IsNullOrWhiteSpace(_tronAddress))
            {
                return _tronAddress;
            }
            var initaddr = GetPubKeyNoPrefix().ToKeccakHash();
            var address = new byte[initaddr.Length - 11];
            Array.Copy(initaddr, 12, address, 1, 20);
            address[0] = GetPublicAddressPrefix();
            var hash = Base58Encoder.TwiceHash(address);
            var bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            var addressChecksum = new byte[25];
            Array.Copy(address, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);
            if (_network == TronNetwork.MainNet)
            {
                _tronAddress = Base58Encoder.Encode(addressChecksum);
            }
            else
            {
                _tronAddress = addressChecksum.ToHex();
            }
            return _tronAddress;
        }

        public static string GetPublicAddress(string privateKey, TronNetwork network = TronNetwork.MainNet)
        {
            var key = new TronECKey(privateKey.HexToByteArray(), true, network);
            return key.GetPublicAddress();
        }

        public ECDSASignature Sign(byte[] hash)
        {
            var sig = DoSign(hash);
            var thisKey = GetPubKey();
            var recId = CalculateRecId(sig, hash, thisKey);
            sig.V = new byte[] { (byte)recId };
            return sig;
        }

        internal static int CalculateRecId(ECDSASignature signature, byte[] hash, byte[] uncompressedPublicKey)
        {
            var recId = -1;

            for (var i = 0; i < 4; i++)
            {
                var rec = ECKey.RecoverFromSignature(i, signature, hash, false);
                if (rec != null)
                {
                    var k = rec.GetPubKey(false);
                    if (k != null && k.SequenceEqual(uncompressedPublicKey))
                    {
                        recId = i;
                        break;
                    }
                }
            }
            if (recId == -1)
                throw new Exception("Could not construct a recoverable key. This should never happen.");
            return recId;
        }

        private ECDSASignature DoSign(byte[] input)
        {
            if (input.Length != 32)
            {
                throw new ArgumentException(
                    "Expected 32 byte input to " + "ECDSA signature, not " + input.Length);
            }

            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            var privKeyParams = new ECPrivateKeyParameters(_ecKey.PrivateKey.D, ECKey.CURVE);
            signer.Init(true, privKeyParams);

            var components = signer.GenerateSignature(input);

            return new ECDSASignature(components[0], components[1]).MakeCanonical();
        }
    }
}


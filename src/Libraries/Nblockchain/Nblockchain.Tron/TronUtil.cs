using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Nblockchain.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Numerics;

namespace Nblockchain.Tron
{
    /// <summary>
    /// Tron 辅助
    /// </summary>
    public static class TronUtil
    {
        /// <summary>
        /// Sun 单位
        /// </summary>
        private static BigInteger _sunUnit = 1_000_000L;

        /// <summary>
        /// 数值转 Sun
        /// </summary>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static BigInteger DecimalToSun(decimal decimalValue)
        {
            return new BigInteger(decimalValue) * _sunUnit;
        }

        /// <summary>
        /// Sun 转数值
        /// </summary>
        /// <param name="sun"></param>
        /// <returns></returns>
        public static decimal SunToDecimal(BigInteger bigIntegerValue)
        {
            var result = bigIntegerValue / _sunUnit;

            var minValue = new BigInteger(decimal.MinValue);
            if (result < minValue)
            {
                return decimal.MinValue;
            }
            var maxValue = new BigInteger(decimal.MaxValue);
            if (result > maxValue)
            {
                return decimal.MaxValue;
            }

            return (decimal)(bigIntegerValue / _sunUnit);
        }

        /// <summary>
        /// 数值 转 Biginter
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">小数精度</param>
        /// <returns></returns>
        public static BigInteger ToBigInteger(decimal value, byte decimals = 0)
        {
            return new BigInteger(value * (long)Math.Pow(10, decimals));
        }

        /// <summary>
        /// Biginter 转数值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">小数精度</param>
        /// <returns></returns>
        public static decimal ToDecimal(BigInteger value, byte decimals = 0)
        {
            var minValue = new BigInteger(decimal.MinValue);
            if (value < minValue)
            {
                return -9999999999999.999999m;
            }
            var maxValue = new BigInteger(decimal.MaxValue);
            if (value > maxValue)
            {
                return 9999999999999.999999m;
            }

            var decimalValue = 0.0m;
            try
            {
                decimalValue = (decimal)(value);
            }
            catch
            {
                return decimalValue;
            }
            if (decimals > 0)
            {
                decimalValue /= Convert.ToDecimal(Math.Pow(10, decimals));
            }
            return decimalValue;
        }

        /// <summary>
        /// 修正ABI参数中的钱包地址（Tron 钱包第1位为网络标识，在ABI编码时需要去掉标识）
        /// </summary>
        /// <param name="addressBytes"></param>
        /// <returns></returns>
        public static string RemoveAddressPrefixToHex(byte[] addressBytes)
        {
            int length = addressBytes.Length;
            if (length != 21)
            {
                throw new Exception($"Invalid wallet address.");
            }
            return Convert.ToHexString(addressBytes.SubArray(1, length - 1)).ToLowerInvariant();
        }

        /// <summary>
        /// 转换地址
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static ByteString ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            byte[]? raw;
            if (address.StartsWith("T"))
            {
                raw = Base58Encoder.DecodeFromBase58Check(address);
                if (raw is null)
                {
                    throw new ArgumentException($"Invalid address: " + address);
                }
            }
            else if (address.StartsWith("41"))
            {
                raw = address.HexToByteArray();
            }
            else if (address.StartsWith("0x"))
            {
                raw = address[2..].HexToByteArray();
            }
            else
            {
                try
                {
                    raw = address.HexToByteArray();
                }
                catch (Exception)
                {
                    throw new ArgumentException($"Invalid address: " + address);
                }
            }
            return ByteString.CopyFrom(raw);
        }
    }
}

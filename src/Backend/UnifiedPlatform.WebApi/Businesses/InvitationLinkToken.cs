using UnifiedPlatform.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace UnifiedPlatform.WebApi
{
    /// <summary>
    /// 推荐链接令牌
    /// </summary>
    public class InvitationLinkToken
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; private set; }

        /// <summary>
        /// 邀请者类型
        /// </summary>
        public InviterType InviterType { get; private set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; private set; } = null!;

        private InvitationLinkToken()
        {
        }

        /// <summary>
        /// 创建推荐链接令牌
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="inviterType">邀请者类型</param>
        /// <returns></returns>
        public static InvitationLinkToken Create(int uid, InviterType inviterType)
        {
            string? tokenText = $"{uid}.{(int)inviterType}";
            tokenText = Convert.ToHexString(Encrypt(tokenText));
            return new InvitationLinkToken()
            {
                Uid = uid,
                InviterType = inviterType,
                Token = tokenText
            };
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="tokenText">令牌文本</param>
        /// <param name="referralLinkToken">推荐链接令牌</param>
        /// <returns></returns>
        public static bool TryDeserialize(string? tokenText, [NotNullWhen(true)] out InvitationLinkToken? invitationLinkToken)
        {
            invitationLinkToken = null;
            if (string.IsNullOrWhiteSpace(tokenText))
            {
                return false;
            }

            byte[] decryptedBytes;
            try
            {
                decryptedBytes = Encrypt(Convert.FromHexString(tokenText));
            }
            catch
            {
                return false;
            }
            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            List<string>? array = decryptedText.Split('.')?.ToList();
            if (array is null || array.Count != 2)
            {
                return false;
            }

            string left = array[0];
            string right = array[1];
            if (!int.TryParse(left, out int value1) || !int.TryParse(right, out int value2))
            {
                return false;
            }
            InviterType inviterType = (InviterType)value2;
            if (!Enum.IsDefined(inviterType))
            {
                return false;
            }
            invitationLinkToken = Create(value1, inviterType);
            return true;
        }

        /// <summary>
        /// 简单加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] source)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                source[i] = (byte)(255 - source[i]);
            }
            return source;
        }

        /// <summary>
        /// 简单加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static byte[] Encrypt(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            return Encrypt(bytes);
        }
    }
}


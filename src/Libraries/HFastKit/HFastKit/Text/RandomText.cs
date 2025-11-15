using System.Text;

namespace HFastKit.Text
{
    /// <summary>
    /// 随机文本
    /// </summary>
    public class RandomText
    {
        /// <summary>
        /// 默认字符表
        /// </summary>
        private static readonly char[] _charArray =
        {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        };

        /// <summary>
        /// 随机生成指定长度字母和数字
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string Generate(int length, bool isIncludeCase = false)
        {
            if (length < 1)
            {
                return string.Empty;
            }
            StringBuilder result = new(length);
            for (int i = 0; i < length; i++)
            {
                var c = _charArray[Random.Shared.Next(_charArray.Length)];
                if (isIncludeCase && Random.Shared.Next(2) == 1)
                {
                    c = char.ToUpper(c);
                }
                result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// 默认字符表
        /// </summary>
        private static readonly char[] _hexCharArray =
        {
            '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f',
        };

        /// <summary>
        /// 随机生成指定长度的HEX字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GenerateHexText(int length, bool isIncludeCase = false)
        {
            if (length < 1)
            {
                return string.Empty;
            }
            StringBuilder result = new(length);
            for (int i = 0; i < length; i++)
            {
                var c = _hexCharArray[Random.Shared.Next(_hexCharArray.Length)];
                if (isIncludeCase && Random.Shared.Next(2) == 1)
                {
                    c = char.ToUpper(c);
                }
                result.Append(c);
            }
            return result.ToString();
        }
    }
}


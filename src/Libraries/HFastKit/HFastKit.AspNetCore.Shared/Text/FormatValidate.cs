using System.Text.RegularExpressions;

namespace HFastKit.AspNetCore.Shared.Text;

/// <summary>
/// 字符串格式验证
/// </summary>
public static class FormatValidate
{
    /// <summary>
    /// Email 匹配正则
    /// </summary>
    private static readonly Regex _emailRegex = new(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);

    /// <summary>
    /// 英文匹配正则
    /// </summary>
    private readonly static Regex _englishRegex = new(@"^[A-Za-z]+$", RegexOptions.Compiled);

    /// <summary>
    /// 数字匹配正则
    /// </summary>
    private readonly static Regex _numberRegex = new(@"^[0-9]*$", RegexOptions.Compiled);

    /// <summary>
    /// 十六进制文本匹配
    /// </summary>
    private static readonly Regex _hex = new(@"^(0[xX])?[0-9a-fA-F]+$", RegexOptions.Compiled);

    /// <summary>
    /// 中文匹配正则
    /// </summary>
    private static readonly Regex _chineseRegex = new(@"^[\u4e00-\u9fa5]+$", RegexOptions.Compiled);

    /// <summary>
    /// 以太坊钱包地址匹配
    /// </summary>
    private static readonly Regex _ethereumAddressRegex = new(@"^0x[0-9a-fA-F]{40}$", RegexOptions.Compiled);

    /// <summary>
    /// Tron钱包地址匹配
    /// </summary>
    private static readonly Regex _tronAddressRegex = new(@"^T[0-9a-zA-Z]{33}$", RegexOptions.Compiled);

    /// <summary>
    /// 交易ID匹配
    /// </summary>
    private static readonly Regex _transactionIdRegex = new(@"^(0[xX])?[0-9a-fA-F]{64}$", RegexOptions.Compiled);

    /// <summary>
    /// 验证字符串是否为邮箱
    /// </summary>
    /// <param name="text">字符串</param>
    /// <returns></returns>
    public static bool IsEmail(string text) => _emailRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为英文
    /// </summary>
    /// <param name="text">字符串</param>
    /// <returns></returns>
    public static bool IsEnglish(string text) => _englishRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为数字
    /// </summary>
    /// <param name="text">字符串</param>
    /// <returns></returns>
    public static bool IsNumber(string text) => _numberRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为十六进制文本
    /// </summary>
    /// <param name="text">字符串</param>
    /// <returns></returns>
    public static bool IsHex(string text) => _hex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为中文
    /// </summary>
    /// <param name="text">字符串</param>
    /// <returns></returns>
    public static bool IsChinese(string text) => _chineseRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为用户名（A-Z、a-z、0-9、下划线组成，不能以下划线开头）
    /// </summary>
    /// <param name="text">字符串</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsUsername(string text, int? minLength = 4, int? maxLength = 30)
    {
        CheckLengthArguments(minLength, maxLength);
        string pattern = @"(?!_)(?!.*?_$)[0-9a-zA-Z_]";
        if (minLength is null && maxLength is null)
        {
            pattern = $@"^{pattern}+$";
        }
        else if (minLength is not null && maxLength is null)
        {
            pattern = $@"^{pattern}{{{minLength},}}$";
        }
        else if (minLength is null && maxLength is not null)
        {
            pattern = $@"^{pattern}{{1,{maxLength}}}$";
        }
        else
        {
            pattern = $@"^{pattern}{{{minLength},{maxLength}}}$";
        }
        return Regex.IsMatch(text, pattern);
    }

    /// <summary>
    /// 验证字符串是否为密码（至少一个数字和字母）
    /// </summary>
    /// <param name="text">字符串</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsPassword(string text, int? minLength = 6, int? maxLength = 30)
    {
        CheckLengthArguments(minLength, maxLength);
        var pattern = @"(?=.*\d)(?=.*[a-zA-Z]).";
        if (minLength is null && maxLength is null)
        {
            pattern = $@"^{pattern}+$";
        }
        else if (minLength is not null && maxLength is null)
        {
            pattern = $@"^{pattern}{{{minLength},}}$";
        }
        else if (minLength is null && maxLength is not null)
        {
            pattern = $@"^{pattern}{{1,{maxLength}}}$";
        }
        else
        {
            pattern = $@"^{pattern}{{{minLength},{maxLength}}}$";
        }
        return Regex.IsMatch(text, pattern);
    }

    /// <summary>
    /// 验证字符串是否为ETH钱包地址
    /// </summary>
    /// <param name="text">钱包地址</param>
    /// <returns>验证结果</returns>
    public static bool IsEthereumAddress(string text) => _ethereumAddressRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为Tron钱包地址
    /// </summary>
    /// <param name="text">钱包地址</param>
    /// <returns>验证结果</returns>
    public static bool IsTronAddress(string text) => _tronAddressRegex.IsMatch(text);

    /// <summary>
    /// 验证字符串是否为区块链交易ID
    /// </summary>
    /// <param name="text">交易ID</param>
    /// <returns>验证结果</returns>
    public static bool IsTransactionId(string text) => _transactionIdRegex.IsMatch(text);

    /// <summary>
    /// 验证长度参数
    /// </summary>
    /// <param name="text">字符串</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns></returns>
    private static void CheckLengthArguments(int? minLength, int? maxLength)
    {
        bool result = minLength is null or not < 1 && maxLength is null or not < 1 && (minLength is null || maxLength is null || maxLength >= minLength);
        if (!result)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}

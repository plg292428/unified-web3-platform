namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 检查登录响应数据
    /// </summary>
    public class DappUserCheckSinginedResult
    {
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool Singined { get; set; } = false;

        /// <summary>
        /// 令牌文本
        /// </summary>
        public string? TokenText { get; set; }
    }
}

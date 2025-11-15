namespace SmallTarget.DbService
{
    /// <summary>
    /// 数据配置异常
    /// </summary>
    public class DataConfigurationException : Exception
    {
        /// <summary>
        /// 数据配置异常
        /// </summary>
        /// <param name="message">异常消息</param>
        public DataConfigurationException(string? message) : base(message)
        {
        }
    }
}


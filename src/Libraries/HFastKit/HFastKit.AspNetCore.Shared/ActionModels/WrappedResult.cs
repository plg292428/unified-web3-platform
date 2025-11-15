using System.Text.Json.Serialization;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// 包装响应结果接口
    /// </summary>
    public interface IWrappedResult
    {
        /// <summary>
        /// 业务处理是否成功
        /// </summary>
        public bool Succeed { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 响应附加数据
        /// </summary>
        public object? Data { get; set; }
    }

    /// <summary>
    /// 包装响应结果
    /// </summary>
    public sealed class WrappedResult : IWrappedResult
    {
        /// <summary>
        /// 业务处理是否成功
        /// </summary>
        public bool Succeed { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 响应附加数据
        /// </summary>
        public object? Data { get; set; }


        public WrappedResult() { }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="succeed">业务逻辑是否成功</param>
        /// <param name="data">附加数据</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns></returns>
        public static WrappedResult Create(bool succeed, object? data = default, string? errorMessage = null)
        {
            return new WrappedResult()
            {
                Succeed = succeed,
                Data = data,
                ErrorMessage = errorMessage,
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static WrappedResult Ok()
        {
            return Create(true);
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">附加数据</param>
        /// <returns></returns>
        public static WrappedResult Ok(object? data)
        {
            return Create(true, data);
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <returns></returns>
        public static WrappedResult Failed(string? errorMessage = null)
        {
            return Create(false, null, errorMessage);
        }
    }

    /// <summary>
    /// 包装响应结果泛型
    /// </summary>
    public sealed class WrappedResult<T> : IConvertToWrappedResult
    {
        /// <summary>
        /// 业务处理是否成功
        /// </summary>
        public bool Succeed { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 响应附加数据
        /// </summary>
        public T? Data { get; set; } = default!;

        /// <summary>
        /// 不要删 Json格式化库默认需要0个参数的构造方法
        /// </summary>
        public WrappedResult() { }

        public WrappedResult(WrappedResult wrappedResult)
        {
            Succeed = wrappedResult.Succeed;
            ErrorMessage = wrappedResult.ErrorMessage;
            Data = (T?)wrappedResult.Data;
        }

        /// <summary>
        /// 实现将 <see cref="WrappedResult"/> 隐式转换为 <see cref="WrappedResult{T}"/>
        /// </summary>
        /// <param name="result"><see cref="WrappedResult"/></param>
        public static implicit operator WrappedResult<T>(WrappedResult result)
        {
            return new WrappedResult<T>(result);
        }

        /// <summary>
        /// 实现将 <see cref="WrappedResult{T}"/> 隐式转换为 <see cref="WrappedResult"/>
        /// </summary>
        /// <param name="result"><see cref="WrappedResult{T}"/></param>
        //public static implicit operator WrappedResult(WrappedResult<T> result)
        //{
        //    return WrappedResult.Create(result.Succeed, result.Data, result.ErrorMessage);
        //}

        /// <summary>
        /// 转换为<see cref="WrappedResult"/>
        /// </summary>
        /// <returns></returns>
        public WrappedResult Convert()
        {
            return WrappedResult.Create(Succeed, Data, ErrorMessage);
        }
    }
}

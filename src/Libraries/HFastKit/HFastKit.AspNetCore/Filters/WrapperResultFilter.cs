using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HFastKit.AspNetCore.Filters
{
    /// <summary>
    /// 包装响应结果过滤器
    /// </summary>
    public class WrapperResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            switch (context.Result)
            {
                case null:
                    break;
                case ObjectResult objectResult:
                    {
                        var resultValue = objectResult.Value;
                        if (objectResult.DeclaredType is not null)
                        {
                            if (objectResult.DeclaredType == typeof(WrappedResult) || objectResult.DeclaredType == typeof(IWrappedResult))
                            {
                                break;
                            }

                            if (objectResult.DeclaredType.IsGenericType && objectResult.DeclaredType.GetGenericTypeDefinition() == typeof(WrappedResult<>))
                            {
                                // 调用转换器
                                if (objectResult.Value is IConvertToWrappedResult convert)
                                {
                                    var wrappedResult = convert.Convert();
                                    objectResult.Value = wrappedResult;
                                    objectResult.DeclaredType = wrappedResult.GetType();
                                    break;
                                }
                            }
                        }

                        var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
                        if (statusCode is >= 200 and < 400)
                        {
                            var wrappedResult = WrappedResult.Ok(resultValue);
                            objectResult.Value = wrappedResult;
                            objectResult.DeclaredType = wrappedResult.GetType();
                        }
                        else
                        {
                            var wrappedResult = WrappedResult.Failed();
                            if (objectResult.Value is not null)
                            {
                                if (objectResult.Value is ValidationProblemDetails validationProblemDetails && validationProblemDetails.Errors.Count > 0)
                                {
                                    foreach (var error in validationProblemDetails.Errors)
                                    {
                                        wrappedResult.ErrorMessage += string.Join(Environment.NewLine, error.Value);
                                    }
                                }
                                else
                                {
                                    if (objectResult.Value is ProblemDetails problemDetails)
                                    {
                                        wrappedResult.ErrorMessage = problemDetails.Title;
                                    }
                                }
                            }
                            wrappedResult.ErrorMessage ??= "Unknown Error";
                            objectResult.Value = wrappedResult;
                            objectResult.DeclaredType = wrappedResult.GetType();
                        }
                        break;
                    }
                case EmptyResult emptyResult:
                    {
                        var wrappedResult = WrappedResult.Ok();
                        context.Result = new ObjectResult(wrappedResult);
                        break;
                    }
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}

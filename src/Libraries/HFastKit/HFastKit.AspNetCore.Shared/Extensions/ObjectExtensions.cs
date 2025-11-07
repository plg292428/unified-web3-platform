using System.Text.Json;

namespace HFastKit.AspNetCore.Shared.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 深拷贝对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T? CloneByJsonSerializer<T>(this T? obj) where T : class?
        {
            if (ReferenceEquals(obj, null))
            {
                return default;
            }
            var step1 = JsonSerializer.Serialize(obj, typeof(T));
            return JsonSerializer.Deserialize<T>(step1);
        }
    }
}

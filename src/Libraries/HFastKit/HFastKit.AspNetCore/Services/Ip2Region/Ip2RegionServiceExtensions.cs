using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// IP 归属查询服务
    /// </summary>
    public static class Ip2RegionServiceExtensions
    {

        private static string _defaultXdbPath = Path.Combine(Environment.CurrentDirectory, "ip2region.xdb");
        /// <summary>
        /// 添加 IP 归属查询服务
        /// </summary>
        /// <param name="services">服务</param>
        public static void AddIp2RegionService(this IServiceCollection services, string? xdbPath = default)
        {
            xdbPath ??= _defaultXdbPath;
            if (!File.Exists(xdbPath)) throw new Exception("ip2region.xdb does not exist");
            services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content, xdbPath));
        }

        /// <summary>
        /// 搜索和修正
        /// </summary>
        /// <param name="searcher"></param>
        /// <returns></returns>
        public static string? SearchAndFix(this ISearcher searcher, string ipStr)
        { 
            var region = searcher.Search(ipStr);

            if (string.IsNullOrEmpty(region))
            {
                return "未知地址";
            }

            // 过滤大本营
            if (region?.Contains("云南") == true || region?.Contains("玉溪") == true)
            {
                return "未知地址";
            }

            var regionArray = region?.Split("|");
            if (regionArray is null || regionArray.Count() != 5) return region;

            // 内网地址
            if (regionArray[3].Contains("内网") || regionArray[4].Contains("内网"))
            {
                return "内网地址";
            }
            region = region?.Replace("0|", "");
            region = region?.Replace("|0", "");
            region = region?.Replace("|", "-");
            return string.IsNullOrWhiteSpace(region) ? "未知地址" : region;
        }
    }
}


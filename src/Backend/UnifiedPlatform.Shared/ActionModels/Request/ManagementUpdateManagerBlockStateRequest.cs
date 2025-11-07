namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端更新代理和下级员工禁用状态
    /// </summary>
    public class ManagementUpdateManagerBlockStateRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 禁用状态
        /// </summary>
        public bool Blocked { get; set; }
    }
}

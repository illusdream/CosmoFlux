using System;

namespace ilsFramework.Core
{
    /// <summary>
    /// 配置表 表内数据基类
    /// </summary>
    [Serializable]
    public class TableItem
    {
        /// <summary>
        /// ID，表头第1列一定是这个,用于作为主键查询整个配置表
        /// </summary>
        public uint ID;
    }
}
using ilsFramework.Core;

namespace Game
{
    public class tShipModular : TableItem
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 模块名字
        /// </summary>
        public string ModularGroup;
        
        /// <summary>
        /// 这个模块对应预制体的位置
        /// </summary>
        public string Path;
        
        /// <summary>
        /// 模块的类型
        /// </summary>
        [Index]
        public EModularType ModularType;
    }
}
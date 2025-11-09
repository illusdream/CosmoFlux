using System;

namespace ilsFramework.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class UIBinder : Attribute
    {
        public UIBinder(string key)
        {
            this.key = key;
        }
        
        public string key { get; }
    }
}
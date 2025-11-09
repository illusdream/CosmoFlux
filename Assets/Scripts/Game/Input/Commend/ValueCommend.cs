using Sirenix.OdinInspector;

namespace Game.Input
{
    public class ValueCommend<T> : BaseCommend
    {
        public ValueCommend(T value) : base()
        {
            Value = value;
        }
        [ShowInInspector]
        public T Value { get; }
    }
}
namespace ilsFramework.Core
{
    public class ModelReference<T> where T : BaseModel
    {
        public T Value => ModelManager.Instance.GetModel<T>();
        
    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace ilsFramework.Core
{
    public class ModelManager : ManagerSingleton<ModelManager>
    {
        Dictionary<Type,BaseModel> models = new Dictionary<Type, BaseModel>();
        public override IEnumerator OnInit()
        {
            yield return null;
        }

        public override void OnUpdate()
        {
           
        }

        public override void OnLateUpdate()
        {
          
        }

        public override void OnLogicUpdate()
        {
           
        }

        public override void OnFixedUpdate()
        {
        
        }

        public override void OnDestroy()
        {
           
        }

        public override void OnDrawGizmos()
        {
          
        }

        public override void OnDrawGizmosSelected()
        {
           
        }

        public T GetModel<T>() where T : BaseModel
        {
            if (models.ContainsKey(typeof(T)))
            {
                return (T)models[typeof(T)];
            }
            else
            {
                var instance = Activator.CreateInstance<T>();
                instance.Initialize();
                models.Add(typeof(T), instance);
                return instance;
            }
        }
    }
}
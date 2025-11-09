using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public abstract class ECSLinkerComponet : MonoBehaviour
    {
        public Unity.Entities.EntityManager EntityManager { get; set; }
        public Entity ECSLinkEntity { get; set; }
        public abstract void OnCreate();
        
        

        public abstract void OnSetArchetype(List<ComponentType> types);
        
        public abstract void OnSetComponentData(Unity.Entities.EntityManager entityManager, Entity entity);
        
        public abstract void OnMonoDestroy(Unity.Entities.EntityManager entityManager, Entity entity);
    }
}
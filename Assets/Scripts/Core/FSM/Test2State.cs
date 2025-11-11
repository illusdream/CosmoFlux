using UnityEngine;

namespace ilsFramework.Core
{
    public class Test2State : State
    {
        public float timer = 2;

        public override State GetTransition()
        {
            if (timer <= 0)
               return  GetStateInSameLayer("1");
            return null;
        }

        public override void OnEnter()
        {
            timer = 2;
            "切换2".WarningSelf();
            base.OnEnter();
        }

        public override void OnUnityUpdate()
        {
            
            timer -= Time.deltaTime;
            base.OnUnityUpdate();
        }
    }
}
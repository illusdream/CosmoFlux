using Cysharp.Threading.Tasks;
using ilsFramework.Core;
using UnityEngine.SceneManagement;

namespace Game
{
    public class InMainMenuState : State
    {
        public const string StateKey = "InMainMenuState";

        public override State GetTransition()
        {
            return GetStateInSameLayer(NormalGameState.StateKey);
            return base.GetTransition();
        }

        public override void AddExitTask(in TransitionSequencer sequencer)
        {
            sequencer.Enqueue(SceneManager.LoadSceneAsync((int)EScene.Test).ToUniTask());
            base.AddExitTask(in sequencer);
        }
    }
}
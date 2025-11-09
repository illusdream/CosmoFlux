using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class MainBaseCameraController : BaseCameraController
    {
        public Camera Camera {get; private set;}
        public override string CameraPrefabPath =>"Main Camera";
        public override bool IsInstance => true;
        
        public CinemachineBrain Brain {get; private set;}

        public void SetMainCamera(Camera camera,CinemachineBrain brain)
        {
            Camera = camera;
            Brain = brain;
        }
    }
}
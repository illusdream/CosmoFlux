using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ilsFramework.Core
{
    /// <summary>
    ///     框架核心与Unity 更新的对接器
    /// </summary>
    [HideLabel]
    [HideMonoScript]
    public class FrameworkCoreMonoHandler : MonoBehaviour
    {
        [ShowInInspector] [HideLabel] public FrameworkCore FrameworkCore => FrameworkCore.Instance;

        public bool FrameworkIsInitialized { get; private set; } = false;
        public void Awake()
        {
            FrameworkCore.frameworkGOBaseTransform = transform;
            StartCoroutine(StartInitialize());
        }

        private IEnumerator StartInitialize()
        {
            yield return FrameworkCore.Instance.Initialize();
            FrameworkIsInitialized = true;
        }

        public void Start()
        {
        }

        public void Update()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.Update();
        }

        public void FixedUpdate()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.FixedUpdate();
        }

        public void LateUpdate()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.LateUpdate();
        }

        public void OnDestroy()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.OnDestroy();
        }
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.OnDrawGizmos();
        }

        public void OnDrawGizmosSelected()
        {
            if (!FrameworkIsInitialized) return;
            FrameworkCore.Instance.OnDrawGizmosSelected();
        }
#endif


        [RuntimeInitializeOnLoadMethod]
        private static void InitializeFramework()
        {

            //挂载
            var instance = new GameObject("ilsFramework");
            DontDestroyOnLoad(instance);
            instance.AddComponent<FrameworkCoreMonoHandler>();

#if UNITY_EDITOR
            if (!Config.GetConfigInEditor<ProcedureConfig>().EnableCommenProcedure)
            {
                return;
            }
#endif
            
            if (SceneManager.GetActiveScene().buildIndex == (int)EScene.Enter)
            {
                return;
            }
            SceneManager.LoadScene((int)EScene.Enter);
        }
    }
}
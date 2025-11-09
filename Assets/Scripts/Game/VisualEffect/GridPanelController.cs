using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.VisualEffect
{
    public class GridPanelController : MonoBehaviour
    {
        private static readonly int Size = Shader.PropertyToID("_Size");
        private static readonly int WireSize1 = Shader.PropertyToID("_WireSize");
        private static readonly int Radius = Shader.PropertyToID("_FadeRadius");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        
        public static Vector2 DefaultGridSize = Vector2.one * 1;
        public static Vector2 DefaultPanelSize = Vector2.one * 40;
        public static Vector2 DefaultWireSize = Vector2.one * 0.05f;
        public static float DefaultFadeRadius = 0.5f;
        public static Color DefaultColor =new Color(0,5.24869823f,11.9843149f,1);
        public static float Instansity = 1;
        public MeshRenderer mRenderer;

        private MaterialPropertyBlock mPropertyBlock;

        //每Unit 中有多少个格子
        public Vector2 GridSize = DefaultGridSize;

        //格子平面的总大小
        public Vector2 PanelSize = DefaultPanelSize;
        public Vector2 WireSize = DefaultWireSize;
        public float FadeRadius = DefaultFadeRadius;
        
        [ColorUsage(true, true)]
        public Color WireColor = DefaultColor;
        
        public void Awake()
        {
            mPropertyBlock = new MaterialPropertyBlock();
            mRenderer.GetPropertyBlock(mPropertyBlock);
        }

        public void Update()
        {
            transform.localScale = new Vector3(PanelSize.x / 10f, 1, PanelSize.y / 10f);
            mPropertyBlock.SetVector(Size, GridSize * PanelSize);
            mPropertyBlock.SetVector(WireSize1,Vector2.one - WireSize);
            mPropertyBlock.SetFloat(Radius,FadeRadius);
            mPropertyBlock.SetColor(Color1,WireColor);
            mRenderer.SetPropertyBlock(mPropertyBlock);
        }

        public void Open(float time)
        {
            this.gameObject.SetActive(true);
            FadeRadius = 0;
            var seq = DOTween.Sequence();
            seq.Append(DOTween.To(()=>FadeRadius,(value)=>FadeRadius=value,0.5f,time));
        }

        public void Close(float time)
        {
            var seq = DOTween.Sequence();
            seq.Append(DOTween.To(()=>FadeRadius,(value)=>FadeRadius=value,0f,time).SetTarget(this));
            seq.OnStepComplete((() =>
            {
                GameObject.Destroy(this.gameObject);
            })).SetTarget(this);
            
        }

        public void OnDestroy()
        {
            
        }
    }
}
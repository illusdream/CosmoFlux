using System;
using UnityEngine;

namespace Game.VisualEffect
{
    public class AimCenterController : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        public MeshRenderer mRenderer;

        private MaterialPropertyBlock mPropertyBlock;

        public Color Color;
        public void Start()
        {
            mPropertyBlock = new MaterialPropertyBlock();
            mRenderer.GetPropertyBlock(mPropertyBlock);
        }

        public void Update()
        {
            mPropertyBlock.SetColor(BaseColor, Color);
            mRenderer.SetPropertyBlock(mPropertyBlock);
        }
    }
}
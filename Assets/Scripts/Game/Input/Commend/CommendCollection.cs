using System.Collections.Generic;
using System.Linq;
using ilsFramework.Core;
using Sirenix.OdinInspector;

namespace Game.Input
{
    public class CommendCollection
    {
        [ShowInInspector]
        private List<ICommend> buffer;

        private int bufferLength = 50;
        
        private int firstCommendFrameIndex = 0;
        private int lastCommendFrameIndex = 0;

        public CommendCollection(int bufferLength)
        {
            this.bufferLength = bufferLength;
            buffer = new List<ICommend>(bufferLength);
        }
        
        public void Update()
        {
            //最后一个默认是最早加入的
            while (buffer.Any() &&  FrameworkCore.Instance.LogicFrameIndex - buffer.Last().FrameIndex >= 50)
            {
                buffer.RemoveAt(buffer.Count - 1);
            }
            UpdateCommendIndexBuffer();
        }

        public void UpdateCommendIndexBuffer()
        {
            if (!buffer.Any())
            {
                return;
            }
            
            firstCommendFrameIndex =FrameworkCore.Instance.LogicFrameIndex - buffer.First().FrameIndex ;
            lastCommendFrameIndex =FrameworkCore.Instance.LogicFrameIndex - buffer.Last().FrameIndex ;
        }

        public void Add(ICommend commend)
        {
            buffer.Insert(0,commend);
        }

        public void Query<T>(int firstIndex, int lastIndex, List<T> result) where T : ICommend
        {
            if (firstIndex > lastCommendFrameIndex)
            {
                return;
            }

            if (lastIndex < firstCommendFrameIndex)
            {
                return;
            }

            foreach (ICommend commend in buffer)
            {
                if (FrameworkCore.Instance.LogicFrameIndex - commend.FrameIndex < firstCommendFrameIndex) continue;
                if (commend.FrameIndex - firstCommendFrameIndex > lastCommendFrameIndex) break;
                if (commend is T target)
                {
                    result.Add(target);
                }
            }
        }

        public bool CheckCurrent<T>(out T commend) where T :class,ICommend
        { 
            commend = null;
            if (!buffer.Any()) return false;
            if (buffer[0].FrameIndex != FrameworkCore.Instance.LogicFrameIndex) return false;
            if (buffer[0] is not T final) return false;
            commend = final;
            return true;
        }

        public void Clear()
        {
            buffer.Clear();
        }
    }
}
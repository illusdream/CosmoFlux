using Sirenix.OdinInspector;

namespace Game
{
    public class ModularEdge
    {
        public ModularEdge()
        {
            
        }
        
        public ModularEdge(int from, int to)
        {
            From = from;
            To = to;
        }
        public ModularEdge(ModularGraphNode from, ModularGraphNode to)
        {
            From = from.ID;
            To = to.ID;
        }
        

        
        [ShowInInspector]
        public int From { get; set; }
        [ShowInInspector]
        public int To { get; set; }
    }
}
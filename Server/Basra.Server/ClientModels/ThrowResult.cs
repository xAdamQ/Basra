using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class ThrowResult
    {
        public int ThrownCard { set; get; }
        public List<int> EatenCardsIds { set; get; }
        public bool Basra { set; get; }
        public bool BigBasra { set; get; }
    }
}
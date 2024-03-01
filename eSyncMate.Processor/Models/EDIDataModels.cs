using Newtonsoft.Json;

namespace eSyncMate.Processor.Models
{
    public class SegmentNode
    {
        public string Name { get; set; }
        public List<string> Data { get; set; }
    }

    public class Items : List<LoopItem>
    {
    }

    public class LoopItem
    {
        public List<SegmentNode> Data { get; set; }
    }

    public class LoopPackage
    {
        public List<SegmentNode> Data { get; set; }
        public Items Items { get; set; }
    }

    public class Packs : List<LoopPackage>
    {
    }

    public class LoopOrder
    {
        public List<SegmentNode> Data { get; set; }
        public Packs Packs { get; set; }
    }

    public class StoreOrders : List<LoopOrder>
    {
    }

    public class ASN
    {
        public List<SegmentNode> StartNodes { get; set; }
        public Packs Packs { get; set; }
        public StoreOrders Orders { get; set; }
        public List<SegmentNode> EndNodes { get; set; }
    }

    public class Invoice810
    {
        public List<SegmentNode> StartNodes { get; set; }
        public Items Items { get; set; }
        public List<SegmentNode> EndNodes { get; set; }
    }

    public class OrderStatus855
    {
        public List<SegmentNode> StartNodes { get; set; }
        public Items Items { get; set; }
        public List<SegmentNode> EndNodes { get; set; }
    }
}

using Microsoft.VisualBasic;

namespace eSyncMate.Processor.Models
{
    public class RouteLogModel
    {
        public int Id { get; set; }
    }

    public class RouteLogDataModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }

    public class SaveRouteLogDataModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateRoutLogDataModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class RouteLogSearchModel
    {
        public string SearchOption { get; set; }
        public string SearchValue { get; set; }
    }
}

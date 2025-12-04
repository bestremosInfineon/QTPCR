namespace QTPCR.Models
{
    public class StateDataItem
    {
        public string? Id { get; set; }
        public string? Comment { get; set; }
        public StateItem State { get; set; }

    }

    public class StateItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Code { get; set; }
        public string? ChangeRequest { get; set; }
    }
}

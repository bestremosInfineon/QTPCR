namespace QTPCR.Models
{
    public class TestState
    {
        public int ID { get; set; }
        public string? STATE_NAME { get; set; }
        public int CODE { get; set; }
        public string? ENABLE_CR { get; set; }
    }

    public class QTPTestState
    {
        public string? StateName { get; set; }
        public string? QtpTransID { get; set; }
        public string? TestID { get; set; }
        public List<object[]> TestStates { get; set; }
    }
}

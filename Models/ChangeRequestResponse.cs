namespace QTPCR.Models
{
    public class ChangeRequestResponse
    {
        public bool Success { get; set; }
        public object ResponseText { get; set; }
        public string? ResponseError { get; set; }
        public string? ResponseStatePerLot { get; set; }
        public bool ResponseHasOtherState { get; set; }
        public bool ResponseHasOtherStateAndNoCR { get; set; }
        public int ResponseOnHoldCount { get; set; }
        public List<int> ResponseNALots { get; set; }
    }
}

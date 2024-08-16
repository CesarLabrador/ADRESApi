using AdresApi.Models;

namespace AdresApi.Services
{
    public interface IDatab
    {
        public DefaultResponse AddRecord(RRAData Data);
        public RFilterData FilterData(FilterData Data);
        public RData GetData(double id);
        public DefaultResponse DeleteRecord(double id);
        public DefaultResponse UpdateRecord(RRRAData Data);
    }
}

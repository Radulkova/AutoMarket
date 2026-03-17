namespace AutoMarket.Services
{
    public interface ICompareService
    {
        const string SessionKey = "CompareCarIds";

        List<int> GetComparedCarIds();
        int GetCount();
        bool Contains(int carId);
        (bool success, string message) Add(int carId);
        void Remove(int carId);
        void Clear();
    }
}
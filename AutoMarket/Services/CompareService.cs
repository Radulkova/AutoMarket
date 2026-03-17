using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AutoMarket.Services
{
    public class CompareService : ICompareService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CompareService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public List<int> GetComparedCarIds()
        {
            var session = httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<int>();
            }

            var json = session.GetString(ICompareService.SessionKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<int>();
            }

            try
            {
                var ids = JsonSerializer.Deserialize<List<int>>(json);
                return ids ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public int GetCount()
        {
            return GetComparedCarIds().Count;
        }

        public bool Contains(int carId)
        {
            return GetComparedCarIds().Contains(carId);
        }

        public (bool success, string message) Add(int carId)
        {
            var ids = GetComparedCarIds();

            if (ids.Contains(carId))
            {
                return (false, "Този автомобил вече е добавен за сравнение.");
            }

            if (ids.Count >= 3)
            {
                return (false, "Може да сравняваш най-много 3 автомобила едновременно.");
            }

            ids.Add(carId);
            Save(ids);

            return (true, "Автомобилът е добавен за сравнение.");
        }

        public void Remove(int carId)
        {
            var ids = GetComparedCarIds();
            if (ids.Remove(carId))
            {
                Save(ids);
            }
        }

        public void Clear()
        {
            var session = httpContextAccessor.HttpContext?.Session;
            session?.Remove(ICompareService.SessionKey);
        }

        private void Save(List<int> ids)
        {
            var session = httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return;
            }

            var json = JsonSerializer.Serialize(ids);
            session.SetString(ICompareService.SessionKey, json);
        }
    }
}
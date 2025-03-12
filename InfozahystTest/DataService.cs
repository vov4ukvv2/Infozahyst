using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfozahystTest
{
    public class DataService
    {
        private readonly INetworkService _networkService;

        public DataService(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public async Task<string> FetchData(string url)
        {
            var data = await _networkService.GetDataFromApiAsync(url);
            return data;
        }
    }
    public interface INetworkService
    {
        Task<string> GetDataFromApiAsync(string url);
    }
}

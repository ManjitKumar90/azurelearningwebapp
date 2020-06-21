using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RediscacheImplementation.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase database;

        private readonly IConfiguration configuration;

        public CacheService(IConfiguration configuration)
        {
            this.configuration = configuration;
            string connectionString = this.configuration["RedisConnectionString"];
            database = GetDataBase(connectionString);
        }

        private IDatabase GetDataBase(string connectionString)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {

                return ConnectionMultiplexer.Connect(connectionString);
            });

            return lazyConnection.Value.GetDatabase();
        }

        public async Task<T> GetData<T>(string key)
        {
            key = key ?? throw new System.ArgumentNullException(nameof(key));

            var result =  await database.StringGetAsync(key).ConfigureAwait(false);

            return (result == RedisValue.Null) ? default(T) : JsonConvert.DeserializeObject<T>(result);
        }

        public async Task SetData<T>(string key, T t, TimeSpan timeSpan)
        {
            var json = JsonConvert.SerializeObject(t);
            await database.StringSetAsync(key, json, timeSpan);
        }
    }
}

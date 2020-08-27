using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewLife.Caching;

namespace testNewLifeCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEnumerable<ICache> _caches;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEnumerable<ICache> caches)
        {
            _logger = logger;
            _caches = caches;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var key = "cash-user";

            _caches.FirstOrDefault(a => a.GetType().Name == CacheProvider.MemoryCache.ToString())
                   .Set(key, new User { Name = "NewLife Memory", CreateTime = DateTime.Now }, 3600);
            
            _caches.FirstOrDefault(a => a.GetType().Name == CacheProvider.FullRedis.ToString())
                   .Set(key, new User { Name = "NewLife Redis", CreateTime = DateTime.Now }, 3600);

            foreach (var cache in _caches)
            {
                Console.WriteLine(cache.GetType().Name);
                var user = cache.Get<User>(key);

                Console.WriteLine(user);
            }
            
            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                             {
                                 Date = DateTime.Now.AddDays(index),
                                 TemperatureC = rng.Next(-20, 55),
                                 Summary = Summaries[rng.Next(Summaries.Length)]
                             })
                             .ToArray();
        }
    }

    public class User
    {
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(CreateTime)}: {CreateTime:O}";
        }
    }

    enum CacheProvider : byte
    {
        MemoryCache = 1,
        FullRedis = 2
    }
}
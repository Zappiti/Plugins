using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Zappiti.Api.Common;
using Zappiti.Common.Cache;
using Zappiti.Common.Core;
using Zappiti.Common.Scrap;
using Zappiti.Common.Scrap.Scraper;
using Zappiti.Framework;

namespace Zappiti.Plugin.MovieScraperExample
{
    class MovieScraperExample : IMovieScraper
    {
        private readonly INfoFactory _nfoFactory;
        private const int CacheDurationDays = 7;

        public MovieScraperExample()
        {
            _nfoFactory = ServiceLocator.Current.GetInstance<INfoFactory>();
        }

        public string Name => "MovieScraper";

        public async Task<SearchResultEntity[]> SearchByTitleAsync(string title, string year, string language, bool includeAdult)
        {
            List<SearchResultEntity> results = new List<SearchResultEntity>();

            string url = $"http://my.scraper.url/search?title={title}&year{year}&language={language}&includeAdult={includeAdult}";

            string content = await GetPageContent(url);

            // TODO : parse your content to create an array of SearchResultEntity

            // Add each of them into the results
            results.Add(new SearchResultEntity
            {
                Id = "MovieId",
                Poster = "http://my.scraper.url/MovieId/Poster.png",
                Title = "Title",
                Year = "2000",
                ItemType = ItemType.Movie,
                Genres = { "Horror", "Comedy" },
            });

            // return the result
            return results.ToArray();
        }

        public async Task<IMovie> GetMovieDetailsAsync(string id, string language)
        {
            IMovie details = _nfoFactory.CreateMovie();

            string url = $"http://my.scraper.url/search?id={id}&language={language}";

            string content = await GetPageContent(url);

            // TODO : parse your content to create to fill the details

            return details;
        }

        public async Task<SearchResultEntity[]> SearchByIdAsync(string id, string language)
        {
            List<SearchResultEntity> result = new List<SearchResultEntity>();
            IMovie detail = await GetMovieDetailsAsync(id, language);

            if (detail != null)
            {
                result.Add(new SearchResultEntity
                {
                    Id = detail.Id,
                    ItemType = ItemType.Movie,
                    Title = detail.Title,
                    Year = detail.Year
                });
            }

            return result.ToArray();
        }


        private async Task<string> GetPageContent(string url)
        {
            IWebRequestHelper webRequestHelper = ServiceLocator.Current.GetInstance<IWebRequestHelper>();
            IWebCache webCache = ServiceLocator.Current.GetInstance<IWebCache>();

            ICacheItem cacheItem = await webCache.Get(url, TimeSpan.FromDays(CacheDurationDays), k =>
            {
                return webRequestHelper.GetDataAsync(url, null, null, 30000, null, new Dictionary<HttpRequestHeader, string>
                                                 {
                                                     { HttpRequestHeader.Accept, "application/json"}
                                                 });
            });

            if (cacheItem == null)
                return null;

            if (cacheItem.Data == null)
                return null;

            return cacheItem.GetUTF8();
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Zappiti.Api.Common;
using Zappiti.Common;
using Zappiti.Common.Cache;
using Zappiti.Common.Core;
using Zappiti.Common.Scrap;
using Zappiti.Common.Scrap.Scraper;
using Zappiti.Framework;

namespace Zappiti.Plugin.TVShowScraperExample
{
    public class TVShowScraperExample : ITvShowScraper
    {
        private readonly INfoFactory _nfoFactory;
        private const int CacheDurationDays = 7;

        public TVShowScraperExample()
        {
            _nfoFactory = ServiceLocator.Current.GetInstance<INfoFactory>();
        }


        public string Name => "TVShowScraper";

        public async Task<SearchResultEntity[]> SearchByTitleAsync(string title, string year, string language, bool getPicture)
        {
            List<SearchResultEntity> results = new List<SearchResultEntity>();

            string url = $"http://my.scraper.url/search?title={title}&year{year}&language={language}";

            string content = await GetPageContent(url);

            // TODO : parse your content to create an array of SearchResultEntity

            string poster = null;
            if (getPicture)
            {
                // optional since it may be time consuming to get the picture url is not required
                poster = "http://my.scraper.url/MovieId/Poster.png";
            }

            // Add each of them into the results
            results.Add(new SearchResultEntity
            {
                Id = "MovieId",
                Poster = poster,
                Title = "Title",
                Year = "2000",
                ItemType = ItemType.Movie,
                Genres = { "Horror", "Comedy" },
            });

            // return the result
            return results.ToArray();
        }

        public async Task<ITVShow> GetTvShowDetailsAsync(string id, string language)
        {
            ITVShow details = _nfoFactory.CreateTVShow();

            string url = $"http://my.scraper.url/search?id={id}&language={language}";

            string content = await GetPageContent(url);

            // TODO : parse your content to create to fill the details

            return details;
        }

        public async Task<SearchResultEntity[]> SearchByIdAsync(string id, string language, bool getPicture)
        {
            List<SearchResultEntity> result = new List<SearchResultEntity>();
            ITVShow detail = await GetTvShowDetailsAsync(id, language);

            if (detail != null)
            {
                result.Add(new SearchResultEntity
                {
                    Id = detail.Id,
                    ItemType = ItemType.TVShow,
                    Title = detail.Title,
                    Year = detail.Premiered,
                    
                });
            }

            return result.ToArray();
        }

        public async Task<ITvEpisode[]> GetTVShowEpisodeDetailsIn(string id, string language, params SeasonEpisodeNumber[] seasonEpisodeNumbers)
        {
            if (seasonEpisodeNumbers == null)
                return new ITvEpisode[] { };

            List<ITvEpisode> episodes = new List<ITvEpisode>();


            string url = $"http://my.scraper.url/searchEpisodes?id={id}&language={language}";

            string content = await GetPageContent(url);

            ITvEpisode episodeDetails = _nfoFactory.CreateEpisode();

            // TODO : parse your content to create to fill the episodeDetails
            episodes.Add(episodeDetails);


            // return all episodes
            return episodes.ToArray();
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

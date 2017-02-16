using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper.Configuration;
using HtmlAgilityPack;
using Kendo.Mvc.Extensions;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;

namespace MovieConnections.Web.Services
{
    public class CrawlingService {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly IGenreService _genreService;

        public CrawlingService(
            IActorService actorService, 
            IMovieService movieService, 
            IGenreService genreService) {
            _actorService = actorService;
            _movieService = movieService;
            _genreService = genreService;
        }

        public void StoreAllMovieInGenres(int page = 1, string hrefInit = null)
        {
            string baseUrl = "http://www.imdb.com";
            HtmlWeb htmlWeb;

            page =  page == 1 ? 1 : page;

            while (page > 0) {
                var allHrefs = GetAllUrlsInSpecifiedGenre(baseUrl, out htmlWeb, page);

                if(allHrefs == null) {
                    page = 0;
                    break;
                }

                var allHrefList = allHrefs.ToList();

                if (allHrefList.Contains(hrefInit))
                {
                    var index = allHrefList.ToList().FindIndex(x => x == hrefInit);

                    if (index >= 0)
                    {
                        allHrefList.RemoveRange(0, index);
                    }
                }

                foreach (var href in allHrefList)
                {
                    var movieParams = new MovieParams();
                    var detailDocument = htmlWeb.Load(href);

                    //< span itemprop = "actors" itemtype = "http://schema.org/Person" itemscope = "" >
                    //    < a href = "/name/nm1785339?ref_=tt_ov_st_sm" itemprop = "url" >
                    //        < span class="itemprop" itemprop="name">Rami Malek</span>
                    //    </a>
                    //</span>

                    HtmlNode documentNode = detailDocument.DocumentNode;
                    var title = GetMovieTitle(documentNode);
                    movieParams.Title = title;

                    var date = GetMovieDate(documentNode);
                    movieParams.Year = date;

                    var originalTitle = GetMovieOriginalTitle(documentNode);
                    movieParams.OriginalTitle = originalTitle;

                    movieParams.Status = Status.Active;
                    movieParams.UserType = UserTypes.Dashboard;


                    var creditNodes = documentNode.SelectNodes(".//div[@class='credit_summary_item']");
                    var stars = new List<string>();

                    if(creditNodes != null)
                        stars = creditNodes.SelectMany(x => x.Descendants("span")
                        .Where(y => y.GetAttributeValue("class", "") == "itemprop" && y.GetAttributeValue("itemprop", "") == "name")
                        .Select(z => z.InnerText)).ToList();


                    string rating;
                    var ratingNode = documentNode.SelectSingleNode(".//span[@itemprop='ratingValue']");

                    if (ratingNode == null)
                        continue;

                    rating = ratingNode.InnerText;

                    movieParams.Rating = Double.Parse(rating.Replace(".", ","));

                    var ratingCount = documentNode.SelectSingleNode(".//span[@itemprop='ratingCount']")?.InnerText;
                    movieParams.NumberOfVotes = Int32.Parse(ratingCount?.Replace(",", ""));

                    var country = GetCountry(documentNode);
                    movieParams.Country = country;

                    var titleWrapper = documentNode.SelectSingleNode("//div[@class='title_wrapper']");

                    var typeAnchor = titleWrapper.SelectSingleNode("div/a[last()]");

                    if (typeAnchor != null)
                        movieParams.MovieType = typeAnchor.InnerText
                            .Contains(MovieType.MiniSeries.GetEnumDescription<DisplayAttribute>().Name) ?
                            MovieType.MiniSeries : typeAnchor.InnerText
                            .Contains(MovieType.Series.GetEnumDescription<DisplayAttribute>().Name) ?
                            MovieType.Series : typeAnchor.InnerText
                            .Contains(MovieType.TvMovie.GetEnumDescription<DisplayAttribute>().Name) ? MovieType.TvMovie : MovieType.Movie;

                    var genreList = GetMovieCategories(documentNode)?.Select(x => x.Name).ToList();

                    var crewHref = baseUrl + documentNode.SelectSingleNode("//a[@class='quicklink']").GetAttributeValue("href", "404");

                    var crewNameCharacterPairs = GetCrewNamesOfMovie(crewHref, htmlWeb, stars);
                    var directors = GetDirectors(crewHref, htmlWeb);

                    movieParams.Director = directors;
                    _movieService.CreateOrUpdate(movieParams);

                    if(genreList != null)
                        _genreService.CreateGenres(genreList, movieParams.BaseEntityId);

                    if(crewNameCharacterPairs != null)
                        _actorService.CreateActors(crewNameCharacterPairs, movieParams.BaseEntityId);

                }
                page = page + 1;
            }
        }
        private string GetDirectors(string crewHref, HtmlWeb htmlWeb) {
            var crewDocument = htmlWeb.Load(crewHref);

            var directorName = "undefined";
            var directorH4Node = crewDocument.DocumentNode
                .SelectSingleNode("//h4[text()='Directed by&nbsp;' or text()='Series Directed by&nbsp;']");

            if (directorH4Node != null) {
                var directors = directorH4Node.ParentNode.SelectNodes("table[1]/tbody/tr[1]/td[@class='name']/a").Select(x => x.InnerText).ToList();
                directorName = string.Join(",", directors);
            }
            return directorName;
        }

        private List<ActorModel> GetCrewNamesOfMovie(string crewHref, HtmlWeb htmlWeb, List<string> stars) {
       
            var actorCharacterDictionary = new Dictionary<string, string>();
            var crewDocument = htmlWeb.Load(crewHref);
            
            var crewList = crewDocument.DocumentNode.SelectNodes("//table[@class='cast_list']");

            if(crewList == null || !crewList.Any())
                return null;
            
            var crewNode = crewList[0].SelectNodes("//tr//td[@class='itemprop']//a//span[@itemprop]");
            if (crewNode != null)
            {
                foreach (var crew in crewNode.ToList())
                {
                    if (actorCharacterDictionary.ContainsKey(crew.InnerHtml)) {

                        try
                        {
                            actorCharacterDictionary[crew.InnerHtml] = actorCharacterDictionary[crew.InnerHtml] + ", " +
                               crewList[0].SelectSingleNode("//tr//td[@class='itemprop']//a//" +
                                                    "span[@itemprop and text() = \"" + crew.InnerHtml + "\"]//..//..//..//td[@class='character']//div")
                                                    ?.InnerText;
                        }
                        catch (Exception)
                        {

                            actorCharacterDictionary.Add(crew.InnerHtml, "");
                        }

                    } else {
                        try
                        {
                            actorCharacterDictionary.Add(crew.InnerHtml, crewList[0].SelectSingleNode("//tr//td[@class='itemprop']//a//" +
                                    "span[@itemprop and text() = \"" + crew.InnerHtml + "\"]//..//..//..//td[@class='character']//div")
                                    ?.InnerText);
                        }
                        catch (Exception)
                        {

                            actorCharacterDictionary.Add(crew.InnerHtml, "");
                        }

                    }
                }
            }
                
            var crewNameCharacterPair = new List<ActorModel>();

            for (int i = 0; i < actorCharacterDictionary.Count(); i++) {
                crewNameCharacterPair.Add(new ActorModel
                {
                    CharacterName = actorCharacterDictionary.Values.ElementAt(i)?.Split('(')[0]?.Split('/')[0].Replace("&nbsp;", "").Trim(),
                    Name = actorCharacterDictionary.Keys.ElementAt(i),
                    IsStar = stars.Contains(actorCharacterDictionary.Keys.ElementAt(i))
                });
            }

            return crewNameCharacterPair;
        }

        private string GetMovieOriginalTitle(HtmlNode titleWrapper) {
            var originalTitleNode = titleWrapper.SelectSingleNode(".//div[@class='originalTitle']");
            var originalTitle = originalTitleNode?.InnerText;
            return originalTitle;
        }
        
        private IEnumerable<GenreCulture> GetMovieCategories(HtmlNode titleWrapper) {
            var genreNode = titleWrapper
                .SelectNodes("//span[@class='itemprop' and @itemprop='genre']");

            var genreList = genreNode?.Select(x => new GenreCulture { Name = x.InnerText });
            return genreList;
        }

        private string GetMovieTitle(HtmlNode titleWrapper) {
            HtmlNode htmlNode = titleWrapper.SelectSingleNode("//div[@class='title_wrapper']");

            var titleNode = htmlNode?.SelectSingleNode("//h1[@itemprop='name']");

            if (titleNode == null)
                return string.Empty;

            var titleAndYear = titleNode.InnerText?.TrimEnd() ?? "";
            var titleAndYearArray = Regex.Split(titleAndYear, "&nbsp;");

            var title = titleAndYearArray[0];
            return title;
        }
        
        private DateTime? GetMovieDate(HtmlNode titleWrapper)
        {
            var releaseDateNode = titleWrapper.SelectSingleNode("//h4[. = 'Release Date:']")?.ParentNode;
            if (releaseDateNode == null)
                return null;

            var releaseDate = releaseDateNode.InnerText?.RemoveWordFromString("Release Date:");

            if (string.IsNullOrEmpty(releaseDate))
                return null;

            var releaseDateParts = releaseDate.Split('(');
            string releaseDateText = string.Empty;
            if(releaseDateParts.Any())
                releaseDateText = releaseDateParts.FirstOrDefault()?.Trim();

            DateTime? date = null;
            DateTime dateValue = DateTime.MinValue;

            if (!string.IsNullOrEmpty(releaseDateText))
            {
                if(!DateTime.TryParse(releaseDateText, out dateValue)) {
                    date = new DateTime(int.Parse(releaseDateText), 1, 1);
                }
            } else {
                return DateTime.Parse(releaseDateText);
            }

            return date;
        }

        private string GetCountry(HtmlNode titleWrapper) {
            var countryNode = titleWrapper.SelectSingleNode("//h4[. = 'Country:']")?.ParentNode;
            if (countryNode == null)
                return string.Empty;

            var countryList = countryNode.ChildNodes?.Where(x => x.Name == "a").Select(x => x.InnerText).ToList();

            if (countryList != null && countryList.Any())
            {
                var countryText = string.Join(", ", countryList);

                return countryText;
            }

            return null;
        }

        private IEnumerable<string> GetAllUrlsInSpecifiedGenre( string baseUrl, out HtmlWeb htmlWeb, int page = 1) {

            var listViewUrl = "http://www.imdb.com/search/title?" +
                              "count=100&num_votes=1000,&sort=user_rating,desc&" +
                              "title_type=feature,tv_movie,tv_series,mini_series,short&user_rating=4.8,10&" +
                              "page=" + page 
                              + "&ref_=adv_nxt";

            htmlWeb = new HtmlWeb();
            var listDocument = htmlWeb.Load(listViewUrl);

            if (listDocument == null)
                return null;

            var allRelatedAnchors = listDocument.DocumentNode.SelectNodes("//a").Where(x => x.ParentNode.Attributes.Contains("class") &&
                            x.ParentNode.Attributes["class"].Value.Contains("lister-item-header")).ToArray();

            var allHrefs = allRelatedAnchors.Select(x => baseUrl + x.GetAttributeValue("href", ""));
            return allHrefs;
        }
    }
}
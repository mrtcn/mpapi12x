using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Models;
using Category = MovieConnections.Crawler.Models.Category;

namespace MovieConnections.Crawler.Services
{
    public class CrawlingService {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly ICategoryService _categoryService;

        public CrawlingService(
            IActorService actorService, 
            IMovieService movieService, 
            ICategoryService categoryService) {
            _actorService = actorService;
            _movieService = movieService;
            _categoryService = categoryService;
        }

        public void StoreAllMovieInGenres()
        {
            string baseUrl = "http://www.imdb.com";
            HtmlWeb htmlWeb;
            MovieParams movieParams;
            var allHrefs = GetAllUrlsInSpecifiedGenre(baseUrl, out htmlWeb, out movieParams);

            foreach (var href in allHrefs)
            {
                var detailDocument = htmlWeb.Load(href);

                //< span itemprop = "actors" itemtype = "http://schema.org/Person" itemscope = "" >
                //    < a href = "/name/nm1785339?ref_=tt_ov_st_sm" itemprop = "url" >
                //        < span class="itemprop" itemprop="name">Rami Malek</span>
                //    </a>
                //</span>

                HtmlNode titleWrapper;
                var title = GetMovieTitle(out titleWrapper, detailDocument);
                movieParams.Title = title;

                var date = GetMovieDate(titleWrapper);
                movieParams.Year = date;

                var originalTitle = GetMovieOriginalTitle(titleWrapper);
                movieParams.OriginalTitle = originalTitle;

                movieParams.Status = Status.Active;
                movieParams.UserType = UserTypes.Dashboard;

                _movieService.CreateOrUpdate(movieParams);

                var categoryList = GetMovieCategories(titleWrapper).Select(x => x.Title).ToList();
                //movieParams.Categories = categoryList.ToList();
                _categoryService.CreateCategories(categoryList, movieParams.Id);

                var crewNameCharacterPairs = GetCrewNamesOfMovie(baseUrl, detailDocument, htmlWeb);
                
                _actorService.CreateActors(crewNameCharacterPairs, movieParams.Id);
                //var name = "";
                //var characterName = "";

                //var characterNameNode = crewName.SelectSingleNode(".//td[@class='character']//div//a");
                //characterNameNode = characterNameNode ?? crewName.SelectSingleNode(".//td[@class='character']//div");
                //if (characterNameNode != null)
                //    //characterName = Regex.Replace(characterNameNode.InnerText, @"\s+", "");
                //    characterName = characterNameNode.InnerText.Trim();

                //var nameNode = crewName.SelectSingleNode(".//span[@itemprop='name' and @class='itemprop']");
                //if (nameNode != null)
                //    //name = Regex.Replace(nameNode.InnerText, @"\s+", "");
                //    name = nameNode.InnerText.Trim();

                //if (!string.IsNullOrEmpty(characterName) && !string.IsNullOrEmpty(name))
                //    movieParams.Actors.Add(new Actor { CharacterName = characterName, Name = name });

            }
        }

        private List<ActorModel> GetCrewNamesOfMovie(string baseUrl, HtmlDocument detailDocument, HtmlWeb htmlWeb) {
            //fullcredits? ref_ = tt_ov_st_sm
            //var actors = detailDocument.DocumentNode.SelectNodes("//span[@itemprop='actors']//a//span").Select(x => new Actor { Name = x.InnerText });
            //movieParams.Actors = actors.ToList();

            var crewHref = baseUrl + detailDocument.DocumentNode.SelectSingleNode("//a[@class='quicklink']").GetAttributeValue("href", "404");
            var crewDocument = htmlWeb.Load(crewHref);

            var crewList = crewDocument.DocumentNode.SelectNodes("//table[@class='cast_list']");

            //var trNodes = crewList[0].SelectNodes("//tr");

            var characterNames = crewList[0].SelectNodes("//td[@class='character']//div//a").Select(x => x.InnerText).ToList();
            var actorNames = crewList[0].SelectNodes("//td[@class='itemprop']//a//span[@itemprop]").Select(x => x.InnerText).ToList();

            var crewNameCharacterPair = new List<ActorModel>();

            for (int i = 0; i < characterNames.Count(); i++) {
                crewNameCharacterPair.Add(new ActorModel {CharacterName = characterNames[i], Name = actorNames[i]});
            }
            //var crewNames = crewList[0].ChildNodes.Select(x => new Actor
            //{
            //    Name = x.Element("span").InnerText,
            //    CharacterName = x.Element("a").InnerText
            //});

            //var crewNames = crewList[0].ChildNodes;
            return crewNameCharacterPair;
        }

        private string GetMovieOriginalTitle(HtmlNode titleWrapper) {
            var originalTitleNode = titleWrapper.SelectSingleNode(".//div[@class='originalTitle']");
            var originalTitle = originalTitleNode?.InnerText;
            return originalTitle;
        }

        private DateTime GetMovieDate(HtmlNode titleWrapper) {
            var year = titleWrapper.SelectSingleNode("//meta[@itemprop='datePublished']").GetAttributeValue("content", "Undefined");
            var date = DateTime.MinValue;

            if (!string.IsNullOrEmpty(year)) {
                date = Convert.ToDateTime(year);
            }
            return date;
        }

        private IEnumerable<Category> GetMovieCategories(HtmlNode titleWrapper) {
            var categoryList = titleWrapper.SelectNodes("//span[@class='itemprop' and @itemprop='genre']").Select(x => new Category { Title = x.InnerText });
            return categoryList;
        }

        private string GetMovieTitle(out HtmlNode titleWrapper, HtmlDocument detailDocument) {
            titleWrapper = detailDocument.DocumentNode.SelectSingleNode("//div[@class='title_wrapper']");

            var titleNode = titleWrapper.SelectSingleNode("//h1[@itemprop='name']");
            var titleAndYear = titleNode.InnerText.TrimEnd();
            var titleAndYearArray = Regex.Split(titleAndYear, "&nbsp;");

            var title = titleAndYearArray[0];
            return title;
        }

        private IEnumerable<string> GetAllUrlsInSpecifiedGenre( string baseUrl, out HtmlWeb htmlWeb, 
            out MovieParams movieParams, string view = "simple", string sort = "moviemeter.asc", 
            string genres = "Crime", string page = "1") {
            
            //var index = 1;
            //while (true)
            //{

            var listViewUrl =
                "http://www.imdb.com/search/title?explore=genres&view=" + view + "&sort=" + sort + "&genres=" + genres + "&page=" +
                page + "&ref_=adv_nxt";

            htmlWeb = new HtmlWeb();
            var listDocument = htmlWeb.Load(listViewUrl);

            movieParams = new MovieParams();

            var allRelatedAnchors = listDocument.DocumentNode.SelectNodes("//a")
                .Where(x => x.ParentNode.ParentNode.Attributes.Contains("class") &&
                            x.ParentNode.ParentNode.Attributes["class"].Value.Contains("lister-item-header")).ToArray();

            var allHrefs = allRelatedAnchors.Select(x => baseUrl + x.GetAttributeValue("href", ""));
            return allHrefs;
        }
    }
}
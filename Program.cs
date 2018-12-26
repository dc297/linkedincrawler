using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

namespace LinkedInCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            InputDto Input = GetInput();

            ChromeOptions ChromeOptions = new ChromeOptions();
            ChromeOptions.AddArgument("headless");
            ChromeOptions.AddArgument("--log-level=3");
            ChromeOptions.AddArgument("--silent");

            IWebDriver Driver = new ChromeDriver(ChromeOptions);

            Crawler Crawler = new Crawler(Input, Driver);

            Crawler.CrawlToHome();
            Crawler.Login();
            Crawler.CrawlToSearchPage();
            if(Input.AdvancedSearch) {
                Input.AdvancedSearchParams = Crawler.GetAdvancedSearchOptions();
                Input.AdvancedSearchParamsStr = GetAdvancedSearchParamsFromUser(Input.AdvancedSearchParams);
            }
            else Console.WriteLine("Skipping advanced search");
            Crawler.CrawlToSearchPage();
            Crawler.SendConnectionRequests();
            Driver.Close();
        }

        static InputDto GetInput(){
            InputDto Input = new InputDto();

            Console.WriteLine("Please enter your username");
            Input.Username = Util.GetInputFromConsole(true);

            Console.WriteLine("Please enter your password");
            Input.Password = Util.GetInputFromConsole(true);

            Console.WriteLine("Please enter a search query");
            Input.SearchQuery = Util.GetInputFromConsole(false);

            Console.WriteLine("Please add a note to be included with the connection request");
            Input.CustomNote = Util.GetInputFromConsole(false);

            Console.WriteLine("Do you wish to do advanced search? Y/N(N)");
            string AdvancedSearch = Util.GetInputFromConsole(false);

            if("Y".Equals(AdvancedSearch)) Input.AdvancedSearch = true;
            else Input.AdvancedSearch = false;

            return Input;
        }

        static string GetAdvancedSearchParamsFromUser(List<AdvancedSearchDTO> AdvancedSearchParams){
            string SearchParams = "";
            foreach(AdvancedSearchDTO AdvancedSearchParam in AdvancedSearchParams){
                Console.WriteLine(AdvancedSearchParam.Title);
                foreach(AdvancedSearchOption AdvancedSearchOption in AdvancedSearchParam.Options){
                    Console.WriteLine(AdvancedSearchOption.DisplayValue + " -> " + AdvancedSearchOption.Value);
                }
                Console.WriteLine("Please enter comma separated values. Enter only the values after `->`. Leave empty to skip");
                string SearchParam = Util.GetInputFromConsole(false);
                SearchParam = SearchParam.Replace(",","\"%2C\"");
                if(!"".Equals(SearchParam)) SearchParams += AdvancedSearchParam.Key.Trim() + "=%5B\"" + SearchParam + "\"%5D&";
                Console.WriteLine();
            }
            Console.WriteLine(SearchParams);
            return SearchParams;
        }
    }
}

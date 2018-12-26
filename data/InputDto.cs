using System.Collections.Generic;
namespace LinkedInCrawler{

    public class InputDto{

        public string Username;
        public string Password;
        public string SearchQuery;

        public string CustomNote;

        public List<AdvancedSearchDTO> AdvancedSearchParams;

        public string AdvancedSearchParamsStr;

        public bool AdvancedSearch;
    }
}
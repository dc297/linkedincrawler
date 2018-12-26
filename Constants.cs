using System;

namespace LinkedInCrawler{
    public class Constants{

        public static String SUCCESS_URL = "https://www.linkedin.com/feed/";

        public static String START_URL = "https://linkedin.com";

        public static String SEARCH_URL = "https://www.linkedin.com/search/results/people/?keywords=";

        public static String LOGIN_ELEMENT = "session_key";

        public static String PASSWORD_ELEMENT = "session_password";

        public static String CONNECT_BUTTON_ELEMENT = "search-result__actions--primary";

        public static String CONNECT_BUTTON_TEXT = "Connect";

        public static String LOGIN_SUBMIT_ELEMENT = "login-submit";

        public static String NEXT_BUTTON_ELEMENT = "next";

        public static String CANCEL_BUTTON_ELEMENT = "cancel";

        public static Int32 HEADER_HEIGHT = 60;

    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace LinkedInCrawler{
    public class Crawler{

        private IWebDriver Driver;
        private InputDto Input;
        public Crawler(InputDto Input, IWebDriver Driver){
            this.Input = Input;
            this.Driver = Driver;
        }

        public void CrawlToHome(){
            Console.WriteLine("Navigating to " + Constants.START_URL);
            Driver.Navigate().GoToUrl(Constants.START_URL);
        }

        public void Login(){
            IWebElement UsernameElement = Driver.FindElement(By.Name(Constants.LOGIN_ELEMENT));
            IWebElement PasswordElement = Driver.FindElement(By.Name(Constants.PASSWORD_ELEMENT));

            UsernameElement.SendKeys(Input.Username);
            PasswordElement.SendKeys(Input.Password);

            IWebElement SubmitButton = Driver.FindElement(By.Id(Constants.LOGIN_SUBMIT_ELEMENT));
            Console.WriteLine("Submitting login form");
            SubmitButton.Click();
        }

        public List<AdvancedSearchDTO> GetAdvancedSearchOptions(){
            IWebElement AllFiltersButton = Driver.FindElement(By.ClassName("search-filters-bar__all-filters"));
            if(AllFiltersButton!=null) {
                AllFiltersButton.Click();
            }
            else {
                Console.WriteLine("Some error occured while getting params for advanced search");
                return null;
            }

            ReadOnlyCollection<IWebElement> AdvancedSearchElementList = Driver.FindElements(By.ClassName("search-s-facet"));
            List<AdvancedSearchDTO> AdvancedSearchItems = new List<AdvancedSearchDTO>();

            foreach(IWebElement AdvancedSearchElement in AdvancedSearchElementList){
                string ItemClass = AdvancedSearchElement.GetAttribute("class");
                if(ItemClass.Contains("flex-shrink-zero")) continue;
                AdvancedSearchDTO AdvancedSearchItem = new AdvancedSearchDTO();
                
                IWebElement TitleElement = AdvancedSearchElement.FindElement(By.ClassName("search-s-facet__name"));
                if(TitleElement!=null){
                    AdvancedSearchItem.Title = TitleElement.Text.Trim();
                }

                if(!"".Equals(ItemClass)){
                    int StartIndex = ItemClass.IndexOf("--") + 2;
                    int Length = ItemClass.IndexOf("inline-block") - StartIndex;
                    String ItemKey = ItemClass.Substring(StartIndex, Length);
                    
                    AdvancedSearchItem.Key = "facet" + Char.ToUpper(ItemKey[0]) + ItemKey.Substring(1);
                }

                ReadOnlyCollection<IWebElement> AdvancedSearchOptionElements = AdvancedSearchElement.FindElements(By.TagName("li"));
                List<AdvancedSearchOption> AdvancedSearchOptions = new List<AdvancedSearchOption>();

                foreach(IWebElement AdvancedSearchOptionElement in AdvancedSearchOptionElements){
                    AdvancedSearchOption AdvancedSearchOption = new AdvancedSearchOption();
                    IWebElement OptionInputElement = AdvancedSearchOptionElement.FindElement(By.TagName("input"));
                    IWebElement OptionLabelElement = AdvancedSearchOptionElement.FindElement(By.TagName("label"));
                    if(OptionInputElement!=null && OptionLabelElement!=null && "checkbox".Equals(OptionInputElement.GetAttribute("type"))){
                        AdvancedSearchOption.Value = OptionInputElement.GetAttribute("value");
                        AdvancedSearchOption.DisplayValue = OptionLabelElement.Text.Trim();
                        AdvancedSearchOptions.Add(AdvancedSearchOption);
                    }
                }

                if(AdvancedSearchOptions.Count!=0) {
                    AdvancedSearchItem.Options = AdvancedSearchOptions;
                    AdvancedSearchItems.Add(AdvancedSearchItem);
                }
            }

            return AdvancedSearchItems;
        }

        public void CrawlToSearchPage(){
            if(!Constants.SUCCESS_URL.Equals(Driver.Url) && !Driver.Url.Contains(Constants.SEARCH_URL)) {
                Console.WriteLine("Login Failed!");
                return;
            }
            Console.WriteLine("Navigating to Search page");
            string Url = Constants.SEARCH_URL + Input.SearchQuery;
            if(Input.AdvancedSearchParamsStr!=null && !"".Equals(Input.AdvancedSearchParamsStr)) Url += "&" + Input.AdvancedSearchParamsStr;
            Driver.Navigate().GoToUrl(Url);
        }

        public void SendConnectionRequests(){
            bool NextPageExists = true;
            WaitUtil.CheckPendingRequests(Driver);
            
            while(NextPageExists){
                WaitUtil.CheckPendingRequests(Driver);
                WaitUtil.CheckDocumentReady(Driver);
                WaitUtil.ScrollToBottomSlow(Driver);
                WaitUtil.CheckOcclusion(Driver);
                ReadOnlyCollection<IWebElement> ConnectButtons = Driver.FindElements(By.ClassName(Constants.CONNECT_BUTTON_ELEMENT));
                foreach(IWebElement ConnectButton in ConnectButtons){
                    if(Constants.CONNECT_BUTTON_TEXT.Equals(ConnectButton.Text)){
                        Console.WriteLine("Sending connection request to " + ConnectButton.GetAttribute("aria-label"));
                        WaitUtil.ScrollToLocation(Driver, ConnectButton.Location.Y - Constants.HEADER_HEIGHT);
                        ConnectButton.Click();

                        if(Input.CustomNote!=null && !"".Equals(Input.CustomNote)){
                            IWebElement NoteButton = Driver.FindElement(By.ClassName("button-secondary-large"));
                            NoteButton.Click();
                            IWebElement NoteTextArea = Driver.FindElement(By.ClassName("send-invite__custom-message"));
                            NoteTextArea.SendKeys(Input.CustomNote);
                        }
                        //System.Threading.Thread.Sleep(500);
                        IWebElement ModalCloseButton = Driver.FindElement(By.Name(Constants.CANCEL_BUTTON_ELEMENT));
                        if(ModalCloseButton!=null){
                            ModalCloseButton.Click();
                        }
                    }
                }
                try{
                    IWebElement NextButton = Driver.FindElement(By.ClassName(Constants.NEXT_BUTTON_ELEMENT));
                    WaitUtil.ScrollToLocation(Driver, NextButton.Location.Y - Constants.HEADER_HEIGHT);
                    NextButton.Click();
                }
                catch(NoSuchElementException e){
                    NextPageExists = false;
                }
            }
        }
    }
}
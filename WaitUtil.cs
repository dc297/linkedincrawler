using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace LinkedInCrawler{

    public class WaitUtil {

        /**
        This method checks if the page is ready or not.
        If the browser has finished executing the JavaScript or not 
        and waits till the Browser has finished executing JavaScript.
         */
        public static void CheckDocumentReady(IWebDriver Driver) {
            int timeoutInNumberOfTries = 50;
            IJavaScriptExecutor  JsDriver = (IJavaScriptExecutor )Driver;
            for (int i = 0; i < timeoutInNumberOfTries; i++) {
                System.Threading.Thread.Sleep(100);
                Object NumberOfAjaxConnections = JsDriver.ExecuteScript("return document.readyState");
                
                if ((string)NumberOfAjaxConnections == "complete") {
                    break;
                }
            }	 
        }

        /**This method waits until all the AJAX requests are finished.
         */
        public static void CheckPendingRequests(IWebDriver Driver) {
            int timeoutInNumberOfTries = 50;
            IJavaScriptExecutor  JsDriver = (IJavaScriptExecutor )Driver;
            for (int i = 0; i < timeoutInNumberOfTries; i++) {
                System.Threading.Thread.Sleep(100);
                Object NumberOfAjaxConnections = JsDriver.ExecuteScript("return window.openHTTPs");
                // return should be a number
                if(NumberOfAjaxConnections!=null){
                    //Console.WriteLine(" " + (Int64)NumberOfAjaxConnections);
                    if ((Int64)NumberOfAjaxConnections == 0) {
                        break;
                    }
                }
                else{
                    MonkeyPatchXMLHttpRequest(Driver);
                }
            }	 
        }

        /**
        To load the occluded items, we need to scroll to the end of the page slowly.
        This method does that.
         */
        public static void ScrollToBottomSlow(IWebDriver Driver){
            IJavaScriptExecutor  JsDriver = (IJavaScriptExecutor )Driver;
            if(JsDriver!=null){
                for(double x = 0; x<=1; x = x+0.05){
                JsDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight*"+x+")");
                }
            }
        }

        /**
        Helper method to scroll to a particular location.
        Uses Selenium's JSExecutor to do  that.
         */
        public static void ScrollToLocation(IWebDriver Driver, int y){
            IJavaScriptExecutor  JsDriver = (IJavaScriptExecutor )Driver;
            if(JsDriver!=null){
                JsDriver.ExecuteScript("window.scrollTo(0, "+y+")");
            }
        }

        /**
        Injecting our own javascript to keep track of AJAX requests.
         */
        private static void MonkeyPatchXMLHttpRequest(IWebDriver Driver) {
            
            IJavaScriptExecutor JsDriver = (IJavaScriptExecutor)Driver;
            string script = "  (function() {" + "var oldOpen = XMLHttpRequest.prototype.open;" + "window.openHTTPs = 0;" +
                    "XMLHttpRequest.prototype.open = function(method, url, async, user, pass) {" + "window.openHTTPs++;" +
                    "this.addEventListener('readystatechange', function() {" + "if(this.readyState == 4) {" + "window.openHTTPs--;" + "}" +
                    "}, false);" + "oldOpen.call(this, method, url, async, user, pass);" + "}" + "})();";
            JsDriver.ExecuteScript(script);
        }

        /**
        The search page on LinkedIn contains some occluded items. 
        We need to wait for those items to Load. This method does just that.
         */
        public static void CheckOcclusion(IWebDriver Driver){
            if(Driver!=null){
                while(true){
                    System.Threading.Thread.Sleep(100);
                    ReadOnlyCollection<IWebElement> OccludedItems = Driver.FindElements(By.ClassName("search-result__occlusion-hint"));
                    if(OccludedItems==null || OccludedItems.Count==0) break;
                }
            }
        }
    
    }

}
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Linq;
using System.Threading;

namespace AutoTikTok
{
    internal class TIkTokApi
    {
        IWebDriver Browser;
        Settings settings;
        bool hideBrowser;
        public TIkTokApi(Settings settings, bool hideBrowser = false)
        {
            this.settings = settings;
            settings.Write("HashTags", "#анимеприколы #аниметоп #анимемемы #аниме #мемы");
            this.hideBrowser = hideBrowser;
        }

        void OpenBrowser()
        {
            Console.WriteLine("Open Browser...");
            var driverService = FirefoxDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            var options = new FirefoxOptions();
            if(hideBrowser)
                options.AddArgument("--headless");
            /*            options.AddArgument($@"--user-data-dir=C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data\");
                        options.AddArgument("--profile-directory=Profile 4");*/
            var profileManager = new FirefoxProfileManager();
            FirefoxProfile profile = profileManager.GetProfile("Selenium");
            options.Profile = profile;
            Browser = new FirefoxDriver(driverService, options);
            Browser.Manage().Window.Maximize();
        }

        public void UploadVideo(string videoPath)
        {
            OpenBrowser();
            Browser.Navigate().GoToUrl("https://www.tiktok.com/upload?lang=ru-RU");
            Thread.Sleep(1000);
            Browser.SwitchTo().Frame(Browser.FindElement(By.XPath("//iframe[contains(@src, 'https://www.tiktok.com/creator#/upload')]")));
            Thread.Sleep(5000);
            Console.WriteLine("Upload Video...");
            Browser.FindElement(By.CssSelector("[name=\"upload-btn\"]")).SendKeys(videoPath);
            Thread.Sleep(5000);
            var textBox = Browser.FindElement(By.XPath("//div[@role=\"combobox\"]"));

            textBox.SendKeys(Keys.Control + "a" + Keys.Delete);
            Console.WriteLine("Write HashTags...");
            settings.Read("HashTags").Split(' ').ToList().ForEach(hashTag =>
            {
                textBox.SendKeys(hashTag);
                Thread.Sleep(1000);
                textBox.SendKeys(Keys.Enter);
            });
            
            Console.WriteLine("Final Stage...");
            var element = Browser.FindElement(By.XPath("//div[contains(@class, 'op-part-v2')]/button[contains(@class, 'tiktok-btn-pc-primary')]"));
            ((IJavaScriptExecutor)Browser).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            element.Click();
            //ScreenShot();
            Thread.Sleep(3000);
            Browser.Quit();
        }

        void ScreenShot()
        {
            Screenshot ss = ((ITakesScreenshot)Browser).GetScreenshot();
            ss.SaveAsFile("Image.png", ScreenshotImageFormat.Png);
        }

    }
}

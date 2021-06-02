using DTO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using OpenQA.Selenium.Remote;

namespace SiteParcer
{
    class Program
    {
        [Obsolete]
        public static IEnumerable<NewsDTO> Crawl()
        {
            string homeUrl = "http://edition.cnn.com/world";

            //options skip errors
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--ignore-certificate-errors-spki-list");
            chromeOptions.AddArgument("--ignore-ssl-errors");
            chromeOptions.AddArgument("test-type");
            chromeOptions.AddArgument("no-sandbox");
            chromeOptions.AddArgument("-incognito");
            chromeOptions.AddArgument("--start-maximized");

            IWebDriver driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);
            driver.Navigate().GoToUrl(homeUrl);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.CssSelector(".metadata-header__title")));

            //get all news
            var elements = driver.FindElements(By.CssSelector(".cd__headline a"));

            List<NewsDTO> news = elements
            .Select(el => new NewsDTO
            {
                Title = el.Text,
                Url = el.GetAttribute("href")
            }).ToList();


            for (int i = 0; i < news.Count; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));

                var n = news[i];
                try
                {
                    driver.Navigate().GoToUrl(n.Url);

                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".pg-headline")));

                    n.Id = (i+1).ToString();
                    n.Author = driver.FindElement(By.CssSelector("meta[name^=author]")).GetAttribute("content");
                    n.Description = driver.FindElement(By.CssSelector("meta[name^=description]")).GetAttribute("content");
                    n.DateOfPublication = DateTime.Parse(driver.FindElement(By.CssSelector("meta[name^=pubdate]")).GetAttribute("content"));
                    n.IsLiked = false;
                }
                catch (Exception){}

                yield return n;
            }

            driver.Close();
        }

        [Obsolete]
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();

            //creds
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";

            using (IConnection conn = factory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                model.QueueDeclare("news", false, false, false, null);

                foreach (var x in Crawl())
                {
                    Console.WriteLine(x);

                    var properties = model.CreateBasicProperties();
                    properties.Persistent = true;

                    model.BasicPublish(
                        "",
                        "news",
                        basicProperties: properties,
                        body: BinaryConverter.ObjectToByteArray(
                                x
                            )
                        );
                }
            }
        }
    }
}
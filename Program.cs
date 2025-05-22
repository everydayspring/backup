using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace rehi.poster
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var productNos = new string[]
            {
                "■상품번호■", "■상품번호■", "■상품번호■", "■상품번호■", "■상품번호■", "■상품번호■", "■상품번호■", "■상품번호■"
            };

            var options = new ChromeOptions();
            options.AddArgument("start-maximized");

            using (var driver = new ChromeDriver(options))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Navigate().GoToUrl("https://service.shopby.co.kr/login");

                wait.Until(d => d.FindElement(By.Id("username"))).SendKeys("■아이디■");
                driver.FindElement(By.Id("password")).SendKeys("■비밀번호■");
                driver.FindElement(By.Name("login_btn")).Click();
                Thread.Sleep(5000);

                foreach (var no in productNos)
                {
                    ImageUpdate(driver, no);
                    Thread.Sleep(5000);
                    Console.WriteLine(no + "완료~");
                }

                Console.WriteLine("모든 작업 완료. 아무 키나 누르면 종료됩니다.");
                Console.ReadKey();
            }
        }
        static void ImageUpdate(IWebDriver driver, string mallProductNo)
        {
            string productListUrl = $"https://service.shopby.co.kr/product/edit?globalProductNo=0&mallProductNo={mallProductNo}";
            driver.Navigate().GoToUrl(productListUrl);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            wait.Until(d =>
            {
                try
                {
                    var iframe = d.FindElement(By.ClassName("remote"));
                    d.SwitchTo().Frame(iframe);
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            IWebElement input = wait.Until(d =>
            {
                try
                {
                    var element = d.FindElement(By.CssSelector("input[placeholder='상품관리코드를 입력 해주세요.']"));
                    return element;
                }
                catch
                {
                    return null;
                }
            });

            string value = "";
            for (int i = 0; i < 10; i++)
            {
                value = input.GetAttribute("value");
                if (!string.IsNullOrEmpty(value)) break;
                Thread.Sleep(500);
            }

            string imgUrl = $"■S3 URL■";
            Console.WriteLine(imgUrl);

            var urlSwitch = wait.Until(d =>
            d.FindElement(By.XPath("//div[contains(@class,'ProductImages_labeled-input')]//label[contains(@class,'Switch_switch')]/input"))
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", urlSwitch);
            Thread.Sleep(500);

            var urlInput = wait.Until(d => d.FindElement(By.CssSelector("input.ProductImages_fill-width__lveZ4")));
            urlInput.Clear();
            urlInput.SendKeys(imgUrl);
            Thread.Sleep(500);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", urlSwitch);
            Thread.Sleep(3000);

            var listRow = wait.Until(d =>
        d.FindElement(By.XPath("//tr[th/span[text()='리스트 이미지']]"))
    );

            var listSwitch = listRow.FindElement(By.XPath(".//label[contains(@class,'Switch_switch')]/input"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", listSwitch);
            Thread.Sleep(500);

            var listInput = listRow.FindElement(By.CssSelector("input.ProductImages_fill-width__lveZ4"));
            listInput.Clear();
            listInput.SendKeys(imgUrl);
            Thread.Sleep(500);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", listSwitch);
            Thread.Sleep(3000);

            var saveButton = wait.Until(d =>
        d.FindElement(By.XPath("//button[@type='submit' and normalize-space(text())='저장']"))
    );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveButton);
            Thread.Sleep(3000);

            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                Thread.Sleep(1000);
            }
            catch (NoAlertPresentException)
            {
                Console.WriteLine("알림창 없음");
            }

            driver.SwitchTo().DefaultContent();
        }

    }
}

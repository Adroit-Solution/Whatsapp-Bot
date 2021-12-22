using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support;
using System;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Whatsapp_Try
{
    class Program
    {
        static void Main(string[] args)
        {
            //DbConnection

            string Connection_String = "Server = HEMANT-PC\\SQLEXPRESS; Database = Whatsapp; Trusted_Connection = True;";
            SqlConnection conn = new SqlConnection(Connection_String);
            conn.Open();

            if (conn.State==System.Data.ConnectionState.Open)
            {
                Send(conn);
            }
            else
            {
                Console.WriteLine("Hello"); 
            }
            conn.Close();
        }
        static void Send(SqlConnection connection)
        {
            SqlCommand Get = new SqlCommand("select * from Didi", connection);
            var data = Get.ExecuteReader();
            List<float> Number = new List<float>();

            //Send Message 
            IWebDriver chrome = new ChromeDriver(@"D:\Vishesh\Software\");
            chrome.Manage().Window.Maximize();
            chrome.Navigate().GoToUrl("https://web.whatsapp.com");
            Console.WriteLine("Press ENTER after login into Whatsapp Web and your chats are visiable.");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key.ToString() == ConsoleKey.Enter.ToString())
            {
                while (data.Read())
                {
                    
                    chrome.Navigate().GoToUrl("https://web.whatsapp.com/send?phone=91" + data["Number"] + "&text=" + $"{data["Name"]}");
                    try
                    {
                        var r = new WebDriverWait(chrome, TimeSpan.FromSeconds(30));
                        var Send = r.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("_4sWnG")));
                        //Thread.Sleep(2000);
                        Send.Click();
                        Thread.Sleep(5000);
                        r.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("_1Ae7k")));
                        Console.WriteLine("Msg Sent");

                    }
                    catch (Exception e)
                    {
                        Number.Add(Convert.ToInt64(data["Number"].ToString()));
                    }
                    finally
                    {
                        Console.WriteLine("done check");
                    }
                }

            }

            data.Close();
            foreach (double number in Number)
            {
                SqlCommand set = new SqlCommand($"Update Didi set Status = 'Message Not Sent' where Number = '{number}'", connection);

                var done = set.ExecuteNonQuery();
            }
            
            Console.WriteLine("Closing Chrome");
            chrome.Close();

        }
    }
}

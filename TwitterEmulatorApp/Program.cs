/*
        Author : Sithembiso Makhanya (smmakhanya@gmail.com)
    DateCreated: 25/1/2016
    Application: TwitterEmulator
                 This app reads two files (user.txt + tweet.txt) and output to console the users and their tweet in order.

*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

namespace TwitterEmulatorApp
{
    class Program
    {
        /// <summary>
        /// This struct will hold the user and their following and tweets
        /// </summary>
        protected internal struct Tweets
        {
            public List<string> UserHandle;
            public List<string> Following;
        }

        /// <summary>
        /// This method reads the file line at a time and extract the people the user is following
        /// </summary>
        /// <param name="UserFileReader"></param>
        /// <returns></returns>
        private static Tweets GetFollowing(StreamReader UserFileReader)
        {
            List<string> following = new List<string>();
            Tweets tweet = new Tweets();
            tweet.Following = new List<string>();
            tweet.UserHandle = new List<string>();

            using (UserFileReader)
            {
                string rawline, @handler, follwers;

                string[] Splittedfollowing;

                try
                {
                    //Read the file one line at a time.
                    while ((rawline = UserFileReader.ReadLine()) != null)
                    {
                        @handler = rawline.Substring(0, (rawline.IndexOf("follows") - 1));
                        follwers = rawline.Substring((rawline.IndexOf("follows"))).Replace(",", "").Replace("follows", "");
                        Splittedfollowing = follwers.Split(' ');

                        tweet.UserHandle.Add(@handler);
                        //Console.WriteLine(@handler);
                        foreach (string follow in Splittedfollowing)
                        {
                            if (!String.IsNullOrEmpty(follow))
                            {
                                tweet.Following.Add(follow);
                                //Console.WriteLine("\t {0}", follow);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("GetFollowing: Unexpected file layout! Correct the file and try again.");
                    Console.ReadKey();
                }
            }
            return tweet;
        }

        /// <summary>
        /// This method reads the user.txt file and extracts the users/ twitter handles
        /// </summary>
        /// <param name="UserFileReader"></param>
        /// <returns>List of handles </returns>
        private static List<string> GetHandles(StreamReader UserFileReader)
        {
            List<string> lines = new List<string>();
            try
            {
                string line;
                string[] SplittedHandles;

                //Read the file one line at a time.
                while ((line = UserFileReader.ReadLine()) != null)
                {
                    SplittedHandles = line.Replace(",", " ").Split(' ');

                    foreach (string user in SplittedHandles)
                    {
                        if ((user != "follows") && (user != ",") && !String.IsNullOrEmpty(user))
                            if (!lines.Contains(user))
                                lines.Add(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetHandles: Unexpected file layout! Correct the file and try again.");
                Console.ReadKey();
            }
            lines.Sort(); //Sort the list before returning to the caller

            return lines.ToList();
        }

        /// <summary>
        ///  This method reads the tweets.txt file and extract the tweets line by line
        /// </summary>
        /// <param name="tweetFileReader"></param>
        /// <param name="listOfTwetterHandles"></param>
        /// <param name="tweetObj"></param>
        private static void GetTweets(StreamReader tweetFileReader, List<string> listOfTwetterHandles, Tweets tweetObj)
        {
            List<string> tweetlineList = new List<string>();

            try
            {
                using (tweetFileReader)
                {
                    string tweetLine;

                    //Read the file one line at a time.
                    while ((tweetLine = tweetFileReader.ReadLine()) != null)
                    {
                        //Append the @ sign and replace > with : in the tweet
                        tweetLine = "@" + tweetLine.Replace(">", ":");
                        //Add all tweets messages to a list
                        tweetlineList.Add(tweetLine);
                    }
                }
                /*
                  Enumerate the the list of twitter handles (users) and out handle and their tweets
                  and tweets of those they follow or following them.
                */
                foreach (string tweetHandle in listOfTwetterHandles)
                {
                    Console.WriteLine("{0} \n", tweetHandle);
                    foreach (string tweets in tweetlineList)
                    {
                        if (tweets.Contains("@" + tweetHandle + ":"))
                        {
                            Console.WriteLine("\t {0}", tweets);             
                        }
                        else
                        {
                            foreach (var f in tweetObj.UserHandle)
                            {
                                if ((tweetHandle != f) && (!tweetObj.Following.Contains(tweetHandle)))
                                {
                                    if (tweets.Contains("@" + f + ":"))
                                    {
                                        Console.WriteLine("\t {0}", tweets);
                                    }
                                }
                            }
                        }               
                    }
                }
            }
            catch (Exception e)
            {
                //Handle exception in a user friendly manner
                Console.WriteLine("GetTweets: Unexpected file layout! Correct the file and try again.");

                //Console.WriteLine(e.Message); // General error logging will go here for troubleshooting purposes.
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            List<string> listOfHandles = new List<string>();
            Tweets tweets;

            //This is a drop off location for input files
            string BaseDir = ConfigurationManager.AppSettings["FileInputDir"];

            if (Directory.Exists(BaseDir))
            {
                try
                {
                    //Read user file
                    using (StreamReader tweetHandleReader = new StreamReader(BaseDir + "user.txt"))
                    {
                        listOfHandles = GetHandles(tweetHandleReader);
                    }

                    //Read user file to get following handles
                    using (StreamReader tweetReader = new StreamReader(BaseDir + "user.txt"))
                    {
                        tweets = GetFollowing(tweetReader);
                    }

                    //Read the tweets
                    using (StreamReader tweetReader = new StreamReader(BaseDir + "tweet.txt"))
                    {
                       GetTweets(tweetReader, listOfHandles, tweets);
                    }
                }
                catch(FileNotFoundException fileException)
                {
                    Console.WriteLine("File not found!!!");
                }                
            }
            else
            {
                Console.WriteLine("File directory does not exists!");
            }
            Console.ReadKey();   //leave console displayed... until user presses any key to close             
        }
    }
}

using Google.Apis.Auth.OAuth2;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Syndication;
using System.Xml;

namespace BlogRssFeed
{
    [Command(Description = "Pushes rss feed to blogger article.", Name ="create")]
    public class CreateCommand
    {
        private string bloggerApiUrl = string.Empty;
        private string rSSFeedUrlStr = string.Empty;
        private int feedArticleStartCountInt = 0;
        private int feedArticleEndCountInt = 0;
        private string authToken = string.Empty;


        [Required]
        [Option(Description = "Api key.", ShortName = "k")]
        public string apiKey { get; set; } = null!;

        [Required]
        [Option(Description = "Auth Credentials json.", ShortName = "c")]
        public string authCredentials { get; set; } = null!;

        [Required]
        [Option(Description = "Blogger id.", ShortName = "i")]
        public string bloggerId { get; set; } = null!;

        [Required]
        [Option(Description = "Labels comma separated.", ShortName = "l")]
        public string labels { get; set; } = null!;

        [Required]
        [Option(Description = "Rss feed url.", ShortName = "r")]
        public string rSSFeedUrl { get; set; } = null!;

        [Required]
        [Option(Description = "starting count of rss feed article.", ShortName = "s")]
        public string feedArticleStartCount { get; set; } = null!;

        [Required]
        [Option(Description = "ending count of rss feed article.", ShortName = "e")]
        public string feedArticleEndCount { get; set; } = null!;

        public int OnExecute(IConsole console)
        {
            try
            {
                string authCredentialsStr = authCredentials;
                byte[] data = Convert.FromBase64String(authCredentialsStr);
                var labelList = labels.Split(',');
                authCredentialsStr = System.Text.Encoding.UTF8.GetString(data);
                authToken = GoogleCredential.FromJson(authCredentialsStr)
                .CreateScoped("https://www.googleapis.com/auth/blogger") // Gathers scopes requested  
                .UnderlyingCredential // Gets the credentials  
                .GetAccessTokenForRequestAsync().Result; // Gets the Access Token  

                bloggerApiUrl = "https://blogger.googleapis.com/v3/blogs/[YourBlogId]/posts?fetchBody=true&fetchImages=true&isDraft=false&key=[YOUR-API-KEY]";
                bloggerApiUrl = bloggerApiUrl.Replace("[YourBlogId]", bloggerId);
                bloggerApiUrl = bloggerApiUrl.Replace("[YOUR-API-KEY]", apiKey);

                this.rSSFeedUrlStr = rSSFeedUrl;
                this.feedArticleStartCountInt = feedArticleStartCount == null ? 0 : int.Parse(feedArticleStartCount);
                this.feedArticleEndCountInt = feedArticleEndCount == null ? 30 : int.Parse(feedArticleEndCount);

                var reader = XmlReader.Create(this.rSSFeedUrlStr);
                var feed = SyndicationFeed.Load(reader);

                List<BlogPost> postList = new List<BlogPost>();
                var feedItems = feed.Items.ToList<SyndicationItem>();

                int startCount = this.feedArticleStartCountInt;
                Console.WriteLine(feed.Items.Count<SyndicationItem>() + " Items found in feed. starting pushing from " + startCount + " to " + this.feedArticleEndCountInt);

                //Loop through all items in the SyndicationFeed
                for (int i = startCount; i < this.feedArticleEndCountInt | i < feed.Items.Count<SyndicationItem>(); i++)
                {
                    BlogPost bp = new BlogPost
                    {
                        title = feedItems[i].Title.Text,
                        content = feedItems[i].Summary.Text + "<P>This article is orginally published <a href=\"" + feedItems[i].Links[0].Uri.OriginalString + "\">here</a></P>",
                        labels = labelList,
                    };

                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(bp));

                        client.DefaultRequestHeaders.Add("Authorization", authToken);
                        var respons = client.PostAsync(bloggerApiUrl, content).Result;

                        var response = respons.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(response);

                        Task.Delay(2000).Wait();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
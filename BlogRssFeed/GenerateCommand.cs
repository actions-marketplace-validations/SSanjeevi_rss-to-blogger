using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using static System.Formats.Asn1.AsnWriter;

namespace BlogRssFeed
{
    public class CreateCommand : CommandLineApplication
    {
        private string bloggerApiUrl = string.Empty;
        private string rSSFeedUrlStr = string.Empty;
        private int feedArticleStartCountInt = 0;
        private int feedArticleEndCountInt = 0;
        private string authToken = string.Empty;

        public CreateCommand()
        {
            try
            {
                this.Name = "Create";
                this.Description = "Create Articles from rss";

                CommandOption apiKey = this.Option("--api-key <apiKey>", "Api key", CommandOptionType.SingleValue);

                CommandOption authCredentials = this.Option("--authCredentials <authCredentials>", "Auth Credentials json", CommandOptionType.SingleValue);

                CommandOption bloggerId = this.Option("--bloggerId <bloggerId>", "Blogger Id", CommandOptionType.SingleValue);

                CommandOption labels = this.Option("--labels <labels>", "Array of labels for article", CommandOptionType.MultipleValue);

                CommandOption rSSFeedUrl = this.Option("--rSSFeedUrl <rSSFeedUrl>", "RSS Feed Url Id", CommandOptionType.SingleValue);

                CommandOption feedArticleStartCount = this.Option("--feedArticleStartCount <feedArticleStartCount>", "feed Article Start Count", CommandOptionType.SingleValue);

                CommandOption feedArticleEndCount = this.Option("--feedArticleEndCount <feedArticleEndCount>", "feed Article End Count", CommandOptionType.SingleValue);

                this.OnExecute((Func<Task<int>>)(async () =>
                {
                    List<string> labelList = new List<string> { "azure", "azure-cloud", "microsoft", "cloud"};
                    if(labels?.Values != null && labels?.Values.Count > 0)
                    {
                        labelList = labels.Values;
                    }
                    string authCredentialsStr = authCredentials?.Value();
                    byte[] data = Convert.FromBase64String(authCredentialsStr);
                    authCredentialsStr = System.Text.Encoding.UTF8.GetString(data);
                    authToken = await GoogleCredential.FromJson(authCredentialsStr)
                    .CreateScoped("https://www.googleapis.com/auth/blogger") // Gathers scopes requested  
                    .UnderlyingCredential // Gets the credentials  
                    .GetAccessTokenForRequestAsync(); // Gets the Access Token  

                    bloggerApiUrl = "https://blogger.googleapis.com/v3/blogs/[YourBlogId]/posts?fetchBody=true&fetchImages=true&isDraft=false&key=[YOUR-API-KEY]";
                    bloggerApiUrl = bloggerApiUrl.Replace("[YourBlogId]", bloggerId?.Value());
                    bloggerApiUrl = bloggerApiUrl.Replace("[YOUR-API-KEY]", apiKey?.Value());
                
                    this.rSSFeedUrlStr = rSSFeedUrl?.Value();
                    this.feedArticleStartCountInt = feedArticleStartCount?.Value() == null ? 0 : int.Parse(feedArticleStartCount.Value());
                    this.feedArticleEndCountInt = feedArticleEndCount?.Value() == null ? 30 : int.Parse(feedArticleEndCount.Value());
                    
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
                            var respons = await client.PostAsync(bloggerApiUrl, content);

                            var response = await respons.Content.ReadAsStringAsync();
                            Console.WriteLine(response);

                            await Task.Delay(2000);
                        }
                    }

                    return 0;
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
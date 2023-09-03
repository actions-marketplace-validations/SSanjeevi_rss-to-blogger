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
        private string rSSFeedUrl = string.Empty;
        private int feedArticleStartCount = 0;
        private int feedArticleEndCount = 0;
        private string authToken = string.Empty;

        public CreateCommand()
        {
            try
            {
                this.Name = "Create";
                this.Description = "Create Name";

                CommandOption authCredentialsOption = this.Option("--auth-Credentials <authCredentialsOption>", "Auth Credentials json", CommandOptionType.SingleValue);

                CommandOption bloggerIdOption = this.Option("--blogger-Id <bloggerIdOption>", "Blogger Id", CommandOptionType.SingleValue);

                CommandOption labelsOption = this.Option("--labels-array <labelsOption>", "Array of labels for article", CommandOptionType.MultipleValue);

                CommandOption rSSFeedUrlOption = this.Option("--rSS-Feed-Url <rSSFeedUrlOption>", "RSS Feed Url Id", CommandOptionType.SingleValue);

                CommandOption feedArticleStartCountOption = this.Option("--feed-Article-Start-Count <feedArticleStartCountOption>", "feed Article Start Count", CommandOptionType.SingleValue);

                CommandOption feedArticleEndCountOption = this.Option("--feed-Article-End-Count <feedArticleEndCountOption>", "feed Article End Count", CommandOptionType.SingleValue);

                this.OnExecute(async () =>
                {
                    List<string> labels = new List<string> { "azure", "azure-cloud", "microsoft", "cloud"};
                    if(labelsOption?.Values != null && labelsOption?.Values.Count > 0)
                    {
                        labels = labelsOption.Values;
                    }
                    string authCredentials = authCredentialsOption?.Value();
                    authToken = await GoogleCredential.FromJson(authCredentials)
                    .CreateScoped("https://www.googleapis.com/auth/blogger") // Gathers scopes requested  
                    .UnderlyingCredential // Gets the credentials  
                    .GetAccessTokenForRequestAsync(); // Gets the Access Token  

                    bloggerApiUrl = "https://blogger.googleapis.com/v3/blogs/[YourBlogId]/posts?fetchBody=true&fetchImages=true&isDraft=false";
                    bloggerApiUrl = bloggerApiUrl.Replace("[YourBlogId]", bloggerIdOption?.Value());
                    rSSFeedUrl = rSSFeedUrlOption?.Value();
                    feedArticleStartCount = feedArticleStartCountOption?.Value() == null ? 0 : int.Parse(feedArticleStartCountOption.Value());
                    feedArticleEndCount = feedArticleEndCountOption?.Value() == null ? 30 : int.Parse(feedArticleEndCountOption.Value());
                    
                    var reader = XmlReader.Create(rSSFeedUrl);
                    var feed = SyndicationFeed.Load(reader);

                    List<BlogPost> postList = new List<BlogPost>();
                    var feedItems = feed.Items.ToList();

                    int startCount = feedArticleStartCount;
                    Console.WriteLine(feed.Items.Count() + " Items found in feed. starting pushing from " + startCount + " to " + feedArticleEndCount);

                    //Loop through all items in the SyndicationFeed
                    for (int i = startCount; i < feedArticleEndCount | i < feed.Items.Count(); i++)
                    {
                        BlogPost bp = new BlogPost
                        {
                            title = feedItems[i].Title.Text,
                            content = feedItems[i].Summary.Text + "<P>This article is orginally published <a href=\"" + feedItems[i].Links[0].Uri.OriginalString + "\">here</a></P>",
                            labels = labels,
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
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
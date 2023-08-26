using Newtonsoft.Json;
using System.Configuration;
using System.ServiceModel.Syndication;
using System.Xml;

Console.WriteLine("Welcome to Blogger Article writer from RSS feed!");
var reader = XmlReader.Create(ConfigurationManager.AppSettings["RSSFeedUrl"]);
var feed = SyndicationFeed.Load(reader);


List<BlogPost> postList = new List<BlogPost>();
var feedItems = feed.Items.ToList();

int startCount = int.Parse(ConfigurationManager.AppSettings["FeedArticleStartCount"]);
Console.WriteLine(feed.Items.Count() + " Items found in feed. starting pushing from " + startCount);

//Loop through all items in the SyndicationFeed
for (int i= startCount; i< feedItems.Count(); i++)
{
    BlogPost bp = new BlogPost
    {
        title = feedItems[i].Title.Text,
        content = feedItems[i].Summary.Text + "<P>This article is orginally published <a href=\"" + feedItems[i].Links[0].Uri.OriginalString + "\">here</a></P>",
        labels = new List<string> { "azure", "azure-cloud", "microsoft", "cloud" },
    };

    using (var client = new HttpClient())
    {
        var content = new StringContent(JsonConvert.SerializeObject(bp));

        client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["AuthToken"]);
        var respons = await client.PostAsync(ConfigurationManager.AppSettings["BloggerApiUrl"], content);

        var response = await respons.Content.ReadAsStringAsync();
        Console.WriteLine(response);

        await Task.Delay(2000);
    }
}


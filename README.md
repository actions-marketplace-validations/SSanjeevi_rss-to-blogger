# Publish Rss feed posts As Posts in Blogger Website automatically


This Github action is to Publish Rss feed posts As Posts in Blogger Website automatically by using command line tool for pulling articles from RSS feed to Blogger as articles, Considering Google allows Blogger api to be used 50 per day, you can use this CLI tool to pull 50 articles per day from any RSS feed mentioned in the parameter - RSSFeedUrl.

We have to generate Oauth token to push articles in blogger for that we need credentials json for the Blogger owner service account by going through the following article and generate the credentials key json - https://cloud.google.com/iam/docs/keys-create-delete#creating .

Add the credentials json and format it to single line content and add to Github secrets and provide the secret name in this parameter - GOOGLE_CREDENTIALS_Json.

Update your blog id in the parameter BloggerId.

Update the RSS feed url to fetch the articles from - eg.: below url we can provide for parameter RSSFeedUrl :
https://techcommunity.microsoft.com/plugins/custom/microsoft/o365/custom-blog-rss?tid=-189242975710986885&board=FastTrackforAzureBlog&size=55

You can provide list of string which we can add to the articles as labels or tags in this parameter Labels.

FeedArticleStartCount and FeedArticleEndCount are the parameters used to control the start count and end count of the RSS feed articles count to be pushed start and end limit.

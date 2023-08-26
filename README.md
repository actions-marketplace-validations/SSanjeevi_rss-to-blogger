# RSS To Blogger articles Command line tool in Dotnet


This Repo is a command line tool for pulling articles from RSS feed to Blogger as articles, Considering Google allows Blogger api to be used 50 per day, you can use this CLI tool to pull 50 articles per day from any RSS feed mentioned in the ./BlogRssFeed/App.config file parameter - RSSFeedUrl.

We have to generate Oauth token for same user as the Blogger owner Google account by going to the blogger api tester app here - https://developers.google.com/blogger/docs/3.0/reference/blogs/get and select Oauth and hit with your blog id and in network tab get the Oauth token from request header and paste it in the app.config param - AuthToken

Update your blog id in the param BloggerApiUrl in place of [YourBlogId]

Run the console app in Visual Studio.

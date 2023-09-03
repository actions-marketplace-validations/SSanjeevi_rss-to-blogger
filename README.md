# Publish Rss feed posts As Posts in Blogger Website automatically


This Github action is to Publish Rss feed posts As Posts in Blogger Website automatically by using command line tool for pulling articles from RSS feed to Blogger as articles, Considering Google allows Blogger api to be used 50 per day, you can use this CLI tool to pull 50 articles per day from any RSS feed mentioned in the parameter - RSSFeedUrl.

We have to generate Oauth token to push articles in blogger for that we need credentials json for the Blogger owner service account by going through the following article and generate the credentials key json - https://cloud.google.com/iam/docs/keys-create-delete#creating .

Add the credentials json and convert it to base64 format and add to Github secrets and provide the secret name in this parameter - GOOGLE_CREDENTIALS_Json.

Create a google api key and add blogger permission to that key and add in secret as ApiKey.

Update your blog id in the parameter BloggerId.

Update the RSS feed url to fetch the articles from - eg.: below url we can provide for parameter RSSFeedUrl :
https://techcommunity.microsoft.com/plugins/custom/microsoft/o365/custom-blog-rss?tid=-189242975710986885&board=FastTrackforAzureBlog&size=55

You can provide list of string which we can add to the articles as labels or tags in this parameter Labels.

FeedArticleStartCount and FeedArticleEndCount are the parameters used to control the start count and end count of the RSS feed articles count to be pushed start and end limit.

Overall the workflow yaml will look like this:


```yml
name: rss-to-blogger-push

# Controls when the workflow will run
on:
  # Allows you to run this workflow manually from the Actions tab
  workflow_dispatch:

# A workflow run is made up of one or more jobs that can run sequentially or in parallel
jobs:
  # This workflow contains a single job called "build"
  build:
    # The type of runner that the job will run on
    runs-on: ubuntu-latest

    # Steps represent a sequence of tasks that will be executed as part of the job
    steps:
        - name: Rss-feed-to-Blogger-action
          uses: SSanjeevi/rss-to-blogger@release-1.14
          with:
           GOOGLE_CREDENTIALS_Json: ${{ secrets.GOOGLE_CREDENTIALS_Json }}
           BloggerId: '4243659437681898774'
           RSSFeedUrl: 'https://techcommunity.microsoft.com/plugins/custom/microsoft/o365/custom-blog-rss?tid=-189242975710986885&board=FastTrackforAzureBlog&size=355'
           FeedArticleStartCount: '66'
           FeedArticleEndCount: '67'
           Labels: 'azure'
           ApiKey: ${{ secrets.ApiKey }}
```

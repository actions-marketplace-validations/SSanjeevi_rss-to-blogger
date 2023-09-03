
namespace BlogRssFeed;

public class BlogPost
{
    public Author author { get; set; }
    public string content { get; set; }
    public List<string> labels { get; set; }
    public string title { get; set; }
}

public class Author
{
    public string displayName { get; set; }
}
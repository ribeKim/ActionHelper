using System;
using System.Collections.Generic;

namespace ActionHelper.scripts.Editor
{
    public class GithubResponse
    {
        public string URL { get; set; }
        public string HtmlURL { get; set; }
        public string AssetsURL { get; set; }
        public string UploadURL { get; set; }
        public string TarballURL { get; set; }
        public string ZipballURL { get; set; }
        public string DiscussionURL { get; set; }
        public int ID { get; set; }
        public string NodeID { get; set; }
        public string TagName { get; set; }
        public string TargetCommitish { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public bool Draft { get; set; }
        public bool Prerelease { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public Author Author { get; set; }
        public List<Asset> Assets { get; set; }
    }

    public class Author
    {
        public string Login { get; set; }
        public int ID { get; set; }
        public string NodeID { get; set; }
        public string AvatarURL { get; set; }
        public string GravatarID { get; set; }
        public string URL { get; set; }
        public string HtmlURL { get; set; }
        public string FollowersURL { get; set; }
        public string FollowingURL { get; set; }
        public string GistsURL { get; set; }
        public string StarredURL { get; set; }
        public string SubscriptionsURL { get; set; }
        public string OrganizationsURL { get; set; }
        public string ReposURL { get; set; }
        public string EventsURL { get; set; }
        public string ReceivedEventsURL { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class Uploader
    {
        public string Login { get; set; }
        public int ID { get; set; }
        public string NodeID { get; set; }
        public string AvatarURL { get; set; }
        public string GravatarID { get; set; }
        public string URL { get; set; }
        public string HtmlURL { get; set; }
        public string FollowersURL { get; set; }
        public string FollowingURL { get; set; }
        public string GistsURL { get; set; }
        public string StarredURL { get; set; }
        public string SubscriptionsURL { get; set; }
        public string OrganizationsURL { get; set; }
        public string ReposURL { get; set; }
        public string EventsURL { get; set; }
        public string ReceivedEventsURL { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class Asset
    {
        public string URL { get; set; }
        public string BrowserDownloadURL { get; set; }
        public int ID { get; set; }
        public string NodeID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string State { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Uploader Uploader { get; set; }
    }
}
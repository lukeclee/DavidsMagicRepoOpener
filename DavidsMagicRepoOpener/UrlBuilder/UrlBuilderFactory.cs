using System;

namespace DavidsMagicRepoOpener.UrlBuilder
{
    public class UrlBuilderFactory : IUrlBuilderFactory
    {
        public IUrlBuilder CreateUrlBuilder(string url)
        {
            if (url.Contains("azure"))
            {
                return new AzureUrlBuilder();
            }

            if (url.Contains("github"))
            {
                return new GitHubUrlBuilder();
            }

            throw new Exception("Url not supported");
        }
    }
}

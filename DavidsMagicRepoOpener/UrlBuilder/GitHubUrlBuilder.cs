using DavidsMagicRepoOpener.Models;

namespace DavidsMagicRepoOpener.UrlBuilder
{
    public class GitHubUrlBuilder : IUrlBuilder
    {
        public GitHubUrlBuilder() { }

        public string BuildUrl(string url, string branch)
        {
            var cleanedUrl = this.CleanUrl(url);

            return $"{cleanedUrl}/blob/{branch}";
        }

        public string BuildUrlToSpecificFile(string url, string fileName, string branch, string solutionDirectory, SelectedTextPoints selection)
        {
            var cleanedUrl = this.CleanUrl(url);

            var scrubbedFileName = fileName.Replace(solutionDirectory, "");

            var urlWithFile = $"{cleanedUrl}/blob/{branch}/{scrubbedFileName}";

            if (!selection.IsTextSelectionStartAndEndTheSame())
            {
                urlWithFile += $"#L{selection.StartLine}-L{selection.EndLine}";
            }

            return urlWithFile;
        }

        private string CleanUrl(string url)
        {
            return url.Replace(".git", "");
        }
    }
}

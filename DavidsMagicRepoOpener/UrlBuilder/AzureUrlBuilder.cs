using DavidsMagicRepoOpener.Models;
using System.Linq;

namespace DavidsMagicRepoOpener.UrlBuilder
{
    public class AzureUrlBuilder : IUrlBuilder
    {
        public AzureUrlBuilder() { }

        public string BuildUrl(string url, string branch)
        {
            var cleanedUrl = this.CleanUrl(url);
            return $"{cleanedUrl}?path=/&version=GB{branch}";
        }

        public  string BuildUrlToSpecificFile(string url, string fileName, string branch, string solutionDirectory, SelectedTextPoints selection)
        {
            var cleanedUrl = this.CleanUrl(url);

            var cleanedSolutionDirectory = this.CleanDirectory(solutionDirectory);

            var cleanedFileName = this.CleanFileName(fileName, cleanedSolutionDirectory, url);

            var urlWithFile = $"{cleanedUrl}?path={cleanedFileName}&version=GB{branch}";

            if (!selection.IsTextSelectionStartAndEndTheSame())
            {
                urlWithFile += $"&line={selection.StartLine}&lineEnd={selection.EndLine}&lineStartColumn={selection.StartColumn}&lineEndColumn={selection.EndColumn}";
            }

            return urlWithFile;
        }

        private string CleanUrl(string url)
        {
            return $"https://{url.Substring(url.IndexOf('@') + 1)}";
        }

        private string CleanFileName(string fileName, string solutionDirectory, string url)
        {
            fileName = fileName.Replace("\\", "/");

            return fileName.Replace(solutionDirectory, "");
        }

        private string CleanDirectory(string solutionDirectory)
        {
            var expandedSolutionDirectory = solutionDirectory.Replace("\\", "/").Split('/');

            var solutionDirectoryForUrl = string.Join("/", expandedSolutionDirectory.Take(expandedSolutionDirectory.Length - 2));

            return solutionDirectoryForUrl;
        }
    }
}

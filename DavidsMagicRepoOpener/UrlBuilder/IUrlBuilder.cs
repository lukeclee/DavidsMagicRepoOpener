using DavidsMagicRepoOpener.Models;

namespace DavidsMagicRepoOpener.UrlBuilder
{
    public interface IUrlBuilder
    {
        string BuildUrl(string url, string branch);
        string BuildUrlToSpecificFile(string url, string fileName, string branch, string solutionDirectory, SelectedTextPoints selection);
    }
}

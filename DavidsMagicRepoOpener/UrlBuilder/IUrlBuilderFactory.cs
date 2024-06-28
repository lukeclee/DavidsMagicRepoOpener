namespace DavidsMagicRepoOpener.UrlBuilder
{
    public interface IUrlBuilderFactory
    {
        IUrlBuilder CreateUrlBuilder(string url);
    }
}
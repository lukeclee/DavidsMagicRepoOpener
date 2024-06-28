using DavidsMagicRepoOpener.Models;
using DavidsMagicRepoOpener.UrlBuilder;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DavidsMagicRepoOpener.RepoOpener
{
    public class RepoOpenerService
    {
        private readonly DTE2 dte;
        private readonly IVsSolution solution;
        private readonly IAsyncServiceProvider serviceProvider;
        private IUrlBuilderFactory urlBuilderFactory;

        public RepoOpenerService(DTE2 dte, IVsSolution solution, IAsyncServiceProvider serviceProvider) 
        {
            this.dte = dte;
            this.solution = solution;
            this.serviceProvider = serviceProvider;
            this.urlBuilderFactory = new UrlBuilderFactory();
        }

        public void OpenRepo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirectory = this.GetSolutionDirectory();

            var remoteUrlString = this.GetRemoteUrlUsingCmd(solutionDirectory);

            var branch = this.GetBranch(solutionDirectory);

            if (!string.IsNullOrEmpty(remoteUrlString) && !string.IsNullOrEmpty(branch))
            {
                string cleanedString = this.urlBuilderFactory.CreateUrlBuilder(remoteUrlString).BuildUrl(remoteUrlString, branch);

                this.OpenWebPage(cleanedString);
            }
            else
            {
                this.ShowErrorMessage();
            }
        }

        public void OpenRepoToSpecificFile()
        {
            var cleanedString = this.GetCleanedUrlString();

            if (!string.IsNullOrEmpty(cleanedString))
            {
                this.OpenWebPage(cleanedString);
            }
            else
            {
                this.ShowErrorMessage();
            }
        }

        public void GenerateUrl()
        {
            var cleanedString = this.GetCleanedUrlString();

            if (!string.IsNullOrEmpty(cleanedString))
            {
                this.AddToClipboard(cleanedString);
            }
            else
            {
                this.ShowErrorMessage();
            }
        }

        private string GetCleanedUrlString()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var fileName = dte.ActiveDocument.FullName;

            var selection = dte.ActiveDocument.Selection as TextSelection;

            var selectedText = new SelectedTextPoints(selection.TopLine, selection.TopPoint.LineCharOffset, selection.BottomLine, selection.BottomPoint.LineCharOffset);

            var solutionDirectory = this.GetSolutionDirectory();

            var remoteUrlString = this.GetRemoteUrlUsingCmd(solutionDirectory);

            var branch = this.GetBranch(solutionDirectory);

            var result = string.Empty;

            if (!string.IsNullOrEmpty(remoteUrlString) && !string.IsNullOrEmpty(branch))
            {
                result = this.urlBuilderFactory.CreateUrlBuilder(remoteUrlString).BuildUrlToSpecificFile(remoteUrlString, fileName, branch, solutionDirectory, selectedText);
            }

            return result;
        }

        private string GetSolutionDirectory()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            solution.GetSolutionInfo(out string solutionDirectory, out _, out _);

            return solutionDirectory;
        }

        private string GetRemoteUrlUsingCmd(string solutionDirectory)
        {
            string command = "git config --get remote.origin.url";

            return this.GetOutputFromCommand(solutionDirectory, command);
        }

        private string GetBranch(string solutionDirectory)
        {
            string command = "git branch --show-current";

            var output =  this.GetOutputFromCommand(solutionDirectory, command);

            if (string.IsNullOrEmpty(output))
            {
                string commandToGetDefaultBranch = "git symbolic-ref refs/remotes/origin/HEAD | sed 's@^refs/remotes/origin/@@'";
                
                output = this.GetOutputFromCommand(solutionDirectory, commandToGetDefaultBranch);
            }

            return output;
        }

        private string GetOutputFromCommand(string solutionDirectory, string command)
        {
            System.Diagnostics.Process cmd = new System.Diagnostics.Process();

            cmd.StartInfo.FileName = "cmd.exe";

            cmd.StartInfo.Arguments = $"/c cd {solutionDirectory} && {command}";

            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;

            cmd.Start();

            string output = cmd.StandardOutput.ReadLine();

            cmd.WaitForExit();

            return output;
        }

        private void OpenWebPage(string url)
        {
            System.Diagnostics.Process webPage = new System.Diagnostics.Process();

            webPage.StartInfo.FileName = url;
            webPage.StartInfo.UseShellExecute = true;

            webPage.Start();
        }

        private void AddToClipboard(string text)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            System.Windows.Forms.Clipboard.SetText(text);
        }

        private string CleanFileName(string fileName, string remoteUrlString)
        {
            return fileName.Replace("\\", "/");
        }

        private void ShowErrorMessage()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            VsShellUtilities.ShowMessageBox(
                (System.IServiceProvider)this.serviceProvider,
                "There was an issue finding the remote repository for this project, please make sure that this is being tracked by git.",
                "Error",
                OLEMSGICON.OLEMSGICON_CRITICAL,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}

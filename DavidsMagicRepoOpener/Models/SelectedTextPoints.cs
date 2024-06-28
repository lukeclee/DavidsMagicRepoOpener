namespace DavidsMagicRepoOpener.Models
{
    public class SelectedTextPoints
    {
        public int StartLine { get; set; }
        public int StartColumn { get; set; }
        public int EndLine { get; set; }
        public int EndColumn { get; set; }

        public SelectedTextPoints(int startLine, int startColumn, int endLine, int endColumn)
        {
            this.StartLine = startLine;
            this.StartColumn = startColumn;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
        }

        public bool IsTextSelectionStartAndEndTheSame()
        {

           if (this.StartLine == this.EndLine)
           {
                if (this.StartColumn == this.EndColumn)
                {
                    return true;
                }
           }

            return false;
        }
    }
}

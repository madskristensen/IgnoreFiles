using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace IgnoreFiles
{
    public class Options : DialogPage
    {
        // Advanced
        [Category("General settings")]
        [DisplayName("Ignore patterns")]
        [Description("A semicolon-separated list of strings. Any file containing one of the strings in the path will be ignored.")]
        [DefaultValue(@"\node_modules;\.git;\packages")]
        public string IgnorePatterns { get; set; } = @"\node_modules;\.git;\packages";

        [Category("General settings")]
        [DisplayName("Show hover tooltip")]
        [Description("Shows tooltip when hovering the mouse over entries in .ignore files.")]
        [DefaultValue(true)]
        public bool ShowTooltip { get; set; } = true;

        public IEnumerable<string> GetIgnorePatterns()
        {
            var raw = IgnorePatterns.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pattern in raw)
            {
                yield return pattern;
            }
        }
    }
}

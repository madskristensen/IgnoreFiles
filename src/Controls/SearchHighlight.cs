using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace IgnoreFiles.Controls
{
    public static class SearchHighlight
    {
        public static readonly DependencyProperty SourceTextProperty = DependencyProperty.RegisterAttached(
            "SourceText", typeof(string), typeof(SearchHighlight), new PropertyMetadata("", TextChanged));


        public static readonly DependencyProperty HighlightStyleProperty = DependencyProperty.RegisterAttached(
            "HighlightStyle", typeof(Style), typeof(SearchHighlight), new PropertyMetadata(default(Style), StyleChanged));

        private static void StyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label lbl = d as Label;
            TextBlock block = d as TextBlock;

            if (lbl != null)
            {
                block = lbl.Content as TextBlock;

                if (block == null)
                {
                    string lblChild = lbl.Content as string;

                    if (lblChild == null)
                    {
                        return;
                    }

                    TextBlock newChild = new TextBlock { Text = lblChild };
                    lbl.Content = newChild;
                    block = newChild;
                }
            }

            if (block == null)
            {
                return;
            }
        }

        public static void SetSourceText(DependencyObject element, string value)
        {
            element.SetValue(SourceTextProperty, value);
        }

        public static string GetSourceText(DependencyObject element)
        {
            return (string)element.GetValue(SourceTextProperty);
        }

        public static void SetHighlightStyle(DependencyObject element, Style value)
        {
            element.SetValue(HighlightStyleProperty, value);
        }

        public static Style GetHighlightStyle(DependencyObject element)
        {
            return (Style)element.GetValue(HighlightStyleProperty);
        }

        public static readonly DependencyProperty HighlightTextProperty = DependencyProperty.RegisterAttached(
            "HighlightText", typeof(string), typeof(SearchHighlight), new PropertyMetadata(default(string), TextChanged));

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label lbl = d as Label;
            TextBlock block = d as TextBlock;

            if (lbl != null)
            {
                block = lbl.Content as TextBlock;

                if (block == null)
                {
                    string lblChild = lbl.Content as string;
                    TextBlock newChild = new TextBlock { Text = lblChild ?? "" };
                    lbl.Content = newChild;
                    block = newChild;
                }
            }

            if (block == null)
            {
                return;
            }

            string searchText = GetHighlightText(d);
            string blockText = GetSourceText(d);

            if (blockText == null)
            {
                return;
            }

            int last = 0;
            block.Inlines.Clear();

            if (!string.IsNullOrEmpty(searchText))
            {
                IReadOnlyList<Range> matches = GetMatchesInText(searchText, blockText);

                for (int i = 0; i < matches.Count; ++i)
                {
                    if (matches[i].Length == 0)
                    {
                        continue;
                    }

                    if (last < matches[i].Start)
                    {
                        string inserted = blockText.Substring(last, matches[i].Start - last);
                        block.Inlines.Add(inserted);
                        last += inserted.Length;
                    }

                    Run highlight = new Run(matches[i].ToString());
                    highlight.SetBinding(FrameworkContentElement.StyleProperty, new Binding
                    {
                        Mode = BindingMode.OneWay,
                        Source = d,
                        Path = new PropertyPath(HighlightStyleProperty)
                    });
                    block.Inlines.Add(highlight);
                    last += matches[i].Length;
                }
            }

            if (last < blockText.Length)
            {
                block.Inlines.Add(blockText.Substring(last));
            }
        }

        public static void SetHighlightText(DependencyObject element, string value)
        {
            element.SetValue(HighlightTextProperty, value);
        }

        public static string GetHighlightText(DependencyObject element)
        {
            return (string)element.GetValue(HighlightTextProperty);
        }

        private static IReadOnlyList<Range> GetMatchesInText(string searchTerm, string text)
        {
            List<Range> ranges = new List<Range>();

            string pattern = Regex.Escape(searchTerm);
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = r.Matches(text);
            ProcessMatchCollection(text, matches, ranges);
            return ranges;
        }

        private static void ProcessMatchCollection(string sourceString, MatchCollection matches, List<Range> ranges)
        {
            foreach (Match match in matches)
            {
                Range range = new Range(match.Index, match.Length, sourceString);
                if (ranges.Count == 0)
                {
                    ranges.Add(range);
                }
                else
                {
                    bool included = false;
                    int sortPosition = 0;
                    for (int i = 0; i < ranges.Count; ++i)
                    {
                        if (ranges[i].Start < range.Start)
                        {
                            sortPosition = i + 1;
                        }

                        Range tmp;
                        if (range.TryUnion(ranges[i], out tmp))
                        {
                            included = true;
                            ranges[i] = tmp;

                            if (i < ranges.Count - 1 && tmp.TryUnion(ranges[i + 1], out range))
                            {
                                ranges[i] = range;
                                ranges.RemoveAt(i + 1);
                                break;
                            }
                        }
                    }

                    if (!included)
                    {
                        if (sortPosition == ranges.Count)
                        {
                            ranges.Add(range);
                        }
                        else
                        {
                            ranges.Insert(sortPosition, range);
                        }
                    }
                }
            }
        }

        public struct Range
        {
            public Range(int start, int length, string sourceString)
            {
                Start = start;
                Length = length;
                SourceString = sourceString;
            }

            public readonly int Length;

            public readonly int Start;

            public readonly string SourceString;

            public bool TryUnion(Range other, out Range composite)
            {
                if (!string.Equals(other.SourceString, SourceString, StringComparison.Ordinal))
                {
                    composite = default(Range);
                    return false;
                }

                bool isLow = Start <= other.Start;
                Range low = isLow ? this : other;
                Range high = isLow ? other : this;

                int lowHigh = low.Start + low.Length;

                if (high.Start > lowHigh)
                {
                    composite = default(Range);
                    return false;
                }

                int highHigh = high.Start + high.Length;

                if (highHigh <= lowHigh)
                {
                    composite = low;
                    return true;
                }

                composite = new Range(low.Start, highHigh - low.Start, SourceString);
                return true;
            }

            public override string ToString()
            {
                return SourceString.Substring(Start, Length);
            }
        }
    }
}

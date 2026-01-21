using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using System.IO;
using Markdig.Helpers;
using Markdig.Parsers;

public class CodeSnippetExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<CodeSnippetParser>())
        {
            pipeline.InlineParsers.Insert(0, new CodeSnippetParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            if (!htmlRenderer.ObjectRenderers.Contains<CodeSnippetRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new CodeSnippetRenderer());
            }
        }
    }

    public CodeSnippetExtension()
    {
        System.Console.WriteLine("[CodeSnippetExtension] Loaded");
    }
}

public class CodeSnippetParser : Markdig.Parsers.InlineParser
{
    public CodeSnippetParser()
    {
        OpeningCharacters = new[] { '@' }; // @ ile başlayanları yakala
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var content = slice.ToString();
        if (content.StartsWith("@code-snippet"))
        {
            var parts = content.Split('"');
            if (parts.Length >= 2)
            {
                var src = parts[1];
                var inline = new CodeSnippetInline { Src = src };
                processor.Inline = inline;

                slice = new StringSlice("");

                return true;
            }
        }
        return false;
    }
}

public class CodeSnippetInline : Inline
{
    public string Src { get; set; }
}

public class CodeSnippetRenderer : HtmlObjectRenderer<CodeSnippetInline>
{
    protected override void Write(HtmlRenderer renderer, CodeSnippetInline obj)
    {
        if (File.Exists(obj.Src))
        {
            var code = File.ReadAllText(obj.Src);
            var escaped = System.Net.WebUtility.HtmlEncode(code);
            renderer.Write($"<pre><code class=\"lang-csharp\">{escaped}</code></pre>");
        }
        else
        {
            renderer.Write($"<pre><code>File not found: {obj.Src}</code></pre>");
        }
    }
}
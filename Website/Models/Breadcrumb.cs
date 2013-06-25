using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Breadcrumb
/// </summary>

#region Breadcrumb
public class Breadcrumb
{
    public string Url { get; set; }
    public string Title { get; set; }

    public Breadcrumb(string Url, string Title)
    {
        this.Url = Url;
        this.Title = Title;
    }

    public string ToHtml()
    {
        return "<a href=\"" + stripQuote(Url) + "\" title=\"" + stripQuote(Title) +
            "\" accesskey=\"1\">" + stripQuote(Title) + "</a>" + " <span>&gt;&gt;</span>";
    }

    private string stripQuote(string input)
    {
        return input == null ? null : input.Replace("\"", "&quot;");
    }
}
#endregion

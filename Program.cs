using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

var url = "https://en.wikipedia.org/wiki/Microsoft";
var web = new HtmlWeb();
var doc = web.Load(url);

var innerText = doc.DocumentNode.SelectNodes("//h2[span[@id=\"History\"]]/following-sibling::h2[1]/preceding-sibling::p[preceding-sibling::h2[span[@id=\"History\"]]]").Select(x => HttpUtility.HtmlDecode(x.InnerText).Trim().ToLower());
var text = string.Join(' ', innerText);
var rgx = new Regex("\\[\\d*\\]|[^\\w -]");
var words = rgx.Replace(text, "").Split(' ');

foreach (var word in words)
{
    System.Console.WriteLine(word);
}

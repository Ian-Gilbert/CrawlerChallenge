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

var frequencies = new Dictionary<string, int>();
foreach (var word in words)
{
    frequencies.TryGetValue(word, out var currentFreq);
    frequencies[word] = ++currentFreq;
}

frequencies = frequencies.OrderByDescending(x => x.Value)
                         .Take(10)
                         .ToDictionary(x => x.Key, x => x.Value);

var maxWordLength = frequencies.Keys.Max(x => x.Length);
var header = $"Word{new String(' ', maxWordLength - 4)} | Frequency";
System.Console.WriteLine(header);
System.Console.WriteLine(new String('-', header.Length));
foreach ((var word, var freq) in frequencies)
{
    System.Console.WriteLine($"{word}{new String(' ', maxWordLength - word.Length)} | {freq}");
}

using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

// Words to exclude can be added as console args.
// Number of words to return can be configured in the GetWordFrequencies call below.
// I chose not to build an elaborate user interface for the configuration because
//  a) I felt that it was out of scope
//  b) this feels more appropriate if this were to be integrated into an api or something.

var words = ParseWordList();
var frequencies = GetWordFrequencies(words, 10, args);
PrettyPrint(frequencies);

IEnumerable<string> ParseWordList()
{
    var url = "https://en.wikipedia.org/wiki/Microsoft";
    var web = new HtmlWeb();
    var doc = web.Load(url);

    // Select the inner text of all <p> tags between the h2 tag with id="History" and the next h2 tag.
    var innerText = doc.DocumentNode.SelectNodes(
                                        @"//h2[span[@id=""History""]]
                                            /following-sibling::h2[1]
                                                /preceding-sibling::p[
                                                    preceding-sibling::h2[span[@id=""History""]]
                                                ]"
                                    ).Select(x => HttpUtility.HtmlDecode(x.InnerText).Trim().ToLower());
    var text = string.Join(' ', innerText); // join paragraphs together
    var rgx = new Regex(@"\[\d*\]|[^\w -]"); // regex to remove footnote indicators and other misc. punctuation and non-text
    return rgx.Replace(text, "").Split(' ');
}

Dictionary<string, int> GetWordFrequencies(IEnumerable<string> words, int topWordCount, params string[] excludedWords)
{
    var frequencies = new Dictionary<string, int>();
    foreach (var word in words)
    {
        // Try to add the word to the frequencies list with a count of 1. If the word already exists, increment the count.
        if (!frequencies.TryAdd(word, 1))
            frequencies[word]++;
    }

    // Removing excluded words after generating the frequencies avoids extra work checking the excluded list against duplicate words.
    foreach (var word in excludedWords)
    {
        frequencies.Remove(word.ToLower());
    }

    // Return top results
    return frequencies.OrderByDescending(x => x.Value)
                      .Take(topWordCount)
                      .ToDictionary(x => x.Key, x => x.Value);
}

void PrettyPrint(Dictionary<string, int> frequencies)
{
    var maxWordLength = frequencies.Keys.Max(x => x.Length);
    var header = $"Word{new String(' ', maxWordLength - 4)} | Frequency";
    System.Console.WriteLine(header);
    System.Console.WriteLine(new String('-', header.Length));
    foreach ((var word, var freq) in frequencies)
    {
        System.Console.WriteLine($"{word}{new String(' ', maxWordLength - word.Length)} | {freq}");
    }
}

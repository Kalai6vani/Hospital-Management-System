using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class WebSearchService
{
    private readonly HttpClient _httpClient;

    public WebSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SearchMedicalInfoAsync(string query)
    {
        // Define a list of trusted sources
        string[] sources =
        {
"https://medlineplus.gov",
"https://www.mayoclinic.org/diseases-conditions",
"https://www.who.int/health-topics",
"https://www.cdc.gov",
"https://www.drugs.com",
"https://www.nhs.uk/conditions"
        };

        //Constructs a search URL by combining the source URL with the query, replacing spaces with hyphens
        foreach (var source in sources)
        {
            string searchUrl = $"{source}/{query.Replace(" ", "-")}";
            string result = await ScrapeWebsite(searchUrl); // Call the scraping method
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }

        return "No relevant medical information found.";
    }

    private async Task<string> ScrapeWebsite(string url)
    {
        //exception handling
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument(); 
            //Creates a new instance of HtmlDocument
            htmlDoc.LoadHtml(response);
            //Loads the HTML content from the response into the HtmlDocument object

            //var contentNode = htmlDoc.DocumentNode.SelectSingleNode("//p"); 
            // Select first paragraph
            //return contentNode?.InnerText ?? "Content not found."; 
            //nullable operator
            //InnerText property returns the text content of the node only for single node

            var contentNode = htmlDoc.DocumentNode.SelectNodes("//p");
            if (contentNode != null && contentNode.Count > 0)
            {
                var content = new StringBuilder();
                int count = 0;
                foreach (var node in contentNode)
                {
                    if (count >= 3) break; // Limit to the first three paragraphs
                    content.AppendLine(node.InnerText);
                    count++;
                }
                return content.ToString();
            }
            return "Content not found.";
        }
        catch (Exception)
        {
            return null; // Ignore failed sites and continue searching
        }
    }
}
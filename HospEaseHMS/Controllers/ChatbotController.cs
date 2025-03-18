using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospEaseHMS.Controllers
{
    [Authorize(Roles = "Doctor,Admin,Patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly WebSearchService _webSearchService;
        
        //perform logging
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(WebSearchService webSearchService, ILogger<ChatbotController> logger)
        {
            _webSearchService = webSearchService;
            _logger = logger;
        }

        [HttpGet("{query}")]
        public async Task<IActionResult> GetMedicalInfo(string query)
        {
            //exception handling + logging
            try
            {
                //log the query
                _logger.LogInformation("Searching for medical information for query: {Query}", query);
                var result = await _webSearchService.SearchMedicalInfoAsync(query);
                _logger.LogInformation("Medical information found for query: {Query}", query);
                return Ok(new { Query = query, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for medical information.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching for medical information.");
            }
        }
    }
}

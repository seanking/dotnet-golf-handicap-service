using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using static System.String;

namespace Course
{
    [Route("api/[controller]")]
    public class CourseController : Controller
    {
        private readonly IElasticClient _client;

        public CourseController(IElasticClient client)
        {
            _client = client;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var response = await _client.GetAsync<Course>(id, idx => idx.Index("course"));
            if (response.Found)
            {
                return Ok(response.Source);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] Course course)
        {
            var response = await _client.IndexAsync(course, idx => idx.Index("course"));
            if (response.IsValid)
            {
                return Ok(response.Id);
            }

            return NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<Course>> Search([FromQuery]string term)
        {
            if (IsNullOrEmpty(term))
            {
                var matchAllResponse = await _client.SearchAsync<Course>(s => s.Index("course").MatchAll());
                return Ok(matchAllResponse.Documents);
            }

            var queryResponse =
                await _client.SearchAsync<Course>(s => 
                    s.Index("course").Query(q => 
                        q.Match(m => m.Field("name").Query(term))));
            return Ok(queryResponse.Documents);
        }
    }
}
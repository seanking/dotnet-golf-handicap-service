using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;

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
    }
}
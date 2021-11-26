using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using static System.String;

namespace Course
{
    [Route("api/[controller]")]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var response = await _courseRepository.FindById(id);
            
            if (response != null)
            {
                return Ok(response);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] Course course)
        {
            var id = await _courseRepository.Save(course);

            if (!IsNullOrEmpty(id))
            {
                return Ok(id);
            }

            return NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<Course>> Search([FromQuery] string name)
        {
            IReadOnlyCollection<Course> courses;
            if (IsNullOrEmpty(name))
            {
                courses = await _courseRepository.FindAll();
            }
            else
            {
                courses = await _courseRepository.FindByName(name);
            }

            return Ok(courses);
        }
    }
}
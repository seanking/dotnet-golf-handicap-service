using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Course
{
    public interface ICourseRepository
    {
        Task<string> Save(Course course);
        Task<Course> FindById(string id);
        Task<IReadOnlyCollection<Course>> FindAll();
        Task<IReadOnlyCollection<Course>> FindByName(string name);
    }

    public class CourseRepository : ICourseRepository
    {
        private readonly ElasticClient _client;
        private static string _course = "course";

        public CourseRepository(ElasticClient _client)
        {
            this._client = _client;
        }

        public async Task<Course> FindById(string id)
        {
            var response = await _client.GetAsync<Course>(id, idx => idx.Index(_course));
            return response.Source;
        }

        public async Task<string> Save(Course course)
        {
            var response = await _client.IndexAsync(course, idx => idx.Index(_course));
            return response.Id;
        }

        public async Task<IReadOnlyCollection<Course>> FindAll()
        {
            var response = await _client.SearchAsync<Course>(s => s.Index(_course).MatchAll());
            return response.Documents;
        }

        public async Task<IReadOnlyCollection<Course>> FindByName(string name)
        {
            var response = await _client.SearchAsync<Course>(s =>
                s.Index(_course).Query(q =>
                    q.Match(m => m.Field("name").Query(name))));

            return response.Documents;
        }
    }
}
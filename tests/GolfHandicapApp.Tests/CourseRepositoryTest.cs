using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using Nest;
using NUnit.Framework;
using static RandomStringUtils.RandomStringUtils;

namespace Course
{
    public class CourseRepositoryTest
    {
        private TestcontainersContainer _testContainer;
        private CourseRepository _repo;
        private int _privateElasticsearchPort = 9200;

        [OneTimeSetUp]
        public async Task init()
        {
            var testContainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("docker.elastic.co/elasticsearch/elasticsearch:7.15.1")
                .WithPortBinding(_privateElasticsearchPort, true)
                .WithEnvironment("discovery.type", "single-node")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(_privateElasticsearchPort));

            _testContainer = testContainersBuilder.Build();
            await _testContainer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task tearDown()
        {
            await _testContainer.StopAsync();
            await _testContainer.DisposeAsync();
        }

        [SetUp]
        public void setupRepository()
        {
            var mappedPublicPort = _testContainer.GetMappedPublicPort(_privateElasticsearchPort);
            var client = BuildClient(mappedPublicPort);
            _repo = new CourseRepository(client);
        }

        [Test]
        public void ShouldSaveCourse()
        {
            // Given
            var course = BuildCourse();

            // When
            var id = _repo.Save(course);

            // Then
            id.Should().Equals(course.Id);
        }

        [Test]
        public void ShouldFindCourseById()
        {
            // Given
            var course = BuildCourse();
            var id = _repo.Save(course);

            // When
            var found = _repo.FindById(id.Result);

            // Then
            found.Should().Equals(course);
        }

        [Test]
        public void ShouldFindAllCourses()
        {
            var course1 = BuildCourse();
            var course2 = BuildCourse();

            _repo.Save(course1);
            _repo.Save(course2);

            // When 
            var result = _repo.FindAll();

            // Then
            result.Should().Equals(new List<Course> {course1, course2});
        }

        [Test]
        public void ShouldFindCourseByName()
        {
            var course1 = BuildCourse();
            var course2 = BuildCourse();

            _repo.Save(course1);
            _repo.Save(course2);

            // When 
            var result = _repo.FindByName(course1.Name);

            // Then
            result.Should().Equals(new List<Course> {course1});
        }

        private static Course BuildCourse()
        {
            return new Course()
            {
                Name = $"Course: {RandomAlphabetic(10)}",
                Tees = new List<Tee>()
                {
                    new Tee()
                    {
                        Name = $"Tee: {RandomAlphabetic(10)}",
                        Rating = 70.2,
                        Slope = 132.1
                    }
                }
            };
        }

        private static ElasticClient BuildClient(ushort mappedPublicPort)
        {
            var node = new Uri($"http://localhost:{mappedPublicPort}");
            var settings = new ConnectionSettings(node);
            return new ElasticClient(settings);
        }
    }
}
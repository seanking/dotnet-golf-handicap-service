using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Nest;
using NUnit.Framework;
using static RandomStringUtils.RandomStringUtils;

namespace Course
{
    public class ContainerTest
    {
        private TestcontainersContainer _testContainer;
        private CourseController _controller;
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
        public void setupController()
        {
            var mappedPublicPort = _testContainer.GetMappedPublicPort(_privateElasticsearchPort);
            var client = BuildClient(mappedPublicPort);
            _controller = new CourseController(client);
        }

        [Test]
        public async Task ShouldAddCourse()
        {
            // Given
            var course = BuildCourse();

            // When 
            var result = await _controller.Post(course);

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(course.Id.ToString());
        }

        [Test]
        public async Task ShouldGetCourse()
        {
            // Given
            var expectedCourse = BuildCourse();
            var postResult = await _controller.Post(expectedCourse);
            var id = (postResult.Result as OkObjectResult).Value;

            // When 
            var result = await _controller.Get(id.ToString());

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedCourse);
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
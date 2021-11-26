using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using static RandomStringUtils.RandomStringUtils;

namespace Course
{
    public class CourseControllerTest
    {
        private CourseController _controller;
        private Mock<ICourseRepository> _mockRepository;

        [SetUp]
        public void setupController()
        {
            _mockRepository = new Mock<ICourseRepository>();
            _controller = new CourseController(_mockRepository.Object);
        }

        [Test]
        public async Task ShouldAddCourse()
        {
            // Given
            var course = BuildCourse();
            _mockRepository.Setup(r => r.Save(It.IsAny<Course>()))
                .ReturnsAsync(course.Id.ToString());

            // When 
            var result = await _controller.Post(course);

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(course.Id.ToString());
            _mockRepository.Verify(r => r.Save(course), Times.Once);
        }

        [Test]
        public async Task ShouldNotAddCourse()
        {
            // Given
            var course = BuildCourse();
            _mockRepository.Setup(r => r.Save(It.IsAny<Course>()));

            // When 
            var result = await _controller.Post(course);

            // Then
            result.Result.Should().BeOfType(typeof(NotFoundResult));
            _mockRepository.Verify(r => r.Save(course), Times.Once);
        }

        [Test]
        public async Task ShouldGetCourse()
        {
            // Given
            var expectedCourse = BuildCourse();
            _mockRepository.Setup(r => r.FindById(It.IsAny<string>()))
                .ReturnsAsync(expectedCourse);

            // When 
            var result = await _controller.Get(expectedCourse.Id.ToString());

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mockRepository.Verify(r => r.Save(expectedCourse), Times.Once);
        }

        [Test]
        public async Task ShouldFindAllCourses()
        {
            // Given
            var course1 = BuildCourse();
            var course2 = BuildCourse();
            var expectedCourses = new ReadOnlyCollection<Course>(new List<Course>() {course1, course2});

            _mockRepository.Setup(r => r.FindAll())
                .ReturnsAsync(expectedCourses);
            // When 
            var result = await _controller.Search(String.Empty);

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Equals(expectedCourses);

            _mockRepository.Verify(r => r.FindAll(), Times.Once);
        }

        [Test]
        public async Task ShouldFindCourseByName()
        {
            // Given
            var course = BuildCourse();
            var expectedCourses = new ReadOnlyCollection<Course>(new List<Course>() {course});

            _mockRepository.Setup(r => r.FindByName(It.IsAny<string>()))
                .ReturnsAsync(expectedCourses);

            // When 
            var result = await _controller.Search(course.Name);

            // Then
            result.Result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Equals(expectedCourses);

            _mockRepository.Verify(r => r.FindByName(course.Name), Times.Once);
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
    }
}
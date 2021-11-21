using FluentAssertions;
using NUnit.Framework;

namespace Course
{
    public class CourseTest
    {
        [Test]
        public void ShouldInitWithGuid()
        {
            // When
            var course = new Course();

            // Then
            course.Id.Should().NotBeEmpty();
        }
    }
}
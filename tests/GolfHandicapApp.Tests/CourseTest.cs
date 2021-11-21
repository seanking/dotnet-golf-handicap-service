using NUnit.Framework;
using static AssertNet.Assertions;

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
            AssertThat(course.Id).IsNotNull();
        }
    }
}
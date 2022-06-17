using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace software_center.tests
{
    [TestFixture]
    public class TestHeaderParser
    {
        [Test]
        public void ShouldParseSingleColumn()
        {
            // Arrange
            var line = "Id";
            var expected = new Header("Id", 0);
            var sut = Create();
            // Act
            var result = sut.Parse(line);
            // Assert
            Expect(result)
                .To.Deep.Equal(new[] { expected });
        }

        [Test]
        public void ShouldParseMultipleColums()
        {
            // Arrange
            var line = @"
Name                         Id                         Version  Source".Trim();
            var expected = new[]
            {
                new Header("Name", 0),
                new Header("Id", 29),
                new Header("Version", 56),
                new Header("Source", 65)
            };
            expected[0].Next = expected[1];
            expected[1].Next = expected[2];
            expected[2].Next = expected[3];
            var sut = Create();

            // Act
            var result = sut.Parse(line);
            // Assert
            Expect(result)
                .To.Deep.Equal(expected);
        }


        private static HeaderParser Create()
        {
            return new();
        }
    }
}
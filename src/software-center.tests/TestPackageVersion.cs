using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace software_center.tests
{
    [TestFixture]
    public class TestPackageVersion
    {
        [TestFixture]
        public class Parsing
        {
            [Test]
            public void ShouldParseSingleDigit()
            {
                // Arrange
                // Act
                var parsed = PackageVersion.TryParse("1", out var result);
                // Assert
                Expect(parsed)
                    .To.Be.True();
                Expect(result.Major)
                    .To.Equal(1);
            }

            [Test]
            public void ShouldParseTwoDigits()
            {
                // Arrange
                // Act
                var parsed = PackageVersion.TryParse("1.2", out var result);
                // Assert
                Expect(parsed)
                    .To.Be.True();
                Expect(result)
                    .To.Deep.Equal(new
                    {
                        Major = 1,
                        Minor = 2,
                        Build = 0,
                        Revision = 0
                    });
            }

            [Test]
            public void ShouldParseThreeDigits()
            {
                // Arrange
                // Act
                var parsed = PackageVersion.TryParse("1.2.123", out var result);
                // Assert
                Expect(parsed)
                    .To.Be.True();
                Expect(result)
                    .To.Deep.Equal(new
                    {
                        Major = 1,
                        Minor = 2,
                        Build = 123,
                        Revision = 0
                    });
            }

            [Test]
            public void ShouldParseFourDigits()
            {
                // Arrange
                // Act
                var parsed = PackageVersion.TryParse("1.2.123.666", out var result);
                // Assert
                Expect(parsed)
                    .To.Be.True();
                Expect(result)
                    .To.Deep.Equal(new
                    {
                        Major = 1,
                        Minor = 2,
                        Build = 123,
                        Revision = 666
                    });
            }

            [TestCase("+")]
            [TestCase("_")]
            public void ShouldParseFourDigitsWithAltDivider(string d)
            {
                // Arrange
                // Act
                var parsed = PackageVersion.TryParse($"1.2.123{d}666", out var result);
                // Assert
                Expect(parsed)
                    .To.Be.True();
                Expect(result)
                    .To.Deep.Equal(new
                    {
                        Major = 1,
                        Minor = 2,
                        Build = 123,
                        Revision = 666
                    });
            }
        }

        [TestFixture]
        public class Comparison
        {
            [Test]
            public void ShouldBeAbleToTestEquality()
            {
                // Arrange
                var first = PackageVersion.Parse("1.2.3.4");
                var copy = PackageVersion.Parse("1.2.3.4");
                var other = PackageVersion.Parse("1.2.3.5");

                // Act
                // Assert
                Expect(first.Equals(copy))
                    .To.Be.True();
                Expect(first == copy)
                    .To.Be.True();
                Expect(first.Equals(other))
                    .To.Be.False();
                Expect(first == other)
                    .To.Be.False();
            }

            [Test]
            public void ShouldNotParseInvalidString()
            {
                // Arrange
                
                // Act
                var parsed = PackageVersion.TryParse("Unknown", out var result);
                
                // Assert
                Expect(parsed)
                    .To.Be.False();
                Expect(result)
                    .To.Be.Null();
            }

            [Test]
            public void ShouldBeAbleToTestGreaterThan()
            {
                // Arrange
                var a = PackageVersion.Parse("1.2.3.4");
                var a1 = a.Clone();
                var b = PackageVersion.Parse("1.2.3.5");
                var c = PackageVersion.Parse("1.4.1.1");
                var d = PackageVersion.Parse("2.0.0");

                // Assert
                Expect(a > b)
                    .To.Be.False();
                Expect(b > a)
                    .To.Be.True();
                Expect(a >= a1)
                    .To.Be.True();
                Expect(c > a)
                    .To.Be.True();
                Expect(d > c)
                    .To.Be.True();
            }
        }
    }
}
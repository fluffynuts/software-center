using System;
using System.Linq;
using NUnit.Framework;
using NExpect;
using NExpect.Implementations;
using static NExpect.Expectations;

namespace software_center.tests
{
    [TestFixture]
    public class TestTabularDataParser
    {
        [Test]
        public void ShouldParseSearchQuery()
        {
            // Arrange
            // partial output from `winget search foo`
            var data = @"
Name                         Id                         Version  Source
-------------------------------------------------------------------------
Beirut Airport               9WZDNCRDHP47               Unknown  msstore
Audit Manager – Bolton Food  9P7F3JG9B0GM               Unknown  msstore
COL Food Weighing            9NBLGGH1R87G               Unknown  msstore
Canada: Wild Food            9WZDNCRDFZKN               Unknown  msstore
SnakeTail                    snakefoot.snaketail        1.9.7.0  winget
foobar2000                   PeterPawlowski.foobar2000  1.6.11   winget
Perfoo                       LAIPIC.Perfoo              1.0.5    winget
Greenfoot                    GreenfootTeam.Greenfoot    3.7.0    winget
NoteTab Light                FookesHolding.NoteTabLight 7.2      winget
PuTTY CAC                    NoMoreFood.PuTTY-CAC       0.77.0.0 winget
".Trim();
            var sut = Create();
            // Act
            var result = sut.Parse<PackageInfo>(data)
                .ToArray();

            // Assert
            Expect(result)
                .To.Contain.Only(10)
                .Items();

            Expect(result)
                .To.Contain.Exactly(1)
                .Matched.By(o =>
                    o.Name == "SnakeTail" &&
                    o.Id == "snakefoot.snaketail" &&
                    o.Version == PackageVersion.Parse("1.9.7.0") &&
                    o.Available is null &&
                    o.Source == "winget"
                );

            Expect(result)
                .To.Contain.Exactly(1)
                .Matched.By(o =>
                    o.Name == "COL Food Weighing" &&
                    o.Version is null &&
                    o.Available is null &&
                    o.Source == "msstore"
                );
        }

        [Test]
        public void ShouldParseUpgradeOutput()
        {
            // Arrange
            var data = @"
Name                                Id                                     Version       Available     Source
-------------------------------------------------------------------------------------------------------------
Humble App 1.1.0+321                HumbleBundle.HumbleApp                 1.1.0+321     1.1.1+341     winget
Microsoft Edge                      Microsoft.Edge                         102.0.1245.41 102.0.1245.44 winget
Vim 8.2 (x64)                       vim.vim                                8.2.5096      8.2.5114      winget
Visual Studio Build Tools 2022      Microsoft.VisualStudio.2022.BuildTools 17.2.3        17.2.4        winget
Slack                               SlackTechnologies.Slack                4.26.3        4.27.154      winget
Microsoft Visual Studio Code (User) Microsoft.VisualStudioCode             1.68.0        1.68.1        winget
6 upgrades available.
3 packages have version numbers that cannot be determined. Using ""--include-unknown"" may show more results.
";
            var sut = Create();
            // Act
            var result = sut.Parse<PackageInfo>(data)
                .ToArray();
            // Assert
            Expect(result)
                .To.Contain.Only(6)
                .Items(
                    () => $"parser should ignore lines which don't conform to the tabular data rules\n{result.Stringify()}"
                );
            Expect(result)
                .To.Contain.Exactly(1)
                .Matched.By(o =>
                    o.Name == "Microsoft Edge" &&
                    o.Id == "Microsoft.Edge" &&
                    o.Version == PackageVersion.Parse("102.0.1245.41") &&
                    o.Available == PackageVersion.Parse("102.0.1245.44") &&
                    o.Source == "winget"
                );
        }

        private static IParser Create()
        {
            return new TabularDataParser(
                new HeaderParser(),
                new AutoConverter()
            );
        }
    }
}
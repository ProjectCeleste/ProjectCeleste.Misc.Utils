using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace ProjectCeleste.Misc.Utils.Tests.XmlUtils
{
    public class Deserialization : IDisposable
    {
        private string _basePath;

        public Deserialization()
        {
            _basePath = $"Deserialization-TestRun-{Guid.NewGuid()}";

            Directory.CreateDirectory(_basePath);
        }

        public void Dispose()
        {
            Directory.Delete(_basePath, true);
        }

        public class DeserializeFromFile : Deserialization
        {
            [Fact]
            public void ReturnsNullWhenFileIsMissing()
            {
                var result = Utils.XmlUtils.DeserializeFromFile<SampleXmlData>("missing file.xml");

                result.Should().BeNull();
            }

            [Fact]
            public void ReturnsNullWhenFileIsEmpty()
            {
                var pathToEmptyXmlFile = $"{_basePath}\\data.xml";
                File.WriteAllText(pathToEmptyXmlFile, "");

                var result = Utils.XmlUtils.DeserializeFromFile<SampleXmlData>(pathToEmptyXmlFile);

                result.Should().BeNull();
            }

            [Fact]
            public void ReturnsExpectedObjectFromFile()
            {
                var pathToXmlFile = $"{_basePath}\\data.xml";
                var xmlContent = "<data content=\"🧙\">\r\n  <element amount=\"1\" />\r\n</data>";
                File.WriteAllText(pathToXmlFile, xmlContent);

                var result = Utils.XmlUtils.DeserializeFromFile<SampleXmlData>(pathToXmlFile);

                result.Should().NotBeNull();
                result.Content.Should().Be("🧙");
                result.Element.Should().NotBeNull();
                result.Element.Amount.Should().Be(1);
            }
        }

        public class DeserializeFromString : Deserialization
        {
            [Fact]
            public void ReturnsNullWhenStringIsNull()
            {
                var result = Utils.XmlUtils.DeserializeFromString<SampleXmlData>(null); 
                result.Should().BeNull();
            }

            [Fact]
            public void ReturnsNullWhenStringIsEmpty()
            {
                var result = Utils.XmlUtils.DeserializeFromString<SampleXmlData>("");
                result.Should().BeNull();
            }

            [Fact]
            public void ReturnsExpectedObject()
            {
                var xmlContent = "<data content=\"🧙\">\r\n  <element amount=\"1\" />\r\n</data>";

                var result = Utils.XmlUtils.DeserializeFromString<SampleXmlData>(xmlContent);

                result.Should().NotBeNull();
                result.Content.Should().Be("🧙");
                result.Element.Should().NotBeNull();
                result.Element.Amount.Should().Be(1);
            }
        }
    }
}

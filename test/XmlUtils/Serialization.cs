using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ProjectCeleste.Misc.Utils.Tests.XmlUtils
{
    public class Serialization : IDisposable
    {
        private string _basePath;

        public Serialization()
        {
            _basePath = $"Serialization-TestRun-{Guid.NewGuid()}";

            Directory.CreateDirectory(_basePath);
        }

        public void Dispose()
        {
            Directory.Delete(_basePath, true);
        }

        public class SerializeToXmlFile : Serialization
        {
            [Fact]
            public void BacksUpExistingXmlFile()
            {
                // Arrange
                var xmlFilePath = $"{_basePath}\\file.xml";
                var xmlFileBackupPath = $"{xmlFilePath}.bak";
                var expectedBackupContents = "ExistingBackupContent";

                var objectToSerialize = new SampleXmlData { Content = "foo" };


                File.WriteAllText(xmlFilePath, expectedBackupContents);

                // Act
                Utils.XmlUtils.SerializeToXmlFile(objectToSerialize, xmlFilePath, true);

                // Assert
                var backupFileExists = File.Exists(xmlFileBackupPath);
                backupFileExists.Should().BeTrue();

                var backupFileContents = File.ReadAllText(xmlFileBackupPath);
                backupFileContents.Should().Be(expectedBackupContents);
            }

            [Fact]
            public void NewBackupOverwritesOldXmlBackupFile()
            {
                // Arrange
                var xmlFilePath = $"{_basePath}\\file.xml";
                var xmlFileBackupPath = $"{xmlFilePath}.bak";
                var previousBackupContents = "PreviousBackupContent";
                var newBackupContents = "NewBackupContent";

                var objectToSerialize = new SampleXmlData { Content = "foo" };

                File.WriteAllText(xmlFileBackupPath, previousBackupContents);
                File.WriteAllText(xmlFilePath, newBackupContents);

                // Act
                Utils.XmlUtils.SerializeToXmlFile(objectToSerialize, xmlFilePath, true);

                // Assert
                var backupFileExists = File.Exists(xmlFileBackupPath);
                backupFileExists.Should().BeTrue();

                var backupFileContents = File.ReadAllText(xmlFileBackupPath);
                backupFileContents.Should().Be(newBackupContents);
            }

            [Fact]
            public void SerializesExpectedData()
            {
                // Arrange
                var xmlFilePath = $"{_basePath}\\file.xml";
                var objectToSerialize = new SampleXmlData { Content = "🧙", Element = new SampleXmlElement { Amount = 1 } };

                // Act
                Utils.XmlUtils.SerializeToXmlFile(objectToSerialize, xmlFilePath);

                // Assert
                var fileContents = File.ReadAllText(xmlFilePath);
                fileContents.Should().Be("<data content=\"🧙\">\r\n  <element amount=\"1\" />\r\n</data>");
            }
        }

        public class SerializeToString : Serialization
        {
            [Fact]
            public void ReturnsNullIfObjectIsNull()
            {
                Utils.XmlUtils.SerializeToString(null).Should().BeNull();
            }

            [Fact]
            public void ReturnsExpectedXmlData()
            {
                // Arrange
                var objectToSerialize = new SampleXmlData { Content = "🧙", Element = new SampleXmlElement { Amount = 1 } };

                // Act
                var xml = Utils.XmlUtils.SerializeToString(objectToSerialize);

                // Assert
                xml.Should().Be("<data content=\"🧙\">\r\n  <element amount=\"1\" />\r\n</data>");
            }
        }
    }
}

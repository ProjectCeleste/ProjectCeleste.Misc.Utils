﻿using Celeste.Misc.Utils;
using FluentAssertions;
using ProjectCeleste.Misc.Utils;
using System.Xml.Serialization;
using Xunit;

namespace ProjectCeleste.Misc.Helper.Tests
{
    public class SerializableStringListTests
    {
        [Fact]
        public void CanSerializeFromXml()
        {
            // Arrange
            var listItems = new[] { "foo", "don", "bar" };
       
            var xmlSerializedObj = new SampleXmlDataWithString
            {
                Content = listItems.ToStringList()
            };

            var expected = new SampleXmlDataWithWrapperType
            {
                Content = new SerializableStringList(listItems)
            };

            var xml = xmlSerializedObj.ToXml();

            // Act
            var deserializedObj = XmlUtils.DeserializeFromString<SampleXmlDataWithWrapperType>(xml);

            // Assert
            deserializedObj.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void CanSerializeToXml()
        {
            // Arrange
            var listItems = new[] { "foo", "don", "bar" };
            var obj = new SampleXmlDataWithWrapperType
            {
                Content = new SerializableStringList(listItems)
            };

            var expected = new SampleXmlDataWithString
            {
                Content = listItems.ToStringList()
            };

            // Act
            var serialized = obj.ToXml();

            // Assert
            serialized.Should().Be(expected.ToXml());
        }

        [XmlRoot(ElementName = "data")]
        public class SampleXmlDataWithWrapperType
        {
            [XmlText]
            public string ContentStr {
                get => Content.SerializeList();
                set
                {
                    Content = new SerializableStringList(value);
                }
            }

            [XmlIgnore]
            public SerializableStringList Content { get; set; }
        }

        [XmlRoot(ElementName = "data")]
        public class SampleXmlDataWithString
        {
            [XmlText]
            public string Content { get; set; }
        }
    }
}

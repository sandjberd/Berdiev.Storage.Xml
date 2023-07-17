//Copyright by Sandjar Berdiev

using Berdiev.Storage.Xml.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Berdiev.Storage.Xml.Internal.Deserializer;
using Berdiev.Storage.Xml.Internal.Serializer;

namespace Berdiev.Storage.Xml.Tests
{
    [TestFixture]
    public class RepositoryDev_Test
    {
        private DirectoryInfo _testDirectory;
        private IRepository _repository;

        [SetUp]
        public async Task Setup()
        {
            _CreateTestDirectory();

            var spec = new RepositorySpecification("Test", Path.Combine(_testDirectory.FullName, "Test_Repository.txt"), Version.Parse("1.0.0"));

            _repository = await RepositoryFactory.CreateRepositoryAsync(spec).ConfigureAwait(false);
        }

        [TearDown]
        public void Teardown()
        {
            _testDirectory.Delete(true);
        }

        [Test]
        public async Task Can_AppendTypedRecord()
        {
            var ser = new Serializable
            {
                Val = 1,
                Sub = new SubSerializable
                {
                    Name = "SubNameFoo"

                },

                Subs = new List<SubSerializable>
                {
                    new SubSerializable {Name = "Foo1"},
                    new SubSerializable {Name = "Foo2"},
                    new SubSerializable {Name = "Foo3"},
                    new SubSerializable {Name = "Foo4"},
                    new SubSerializable {Name = "Foo5"},
                    new SubSerializable {Name = "Foo6"},
                },

                //Ip = new IPAddress(0x2414188f)
            };

            await _repository.AppendRecordAsync(Record<Serializable>.Create("RootRecord", ser)).ConfigureAwait(false);

            var records = await _repository.GetRecordsAsync().ConfigureAwait(false);

            var t = records;

            var deserializer = RecordDeserializerService.Create();

            deserializer.Register();

            var s = deserializer.Deserialize<Serializable>(records, "RootRecord");

            var ff = (Serializable) s;
        }

        [Test]
        public async Task Can_Append()
        {
            await Task.Run(async () =>
                await _repository.AppendRecordAsync(Record.Create("MyTest", "MyFollowingTest",
                    RecordAttribute.From("Id", "Foo"))).ConfigureAwait(false)).ConfigureAwait(false);

            await _repository.AppendRecordAsync(Record.Create("MyTest", "MyFollowingTest", RecordAttribute.From("Id", "Foo"))).ConfigureAwait(false);

            await _repository.AppendRecordAsync(Record.CreateWithChildren("Secondrecord", new[]
            {
                Record.Create("Child", "ChildValue"),
                Record.Create("Child", "ChildValue"),
                Record.Create("Child", "ChildValue"),
                Record.Create("Child", "ChildValue"),
                Record.Create("Child", "ChildValue")
            })).ConfigureAwait(false);

            var records = await _repository.GetRecordsAsync().ConfigureAwait(false);

            Assert.AreEqual(3, records.Count);
            Assert.AreEqual(5, records[2].InnerRecords.Count);
        }

        private void _CreateTestDirectory()
        {
            var testDirectoryName = Guid.NewGuid().ToString("N");

            var mainDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory()));

            _testDirectory = mainDir.CreateSubdirectory(testDirectoryName);
        }

        private class Serializable : IRecordSerializable
        {
            public int Val { get; set; }

            public List<int> Numbers => new List<int>
            {
                1,2,3,4
            };

            public List<String> Texts => new List<String>
            {
                "T1", "T2"
            };

            public List<char> Chars => new List<char>
            {
                'f', 'u', 'c', 'k'
            };

            public IPAddress Ip { get; set; }

            public SubSerializable Sub { get; set; }

            public List<SubSerializable> Subs { get; set; }

            public Dictionary<SubSerializable, String> MyDict => new Dictionary<SubSerializable, string>
            {
                {new SubSerializable
                {
                    Name = "1dict"
                }, "Foodict1" },

                {new SubSerializable
                {
                    Name = "2dict"
                }, "Foodict2" },

                {new SubSerializable
                {
                    Name = "3dict"
                }, "Foodict3" },

            };
        }

        private class SubSerializable : IRecordSerializable
        {
            public string Name { get; set; }
        }
    }
}
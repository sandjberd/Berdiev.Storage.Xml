//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Berdiev.Storage.Xml.Internal;
using NUnit.Framework;

namespace Berdiev.Storage.Xml.Tests
{
    [TestFixture]
    public class Repository_Test
    {
        private DirectoryInfo _testDirectory;
        private IRepository _repository;

        [SetUp]
        public async Task Setup()
        {
            _CreateTestDirectory();

            var spec = new RepositorySpecification("Test", Path.Combine(_testDirectory.FullName, "Test_Repository.xml"), Version.Parse("1.0.0"));

            _repository = await RepositoryFactory.CreateRepositoryAsync(spec).ConfigureAwait(false);
        }

        [TearDown]
        public void Teardown()
        {
            _testDirectory.Delete(true);
        }

        [Test]
        public async Task CanDelete()
        {
            var record1 = Record.Create("Foo", "Bar");
            var record2 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "1"));
            var record3 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "2"), RecordAttribute.From("Size", "1"));

            await _repository.AppendRecordAsync(record1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(record2).ConfigureAwait(false);
            await _repository.AppendRecordAsync(record3).ConfigureAwait(false);


            await _repository.DeleteAsync(record2).ConfigureAwait(false);

            var records = await _repository.GetRecordsAsync().ConfigureAwait(false);

            Assert.AreEqual(2, records.Count);
        }

        [Test]
        public async Task CanUpdate()
        {
            var record1 = Record.Create("Foo", "Bar");
            var record2 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "1"));
            var record3 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "2"), RecordAttribute.From("Size", "1"));

            await _repository.AppendRecordAsync(record1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(record2).ConfigureAwait(false);
            await _repository.AppendRecordAsync(record3).ConfigureAwait(false);


            await _repository.UpdateRecordAsync(record2, record1).ConfigureAwait(false);
        }

        [Test]
        public async Task CanDeleteStructured()
        {
            var childRecord = Record.Create("Child", "I am a child");

            var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
            var parentRecord2 = Record.CreateWithChildren("Parent", new[] { childRecord, childRecord }, RecordAttribute.From("ParentId", "2"));

            await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);

            await _repository.DeleteAsync(parentRecord2).ConfigureAwait(false);
            
            var records = await _repository.GetRecordsAsync().ConfigureAwait(false);

            Assert.AreEqual(1, records.Count);

        }

        [Test]
        public async Task CanUpdateStructured()
        {
            var childRecord = Record.Create("Child", "I am a child");
            var childRecord2 = Record.Create("Child", "I am an updated child");

            var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
            var parentRecord2 = Record.CreateWithChildren("Parent", new[] { childRecord, childRecord }, RecordAttribute.From("ParentId", "2"));

            var parentRecord3 = Record.CreateWithChildren("Parent", new[] { childRecord2 }, RecordAttribute.From("ParentId", "2"));

            await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);

            await _repository.UpdateRecordAsync(parentRecord2, parentRecord3).ConfigureAwait(false);

        }

        [Test]
        public async Task CanCreateStructured()
        {
            var childRecord = Record.Create("Child", "I am a child");
            
            var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
            var parentRecord2 = Record.CreateWithChildren("Parent", new []{childRecord, childRecord}, RecordAttribute.From("ParentId", "2"));

            await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);
        }

        [Test]
        public async Task CanRetrieveTasks()
        {
            var childRecord = Record.Create("Child", "I am a child", RecordAttribute.From("ChildId", "1"));

            var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
            var parentRecord2 = Record.CreateWithChildren("Parent", new[] { childRecord, childRecord }, RecordAttribute.From("ParentId", "2"));

            await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
            await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);
            await _repository.AppendRecordAsync(Record.Create("child", "Moof")).ConfigureAwait(false);

            //Query test
            var allRecords = await _repository.GetRecordsAsync().ConfigureAwait(false);

            var recordsThatContain1InAttributes = new List<IRecord>();
            var recordsThatContain1InAttributes_ = allRecords.WhereRecord(r =>
                r.Attributes.Any(attribute => attribute.AttributeValue.Equals("1", StringComparison.Ordinal)));
            
            foreach (var record in allRecords)
            {
                var attr1 = record.WhereRecord(r =>
                {
                    return r.Attributes.Any(attribute => attribute.AttributeValue.Equals("1", StringComparison.Ordinal));
                });

                recordsThatContain1InAttributes.AddRange(attr1);
            }

            Assert.AreEqual(4, recordsThatContain1InAttributes.Count);
            Assert.AreEqual(4, recordsThatContain1InAttributes_.ToList().Count);

            //Repository test

            var foundChildRecords = await _repository.GetRecordsByNameAsync("Child").ConfigureAwait(false);
            var foundChildRecordsSmall = await _repository.GetRecordsByNameAsync("child").ConfigureAwait(false);

            Assert.AreEqual(1, foundChildRecordsSmall.Count);
            Assert.AreEqual(3, foundChildRecords.Count);
        }

        [Test]
        public async Task Can_AppendTypedRecord()
        {
            var ser = new DummySerialization()
            {
                Date = DateTime.UtcNow,
                TimeSpan = TimeSpan.FromSeconds(10),
                Version = new Version(1, 0, 0),
                Name = "Foo",
                Id = Guid.NewGuid(),
                IsCool = true,
                Inception = new List<List<int>>
                {
                    new List<int>
                    {
                        1,2,3,4,5,66,7777,52,0,88,999
                    },
                    new List<int>
                    {
                        1,2,3,4,5,66,
                    },
                    new List<int>
                    {
                        66,7777,52,0,88,999,123,1,2,3
                    }
                }
            };

            var children = new List<DummySubSerialization>();

            for (int i = 0; i < 7; i++)
            {
                var x = new DummySubSerialization
                {
                    MyDictionary = new Dictionary<Guid, string>
                    {
                        {Guid.NewGuid(), ser.Name},
                        {Guid.NewGuid(), ser.Name},
                        {Guid.NewGuid(), ser.Name},
                        {Guid.NewGuid(), ser.Name},
                        {Guid.NewGuid(), ser.Name},
                        {Guid.NewGuid(), "<f>hhhh</f>"},
                    },

                    NuDict = new Dictionary<Version, DateTime>
                    {
                        {Version.Parse("1.0.0"), DateTime.UtcNow},
                        {Version.Parse("1.1.0"), DateTime.UtcNow},
                        {Version.Parse("1.0.2"), DateTime.UtcNow},
                    }
                };

                children.Add(x);

            }

            ser.Children = children;

            await _repository.AppendRecordAsync(Record<DummySerialization>.Create("RootRecord", ser)).ConfigureAwait(false);

            var records = await _repository.GetRecordsAsync().ConfigureAwait(false);

            var deserializedObject = await _repository.LoadDataAsync<DummySerialization>(records, "RootRecord").ConfigureAwait(false);

            Assert.AreEqual(ser.Date, deserializedObject.Date);
            Assert.AreEqual(ser.TimeSpan, deserializedObject.TimeSpan);
            Assert.AreEqual(ser.Version, deserializedObject.Version);
            Assert.AreEqual(ser.Name, deserializedObject.Name);
            Assert.AreEqual(ser.Id, deserializedObject.Id);
            Assert.AreEqual(ser.Inception, deserializedObject.Inception);

            Assert.AreEqual(deserializedObject.Children.Count, 7);
            Assert.AreEqual(deserializedObject.Children[0].MyDictionary.Count, 6);
            Assert.AreEqual(deserializedObject.Children[0].NuDict.Count, 3);

        }

        private void _CreateTestDirectory()
        {
            var testDirectoryName = Guid.NewGuid().ToString("N");

            var mainDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory()));

            _testDirectory = mainDir.CreateSubdirectory(testDirectoryName);
        }
    }
}
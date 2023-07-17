# Berdiev.Storage.Xml
playground for XML file content handling (... 2019)

### Create a repository

The repository is the entry point for storing and loading records. The repository is represented by a XML file.

```
var spec = new RepositorySpecification("Test", "MyFirstRepository.xml", Version.Parse("1.0.0"));

var repository = await RepositoryFactory.CreateRepositoryAsync(spec).ConfigureAwait(false);
```

### Create some records

```
var record1 = Record.Create("Foo", "Bar");
var record2 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "1"));
var record3 = Record.Create("WithAttribute", "Bar", RecordAttribute.From("Id", "2"), RecordAttribute.From("Size", "1"));
```

### Append records
```
await repository.AppendRecordAsync(record1).ConfigureAwait(false);
await repository.AppendRecordAsync(record2).ConfigureAwait(false);
await repository.AppendRecordAsync(record3).ConfigureAwait(false);
```

![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/0c46abdc-834e-4400-80d0-65f1812124c0)



### Retrieve records

You can get all records from the repository.

`var records = await _repository.GetRecordsAsync().ConfigureAwait(false);`

It is also possible to get records filtered by name.

`var filteredRecords = await _repository.GetRecordsByNameAsync("Foo").ConfigureAwait(false);`

If this is not enough you can query the children of a single record or a sequence of records.

```
var allRecords = await _repository.GetRecordsAsync().ConfigureAwait(false);

var recordsThatContain_1_InAttributes = allRecords.WhereRecord(r =>
                r.Attributes.Any(attribute => attribute.AttributeValue.Equals("1", StringComparison.Ordinal))
);

// recordsThatContain_1_InAttributes contains all records that has "1" as value in attributes
```

Or you can also query a single record

```
var allRecords = await _repository.GetRecordsAsync().ConfigureAwait(false);
var recordsThatContain_1_InAttributes = new List<IRecord>();

foreach (var record in allRecords)
{
    var attr1 = record.WhereRecord(r =>
    {
        return r.Attributes.Any(attribute => attribute.AttributeValue.Equals("1", StringComparison.Ordinal));
    });

    recordsThatContain_1_InAttributes.AddRange(attr1);
}

// recordsThatContain_1_InAttributes contains all records that has "1" as value in attributes
```

### Delete Record

`await _repository.DeleteAsync(record2).ConfigureAwait(false);`

![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/ed92fbc0-d33b-4093-ab79-e50ef005fc5a)


### Update Record

`await _repository.UpdateRecordAsync(record2, record1).ConfigureAwait(false);`

![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/6026335b-a820-4ff8-b010-4187297e8e5a)


## Structured Records

### Create structured records
```
var childRecord = Record.Create("Child", "I am a child");
var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
var parentRecord2 = Record.CreateWithChildren("Parent", new []{childRecord, childRecord}, 
RecordAttribute.From("ParentId", "2"));

await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);
```
![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/92dc6673-012a-48ab-ac70-b9f5c14b6bd2)


## Delete structured records

`await _repository.DeleteAsync(parentRecord2).ConfigureAwait(false);`

![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/cdcacb2f-c88a-4dc5-949c-8bdd8ed238ff)


__ATTENTION:_ If you delete a record that is not distinct this will also affect the XML file._

`await _repository.DeleteAsync(childRecord).ConfigureAwait(false);`

![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/8a358eda-4a5f-4b4b-8c41-172f700af806)


## Update structured records
```
var childRecord = Record.Create("Child", "I am a child");
var childRecord2 = Record.Create("Child", "I am an updated child");

var parentRecord1 = Record.CreateWithChild("Parent", childRecord, RecordAttribute.From("ParentId", "1"));
var parentRecord2 = Record.CreateWithChildren("Parent", new []{childRecord, childRecord}, 
RecordAttribute.From("ParentId", "2"));

var parentRecord3 = Record.CreateWithChildren("Parent", new[] { childRecord2 }, RecordAttribute.From("ParentId", "2"));

await _repository.AppendRecordAsync(parentRecord1).ConfigureAwait(false);
await _repository.AppendRecordAsync(parentRecord2).ConfigureAwait(false);

await _repository.UpdateRecordAsync(parentRecord2, parentRecord3).ConfigureAwait(false);
```
![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/5843fffe-93b2-4a61-b1b9-fa466e90ef64)


## Serialization / Deserialization

### Serialize a class

The entry class must be marked with `IRecordSerializable`.

```
public class DummySerialization : IRecordSerializable
{
    public String Name { get; set; }

    public bool IsCool { get; set; }

    public Version Version { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan TimeSpan { get; set; }

    public Guid Id { get; set; }

    public Guid SubId { get; set; }

    public IReadOnlyList<DummySubSerialization> Children { get; set; }

    public IReadOnlyList<List<int>> Inception { get; set; }
}
```

```
public class DummySubSerialization : IRecordSerializable
{
    public Dictionary<Guid, String> MyDictionary { get; set; }

    public Dictionary<Version, DateTime> NuDict { get; set; }

    public List<int> Numbers { get; set; }
}
```

Now create a dummy serializable and use the repository to store the instance.

```
var ser = new DummySerialization()
{
    Date = DateTime.UtcNow,
    TimeSpan = TimeSpan.FromSeconds(10),
    .......
}

await _repository.AppendRecordAsync(Record<DummySerialization>.Create("RootRecord", ser)).ConfigureAwait(false);

```
![image](https://github.com/sandjberd/Berdiev.Storage.Xml/assets/13351348/fd126444-47b8-442e-a442-b4b12c6262a9)


### Deserialize a record

At first you have to get all records

`var records = await _repository.GetRecordsAsync().ConfigureAwait(false);`

You can deserialize the whole instance of DummySerialization

`var deserializedObject = await _repository.LoadDataAsync<DummySerialization>(records, "RootRecord").ConfigureAwait(false);`

You can also deserialize a part of the serialized instance

`var subId= await _repository.LoadDataAsync<Guid>(records, "SubId").ConfigureAwait(false);`

Veeeery basic POC of something that was bothering me.

I wanted to be able to describe the mapping between an Excel cell (while parsing a worksheet) and some target property of an unrelated class. For example:

- Map a name from a cell (a string) to a property of type string in a class - a direct assignment.
- Map an age (written as a string) to a property of type integer in a class - requiring a parse.
- Map a DateTime (possibly written as a string in an unsupported format string) to a property of type DateTime in a class - requiring more custom logic.

Core to the notion is a basic "column definition", that describes the relationship between the cells in a column (which are assumed to all be of the same type) and a target property of a class that represents a row of cells (representing a single entity/instance). A sort of deserialization.

I wanted to be able to do something like this:

```csharp
class Person
{
  public string Name;
  public int Age;
  public DateTime BirthDate;
}

var personColumnDefinitions = new ColumnDefinitions<Person>();

// Describe mapping between columns and properties. Assume that Add has a bunch of overloads
// that match all supported target property types (here, string, int, and DateTime)
personColumnDefinitions.Add(x => x.Name, "Name");
personColumnDefinitions.Add(x => x.Age, "Current age");
personColumnDefinitions.Add(x => x.BirthDate, "Time of birth");

var worksheet = new Worksheet(); // This was from reading an Excel file somewhere

var people = new List<Person>();

foreach (var row in worksheet.Rows)
{
  // ParseRow instantiates a new Person object with all properties set.
  people.Add(personColumnDefinitions.ParseRow(row));
}
```
*Crucially*, I want this to be as type-safe as possible (so avoid reflection) while still removing as much boilerplate as possible (so no bespoke map delegates for every scenario).

Can't tell if this is such a normal use case that there's already a NuGet for it or if it's so niche that everyone who needs it just ends up putting together their own solution.

I think I've managed an implementation that essentially only has a single point of type unsafety, when assigning the converted value to a property in the target object. I'm fairly sure this can be resolved with the Expression API, too.

... this *has* be a well-solved problem and I've just been re-re-re-re-inventing the wheel.

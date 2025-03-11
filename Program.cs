/*var columnDefinition = new ColumnDefinition<SourceExample, TargetExample> {
  SourceProperty = x => x.Name,
  TargetProperty = x => x.Name
};*/

using System;

var nameColumnDefinition = new StringExcelColumnDefinition<TargetExample>(target => target.Name);
var ageColumnDefinition = new IntExcelColumnDefinition<TargetExample>(target => target.Age);

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var sourceExample = new SourceExample()
{
  Age = "37",
  Name = "John Doe",
};

var targetExample = new TargetExample();

var nameShim = new ExcelCellShim { DisplayValue = "John Doe" };
var ageShim = new ExcelCellShim { DisplayValue = "37" };

Console.WriteLine($"Converting source {nameShim.DisplayValue} and {ageShim.DisplayValue}");

nameColumnDefinition.Map(nameShim, targetExample);
ageColumnDefinition.Map(ageShim, targetExample);

Console.WriteLine($"Became '{targetExample.Name}' and '{targetExample.Age}'");
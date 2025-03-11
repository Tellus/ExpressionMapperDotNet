using System.Linq.Expressions;
using System.Reflection;

/// We want to be able to do this:
/// new ColumnDefinition {
///  SourceProperty = x => x.Name,
///  TargetProperty = x => x.Name
/// }
/// 
public interface IColumnDefinition<TSource, TTarget, TSourceProperty, TTargetProperty>
{
  public void Map(TSource sourceObject, TTarget targetObject);
}

public abstract class ColumnDefinition<TSource, TTarget, TSourceProperty, TTargetProperty> : IColumnDefinition<TSource, TTarget, TSourceProperty, TTargetProperty>
{
  protected ColumnDefinition(
    Expression<Func<TSource, TSourceProperty>> sourceProperty,
    Expression<Func<TTarget, TTargetProperty>> targetProperty
  )
  {
    if (sourceProperty.Body is MemberExpression)
      SourceProperty = sourceProperty;
    else
    {
      throw new ArgumentException($"{nameof(sourceProperty)} should be a member expression");
    }
    
    if (targetProperty.Body is MemberExpression)
      TargetProperty = targetProperty;
    else
    {
      throw new ArgumentException($"{nameof(targetProperty)} should be a member expression");
    }
  }
  
  protected Expression<Func<TSource, TSourceProperty>> SourceProperty { get; init; }
  protected Expression<Func<TTarget, TTargetProperty>> TargetProperty { get; init; }
  
  public MemberExpression TargetPropertyExpression => TargetProperty.Body as MemberExpression ?? throw new ArgumentException($"Expression '{SourceProperty}' is not a member expression");
  
  /// <summary>
  /// Retrieves the source object's property's value by compiling the expression and invoking it.
  /// </summary>
  /// <param name="sourceObject"></param>
  /// <returns></returns>
  public TSourceProperty GetSourceValue(TSource sourceObject) => SourceProperty.Compile()(sourceObject);
  
  public abstract TTargetProperty GetSourceValueAsTargetType(TSource sourceObject);
  
  public virtual void Map(TSource sourceObject, TTarget targetObject)
  {
    if (TargetPropertyExpression.Member is PropertyInfo propertyInfo)
    {
      // FIXME: this is the only place (?) that we don't have type safety. Can we improve it?
      var newValue = GetSourceValueAsTargetType(sourceObject);
      
      propertyInfo.SetValue(targetObject, newValue);
    }
    else
    {
      throw new ArgumentException($"{nameof(TargetPropertyExpression.Member)} is not a {nameof(PropertyInfo)}");
    }
  }
}

public class ExcelCellShim
{
  public string DisplayValue { get; set; }
}

public abstract class
  ExcelColumnDefinition<TTarget, TTargetProperty>(
    Expression<Func<ExcelCellShim, string>> sourceProperty,
    Expression<Func<TTarget, TTargetProperty>> targetProperty
  ) : ColumnDefinition<ExcelCellShim, TTarget, string, TTargetProperty>(sourceProperty, targetProperty)
{

}

public class IntExcelColumnDefinition<TTarget>(
  Expression<Func<TTarget, int>> targetProperty
) : ExcelColumnDefinition<TTarget, int>(src => src.DisplayValue, targetProperty)
{
  public override int GetSourceValueAsTargetType(ExcelCellShim sourceObject) => int.Parse(SourceProperty.Compile()(sourceObject));
}

public class StringExcelColumnDefinition<TTarget>(
  Expression<Func<TTarget, string>> targetProperty
) : ExcelColumnDefinition<TTarget, string>(src => src.DisplayValue, targetProperty)
{
  public override string GetSourceValueAsTargetType(ExcelCellShim sourceObject) => sourceObject.DisplayValue;
}

public class SourceExample {
  public string Name { get; set; }
  public string Age { get; set; }
}

public class TargetExample {
  public string Name { get; set; }
  public int Age { get; set; }
}

using System.ComponentModel;
using System.Reflection;

namespace QTip.Domain.Enums;

public static class EnumExtensions
{
    public static string GetDescription(this PiiTag tag)
    {
        Type type = tag.GetType();
        string name = Enum.GetName(type, tag) ?? throw new InvalidOperationException("Enum name could not be resolved.");

        FieldInfo? field = type.GetField(name);
        if (field is null)
        {
            throw new InvalidOperationException("Enum field could not be resolved.");
        }

        DescriptionAttribute? attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        if (attribute is not null)
        {
            return attribute.Description;
        }

        return name;
    }
}


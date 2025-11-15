using System.ComponentModel;
using System.Reflection;

namespace HFastKit.Extensions;

/// <summary>
/// Enum 拓展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <param name="enumObject">枚举对象</param>
    /// <returns></returns>
    public static string GetDescription(this Enum enumObject)
    {
        Type enumType = enumObject.GetType();
        string? name = Enum.GetName(enumType, enumObject);
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        FieldInfo? fieldInfo = enumType.GetField(name);
        if (fieldInfo is null)
        {
            return string.Empty;
        }
        if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) is DescriptionAttribute attrribute)
        {
            return attrribute.Description;
        }
        return string.Empty;
    }
}


#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

#endregion

namespace DominatorHouseCore.Diagnostics.Helpers
{
    /// <summary>
    ///     Uses to dump object to string with name=value. Uses for diagnostics.
    /// </summary>
    public class ObjectDumper
    {
        private int _level;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly List<int> _hashListOfFoundElements;

        private ObjectDumper(int indentSize)
        {
            _indentSize = indentSize;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new List<int>();
        }

        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
                return _stringBuilder.ToString();
            }

            var objectType = element.GetType();
            var bIsObjectNotEnnumerable = !typeof(IEnumerable).IsAssignableFrom(objectType);
            if (bIsObjectNotEnnumerable)
            {
                Write("{{{0}}}", objectType.FullName);
                _hashListOfFoundElements.Add(element.GetHashCode());
                _level++;
            }

            var enumerableElement = element as IEnumerable;
            if (enumerableElement != null)
            {
                RecursivelyEnumerateAndDumpElements(enumerableElement);
            }
            else
            {
                var members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                RecursivelyEnumerateMemberInfoAndDumpElements(members, element);
            }

            if (bIsObjectNotEnnumerable) _level--;

            return _stringBuilder.ToString();
        }

        private void RecursivelyEnumerateAndDumpElements(IEnumerable enumerableElement)
        {
            foreach (var item in enumerableElement)
                if (item is IEnumerable && !(item is string))
                {
                    _level++;
                    DumpElement(item);
                    _level--;
                }
                else
                {
                    if (!AlreadyTouched(item))
                        DumpElement(item);
                    else
                        Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
                }
        }

        private void RecursivelyEnumerateMemberInfoAndDumpElements(MemberInfo[] members, object element)
        {
            foreach (var memberInfo in members)
            {
                if (IsMemberNotDumbable(memberInfo))
                    continue;

                var Info = GetTypeAndValue(memberInfo, element);
                var type = Info.Item1;
                var value = Info.Item2;

                if (type.IsValueType || type == typeof(string))
                {
                    Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                }
                else
                {
                    var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                    Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                    var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                    _level++;
                    if (!alreadyTouched)
                        DumpElement(value);
                    else
                        Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                    _level--;
                }
            }
        }

        private bool IsMemberNotDumbable(MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            var propertyInfo = memberInfo as PropertyInfo;

            if (fieldInfo == null && propertyInfo == null)
                return true;

            if (propertyInfo != null && Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute)))
                return true;

            return false;
        }

        private Tuple<Type, object> GetTypeAndValue(MemberInfo memberInfo, object element)
        {
            var fieldInfo = memberInfo as FieldInfo;
            var propertyInfo = memberInfo as PropertyInfo;

            var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
            var value = fieldInfo != null
                ? fieldInfo.GetValue(element)
                : propertyInfo.GetValue(element, null);
            return new Tuple<Type, object>(type, value);
        }

        private bool AlreadyTouched(object value)
        {
            if (value == null)
                return false;

            var hash = value.GetHashCode();
            for (var i = 0; i < _hashListOfFoundElements.Count; i++)
                if (_hashListOfFoundElements[i] == hash)
                    return true;
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return "null";

            if (o is DateTime)
                return ((DateTime) o).ToShortDateString();

            if (o is string)
                return string.Format("\"{0}\"", o);

            if (o is char && (char) o == '\0')
                return string.Empty;

            if (o is ValueType)
                return o.ToString();

            if (o is IEnumerable)
                return "...";

            return "{ }";
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UHSB_MasterService.ViewModels;

namespace UHSB_MasterService.Common
{
    public static class EnumHelper<T>
    {
        // to get the byte or int value of Enum
        public static int GetValueOf(string enumName)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            string enumString = "";
            string[] finalStr = new string[2];
            foreach (var field in type.GetFields())
            {
                if (field.FieldType != typeof(Int32))
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                                       typeof(DisplayAttribute)) as DisplayAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Name == enumName)
                        {
                            if (attribute == null)
                            {
                                enumString = enumName;
                            }
                            enumString = Convert.ToString(field);
                            finalStr = enumString.Split(' ');
                            object value = Enum.Parse(type, finalStr[1]);
                            return Convert.ToInt32(value);
                        }
                    }
                }
            }
            return 0;
        }


        public static string GetValueFromName(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                if (field.Name == name)
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                    if (attribute == null)
                    {
                        return name;
                    }
                    return attribute.GetName();
                }
            }

            throw new ArgumentOutOfRangeException("name");
        }

        /// <summary>
        /// Simplier version of enum helper
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(int? value)
        {
            try
            {
                var type = typeof(T);

                if (type == typeof(CommonEnum.RCMType))
                {
                    var x = 2;
                    var dd = 4;
                    x = dd;
                }

                if (!type.IsEnum) throw new InvalidOperationException();

                if (value == null || value == 0)
                {
                    return "";
                }

                var myEnum = GetEnum(value);
                var output = "";

                var member = type.GetMember(myEnum.ToString());

                var attribute = Attribute.GetCustomAttribute(member[0],
                        typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute == null)
                {
                    output = Enum.GetName(type, value);
                }
                else
                {
                    output = attribute.GetName();
                }
                return output;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return "";
            }

        }

        public static string GetValueFromName2(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (field.Name == name || field.Name.Contains(name))
                    {
                        //return (T)field.GetValue(null);
                        return attribute.GetName();
                    }
                }
                //else
                //{
                //    if (field.Name == name)
                //        return (T)field.GetValue(null);
                //}
            }

            throw new ArgumentOutOfRangeException("name");
        }

        public static Dictionary<byte, string> GetFriendlyEnumDictionary()
        {

            var type = typeof(T);
            Dictionary<byte, string> data = new Dictionary<byte, string>();
            MemberInfo[] memberInfos = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < memberInfos.Length; i++)
            {
                byte key;
                string value;

                key = (byte)(int)Enum.Parse(type, memberInfos[i].Name);

                var dattr = memberInfos[i].GetCustomAttribute<DisplayAttribute>();
                if (dattr != null)
                {
                    value = dattr.Name;
                }
                else
                {
                    value = memberInfos[i].Name;
                }
                data.Add(key, value);

            }

            return data;
        }

        private static object GetEnum(int? value)
        {

            if (value == null)
            {
                var foo = (T)Enum.ToObject(typeof(T), 0);
                return foo;
            }
            else
            {
                var foo = (T)Enum.Parse(typeof(T), value.ToString());
                //var foo = (T)Enum.ToObject(typeof(T), value);
                return foo;
            }
        }
        public static int GetValueFromDisplayName(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        // return (T)field.GetValue(name);
                        return (int)field.GetValue(name);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (int)field.GetValue(name);
                }
            }

            throw new ArgumentOutOfRangeException("name");
        }
    }

    public static class EnumHelper
    {
        public static FilterGridModel GetOutputEnum(object myEnum)
        {
            var model = new FilterGridModel();
            var type = myEnum.GetType();

            var member = type.GetMember(myEnum.ToString());

            var attribute = Attribute.GetCustomAttribute(member[0],
                    typeof(DropDownAttribute)) as DropDownAttribute;
            if (attribute == null)
            {
            }
            else
            {
                var customAttributesData = member[0].GetCustomAttributesData();
                foreach (var data in customAttributesData)
                {
                    if (data.AttributeType != typeof(DropDownAttribute))
                    {
                        continue;
                    }
                    if (data.ConstructorArguments.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        var providedType = (Type)data.ConstructorArguments[0].Value;
                        if (providedType.BaseType == typeof(Enum))
                        {
                            model.SearchText = GetEnumAsDictionary(providedType);
                            model.DataType = GridEnum.DataTypeEnum.Enum;
                        }
                        else if
                            (providedType == typeof(int) || providedType == typeof(int?) ||
                            providedType.BaseType == typeof(decimal) || providedType == typeof(decimal?) ||
                            providedType == typeof(byte) || providedType == typeof(byte?))
                        {
                            model.DataType = GridEnum.DataTypeEnum.Numeric;
                        }
                        else if (providedType == typeof(string))
                        {
                            model.DataType = GridEnum.DataTypeEnum.String;
                        }
                        else if (providedType == typeof(decimal))
                        {
                            model.DataType = GridEnum.DataTypeEnum.Decimal;
                        }
                        else if (providedType == typeof(DateTime) || providedType == typeof(DateTime?))
                        {
                            model.DataType = GridEnum.DataTypeEnum.DateTime;
                        }
                        else if (providedType == typeof(bool) || providedType == typeof(bool?))
                        {
                            model.SearchText = GetEnumAsDictionary(typeof(CommonEnum.YesNoBoth));
                            model.DataType = GridEnum.DataTypeEnum.Boolean;
                        }
                    }
                }
            }

            return model;
        }

        private static Dictionary<byte, string> GetEnumAsDictionary(Type type)
        {
            Dictionary<byte, string> data = new Dictionary<byte, string>();
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < memberInfos.Length; i++)
            {
                byte key;
                string value;

                key = (byte)(int)Enum.Parse(type, memberInfos[i].Name);

                var dattr = memberInfos[i].GetCustomAttribute<DisplayAttribute>();
                if (dattr != null)
                {
                    value = dattr.Name;
                }
                else
                {
                    value = memberInfos[i].Name;
                }
                data.Add(key, value);
            }

            return data.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace HeaderHero.Serialization
{
    public interface IJsonSerializerLoadCompleted
    {
        void LoadCompleted(Dictionary<string,object> h);
    }

    public interface IJsonSerializerLoading
    {
        void Loading(Dictionary<string, object> h);
    }

    public class JsonSerializerIgnore : Attribute
    {
    }

    public class JsonSerializerName : Attribute
    {
        public string Name;
        public JsonSerializerName(string name)
        {
            Name = name;
        }
    }

    public class JsonSerializerVirtual : Attribute
    {
        public string ClassPropertyName = "Class";

        public Type GetType(Dictionary<string,object> o)
        {
            string name = o[ClassPropertyName] as string;
            if (name == null) return null;

            Type t = null;
            Assembly[] av = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in av)
            {
                t = Type.GetType(name + "," + a.GetName());
                if (t != null)
                    break;
            }
            return t;
        }
    }

    public class JsonSerializerReferenceBy : Attribute
    {
        private string _attribute;
        public JsonSerializerReferenceBy(string a)
        {
            _attribute = a;
        }
        public object ObjectToSave(object o)
        {
            if (o == null)
                return o;

            if (o is System.Collections.IList)
            {
                List<object> a = new List<object>();
                foreach (var item in o as System.Collections.IEnumerable)
                    a.Add(Get(item));
                return a;
            }

            return Get(o);
        }

        private object Get(object o)
        {
            if (o.GetType().GetProperty(_attribute) != null)
                return o.GetType().GetProperty(_attribute).GetValue(o, null);
            else
                return o.GetType().GetField(_attribute).GetValue(o);
        }
    }

    static public class MemberInfoExtension
    {
        static public bool Has<T>(this MemberInfo info) where T : Attribute
        {
            return Attribute.GetCustomAttributes(info).Any(a => a is T);
        }

        static public T Get<T>(this MemberInfo info) where T : Attribute
        {
            foreach (Attribute a in Attribute.GetCustomAttributes(info))
                if (a is T)
                    return (a as T);
            return null;
        }
    }

    public class JsonSerializer
    {
        public class Reference
        {
            public object Object;
            public string Field;
            public object Ref;
            public Reference(object o, string f, object r)
            {
                Object = o;
                Field = f;
                Ref = r;
            }
            public void Set(object value)
            {
                if (Object.GetType().GetProperty(Field) != null)
                    Object.GetType().GetProperty(Field).SetValue(Object, value, null);
                else
                    Object.GetType().GetField(Field).SetValue(Object, value);
            }
        }

        public List<Reference> References = new List<Reference>();

        public delegate string NameConverterDelegate(string name);
        public NameConverterDelegate NameConverter;

        private bool ShallSave(MemberInfo info)
        {
            if ((info is PropertyInfo) && !(info as PropertyInfo).CanWrite)
                return false;
            return !info.Has<JsonSerializerIgnore>();
        }

        private string Name(MemberInfo info)
        {
            JsonSerializerName js = info.Get<JsonSerializerName>();
            if (js != null) return js.Name;
            if (NameConverter == null)
                return info.Name;
            return NameConverter(info.Name);
        }

        private object ObjectToSave(MemberInfo info, object o)
        {
            JsonSerializerReferenceBy jsrb = info.Get<JsonSerializerReferenceBy>();
            if (jsrb != null) return jsrb.ObjectToSave(o);
            return o;
        }

        private bool IsReference(MemberInfo info)
        {
            return info.Has<JsonSerializerReferenceBy>();
        }

        private void SaveReference(object o, string property, object id)
        {
            References.Add(new Reference(o, property, id));
        }

        public Dictionary<string,object> SaveObject(object o, JsonSerializerVirtual virt)
        {
            Dictionary<string, object> h = new Dictionary<string, object>();
            foreach (PropertyInfo pi in o.GetType().GetProperties())
            {
                if (!ShallSave(pi))
                    continue;

                object item = ObjectToSave(pi, pi.GetValue(o, null));
                JsonSerializerVirtual v = pi.Get<JsonSerializerVirtual>();
                if (item != null)
                    h[Name(pi)] = ToJson(item, v);
            }
            foreach (FieldInfo fi in o.GetType().GetFields())
            {
                if (!ShallSave(fi))
                    continue;

                object item = ObjectToSave(fi, fi.GetValue(o));
                JsonSerializerVirtual v = fi.Get<JsonSerializerVirtual>();
                if (item != null)
                    h[Name(fi)] = ToJson(item, v);
            }
            if (virt != null)
                h[virt.ClassPropertyName] = o.GetType().FullName;
            return h;
        }

        public void LoadObject(object o, dynamic h)
        {
            if (o is IJsonSerializerLoading)
                (o as IJsonSerializerLoading).Loading(h as Dictionary<string, object>);

            foreach (PropertyInfo pi in o.GetType().GetProperties())
            {
                if (!ShallSave(pi))
                    continue;

                string name = Name(pi);
                if (pi.CanWrite && h.ContainsKey(name))
                {
                    JsonSerializerVirtual v = pi.Get<JsonSerializerVirtual>();
                    if (IsReference(pi))
                        SaveReference(o, pi.Name, h[name]);
                    else
                    {
                        object value = FromJson(o, h[name], pi.PropertyType, v);
                        pi.SetValue(o, value, null);
                    }
                }
            }
            foreach (FieldInfo fi in o.GetType().GetFields())
            {
                if (!ShallSave(fi))
                    continue;

                string name = Name(fi);
                if (h.ContainsKey(name))
                {
                    JsonSerializerVirtual v = fi.Get<JsonSerializerVirtual>();
                    if (IsReference(fi))
                        SaveReference(o, fi.Name, h[name]);
                    else
                    {
                        object value = FromJson(o, h[name], fi.FieldType, v);
                        fi.SetValue(o, value);
                    }
                }
            }

            if (o is IJsonSerializerLoadCompleted)
                (o as IJsonSerializerLoadCompleted).LoadCompleted(h as Dictionary<string,object>);
        }

        bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        object ToJson(object o, JsonSerializerVirtual virt)
        {
            if (o is int)
                return (double)(int)o;
            else if (o is float)
                return (double)(float)o;
            else if (o is double)
                return (double)o;
            else if (o is string || o is bool || o == null)
                return o;
            else if (o is DateTime)
                return (o as DateTime?).GetValueOrDefault().ToDictionary();
            else if (o is Enum)
                return o.ToString();
            else if (o is TimeSpan)
                return ((TimeSpan)o).TotalSeconds;
            else if (o is System.Collections.IList)
            {
                List<object> a = new List<object>();
                foreach (var item in o as System.Collections.IEnumerable)
                    a.Add(ToJson(item, virt));
                return a;
            }
            else if (o is System.Collections.IDictionary)
            {
                Dictionary<string, object> h = new Dictionary<string, object>();
                foreach (System.Collections.DictionaryEntry item in o as System.Collections.IDictionary)
                    h[(string)item.Key] = ToJson(item.Value, virt);
                return h;
            }
            else if (o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                var a = new List<object>();
                foreach (var item in (System.Collections.IEnumerable)o)
                    a.Add(ToJson(item, virt));
                return a;
            }
            else
                return SaveObject(o, virt);
        }

        object Construct(object parent, Type t)
        {
            // First check if there is a constructor that takes parent type
            if (parent != null)
            {
                ConstructorInfo parent_constructor = t.GetConstructor(new Type[] { parent.GetType() });
                if (parent_constructor != null)
                    return parent_constructor.Invoke(new object[] { parent });
            }
            return t.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        object FromJson(object parent, object o, Type t, JsonSerializerVirtual virt)
        {
            if (t.IsSubclassOf(typeof(Enum)))
                return Enum.Parse(t, o as string);
            else if (t == typeof(TimeSpan))
                return new TimeSpan((long)((double)o * TimeSpan.TicksPerSecond));
            else if (o is string || o is int || o is bool)
                return o;
            else if (o is double)
            {
                if (t == typeof(double) || t == typeof(double?))
                    return (double)o;
                else if (t == typeof(float) || t == typeof(float?))
                    return (float)(double)o;
                else if (t == typeof(int) || t == typeof(int?))
                    return (int)(double)o;
                else if (t == typeof(Object))
                    return o;
                else
                    throw new Exception("error");
            }
            else if (t == typeof(DateTime) || t == typeof(DateTime?))
            {
                return DateTimeExtensions.FromDictionary(o as Dictionary<string, object>);
            }
            else if (typeof(System.Collections.IDictionary).IsAssignableFrom(t))
            {
                Type key_type = t.GetGenericArguments()[0];
                Type value_type = t.GetGenericArguments()[1];
                System.Collections.IDictionary res = t.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Collections.IDictionary;
                foreach (var item in o as Dictionary<string, object>)
                    res[FromJson(parent, item.Key, key_type, virt)] = FromJson(parent, item.Value, value_type, virt);
                return res;
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                var value_type = t.GetGenericArguments()[0];
                var object_enumerable = o;
                var cast_method = typeof(Enumerable).GetMethod("Cast");
                var cast_to_value_type_method = cast_method.MakeGenericMethod(value_type);
                var typed_enumerable = cast_to_value_type_method.Invoke(null, new[] { object_enumerable });
                var hash_set_constructor = t.GetConstructor(new[] { typed_enumerable.GetType() });
                Debug.Assert(hash_set_constructor != null);
                var hash_set = hash_set_constructor.Invoke(new[] { typed_enumerable });
                return hash_set;
            }
            else if (o is List<object>)
            {
                if (t.IsArray)
                {
                    Array res = Array.CreateInstance(t.GetElementType(), (o as List<object>).Count);
                    int i = 0;
                    foreach (object item in o as List<object>)
                    {
                        res.SetValue(FromJson(parent, item, t.GetElementType(), virt), i);
                        ++i;
                    }
                    return res;
                }
                else
                {
                    Type inner_type = t.GetGenericArguments()[0];
                    System.Collections.IList res = t.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Collections.IList;
                    foreach (object item in o as List<object>)
                        res.Add(FromJson(parent, item, inner_type, virt));
                    return res;
                }
            }
            else if (o is Dictionary<string, object>)
            {
                if (virt != null)
                    t = virt.GetType(o as Dictionary<string, object>);
                object res = Construct(parent, t);
                LoadObject(res, o as Dictionary<string, object>);
                return res;
            }
            else
                throw new Exception("error");
        }

        static public Dictionary<string,object> Save(object o)
        {
            return (new JsonSerializer()).SaveObject(o, null);
        }

        static public void Load(object o, Dictionary<string,object> d)
        {
            (new JsonSerializer()).LoadObject(o, d);
        }

        /// <summary>
        /// Clones object through Json serializer.
        /// </summary>
        static public T Clone<T>(T o) where T:class
        {
            JsonSerializer js = new JsonSerializer();
            object json = js.ToJson(o, new JsonSerializerVirtual());
            return js.FromJson(null, json, typeof(T), new JsonSerializerVirtual()) as T;
        }

        /// <summary>
        /// Tests equality through Json serializer.
        /// </summary>
        static public bool Equals<T>(T a, T b) where T : class
        {
            JsonSerializer js = new JsonSerializer();
            Dictionary<string, object> jsona = js.ToJson(a, new JsonSerializerVirtual()) as Dictionary<string, object>;
            Dictionary<string, object> jsonb = js.ToJson(b, new JsonSerializerVirtual()) as Dictionary<string, object>;
            return Sjson.Encode(jsona) == Sjson.Encode(jsonb);
        }
    }

    public static class DateTimeExtensions
    {
       public static DateTime FromDictionary(Dictionary<string,object> h)
        {
            int year = (int)(double)h["Year"];
            int month = (int)(double)h["Month"];
            int day = (int)(double)h["Day"];
            int hour = (int)(double)h["Hour"];
            int minute = (int)(double)h["Minute"];
            int second = (int)(double)h["Second"];
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static Dictionary<string,object> ToDictionary(this DateTime c)
        {
            return new Dictionary<string,object>{{"Year", (double)c.Year}, {"Month", (double)c.Month}, {"Day", (double)c.Day},
                {"Hour", (double)c.Hour}, {"Minute", (double)c.Minute}, {"Second", (double)c.Second}};
        }
    }
}

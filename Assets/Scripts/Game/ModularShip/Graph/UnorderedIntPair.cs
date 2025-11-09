using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
    public struct UnorderedIntPair : IEquatable<UnorderedIntPair>
    {
        public int a;
        public int b;

        public UnorderedIntPair(int first, int second)
        {
            // 总是将较小的值存储在a，较大的值存储在b
            if (first <= second)
            {
                a = first;
                b = second;
            }
            else
            {
                a = second;
                b = first;
            }
        }

        // 实现IEquatable接口
        public bool Equals(UnorderedIntPair other)
        {
            return a == other.a && b == other.b;
        }

        // 重写Equals方法
        public override bool Equals(object obj)
        {
            return obj is UnorderedIntPair other && Equals(other);
        }

        // 重写GetHashCode方法
        public override int GetHashCode()
        {
            // 使用位运算组合哈希值
            return a.GetHashCode() ^ (b.GetHashCode() << 2);
        }

        // 重写ToString方法以便调试
        public override string ToString()
        {
            return $"({a}, {b})";
        }

        // 重载==运算符
        public static bool operator ==(UnorderedIntPair left, UnorderedIntPair right)
        {
            return left.Equals(right);
        }

        // 重载!=运算符
        public static bool operator !=(UnorderedIntPair left, UnorderedIntPair right)
        {
            return !left.Equals(right);
        }
    }
    
    public class UnorderedIntPairConverter : JsonConverter<UnorderedIntPair>
    {
        public override void WriteJson(JsonWriter writer, UnorderedIntPair value, JsonSerializer serializer)
        {
            // 将UnorderedIntPair序列化为字符串，例如："1-2"
            writer.WriteValue($"{value.a}:{value.b}");
        }

        public override UnorderedIntPair ReadJson(JsonReader reader, Type objectType, UnorderedIntPair existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // 从读取器中获取字符串
            string s = (string)reader.Value;
            // 解析字符串，这里假设字符串格式为"整数-整数"
            var parts = s.Split(':');
            if (parts.Length != 2)
                throw new JsonException("Invalid UnorderedIntPair string format.");

            if (int.TryParse(parts[0], out int a) && int.TryParse(parts[1], out int b))
            {
                return new UnorderedIntPair { a = a, b = b };
            }
            else
            {
                throw new JsonException("Invalid integers in UnorderedIntPair string.");
            }
        }
    }
    
    public class UnorderedIntPairDictionaryConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // 检查对象类型是否是Dictionary<UnorderedIntPair, T>
        return objectType.IsGenericType && 
               objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
               objectType.GetGenericArguments()[0] == typeof(UnorderedIntPair);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Type valueType = value.GetType();
        Type valueGenericArg = valueType.GetGenericArguments()[1];

        writer.WriteStartObject();
        foreach (DictionaryEntry entry in (IDictionary)value)
        {
            UnorderedIntPair key = (UnorderedIntPair)entry.Key;
            string keyString = $"{key.a}:{key.b}";
            writer.WritePropertyName(keyString);
            serializer.Serialize(writer, entry.Value, valueGenericArg);
        }
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Type valueType = objectType.GetGenericArguments()[1];
        Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeof(UnorderedIntPair),valueType);
        object dictionary = Activator.CreateInstance(dictionaryType);

        JObject obj = JObject.Load(reader);
        foreach (var property in obj.Properties())
        {
            var keyParts = property.Name.Split(':');
            if (keyParts.Length != 2)
                throw new JsonSerializationException("Invalid key format for UnorderedIntPair");

            int first = int.Parse(keyParts[0]);
            int second = int.Parse(keyParts[1]);
            UnorderedIntPair key = new UnorderedIntPair(first, second);

            object valueValue = property.Value.ToObject(valueType, serializer);

            ((IDictionary)dictionary).Add(key, valueValue);
        }
        return dictionary;
    }
}
}
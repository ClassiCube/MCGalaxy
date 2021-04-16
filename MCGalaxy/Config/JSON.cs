// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy.Config {
    
    public sealed class JsonArray  : List<object> { }
    
    public sealed class JsonObject : Dictionary<string, object> {
        public object Meta;
        
        public void Deserialise(ConfigElement[] elems, object instance) {
            foreach (var kvp in this) {
                ConfigElement.Parse(elems, instance, kvp.Key, (string)kvp.Value);
            }
        }
    }
    
    public delegate void JsonOnMember(JsonObject obj, string key, object value);
    
    /// <summary> Implements a simple JSON parser. </summary>
    public sealed class JsonReader {
        public readonly string Value;
        /// <summary> Whether an error occurred while parsing the given JSON. </summary>
        public bool Failed;
        /// <summary> Callback invoked when a member of an object has been parsed. </summary>
        public JsonOnMember OnMember;
        
        int offset;
        char Cur { get { return Value[offset]; } }
        StringBuilder strBuffer = new StringBuilder(96);
        
        public JsonReader(string value) {
            Value    = value;
            OnMember = DefaultOnMember;
        }
        
        static void DefaultOnMember(JsonObject obj, string key, object value) { obj[key] = value; }
        
        
        const int T_NONE = 0, T_NUM = 1, T_TRUE = 2, T_FALSE = 3, T_NULL = 4;
        static bool IsWhitespace(char c) {
            return c == '\r' || c == '\n' || c == '\t' || c == ' ';
        }
        
        bool NextConstant(string value) {
            if (offset + value.Length > Value.Length) return false;
            
            for (int i = 0; i < value.Length; i++) {
                if (Value[offset + i] != value[i]) return false;
            }
            
            offset += value.Length; return true;
        }
        
        int NextToken() {
            for (; offset < Value.Length && IsWhitespace(Cur); offset++);
            if (offset >= Value.Length) return T_NONE;
            
            char c = Cur; offset++;
            if (c == '{' || c == '}') return c;
            if (c == '[' || c == ']') return c;
            if (c == ',' || c == '"' || c == ':') return c;
            
            if (IsNumber(c)) return T_NUM;
            offset--;
            
            if (NextConstant("true"))  return T_TRUE;
            if (NextConstant("false")) return T_FALSE;
            if (NextConstant("null"))  return T_NULL;
            
            // invalid token
            offset++; return T_NONE;
        }
        
        /// <summary> Parses the given JSON and then returns the root element. </summary>
        /// <returns> Either a JsonObject, a JsonArray, a string, or null </returns>
        public object Parse() { return ParseValue(NextToken()); }
        
        object ParseValue(int token) {
            switch (token) {
                case '{': return ParseObject();
                case '[': return ParseArray();
                case '"': return ParseString();
                    
                case T_NUM:   return ParseNumber();
                case T_TRUE:  return "true";
                case T_FALSE: return "false";
                case T_NULL:  return null;
                    
                default: return null;
            }
        }
        
        JsonObject ParseObject() {
            JsonObject obj = new JsonObject();
            while (true) {
                int token = NextToken();
                if (token == ',') continue;
                if (token == '}') return obj;
                
                if (token != '"') { Failed = true; return null; }
                string key = ParseString();
                
                token = NextToken();
                if (token != ':') { Failed = true; return null; }
                
                token = NextToken();
                if (token == T_NONE) { Failed = true; return null; }
                
                object value = ParseValue(token);
                OnMember(obj, key, value);
            }
        }
        
        JsonArray ParseArray() {
            JsonArray arr = new JsonArray();
            while (true) {
                int token = NextToken();
                if (token == ',') continue;
                if (token == ']') return arr;
                
                if (token == T_NONE) { Failed = true; return null; }
                arr.Add(ParseValue(token));
            }
        }
        
        string ParseString() {
            StringBuilder s = strBuffer; s.Length = 0;
            
            for (; offset < Value.Length;) {
                char c = Cur; offset++;
                if (c == '"') return s.ToString();
                if (c != '\\') { s.Append(c); continue; }
                
                if (offset >= Value.Length) break;
                c = Cur; offset++;
                if (c == '/' || c == '\\' || c == '"') { s.Append(c); continue; }
                
                if (c != 'u') break;
                if (offset + 4 > Value.Length) break;
                
                // form of \uYYYY
                int aH = Colors.UnHex(Value[offset + 0]);
                int aL = Colors.UnHex(Value[offset + 1]);
                int bH = Colors.UnHex(Value[offset + 2]);
                int bL = Colors.UnHex(Value[offset + 3]);
                
                if (aH == -1 || aL == -1 || bH == -1 || bL == -1) break;
                int codePoint = (aH << 12) | (aL << 8) | (bH << 4) | bL;
                s.Append((char)codePoint);
                offset += 4;
            }
            
            Failed = true; return null;
        }
        
        static bool IsNumber(char c) {
            return c == '-' || c == '.' || (c >= '0' && c <= '9');
        }
        
        string ParseNumber() {
            int start = offset - 1;
            for (; offset < Value.Length && IsNumber(Cur); offset++);
            return Value.Substring(start, offset - start);
        }
    }
    
    public class JsonWriter {
        readonly TextWriter w;
        public JsonWriter(TextWriter dst) { w = dst; }
        
        static char Hex(char c, int shift) {
            int x = (c >> shift) & 0x0F;
            return (char)(x <= 9 ? ('0' + x) : ('a' + (x - 10)));
        }
        
        public void WriteNull() { w.Write("null"); }
        public void Write(string value) { w.Write(value); }
        
        public void WriteString(string value) {
            w.Write('"');
            foreach (char c in value) {
                if (c == '/')         { w.Write("\\/");
                } else if (c == '\\') { w.Write("\\\\");
                } else if (c == '"')  { w.Write("\\\"");
                } else if (c >= ' ' && c <= '~') { w.Write(c);
                } else {
                    w.Write("\\u");
                    w.Write(Hex(c, 12)); w.Write(Hex(c, 8));
                    w.Write(Hex(c, 4));  w.Write(Hex(c, 0));
                }
            }
            w.Write('"');
        }
        
        public void WriteArray<T>(IList<T> array) {
            w.Write("[\r\n");
            string separator = "";
            
            for (int i = 0; i < array.Count; i++) {
                w.Write(separator);
                object value = array[i];               
                WriteValue(value);
                separator = ",\r\n";
            }
            w.Write("]\r\n");
        }
        
        public void WriteObject(object value) {
            w.Write("{\r\n");
            SerialiseObject(value);
            w.Write("\r\n}");
        }
        
        internal void WriteObjectKey(string name) {
            Write("    "); WriteString(name); Write(": ");
        }
        
        
        protected virtual void WriteValue(object value) {
            // TODO this is awful code
            if (value == null) {
                WriteNull();
            } else if (value is int) {
                Write(value.ToString());
            } else if (value is bool) {
                bool val = (bool)value;
                Write(val ? "true" : "false");
            } else if (value is string) {
                WriteString((string)value);
            } else if (value is JsonArray) {
                WriteArray((JsonArray)value);
            }  else if (value is JsonObject) {
                WriteObject(value);
            } else {
                throw new InvalidOperationException("Unknown datatype: " + value.GetType());
            }
        }
        
        protected virtual void SerialiseObject(object value) {
            string separator = null;
            JsonObject obj   = (JsonObject)value;
            
            foreach (var kvp in obj) {
                Write(separator);
                WriteObjectKey(kvp.Key);
                WriteValue(kvp.Value);
                separator = ",\r\n";
            }
        }
    }
    
    public class JsonConfigWriter : JsonWriter {
        ConfigElement[] elems;    
        public JsonConfigWriter(TextWriter dst, ConfigElement[] cfg) : base(dst) { elems = cfg; }
        
        // Only ever write an object
        protected override void WriteValue(object value) { WriteObject(value); }
        
        void WriteConfigValue(ConfigAttribute a, string value) {
            if (String.IsNullOrEmpty(value)) {
                WriteNull();
            } else if (a is ConfigBoolAttribute || a is ConfigIntegerAttribute || a is ConfigRealAttribute) {
                Write(value);
            } else {
                WriteString(value);
            }
        }
        
        protected override void SerialiseObject(object value) {
            string separator = null;
            
            for (int i = 0; i < elems.Length; i++) {
                ConfigElement elem = elems[i];
                ConfigAttribute a  = elem.Attrib;
                Write(separator);
                
                WriteObjectKey(a.Name);
                object raw  = elem.Field.GetValue(value);
                string text = elem.Attrib.Serialise(raw);
                
                WriteConfigValue(a, text);
                separator = ",\r\n";
            }
        }
    }
    
    public static class JsonSerialisers {
        
        static void WriteConfigValue(JsonWriter w, ConfigAttribute a, string value) {
            if (String.IsNullOrEmpty(value)) {
                w.WriteNull();
            } else if (a is ConfigBoolAttribute || a is ConfigIntegerAttribute || a is ConfigRealAttribute) {
                w.Write(value);
            } else {
                w.WriteString(value);
            }
        }
        
        public static void WriteConfig(JsonWriter w, ConfigElement[] elems, object instance) {
            string separator = null;
            
            for (int i = 0; i < elems.Length; i++) {
                ConfigElement elem = elems[i];
                ConfigAttribute a = elem.Attrib;
                w.Write(separator);
                
                w.WriteObjectKey(a.Name);
                object raw = elem.Field.GetValue(instance);
                string value = elem.Attrib.Serialise(raw);
                
                WriteConfigValue(w, a, value);
                separator = ",\r\n";
            }
        }
    }
    
    public static class Json {
        
        [Obsolete("Use JsonReader instead")]
        public static object Parse(string s, out bool success) {
            JsonReader reader = new JsonReader(s);
            object obj = reader.Parse();
            success    = !reader.Failed;
            return obj;
        }
        
        [Obsolete("Use JsonWriter instead")]
        public static void Serialise(TextWriter dst, ConfigElement[] elems, object instance) {
            JsonConfigWriter w = new JsonConfigWriter(dst, elems);
            w.WriteObject(instance);
        }
        
        /// <summary> Shorthand for serialising an object to a JSON object </summary>
        public static string SerialiseObject(object obj) {
            StringWriter dst  = new StringWriter();
            JsonWriter   w    = new JsonWriter(dst);
            
            w.WriteObject(obj);
            return dst.ToString();
        }
    }
}
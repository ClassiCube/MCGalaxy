// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy.Config {

    public class JsonContext {
        public string Val; public int Idx; public bool Success = true;
        public char Cur { get { return Val[Idx]; } }
        internal StringBuilder strBuffer = new StringBuilder(96);
    }
    
    public sealed class JsonArray : List<object> { }
    
    public sealed class JsonObject {
        // NOTE: BlockDefinitions entries have about 30 members
        public List<string> Keys = new List<string>(30);
        public List<object> Values = new List<object>(30);
        
        public void Deserialise(ConfigElement[] elems, object instance) {
            for (int i = 0; i < Keys.Count; i++) {
                ConfigElement.Parse(elems, instance, Keys[i], (string)Values[i]);
            }
        }
    }
    
    public static class Json {
        const int T_NONE = 0, T_NUM = 1, T_TRUE = 2, T_FALSE = 3, T_NULL = 4;
        
        static bool IsWhitespace(char c) {
            return c == '\r' || c == '\n' || c == '\t' || c == ' ';
        }
        
        static bool NextConstant(JsonContext ctx, string value) {
            if (ctx.Idx + value.Length > ctx.Val.Length) return false;
            
            for (int i = 0; i < value.Length; i++) {
                if (ctx.Val[ctx.Idx + i] != value[i]) return false;
            }
            
            ctx.Idx += value.Length; return true;
        }
        
        static int NextToken(JsonContext ctx) {
            for (; ctx.Idx < ctx.Val.Length && IsWhitespace(ctx.Cur); ctx.Idx++);
            if (ctx.Idx >= ctx.Val.Length) return T_NONE;
            
            char c = ctx.Cur; ctx.Idx++;
            if (c == '{' || c == '}') return c;
            if (c == '[' || c == ']') return c;
            if (c == ',' || c == '"' || c == ':') return c;
            
            if (IsNumber(c)) return T_NUM;
            ctx.Idx--;
            
            if (NextConstant(ctx, "true"))  return T_TRUE;
            if (NextConstant(ctx, "false")) return T_FALSE;
            if (NextConstant(ctx, "null"))  return T_NULL;
            
            // invalid token
            ctx.Idx++; return T_NONE;
        }
        
        public static object ParseStream(JsonContext ctx) {
            return ParseValue(NextToken(ctx), ctx);
        }
        
        static object ParseValue(int token, JsonContext ctx) {
            switch (token) {
                case '{': return ParseObject(ctx);
                case '[': return ParseArray(ctx);
                case '"': return ParseString(ctx);
                    
                case T_NUM:   return ParseNumber(ctx);
                case T_TRUE:  return "true";
                case T_FALSE: return "false";
                case T_NULL:  return null;
                    
                default: return null;
            }
        }
        
        static JsonObject ParseObject(JsonContext ctx) {
            JsonObject members = new JsonObject();
            while (true) {
                int token = NextToken(ctx);
                if (token == ',') continue;
                if (token == '}') return members;
                
                if (token != '"') { ctx.Success = false; return null; }
                string key = ParseString(ctx);
                
                token = NextToken(ctx);
                if (token != ':') { ctx.Success = false; return null; }
                
                token = NextToken(ctx);
                if (token == T_NONE) { ctx.Success = false; return null; }
                
                object value = ParseValue(token, ctx);
                members.Keys.Add(key);
                members.Values.Add(value);
            }
        }
        
        static JsonArray ParseArray(JsonContext ctx) {
            JsonArray elements = new JsonArray();
            while (true) {
                int token = NextToken(ctx);
                if (token == ',') continue;
                if (token == ']') return elements;
                
                if (token == T_NONE) { ctx.Success = false; return null; }
                elements.Add(ParseValue(token, ctx));
            }
        }
        
        static string ParseString(JsonContext ctx) {
            StringBuilder s = ctx.strBuffer; s.Length = 0;
            
            for (; ctx.Idx < ctx.Val.Length;) {
                char c = ctx.Cur; ctx.Idx++;
                if (c == '"') return s.ToString();
                if (c != '\\') { s.Append(c); continue; }
                
                if (ctx.Idx >= ctx.Val.Length) break;
                c = ctx.Cur; ctx.Idx++;
                if (c == '/' || c == '\\' || c == '"') { s.Append(c); continue; }
                
                if (c != 'u') break;
                if (ctx.Idx + 4 > ctx.Val.Length) break;
                
                // form of \uYYYY
                int aH = Colors.UnHex(ctx.Val[ctx.Idx + 0]);
                int aL = Colors.UnHex(ctx.Val[ctx.Idx + 1]);
                int bH = Colors.UnHex(ctx.Val[ctx.Idx + 2]);
                int bL = Colors.UnHex(ctx.Val[ctx.Idx + 3]);
                
                if (aH == -1 || aL == -1 || bH == -1 || bL == -1) break;
                int codePoint = (aH << 12) | (aL << 8) | (bH << 4) | bL;
                s.Append((char)codePoint);
                ctx.Idx += 4;
            }
            
            ctx.Success = false; return null;
        }
        
        static bool IsNumber(char c) {
            return c == '-' || c == '.' || (c >= '0' && c <= '9');
        }
        
        static string ParseNumber(JsonContext ctx) {
            int start = ctx.Idx - 1;
            for (; ctx.Idx < ctx.Val.Length && IsNumber(ctx.Cur); ctx.Idx++);
            return ctx.Val.Substring(start, ctx.Idx - start);
        }
        
        
        static char Hex(char c, int shift) {
            int x = (c >> shift) & 0x0F;
            return (char)(x <= 9 ? ('0' + x) : ('a' + (x - 10)));
        }
        
        static void WriteString(StreamWriter w, string value) {
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
        
        static void WriteValue(StreamWriter w, ConfigAttribute a, string value) {
            if (String.IsNullOrEmpty(value)) {
                w.Write("null");
            } else if (a is ConfigBoolAttribute || a is ConfigIntegerAttribute || a is ConfigRealAttribute) {
                w.Write(value);
            } else {
                WriteString(w, value);
            }
        }
        
        public static void Serialise(StreamWriter w, ConfigElement[] elems, object instance) {
            w.Write("{\r\n");
            string separator = null;
            
            for (int i = 0; i < elems.Length; i++) {
                ConfigElement elem = elems[i];
                ConfigAttribute a = elem.Attrib;
                w.Write(separator);
                
                w.Write("    "); WriteString(w, a.Name); w.Write(": ");
                object raw = elem.Field.GetValue(instance);
                string value = elem.Attrib.Serialise(raw);
                
                WriteValue(w, a, value);
                separator = ",\r\n";
            }
            w.Write("\r\n}");
        }
    }
}
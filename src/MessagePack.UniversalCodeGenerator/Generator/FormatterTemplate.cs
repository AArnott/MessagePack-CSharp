﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace MessagePack.CodeGenerator.Generator
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class FormatterTemplate : FormatterTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("#pragma warning disable 618\r\n#pragma warning disable 612\r\n#pragma warning disable" +
                    " 414\r\n#pragma warning disable 168\r\n\r\nnamespace ");
            
            #line 11 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    using System;\r\n\tusing System.Buffers;\r\n    using MessagePack;\r\n\r\n");
            
            #line 17 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var objInfo in objectSerializationInfos) { 
            
            #line default
            #line hidden
            this.Write("\r\n    public sealed class ");
            
            #line 19 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.Name));
            
            #line default
            #line hidden
            this.Write("Formatter : global::MessagePack.Formatters.IMessagePackFormatter<");
            
            #line 19 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));
            
            #line default
            #line hidden
            this.Write(">\r\n    {\r\n");
            
            #line 21 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if( objInfo.IsStringKey) { 
            
            #line default
            #line hidden
            this.Write("\r\n        readonly global::MessagePack.Internal.AutomataDictionary ____keyMapping" +
                    ";\r\n        readonly byte[][] ____stringByteKeys;\r\n\r\n        public ");
            
            #line 26 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.Name));
            
            #line default
            #line hidden
            this.Write("Formatter()\r\n        {\r\n            this.____keyMapping = new global::MessagePack" +
                    ".Internal.AutomataDictionary()\r\n            {\r\n");
            
            #line 30 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var x in objInfo.Members) { 
            
            #line default
            #line hidden
            this.Write("                { \"");
            
            #line 31 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.StringKey));
            
            #line default
            #line hidden
            this.Write("\", ");
            
            #line 31 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.IntKey));
            
            #line default
            #line hidden
            this.Write("},\r\n");
            
            #line 32 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            };\r\n\r\n            this.____stringByteKeys = new byte[][]\r\n           " +
                    " {\r\n");
            
            #line 37 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var x in objInfo.Members.Where(x => x.IsReadable)) { 
            
            #line default
            #line hidden
            this.Write("                global::MessagePack.MessagePackBinary.GetEncodedStringBytes(\"");
            
            #line 38 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.StringKey));
            
            #line default
            #line hidden
            this.Write("\"),\r\n");
            
            #line 39 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                \r\n            };\r\n        }\r\n\r\n");
            
            #line 43 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n        public void Serialize(ref BufferWriter writer, ");
            
            #line 45 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));
            
            #line default
            #line hidden
            this.Write(" value, global::MessagePack.IFormatterResolver formatterResolver)\r\n        {\r\n");
            
            #line 47 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if( objInfo.IsClass) { 
            
            #line default
            #line hidden
            this.Write("            if (value == null)\r\n            {\r\n                global::MessagePac" +
                    "k.MessagePackBinary.WriteNil(ref writer);\r\n                return;\r\n            }\r\n");
            
            #line 53 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            \r\n");
            
            #line 54 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
if(objInfo.HasIMessagePackSerializationCallbackReceiver && objInfo.NeedsCastOnBefore) { 
            
            #line default
            #line hidden
            this.Write("            ((IMessagePackSerializationCallbackReceiver)value).OnBeforeSerialize(" +
                    ");\r\n");
            
            #line 56 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else if(objInfo.HasIMessagePackSerializationCallbackReceiver) { 
            
            #line default
            #line hidden
            this.Write("            value.OnBeforeSerialize();\r\n");
            
            #line 58 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            
            #line 59 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if( objInfo.IsIntKey) { if( (objInfo.MaxKey + 1) <= 15) { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(wri" +
                    "ter, ");
            
            #line 60 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.MaxKey + 1));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 61 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteArrayHeader(ref writer, ");
            
            #line 62 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.MaxKey + 1));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 63 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } } else if( objInfo.WriteCount <= 15) { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteFixedMapHeaderUnsafe(write" +
                    "r, ");
            
            #line 64 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.WriteCount));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 65 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteMapHeader(ref writer, ");
            
            #line 66 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.WriteCount));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 67 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            
            #line 68 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if(objInfo.IsIntKey) { 
            
            #line default
            #line hidden
            
            #line 69 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 for(var i =0; i<= objInfo.MaxKey; i++) { var member = objInfo.GetMember(i); 
            
            #line default
            #line hidden
            
            #line 70 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if( member == null) { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteNil(ref writer);\r\n");
            
            #line 72 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            ");
            
            #line 73 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.GetSerializeMethodString()));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 74 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } } } else { 
            
            #line default
            #line hidden
            
            #line 75 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 var index = 0; foreach(var x in objInfo.Members) { 
            
            #line default
            #line hidden
            this.Write("            global::MessagePack.MessagePackBinary.WriteRaw(ref writer, this.____strin" +
                    "gByteKeys[");
            
            #line 76 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index++));
            
            #line default
            #line hidden
            this.Write("]);\r\n            ");
            
            #line 77 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.GetSerializeMethodString()));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 78 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } } 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n        public ");
            
            #line 81 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));
            
            #line default
            #line hidden
            this.Write(" Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormat" +
                    "terResolver formatterResolver)\r\n        {\r\n            if (global::MessagePack.M" +
                    "essagePackBinary.IsNil(byteSequence))\r\n            {\r\n");
            
            #line 85 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if( objInfo.IsClass) { 
            
            #line default
            #line hidden
            this.Write("                byteSequence = byteSequence.Slice(1);\r\n                return nul" +
                    "l;\r\n");
            
            #line 88 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("                throw new InvalidOperationException(\"typecode is null, struct not" +
                    " supported\");\r\n");
            
            #line 90 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            }\r\n\r\n");
            
            #line 93 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if(objInfo.IsStringKey) { 
            
            #line default
            #line hidden
            this.Write("            var length = global::MessagePack.MessagePackBinary.ReadMapHeader(ref " +
                    "byteSequence);\r\n");
            
            #line 95 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(re" +
                    "f byteSequence);\r\n");
            
            #line 97 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 99 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var x in objInfo.Members) { 
            
            #line default
            #line hidden
            this.Write("            var __");
            
            #line 100 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.Name));
            
            #line default
            #line hidden
            this.Write("__ = default(");
            
            #line 100 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.Type));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 101 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n            for (int i = 0; i < length; i++)\r\n            {\r\n");
            
            #line 105 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 if(objInfo.IsStringKey) { 
            
            #line default
            #line hidden
            this.Write(@"                var stringKey = global::MessagePack.MessagePackBinary.ReadStringSegment(ref byteSequence);
                int key;
                if (!____keyMapping.TryGetValue(stringKey, out key))
                {
                    global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                    continue;
                }
");
            
            #line 113 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("                var key = i;\r\n");
            
            #line 115 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n                switch (key)\r\n                {\r\n");
            
            #line 119 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var x in objInfo.Members) { 
            
            #line default
            #line hidden
            this.Write("                    case ");
            
            #line 120 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.IntKey));
            
            #line default
            #line hidden
            this.Write(":\r\n                        __");
            
            #line 121 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.Name));
            
            #line default
            #line hidden
            this.Write("__ = ");
            
            #line 121 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.GetDeserializeMethodString()));
            
            #line default
            #line hidden
            this.Write(";\r\n                        break;\r\n");
            
            #line 123 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                    default:\r\n                        global::MessagePack.Message" +
                    "PackBinary.ReadNextBlock(ref byteSequence);\r\n                        break;\r\n   " +
                    "             }\r\n            }\r\n\r\n            var ____result = new ");
            
            #line 130 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.GetConstructorString()));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 131 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 foreach(var x in objInfo.Members.Where(x => x.IsWritable)) { 
            
            #line default
            #line hidden
            this.Write("            ____result.");
            
            #line 132 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.Name));
            
            #line default
            #line hidden
            this.Write(" = __");
            
            #line 132 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(x.Name));
            
            #line default
            #line hidden
            this.Write("__;\r\n");
            
            #line 133 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            
            #line 134 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
if(objInfo.HasIMessagePackSerializationCallbackReceiver && objInfo.NeedsCastOnAfter) { 
            
            #line default
            #line hidden
            this.Write("            ((IMessagePackSerializationCallbackReceiver)____result).OnAfterDeseri" +
                    "alize();\r\n");
            
            #line 136 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } else if(objInfo.HasIMessagePackSerializationCallbackReceiver) { 
            
            #line default
            #line hidden
            this.Write("            ____result.OnAfterDeserialize();\r\n");
            
            #line 138 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            return ____result;\r\n        }\r\n    }\r\n\r\n");
            
            #line 143 "D:\git\MessagePack-CSharp\src\MessagePack.UniversalCodeGenerator\Generator\FormatterTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("}\r\n\r\n#pragma warning restore 168\r\n#pragma warning restore 414\r\n#pragma warning re" +
                    "store 618\r\n#pragma warning restore 612");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class FormatterTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}

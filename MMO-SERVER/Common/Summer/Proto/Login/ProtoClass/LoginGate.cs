// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: LoginGate.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace HS.Protobuf.LoginGate {

  /// <summary>Holder for reflection information generated from LoginGate.proto</summary>
  public static partial class LoginGateReflection {

    #region Descriptor
    /// <summary>File descriptor for LoginGate.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static LoginGateReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9Mb2dpbkdhdGUucHJvdG8SFUhTLlByb3RvYnVmLkxvZ2luR2F0ZRofQ29t",
            "bW9uL1Byb3RvU291cmNlL0NvbW1vbi5wcm90byJHCghFbnZlbG9wZRIUCgxw",
            "cm90b2NvbENvZGUYASABKAUSFwoPZW5jcnlwdGlvbkxldmVsGAIgASgFEgwK",
            "BGRhdGEYAyABKAwqUAoQTG9naW5HYXRlUHJvdG9jbBIaChZMT0dJTkdBVEVf",
            "UFJPVE9DTF9OT05FEAASIAoaTE9HSU5HQVRFX1BST1RPQ0xfRU5WRUxPUEUQ",
            "iaQBYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::HS.Protobuf.Common.CommonReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::HS.Protobuf.LoginGate.LoginGateProtocl), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::HS.Protobuf.LoginGate.Envelope), global::HS.Protobuf.LoginGate.Envelope.Parser, new[]{ "ProtocolCode", "EncryptionLevel", "Data" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum LoginGateProtocl {
    [pbr::OriginalName("LOGINGATE_PROTOCL_NONE")] None = 0,
    [pbr::OriginalName("LOGINGATE_PROTOCL_ENVELOPE")] Envelope = 21001,
  }

  #endregion

  #region Messages
  public sealed partial class Envelope : pb::IMessage<Envelope>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Envelope> _parser = new pb::MessageParser<Envelope>(() => new Envelope());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Envelope> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::HS.Protobuf.LoginGate.LoginGateReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Envelope() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Envelope(Envelope other) : this() {
      protocolCode_ = other.protocolCode_;
      encryptionLevel_ = other.encryptionLevel_;
      data_ = other.data_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Envelope Clone() {
      return new Envelope(this);
    }

    /// <summary>Field number for the "protocolCode" field.</summary>
    public const int ProtocolCodeFieldNumber = 1;
    private int protocolCode_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ProtocolCode {
      get { return protocolCode_; }
      set {
        protocolCode_ = value;
      }
    }

    /// <summary>Field number for the "encryptionLevel" field.</summary>
    public const int EncryptionLevelFieldNumber = 2;
    private int encryptionLevel_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int EncryptionLevel {
      get { return encryptionLevel_; }
      set {
        encryptionLevel_ = value;
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 3;
    private pb::ByteString data_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString Data {
      get { return data_; }
      set {
        data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Envelope);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Envelope other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ProtocolCode != other.ProtocolCode) return false;
      if (EncryptionLevel != other.EncryptionLevel) return false;
      if (Data != other.Data) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ProtocolCode != 0) hash ^= ProtocolCode.GetHashCode();
      if (EncryptionLevel != 0) hash ^= EncryptionLevel.GetHashCode();
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ProtocolCode != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ProtocolCode);
      }
      if (EncryptionLevel != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(EncryptionLevel);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Data);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ProtocolCode != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ProtocolCode);
      }
      if (EncryptionLevel != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(EncryptionLevel);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Data);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (ProtocolCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ProtocolCode);
      }
      if (EncryptionLevel != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(EncryptionLevel);
      }
      if (Data.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Data);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Envelope other) {
      if (other == null) {
        return;
      }
      if (other.ProtocolCode != 0) {
        ProtocolCode = other.ProtocolCode;
      }
      if (other.EncryptionLevel != 0) {
        EncryptionLevel = other.EncryptionLevel;
      }
      if (other.Data.Length != 0) {
        Data = other.Data;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ProtocolCode = input.ReadInt32();
            break;
          }
          case 16: {
            EncryptionLevel = input.ReadInt32();
            break;
          }
          case 26: {
            Data = input.ReadBytes();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            ProtocolCode = input.ReadInt32();
            break;
          }
          case 16: {
            EncryptionLevel = input.ReadInt32();
            break;
          }
          case 26: {
            Data = input.ReadBytes();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ControlCenter.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace HS.Protobuf.ControlCenter {

  /// <summary>Holder for reflection information generated from ControlCenter.proto</summary>
  public static partial class ControlCenterReflection {

    #region Descriptor
    /// <summary>File descriptor for ControlCenter.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ControlCenterReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChNDb250cm9sQ2VudGVyLnByb3RvEhlIUy5Qcm90b2J1Zi5Db250cm9sQ2Vu",
            "dGVyGh9Db21tb24vUHJvdG9Tb3VyY2UvQ29tbW9uLnByb3RvIlcKGVNlcnZl",
            "ckluZm9SZWdpc3RlclJlcXVlc3QSOgoOc2VydmVySW5mb05vZGUYASABKAsy",
            "Ii5IUy5Qcm90b2J1Zi5Db21tb24uU2VydmVySW5mb05vZGUiVQoaU2VydmVy",
            "SW5mb1JlZ2lzdGVyUmVzcG9uc2USEgoKcmVzdWx0Q29kZRgBIAEoBRIRCgly",
            "ZXN1bHRNc2cYAiABKAkSEAoIc2VydmVySWQYAyABKAUqnwEKFENvbnRyb2xD",
            "ZW50ZXJQcm90b2NsEh4KGkNPTlRST0xDRU5URVJfUFJPVE9DTF9OT05FEAAS",
            "MgotQ09OVFJPTENFTlRFUl9QUk9UT0NMX1NFUlZFUklORk9fUkVHSVNURVJf",
            "UkVREJFOEjMKLkNPTlRST0xDRU5URVJfUFJPVE9DTF9TRVJWRVJJTkZPX1JF",
            "R0lTVEVSX1JFU1AQkk5iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::HS.Protobuf.Common.CommonReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::HS.Protobuf.ControlCenter.ControlCenterProtocl), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::HS.Protobuf.ControlCenter.ServerInfoRegisterRequest), global::HS.Protobuf.ControlCenter.ServerInfoRegisterRequest.Parser, new[]{ "ServerInfoNode" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::HS.Protobuf.ControlCenter.ServerInfoRegisterResponse), global::HS.Protobuf.ControlCenter.ServerInfoRegisterResponse.Parser, new[]{ "ResultCode", "ResultMsg", "ServerId" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ControlCenterProtocl {
    [pbr::OriginalName("CONTROLCENTER_PROTOCL_NONE")] None = 0,
    /// <summary>
    /// [ServerInfoRegisterRequest]
    /// </summary>
    [pbr::OriginalName("CONTROLCENTER_PROTOCL_SERVERINFO_REGISTER_REQ")] ServerinfoRegisterReq = 10001,
    /// <summary>
    /// [ServerInfoRegisterResponse]
    /// </summary>
    [pbr::OriginalName("CONTROLCENTER_PROTOCL_SERVERINFO_REGISTER_RESP")] ServerinfoRegisterResp = 10002,
  }

  #endregion

  #region Messages
  public sealed partial class ServerInfoRegisterRequest : pb::IMessage<ServerInfoRegisterRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ServerInfoRegisterRequest> _parser = new pb::MessageParser<ServerInfoRegisterRequest>(() => new ServerInfoRegisterRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ServerInfoRegisterRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::HS.Protobuf.ControlCenter.ControlCenterReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterRequest(ServerInfoRegisterRequest other) : this() {
      serverInfoNode_ = other.serverInfoNode_ != null ? other.serverInfoNode_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterRequest Clone() {
      return new ServerInfoRegisterRequest(this);
    }

    /// <summary>Field number for the "serverInfoNode" field.</summary>
    public const int ServerInfoNodeFieldNumber = 1;
    private global::HS.Protobuf.Common.ServerInfoNode serverInfoNode_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::HS.Protobuf.Common.ServerInfoNode ServerInfoNode {
      get { return serverInfoNode_; }
      set {
        serverInfoNode_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ServerInfoRegisterRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ServerInfoRegisterRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ServerInfoNode, other.ServerInfoNode)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (serverInfoNode_ != null) hash ^= ServerInfoNode.GetHashCode();
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
      if (serverInfoNode_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ServerInfoNode);
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
      if (serverInfoNode_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ServerInfoNode);
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
      if (serverInfoNode_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ServerInfoNode);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ServerInfoRegisterRequest other) {
      if (other == null) {
        return;
      }
      if (other.serverInfoNode_ != null) {
        if (serverInfoNode_ == null) {
          ServerInfoNode = new global::HS.Protobuf.Common.ServerInfoNode();
        }
        ServerInfoNode.MergeFrom(other.ServerInfoNode);
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
          case 10: {
            if (serverInfoNode_ == null) {
              ServerInfoNode = new global::HS.Protobuf.Common.ServerInfoNode();
            }
            input.ReadMessage(ServerInfoNode);
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
          case 10: {
            if (serverInfoNode_ == null) {
              ServerInfoNode = new global::HS.Protobuf.Common.ServerInfoNode();
            }
            input.ReadMessage(ServerInfoNode);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class ServerInfoRegisterResponse : pb::IMessage<ServerInfoRegisterResponse>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ServerInfoRegisterResponse> _parser = new pb::MessageParser<ServerInfoRegisterResponse>(() => new ServerInfoRegisterResponse());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ServerInfoRegisterResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::HS.Protobuf.ControlCenter.ControlCenterReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterResponse(ServerInfoRegisterResponse other) : this() {
      resultCode_ = other.resultCode_;
      resultMsg_ = other.resultMsg_;
      serverId_ = other.serverId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ServerInfoRegisterResponse Clone() {
      return new ServerInfoRegisterResponse(this);
    }

    /// <summary>Field number for the "resultCode" field.</summary>
    public const int ResultCodeFieldNumber = 1;
    private int resultCode_;
    /// <summary>
    ///0 �ɹ� С��0��������  ����0�쳣����
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ResultCode {
      get { return resultCode_; }
      set {
        resultCode_ = value;
      }
    }

    /// <summary>Field number for the "resultMsg" field.</summary>
    public const int ResultMsgFieldNumber = 2;
    private string resultMsg_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ResultMsg {
      get { return resultMsg_; }
      set {
        resultMsg_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "serverId" field.</summary>
    public const int ServerIdFieldNumber = 3;
    private int serverId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ServerId {
      get { return serverId_; }
      set {
        serverId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ServerInfoRegisterResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ServerInfoRegisterResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ResultCode != other.ResultCode) return false;
      if (ResultMsg != other.ResultMsg) return false;
      if (ServerId != other.ServerId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ResultCode != 0) hash ^= ResultCode.GetHashCode();
      if (ResultMsg.Length != 0) hash ^= ResultMsg.GetHashCode();
      if (ServerId != 0) hash ^= ServerId.GetHashCode();
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
      if (ResultCode != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ResultCode);
      }
      if (ResultMsg.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ResultMsg);
      }
      if (ServerId != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(ServerId);
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
      if (ResultCode != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ResultCode);
      }
      if (ResultMsg.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ResultMsg);
      }
      if (ServerId != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(ServerId);
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
      if (ResultCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ResultCode);
      }
      if (ResultMsg.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ResultMsg);
      }
      if (ServerId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ServerId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ServerInfoRegisterResponse other) {
      if (other == null) {
        return;
      }
      if (other.ResultCode != 0) {
        ResultCode = other.ResultCode;
      }
      if (other.ResultMsg.Length != 0) {
        ResultMsg = other.ResultMsg;
      }
      if (other.ServerId != 0) {
        ServerId = other.ServerId;
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
            ResultCode = input.ReadInt32();
            break;
          }
          case 18: {
            ResultMsg = input.ReadString();
            break;
          }
          case 24: {
            ServerId = input.ReadInt32();
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
            ResultCode = input.ReadInt32();
            break;
          }
          case 18: {
            ResultMsg = input.ReadString();
            break;
          }
          case 24: {
            ServerId = input.ReadInt32();
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
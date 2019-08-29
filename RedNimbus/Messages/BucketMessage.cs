// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BucketMessage.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace RedNimbus.Messages {

  /// <summary>Holder for reflection information generated from BucketMessage.proto</summary>
  public static partial class BucketMessageReflection {

    #region Descriptor
    /// <summary>File descriptor for BucketMessage.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BucketMessageReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChNCdWNrZXRNZXNzYWdlLnByb3RvEhJSZWROaW1idXMuTWVzc2FnZXMieQoN",
            "QnVja2V0TWVzc2FnZRISCgpzdWNjZXNzZnVsGAEgASgIEhQKDGVycm9yTWVz",
            "c2FnZRgCIAEoCRINCgV0b2tlbhgDIAEoCRIMCgRwYXRoGAQgASgJEhMKC3Jl",
            "dHVybkl0ZW1zGAUgAygJEgwKBGZpbGUYBiABKAxiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::RedNimbus.Messages.BucketMessage), global::RedNimbus.Messages.BucketMessage.Parser, new[]{ "Successful", "ErrorMessage", "Token", "Path", "ReturnItems", "File" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BucketMessage : pb::IMessage<BucketMessage> {
    private static readonly pb::MessageParser<BucketMessage> _parser = new pb::MessageParser<BucketMessage>(() => new BucketMessage());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BucketMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::RedNimbus.Messages.BucketMessageReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BucketMessage() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BucketMessage(BucketMessage other) : this() {
      successful_ = other.successful_;
      errorMessage_ = other.errorMessage_;
      token_ = other.token_;
      path_ = other.path_;
      returnItems_ = other.returnItems_.Clone();
      file_ = other.file_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BucketMessage Clone() {
      return new BucketMessage(this);
    }

    /// <summary>Field number for the "successful" field.</summary>
    public const int SuccessfulFieldNumber = 1;
    private bool successful_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Successful {
      get { return successful_; }
      set {
        successful_ = value;
      }
    }

    /// <summary>Field number for the "errorMessage" field.</summary>
    public const int ErrorMessageFieldNumber = 2;
    private string errorMessage_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ErrorMessage {
      get { return errorMessage_; }
      set {
        errorMessage_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "token" field.</summary>
    public const int TokenFieldNumber = 3;
    private string token_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Token {
      get { return token_; }
      set {
        token_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "path" field.</summary>
    public const int PathFieldNumber = 4;
    private string path_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Path {
      get { return path_; }
      set {
        path_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "returnItems" field.</summary>
    public const int ReturnItemsFieldNumber = 5;
    private static readonly pb::FieldCodec<string> _repeated_returnItems_codec
        = pb::FieldCodec.ForString(42);
    private readonly pbc::RepeatedField<string> returnItems_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> ReturnItems {
      get { return returnItems_; }
    }

    /// <summary>Field number for the "file" field.</summary>
    public const int FileFieldNumber = 6;
    private pb::ByteString file_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString File {
      get { return file_; }
      set {
        file_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BucketMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BucketMessage other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Successful != other.Successful) return false;
      if (ErrorMessage != other.ErrorMessage) return false;
      if (Token != other.Token) return false;
      if (Path != other.Path) return false;
      if(!returnItems_.Equals(other.returnItems_)) return false;
      if (File != other.File) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Successful != false) hash ^= Successful.GetHashCode();
      if (ErrorMessage.Length != 0) hash ^= ErrorMessage.GetHashCode();
      if (Token.Length != 0) hash ^= Token.GetHashCode();
      if (Path.Length != 0) hash ^= Path.GetHashCode();
      hash ^= returnItems_.GetHashCode();
      if (File.Length != 0) hash ^= File.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Successful != false) {
        output.WriteRawTag(8);
        output.WriteBool(Successful);
      }
      if (ErrorMessage.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ErrorMessage);
      }
      if (Token.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Token);
      }
      if (Path.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Path);
      }
      returnItems_.WriteTo(output, _repeated_returnItems_codec);
      if (File.Length != 0) {
        output.WriteRawTag(50);
        output.WriteBytes(File);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Successful != false) {
        size += 1 + 1;
      }
      if (ErrorMessage.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ErrorMessage);
      }
      if (Token.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Token);
      }
      if (Path.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Path);
      }
      size += returnItems_.CalculateSize(_repeated_returnItems_codec);
      if (File.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(File);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BucketMessage other) {
      if (other == null) {
        return;
      }
      if (other.Successful != false) {
        Successful = other.Successful;
      }
      if (other.ErrorMessage.Length != 0) {
        ErrorMessage = other.ErrorMessage;
      }
      if (other.Token.Length != 0) {
        Token = other.Token;
      }
      if (other.Path.Length != 0) {
        Path = other.Path;
      }
      returnItems_.Add(other.returnItems_);
      if (other.File.Length != 0) {
        File = other.File;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Successful = input.ReadBool();
            break;
          }
          case 18: {
            ErrorMessage = input.ReadString();
            break;
          }
          case 26: {
            Token = input.ReadString();
            break;
          }
          case 34: {
            Path = input.ReadString();
            break;
          }
          case 42: {
            returnItems_.AddEntriesFrom(input, _repeated_returnItems_codec);
            break;
          }
          case 50: {
            File = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
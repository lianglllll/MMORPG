// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: message.proto

// This CPP symbol can be defined to use imports that match up to the framework
// imports needed when using CocoaPods.
#if !defined(GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS)
 #define GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS 0
#endif

#if GPB_USE_PROTOBUF_FRAMEWORK_IMPORTS
 #import <Protobuf/GPBProtocolBuffers_RuntimeSupport.h>
#else
 #import "GPBProtocolBuffers_RuntimeSupport.h"
#endif

#import "Message.pbobjc.h"
// @@protoc_insertion_point(imports)

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#pragma clang diagnostic ignored "-Wdollar-in-identifier-extension"

#pragma mark - Objective C Class declarations
// Forward declarations of Objective C classes that we can use as
// static values in struct initializers.
// We don't use [Foo class] because it is not a static value.
GPBObjCClassDeclaration(Request);
GPBObjCClassDeclaration(Response);
GPBObjCClassDeclaration(UserLoginRequest);
GPBObjCClassDeclaration(UserLoginResponse);
GPBObjCClassDeclaration(UserRegisterRequest);
GPBObjCClassDeclaration(UserRegisterResponse);
GPBObjCClassDeclaration(Vector3);

#pragma mark - MessageRoot

@implementation MessageRoot

// No extensions in the file and no imports, so no need to generate
// +extensionRegistry.

@end

#pragma mark - MessageRoot_FileDescriptor

static GPBFileDescriptor *MessageRoot_FileDescriptor(void) {
  // This is called by +initialize so there is no need to worry
  // about thread safety of the singleton.
  static GPBFileDescriptor *descriptor = NULL;
  if (!descriptor) {
    GPB_DEBUG_CHECK_RUNTIME_VERSIONS();
    descriptor = [[GPBFileDescriptor alloc] initWithPackage:@"proto"
                                                     syntax:GPBFileSyntaxProto3];
  }
  return descriptor;
}

#pragma mark - Vector3

@implementation Vector3

@dynamic x;
@dynamic y;
@dynamic z;

typedef struct Vector3__storage_ {
  uint32_t _has_storage_[1];
  int32_t x;
  int32_t y;
  int32_t z;
} Vector3__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "x",
        .dataTypeSpecific.clazz = Nil,
        .number = Vector3_FieldNumber_X,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Vector3__storage_, x),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "y",
        .dataTypeSpecific.clazz = Nil,
        .number = Vector3_FieldNumber_Y,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Vector3__storage_, y),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "z",
        .dataTypeSpecific.clazz = Nil,
        .number = Vector3_FieldNumber_Z,
        .hasIndex = 2,
        .offset = (uint32_t)offsetof(Vector3__storage_, z),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Vector3 class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Vector3__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Entity

@implementation Entity

@dynamic id_p;
@dynamic speed;
@dynamic hasPosition, position;
@dynamic hasRotation, rotation;

typedef struct Entity__storage_ {
  uint32_t _has_storage_[1];
  int32_t id_p;
  int32_t speed;
  Vector3 *position;
  Vector3 *rotation;
} Entity__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "id_p",
        .dataTypeSpecific.clazz = Nil,
        .number = Entity_FieldNumber_Id_p,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Entity__storage_, id_p),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "speed",
        .dataTypeSpecific.clazz = Nil,
        .number = Entity_FieldNumber_Speed,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Entity__storage_, speed),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "position",
        .dataTypeSpecific.clazz = GPBObjCClass(Vector3),
        .number = Entity_FieldNumber_Position,
        .hasIndex = 2,
        .offset = (uint32_t)offsetof(Entity__storage_, position),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "rotation",
        .dataTypeSpecific.clazz = GPBObjCClass(Vector3),
        .number = Entity_FieldNumber_Rotation,
        .hasIndex = 3,
        .offset = (uint32_t)offsetof(Entity__storage_, rotation),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Entity class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Entity__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Package

@implementation Package

@dynamic hasRequest, request;
@dynamic hasResponse, response;

typedef struct Package__storage_ {
  uint32_t _has_storage_[1];
  Request *request;
  Response *response;
} Package__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "request",
        .dataTypeSpecific.clazz = GPBObjCClass(Request),
        .number = Package_FieldNumber_Request,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Package__storage_, request),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "response",
        .dataTypeSpecific.clazz = GPBObjCClass(Response),
        .number = Package_FieldNumber_Response,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Package__storage_, response),
        .flags = GPBFieldOptional,
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Package class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Package__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Request

@implementation Request

@dynamic hasUserRegister, userRegister;
@dynamic hasUserLogin, userLogin;

typedef struct Request__storage_ {
  uint32_t _has_storage_[1];
  UserRegisterRequest *userRegister;
  UserLoginRequest *userLogin;
} Request__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "userRegister",
        .dataTypeSpecific.clazz = GPBObjCClass(UserRegisterRequest),
        .number = Request_FieldNumber_UserRegister,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Request__storage_, userRegister),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldTextFormatNameCustom),
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "userLogin",
        .dataTypeSpecific.clazz = GPBObjCClass(UserLoginRequest),
        .number = Request_FieldNumber_UserLogin,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Request__storage_, userLogin),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldTextFormatNameCustom),
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Request class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Request__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
#if !GPBOBJC_SKIP_MESSAGE_TEXTFORMAT_EXTRAS
    static const char *extraTextFormatInfo =
        "\002\001\014\000\002\t\000";
    [localDescriptor setupExtraTextInfo:extraTextFormatInfo];
#endif  // !GPBOBJC_SKIP_MESSAGE_TEXTFORMAT_EXTRAS
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - Response

@implementation Response

@dynamic hasUserRegister, userRegister;
@dynamic hasUserLogin, userLogin;

typedef struct Response__storage_ {
  uint32_t _has_storage_[1];
  UserRegisterResponse *userRegister;
  UserLoginResponse *userLogin;
} Response__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "userRegister",
        .dataTypeSpecific.clazz = GPBObjCClass(UserRegisterResponse),
        .number = Response_FieldNumber_UserRegister,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(Response__storage_, userRegister),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldTextFormatNameCustom),
        .dataType = GPBDataTypeMessage,
      },
      {
        .name = "userLogin",
        .dataTypeSpecific.clazz = GPBObjCClass(UserLoginResponse),
        .number = Response_FieldNumber_UserLogin,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(Response__storage_, userLogin),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldTextFormatNameCustom),
        .dataType = GPBDataTypeMessage,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[Response class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(Response__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
#if !GPBOBJC_SKIP_MESSAGE_TEXTFORMAT_EXTRAS
    static const char *extraTextFormatInfo =
        "\002\001\014\000\002\t\000";
    [localDescriptor setupExtraTextInfo:extraTextFormatInfo];
#endif  // !GPBOBJC_SKIP_MESSAGE_TEXTFORMAT_EXTRAS
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserRegisterRequest

@implementation UserRegisterRequest

@dynamic username;
@dynamic password;

typedef struct UserRegisterRequest__storage_ {
  uint32_t _has_storage_[1];
  NSString *username;
  NSString *password;
} UserRegisterRequest__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "username",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterRequest_FieldNumber_Username,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserRegisterRequest__storage_, username),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "password",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterRequest_FieldNumber_Password,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserRegisterRequest__storage_, password),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserRegisterRequest class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserRegisterRequest__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserRegisterResponse

@implementation UserRegisterResponse

@dynamic code;
@dynamic message;

typedef struct UserRegisterResponse__storage_ {
  uint32_t _has_storage_[1];
  int32_t code;
  NSString *message;
} UserRegisterResponse__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "code",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterResponse_FieldNumber_Code,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserRegisterResponse__storage_, code),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "message",
        .dataTypeSpecific.clazz = Nil,
        .number = UserRegisterResponse_FieldNumber_Message,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserRegisterResponse__storage_, message),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserRegisterResponse class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserRegisterResponse__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserLoginRequest

@implementation UserLoginRequest

@dynamic username;
@dynamic password;

typedef struct UserLoginRequest__storage_ {
  uint32_t _has_storage_[1];
  NSString *username;
  NSString *password;
} UserLoginRequest__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "username",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginRequest_FieldNumber_Username,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserLoginRequest__storage_, username),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
      {
        .name = "password",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginRequest_FieldNumber_Password,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserLoginRequest__storage_, password),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserLoginRequest class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserLoginRequest__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end

#pragma mark - UserLoginResponse

@implementation UserLoginResponse

@dynamic code;
@dynamic message;

typedef struct UserLoginResponse__storage_ {
  uint32_t _has_storage_[1];
  int32_t code;
  NSString *message;
} UserLoginResponse__storage_;

// This method is threadsafe because it is initially called
// in +initialize for each subclass.
+ (GPBDescriptor *)descriptor {
  static GPBDescriptor *descriptor = nil;
  if (!descriptor) {
    static GPBMessageFieldDescription fields[] = {
      {
        .name = "code",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginResponse_FieldNumber_Code,
        .hasIndex = 0,
        .offset = (uint32_t)offsetof(UserLoginResponse__storage_, code),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeInt32,
      },
      {
        .name = "message",
        .dataTypeSpecific.clazz = Nil,
        .number = UserLoginResponse_FieldNumber_Message,
        .hasIndex = 1,
        .offset = (uint32_t)offsetof(UserLoginResponse__storage_, message),
        .flags = (GPBFieldFlags)(GPBFieldOptional | GPBFieldClearHasIvarOnZero),
        .dataType = GPBDataTypeString,
      },
    };
    GPBDescriptor *localDescriptor =
        [GPBDescriptor allocDescriptorForClass:[UserLoginResponse class]
                                     rootClass:[MessageRoot class]
                                          file:MessageRoot_FileDescriptor()
                                        fields:fields
                                    fieldCount:(uint32_t)(sizeof(fields) / sizeof(GPBMessageFieldDescription))
                                   storageSize:sizeof(UserLoginResponse__storage_)
                                         flags:(GPBDescriptorInitializationFlags)(GPBDescriptorInitializationFlag_UsesClassRefs | GPBDescriptorInitializationFlag_Proto3OptionalKnown)];
    #if defined(DEBUG) && DEBUG
      NSAssert(descriptor == nil, @"Startup recursed!");
    #endif  // DEBUG
    descriptor = localDescriptor;
  }
  return descriptor;
}

@end


#pragma clang diagnostic pop

// @@protoc_insertion_point(global_scope)

using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    public class UserCreateInputType : InputObjectType<UserWithoutIsActiveDTO>
    {
        protected override void Configure(IInputObjectTypeDescriptor<UserWithoutIsActiveDTO> descriptor)
        {
            descriptor.Field(u => u.firstName).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.lastName).Type<StringType>();
            descriptor.Field(u => u.email).Type<StringType>();
            descriptor.Field(u => u.password).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.address).Type<StringType>();
        }
    }

    public class UserUpdateInputType : InputObjectType<UserUpdateDTO>
    {
        protected override void Configure(IInputObjectTypeDescriptor<UserUpdateDTO> descriptor)
        {
            descriptor.Field(u => u.firstName).Type<StringType>();
            descriptor.Field(u => u.lastName).Type<StringType>();
            descriptor.Field(u => u.email).Type<StringType>();
            descriptor.Field(u => u.password).Type<StringType>();
            descriptor.Field(u => u.address).Type<StringType>();
        }
    }
} 
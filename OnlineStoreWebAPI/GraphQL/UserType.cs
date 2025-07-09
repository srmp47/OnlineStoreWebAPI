using HotChocolate.Types;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.GraphQL
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(u => u.userId).Type<NonNullType<IdType>>();
            descriptor.Field(u => u.firstName).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.lastName).Type<StringType>();
            descriptor.Field(u => u.email).Type<StringType>();
            descriptor.Field(u => u.password).Ignore(); // Don't expose password
            descriptor.Field(u => u.address).Type<StringType>();
            descriptor.Field(u => u.isActive).Type<NonNullType<BooleanType>>();
            descriptor.Field(u => u.orders).Ignore(); // Ignore orders for now
        }
    }
} 
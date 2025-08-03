using AutoMapper;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Helper
{
    public class ServiceTestBase
    {
        protected readonly IMapper Mapper;
        protected readonly Mock<ILogger> MockLogger;

        protected ServiceTestBase()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserUpdateDTO, User>();
                cfg.CreateMap<User, UserUpdateDTO>();
                cfg.CreateMap<User, UserWithoutOrdersDTO>();
                cfg.CreateMap<UserWithoutIsActiveDTO, User>();
                cfg.CreateMap<Product, ProductUpdateDTO>();
                cfg.CreateMap<ProductUpdateDTO, Product>();
            });

            Mapper = mapperConfig.CreateMapper();
            MockLogger = new Mock<ILogger>();
        }
    

        protected Mock<T> CreateMock<T>() where T : class
        {
            return new Mock<T>();
        }

        protected void AssertSuccess<T>(T result) where T : class
        {
            result.Should().NotBeNull();
        }

        protected void AssertFailure<T>(T result) where T : class
        {
            result.Should().BeNull();
        }

        protected void AssertTrue(bool condition)
        {
            condition.Should().BeTrue();
        }

        protected void AssertFalse(bool condition)
        {
            condition.Should().BeFalse();
        }
    }
}

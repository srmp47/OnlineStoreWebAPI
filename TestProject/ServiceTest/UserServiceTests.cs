using AutoMapper;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Helper;
using TestProject.Helper.Mock;

namespace TestProject.ServiceTest
{
    public class UserServiceTests : ServiceTestBase
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockRepo = CreateMock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object, Mapper);
        }

        [Fact]
        public async Task ActivateUserByIdAsync_UserExists_ActivatesUser()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            user.isActive = false;
            _mockRepo.Setup(r => r.GetUserByIdAsync(user.userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.activateUserByIdAsync(user.userId);

            // Assert
            result.Should().BeTrue();
            user.isActive.Should().BeTrue();
            _mockRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task ActivateUserByIdAsync_UserNotExists_ReturnsFalse()
        {
            // Arrange
            int userId = 999;
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.activateUserByIdAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CreateNewUserAsync_CallsRepository()
        {
            // Arrange
            var newUser = MockData.CreateMockUser();
            _mockRepo.Setup(r => r.CreateUserAsync(newUser)).Returns(Task.CompletedTask);

            // Act
            await _userService.createNewUserAsync(newUser);

            // Assert
            _mockRepo.Verify(r => r.CreateUserAsync(newUser), Times.Once);
        }

        [Fact]
        public async Task DeactivateUserByIdAsync_UserExists_DeactivatesUser()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            _mockRepo.Setup(r => r.GetUserByIdAsync(user.userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.deActivateUserByUserIdAsync(user.userId);

            // Assert
            result.Should().BeTrue();
            user.isActive.Should().BeFalse();
            _mockRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_UserExists_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _mockRepo.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.deleteUserByIdAsync(userId);

            // Assert
            result.Should().BeTrue();
            _mockRepo.Verify(r => r.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            // Arrange
            var users = new List<User> { MockData.CreateMockUser(), MockData.CreateMockUser() };
            _mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var pagination = new PaginationParameters { PageId = 1, PageSize = 2 };
            var result = await _userService.getAllUsersAsync(pagination);

            // Assert
            result.Should().HaveCount(2);
            _mockRepo.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }
        [Fact]
        public async Task IsActiveUserWithIdAsync_ActiveUser_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _mockRepo.Setup(r => r.isActiveUserWithId(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.isActiveUserWithIdAsync(userId);

            // Assert
            result.Should().BeTrue();
            _mockRepo.Verify(r => r.isActiveUserWithId(userId), Times.Once);
        }
        [Fact]
        public async Task DeActivateUserByUserIdAsync_WithInvalidId_ReturnFalse()
        {
            // Arrange
            int userId = 999;
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.deActivateUserByUserIdAsync(userId);

            // Assert
            result.Should().BeFalse();
            _mockRepo.Verify(r => r.GetUserByIdAsync(userId), Times.Once);

        }

        [Fact]
        public async Task getUserByIdAsync_WithValidId_RerutnUser()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            _mockRepo.Setup(r => r.GetUserByIdAsync(user.userId)).ReturnsAsync(user);


            // Act
            var result = await _userService.getUserByIdAsync(user.userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(user);
            _mockRepo.Verify(r => r.GetUserByIdAsync(user.userId), Times.Once);

        }
        [Fact]
        public async Task getUserByIdAsync_WithInValidId_RerutnNull()
        {
            // Arrange
            int userId = 999;
            _mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.getUserByIdAsync(userId);

            // Assert
            result.Should().BeNull();
            _mockRepo.Verify(r => r.GetUserByIdAsync(userId), Times.Once);

        }
        [Fact]
        public async Task isThereUserWithIdAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _mockRepo.Setup(r => r.isThereUserWithId(userId)).ReturnsAsync(true);
            // Act
            var result = await _userService.isThereUserWithIdAsync(userId);
            // Assert
            result.Should().BeTrue();
            _mockRepo.Verify(r => r.isThereUserWithId(userId), Times.Once);
        }
        [Fact]
        public async Task isThereUserWithIdAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            int userId = 999;
            _mockRepo.Setup(r => r.isThereUserWithId(userId)).ReturnsAsync(false);
            // Act
            var result = await _userService.isThereUserWithIdAsync(userId);
            // Assert
            result.Should().BeFalse();
            _mockRepo.Verify(r => r.isThereUserWithId(userId), Times.Once);
        }
        [Fact]
        public async Task isActiveUserWithIdAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _mockRepo.Setup(r => r.isActiveUserWithId(userId)).ReturnsAsync(true);
            // Act
            var result = await _userService.isActiveUserWithIdAsync(userId);
            // Assert
            result.Should().BeTrue();
            _mockRepo.Verify(r => r.isActiveUserWithId(userId), Times.Once);
        }
        [Fact]
        public async Task isActiveUserWithIdAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            int userId = 999;
            _mockRepo.Setup(r => r.isActiveUserWithId(userId)).ReturnsAsync(false);
            // Act
            var result = await _userService.isActiveUserWithIdAsync(userId);
            // Assert
            result.Should().BeFalse();
            _mockRepo.Verify(r => r.isActiveUserWithId(userId), Times.Once);
        }
        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser()
        {
            // Arrange
            var userId = 50;
            var originalUser = MockData.CreateMockUser();
            originalUser.userId = userId;
            var newUserDto = MockData.CreateMockUserDto();
            _mockRepo.Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.updateUserAsync(userId, newUserDto);

            // Assert
            _mockRepo.Verify(r => r.UpdateUserAsync(It.Is<User>(u =>
                u.userId == userId &&
                u.address == newUserDto.address &&
                u.email == newUserDto.email &&
                u.firstName == newUserDto.firstName &&
                u.lastName == newUserDto.lastName &&
                u.password == newUserDto.password 
            )), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(userId, result.userId);
            // Add more assertions for mapped properties
        }
    }
}

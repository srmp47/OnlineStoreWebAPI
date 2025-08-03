using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.Repository;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DBContext;
using TestProject.Helper;
using TestProject.Helper.Mock;
using FluentAssertions;
using Xunit;

namespace TestProject.Repository
{
    public class UserRepositoryTests : TestBase
    {
        private readonly UserRepository _userRepository;
        private readonly OnlineStoreDBContext _context;

        public UserRepositoryTests()
        {
            _context = GetDbContext();
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = MockData.CreateMockUsers(3);
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(user.userId);

            // Assert
            result.Should().NotBeNull();
            result.userId.Should().Be(user.userId);
            result.firstName.Should().Be(user.firstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _userRepository.GetUserByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = MockData.CreateMockUser();

            // Act
            await _userRepository.CreateUserAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            user.userId.Should().BeGreaterThan(0);
            
            var savedUser = await _context.Users.FindAsync(user.userId);
            savedUser.Should().NotBeNull();
            savedUser.firstName.Should().Be(user.firstName);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var updatedFirstName = "UpdatedFirstName";
            user.firstName = updatedFirstName;

            // Act
            await _userRepository.UpdateUserAsync(user);

            // Assert
            var savedUser = await _context.Users.FindAsync(user.userId);
            savedUser.Should().NotBeNull();
            savedUser.firstName.Should().Be(updatedFirstName);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.DeleteUserAsync(user.userId);
            await _context.SaveChangesAsync();

            // Assert
            result.Should().BeTrue();
            
            var deletedUser = await _context.Users.FindAsync(user.userId);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task IsThereUserWithId_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.isThereUserWithId(user.userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsThereUserWithId_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _userRepository.isThereUserWithId(invalidId);

            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task DeleteUserAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _userRepository.DeleteUserAsync(invalidId);
            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task isActiveUserAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            user.isActive = true;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            // Act
            var result = await _userRepository.isActiveUserWithId(user.userId);
            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task isActiveUserAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _userRepository.isActiveUserWithId(invalidId);
            // Assert
            result.Should().BeFalse();
        }
    }
} 
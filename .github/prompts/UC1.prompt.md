# UC1: 帳號註冊 - TDD 紅燈測試設計

## 📋 需求規格

### 功能描述
- **Use Case**: UC1 註冊帳號
- **Actor**: 租車用戶
- **前置條件**: 無
- **後置條件**: 帳戶建立成功，可進行登入
- **業務流程**: `RegisterUser → 建立 User Aggregate → 儲存至 UserRepository`

### 業務規則

#### 必填欄位
- `Username`: 用戶名稱（3-50 字元，僅允許英數字和底線）
- `Email`: 電子信箱（必須為有效格式）
- `Password`: 密碼（至少 8 字元，必須包含大小寫字母、數字和特殊字元）
- `Sex`: 性別（選填，男/女/其他）
- `Occupation`: 職業（選填）
- `PhoneNumber`: 電話號碼（選填，必須為有效格式）
- `DateOfBirth`: 出生日期（選填，必須為過去日期）
- `Address`: 地址（選填）

#### 唯一性約束
- `Username` 必須唯一（不區分大小寫）
- `Email` 必須唯一（不區分大小寫）

#### 安全性
- 密碼必須經過雜湊處理後儲存（使用 BCrypt）
- API 回應不得包含密碼（明文或雜湊值）

---

## 🧪 TDD 紅燈測試案例設計

### 測試實作順序

```
階段 1: Domain Layer (由內而外)
  ├─ Phase 1A: Login 登入帳號與身分檢核 （一般使用者/VIP 使用者/管理者）
  ├─ Phase 1B: Password Value Object
  └─ Phase 1C: User Entity

階段 2: Application Layer
  ├─ Phase 2A: RegisterUserUseCase (Happy Path)
  └─ Phase 2B: RegisterUserUseCase (Error Handling)

階段 3: Infrastructure Layer
  └─ Phase 3A: UserRepositoryAdapter

階段 4: API Layer
  └─ Phase 4A: Integration Tests
```

---

## 🟢 階段 1A: Domain Layer - Email Value Object Tests

### 測試專案
- **位置**: `RentalCar.Domain.Tests/ValueObjects/EmailTests.cs`
- **框架**: xUnit + FluentAssertions

### 測試案例

#### ✅ TC-D01: 正常路徑 - 建立有效 Email
```csharp
[Theory]
[InlineData("user@example.com")]
[InlineData("test.user@company.co.uk")]
[InlineData("admin+tag@domain.org")]
public void Should_CreateEmail_WhenValidFormatProvided(string validEmail)
{
    // Arrange & Act
    var email = new Email(validEmail);

    // Assert
    email.Should().NotBeNull();
    email.Value.Should().Be(validEmail.ToLowerInvariant());
}
```

**預期結果**: ❌ 紅燈 - `Email` 類別尚未定義

---

#### ❌ TC-D02: 驗證失敗 - Email 為 null
```csharp
[Fact]
public void Should_ThrowArgumentNullException_WhenEmailIsNull()
{
    // Arrange
    string nullEmail = null;

    // Act
    Action act = () => new Email(nullEmail);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithMessage("*email*");
}
```

**預期結果**: ❌ 紅燈 - `Email` 類別不存在且未實作驗證

---

#### ❌ TC-D03: 驗證失敗 - Email 為空字串
```csharp
[Theory]
[InlineData("")]
[InlineData("   ")]
public void Should_ThrowArgumentException_WhenEmailIsEmpty(string emptyEmail)
{
    // Arrange & Act
    Action act = () => new Email(emptyEmail);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*不能為空*");
}
```

**預期結果**: ❌ 紅燈 - 未實作空值驗證

---

#### ❌ TC-D04: 驗證失敗 - Email 格式錯誤
```csharp
[Theory]
[InlineData("invalid-email")]           // 缺少 @
[InlineData("@example.com")]            // 缺少本地部分
[InlineData("user@")]                   // 缺少域名
[InlineData("user @example.com")]       // 包含空格
[InlineData("user@domain")]             // 缺少頂級域名
[InlineData("user..name@example.com")]  // 連續點號
public void Should_ThrowArgumentException_WhenEmailFormatIsInvalid(string invalidEmail)
{
    // Arrange & Act
    Action act = () => new Email(invalidEmail);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*Email 格式不正確*");
}
```

**預期結果**: ❌ 紅燈 - 未實作格式驗證邏輯

---

#### ✅ TC-D05: Email 正規化 - 自動轉小寫
```csharp
[Fact]
public void Should_NormalizeEmailToLowerCase_WhenCreated()
{
    // Arrange
    var mixedCaseEmail = "User@Example.COM";

    // Act
    var email = new Email(mixedCaseEmail);

    // Assert
    email.Value.Should().Be("user@example.com");
}
```

**預期結果**: ❌ 紅燈 - 未實作正規化邏輯

---

#### ✅ TC-D06: Value Object 相等性比較
```csharp
[Fact]
public void Should_BeEqual_WhenEmailValuesAreTheSame()
{
    // Arrange
    var email1 = new Email("test@example.com");
    var email2 = new Email("test@example.com");

    // Act & Assert
    email1.Should().Be(email2);
    (email1 == email2).Should().BeTrue();
    email1.GetHashCode().Should().Be(email2.GetHashCode());
}
```

**預期結果**: ❌ 紅燈 - 未實作 Value Object 相等性

---

## 🟡 階段 1B: Domain Layer - Password Value Object Tests

### 測試專案
- **位置**: `RentalCar.Domain.Tests/ValueObjects/PasswordTests.cs`

### 測試案例

#### ✅ TC-D07: 正常路徑 - 建立有效密碼
```csharp
[Theory]
[InlineData("Test@1234")]
[InlineData("MyP@ssw0rd!")]
[InlineData("Secure#Pass99")]
public void Should_CreatePassword_WhenValidPasswordProvided(string validPassword)
{
    // Arrange & Act
    var password = Password.Create(validPassword);

    // Assert
    password.Should().NotBeNull();
    password.HashedValue.Should().NotBeNullOrEmpty();
    password.HashedValue.Should().NotBe(validPassword); // 確保已雜湊
}
```

**預期結果**: ❌ 紅燈 - `Password` 類別不存在

---

#### ❌ TC-D08: 驗證失敗 - 密碼為 null 或空
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void Should_ThrowArgumentException_WhenPasswordIsNullOrEmpty(string invalidPassword)
{
    // Arrange & Act
    Action act = () => Password.Create(invalidPassword);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*密碼不能為空*");
}
```

**預期結果**: ❌ 紅燈 - 未實作空值驗證

---

#### ❌ TC-D09: 驗證失敗 - 密碼長度不足
```csharp
[Theory]
[InlineData("Test@1")]      // 6 字元
[InlineData("Abc@12")]      // 6 字元
[InlineData("Pass@1")]      // 6 字元
public void Should_ThrowArgumentException_WhenPasswordIsTooShort(string shortPassword)
{
    // Arrange & Act
    Action act = () => Password.Create(shortPassword);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*至少 8 個字元*");
}
```

**預期結果**: ❌ 紅燈 - 未實作長度驗證

---

#### ❌ TC-D10: 驗證失敗 - 密碼缺少大寫字母
```csharp
[Theory]
[InlineData("test@1234")]
[InlineData("mypassword@99")]
public void Should_ThrowArgumentException_WhenPasswordHasNoUpperCase(string password)
{
    // Arrange & Act
    Action act = () => Password.Create(password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*至少包含一個大寫字母*");
}
```

**預期結果**: ❌ 紅燈 - 未實作大寫字母驗證

---

#### ❌ TC-D11: 驗證失敗 - 密碼缺少小寫字母
```csharp
[Theory]
[InlineData("TEST@1234")]
[InlineData("MYPASSWORD@99")]
public void Should_ThrowArgumentException_WhenPasswordHasNoLowerCase(string password)
{
    // Arrange & Act
    Action act = () => Password.Create(password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*至少包含一個小寫字母*");
}
```

**預期結果**: ❌ 紅燈 - 未實作小寫字母驗證

---

#### ❌ TC-D12: 驗證失敗 - 密碼缺少數字
```csharp
[Theory]
[InlineData("Test@Password")]
[InlineData("MyPass@word")]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange & Act
    Action act = () => Password.Create(password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*至少包含一個數字*");
}
```

**預期結果**: ❌ 紅燈 - 未實作數字驗證

---

#### ❌ TC-D13: 驗證失敗 - 密碼缺少特殊字元
```csharp
[Theory]
[InlineData("Test1234")]
[InlineData("MyPassword99")]
public void Should_ThrowArgumentException_WhenPasswordHasNoSpecialChar(string password)
{
    // Arrange & Act
    Action act = () => Password.Create(password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*至少包含一個特殊字元*");
}
```

**預期結果**: ❌ 紅燈 - 未實作特殊字元驗證

---

#### ✅ TC-D14: 密碼雜湊 - 使用 BCrypt
```csharp
[Fact]
public void Should_HashPasswordUsingBCrypt_WhenCreated()
{
    // Arrange
    var plainPassword = "Test@1234";

    // Act
    var password = Password.Create(plainPassword);

    // Assert
    password.HashedValue.Should().StartWith("$2"); // BCrypt 雜湊前綴
    password.HashedValue.Length.Should().Be(60);   // BCrypt 固定長度
}
```

**預期結果**: ❌ 紅燈 - 未實作雜湊邏輯

---

#### ✅ TC-D15: 密碼驗證 - 正確密碼比對成功
```csharp
[Fact]
public void Should_VerifyPassword_WhenCorrectPasswordProvided()
{
    // Arrange
    var plainPassword = "Test@1234";
    var password = Password.Create(plainPassword);

    // Act
    var isValid = password.Verify(plainPassword);

    // Assert
    isValid.Should().BeTrue();
}
```

**預期結果**: ❌ 紅燈 - 未實作驗證方法

---

#### ❌ TC-D16: 密碼驗證 - 錯誤密碼比對失敗
```csharp
[Fact]
public void Should_FailVerification_WhenWrongPasswordProvided()
{
    // Arrange
    var password = Password.Create("Test@1234");

    // Act
    var isValid = password.Verify("WrongPassword@99");

    // Assert
    isValid.Should().BeFalse();
}
```

**預期結果**: ❌ 紅燈 - 未實作驗證邏輯

---

## 🔵 階段 1C: Domain Layer - User Entity Tests

### 測試專案
- **位置**: `RentalCar.Domain.Tests/Entities/UserTests.cs`

### 測試案例

#### ✅ TC-D17: 正常路徑 - 建立有效 User
```csharp
[Fact]
public void Should_CreateUser_WhenValidDataProvided()
{
    // Arrange
    var userId = Guid.NewGuid();
    var username = "testuser";
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    var user = new User(userId, username, email, password);

    // Assert
    user.Should().NotBeNull();
    user.UserId.Should().Be(userId);
    user.Username.Should().Be(username);
    user.Email.Should().Be(email);
    user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
}
```

**預期結果**: ❌ 紅燈 - `User` 類別不存在

---

#### ❌ TC-D18: 驗證失敗 - UserId 為空
```csharp
[Fact]
public void Should_ThrowArgumentException_WhenUserIdIsEmpty()
{
    // Arrange
    var emptyGuid = Guid.Empty;
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(emptyGuid, "testuser", email, password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*UserId 不能為空*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 UserId 驗證

---

#### ❌ TC-D19: 驗證失敗 - Username 為 null 或空
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void Should_ThrowArgumentException_WhenUsernameIsNullOrEmpty(string invalidUsername)
{
    // Arrange
    var userId = Guid.NewGuid();
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(userId, invalidUsername, email, password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*用戶名不能為空*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 Username 空值驗證

---

#### ❌ TC-D20: 驗證失敗 - Username 太短
```csharp
[Theory]
[InlineData("ab")]      // 2 字元
[InlineData("u")]       // 1 字元
public void Should_ThrowArgumentException_WhenUsernameTooShort(string shortUsername)
{
    // Arrange
    var userId = Guid.NewGuid();
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(userId, shortUsername, email, password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*用戶名長度必須在 3 到 50 個字元之間*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 Username 長度驗證

---

#### ❌ TC-D21: 驗證失敗 - Username 太長
```csharp
[Fact]
public void Should_ThrowArgumentException_WhenUsernameTooLong()
{
    // Arrange
    var userId = Guid.NewGuid();
    var longUsername = new string('a', 51); // 51 字元
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(userId, longUsername, email, password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*用戶名長度必須在 3 到 50 個字元之間*");
}
```

**預期結果**: ❌ 紅燈 - 未實作長度上限驗證

---

#### ❌ TC-D22: 驗證失敗 - Username 包含非法字元
```csharp
[Theory]
[InlineData("user@name")]       // 包含 @
[InlineData("user name")]       // 包含空格
[InlineData("user-name")]       // 包含連字號
[InlineData("用戶名稱")]         // 包含中文
[InlineData("user#123")]        // 包含特殊字元
public void Should_ThrowArgumentException_WhenUsernameContainsInvalidChars(string invalidUsername)
{
    // Arrange
    var userId = Guid.NewGuid();
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(userId, invalidUsername, email, password);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*用戶名只能包含英數字和底線*");
}
```

**預期結果**: ❌ 紅燈 - 未實作字元驗證

---

#### ✅ TC-D23: Username 正規化 - 自動轉小寫
```csharp
[Fact]
public void Should_NormalizeUsernameToLowerCase_WhenCreated()
{
    // Arrange
    var userId = Guid.NewGuid();
    var mixedCaseUsername = "TestUser";
    var email = new Email("test@example.com");
    var password = Password.Create("Test@1234");

    // Act
    var user = new User(userId, mixedCaseUsername, email, password);

    // Assert
    user.Username.Should().Be("testuser");
}
```

**預期結果**: ❌ 紅燈 - 未實作正規化邏輯

---

#### ❌ TC-D24: 驗證失敗 - Email 為 null
```csharp
[Fact]
public void Should_ThrowArgumentNullException_WhenEmailIsNull()
{
    // Arrange
    var userId = Guid.NewGuid();
    Email nullEmail = null;
    var password = Password.Create("Test@1234");

    // Act
    Action act = () => new User(userId, "testuser", nullEmail, password);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithMessage("*email*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 Email 空值檢查

---

#### ❌ TC-D25: 驗證失敗 - Password 為 null
```csharp
[Fact]
public void Should_ThrowArgumentNullException_WhenPasswordIsNull()
{
    // Arrange
    var userId = Guid.NewGuid();
    var email = new Email("test@example.com");
    Password nullPassword = null;

    // Act
    Action act = () => new User(userId, "testuser", email, nullPassword);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithMessage("*password*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 Password 空值檢查

---

## 🟠 階段 2A: Application Layer - RegisterUserUseCase Tests (Happy Path)

### 測試專案
- **位置**: `RentalCar.Application.Tests/UseCases/RegisterUserUseCaseTests.cs`
- **Mocking 框架**: Moq

### 測試案例

#### ✅ TC-A01: 正常路徑 - 成功註冊用戶
```csharp
[Fact]
public async Task Should_CreateUser_WhenValidRequestProvided()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
    mockRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    var result = await useCase.ExecuteAsync(request);

    // Assert
    result.Should().NotBeNull();
    result.UserId.Should().NotBeEmpty();
    result.Username.Should().Be("testuser");
    result.Email.Should().Be("test@example.com");
}
```

**預期結果**: ❌ 紅燈 - `RegisterUserUseCase` 不存在

---

#### ✅ TC-A02: Repository 互動 - 應呼叫 AddAsync 一次
```csharp
[Fact]
public async Task Should_CallRepositoryAddAsync_ExactlyOnce()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
    mockRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    await useCase.ExecuteAsync(request);

    // Assert
    mockRepo.Verify(r => r.AddAsync(It.Is<User>(u => 
        u.Username == "testuser" && 
        u.Email.Value == "test@example.com"
    )), Times.Once);
}
```

**預期結果**: ❌ 紅燈 - 未實作 Repository 呼叫

---

#### ✅ TC-A03: 安全性 - 回應不包含密碼
```csharp
[Fact]
public async Task Should_NotReturnPassword_InResponse()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
    mockRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    var result = await useCase.ExecuteAsync(request);

    // Assert
    var resultType = result.GetType();
    resultType.GetProperty("Password").Should().BeNull();
    resultType.GetProperty("PasswordHash").Should().BeNull();
}
```

**預期結果**: ❌ 紅燈 - DTO 未定義或包含密碼欄位

---

## 🔴 階段 2B: Application Layer - RegisterUserUseCase Tests (Error Handling)

### 測試案例

#### ❌ TC-A04: 驗證失敗 - Request 為 null
```csharp
[Fact]
public async Task Should_ThrowArgumentNullException_WhenRequestIsNull()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    RegisterUserRequest nullRequest = null;

    // Act
    Func<Task> act = async () => await useCase.ExecuteAsync(nullRequest);

    // Assert
    await act.Should().ThrowAsync<ArgumentNullException>()
        .WithMessage("*request*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 null 檢查

---

#### ❌ TC-A05: 業務規則違反 - Username 已存在
```csharp
[Fact]
public async Task Should_ThrowDomainException_WhenUsernameAlreadyExists()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync("testuser")).ReturnsAsync(true);
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    Func<Task> act = async () => await useCase.ExecuteAsync(request);

    // Assert
    await act.Should().ThrowAsync<DomainException>()
        .WithMessage("*用戶名已被使用*");
}
```

**預期結果**: ❌ 紅燈 - 未實作唯一性檢查

---

#### ❌ TC-A06: 業務規則違反 - Email 已註冊
```csharp
[Fact]
public async Task Should_ThrowDomainException_WhenEmailAlreadyRegistered()
{
    // Arrange
    var existingUser = new User(
        Guid.NewGuid(),
        "otheruser",
        new Email("test@example.com"),
        Password.Create("Other@1234")
    );
    
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
    mockRepo.Setup(r => r.FindByEmailAsync("test@example.com")).ReturnsAsync(existingUser);
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "newuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    Func<Task> act = async () => await useCase.ExecuteAsync(request);

    // Assert
    await act.Should().ThrowAsync<DomainException>()
        .WithMessage("*Email 已被註冊*");
}
```

**預期結果**: ❌ 紅燈 - 未實作 Email 唯一性檢查

---

#### ❌ TC-A07: 例外處理 - Repository 拋出例外
```csharp
[Fact]
public async Task Should_PropagateException_WhenRepositoryThrowsException()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
    mockRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
    mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
        .ThrowsAsync(new InvalidOperationException("Database error"));
    
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Test@1234"
    };

    // Act
    Func<Task> act = async () => await useCase.ExecuteAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>()
        .WithMessage("*Database error*");
}
```

**預期結果**: ❌ 紅燈 - 未處理例外傳播

---

#### ❌ TC-A08: 驗證失敗 - Request 欄位驗證
```csharp
[Theory]
[InlineData("", "test@example.com", "Test@1234")]          // 空 Username
[InlineData("testuser", "", "Test@1234")]                   // 空 Email
[InlineData("testuser", "test@example.com", "")]            // 空 Password
[InlineData("ab", "test@example.com", "Test@1234")]         // Username 太短
[InlineData("testuser", "invalid-email", "Test@1234")]      // Email 格式錯誤
[InlineData("testuser", "test@example.com", "weak")]        // 密碼太弱
public async Task Should_ThrowValidationException_WhenRequestDataIsInvalid(
    string username, string email, string password)
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var useCase = new RegisterUserUseCase(mockRepo.Object);
    var request = new RegisterUserRequest
    {
        Username = username,
        Email = email,
        Password = password
    };

    // Act
    Func<Task> act = async () => await useCase.ExecuteAsync(request);

    // Assert
    await act.Should().ThrowAsync<ValidationException>();
}
```

**預期結果**: ❌ 紅燈 - 未實作輸入驗證

---

## 🟣 階段 3A: Infrastructure Layer - UserRepositoryAdapter Tests

### 測試專案
- **位置**: `RentalCar.Infrastructure.Tests/Repositories/UserRepositoryAdapterTests.cs`
- **Database**: In-Memory Database (Entity Framework Core)

### 測試案例

#### ✅ TC-I01: 正常路徑 - 儲存用戶到資料庫
```csharp
[Fact]
public async Task Should_SaveUser_ToDatabase()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user = new User(
        Guid.NewGuid(),
        "testuser",
        new Email("test@example.com"),
        Password.Create("Test@1234")
    );

    // Act
    await repository.AddAsync(user);
    await context.SaveChangesAsync();

    // Assert
    var savedUser = await repository.GetByIdAsync(user.UserId);
    savedUser.Should().NotBeNull();
    savedUser.Username.Should().Be("testuser");
}
```

**預期結果**: ❌ 紅燈 - `UserRepositoryAdapter` 不存在

---

#### ✅ TC-I02: 查詢 - 根據 UserId 查找用戶
```csharp
[Fact]
public async Task Should_FindUserById_WhenUserExists()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var userId = Guid.NewGuid();
    var user = new User(userId, "testuser", new Email("test@example.com"), Password.Create("Test@1234"));
    await repository.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var foundUser = await repository.GetByIdAsync(userId);

    // Assert
    foundUser.Should().NotBeNull();
    foundUser.UserId.Should().Be(userId);
}
```

**預期結果**: ❌ 紅燈 - 未實作 GetByIdAsync

---

#### ✅ TC-I03: 查詢 - 根據 Username 查找用戶
```csharp
[Fact]
public async Task Should_FindUserByUsername_WhenUserExists()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user = new User(Guid.NewGuid(), "testuser", new Email("test@example.com"), Password.Create("Test@1234"));
    await repository.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var foundUser = await repository.FindByUsernameAsync("testuser");

    // Assert
    foundUser.Should().NotBeNull();
    foundUser.Username.Should().Be("testuser");
}
```

**預期結果**: ❌ 紅燈 - 未實作 FindByUsernameAsync

---

#### ✅ TC-I04: 查詢 - 根據 Email 查找用戶
```csharp
[Fact]
public async Task Should_FindUserByEmail_WhenUserExists()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user = new User(Guid.NewGuid(), "testuser", new Email("test@example.com"), Password.Create("Test@1234"));
    await repository.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var foundUser = await repository.FindByEmailAsync("test@example.com");

    // Assert
    foundUser.Should().NotBeNull();
    foundUser.Email.Value.Should().Be("test@example.com");
}
```

**預期結果**: ❌ 紅燈 - 未實作 FindByEmailAsync

---

#### ✅ TC-I05: 查詢 - 用戶不存在時回傳 null
```csharp
[Fact]
public async Task Should_ReturnNull_WhenUserNotFound()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);

    // Act
    var foundUser = await repository.FindByUsernameAsync("nonexistent");

    // Assert
    foundUser.Should().BeNull();
}
```

**預期結果**: ❌ 紅燈 - 查詢邏輯未實作

---

#### ✅ TC-I06: 唯一性檢查 - 用戶名存在
```csharp
[Fact]
public async Task Should_ReturnTrue_WhenUsernameExists()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user = new User(Guid.NewGuid(), "testuser", new Email("test@example.com"), Password.Create("Test@1234"));
    await repository.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var exists = await repository.ExistsAsync("testuser");

    // Assert
    exists.Should().BeTrue();
}
```

**預期結果**: ❌ 紅燈 - 未實作 ExistsAsync

---

#### ✅ TC-I07: 唯一性檢查 - 用戶名不存在
```csharp
[Fact]
public async Task Should_ReturnFalse_WhenUsernameDoesNotExist()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);

    // Act
    var exists = await repository.ExistsAsync("nonexistent");

    // Assert
    exists.Should().BeFalse();
}
```

**預期結果**: ❌ 紅燈 - 未實作存在性檢查

---

#### ❌ TC-I08: 唯一性約束 - 重複 Username 拋出例外
```csharp
[Fact]
public async Task Should_ThrowException_WhenDuplicateUsernameInserted()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user1 = new User(Guid.NewGuid(), "testuser", new Email("user1@example.com"), Password.Create("Test@1234"));
    var user2 = new User(Guid.NewGuid(), "testuser", new Email("user2@example.com"), Password.Create("Test@5678"));
    
    await repository.AddAsync(user1);
    await context.SaveChangesAsync();

    // Act
    await repository.AddAsync(user2);
    Func<Task> act = async () => await context.SaveChangesAsync();

    // Assert
    await act.Should().ThrowAsync<DbUpdateException>();
}
```

**預期結果**: ❌ 紅燈 - 未設定資料庫唯一性約束

---

#### ❌ TC-I09: 唯一性約束 - 重複 Email 拋出例外
```csharp
[Fact]
public async Task Should_ThrowException_WhenDuplicateEmailInserted()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user1 = new User(Guid.NewGuid(), "user1", new Email("test@example.com"), Password.Create("Test@1234"));
    var user2 = new User(Guid.NewGuid(), "user2", new Email("test@example.com"), Password.Create("Test@5678"));
    
    await repository.AddAsync(user1);
    await context.SaveChangesAsync();

    // Act
    await repository.AddAsync(user2);
    Func<Task> act = async () => await context.SaveChangesAsync();

    // Assert
    await act.Should().ThrowAsync<DbUpdateException>();
}
```

**預期結果**: ❌ 紅燈 - 未設定 Email 唯一性約束

---

#### ✅ TC-I10: 並行處理 - 樂觀鎖定
```csharp
[Fact]
public async Task Should_HandleConcurrentRegistrations_Correctly()
{
    // Arrange
    await using var context = CreateInMemoryContext();
    var repository = new UserRepositoryAdapter(context);
    
    var user1 = new User(Guid.NewGuid(), "testuser", new Email("test1@example.com"), Password.Create("Test@1234"));
    var user2 = new User(Guid.NewGuid(), "testuser", new Email("test2@example.com"), Password.Create("Test@5678"));

    // Act - 模擬並行插入
    var task1 = Task.Run(async () =>
    {
        await using var ctx1 = CreateInMemoryContext();
        var repo1 = new UserRepositoryAdapter(ctx1);
        await repo1.AddAsync(user1);
        await ctx1.SaveChangesAsync();
    });

    var task2 = Task.Run(async () =>
    {
        await using var ctx2 = CreateInMemoryContext();
        var repo2 = new UserRepositoryAdapter(ctx2);
        await repo2.AddAsync(user2);
        await ctx2.SaveChangesAsync();
    });

    // Assert - 至少有一個應該失敗
    var results = await Task.WhenAll(
        task1.ContinueWith(t => t.Exception == null),
        task2.ContinueWith(t => t.Exception == null)
    );
    
    results.Count(r => r).Should().Be(1); // 只有一個成功
}
```

**預期結果**: ❌ 紅燈 - 未處理並行情況

---

## 🟤 階段 4A: API Layer - Integration Tests

### 測試專案
- **位置**: `RentalCar.API.IntegrationTests/Controllers/UsersControllerTests.cs`
- **框架**: WebApplicationFactory

### 測試案例

#### ✅ TC-API01: HTTP POST - 成功註冊回傳 201 Created
```csharp
[Fact]
public async Task POST_Register_Should_Return201Created_WhenValidRequest()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    response.Headers.Location.Should().NotBeNull();
    response.Headers.Location.ToString().Should().Contain("/api/users/");
}
```

**預期結果**: ❌ 紅燈 - API 端點不存在

---

#### ✅ TC-API02: HTTP POST - 成功回應包含用戶資料
```csharp
[Fact]
public async Task POST_Register_Should_ReturnUserDto_WhenSuccessful()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);
    var result = await response.Content.ReadFromJsonAsync<UserDto>();

    // Assert
    result.Should().NotBeNull();
    result.UserId.Should().NotBeEmpty();
    result.Username.Should().Be("testuser");
    result.Email.Should().Be("test@example.com");
}
```

**預期結果**: ❌ 紅燈 - 回應格式未定義

---

#### ❌ TC-API03: HTTP POST - 無效請求回傳 400 Bad Request
```csharp
[Theory]
[InlineData("", "test@example.com", "Test@1234")]          // 空 Username
[InlineData("testuser", "invalid-email", "Test@1234")]     // 無效 Email
[InlineData("testuser", "test@example.com", "weak")]       // 弱密碼
public async Task POST_Register_Should_Return400BadRequest_WhenInvalidData(
    string username, string email, string password)
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { username, email, password };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    error.Should().NotBeNull();
    error.Errors.Should().NotBeEmpty();
}
```

**預期結果**: ❌ 紅燈 - 未實作請求驗證

---

#### ❌ TC-API04: HTTP POST - 重複用戶名回傳 409 Conflict
```csharp
[Fact]
public async Task POST_Register_Should_Return409Conflict_WhenUsernameExists()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };
    
    // 第一次註冊
    await client.PostAsJsonAsync("/api/users/register", request);

    // 第二次使用相同用戶名
    var duplicateRequest = new
    {
        username = "testuser",
        email = "different@example.com",
        password = "Test@5678"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", duplicateRequest);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    error.Message.Should().Contain("用戶名已被使用");
}
```

**預期結果**: ❌ 紅燈 - 未處理衝突情況

---

#### ❌ TC-API05: HTTP POST - 重複 Email 回傳 409 Conflict
```csharp
[Fact]
public async Task POST_Register_Should_Return409Conflict_WhenEmailExists()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "user1",
        email = "test@example.com",
        password = "Test@1234"
    };
    
    await client.PostAsJsonAsync("/api/users/register", request);

    var duplicateRequest = new
    {
        username = "user2",
        email = "test@example.com",
        password = "Test@5678"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", duplicateRequest);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    error.Message.Should().Contain("Email 已被註冊");
}
```

**預期結果**: ❌ 紅燈 - 未處理 Email 衝突

---

#### ✅ TC-API06: 安全性 - 回應不包含密碼
```csharp
[Fact]
public async Task POST_Register_Should_NotReturnPasswordInResponse()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);
    var content = await response.Content.ReadAsStringAsync();

    // Assert
    content.Should().NotContain("password", "回應不應包含 password 欄位");
    content.Should().NotContain("passwordHash", "回應不應包含 passwordHash 欄位");
    content.Should().NotContain("Test@1234", "回應不應包含明文密碼");
}
```

**預期結果**: ❌ 紅燈 - DTO 可能洩漏密碼

---

#### ✅ TC-API07: Content-Type 驗證
```csharp
[Fact]
public async Task POST_Register_Should_ReturnJsonContentType()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
}
```

**預期結果**: ❌ 紅燈 - Content-Type 未設定

---

#### ❌ TC-API08: 錯誤處理 - 伺服器錯誤回傳 500
```csharp
[Fact]
public async Task POST_Register_Should_Return500_WhenUnexpectedErrorOccurs()
{
    // Arrange
    var client = _factory
        .WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // 注入會拋出例外的 Repository Mock
                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                    .ThrowsAsync(new Exception("Database connection failed"));
                    
                services.AddScoped(_ => mockRepo.Object);
            });
        })
        .CreateClient();

    var request = new
    {
        username = "testuser",
        email = "test@example.com",
        password = "Test@1234"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/users/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    error.Message.Should().Contain("發生錯誤");
}
```

**預期結果**: ❌ 紅燈 - 未處理例外

---

## 📊 測試覆蓋率目標

| 層級 | 測試案例數 | 目標覆蓋率 |
|------|-----------|-----------|
| Domain Layer | 25 | 100% |
| Application Layer | 8 | 95%+ |
| Infrastructure Layer | 10 | 90%+ |
| API Layer | 8 | 85%+ |
| **總計** | **51** | **95%+** |

---

## 🔧 測試環境設定

### 必要 NuGet 套件

```xml
<!-- 測試框架 -->
<PackageReference Include="xunit" Version="2.6.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />

<!-- Assertion Library -->
<PackageReference Include="FluentAssertions" Version="6.12.0" />

<!-- Mocking -->
<PackageReference Include="Moq" Version="4.20.0" />

<!-- Integration Testing -->
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />

<!-- In-Memory Database -->
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />

<!-- Password Hashing -->
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

---

## 🚀 TDD 實作流程

### 步驟 1: 建立測試專案結構
```bash
dotnet new xunit -n RentalCar.Domain.Tests
dotnet new xunit -n RentalCar.Application.Tests
dotnet new xunit -n RentalCar.Infrastructure.Tests
dotnet new xunit -n RentalCar.API.IntegrationTests
```

### 步驟 2: 依序執行 TDD 循環
1. **寫紅燈測試** → 執行測試（應該失敗）
2. **寫最少程式碼** → 讓測試通過（綠燈）
3. **重構** → 改善程式碼品質
4. **重複** → 進入下一個測試案例

### 步驟 3: 驗證測試覆蓋率
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
```

---

## 📝 測試數據建立器範例

### User Test Data Builder
```csharp
public class UserBuilder
{
    private Guid _userId = Guid.NewGuid();
    private string _username = "testuser";
    private Email _email = new Email("test@example.com");
    private Password _password = Password.Create("Test@1234");

    public UserBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = new Email(email);
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = Password.Create(password);
        return this;
    }

    public User Build()
    {
        return new User(_userId, _username, _email, _password);
    }
}

// 使用範例
var user = new UserBuilder()
    .WithUsername("john_doe")
    .WithEmail("john@example.com")
    .Build();
```

---

## 🎯 下一步行動

1. **立即開始**: 從 `TC-D01` (Email Value Object) 開始實作
2. **遵循 TDD**: 嚴格執行 紅燈 → 綠燈 → 重構 循環
3. **逐層推進**: Domain → Application → Infrastructure → API
4. **持續驗證**: 每完成一個階段執行完整測試套件
5. **文檔更新**: 記錄測試結果和實作決策

---

## ✅ 成功指標

- [ ] 所有 51 個測試案例完成
- [ ] 測試覆蓋率達 95% 以上
- [ ] 所有測試皆為綠燈
- [ ] 符合六角架構依賴規則
- [ ] API 端點正常運作
- [ ] 密碼安全儲存
- [ ] 唯一性約束生效
- [ ] 錯誤處理完善

---

**注意**: 此文件僅包含測試設計，實際測試程式碼需在各測試專案中實作。每個測試案例的「預期結果」標註為 ❌ 表示 TDD 紅燈階段的預期狀態。

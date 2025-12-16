# 租車系統 (RentalCar) - AI 編碼指南

## 系統概述

這是一個遵循 **Hexagonal Architecture (六角架構)** 的線上租車預約系統，僅提供 Web API，無 UI 介面。用戶須先註冊並登入後才能進行車輛預約、選擇車型、設定租期並計算租金。

## 架構層級與專案結構

系統分為 4 個核心層級，對應不同的專案：

### 1. Web API Layer (Adapter In)
- **路徑**: `RentalCar\adapter\In`
- **職責**: HTTP 端點、請求驗證、回應格式化
- **原則**: 不包含業務邏輯，僅負責協議轉換（HTTP ↔ Application）
- **實作要點**: 
  - Controllers 應使用 Application Layer 的 Use Cases
  - 回傳標準化的 API 回應結構
  - 處理認證/授權（例如 JWT Token 驗證）

### 2. Application Layer
- **路徑**: `RentalCar\adapter`
- **職責**: 協調 Use Cases、應用服務、DTO 轉換
- **原則**: 編排 Domain 邏輯，但不實作業務規則
- **實作要點**:
  - 定義 Use Case 介面和實作（如 `RentCarUseCase`, `RegisterUserUseCase`）
  - 處理跨 Aggregate 的協調邏輯
  - 定義 Port 介面（如 `ICarRepository`, `IUserRepository`）

### 3. Domain Layer
- **路徑**: `RentalCar\adapter` （核心業務邏輯）
- **職責**: 業務規則、實體、值物件、Domain Services
- **原則**: 完全獨立，不依賴外部框架或基礎設施
- **實作要點**:
  - 定義 Aggregates（如 `Car`, `User`, `Reservation`）
  - 封裝業務不變量（如：租用時間驗證、車輛可用性檢查）
  - 實作租金計算邏輯於 Domain Services 或 Entities
  - 使用 Value Objects（如 `RentalPeriod`, `Price`）

### 4. Infrastructure Layer
- **路徑**: `RentalCar\Infrastructure`（推測位置）
- **職責**: 資料庫存取、外部服務整合、技術實作
- **原則**: 實作 Application Layer 定義的 Port 介面
- **實作要點**:
  - Repository 實作（如 `CarRepositoryAdapter`）
  - 資料庫 Context 和 Migrations
  - 第三方服務整合（如支付、通知）

### Configuration 資料夾
- **用途**: AOP 橫切關注點、外部套件整合
- **範例**: 日誌記錄、例外處理、快取、驗證過濾器

## 核心業務流程

### 用戶註冊與登入
```
1. RegisterUser → 建立 User Aggregate → 儲存至 UserRepository
2. Login → 驗證憑證 → 發行 JWT Token
3. 所有租車操作需經過認證中介軟體
```

### 車輛預約流程
```
1. 檢查用戶已登入（Authentication）
2. 查詢可用車輛（CarRepository.FindAvailableCars）
3. 選擇車型和租期 → 建立 Reservation Aggregate
4. 計算租金（Domain Service: RentalPriceCalculator）
5. 保留車輛 → 更新車輛狀態
```

## 關鍵設計原則

### Dependency Rule（依賴規則）
- **箭頭方向**: Infrastructure → Application → Domain
- **禁止**: Domain 不得依賴 Application 或 Infrastructure
- **使用 DI**: 透過 Port/Adapter 模式注入依賴

### Domain 邏輯位置
- **租金計算**: 必須在 Domain Layer（例如 `RentalPriceCalculator` Domain Service）
- **車輛可用性檢查**: Domain Entity 的 `IsAvailableForPeriod()` 方法
- **租用時間驗證**: Value Object `RentalPeriod` 的建構子

### API 設計慣例
- **端點命名**: RESTful 風格（`POST /api/reservations`, `GET /api/cars`）
- **認證**: 使用 Bearer Token (JWT)
- **錯誤回應**: 統一結構（包含 `statusCode`, `message`, `errors`）
- **無 UI**: 所有操作透過 API，文件使用 Swagger/OpenAPI

## 開發工作流程

### 新增功能步驟
1. **Domain First**: 從 Domain Entities/Value Objects 開始設計
2. **定義 Port**: 在 Application Layer 定義 Repository 或 Service 介面
3. **實作 Use Case**: Application Layer 建立編排邏輯
4. **Adapter 實作**: Infrastructure 實作 Port，Web API 暴露端點
5. **測試**: 單元測試 Domain 邏輯，整合測試 API 端點

### 測試策略
- **Domain Tests**: 測試業務規則，不依賴 DB 或外部服務
- **Application Tests**: 使用 Mock Repositories
- **Integration Tests**: 測試完整的 API 端點到資料庫

## 常見模式

### Repository Pattern
```csharp
// Application Layer (Port)
public interface ICarRepository
{
    Task<Car> GetByIdAsync(Guid id);
    Task<IEnumerable<Car>> FindAvailableCarsAsync(DateTime from, DateTime to);
}

// Infrastructure Layer (Adapter)
public class CarRepositoryAdapter : ICarRepository { ... }
```

### Use Case Pattern
```csharp
public class RentCarUseCase
{
    private readonly ICarRepository _carRepository;
    private readonly IReservationRepository _reservationRepository;
    
    public async Task<ReservationDto> ExecuteAsync(RentCarRequest request)
    {
        // 1. 驗證用戶已登入
        // 2. 檢查車輛可用性
        // 3. 建立預約
        // 4. 計算租金
        // 5. 儲存並回傳
    }
}
```

## 注意事項

- **禁止 Anemic Domain Model**: Entity 必須包含行為，不只是資料容器
- **不洩漏 Domain 到 API**: 使用 DTO 進行層級間轉換
- **交易邊界**: 以 Aggregate 為單位進行 Repository 操作
- **認證先決條件**: 所有租車相關 API 必須驗證用戶已登入

## 快速參考

- **新增車型**: `Domain/Entities/Car.cs` → `Application/UseCases/AddCarUseCase.cs` → `WebAPI/Controllers/CarsController.cs`
- **修改租金計算**: 直接修改 `Domain/Services/RentalPriceCalculator.cs`
- **認證配置**: `Configuration/Authentication/JwtConfiguration.cs`

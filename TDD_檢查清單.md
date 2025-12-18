# ? TDD 紅燈階段檢查清單

## 已完成的工作 ?

### 專案結構
- [x] `Domain.RentalCarLab1\ValueObjects\Password.cs` - Password Value Object 骨架
- [x] `Domain.RentalCarLab1\Account.cs` - Account Entity
- [x] `Domain.RentalCar.Tests\Test_DomainRentalCar.cs` - 單元測試

### 測試案例
- [x] TC-D10: 驗證密碼缺少大寫字母
- [x] TC-D11: 驗證密碼缺少小寫字母
- [x] TC-D12: 驗證密碼缺少數字
- [x] TC-D13: 驗證密碼缺少特殊字元
- [x] TC-D14: 驗證 BCrypt 雜湊

### 建置狀態
- [x] 專案編譯成功
- [x] 無編譯錯誤
- [x] 測試可以執行（預期失敗）

---

## 下一步行動 ??

### 立即執行
- [ ] 安裝 BCrypt.Net-Next NuGet 套件
- [ ] 修改 `Password.Create()` 方法
- [ ] 實作 `Password.Verify()` 方法
- [ ] 執行測試確認綠燈

### 詳細步驟
查看 `快速開始_綠燈階段.md` 獲得逐步指引

---

## 檔案清單 ??

| 檔案 | 用途 | 狀態 |
|------|------|------|
| `Domain.RentalCarLab1\ValueObjects\Password.cs` | Password 骨架 | ? 已建立 |
| `Domain.RentalCarLab1\Account.cs` | Account Entity | ? 已建立 |
| `Domain.RentalCar.Tests\Test_DomainRentalCar.cs` | 單元測試 | ? 已建立 |
| `TDD_紅燈階段_Password_骨架說明.md` | 詳細說明 | ? 已建立 |
| `TDD_紅燈階段_完成總結.md` | 完成總結 | ? 已建立 |
| `快速開始_綠燈階段.md` | 實作指南 | ? 已建立 |
| `TDD_檢查清單.md` | 本檔案 | ? 已建立 |

---

## ?? 學習重點

本次練習學到：
1. ? TDD 紅燈階段的意義（先寫測試，確認失敗）
2. ? Value Object 設計模式
3. ? 工廠方法模式（Create）
4. ? DDD 封裝原則
5. ? MSTest 測試框架
6. ? .NET Framework 4.8 開發

---

**目前階段**: ?? 紅燈（已完成）  
**準備進入**: ?? 綠燈  
**最終目標**: ?? 重構

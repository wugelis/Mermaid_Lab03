# RentalCar 線上預先車輛租用系統

## 系統說明：
這是一個提供租車用戶可在線上預先租用車輛的系統。系統功能為，租車用戶可以在線上預約租車，而在租用車輛之前，租車用戶必須先註冊自已的帳戶資料後，並進行登入後才可預先租用車輛。在租用車輛時，可以選擇車型、租用時間區間、並計算租金。

## 系統架構：

本系統的網站只提供 Web API 方法呼叫，沒有 UI 介面，並遵循 Hexagonal Architecture 並分為 4 層，有 4 個主要專案

Web API
Application
Domain
Infrastructure

其他 AOP 與 外部套件功能放置在方案資料夾 Configuration 的下面

## Web API

放置在方案資料夾 RentalCar\adapter\In

## Application

放置在方案資料夾 RentalCar\adapter

# Domain

放置在方案資料夾 RentalCar\adapter
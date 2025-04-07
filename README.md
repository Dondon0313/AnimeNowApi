# AnimeNow API - 動漫追番後端服務

## 專案簡介

AnimeNow API 是 AnimeNow 動漫追番平台的後端服務，提供番劇和劇集的數據管理與交互。

## 技術棧

- .NET Core
- Entity Framework Core
- AutoMapper
- SQL Server (In-Memory Database)
- Swagger UI

## 主要功能

### 番劇（Bangumi）相關 API
- 獲取所有番劇
- 根據 ID 查詢番劇
- 按狀態篩選番劇（連載中、即將播出等）
- 按播出日篩選番劇

### 劇集（Episode）相關 API
- 獲取所有劇集
- 根據 ID 查詢劇集
- 獲取指定番劇的所有劇集
- 增加劇集觀看次數

## 數據模型

- Bangumi：番劇基本信息
- Episode：單集詳細信息
- Genre：番劇類型
- BangumiGenre：番劇與類型的多對多關係

## 特色

- 使用 DTO 進行數據傳輸
- 自動數據庫初始化
- CORS 支持
- Swagger 文檔

## 環境要求

- .NET Core SDK
- Visual Studio 或 Visual Studio Code
# Acme.ProductSelling

English | [Tiếng Việt](#tiếng-việt)

---

## English

### Project Overview

Acme.ProductSelling is a layered e-commerce solution built on the **ABP Framework** and **.NET 10**. It follows **Domain-Driven Design (DDD)** and is organized around a storefront, admin back office, API layer, background jobs, and payment integrations.

### Key Features

- 📦 **Catalog management**: Products, categories, manufacturers, specifications, and lookup data.
- 🛒 **Cart and orders**: Customer checkout, order history, in-store orders, fulfillment, and status flows.
- 🏪 **Multi-store inventory**: Store and inventory tracking with transfer and low-stock workflows.
- 💳 **Payments**: Payment gateway integrations for **MoMo**, **VNPay**, **PayPal**, and COD handling.
- 🤖 **AI chatbot**: Gemini-powered assistant for customers and back-office roles.
- 🔐 **Identity and admin**: ABP Identity/OpenIddict, role-based access, and admin pages.
- 🐳 **Docker support**: Local environment orchestration with Docker Compose and database migration startup.

### Pre-requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node.js (v18 or 20)](https://nodejs.org/en)
- [ABP CLI](https://abp.io/docs/latest/cli)

### Getting Started

#### 1. Configuration

Check `ConnectionStrings` and environment-specific settings in the following files. Use the matching `appsettings.example.json` files as a template:

- `src/Acme.ProductSelling.Web/appsettings.json`
- `src/Acme.ProductSelling.DbMigrator/appsettings.json`

#### 2. Install Dependencies

Run this from the repository root to install client-side libraries:

```bash
abp install-libs
```

#### 3. Database Migration

Run the `DbMigrator` project to apply migrations and seed initial data:

```bash
cd src/Acme.ProductSelling.DbMigrator
dotnet run
```

#### 4. Development Certificates

Generate an `openiddict.pfx` for local OpenIddict development:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 061ebb66-ca81-4cf3-9b06-09065eb6cf4c
```

_(Note: Replace the password if necessary.)_

### Solution Structure

This solution is split into the following modules:

#### Core Projects (`src/`)

- **Acme.ProductSelling.Domain.Shared**: Constants, enums, and other objects shared across all layers.
- **Acme.ProductSelling.Domain**: The core layer containing entities, domain services, repository interfaces, and business logic.
- **Acme.ProductSelling.Application.Contracts**: Application service interfaces and Data Transfer Objects (DTOs).
- **Acme.ProductSelling.Application**: Concrete implementations of application services.
- **Acme.ProductSelling.EntityFrameworkCore**: Data access layer using Entity Framework Core.
- **Acme.ProductSelling.HttpApi**: Web API controllers for external interaction.
- **Acme.ProductSelling.HttpApi.Client**: C# client library for remote service consumption.
- **Acme.ProductSelling.Web**: The main web application (MVC/Razor Pages).
- **Acme.ProductSelling.DbMigrator**: Console application for migrations and data seeding.
- **Acme.ProductSelling.PaymentGateway.\***: Integration modules for MoMo, VNPay, and PayPal.

#### Test Projects (`test/`)

- **Acme.ProductSelling.Domain.Tests**: Unit tests for the domain layer.
- **Acme.ProductSelling.Application.Tests**: Unit tests for application services.
- **Acme.ProductSelling.EntityFrameworkCore.Tests**: Integration tests for database operations.
- **Acme.ProductSelling.Web.Tests**: UI and integration tests for the web layer.

### Core Business Modules

The main business modules implemented in `Domain` and `Application` are:

- 🛒 **Carts & Orders**: Full checkout flow and order management.
- 📦 **Products & Categories**: Product catalog with manufacturer and specification support.
- 🤖 **AI Chatbot**: Intelligent assistant using **Google Gemini API** (supports Customer & Staff roles).
- 💬 **Comments**: User feedback and interaction system.
- 🔐 **Identity & Security**: Customized user management and OpenIddict integration.
- 🏪 **Stores & Inventory**: Multi-store support and inventory tracking.

### Docker Support

The repository includes a `docker-compose.yml` file to spin up the local environment:

- **Database**: SQL Server.
- **Automation**: Database migration and seeding on startup.
  Start it with:

```bash
docker-compose up -d
```

### Deployment

Deploying an ABP application follows the same process as deploying any .NET or ASP.NET Core application.

- **Production Certificates**: Always use a valid RSA certificate for OpenIddict in production.
- **Guides**: Refer to the [ABP Deployment Documentation](https://abp.io/docs/latest/Deployment/Index) for detailed steps.

### Additional Resources

- [ABP Framework Documentation](https://abp.io/docs/latest)
- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [Domain-Driven Design Fundamentals](https://abp.io/docs/latest/framework/architecture/domain-driven-design)

---

## Tiếng Việt

### Tổng quan dự án

Acme.ProductSelling là một giải pháp thương mại điện tử phân lớp, được xây dựng trên **ABP Framework** và **.NET 10**. Dự án tuân theo **Thiết kế hướng tên miền (DDD)** và được tổ chức theo các khối: storefront, khu quản trị, lớp API, job nền và tích hợp thanh toán.

### Các tính năng chính

- 📦 **Quản lý danh mục**: Sản phẩm, danh mục, nhà sản xuất, đặc tả kỹ thuật và dữ liệu tra cứu.
- 🛒 **Giỏ hàng và đơn hàng**: Thanh toán cho khách hàng, lịch sử đơn hàng, đơn tại quầy, quy trình xử lý đơn.
- 🏪 **Kho đa cửa hàng**: Theo dõi cửa hàng, tồn kho, điều chuyển và cảnh báo hàng sắp hết.
- 💳 **Thanh toán**: Tích hợp **MoMo**, **VNPay**, **PayPal** và luồng xử lý COD.
- 🤖 **Chatbot AI**: Trợ lý dùng Gemini cho khách hàng và các vai trò nội bộ.
- 🔐 **Định danh và quản trị**: ABP Identity/OpenIddict, phân quyền theo vai trò và các trang quản trị.
- 🐳 **Hỗ trợ Docker**: Chạy môi trường local bằng Docker Compose kèm migration database.

### Yêu cầu hệ thống

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node.js (v18 hoặc 20)](https://nodejs.org/en)
- [ABP CLI](https://abp.io/docs/latest/cli)

### Hướng dẫn bắt đầu

#### 1. Cấu hình

Kiểm tra và cập nhật `ConnectionStrings` trong các tệp sau để phù hợp với cơ sở dữ liệu của bạn. Bạn có thể tham khảo tệp `appsettings.example.json` để biết cấu trúc yêu cầu:

- `src/Acme.ProductSelling.Web/appsettings.json`
- `src/Acme.ProductSelling.DbMigrator/appsettings.json`

#### 2. Cài đặt thư viện

Chạy lệnh sau tại thư mục gốc để cài đặt các thư viện phía client:

```bash
abp install-libs
```

#### 3. Khởi tạo cơ sở dữ liệu

Chạy dự án `DbMigrator` để tạo cơ sở dữ liệu và nạp dữ liệu mẫu:

```bash
cd src/Acme.ProductSelling.DbMigrator
dotnet run
```

#### 4. Chứng chỉ phát triển

Tạo tệp `openiddict.pfx` cho OpenIddict:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 061ebb66-ca81-4cf3-9b06-09065eb6cf4c
```

_(Lưu ý: Bạn có thể thay đổi mật khẩu nếu cần.)_

### Cấu trúc giải pháp

Giải pháp này tuân thủ các mẫu thiết kế hướng tên miền (DDD) tiêu chuẩn của ABP Framework:

#### Các dự án chính (`src/`)

- **Acme.ProductSelling.Domain.Shared**: Chứa các hằng số, enum và các đối tượng dùng chung cho tất cả các lớp.
- **Acme.ProductSelling.Domain**: Lớp cốt lõi chứa các thực thể (entities), dịch vụ miền (domain services), các interface repository và logic nghiệp vụ.
- **Acme.ProductSelling.Application.Contracts**: Định nghĩa các interface dịch vụ ứng dụng và các đối tượng chuyển đổi dữ liệu (DTOs).
- **Acme.ProductSelling.Application**: Chứa các triển khai thực tế của các dịch vụ ứng dụng.
- **Acme.ProductSelling.EntityFrameworkCore**: Lớp truy cập dữ liệu, triển khai các repository bằng Entity Framework Core.
- **Acme.ProductSelling.HttpApi**: Chứa các Web API controller, cho phép hệ thống bên ngoài tương tác với ứng dụng.
- **Acme.ProductSelling.HttpApi.Client**: Thư viện client C# được sử dụng để gọi API từ các ứng dụng .NET khác.
- **Acme.ProductSelling.Web**: Ứng dụng web chính (MVC/Razor Pages), bao gồm các thành phần giao diện và kiểu dáng.
- **Acme.ProductSelling.DbMigrator**: Một ứng dụng console để quản lý việc cập nhật cơ sở dữ liệu (migration) và nạp dữ liệu mẫu.
- **Acme.ProductSelling.PaymentGateway.\***: Các module chuyên biệt để tích hợp với các nhà cung cấp thanh toán (**MoMo**, **VNPay**, **PayPal**).

#### Các dự án kiểm thử (`test/`)

- **Acme.ProductSelling.Domain.Tests**: Kiểm thử đơn vị cho lớp domain.
- **Acme.ProductSelling.Application.Tests**: Kiểm thử đơn vị cho các dịch vụ ứng dụng.
- **Acme.ProductSelling.EntityFrameworkCore.Tests**: Kiểm thử tích hợp cho các thao tác cơ sở dữ liệu.
- **Acme.ProductSelling.Web.Tests**: Kiểm thử giao diện và tích hợp cho lớp web.

### Các Module Nghiệp vụ Chính

Các module sau đây được triển khai trong lớp `Domain` và `Application`:

- 🛒 **Giỏ hàng & Đơn hàng**: Quy trình thanh toán và quản lý đơn hàng hoàn chỉnh.
- 📦 **Sản phẩm & Danh mục**: Danh mục sản phẩm với hỗ trợ nhà sản xuất và đặc tính kỹ thuật.
- 🤖 **Chatbot AI**: Trợ lý thông minh sử dụng **Google Gemini API** (Hỗ trợ cả Khách hàng & Nhân viên).
- 💬 **Bình luận**: Hệ thống phản hồi và tương tác của người dùng.
- 🔐 **Định danh & Bảo mật**: Quản lý người dùng và tích hợp OpenIddict được tùy chỉnh.
- 🏪 **Cửa hàng & Kho hàng**: Hỗ trợ nhiều cửa hàng và theo dõi tồn kho.

### Hỗ trợ Docker

Giải pháp đi kèm với tệp `docker-compose.yml` để khởi tạo môi trường nhanh chóng:

- **Cơ sở dữ liệu**: SQL Server 2025 (phiên bản mới nhất).
- **Tự động hóa**: Tự động cập nhật cơ sở dữ liệu và nạp dữ liệu khi khởi động.
  Để khởi động môi trường, chạy lệnh:

```bash
docker-compose up -d
```

### Triển khai

Việc triển khai ứng dụng ABP tương tự như triển khai bất kỳ ứng dụng .NET hoặc ASP.NET Core nào.

- **Chứng chỉ Production**: Luôn sử dụng chứng chỉ RSA hợp lệ cho OpenIddict trong môi trường thực tế.
- **Hướng dẫn**: Tham khảo [Tài liệu triển khai ABP](https://abp.io/docs/latest/Deployment/Index) để biết thêm chi tiết.

## Thông tin thêm

- [Tài liệu ABP Framework](https://abp.io/docs/latest)
- [Tài liệu OpenIddict](https://documentation.openiddict.com/)
- [Kiến thức cơ bản về DDD](https://abp.io/docs/latest/framework/architecture/domain-driven-design)

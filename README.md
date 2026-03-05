# Acme.ProductSelling

English | [Tiếng Việt](#tiếng-việt)

---

## English

### Project Overview

Acme.ProductSelling is a modern, layered e-commerce solution built on the **ABP Framework** and **.NET 10**. This project follows **Domain-Driven Design (DDD)** principles and provides a robust foundation for building high-performance product selling platforms.

### Key Features

- 📦 **Product Management**: Manage products, categories, manufacturers, and inventories.
- 🛒 **Shopping Experience**: Fully functional shopping cart and order processing system.
- 💳 **Payment Integrations**: Pre-integrated with popular payment gateways: **MoMo, VNPay, and PayPal**.
- 🤖 **AI Chatbot**: Intelligent assistant powered by **Google Gemini API**, available for both customers and store roles (**Cashier, Manager, Warehouse, Admin**) to assist with operations and inquiries.
- 🐳 **Docker Ready**: Fully containerized environment using Docker Compose for rapid development and deployment.
- 🏗️ **Clean Architecture**: Based on DDD patterns for high maintainability and scalability.

### Pre-requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node.js (v18 or 20)](https://nodejs.org/en)
- [ABP CLI](https://abp.io/docs/latest/cli)

### Getting Started

#### 1. Configuration

Check the `ConnectionStrings` in the following files and update them with your database details. You can refer to the `appsettings.example.json` files for the required structure:

- `src/Acme.ProductSelling.Web/appsettings.json`
- `src/Acme.ProductSelling.DbMigrator/appsettings.json`

#### 2. Install Dependencies

Run the following command in the root folder to install client-side libraries:

```bash
abp install-libs
```

#### 3. Database Migration

Run the `DbMigrator` project to create the database and seed initial data:

```bash
cd src/Acme.ProductSelling.DbMigrator
dotnet run
```

#### 4. Development Certificates

Generate an `openiddict.pfx` for OpenIddict:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 061ebb66-ca81-4cf3-9b06-09065eb6cf4c
```

_(Note: Replace the password if necessary.)_

### Solution Structure

This solution follows the standard Domain-Driven Design (DDD) patterns of the ABP Framework:

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

The following business modules are implemented within the `Domain` and `Application` layers:

- 🛒 **Carts & Orders**: Full checkout flow and order management.
- 📦 **Products & Categories**: Product catalog with manufacturer and specification support.
- 🤖 **AI Chatbot**: Intelligent assistant using **Google Gemini API** (supports Customer & Staff roles).
- 💬 **Comments**: User feedback and interaction system.
- 🔐 **Identity & Security**: Customized user management and OpenIddict integration.
- 🏪 **Stores & Inventory**: Multi-store support and inventory tracking.

### Docker Support

The solution includes a `docker-compose.yml` file to quickly spin up the environment:

- **Database**: SQL Server 2025 (latest).
- **Automation**: Automatic database migration and seeding on startup.
  To start the environment, run:

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

Acme.ProductSelling là một giải pháp thương mại điện tử hiện đại, phân lớp, được xây dựng trên nền tảng **ABP Framework** và **.NET 10**. Dự án tuân thủ các nguyên tắc **Thiết kế hướng tên miền (DDD)**, cung cấp nền tảng vững chắc để xây dựng các hệ thống bán hàng hiệu suất cao.

### Các tính năng chính

- 📦 **Quản lý sản phẩm**: Quản lý danh mục, nhà sản xuất, sản phẩm và kho hàng.
- 🛒 **Trải nghiệm mua sắm**: Hệ thống giỏ hàng và xử lý đơn hàng hoàn chỉnh.
- 💳 **Tích hợp thanh toán**: Hỗ trợ sẵn các cổng thanh toán phổ biến: **MoMo, VNPay và PayPal**.
- 🤖 **Chatbot AI**: Trợ lý thông minh sử dụng **Google Gemini API**, hỗ trợ cả khách hàng và các vai trò cửa hàng (**Thu ngân, Quản lý, Kho, Admin**) trong vận hành và giải đáp thắc mắc.
- 🐳 **Sẵn sàng cho Docker**: Môi trường được đóng gói hoàn chỉnh bằng Docker Compose giúp phát triển và triển khai nhanh chóng.
- 🏗️ **Kiến trúc sạch**: Dựa trên các mẫu thiết kế DDD giúp dễ dàng bảo trì và mở rộng.

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

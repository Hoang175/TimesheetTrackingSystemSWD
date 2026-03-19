# 🕒 Timesheet Tracking System (SWD Project)

Dự án quản lý chấm công và thời gian làm việc (Timesheet Tracking System) được phát triển cho môn học **Software Architecture and Design (SWD)**. Hệ thống giúp tự động hóa quy trình ghi nhận giờ làm, tổng hợp bảng công và phê duyệt từ bộ phận HR.

## 🏗️ Kiến trúc hệ thống (Architecture)

Dự án được xây dựng theo mô hình **3-Tier Architecture (Kiến trúc 3 lớp)** nhằm đảm bảo tính phân tách và dễ dàng bảo trì:

* **TimesheetTrackingSystemSWD (MVC):** Lớp Presentation (Giao diện người dùng), chứa Controllers và Views. Nhận request từ người dùng và trả về kết quả.
* **TimesheetTrackingSystemSWD.BLL (Business Logic Layer):** Lớp xử lý nghiệp vụ. Chứa các quy tắc hệ thống (ví dụ: logic tính tổng giờ làm, kiểm tra hợp lệ trước khi approve/reject).
* **TimesheetTrackingSystemSWD.DAL (Data Access Layer):** Lớp giao tiếp với cơ sở dữ liệu. Chứa Entity Framework Core `DbContext`, các Models sinh ra từ Database và Repository pattern.

## 🛠️ Công nghệ sử dụng

* **Framework:** .NET 8.0
* **Web Pattern:** ASP.NET Core MVC
* **ORM:** Entity Framework Core (Database-First Approach)
* **Database:** SQL Server

---


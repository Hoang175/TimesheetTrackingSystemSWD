-- Tạo Database
CREATE DATABASE TimesheetTrackingSystemSWD;
GO
USE TimesheetTrackingSystemSWD;
GO

-- 1. Table Roles (Vai trò hệ thống hiếm khi xóa, nhưng vẫn nên có)
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- 2. Table Departments
CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- 3. Table Users
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Status NVARCHAR(20) DEFAULT 'Active', -- Active, Locked
    RoleID INT NOT NULL,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- 4. Table Employees
CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE, 
    EmployeeCode NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentID INT,
    Position NVARCHAR(100),
    HireDate DATE,
    EmploymentStatus NVARCHAR(20) DEFAULT 'Active',
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Employees_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

-- 5. Table AttendanceRecords (Dữ liệu giao dịch)
CREATE TABLE AttendanceRecords (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    WorkDate DATE NOT NULL,
    CheckInTime DATETIME,
    CheckOutTime DATETIME,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Attendance_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 6. Table Timesheets (Bảng tổng hợp)
CREATE TABLE Timesheets (
    TimesheetID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    TotalWorkingHours DECIMAL(5,2),
    Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
    ApprovedBy INT NULL, 
    ApprovedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Timesheets_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    CONSTRAINT FK_Timesheets_Users FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID)
);

-- 7. Table Holidays
CREATE TABLE Holidays (
    HolidayID INT IDENTITY(1,1) PRIMARY KEY,
    HolidayDate DATE NOT NULL,
    HolidayName NVARCHAR(100) NOT NULL,
    IsPaidLeave BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- 8. Table SystemLogs (Append-only: Không cần xóa mềm hay UpdatedAt)
CREATE TABLE SystemLogs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Action NVARCHAR(255) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_SystemLogs_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- INSERT DATA MẪU
INSERT INTO Roles (RoleName, Description) 
VALUES ('Admin', 'System Administrator'), ('HR', 'Human Resources'), ('Employee', 'Standard Employee');
INSERT INTO Departments (DepartmentName, Description)
VALUES ('IT', 'Information Technology'), ('HR', 'Human Resources Dept');
INSERT INTO Users (Username, PasswordHash, FullName, Email, RoleID)

VALUES ('admin', '123', 'Nguyen Van Admin', 'admin@mail.com', 1), ('hr', '123', 'Tran Thi HR', 'hr@mail.com', 2), ('emp', '123', 'Le Van Nhan Vien', 'emp01@mail.com', 3);
INSERT INTO Employees (UserID, EmployeeCode, DepartmentID, Position, HireDate) 
VALUES (3, 'EMP-001', 1, 'Software Engineer', '2025-01-01');
INSERT INTO AttendanceRecords (EmployeeID, WorkDate, CheckInTime, CheckOutTime) 
VALUES (1, '2026-03-18', '2026-03-18 08:00:00', '2026-03-18 17:00:00'), (1, '2026-03-19', '2026-03-19 08:15:00', '2026-03-19 17:30:00');
INSERT INTO Timesheets (EmployeeID, PeriodStart, PeriodEnd, TotalWorkingHours, Status)
VALUES (1, '2026-03-01', '2026-03-31', 16.5, 'Pending');
INSERT INTO Holidays (HolidayDate, HolidayName, IsPaidLeave)
VALUES ('2026-04-30', 'Reunification Day', 1), ('2026-05-01', 'Labor Day', 1);
-- File: HRManagement.sql
-- Description: Database schema for HR Management System for Thanh Cong Shoe Company
-- Created: April 13, 2025

-- Create Database
CREATE DATABASE HRManagement;
GO

USE HRManagement;
GO

-- 1. Admins Table
CREATE TABLE Admins (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(15),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Departments Table
CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(200)
);

-- 3. Positions Table
CREATE TABLE Positions (
    PositionID INT IDENTITY(1,1) PRIMARY KEY,
    PositionName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(200)
);

-- 4. Employees Table
CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(200),
    Phone NVARCHAR(15),
    Email NVARCHAR(100),
    DepartmentID INT,
    PositionID INT,
    BaseSalary DECIMAL(18,2),
    ContractStartDate DATE,
    ContractEndDate DATE,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID),
    FOREIGN KEY (PositionID) REFERENCES Positions(PositionID)
);

-- 5. Attendance Table
CREATE TABLE Attendance (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    AttendanceDate DATE,
    CheckInTime DATETIME,
    CheckOutTime DATETIME,
    OvertimeHours DECIMAL(5,2),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 6. LeaveRequests Table
CREATE TABLE LeaveRequests (
    LeaveID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    LeaveType NVARCHAR(50),
    StartDate DATE,
    EndDate DATE,
    Reason NVARCHAR(200),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 7. Payroll Table
CREATE TABLE Payroll (
    PayrollID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    Month INT,
    Year INT,
    BaseSalary DECIMAL(18,2),
    Allowance DECIMAL(18,2),
    Bonus DECIMAL(18,2),
    Tax DECIMAL(18,2),
    Insurance DECIMAL(18,2),
    NetSalary DECIMAL(18,2),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 8. Recruitment Table
CREATE TABLE Recruitment (
    CandidateID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(15),
    AppliedPositionID INT,
    CVPath NVARCHAR(200),
    InterviewDate DATETIME,
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AppliedPositionID) REFERENCES Positions(PositionID)
);

-- 9. Training Table
CREATE TABLE Training (
    TrainingID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    TrainingName NVARCHAR(100),
    StartDate DATE,
    EndDate DATE,
    Result NVARCHAR(50),
    Evaluation NVARCHAR(200),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 10. PerformanceEvaluations Table
CREATE TABLE PerformanceEvaluations (
    EvaluationID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    EvaluationDate DATE,
    EvaluatorID INT,
    SelfScore DECIMAL(5,2),
    ManagerScore DECIMAL(5,2),
    PeerScore DECIMAL(5,2),
    Comments NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    FOREIGN KEY (EvaluatorID) REFERENCES Employees(EmployeeID)
);

-- Insert Sample Data

-- Admins
INSERT INTO Admins (Username, Password, FullName, Email, Phone)
VALUES ('admin1', 'hashed_password1', N'Nguyễn Văn Quản', 'admin1@thanhcong.com', '0123456789');

-- Departments
INSERT INTO Departments (DepartmentName, Description)
VALUES 
(N'Sản xuất', N'Phòng ban sản xuất giày'),
(N'Nhân sự', N'Quản lý nhân sự công ty'),
(N'Kinh doanh', N'Phòng kinh doanh và tiếp thị');

-- Positions
INSERT INTO Positions (PositionName, Description)
VALUES 
(N'Nhân viên sản xuất', N'Làm việc tại dây chuyền sản xuất'),
(N'Quản lý nhân sự', N'Quản lý hồ sơ và quy trình nhân sự'),
(N'Nhân viên kinh doanh', N'Tìm kiếm và chăm sóc khách hàng');

-- Employees
INSERT INTO Employees (FullName, DateOfBirth, Gender, Address, Phone, Email, DepartmentID, PositionID, BaseSalary, ContractStartDate, ContractEndDate)
VALUES 
(N'Trần Thị Hoa', '1990-05-20', N'Nữ', N'123 Đường Láng, Hà Nội', '0987654321', 'hoa.tran@thanhcong.com', 1, 1, 8000000, '2023-01-01', '2025-12-31'),
(N'Nguyễn Văn Hùng', '1985-03-15', N'Nam', N'456 Đường Giải Phóng, Hà Nội', '0912345678', 'hung.nguyen@thanhcong.com', 2, 2, 15000000, '2022-06-01', '2024-06-01');

-- Attendance
INSERT INTO Attendance (EmployeeID, AttendanceDate, CheckInTime, CheckOutTime, OvertimeHours, Status)
VALUES 
(1, '2025-04-01', '2025-04-01 08:00:00', '2025-04-01 17:00:00', 2.5, 'Present'),
(2, '2025-04-01', '2025-04-01 08:30:00', '2025-04-01 17:30:00', 0, 'Late');

-- LeaveRequests
INSERT INTO LeaveRequests (EmployeeID, LeaveType, StartDate, EndDate, Reason, Status)
VALUES 
(1, 'Annual', '2025-04-10', '2025-04-12', N'Nghỉ phép năm', 'Approved'),
(2, 'Sick', '2025-04-05', '2025-04-06', N'Ốm', 'Pending');

-- Payroll
INSERT INTO Payroll (EmployeeID, Month, Year, BaseSalary, Allowance, Bonus, Tax, Insurance, NetSalary)
VALUES 
(1, 4, 2025, 8000000, 500000, 1000000, 800000, 600000, 8100000),
(2, 4, 2025, 15000000, 1000000, 2000000, 1500000, 1000000, 14500000);

-- Recruitment
INSERT INTO Recruitment (FullName, Email, Phone, AppliedPositionID, CVPath, InterviewDate, Status)
VALUES 
(N'Lê Văn Nam', 'nam.le@gmail.com', '0935123456', 1, '/cvs/nam_le.pdf', '2025-04-15 10:00:00', 'New');

-- Training
INSERT INTO Training (EmployeeID, TrainingName, StartDate, EndDate, Result, Evaluation)
VALUES 
(1, N'Kỹ năng sản xuất', '2025-03-01', '2025-03-05', 'Pass', N'Tốt');

-- PerformanceEvaluations
INSERT INTO PerformanceEvaluations (EmployeeID, EvaluationDate, EvaluatorID, SelfScore, ManagerScore, PeerScore, Comments)
VALUES 
(1, '2025-04-10', 2, 8.5, 8.0, 7.5, N'Làm việc chăm chỉ, cần cải thiện kỹ năng giao tiếp');

GO
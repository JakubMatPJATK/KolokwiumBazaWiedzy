-- Created by Redgate Data Modeler (https://datamodeler.redgate-platform.com)
-- Last modification date: 2026-05-25 14:56:50.318

-- tables
-- Table: Assignments
CREATE TABLE Assignments (
    Id int  NOT NULL IDENTITY,
    LessonsId int  NOT NULL,
    Title nvarchar(200)  NOT NULL,
    DueDate datetime  NOT NULL,
    MaxScore int  NOT NULL,
    CONSTRAINT Assignments_pk PRIMARY KEY  (Id)
);

-- Table: Categories
CREATE TABLE Categories (
    Id int  NOT NULL IDENTITY,
    Name nvarchar(100)  NOT NULL,
    Description nvarchar(max)  NOT NULL,
    CONSTRAINT Categories_pk PRIMARY KEY  (Id)
);

-- Table: Certificates
CREATE TABLE Certificates (
    Id int  NOT NULL IDENTITY,
    UserId int  NOT NULL,
    CourseId int  NOT NULL,
    IssueDate datetime  NOT NULL,
    CertificateCode uniqueidentifier  NOT NULL,
    CONSTRAINT Certificates_pk PRIMARY KEY  (Id)
);

-- Table: Courses
CREATE TABLE Courses (
    Id int  NOT NULL IDENTITY,
    Title nvarchar(200)  NOT NULL,
    Description nvarchar(max)  NOT NULL,
    CategoryId int  NOT NULL,
    InstructorId int  NOT NULL,
    CONSTRAINT Courses_pk PRIMARY KEY  (Id)
);

-- Table: Enrollments
CREATE TABLE Enrollments (
    Id int  NOT NULL IDENTITY,
    UserId int  NOT NULL,
    CourseId int  NOT NULL,
    EnrollmentDate datetime  NOT NULL,
    ProgressPercent int  NOT NULL,
    CONSTRAINT Enrollments_pk PRIMARY KEY  (Id)
);

-- Table: Lessons
CREATE TABLE Lessons (
    Id int  NOT NULL IDENTITY,
    CourseId int  NOT NULL,
    Title nvarchar(200)  NOT NULL,
    DurationMinutes int  NOT NULL,
    CONSTRAINT Lessons_pk PRIMARY KEY  (Id)
);

-- Table: Payments
CREATE TABLE Payments (
    Id int  NOT NULL IDENTITY,
    UserId int  NOT NULL,
    Amount decimal(7,2)  NOT NULL,
    PaymentDate datetime  NOT NULL,
    PaymentMethod nvarchar(100)  NOT NULL,
    CourseId int  NOT NULL,
    CONSTRAINT Payments_pk PRIMARY KEY  (Id)
);

-- Table: Reviews
CREATE TABLE Reviews (
    Id int  NOT NULL IDENTITY,
    CourseId int  NOT NULL,
    UserId int  NOT NULL,
    Rating int  NOT NULL,
    Comment nvarchar(500)  NOT NULL,
    CONSTRAINT Reviews_pk PRIMARY KEY  (Id)
);

-- Table: Roles
CREATE TABLE Roles (
    Id int  NOT NULL IDENTITY,
    Name nvarchar(100)  NOT NULL,
    Description nvarchar(max)  NOT NULL,
    CONSTRAINT Roles_pk PRIMARY KEY  (Id)
);

-- Table: Submissions
CREATE TABLE Submissions (
    Id int  NOT NULL IDENTITY,
    UserId int  NOT NULL,
    AssignmentId int  NOT NULL,
    SubmittedAt datetime  NOT NULL,
    Score int  NOT NULL,
    CONSTRAINT Submissions_pk PRIMARY KEY  (Id)
);

-- Table: UserRoles
CREATE TABLE UserRoles (
    UserId int  NOT NULL,
    RoleId int  NOT NULL,
    AssignedAt datetime  NOT NULL,
    CONSTRAINT UserRoles_pk PRIMARY KEY  (UserId,RoleId)
);

-- Table: Users
CREATE TABLE Users (
    Id int  NOT NULL IDENTITY,
    FirstName nvarchar(50)  NOT NULL,
    LastName nvarchar(100)  NOT NULL,
    Email nvarchar(200)  NOT NULL,
    CONSTRAINT Users_pk PRIMARY KEY  (Id)
);

-- foreign keys
-- Reference: Assignments_Lessons (table: Assignments)
ALTER TABLE Assignments ADD CONSTRAINT Assignments_Lessons
    FOREIGN KEY (LessonsId)
    REFERENCES Lessons (Id);

-- Reference: Certificates_Courses (table: Certificates)
ALTER TABLE Certificates ADD CONSTRAINT Certificates_Courses
    FOREIGN KEY (CourseId)
    REFERENCES Courses (Id);

-- Reference: Certificates_Users (table: Certificates)
ALTER TABLE Certificates ADD CONSTRAINT Certificates_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- Reference: Courses_Categories (table: Courses)
ALTER TABLE Courses ADD CONSTRAINT Courses_Categories
    FOREIGN KEY (CategoryId)
    REFERENCES Categories (Id);

-- Reference: Courses_Users (table: Courses)
ALTER TABLE Courses ADD CONSTRAINT Courses_Users
    FOREIGN KEY (InstructorId)
    REFERENCES Users (Id);

-- Reference: Enrollments_Courses (table: Enrollments)
ALTER TABLE Enrollments ADD CONSTRAINT Enrollments_Courses
    FOREIGN KEY (CourseId)
    REFERENCES Courses (Id);

-- Reference: Enrollments_Users (table: Enrollments)
ALTER TABLE Enrollments ADD CONSTRAINT Enrollments_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- Reference: Lessons_Courses (table: Lessons)
ALTER TABLE Lessons ADD CONSTRAINT Lessons_Courses
    FOREIGN KEY (CourseId)
    REFERENCES Courses (Id);

-- Reference: Payments_Courses (table: Payments)
ALTER TABLE Payments ADD CONSTRAINT Payments_Courses
    FOREIGN KEY (CourseId)
    REFERENCES Courses (Id);

-- Reference: Payments_Users (table: Payments)
ALTER TABLE Payments ADD CONSTRAINT Payments_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- Reference: Reviews_Courses (table: Reviews)
ALTER TABLE Reviews ADD CONSTRAINT Reviews_Courses
    FOREIGN KEY (CourseId)
    REFERENCES Courses (Id);

-- Reference: Reviews_Users (table: Reviews)
ALTER TABLE Reviews ADD CONSTRAINT Reviews_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- Reference: Submissions_Assignments (table: Submissions)
ALTER TABLE Submissions ADD CONSTRAINT Submissions_Assignments
    FOREIGN KEY (AssignmentId)
    REFERENCES Assignments (Id);

-- Reference: Submissions_Users (table: Submissions)
ALTER TABLE Submissions ADD CONSTRAINT Submissions_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- Reference: UserRoles_Roles (table: UserRoles)
ALTER TABLE UserRoles ADD CONSTRAINT UserRoles_Roles
    FOREIGN KEY (RoleId)
    REFERENCES Roles (Id);

-- Reference: UserRoles_Users (table: UserRoles)
ALTER TABLE UserRoles ADD CONSTRAINT UserRoles_Users
    FOREIGN KEY (UserId)
    REFERENCES Users (Id);

-- End of file.

-- =========================================
-- Users
-- =========================================
INSERT INTO Users (FirstName, LastName, Email)
VALUES
('John', 'Smith', 'john.smith@email.com'),
('Anna', 'Johnson', 'anna.johnson@email.com'),
('Michael', 'Brown', 'michael.brown@email.com'),
('Emily', 'Davis', 'emily.davis@email.com'),
('Daniel', 'Wilson', 'daniel.wilson@email.com');

-- =========================================
-- Roles
-- =========================================
INSERT INTO Roles (Name, Description)
VALUES
('Administrator', 'System administrator'),
('Instructor', 'Course instructor'),
('Student', 'Platform student'),
('Moderator', 'Content moderator'),
('Guest', 'Guest account');

-- =========================================
-- UserRoles
-- =========================================
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES
(1, 1, '2026-01-01'),
(2, 2, '2026-01-02'),
(3, 3, '2026-01-03'),
(4, 3, '2026-01-04'),
(5, 2, '2026-01-05');

-- =========================================
-- Categories
-- =========================================
INSERT INTO Categories (Name, Description)
VALUES
('Programming', 'Software development courses'),
('Database', 'Database management courses'),
('Design', 'Graphic and UI design courses'),
('Marketing', 'Digital marketing courses'),
('Business', 'Business management courses');

-- =========================================
-- Courses
-- =========================================
INSERT INTO Courses (Title, Description, CategoryId, InstructorId)
VALUES
('C# Fundamentals', 'Introduction to C# programming', 1, 2),
('SQL Server Basics', 'Learn SQL Server databases', 2, 5),
('UI Design Essentials', 'Introduction to UI design', 3, 2),
('Digital Marketing 101', 'Basics of digital marketing', 4, 5),
('Project Management Basics', 'Learn project management', 5, 2);

-- =========================================
-- Lessons
-- =========================================
INSERT INTO Lessons (CourseId, Title, DurationMinutes)
VALUES
(1, 'Variables and Data Types', 45),
(2, 'Creating Tables', 60),
(3, 'Color Theory', 40),
(4, 'SEO Fundamentals', 50),
(5, 'Agile Methodology', 55);

-- =========================================
-- Assignments
-- =========================================
INSERT INTO Assignments (LessonsId, Title, DueDate, MaxScore)
VALUES
(1, 'C# Variables Exercise', '2026-06-01', 100),
(2, 'SQL Tables Assignment', '2026-06-03', 100),
(3, 'UI Mockup Design', '2026-06-05', 100),
(4, 'SEO Strategy Task', '2026-06-07', 100),
(5, 'Agile Sprint Planning', '2026-06-10', 100);

-- =========================================
-- Enrollments
-- =========================================
INSERT INTO Enrollments (UserId, CourseId, EnrollmentDate, ProgressPercent)
VALUES
(3, 1, '2026-05-01', 40),
(4, 2, '2026-05-02', 65),
(1, 3, '2026-05-03', 20),
(3, 4, '2026-05-04', 80),
(4, 5, '2026-05-05', 50);

-- =========================================
-- Payments
-- =========================================
INSERT INTO Payments (UserId, Amount, PaymentDate, PaymentMethod, CourseId)
VALUES
(3, 199.99, '2026-05-01', 'Credit Card', 1),
(4, 149.99, '2026-05-02', 'PayPal', 2),
(1, 99.99, '2026-05-03', 'Bank Transfer', 3),
(3, 249.99, '2026-05-04', 'Credit Card', 4),
(4, 179.99, '2026-05-05', 'Blik', 5);

-- =========================================
-- Reviews
-- =========================================
INSERT INTO Reviews (CourseId, UserId, Rating, Comment)
VALUES
(1, 3, 5, 'Excellent programming course'),
(2, 4, 4, 'Very useful SQL lessons'),
(3, 1, 5, 'Amazing design materials'),
(4, 3, 3, 'Interesting but too short'),
(5, 4, 4, 'Good project management basics');

-- =========================================
-- Certificates
-- =========================================
INSERT INTO Certificates (UserId, CourseId, IssueDate, CertificateCode)
VALUES
(3, 1, '2026-06-15', NEWID()),
(4, 2, '2026-06-16', NEWID()),
(1, 3, '2026-06-17', NEWID()),
(3, 4, '2026-06-18', NEWID()),
(4, 5, '2026-06-19', NEWID());

-- =========================================
-- Submissions
-- =========================================
INSERT INTO Submissions (UserId, AssignmentId, SubmittedAt, Score)
VALUES
(3, 1, '2026-05-20 10:00:00', 95),
(4, 2, '2026-05-21 11:30:00', 88),
(1, 3, '2026-05-22 09:15:00', 91),
(3, 4, '2026-05-23 14:45:00', 76),
(4, 5, '2026-05-24 16:20:00', 84);



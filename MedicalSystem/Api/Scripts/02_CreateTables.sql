USE Db;

-- Таблица Patients
IF OBJECT_ID('Patients', 'U') IS NULL
BEGIN
    CREATE TABLE Patients (
        PatientID INT PRIMARY KEY IDENTITY(1,1),
        CardNumber NVARCHAR(20) UNIQUE NOT NULL,
        OmsNumber NVARCHAR(16) UNIQUE NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Patronymic NVARCHAR(50),
        BirthDate DATE NOT NULL,
        Gender NCHAR(1) NOT NULL,
        Phone NVARCHAR(20),
        Address NVARCHAR(50),
        Email NVARCHAR(50),
        Allergies NVARCHAR(500),
        ChronicDiseases NVARCHAR(500),
        CreatedDate DATETIME2 DEFAULT GETDATE()
    );
END

-- Таблица Departments
IF OBJECT_ID('Departments', 'U') IS NULL
BEGIN
    CREATE TABLE Departments (
        DepartmentID INT PRIMARY KEY IDENTITY(1,1),
        DepartmentName NVARCHAR(50) UNIQUE NOT NULL,
        DepartmentCode NVARCHAR(20) UNIQUE NOT NULL,
        DepartmentType NVARCHAR(50) NOT NULL,
        HeadDoctorID INT,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME2 DEFAULT GETDATE()
    );
END

-- Таблица Users
IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY IDENTITY(1,1),
        Login NVARCHAR(50) UNIQUE NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(50),
        LastName NVARCHAR(50),
        Patronymic NVARCHAR(50),
        Role NVARCHAR(50) NOT NULL,
        Specialty NVARCHAR(50),
        DepartmentID INT NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );

    ALTER TABLE Users 
        ADD CONSTRAINT FK_Users_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID);
END

-- Таблица Diagnoses
IF OBJECT_ID('Diagnoses', 'U') IS NULL
BEGIN
    CREATE TABLE Diagnoses (
        DiagnosisID INT PRIMARY KEY IDENTITY(1,1),
        ICD10Code NVARCHAR(10) NOT NULL,
        DiagnosisName NVARCHAR(50) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500)
    );
END

-- Таблица Visits
IF OBJECT_ID('Visits', 'U') IS NULL
BEGIN
    CREATE TABLE Visits (
        VisitID INT PRIMARY KEY IDENTITY(1,1),
        PatientID INT NOT NULL,
        DoctorID INT NOT NULL,
        VisitDate DATETIME2 NOT NULL,
        VisitType NVARCHAR(50) NOT NULL,
        Complaints NVARCHAR(500),
        Anamnesis NVARCHAR(500),
        ObjectiveStatus NVARCHAR(500),
        DiagnosisID INT,
        TreatmentPlan NVARCHAR(500),
        Recommendations NVARCHAR(500),
        NextVisitDate DATE,
        CreatedDate DATETIME2 DEFAULT GETDATE()
    );

    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID);
    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Users FOREIGN KEY (DoctorID) REFERENCES Users(UserID);
    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Diagnoses FOREIGN KEY (DiagnosisID) REFERENCES Diagnoses(DiagnosisID);
END

-- Таблица Treatments
IF OBJECT_ID('Treatments', 'U') IS NULL
BEGIN
    CREATE TABLE Treatments (
        TreatmentID INT PRIMARY KEY IDENTITY(1,1),
        VisitID INT NOT NULL,
        TreatmentType NVARCHAR(50),
        Dosage NVARCHAR(100),
        Frequency NVARCHAR(100),
        Duration NVARCHAR(100),
        Instructions NVARCHAR(500),
        Eye NCHAR(2),
        StartDate DATE,
        EndDate DATE
    );

    ALTER TABLE Treatments ADD CONSTRAINT FK_Treatments_Visits FOREIGN KEY (VisitID) REFERENCES Visits(VisitID);
END

-- Таблица SavedFilters
IF OBJECT_ID('SavedFilters', 'U') IS NULL
BEGIN
    CREATE TABLE SavedFilters (
        FilterID INT PRIMARY KEY IDENTITY(1,1),
        UserID INT NOT NULL,
        FilterName NVARCHAR(50) NOT NULL,
        FilterJSON NVARCHAR(MAX) NOT NULL,
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
    );
END

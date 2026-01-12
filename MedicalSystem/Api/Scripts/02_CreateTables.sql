IF OBJECT_ID('Patients', 'U') IS NULL
BEGIN
    CREATE TABLE Patients (
        Id BIGINT PRIMARY KEY IDENTITY(1,1),
        CardNumber NVARCHAR(10) UNIQUE NOT NULL,
        OmsNumber NVARCHAR(16) UNIQUE NULL,
        LastName NVARCHAR(50) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        Patronymic NVARCHAR(50),
        BirthDate DATE NOT NULL,
        Gender NCHAR(1) NOT NULL,
        Phone NVARCHAR(20),
        Address NVARCHAR(50),
        Email NVARCHAR(50),
        Allergies NVARCHAR(500),
        ChronicDiseases NVARCHAR(500),
        CreatedDate DATE DEFAULT GETDATE()
    );
END


IF OBJECT_ID('Departments', 'U') IS NULL
BEGIN
    CREATE TABLE Departments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(50) UNIQUE NOT NULL,
        Code NVARCHAR(20) UNIQUE NOT NULL,
        Type NVARCHAR(50) NOT NULL,
        HeadDoctorId INT,
        IsActive BIT DEFAULT 1
    );
END


IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        Id BIGINT PRIMARY KEY IDENTITY(1,1),
        Login NVARCHAR(50) UNIQUE NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        LastName NVARCHAR(50),
        FirstName NVARCHAR(50),
        Patronymic NVARCHAR(50),
        Role NVARCHAR(50) NOT NULL,
        Specialty NVARCHAR(50),
        DepartmentId INT NOT NULL,
        IsActive BIT DEFAULT 1
    );

    ALTER TABLE Users 
        ADD CONSTRAINT FK_Users_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id);
END


IF OBJECT_ID('Diagnoses', 'U') IS NULL
BEGIN
    CREATE TABLE Diagnoses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Code NVARCHAR(10) NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500)
    );
END


IF OBJECT_ID('Visits', 'U') IS NULL
BEGIN
    CREATE TABLE Visits (
        Id BIGINT PRIMARY KEY IDENTITY(1,1),
        PatientId BIGINT NOT NULL,
        DoctorId BIGINT NOT NULL,
        Type NVARCHAR(50) NOT NULL,
        Complaints NVARCHAR(500),
        Anamnesis NVARCHAR(500),
        ObjectiveStatus NVARCHAR(500),
        DiagnosisId INT,
        TreatmentPlan NVARCHAR(500),
        Recommendations NVARCHAR(500),
        NextVisitDate DATE,
        Date DATETIME2 DEFAULT GETDATE()
    );

    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Patients FOREIGN KEY (PatientId) REFERENCES Patients(Id);
    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Users FOREIGN KEY (DoctorId) REFERENCES Users(Id);
    ALTER TABLE Visits ADD CONSTRAINT FK_Visits_Diagnoses FOREIGN KEY (DiagnosisId) REFERENCES Diagnoses(Id);
END


IF OBJECT_ID('Treatments', 'U') IS NULL
BEGIN
    CREATE TABLE Treatments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        VisitId BIGINT NOT NULL,
        Type NVARCHAR(50),
        Dosage NVARCHAR(100),
        Frequency NVARCHAR(100),
        Duration NVARCHAR(100),
        Instructions NVARCHAR(500),
        Eye NCHAR(2),
        StartDate DATE,
        EndDate DATE
    );

    ALTER TABLE Treatments ADD CONSTRAINT FK_Treatments_Visits FOREIGN KEY (VisitId) REFERENCES Visits(Id);
END


IF OBJECT_ID('SavedFilters', 'U') IS NULL
BEGIN
    CREATE TABLE SavedFilters (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId BIGINT NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        JSON NVARCHAR(MAX) NOT NULL,
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
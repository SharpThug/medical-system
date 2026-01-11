USE Db;

IF NOT EXISTS (SELECT 1 FROM Departments WHERE DepartmentCode = 'OFT')
BEGIN
    INSERT INTO Departments (DepartmentName, DepartmentCode, DepartmentType)
    VALUES ('Офтальмология', 'OFT', 'Хирургическое');
END

IF NOT EXISTS (SELECT 1 FROM Departments WHERE DepartmentCode = 'TER')
BEGIN
    INSERT INTO Departments (DepartmentName, DepartmentCode, DepartmentType)
    VALUES ('Терапия', 'TER', 'Терапевтическое');
END
USE Db;

IF NOT EXISTS (SELECT 1 FROM Departments WHERE Code = 'OFT')
BEGIN
    INSERT INTO Departments (Name, Code, Type)
    VALUES ('Офтальмология', 'OFT', 'Хирургическое');
END

IF NOT EXISTS (SELECT 1 FROM Departments WHERE Code = 'TER')
BEGIN
    INSERT INTO Departments (Name, Code, Type)
    VALUES ('Терапия', 'TER', 'Терапевтическое');
END
using Microsoft.Data.SqlClient;
using System.Data;
using BCrypt.Net;

namespace Api
{
    public class DatabaseSeedingService : IDatabaseSeeder
    {
        private readonly string _connectionString;

        public DatabaseSeedingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SeedDataAsync()
        {
            var departments = new[]
            {
                new { Name = "Офтальмология", Code = "OFT", Type = "Хирургическое" },
                new { Name = "Терапия", Code = "TER", Type = "Терапевтическое" }
            };

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Departments
                    WHERE Code = @Code
                )
                INSERT INTO Departments (Name, Code, Type)
                VALUES (@Name, @Code, @Type);
                """,
                conn))
            {
                foreach (var dept in departments)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = dept.Name;
                    cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 20).Value = dept.Code;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = dept.Type;
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            string plainPassword = "12345";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            using (SqlCommand cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Users
                    WHERE Login = @Login
                )
                INSERT INTO Users 
                    (Login, Password, LastName, FirstName, Patronymic, Role, Specialty, DepartmentId, IsActive)
                VALUES 
                    (@Login, @Password, @LastName, @FirstName, @Patronymic, @Role, @Specialty, @DepartmentId, 1);
                """,
                conn))
                        {
                cmd.Parameters.Add("@Login", SqlDbType.NVarChar, 50).Value = "Ivanova";
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = hashedPassword;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = "Иванова";
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = "Ирина";
                cmd.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50).Value = "Сергеевна";
                cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = "Doctor";
                cmd.Parameters.Add("@Specialty", SqlDbType.NVarChar, 50).Value = "Офтальмолог";
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = 1;

                await cmd.ExecuteNonQueryAsync();
            }

            await SeedPatientsAsync();

            Console.WriteLine("Данные вставлены в базу, включая пользователя Ivanova.");
        }

        private async Task SeedPatientsAsync()
        {
            var patients = new[]
            {
                new { CardNumber = "P0001", OmsNumber = "1234567890123456", LastName = "Петров", FirstName = "Алексей", Patronymic = "Сергеевич", BirthDate = new DateTime(1985, 4, 12), Gender = "M", Phone = "+7 900 123 45 67", Address = "г. Москва, ул. Ленина, д.1", Email = "petrov@example.com", Allergies = "Нет", ChronicDiseases = "Близорукость, Астигматизм" },
                new { CardNumber = "P0002", OmsNumber = "2345678901234567", LastName = "Иванова", FirstName = "Мария", Patronymic = "Игоревна", BirthDate = new DateTime(1990, 9, 5), Gender = "F", Phone = "+7 900 234 56 78", Address = "г. Санкт-Петербург, ул. Пушкина, д.5", Email = "ivanova@example.com", Allergies = "Пенициллин", ChronicDiseases = "Катаракта начальная" },
                new { CardNumber = "P0003", OmsNumber = "3456789012345678", LastName = "Смирнов", FirstName = "Дмитрий", Patronymic = "Александрович", BirthDate = new DateTime(2000, 12, 21), Gender = "M", Phone = "+7 900 345 67 89", Address = "г. Казань, ул. Баумана, д.10", Email = "smirnov@example.com", Allergies = "Нет", ChronicDiseases = "Дальнозоркость возрастная" },
                new { CardNumber = "P0004", OmsNumber = "4567890123456789", LastName = "Кузнецова", FirstName = "Екатерина", Patronymic = "Павловна", BirthDate = new DateTime(1988, 3, 15), Gender = "F", Phone = "+7 901 123 45 01", Address = "г. Новосибирск, ул. Кирова, д.12", Email = "kuznetsova@example.com", Allergies = "Арахис", ChronicDiseases = "Глаукома начальной стадии" },
                new { CardNumber = "P0005", OmsNumber = "5678901234567890", LastName = "Федоров", FirstName = "Иван", Patronymic = "Михайлович", BirthDate = new DateTime(1975, 7, 22), Gender = "M", Phone = "+7 902 234 56 23", Address = "г. Екатеринбург, ул. Мира, д.3", Email = "fedorov@example.com", Allergies = "Нет", ChronicDiseases = "Пресбиопия, Катаракта" },
                new { CardNumber = "P0006", OmsNumber = "6789012345678901", LastName = "Соколова", FirstName = "Ольга", Patronymic = "Владимировна", BirthDate = new DateTime(1995, 1, 30), Gender = "F", Phone = "+7 903 345 67 34", Address = "г. Нижний Новгород, ул. Советская, д.8", Email = "sokolova@example.com", Allergies = "Пыльца", ChronicDiseases = "Близорукость высокой степени" },
                new { CardNumber = "P0007", OmsNumber = "7890123456789012", LastName = "Морозов", FirstName = "Александр", Patronymic = "Сергеевич", BirthDate = new DateTime(1982, 11, 2), Gender = "M", Phone = "+7 904 456 78 45", Address = "г. Самара, ул. Галактионовская, д.20", Email = "morozov@example.com", Allergies = "Нет", ChronicDiseases = "Кератоконус" },
                new { CardNumber = "P0008", OmsNumber = "8901234567890123", LastName = "Попова", FirstName = "Анна", Patronymic = "Дмитриевна", BirthDate = new DateTime(1992, 5, 18), Gender = "F", Phone = "+7 905 567 89 56", Address = "г. Ростов-на-Дону, ул. Будённого, д.14", Email = "popova@example.com", Allergies = "Лактоза", ChronicDiseases = "Диабетическая ретинопатия" },
                new { CardNumber = "P0009", OmsNumber = "9012345678901234", LastName = "Лебедев", FirstName = "Сергей", Patronymic = "Викторович", BirthDate = new DateTime(1980, 2, 10), Gender = "M", Phone = "+7 906 678 90 67", Address = "г. Волгоград, ул. Комсомольская, д.7", Email = "lebedev@example.com", Allergies = "Нет", ChronicDiseases = "Синдром сухого глаза" },
                new { CardNumber = "P0010", OmsNumber = "0123456789012345", LastName = "Козлова", FirstName = "Ирина", Patronymic = "Александровна", BirthDate = new DateTime(1987, 8, 25), Gender = "F", Phone = "+7 907 789 01 78", Address = "г. Пермь, ул. Сибирская, д.11", Email = "kozlova@example.com", Allergies = "Пыль", ChronicDiseases = "Макулярная дегенерация" },
                new { CardNumber = "P0011", OmsNumber = "1123456789012345", LastName = "Новиков", FirstName = "Михаил", Patronymic = "Андреевич", BirthDate = new DateTime(1991, 12, 3), Gender = "M", Phone = "+7 908 890 12 89", Address = "г. Уфа, ул. Ленина, д.9", Email = "novikov@example.com", Allergies = "Нет", ChronicDiseases = "Астигматизм, Близорукость" },
                new { CardNumber = "P0012", OmsNumber = "1223456789012345", LastName = "Волкова", FirstName = "Светлана", Patronymic = "Павловна", BirthDate = new DateTime(1996, 6, 9), Gender = "F", Phone = "+7 909 901 23 90", Address = "г. Краснодар, ул. Красная, д.2", Email = "volkova@example.com", Allergies = "Арахис", ChronicDiseases = "Воспаление век (блефарит)" },
                new { CardNumber = "P0013", OmsNumber = "1323456789012345", LastName = "Сергеев", FirstName = "Андрей", Patronymic = "Николаевич", BirthDate = new DateTime(1983, 9, 17), Gender = "M", Phone = "+7 910 012 34 12", Address = "г. Ярославль, ул. Первомайская, д.6", Email = "sergeev@example.com", Allergies = "Пенициллин", ChronicDiseases = "Отслоение сетчатки (в анамнезе)" },
                new { CardNumber = "P0014", OmsNumber = "1423456789012345", LastName = "Михайлова", FirstName = "Татьяна", Patronymic = "Сергеевна", BirthDate = new DateTime(1994, 4, 27), Gender = "F", Phone = "+7 911 123 45 23", Address = "г. Тольятти, ул. Советская, д.13", Email = "mikhailova@example.com", Allergies = "Нет", ChronicDiseases = "Косоглазие (страбизм)" },
                new { CardNumber = "P0015", OmsNumber = "1523456789012345", LastName = "Егоров", FirstName = "Денис", Patronymic = "Владимирович", BirthDate = new DateTime(1978, 11, 11), Gender = "M", Phone = "+7 912 234 56 34", Address = "г. Челябинск, ул. Ленина, д.5", Email = "egorov@example.com", Allergies = "Пыльца", ChronicDiseases = "Глаукома открытоугольная" },
                new { CardNumber = "P0016", OmsNumber = "1623456789012345", LastName = "Александрова", FirstName = "Марина", Patronymic = "Игоревна", BirthDate = new DateTime(1989, 1, 19), Gender = "F", Phone = "+7 913 345 67 45", Address = "г. Воронеж, ул. Пушкинская, д.8", Email = "alexandrova@example.com", Allergies = "Лактоза", ChronicDiseases = "Амблиопия (ленивый глаз)" },
                new { CardNumber = "P0017", OmsNumber = "1723456789012345", LastName = "Кузьмин", FirstName = "Никита", Patronymic = "Александрович", BirthDate = new DateTime(1993, 3, 23), Gender = "M", Phone = "+7 914 456 78 56", Address = "г. Оренбург, ул. Мира, д.4", Email = "kuzmin@example.com", Allergies = "Нет", ChronicDiseases = "Увеит хронический" },
                new { CardNumber = "P0018", OmsNumber = "1823456789012345", LastName = "Григорьева", FirstName = "Елена", Patronymic = "Сергеевна", BirthDate = new DateTime(1997, 8, 7), Gender = "F", Phone = "+7 915 567 89 67", Address = "г. Иркутск, ул. Ленина, д.10", Email = "grigorieva@example.com", Allergies = "Пенициллин", ChronicDiseases = "Дистрофия роговицы" },
                new { CardNumber = "P0019", OmsNumber = "1923456789012345", LastName = "Мельников", FirstName = "Роман", Patronymic = "Дмитриевич", BirthDate = new DateTime(1986, 12, 14), Gender = "M", Phone = "+7 916 678 90 78", Address = "г. Кемерово, ул. Советская, д.9", Email = "melnikov@example.com", Allergies = "Нет", ChronicDiseases = "Катаракта зрелая" },
                new { CardNumber = "P0020", OmsNumber = "2023456789012345", LastName = "Фролова", FirstName = "Наталья", Patronymic = "Владимировна", BirthDate = new DateTime(1991, 6, 30), Gender = "F", Phone = "+7 917 789 01 89", Address = "г. Ярославль, ул. Красная, д.15", Email = "frolova@example.com", Allergies = "Арахис", ChronicDiseases = "Ретинопатия недоношенных" },
                new { CardNumber = "P0021", OmsNumber = "2123456789012345", LastName = "Савельев", FirstName = "Владимир", Patronymic = "Игоревич", BirthDate = new DateTime(1984, 5, 5), Gender = "M", Phone = "+7 918 890 12 90", Address = "г. Пенза, ул. Ленина, д.11", Email = "savelev@example.com", Allergies = "Пыльца", ChronicDiseases = "Пигментный ретинит" },
                new { CardNumber = "P0022", OmsNumber = "2223456789012345", LastName = "Мухина", FirstName = "Оксана", Patronymic = "Павловна", BirthDate = new DateTime(1992, 9, 20), Gender = "F", Phone = "+7 919 901 23 01", Address = "г. Тула, ул. Советская, д.6", Email = "mukhina@example.com", Allergies = "Лактоза", ChronicDiseases = "Глазная мигрень" },
                new { CardNumber = "P0023", OmsNumber = "2323456789012345", LastName = "Беляев", FirstName = "Артём", Patronymic = "Михайлович", BirthDate = new DateTime(1988, 7, 12), Gender = "M", Phone = "+7 920 012 34 12", Address = "г. Владимир, ул. Мира, д.3", Email = "belyaev@example.com", Allergies = "Нет", ChronicDiseases = "Нистагм" },
                new { CardNumber = "P0024", OmsNumber = "2423456789012345", LastName = "Семёнова", FirstName = "Людмила", Patronymic = "Николаевна", BirthDate = new DateTime(1990, 10, 8), Gender = "F", Phone = "+7 921 123 45 23", Address = "г. Смоленск, ул. Ленина, д.7", Email = "semenova@example.com", Allergies = "Пыльца", ChronicDiseases = "Глазной герпес рецидивирующий" },
                new { CardNumber = "P0025", OmsNumber = "2523456789012345", LastName = "Крылов", FirstName = "Константин", Patronymic = "Владимирович", BirthDate = new DateTime(1983, 2, 28), Gender = "M", Phone = "+7 922 234 56 34", Address = "г. Томск, ул. Советская, д.4", Email = "krylov@example.com", Allergies = "Нет", ChronicDiseases = "Аллергический конъюнктивит" },
                new { CardNumber = "P0026", OmsNumber = "2623456789012345", LastName = "Орлов", FirstName = "Павел", Patronymic = "Анатольевич", BirthDate = new DateTime(1979, 3, 14), Gender = "M", Phone = "+7 923 345 67 45", Address = "г. Брянск, ул. Калинина, д.2", Email = "orlov@example.com", Allergies = "Нет", ChronicDiseases = "Диабетический макулярный отек" },
                new { CardNumber = "P0027", OmsNumber = "2723456789012345", LastName = "Тихонова", FirstName = "Вероника", Patronymic = "Олеговна", BirthDate = new DateTime(1995, 7, 22), Gender = "F", Phone = "+7 924 456 78 56", Address = "г. Белгород, ул. Гагарина, д.3", Email = "tikhonova@example.com", Allergies = "Антибиотики", ChronicDiseases = "Кератит" },
                new { CardNumber = "P0028", OmsNumber = "2823456789012345", LastName = "Макаров", FirstName = "Геннадий", Patronymic = "Сергеевич", BirthDate = new DateTime(1981, 11, 30), Gender = "M", Phone = "+7 925 567 89 67", Address = "г. Владивосток, ул. Набережная, д.5", Email = "makarov@example.com", Allergies = "Нет", ChronicDiseases = "Глаукома закрытоугольная" },
                new { CardNumber = "P0029", OmsNumber = "2923456789012345", LastName = "Захарова", FirstName = "Ксения", Patronymic = "Витальевна", BirthDate = new DateTime(1993, 4, 18), Gender = "F", Phone = "+7 926 678 90 78", Address = "г. Хабаровск, ул. Ленина, д.12", Email = "zakharova@example.com", Allergies = "Пыль", ChronicDiseases = "Макулярный разрыв" },
                new { CardNumber = "P0030", OmsNumber = "3023456789012345", LastName = "Воронов", FirstName = "Евгений", Patronymic = "Иванович", BirthDate = new DateTime(1976, 8, 9), Gender = "M", Phone = "+7 927 789 01 89", Address = "г. Архангельск, ул. Северная, д.7", Email = "voronov@example.com", Allergies = "Нет", ChronicDiseases = "Ангиопатия сетчатки" },
                new { CardNumber = "P0031", OmsNumber = "3123456789012345", LastName = "Данилова", FirstName = "Алиса", Patronymic = "Романовна", BirthDate = new DateTime(1998, 2, 25), Gender = "F", Phone = "+7 928 890 12 90", Address = "г. Сочи, ул. Курортная, д.4", Email = "danilova@example.com", Allergies = "Пенициллин", ChronicDiseases = "Миопия средней степени" },
                new { CardNumber = "P0032", OmsNumber = "3223456789012345", LastName = "Жуков", FirstName = "Станислав", Patronymic = "Алексеевич", BirthDate = new DateTime(1985, 6, 11), Gender = "M", Phone = "+7 929 901 23 01", Address = "г. Калининград, ул. Балтийская, д.9", Email = "zhukov@example.com", Allergies = "Нет", ChronicDiseases = "Хориоретинит" },
                new { CardNumber = "P0033", OmsNumber = "3323456789012345", LastName = "Сорокина", FirstName = "Валентина", Patronymic = "Петровна", BirthDate = new DateTime(1972, 12, 19), Gender = "F", Phone = "+7 930 012 34 12", Address = "г. Тверь, ул. Кирова, д.6", Email = "sorokina@example.com", Allergies = "Шерсть животных", ChronicDiseases = "Катаракта послеоперационная" },
                new { CardNumber = "P0034", OmsNumber = "3423456789012345", LastName = "Тарасов", FirstName = "Григорий", Patronymic = "Викторович", BirthDate = new DateTime(1989, 5, 7), Gender = "M", Phone = "+7 931 123 45 23", Address = "г. Липецк, ул. Зеленая, д.3", Email = "tarasov@example.com", Allergies = "Нет", ChronicDiseases = "Птоз верхнего века" },
                new { CardNumber = "P0035", OmsNumber = "3523456789012345", LastName = "Устинова", FirstName = "Галина", Patronymic = "Николаевна", BirthDate = new DateTime(1990, 9, 13), Gender = "F", Phone = "+7 932 234 56 34", Address = "г. Курск, ул. Садовая, д.8", Email = "ustinova@example.com", Allergies = "Пыльца березы", ChronicDiseases = "Эпиретинальный фиброз" },
                new { CardNumber = "P0036", OmsNumber = "3623456789012345", LastName = "Фомин", FirstName = "Валерий", Patronymic = "Олегович", BirthDate = new DateTime(1977, 1, 28), Gender = "M", Phone = "+7 933 345 67 45", Address = "г. Орел, ул. Московская, д.5", Email = "fomin@example.com", Allergies = "Нет", ChronicDiseases = "Иридоциклит" },
                new { CardNumber = "P0037", OmsNumber = "3723456789012345", LastName = "Чернова", FirstName = "Диана", Patronymic = "Ильинична", BirthDate = new DateTime(1994, 3, 17), Gender = "F", Phone = "+7 934 456 78 56", Address = "г. Саратов, ул. Волжская, д.10", Email = "chernova@example.com", Allergies = "Морепродукты", ChronicDiseases = "Кератоглобус" },
                new { CardNumber = "P0038", OmsNumber = "3823456789012345", LastName = "Широков", FirstName = "Борис", Patronymic = "Федорович", BirthDate = new DateTime(1968, 10, 5), Gender = "M", Phone = "+7 935 567 89 67", Address = "г. Ульяновск, ул. Гончарова, д.4", Email = "shirokov@example.com", Allergies = "Нет", ChronicDiseases = "Возрастная дальнозоркость, Катаракта" },
                new { CardNumber = "P0039", OmsNumber = "3923456789012345", LastName = "Щербакова", FirstName = "Эльвира", Patronymic = "Рафаэлевна", BirthDate = new DateTime(1983, 7, 8), Gender = "F", Phone = "+7 936 678 90 78", Address = "г. Чебоксары, ул. К. Маркса, д.7", Email = "scherbakova@example.com", Allergies = "Йод", ChronicDiseases = "Глазная гипертензия" },
                new { CardNumber = "P0040", OmsNumber = "4023456789012345", LastName = "Яковлев", FirstName = "Арсений", Patronymic = "Денисович", BirthDate = new DateTime(1996, 11, 21), Gender = "M", Phone = "+7 937 789 01 89", Address = "г. Барнаул, ул. Ползунова, д.6", Email = "yakovlev@example.com", Allergies = "Нет", ChronicDiseases = "Анизометропия" },
                new { CardNumber = "P0041", OmsNumber = "4123456789012345", LastName = "Борисова", FirstName = "Зоя", Patronymic = "Владиславовна", BirthDate = new DateTime(1987, 4, 3), Gender = "F", Phone = "+7 938 890 12 90", Address = "г. Иваново, ул. Лежневская, д.9", Email = "borisova@example.com", Allergies = "Аспирин", ChronicDiseases = "Халькоз (отложение меди в глазу)" },
                new { CardNumber = "P0042", OmsNumber = "4223456789012345", LastName = "Владимиров", FirstName = "Святослав", Patronymic = "Геннадьевич", BirthDate = new DateTime(1979, 8, 16), Gender = "M", Phone = "+7 939 901 23 01", Address = "г. Магнитогорск, ул. Пионерская, д.11", Email = "vladimirov@example.com", Allergies = "Нет", ChronicDiseases = "Колобома радужки" },
                new { CardNumber = "P0043", OmsNumber = "4323456789012345", LastName = "Гаврилова", FirstName = "Регина", Patronymic = "Станиславовна", BirthDate = new DateTime(1991, 12, 7), Gender = "F", Phone = "+7 940 012 34 12", Address = "г. Тюмень, ул. Республики, д.15", Email = "gavrilova@example.com", Allergies = "Шоколад", ChronicDiseases = "Дистрофия Фукса" },
                new { CardNumber = "P0044", OmsNumber = "4423456789012345", LastName = "Демин", FirstName = "Ярослав", Patronymic = "Русланович", BirthDate = new DateTime(1984, 2, 14), Gender = "M", Phone = "+7 941 123 45 23", Address = "г. Омск, ул. Ленина, д.20", Email = "demin@example.com", Allergies = "Нет", ChronicDiseases = "Афакия (отсутствие хрусталика)" },
                new { CardNumber = "P0045", OmsNumber = "4523456789012345", LastName = "Ефимова", FirstName = "Ульяна", Patronymic = "Артемовна", BirthDate = new DateTime(1997, 6, 28), Gender = "F", Phone = "+7 942 234 56 34", Address = "г. Киров, ул. Воровского, д.8", Email = "efimova@example.com", Allergies = "Пыльца полыни", ChronicDiseases = "Пигментная дисперсия" },
                new { CardNumber = "P0046", OmsNumber = "4623456789012345", LastName = "Зимин", FirstName = "Всеволод", Patronymic = "Борисович", BirthDate = new DateTime(1973, 10, 11), Gender = "M", Phone = "+7 943 345 67 45", Address = "г. Астрахань, ул. Советская, д.13", Email = "zimin@example.com", Allergies = "Нет", ChronicDiseases = "Гетерохромия радужки" },
                new { CardNumber = "P0047", OmsNumber = "4723456789012345", LastName = "Ильина", FirstName = "Карина", Patronymic = "Эдуардовна", BirthDate = new DateTime(1986, 5, 24), Gender = "F", Phone = "+7 944 456 78 56", Address = "г. Мурманск, ул. Ленинградская, д.5", Email = "ilina@example.com", Allergies = "Молочные продукты", ChronicDiseases = "Кератомаляция" },
                new { CardNumber = "P0048", OmsNumber = "4823456789012345", LastName = "Карпов", FirstName = "Тимофей", Patronymic = "Яковлевич", BirthDate = new DateTime(1965, 9, 9), Gender = "M", Phone = "+7 945 567 89 67", Address = "г. Владикавказ, ул. Мира, д.7", Email = "karpov@example.com", Allergies = "Нет", ChronicDiseases = "Склерит" },
                new { CardNumber = "P0049", OmsNumber = "4923456789012345", LastName = "Ларина", FirstName = "Жанна", Patronymic = "Валерьевна", BirthDate = new DateTime(1992, 1, 31), Gender = "F", Phone = "+7 946 678 90 78", Address = "г. Грозный, ул. Маяковского, д.9", Email = "larina@example.com", Allergies = "Анестетики", ChronicDiseases = "Миодезопсия (летающие мушки)" },
                new { CardNumber = "P0050", OmsNumber = "5023456789012345", LastName = "Маслов", FirstName = "Прохор", Patronymic = "Семенович", BirthDate = new DateTime(1978, 7, 4), Gender = "M", Phone = "+7 947 789 01 89", Address = "г. Йошкар-Ола, ул. Воинов-Интернационалистов, д.4", Email = "maslov@example.com", Allergies = "Нет", ChronicDiseases = "Окклюзия центральной вены сетчатки" },
                new { CardNumber = "P0051", OmsNumber = "5123456789012345", LastName = "Никитина", FirstName = "Эмма", Patronymic = "Георгиевна", BirthDate = new DateTime(1980, 3, 19), Gender = "F", Phone = "+7 948 890 12 90", Address = "г. Сыктывкар, ул. Коммунистическая, д.12", Email = "nikitina@example.com", Allergies = "Соевые продукты", ChronicDiseases = "Периферическая дистрофия сетчатки" },
                new { CardNumber = "P0052", OmsNumber = "5223456789012345", LastName = "Овчинников", FirstName = "Матвей", Patronymic = "Аркадьевич", BirthDate = new DateTime(1994, 8, 12), Gender = "M", Phone = "+7 949 901 23 01", Address = "г. Рязань, ул. Почтовая, д.6", Email = "ovchinnikov@example.com", Allergies = "Нет", ChronicDiseases = "Ретинобластома (в ремиссии)" },
                new { CardNumber = "P0053", OmsNumber = "5323456789012345", LastName = "Панова", FirstName = "Лидия", Patronymic = "Вениаминовна", BirthDate = new DateTime(1975, 12, 25), Gender = "F", Phone = "+7 950 012 34 12", Address = "г. Псков, ул. Некрасова, д.3", Email = "panova@example.com", Allergies = "Пыльца амброзии", ChronicDiseases = "Скотома (слепое пятно)" },
                new { CardNumber = "P0054", OmsNumber = "5423456789012345", LastName = "Романов", FirstName = "Феликс", Patronymic = "Эмильевич", BirthDate = new DateTime(1982, 4, 8), Gender = "M", Phone = "+7 951 123 45 23", Address = "г. Ставрополь, ул. Дзержинского, д.8", Email = "romanov@example.com", Allergies = "Нет", ChronicDiseases = "Трахома (в хронической форме)" },
                new { CardNumber = "P0055", OmsNumber = "5523456789012345", LastName = "Савина", FirstName = "Нонна", Patronymic = "Рудольфовна", BirthDate = new DateTime(1969, 6, 17), Gender = "F", Phone = "+7 952 234 56 34", Address = "г. Тамбов, ул. Интернациональная, д.5", Email = "savina@example.com", Allergies = "Новокаин", ChronicDiseases = "Увеальная меланома (после лечения)" },
                new { CardNumber = "P0056", OmsNumber = "5623456789012345", LastName = "Тимофеев", FirstName = "Иннокентий", Patronymic = "Платонович", BirthDate = new DateTime(1971, 10, 30), Gender = "M", Phone = "+7 953 345 67 45", Address = "г. Кострома, ул. Симановского, д.2", Email = "timofeev@example.com", Allergies = "Нет", ChronicDiseases = "Фотокератит хронический" },
                new { CardNumber = "P0057", OmsNumber = "5723456789012345", LastName = "Уварова", FirstName = "Клавдия", Patronymic = "Степановна", BirthDate = new DateTime(1958, 2, 14), Gender = "F", Phone = "+7 954 456 78 56", Address = "г. Курган, ул. Гоголя, д.7", Email = "uvarova@example.com", Allergies = "Рыба", ChronicDiseases = "Хориоидальная неоваскуляризация" },
                new { CardNumber = "P0058", OmsNumber = "5823456789012345", LastName = "Филиппов", FirstName = "Вадим", Patronymic = "Робертович", BirthDate = new DateTime(1988, 9, 21), Gender = "M", Phone = "+7 955 567 89 67", Address = "г. Чита, ул. Бутина, д.4", Email = "filippov@example.com", Allergies = "Нет", ChronicDiseases = "Циклит" },
                new { CardNumber = "P0059", OmsNumber = "5923456789012345", LastName = "Харитонова", FirstName = "Раиса", Patronymic = "Филипповна", BirthDate = new DateTime(1974, 5, 6), Gender = "F", Phone = "+7 956 678 90 78", Address = "г. Якутск, ул. Орджоникидзе, д.9", Email = "kharitonova@example.com", Allergies = "Мед", ChronicDiseases = "Эктропион (выворот века)" },
                new { CardNumber = "P0060", OmsNumber = "6023456789012345", LastName = "Цветков", FirstName = "Глеб", Patronymic = "Всеволодович", BirthDate = new DateTime(1999, 11, 28), Gender = "M", Phone = "+7 957 789 01 89", Address = "г. Петрозаводск, ул. Дзержинского, д.6", Email = "tsvetkov@example.com", Allergies = "Нет", ChronicDiseases = "Энтропион (заворот века)" }
            };


            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Patients
                    WHERE CardNumber = @CardNumber
                )
                INSERT INTO Patients
                    (CardNumber, OmsNumber, LastName, FirstName, Patronymic, BirthDate, Gender, Phone, Address, Email, Allergies, ChronicDiseases)
                VALUES
                    (@CardNumber, @OmsNumber, @LastName, @FirstName, @Patronymic, @BirthDate, @Gender, @Phone, @Address, @Email, @Allergies, @ChronicDiseases);
                """,
                conn);

            foreach (var patient in patients)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 10).Value = patient.CardNumber;
                cmd.Parameters.Add("@OmsNumber", SqlDbType.NVarChar, 16).Value = (object?)patient.OmsNumber ?? DBNull.Value;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = patient.LastName;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = patient.FirstName;
                cmd.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50).Value = (object?)patient.Patronymic ?? DBNull.Value;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = patient.BirthDate;
                cmd.Parameters.Add("@Gender", SqlDbType.NChar, 1).Value = patient.Gender;
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)patient.Phone ?? DBNull.Value;
                cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 50).Value = (object?)patient.Address ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = (object?)patient.Email ?? DBNull.Value;
                cmd.Parameters.Add("@Allergies", SqlDbType.NVarChar, 500).Value = (object?)patient.Allergies ?? DBNull.Value;
                cmd.Parameters.Add("@ChronicDiseases", SqlDbType.NVarChar, 500).Value = (object?)patient.ChronicDiseases ?? DBNull.Value;

                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Пациенты успешно добавлены в базу.");
        }
    }
}

namespace Api
{
    public class PotentialSqlInjectionException : Exception
    {
        public PotentialSqlInjectionException(string tableName)
            : base($"Обнаружена попытка недопустимого имени таблицы: {tableName}")
        { }
    }
}

namespace NeraXTools.Database.PostgreSql
{
    public class PostgreSqlDTO
    {
        public string QueryOrProcedureName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public Type ReturnType { get; set; } = typeof(object);
        public bool IsStoredProcedure { get; set; } = false; // تشخیص نوع فراخوانی
        public string SpecificConnectionString { get; set; } = null;
        public CancellationToken ct { get; set; } = CancellationToken.None;
        public int TimeoutFromSec { get; set; } = -1; // -1 برای حالت داینامیک
    }

    public class ResultPostgreSqlDTO<T>
    {
        public string Identifier { get; set; } // نام کوئری یا فانکشن
        public T Result { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
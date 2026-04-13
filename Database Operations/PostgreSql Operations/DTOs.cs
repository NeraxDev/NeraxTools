namespace NeraXTools.Database.PostgreSql
{
    public class PostgreSqlDTO
    {
        public string QueryOrProcedureName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public Type ReturnType { get; set; } = typeof(object);

        /// <summary>
        /// Specifies how the command should be executed: as a raw SQL query or as a stored procedure call.
        /// Default is Query.
        ///
        /// Example:
        /// Query:
        /// SELECT * FROM Users WHERE Id = @param1 AND Name = @param2
        ///
        /// Procedure:
        /// CALL GetUserById(@param1, @param2, ...)
        /// </summary>
        internal QueryType QueryType { get; set; } = QueryType.Query;

        public string ConnectionString { get; set; } = null;
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
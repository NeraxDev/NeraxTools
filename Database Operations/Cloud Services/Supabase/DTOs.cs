using Supabase;
using System.Reflection.Metadata.Ecma335;

namespace NeraXTools.Database_Operations.Cloud_Services.SupabaseOP
{
    public class SupabaseDTO
    {
        internal string FunctionName { get; set; }
        internal Dictionary<string, object> Parameters { get; set; } = new();
        internal Type ReturnType { get; set; } // برای اینکه بدانیم خروجی هر کدام چیست
        internal Supabase.Client SpecificClient { get; set; } = null;// اختیاری: اگر ست نشود از کلاینت پیش‌فرض استفاده می‌شود
        internal CancellationToken ct { get; set; } = CancellationToken.None; // برای عملیات‌های طولانی‌مدت یا حساس به زمان
        internal int TimeOutFormSec { get; set; } = Timeout.Infinite; // برای تنظیم تایم‌اوت در صورت نیاز
    }

    public class ResultSupabaseDTO<T>
    {
        public string FunctionName { get; set; }
        public T Result { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = null;
    }
}
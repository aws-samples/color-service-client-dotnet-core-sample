using System;

namespace WebUIApp.Utility
{
    public static class Extensions
    {
        public static Exception Innermost(this Exception ex)
        {
            if (ex == null)
                throw new NullReferenceException(nameof(ex));

            while (ex.InnerException != null)
                ex = ex.InnerException;

            return ex;
        }
    }
}

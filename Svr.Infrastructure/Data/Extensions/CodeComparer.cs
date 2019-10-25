using System.Collections.Generic;

namespace Svr.Infrastructure.Data.Extensions
{
    public class CodeComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            var a = x?.ToString().Split('.');
            var b = y?.ToString().Split('.');
            for (var i = 0; (i < a?.Length) && (i < b?.Length); i++)
            {
                if (long.TryParse(a[i], out var c) && long.TryParse(b[i], out var d))
                {
                    if (c > d)
                        return 1;
                    else if (c < d)
                        return -1;
                }
                else
                {
                    if (long.TryParse(a[i], out c))
                        return 1;
                    else
                        return -1;
                }
            }
            if (a?.Length > b?.Length)
            {
                return 1;
            }
            if (a?.Length < b?.Length)
            {
                return -1;
            }
            return 0;
        }
    }
}

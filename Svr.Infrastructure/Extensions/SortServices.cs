using Svr.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Svr.Infrastructure.Extensions
{
    public static class SortExtensions
    {
        public static IEnumerable<FileInfo> Sort(this IEnumerable<FileInfo> list, SortState sortOrder = SortState.NameAsc)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return list.OrderByDescending(p => p.Name);
                case SortState.CodeAsc:
                    return list.OrderBy(p => p.Extension);
                case SortState.CodeDesc:
                    return list.OrderByDescending(p => p.Extension);
                case SortState.CreatedOnUtcAsc:
                    return list.OrderBy(p => p.CreationTime);
                case SortState.CreatedOnUtcDesc:
                    return list.OrderByDescending(p => p.CreationTime);
                case SortState.UpdatedOnUtcAsc:
                    return list.OrderBy(p => p.LastWriteTime);
                case SortState.UpdatedOnUtcDesc:
                    return list.OrderByDescending(p => p.LastWriteTime);
                case SortState.NameAsc:
                    return list.OrderBy(s => s.Name);
                case SortState.DescriptionAsc:
                    return list;
                case SortState.DescriptionDesc:
                    return list;
                case SortState.LordAsc:
                    return list;
                case SortState.LordDesc:
                    return list;
                case SortState.OwnerAsc:
                    return list;
                case SortState.OwnerDesc:
                    return list;
                default:
                    return list.OrderBy(s => s.Name);
            }
        }
    }
}

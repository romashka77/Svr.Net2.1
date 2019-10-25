
namespace Svr.Core.Entities
{
    /// <summary>
    /// поле сортировки
    /// </summary>
    public enum SortState
    {
        NameAsc,    // по имени по возрастанию
        NameDesc,   // по имени по убыванию
        SumAsc,    // по сумме иска по возрастанию
        SumDesc,
        CodeAsc,    // по коду по возрастанию
        CodeDesc,    // по коду по убыванию
        CodeSubjectClaimAsc,
        CodeSubjectClaimDesc,
        DescriptionAsc,    // по возрастанию
        DescriptionDesc,    // по убыванию
        CreatedOnUtcAsc,
        CreatedOnUtcDesc,
        UpdatedOnUtcAsc,
        UpdatedOnUtcDesc,
        LordAsc,    // по владельцу по возрастанию
        LordDesc,    // по владельцу по убыванию
        OwnerAsc,    // по владельцу по возрастанию
        OwnerDesc    // по владельцу по убыванию

    }
}

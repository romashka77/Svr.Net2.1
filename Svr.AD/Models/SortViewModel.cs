using Svr.Core.Entities;

namespace Svr.AD.Models
{
    public class SortViewModel
    {
        public SortState NameSort { get; private set; } // значение для сортировки по имени
        public SortState CodeSort { get; private set; }    // значение для сортировки по коду
        public SortState SumSort { get; private set; }    // значение для сортировки по сумме иска
        public SortState CodeSubjectClaimSort { get; private set; }    // значение для сортировки по коду предмета иска
        public SortState DescriptionSort { get; private set; }    // значение для сортировки по описанию
        public SortState CreatedOnUtcSort { get; private set; }    // значение для сортировки по дате
        public SortState UpdatedOnUtcSort { get; private set; }    // значение для сортировки по дате
        public SortState LordSort { get; private set; }    // значение для сортировки по владельцу
        public SortState OwnerSort { get; private set; }    // значение для сортировки по владельцу
        public SortState Current { get; private set; }     // текущее значение сортировки
        public bool Up { get; private set; }  // Сортировка по возрастанию или убыванию

        public SortViewModel(SortState sortOrder)
        {
            // значения по умолчанию
            NameSort = SortState.NameAsc;
            CodeSort = SortState.CodeAsc;
            SumSort = SortState.SumAsc;
            CodeSubjectClaimSort = SortState.CodeSubjectClaimAsc;
            DescriptionSort = SortState.DescriptionAsc;
            CreatedOnUtcSort = SortState.CreatedOnUtcAsc;
            UpdatedOnUtcSort = SortState.UpdatedOnUtcAsc;
            LordSort = SortState.LordAsc;
            OwnerSort = SortState.OwnerAsc;
            Up = true;

            if (sortOrder == SortState.CodeDesc || sortOrder == SortState.NameDesc || sortOrder == SortState.DescriptionDesc || sortOrder == SortState.CreatedOnUtcDesc || sortOrder == SortState.UpdatedOnUtcDesc || sortOrder == SortState.OwnerDesc || sortOrder == SortState.LordDesc || sortOrder == SortState.CodeSubjectClaimDesc || sortOrder == SortState.SumDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    Current = NameSort = SortState.NameAsc;
                    break;
                case SortState.CodeAsc:
                    Current = CodeSort = SortState.CodeDesc;
                    break;
                case SortState.CodeDesc:
                    Current = CodeSort = SortState.CodeAsc;
                    break;
                case SortState.SumAsc:
                    Current = SumSort = SortState.SumDesc;
                    break;
                case SortState.SumDesc:
                    Current = SumSort = SortState.SumAsc;
                    break;
                case SortState.CodeSubjectClaimAsc:
                    Current = CodeSort = SortState.CodeSubjectClaimDesc;
                    break;
                case SortState.CodeSubjectClaimDesc:
                    Current = CodeSort = SortState.CodeSubjectClaimAsc;
                    break;
                case SortState.DescriptionAsc:
                    Current = DescriptionSort = SortState.DescriptionDesc;
                    break;
                case SortState.DescriptionDesc:
                    Current = DescriptionSort = SortState.DescriptionAsc;
                    break;
                case SortState.CreatedOnUtcAsc:
                    Current = CreatedOnUtcSort = SortState.CreatedOnUtcDesc;
                    break;
                case SortState.CreatedOnUtcDesc:
                    Current = CreatedOnUtcSort = SortState.CreatedOnUtcAsc;
                    break;
                case SortState.UpdatedOnUtcAsc:
                    Current = UpdatedOnUtcSort = SortState.UpdatedOnUtcDesc;
                    break;
                case SortState.UpdatedOnUtcDesc:
                    Current = UpdatedOnUtcSort = SortState.UpdatedOnUtcAsc;
                    break;
                case SortState.OwnerAsc:
                    Current = OwnerSort = SortState.OwnerDesc;
                    break;
                case SortState.OwnerDesc:
                    Current = OwnerSort = SortState.OwnerAsc;
                    break;
                case SortState.LordAsc:
                    Current = LordSort = SortState.LordDesc;
                    break;
                case SortState.LordDesc:
                    Current = LordSort = SortState.LordAsc;
                    break;
                default:
                    Current = NameSort = SortState.NameDesc;
                    break;
            }
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            CodeSort = sortOrder == SortState.CodeAsc ? SortState.CodeDesc : SortState.CodeAsc;
            SumSort = sortOrder == SortState.SumAsc ? SortState.SumDesc : SortState.SumAsc;
            CodeSubjectClaimSort = sortOrder == SortState.CodeSubjectClaimAsc ? SortState.CodeSubjectClaimDesc : SortState.CodeSubjectClaimAsc;
            DescriptionSort = sortOrder == SortState.DescriptionAsc ? SortState.DescriptionDesc : SortState.DescriptionAsc;
            CreatedOnUtcSort = sortOrder == SortState.CreatedOnUtcAsc ? SortState.CreatedOnUtcDesc : SortState.CreatedOnUtcAsc;
            UpdatedOnUtcSort = sortOrder == SortState.UpdatedOnUtcAsc ? SortState.UpdatedOnUtcDesc : SortState.UpdatedOnUtcAsc;
            OwnerSort = sortOrder == SortState.OwnerAsc ? SortState.OwnerDesc : SortState.OwnerAsc;
            LordSort = sortOrder == SortState.LordAsc ? SortState.LordDesc : SortState.LordAsc;
            Current = sortOrder;
        }
    }
}
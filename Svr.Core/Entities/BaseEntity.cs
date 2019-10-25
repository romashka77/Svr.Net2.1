using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// ������� ����� ��� ��������
    /// </summary>
    public abstract class BaseEntity
    {
        private const string Error = "������: ";
        protected const string ErrorStringEmpty = "����������, ��������� ����: {0}";
        protected const string ErrorStringMaxLength = "������������ ����� ����: {0} �� ����� {1} ��������";
        /// <summary>
        /// ���������� ��� ������ ������������� ��������
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// ���� � ����� ��������
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "���� ��������")]
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// ���� � ����� ����������
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "���� ���������")]
        public DateTime UpdatedOnUtc { get; set; }
        //[NotMapped]//����� �� ���������� ������� � �������.
        public override string ToString() => "������� ��������";
        public virtual string MessageErrorFind() => $"{Error}{ToString()} � Id={Id}: �� ������� �����.";
        public virtual string MessageAddOk() => $"��������: .";
        public virtual string MessageAddError() => $"{Error}{ToString()} - ��������� ������� �����������.";
        public virtual string MessageEditOk() => $"��������: {ToString()} � Id={Id}.";
        public virtual string MessageEditError() => $"{Error}{ToString()} � Id={Id}: �� ������� �����.";
        public virtual string MessageEditErrorNoknow() => $"{Error}{ToString()} � Id={Id}: �������������� ������ ��� ����������.";
        public virtual string MessageDeleteOk() => $"������: {ToString()} � Id={Id}.";
        public virtual string MessageDeleteError() => $"{Error}{ToString()} � Id={Id}: �� ������� �������.";
        // ��������� ���������� �������� https://metanit.com/sharp/entityframeworkcore/3.2.php
    }
}

using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Attributes;

namespace BusinessTracker.Domain.Models.Dto
{
    /// <summary>
    /// Записть в журнале в клиенсткой программе.
    /// </summary>
    public class JournalRowDto : IDto
    {
        /// <summary>
        /// Уникальный код транзакции.
        /// </summary>
        [ColumnMapping("journalid")]
        public long Code { get; set; }

        /// <summary>
        /// Уникальный код типа транзакции.
        /// </summary>
        [ColumnMapping("transtype")]
        public int TypeCode { get; set; }

        /// <summary>
        /// Наименование типа транзакции.
        /// </summary>
        public string TransTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Номер чека.
        /// </summary>
        [ColumnMapping("checknum")]
        public int ReceiptNumber { get; set; }

        /// <summary>
        /// Уникальный код продукта.
        /// </summary>
        public long? ProductCode { get; set; }

        /// <summary>
        /// Уникальный код категории продуктов.
        /// </summary>
        public long? CategoryCode { get; set; }

        /// <summary>
        /// Код сотрудника.
        /// </summary>
        public int? EmployeeCode { get; set; }

        /// <summary>
        /// Дата время транзакции.
        /// </summary>
        [ColumnMapping("dater")]
        public DateTime Period { get; set; }

        /// <summary>
        /// Количество.
        /// </summary>
        [ColumnMapping("quantity")]
        public double Quantity { get; set; }

        /// <summary>
        /// Цена.
        /// </summary>
        [ColumnMapping("price")]
        public double Price { get; set; }

        /// <summary>
        /// Сумма скидки.
        /// </summary>
        [ColumnMapping("sumdiscount")]
        public double Discount { get; set; }

        /// <summary>
        /// "Raw" id from journal table for logic
        /// </summary>
        [ColumnMapping("id")]
        public int RawId { get; set; }

        /// <summary>
        /// "Raw" loginid from journal table for logic
        /// </summary>
        [ColumnMapping("loginid")]
        public int RawLoginId { get; set; }

        public override string ToString() =>
            $"{Period}:Транзакция - {Quantity * Price}, Тип: {TransTypeName}, Продукт: {ProductCode}, Сотрудник: {EmployeeCode}";
    }
}
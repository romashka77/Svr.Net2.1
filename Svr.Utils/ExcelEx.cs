using OfficeOpenXml;

using Svr.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Svr.Utils
{
    public class ExcelEx
    {
        private static int GetNumRow(ExcelWorksheet worksheet, string cat = "")
        {
            var res = 0;
            var rage = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
            var enumerable = rage.ToList();
            if (enumerable.Any())
                res = enumerable.Last().End.Row;
            return res;
        }
        public class NameSum
        {
            public string Name { get; set; }
            public decimal Sum { get; set; }
        }
        public class Rec
        {
            //количество
            public int Count { get; set; }
            //сумма
            public decimal Sum { get; set; }
            public List<NameSum> ListNameSum { get; }
            public Rec()
            {
                ListNameSum = new List<NameSum>();
            }
        }
        public class Record
        {
            //удовлетворено
            public Rec Satisfied { get; }
            //отказано
            public Rec Denied { get; }
            public Record()
            {
                Satisfied = new Rec();
                Denied = new Rec();
            }
        }
        public static List<Rec> InitialRec(int count = 11)
        {
            var res = new List<Rec>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Rec());
            return res;
        }
        public static List<Record> InitialRecord(int count = 9)
        {
            var res = new List<Record>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Record());
            return res;
        }
        private static decimal RoundHundred(decimal hundred)
        {
            return Math.Round(hundred / 1000, 1);
        }
        #region CellToInt
        private static int CellToInt(string text, int count = 0)
        {
            return int.TryParse(text, out var t) ? count + t : count;
        }
        #endregion
        #region CellToDec
        private static decimal CellToDec(string text, decimal count = 0)
        {
            return decimal.TryParse(text, out var t) ? count + t : count;
        }
        #endregion
        public static void InitLog(ExcelRange cellslog, int l)
        {
            cellslog[$"B{l}"].Value = $"Иски";
            cellslog[$"C{l}"].Value = $"Сумма иска";
            cellslog[$"D{l}"].Value = $"Инстанция 1";
            cellslog[$"E{l}"].Value = $"Сумма удовлетворено 1";
            cellslog[$"F{l}"].Value = $"Сумма отказано 1";
            cellslog[$"G{l}"].Value = $"Инстанция 2";
            cellslog[$"H{l}"].Value = $"Сумма удовлетворено 2";
            cellslog[$"I{l}"].Value = $"Сумма отказано 2";
            cellslog[$"J{l}"].Value = $"Инстанция 3";
            cellslog[$"K{l}"].Value = $"Сумма удовлетворено 3";
            cellslog[$"L{l}"].Value = $"Сумма отказано 3";
            cellslog[$"M{l}"].Value = $"Инстанция 4";
            cellslog[$"N{l}"].Value = $"Сумма удовлетворено 4";
            cellslog[$"O{l}"].Value = $"Сумма отказано 4";
        }
        public static void SetCells(ExcelWorksheet worksheet, ExcelWorksheet worksheetLog, IReadOnlyList<Record> record, ref int l, string cat = "", byte d = 0)
        {
            if (record[1 + d].Satisfied.Count <= 0 && record[1 + d].Denied.Count <= 0 &&
                record[3 + d].Satisfied.Count <= 0 && record[3 + d].Denied.Count <= 0 &&
                record[5 + d].Satisfied.Count <= 0 && record[5 + d].Denied.Count <= 0 &&
                record[7 + d].Satisfied.Count <= 0 && record[7 + d].Denied.Count <= 0) return;
            var n = GetNumRow(worksheet, cat);
            if (n == 0) return;
            var cells = worksheet.Cells;
            var cellsLog = worksheetLog.Cells;
            cellsLog[$"A{l++}"].Value = cat;
            //cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[1 + d].Satisfied.Count);
            //cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[1 + d].Satisfied.Sum));
            cells[$"H{n}"].Value = CellToDec(cells[$"H{n}"].Text, RoundHundred(record[1 + d].Satisfied.Sum));
            for (int i = 0; i < record[1 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"D{l}"].Value = record[1 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"E{l++}"].Value = record[1 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[1 + d].Denied.Count);
            //cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[1 + d].Denied.Sum));
            cells[$"T{n}"].Value = CellToDec(cells[$"T{n}"].Text, RoundHundred(record[1 + d].Denied.Sum));
            for (int i = 0; i < record[1 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"D{l}"].Value = record[1 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"F{l++}"].Value = record[1 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[3 + d].Satisfied.Count);
            //cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[3 + d].Satisfied.Sum));
            cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[3 + d].Satisfied.Sum));
            for (int i = 0; i < record[3 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"G{l}"].Value = record[3 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"H{l++}"].Value = record[3 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[3 + d].Denied.Count);
            //cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[3 + d].Denied.Sum));
            cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[3 + d].Denied.Sum));
            for (int i = 0; i < record[3 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"G{l}"].Value = record[3 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"I{l++}"].Value = record[3 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[5 + d].Satisfied.Count);
            //cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[5 + d].Satisfied.Sum));
            cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[5 + d].Satisfied.Sum));
            for (int i = 0; i < record[5 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"J{l}"].Value = record[5 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"K{l++}"].Value = record[5 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[5 + d].Denied.Count);
            //cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[5 + d].Denied.Sum));
            cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[5 + d].Denied.Sum));
            for (int i = 0; i < record[5 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"J{l}"].Value = record[5 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"L{l++}"].Value = record[5 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, record[7 + d].Satisfied.Count);
            //cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, RoundHundred(record[7 + d].Satisfied.Sum));
            cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[7 + d].Satisfied.Sum));
            for (int i = 0; i < record[7 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"M{l}"].Value = record[7 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"N{l++}"].Value = record[7 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, record[7 + d].Denied.Count);
            //cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, RoundHundred(record[7 + d].Denied.Sum));
            cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[7 + d].Denied.Sum));
            for (int i = 0; i < record[7 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"M{l}"].Value = record[7 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"O{l++}"].Value = record[7 + d].Denied.ListNameSum[i].Sum;
            }
        }

        #region GetSumInstances
        public static void GetSumInstances(List<Instance> instances, Rec satisfied,
            Rec denied, Rec end, Rec no, Record duty, Record services, Record cost, Record dutyPaid)
        {
            foreach (var item in instances)
            {
                if (item.CourtDecision == null) continue;
                if (item.CourtDecision.Name.ToUpper().Equals("Удовлетворено (частично)".ToUpper()))
                {
                    satisfied.Count++;
                    satisfied.Sum += item?.SumSatisfied ?? 0;
                    denied.Sum += item?.SumDenied ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Отказано".ToUpper()))
                {
                    denied.Count++;
                    denied.Sum += item?.SumDenied ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Прекращено".ToUpper()))
                {
                    end.Count++;
                    end.Sum += item?.Claim?.Sum ?? 0;
                }
                else if (item.CourtDecision.Name.ToUpper().Equals("Оставлено без рассмотрения".ToUpper()))
                {
                    no.Count++;
                    no.Sum += item?.Claim?.Sum ?? 0;
                }

                if (item?.DutySatisfied != null && item.DutySatisfied > 0)
                {
                    duty.Satisfied.Count++;
                    duty.Satisfied.Sum = duty.Satisfied.Sum + (item?.DutySatisfied ?? 0);
                    duty.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutySatisfied ?? 0) });
                }

                if (item?.DutyDenied != null && item?.DutyDenied > 0)
                {
                    duty.Denied.Count++;
                    duty.Denied.Sum = duty.Denied.Sum + (item?.DutyDenied ?? 0);
                    duty.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutyDenied ?? 0) });
                }

                if (item?.ServicesSatisfied != null && item?.ServicesSatisfied > 0)
                {
                    services.Satisfied.Count++;
                    services.Satisfied.Sum = services.Satisfied.Sum + (item?.ServicesSatisfied ?? 0);
                    services.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.ServicesSatisfied ?? 0) });
                }

                if (item?.ServicesDenied != null && item?.ServicesDenied > 0)
                {
                    services.Denied.Count++;
                    services.Denied.Sum = services.Denied.Sum + (item?.ServicesDenied ?? 0);
                    services.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.ServicesDenied ?? 0) });
                }

                if (item?.СostSatisfied != null && item?.СostSatisfied > 0)
                {
                    cost.Satisfied.Count++;
                    cost.Satisfied.Sum = cost.Satisfied.Sum + (item?.СostSatisfied ?? 0);
                    cost.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.СostSatisfied ?? 0) });
                }

                if (item?.СostDenied != null && item?.СostDenied > 0)
                {
                    cost.Denied.Count++;
                    cost.Denied.Sum = cost.Denied.Sum + (item?.СostDenied ?? 0);
                    cost.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.СostDenied ?? 0) });
                }

                //-----------------
                if (item?.DutyPaid != null && item?.DutyPaid > 0)
                {
                    dutyPaid.Satisfied.Count++;
                    dutyPaid.Satisfied.Sum = dutyPaid.Satisfied.Sum + (item?.DutyPaid ?? 0);
                    dutyPaid.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutyPaid ?? 0) });
                }
            }
        }
        #endregion
        public static void SetCells2(ExcelWorksheet worksheet, IReadOnlyList<Rec> record, string cat = "")
        {
            if (string.IsNullOrEmpty(cat) || (record.Sum(rec => rec.Count) == 0)) return;
            var n = GetNumRow(worksheet, cat);
            if (n == 0) return;
            var cells = worksheet.Cells;
            //cells[$"C{n}"].Value = CellToInt(cells[$"C{n}"].Text, record[0].Count);
            //cells[$"D{n}"].Value = CellToDec(cells[$"D{n}"].Text, RoundHundred(record[0].Sum));

            //инст.1
            //удовл.
            cells[$"G{n}"].Value = CellToInt(cells[$"G{n}"].Text, record[1].Count);
            cells[$"H{n}"].Value = CellToDec(cells[$"H{n}"].Text, RoundHundred(record[1].Sum));
            //отказ.
            cells[$"S{n}"].Value = CellToInt(cells[$"S{n}"].Text, record[2].Count);
            cells[$"T{n}"].Value = CellToDec(cells[$"T{n}"].Text, RoundHundred(record[2].Sum));
            //инст.2
            //удовл.
            cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[3].Count);
            cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[3].Sum));
            //отказ.
            cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[4].Count);
            cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[4].Sum));
            //инст.3
            //удовл.
            cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[5].Count);
            cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[5].Sum));
            //отказ.
            cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[6].Count);
            cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[6].Sum));
            //инст.4
            //удовл.
            cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[7].Count);
            cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[7].Sum));
            //отказ.
            cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[8].Count);
            cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[8].Sum));
            //прекращ.
            cells[$"AC{n}"].Value = CellToInt(cells[$"AC{n}"].Text, record[9].Count);
            cells[$"AD{n}"].Value = CellToDec(cells[$"AD{n}"].Text, RoundHundred(record[9].Sum));
            //оставлено
            cells[$"AE{n}"].Value = CellToInt(cells[$"AE{n}"].Text, record[10].Count);
            cells[$"AF{n}"].Value = CellToDec(cells[$"AF{n}"].Text, RoundHundred(record[10].Sum));
            var regex = new Regex(@"(\d+|\.[^.]*)$");//(@"\.[^.]*$");
            SetCells2(worksheet, record, regex.Replace(cat, string.Empty, 1));
        }
    }
}

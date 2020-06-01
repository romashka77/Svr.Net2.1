using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svr.Utils
{
    public class ExcelEx
    {
        //private static int GetNumRow(ExcelWorksheet worksheet, string cat = "")
        //{
        //    var res = 0;
        //    var rage = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
        //    var enumerable = rage.ToList();
        //    if (enumerable.Any())
        //        res = enumerable.Last().End.Row;
        //    return res;
        //}
        //private class NameSum
        //{
        //    public string Name { get; set; }
        //    public decimal Sum { get; set; }
        //}
        //private class Rec
        //{
        //    //количество
        //    public int Count { get; set; }
        //    //сумма
        //    public decimal Sum { get; set; }
        //    public List<NameSum> ListNameSum { get; }
        //    public Rec()
        //    {
        //        ListNameSum = new List<NameSum>();
        //    }
        //}
        //public static void SetCells2(ExcelWorksheet worksheet, IReadOnlyList<Rec> record, string cat = "")
        //{
        //    if (string.IsNullOrEmpty(cat) || (record.Sum(rec => rec.Count) == 0)) return;
        //    var n = GetNumRow(worksheet, cat);
        //    if (n == 0) return;
        //    var cells = worksheet.Cells;
        //    //cells[$"C{n}"].Value = CellToInt(cells[$"C{n}"].Text, record[0].Count);
        //    //cells[$"D{n}"].Value = CellToDec(cells[$"D{n}"].Text, RoundHundred(record[0].Sum));

        //    //инст.1
        //    //удовл.
        //    cells[$"G{n}"].Value = CellToInt(cells[$"G{n}"].Text, record[1].Count);
        //    cells[$"H{n}"].Value = CellToDec(cells[$"H{n}"].Text, RoundHundred(record[1].Sum));
        //    //отказ.
        //    cells[$"S{n}"].Value = CellToInt(cells[$"S{n}"].Text, record[2].Count);
        //    cells[$"T{n}"].Value = CellToDec(cells[$"T{n}"].Text, RoundHundred(record[2].Sum));
        //    //инст.2
        //    //удовл.
        //    cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[3].Count);
        //    cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[3].Sum));
        //    //отказ.
        //    cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[4].Count);
        //    cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[4].Sum));
        //    //инст.3
        //    //удовл.
        //    cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[5].Count);
        //    cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[5].Sum));
        //    //отказ.
        //    cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[6].Count);
        //    cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[6].Sum));
        //    //инст.4
        //    //удовл.
        //    cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[7].Count);
        //    cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[7].Sum));
        //    //отказ.
        //    cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[8].Count);
        //    cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[8].Sum));
        //    //прекращ.
        //    cells[$"AC{n}"].Value = CellToInt(cells[$"AC{n}"].Text, record[9].Count);
        //    cells[$"AD{n}"].Value = CellToDec(cells[$"AD{n}"].Text, RoundHundred(record[9].Sum));
        //    //оставлено
        //    cells[$"AE{n}"].Value = CellToInt(cells[$"AE{n}"].Text, record[10].Count);
        //    cells[$"AF{n}"].Value = CellToDec(cells[$"AF{n}"].Text, RoundHundred(record[10].Sum));
        //    var regex = new Regex(@"(\d+|\.[^.]*)$");//(@"\.[^.]*$");
        //    SetCells2(worksheet, record, regex.Replace(cat, string.Empty, 1));
        //}
    }
}

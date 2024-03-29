﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Svr.Utils.Controllers
{
    //https://zennolab.com/discussion/threads/generacija-krasivyx-excel-otchjotov-po-shablonu.33585/

    //https://habr.com/ru/post/109820/
    //http://www.pvsm.ru/programmirovanie/49187#begin

    //https://riptutorial.com/ru/epplus/example/26411/text-alignment-and-word-wrap
    //https://ru.inettools.net/image/opredelit-tsvet-piksela-na-kartinke-onlayn
    //https://stackoverflow.com/questions/3604562/download-file-of-any-type-in-asp-net-mvc-using-fileresult
    public class MessageReportController : MessageController
    {
        public readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        public readonly IRegionRepository regionRepository;
        public readonly IDistrictRepository districtRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly IClaimRepository claimRepository;
        private readonly IInstanceRepository instanceRepository;
        public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ReportsFolder = "Reports";
        private const string TemplatesFolder = "Templates";
        private const string FileTemplateNameOut = "0901.xlsx";
        private const string FileTemplateNameIn = "0902.xlsx";

        public FileInfo Template { get; set; }

        #region Конструктор
        public MessageReportController(ICategoryDisputeRepository categoryDisputeRepository, IHostingEnvironment hostingEnvironment, IRegionRepository regionRepository, IDistrictRepository districtRepository, IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository, IInstanceRepository instanceRepository)
        {
            this.categoryDisputeRepository = categoryDisputeRepository ?? throw new ArgumentNullException(nameof(categoryDisputeRepository));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            this.regionRepository = regionRepository ?? throw new ArgumentNullException(nameof(regionRepository));
            this.districtRepository = districtRepository ?? throw new ArgumentNullException(nameof(districtRepository));
            this.groupClaimRepository = groupClaimRepository ?? throw new ArgumentNullException(nameof(groupClaimRepository));
            this.claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
            this.instanceRepository = instanceRepository ?? throw new ArgumentNullException(nameof(instanceRepository));
        }
        #endregion
        #region GetFileTemplateName
        private async Task<FileInfo> GetFileTemplateName(string category)
        {
            string fileTemplateName;
            if (category.ToLong() == null)
            {
                StatusMessage = "Ошибка: Выберите категорию.";
                return null;
            }

            if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Входящие".ToUpper()))
                fileTemplateName = FileTemplateNameIn;
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Исходящие".ToUpper()))
                fileTemplateName = FileTemplateNameOut;
            else
            {
                StatusMessage = "Ошибка: Категория не определена.";
                return null;
            }
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, TemplatesFolder, fileTemplateName));
            if (fileInfo.Exists) return fileInfo;
            StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
            return null;
        }
        #endregion
        #region Excel
        private int GetNumRow(ExcelWorksheet worksheet, string cat = "")
        {
            var res = 0;
            var rage = from cell in worksheet.Cells["A:A"] where cell.Text.Equals(cat) select cell;
            var enumerable = rage.ToList();
            if (enumerable.Any())
                res = enumerable.Last().End.Row;
            return res;
        }
        private class NameSum
        {
            public string Name { get; set; }
            public decimal Sum { get; set; }
        }
        private class Rec
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
        private class Record
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
        private List<Rec> InitialRec(int count = 11)
        {
            var res = new List<Rec>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Rec());
            return res;
        }
        private List<Record> InitialRecord(int count = 9)
        {
            var res = new List<Record>(count);
            for (var i = 0; i < count; i++)
                res.Add(new Record());
            return res;
        }
        private decimal RoundHundred(decimal hundred)
        {
            return Math.Round(hundred / 1000, 1);
        }
        #region CellToInt
        private int CellToInt(string text, int count = 0)
        {
            return int.TryParse(text, out var t) ? count + t : count;
        }
        #endregion
        #region CellToDec
        private decimal CellToDec(string text, decimal count = 0)
        {
            return decimal.TryParse(text, out var t) ? count + t : count;
        }
        #endregion
        private void InitLog(ExcelRange cellslog, int l)
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
        private void SetCells(ExcelWorksheet worksheet, ExcelWorksheet worksheetLog, IReadOnlyList<Record> record, ref int l, string cat = "", byte d = 0)
        {
            //if (record[1 + d].Satisfied.Count <= 0 && record[1 + d].Denied.Count <= 0 &&
            //    record[3 + d].Satisfied.Count <= 0 && record[3 + d].Denied.Count <= 0 &&
            //    record[5 + d].Satisfied.Count <= 0 && record[5 + d].Denied.Count <= 0 &&
            //    record[7 + d].Satisfied.Count <= 0 && record[7 + d].Denied.Count <= 0) return;
            var n = GetNumRow(worksheet, cat);
            if (n == 0) return;
            var cells = worksheet.Cells;
            var cellsLog = worksheetLog.Cells;
            cellsLog[$"A{l++}"].Value = cat;
            //cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[1 + d].Satisfied.Count);
            //cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[1 + d].Satisfied.Sum));
            cells[$"G{n}"].Value = CellToInt(cells[$"G{n}"].Text, record[1 + d].Satisfied.Count);
            cells[$"H{n}"].Value = CellToDec(cells[$"H{n}"].Text, RoundHundred(record[1 + d].Satisfied.Sum));
            for (int i = 0; i < record[1 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"D{l}"].Value = record[1 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"E{l++}"].Value = record[1 + d].Satisfied.ListNameSum[i].Sum;

            }
            //cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[1 + d].Denied.Count);
            //cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[1 + d].Denied.Sum));
            cells[$"S{n}"].Value = CellToInt(cells[$"S{n}"].Text, record[1 + d].Denied.Count);
            cells[$"T{n}"].Value = CellToDec(cells[$"T{n}"].Text, RoundHundred(record[1 + d].Denied.Sum));
            for (int i = 0; i < record[1 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"D{l}"].Value = record[1 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"F{l++}"].Value = record[1 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[3 + d].Satisfied.Count);
            //cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[3 + d].Satisfied.Sum));
            cells[$"I{n}"].Value = CellToInt(cells[$"I{n}"].Text, record[3 + d].Satisfied.Count);
            cells[$"J{n}"].Value = CellToDec(cells[$"J{n}"].Text, RoundHundred(record[3 + d].Satisfied.Sum));
            for (int i = 0; i < record[3 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"G{l}"].Value = record[3 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"H{l++}"].Value = record[3 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[3 + d].Denied.Count);
            //cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[3 + d].Denied.Sum));
            cells[$"U{n}"].Value = CellToInt(cells[$"U{n}"].Text, record[3 + d].Denied.Count);
            cells[$"V{n}"].Value = CellToDec(cells[$"V{n}"].Text, RoundHundred(record[3 + d].Denied.Sum));
            for (int i = 0; i < record[3 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"G{l}"].Value = record[3 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"I{l++}"].Value = record[3 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[5 + d].Satisfied.Count);
            //cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[5 + d].Satisfied.Sum));
            cells[$"K{n}"].Value = CellToInt(cells[$"K{n}"].Text, record[5 + d].Satisfied.Count);
            cells[$"L{n}"].Value = CellToDec(cells[$"L{n}"].Text, RoundHundred(record[5 + d].Satisfied.Sum));
            for (int i = 0; i < record[5 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"J{l}"].Value = record[5 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"K{l++}"].Value = record[5 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[5 + d].Denied.Count);
            //cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[5 + d].Denied.Sum));
            cells[$"W{n}"].Value = CellToInt(cells[$"W{n}"].Text, record[5 + d].Denied.Count);
            cells[$"X{n}"].Value = CellToDec(cells[$"X{n}"].Text, RoundHundred(record[5 + d].Denied.Sum));
            for (int i = 0; i < record[5 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"J{l}"].Value = record[5 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"L{l++}"].Value = record[5 + d].Denied.ListNameSum[i].Sum;
            }
            //cells[$"O{n}"].Value = CellToInt(cells[$"O{n}"].Text, record[7 + d].Satisfied.Count);
            //cells[$"P{n}"].Value = CellToDec(cells[$"P{n}"].Text, RoundHundred(record[7 + d].Satisfied.Sum));
            cells[$"M{n}"].Value = CellToInt(cells[$"M{n}"].Text, record[7 + d].Satisfied.Count);
            cells[$"N{n}"].Value = CellToDec(cells[$"N{n}"].Text, RoundHundred(record[7 + d].Satisfied.Sum));
            for (int i = 0; i < record[7 + d].Satisfied.ListNameSum.Count; i++)
            {
                cellsLog[$"M{l}"].Value = record[7 + d].Satisfied.ListNameSum[i].Name;
                cellsLog[$"N{l++}"].Value = record[7 + d].Satisfied.ListNameSum[i].Sum;
            }
            //cells[$"AA{n}"].Value = CellToInt(cells[$"AA{n}"].Text, record[7 + d].Denied.Count);
            //cells[$"AB{n}"].Value = CellToDec(cells[$"AB{n}"].Text, RoundHundred(record[7 + d].Denied.Sum));
            cells[$"Y{n}"].Value = CellToInt(cells[$"Y{n}"].Text, record[7 + d].Denied.Count);
            cells[$"Z{n}"].Value = CellToDec(cells[$"Z{n}"].Text, RoundHundred(record[7 + d].Denied.Sum));
            for (int i = 0; i < record[7 + d].Denied.ListNameSum.Count; i++)
            {
                cellsLog[$"M{l}"].Value = record[7 + d].Denied.ListNameSum[i].Name;
                cellsLog[$"O{l++}"].Value = record[7 + d].Denied.ListNameSum[i].Sum;
            }
        }

        #region GetSumInstances
        private void GetSumInstances(List<Instance> instances, Rec satisfied,
            Rec denied, Rec end, Rec no, Record duty, Record services, Record cost, Record dutyPaid)
        {
            foreach (var item in instances)
            {
                if (item.CourtDecision == null) continue;//item?.Claim?.ca
                if (item.CourtDecision.Name.ToUpper().Equals("Удовлетворено (частично)".ToUpper()) 
                    && (((Template.Name.Equals(FileTemplateNameIn) && ((item?.Claim?.GroupClaim?.Code?.ToLong() ?? 0) < 25))
                    ||((Template.Name.Equals(FileTemplateNameOut) && ((item?.Claim?.GroupClaim?.Code?.ToLong() ?? 0) < 18))))))
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
                bool flgCount = true;
                // Услуги пред.удов.
                if (item?.ServicesSatisfied != null && item?.ServicesSatisfied > 0)
                {
                    if (flgCount)
                        services.Satisfied.Count++;
                    flgCount = false;
                    services.Satisfied.Sum = services.Satisfied.Sum + (item?.ServicesSatisfied ?? 0);
                    services.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.ServicesSatisfied ?? 0) });
                }
                // Гос.пошлина удов.
                if (item?.DutySatisfied != null && item.DutySatisfied > 0)
                {
                    if (flgCount)
                        duty.Satisfied.Count++;
                    flgCount = false;
                    duty.Satisfied.Sum = duty.Satisfied.Sum + (item?.DutySatisfied ?? 0);
                    duty.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutySatisfied ?? 0) });
                }
                // Суд.издер.удов.
                if (item?.СostSatisfied != null && item?.СostSatisfied > 0)
                {
                    if (flgCount)
                        cost.Satisfied.Count++;
                    flgCount = false;
                    cost.Satisfied.Sum = cost.Satisfied.Sum + (item?.СostSatisfied ?? 0);
                    cost.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.СostSatisfied ?? 0) });
                }


                flgCount = true;
                // Услуги пред.отк.
                if (item?.ServicesDenied != null && item?.ServicesDenied > 0)
                {
                    if (flgCount)
                        services.Denied.Count++;
                    flgCount = false;
                    services.Denied.Sum = services.Denied.Sum + (item?.ServicesDenied ?? 0);
                    services.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.ServicesDenied ?? 0) });
                }
                // Гос.пошлина отк.
                if (item?.DutyDenied != null && item?.DutyDenied > 0)
                {
                    if (flgCount)
                        duty.Denied.Count++;
                    flgCount = false;
                    duty.Denied.Sum = duty.Denied.Sum + (item?.DutyDenied ?? 0);
                    duty.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutyDenied ?? 0) });
                }
                //Суд.издер.отк.
                if (item?.СostDenied != null && item?.СostDenied > 0)
                {
                    if (flgCount)
                        cost.Denied.Count++;
                    flgCount = false;
                    cost.Denied.Sum = cost.Denied.Sum + (item?.СostDenied ?? 0);
                    cost.Denied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.СostDenied ?? 0) });
                }

                //-----------------
                flgCount = true;
                if (item?.DutyPaid != null && item?.DutyPaid > 0)
                {
                    if (flgCount)
                        dutyPaid.Satisfied.Count++;
                    flgCount = false;
                    dutyPaid.Satisfied.Sum = dutyPaid.Satisfied.Sum + (item?.DutyPaid ?? 0);
                    dutyPaid.Satisfied.ListNameSum.Add(new NameSum { Name = $"{ item.Name} {item.Claim.Code}", Sum = (item?.DutyPaid ?? 0) });
                }
            }
        }
        #endregion
        private void SetCells2(ExcelWorksheet worksheet, IReadOnlyList<Rec> record, string cat = "")
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
        #endregion
        public async Task<ExcelPackage> CreateExcelPackage(string userName, string owner = null,
            DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            const int MAX0901 = 18;
            const int MAX0902 = 25;

            const int I0901 = 4;
            const int I0902 = 6;

            Template = await GetFileTemplateName(category);
            if (Template == null) return null;
            var package = new ExcelPackage(Template, true);
            package.Workbook.Properties.Author = userName;
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            var worksheetLog = package.Workbook.Worksheets.Add("log");
            //Группы споров
            var groupClaims =
                    (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong())))
                    .OrderBy(a => a.Code.ToLong());
            //Гос.пошлина
            var duty = InitialRecord();
            //Услуги пред.
            var services = InitialRecord();
            //Суд.издержки
            var cost = InitialRecord();

            var dutyPaid = InitialRecord();

            var cellslog = worksheetLog.Cells;
            int l = 1;
            InitLog(cellslog, l);

            worksheetLog.Column(l + 1).Width = 50;

            for (int i = 0; i < 12; i++)
            {
                if ((i % 3) == 0)
                    worksheetLog.Column(i + l + 3).Width = 50;
            }
            l++;
            foreach (var groupClaim in groupClaims)
            {
                if ((groupClaim.Id == 83) || (groupClaim.Id == 52))
                    continue;

                //cellslog[$"A{l}"].Value = $"{ groupClaim.Code}";
                //cellslog[$"B{l}"].Value = $"{groupClaim.Name}";
                //l++;
                byte flg = 0;
                long? groupClaimCode = groupClaim.Code.ToLong();
                if ((Template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > 0) && (groupClaimCode < I0902)) || (Template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > 0) && (groupClaimCode < I0901)))
                {
                    flg = 1;
                }
                else if ((Template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > (I0902 - 1)) && (groupClaimCode <= MAX0902)) || (Template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > (I0901 - 1)) && (groupClaimCode <= MAX0901)))
                {
                    flg = 2;
                }

                /// Предметы иска
                var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code);
                foreach (var subjectClaim in subjectClaims)
                {
                    try
                    {
                        cellslog[$"A{l}"].Value = $"{ subjectClaim.Code}";
                        cellslog[$"B{l++}"].Value = $"{subjectClaim.Name}";
                        var groupRecord = InitialRec();
                        var claims = claimRepository.List(new ClaimSpecificationReport(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                        var nn = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell);
                        if (nn.Count() == 0) continue;
                        var n = nn?.Last().End.Row;
                        if (n == null) continue;

                        if (dateS != null)
                        {
                            claims = claims.Where(c => c.DateIn >= dateS);
                        }
                        if (datePo != null)
                        {
                            claims = claims.Where(c => c.DateIn <= datePo);
                        }
                        var count = await claims.CountAsync();
                        if (count > 0)
                        {
                            foreach (var claim in claims)
                            {
                                cellslog[$"B{l}"].Value = $"{ claim.Code}";//{ claim.Name}
                                cellslog[$"C{l++}"].Value = $"{ claim.Sum}";
                            }
                            groupRecord[0].Count += count;
                            var sum = await claims.SumAsync(c => c.Sum);
                            if (sum != null)
                            {
                                groupRecord[0].Sum += (sum ?? 0);
                            }
                        }
                        var instances = instanceRepository.ListReport().Where(i => i.Claim.SubjectClaimId == subjectClaim.Id);
                        if (!String.IsNullOrWhiteSpace(owner))
                        {
                            instances = instances.Where(i => i.Claim.DistrictId == owner.ToLong());
                        }
                        //instances = instances.Where(i => i.ClaimId == itemp);
                        if (dateS != null)
                        {
                            instances = instances.Where(c => c.DateInCourtDecision >= dateS);
                        }
                        if (datePo != null)
                        {
                            instances = instances.Where(c => c.DateInCourtDecision <= datePo);
                        }
                        //-------------------------------
                        var instancesMax = instances.GroupBy(p => p.ClaimId, p => p.Number, (key, g) => new { ClaimsId = key, MaxNumber = g.Max() });
                        instances = instances.Join(instancesMax, p => p.ClaimId, t => t.ClaimsId, (p, t) => new
                        {
                            p.Claim,
                            p.ClaimId,
                            p.CourtDecision,
                            p.CourtDecisionId,
                            p.CreatedOnUtc,
                            p.DateCourtDecision,
                            p.DateInCourtDecision,
                            p.DateTransfer,
                            p.Description,
                            p.DutyDenied,
                            p.DutyPaid,
                            p.DutySatisfied,
                            p.Id,
                            p.Name,
                            p.Number,
                            p.ServicesDenied,
                            p.ServicesSatisfied,
                            p.SumDenied,
                            p.SumSatisfied,
                            p.UpdatedOnUtc,
                            p.СostDenied,
                            p.СostSatisfied,
                            MaxNumber = t.MaxNumber
                        }).Where(r => r.Number == r.MaxNumber).Select(p => new Instance
                        {
                            Claim = p.Claim,
                            ClaimId = p.ClaimId,
                            CourtDecision = p.CourtDecision,
                            CourtDecisionId = p.CourtDecisionId,
                            CreatedOnUtc = p.CreatedOnUtc,
                            DateCourtDecision = p.DateCourtDecision,
                            DateInCourtDecision = p.DateInCourtDecision,
                            DateTransfer = p.DateTransfer,
                            Description = p.Description,
                            DutyDenied = p.DutyDenied,
                            DutyPaid = p.DutyPaid,
                            DutySatisfied = p.DutySatisfied,
                            Id = p.Id,
                            Name = p.Name,
                            Number = p.Number,
                            ServicesDenied = p.ServicesDenied,
                            ServicesSatisfied = p.ServicesSatisfied,
                            SumDenied = p.SumDenied,
                            SumSatisfied = p.SumSatisfied,
                            UpdatedOnUtc = p.UpdatedOnUtc,
                            СostDenied = p.СostDenied,
                            СostSatisfied = p.СostSatisfied
                        });
                        //-------------------------------
                        var instances1 = await instances.Where(i => i.Number == 1).AsNoTracking().ToListAsync();
                        if (instances1.Count > 0)
                        {
                            foreach (var instance in instances1)
                            {
                                cellslog[$"D{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"E{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"F{l}"].Value = $"{ instance.SumDenied}";
                                if (instance.CourtDecision != null)
                                    cellslog[$"G{l}"].Value = $"{instance.CourtDecision.Name}";
                                l++;
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 1;
                            else if (flg == 2)
                                k = 2;
                            GetSumInstances(instances1, groupRecord[1], groupRecord[2], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances2 = await instances.Where(i => i.Number == 2).AsNoTracking().ToListAsync();
                        if (instances2.Count > 0)
                        {
                            foreach (var instance in instances2)
                            {
                                cellslog[$"G{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"H{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"I{l}"].Value = $"{ instance.SumDenied}";
                                if (instance.CourtDecision != null)
                                    cellslog[$"J{l}"].Value = $"{instance.CourtDecision.Name}";
                                l++;
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 3;
                            else if (flg == 2)
                                k = 4;
                            GetSumInstances(instances2, groupRecord[3], groupRecord[4], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances3 = await instances.Where(i => i.Number == 3).AsNoTracking().ToListAsync();
                        if (instances3.Count > 0)
                        {
                            foreach (var instance in instances3)
                            {
                                cellslog[$"J{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"K{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"L{l}"].Value = $"{ instance.SumDenied}";
                                if (instance.CourtDecision != null)
                                    cellslog[$"M{l}"].Value = $"{instance.CourtDecision.Name}";
                                l++;
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 5;
                            else if (flg == 2)
                                k = 6;
                            GetSumInstances(instances3, groupRecord[5], groupRecord[6], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances4 = await instances.Where(i => i.Number == 4).AsNoTracking().ToListAsync();
                        if (instances4.Count > 0)
                        {
                            foreach (var instance in instances4)
                            {
                                cellslog[$"M{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"N{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"O{l++}"].Value = $"{ instance.SumDenied}";
                                if (instance.CourtDecision != null)
                                    cellslog[$"P{l}"].Value = $"{instance.CourtDecision.Name}";
                                l++;
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 7;
                            else if (flg == 2)
                                k = 8;
                            GetSumInstances(instances4, groupRecord[7], groupRecord[8], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        SetCells2(worksheet, groupRecord, subjectClaim.Code);
                        //groupRecord = null;
                    }
                    catch (Exception e)
                    {
                        //logger.LogError($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        //Console.WriteLine($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        throw new Exception($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                    }
                }
            }
            SetCells(worksheet, worksheetLog, duty, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.1" : $"{MAX0901}.1"));
            SetCells(worksheet, worksheetLog, dutyPaid, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.1" : $"{MAX0901}.1"));
            SetCells(worksheet, worksheetLog, services, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.2" : ""));
            SetCells(worksheet, worksheetLog, cost, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.3" : $"{MAX0901}.2"));


            SetCells(worksheet, worksheetLog, duty, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.4" : $"{MAX0901}.3"), 1);
            SetCells(worksheet, worksheetLog, dutyPaid, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.4" : $"{MAX0901}.3"), 1);
            SetCells(worksheet, worksheetLog, services, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.5" : ""), 1);
            SetCells(worksheet, worksheetLog, cost, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.6" : $"{MAX0901}.4"), 1);


            SetCells(worksheet, worksheetLog, duty, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));
            SetCells(worksheet, worksheetLog, dutyPaid, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));
            SetCells(worksheet, worksheetLog, services, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : ""));
            SetCells(worksheet, worksheetLog, cost, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));

            SetCells(worksheet, worksheetLog, duty, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);
            SetCells(worksheet, worksheetLog, dutyPaid, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);
            SetCells(worksheet, worksheetLog, services, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : ""), 1);
            SetCells(worksheet, worksheetLog, cost, ref l, (Template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);

            return package;
        }

        public string GetFileName(DateTime? dateS, DateTime? datePo)
        {
            return $"{dateS?.ToString("yyyy.MM.dd")}-{datePo?.ToString("yyyy.MM.dd")} {Template.Name}";
        }

        public async Task<string> GetPath(long? lord, long? owner)
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath, ReportsFolder);
            if (lord != null)
            {
                var region = await regionRepository.GetByIdAsync(lord);
                path = Path.Combine(path, region.Name);
            }
            if (owner != null)
            {
                var district = await districtRepository.GetByIdAsync(owner);
                path = Path.Combine(path, district.Name);
            }
            return path;
        }
    }
}

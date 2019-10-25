using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public static class DataContextSeed
    {
        private static Region _region;
        private static CategoryDispute _categoryDispute;
        private static GroupClaim _groupClaim;
        private static DirName _dirName;
        private static Dir _dir;

        private static string StrTrim(string str, int len = 100)
        {
            if (str.Length < len) { len = str.Length; }
            return str.Substring(0, len);
        }
        public static async Task SeedAsync(DataContext dataContext/*, ILoggerFactory loggerFactory, int? retry = 0*/)
        {
            //int retryForAvailability = retry.Value;
            try
            {
                if (!dataContext.Regions.Any())
                {
                    await dataContext.Regions.AddRangeAsync(GetPreconfiguredRegions());
                    if (!dataContext.Districts.Any())
                    {
                        await dataContext.Districts.AddRangeAsync(GetPreconfiguredDistricts());
                        await dataContext.SaveChangesAsync();
                    }
                }
                if (!dataContext.CategoryDisputes.Any())
                {
                    //Входящие
                    await dataContext.CategoryDisputes.AddRangeAsync(await GetPreconfiguredCategoryDisputeIn(dataContext));
                    await dataContext.SaveChangesAsync();
                    //Исходящие
                    await dataContext.CategoryDisputes.AddRangeAsync(await GetPreconfiguredCategoryDisputeOut(dataContext));
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.DirName.Any())
                {
                    //dataContext.DirName.AddRange(GetPreconfiguredDirNamePerformers(dataContext));
                    //await dataContext.SaveChangesAsync();

                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameOpf(dataContext));
                    await dataContext.SaveChangesAsync();
                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameCourt(dataContext));
                    await dataContext.SaveChangesAsync();
                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameTypeApplicant(dataContext));
                    await dataContext.SaveChangesAsync();

                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameCourtDecision1(dataContext));
                    await dataContext.SaveChangesAsync();
                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameCourtDecision2(dataContext));
                    await dataContext.SaveChangesAsync();
                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameCourtDecision3(dataContext));
                    await dataContext.SaveChangesAsync();
                    await dataContext.DirName.AddRangeAsync(await GetPreconfiguredDirNameCourtDecision4(dataContext));
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.Performers.Any())
                {
                    await dataContext.Performers.AddRangeAsync(GetPreconfiguredPerformers());
                    await dataContext.SaveChangesAsync();
                }
                if (!dataContext.DistrictPerformers.Any())
                {
                    await dataContext.DistrictPerformers.AddAsync(new DistrictPerformer { DistrictId = 1, PerformerId = 1 });
                    await dataContext.DistrictPerformers.AddAsync(new DistrictPerformer { DistrictId = 1, PerformerId = 2 });
                    await dataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при заполнении справочников: {ex.Message}");
                //if (retryForAvailability < 10)
                //{
                //    throw;
                //    retryForAvailability++;
                //    var log = loggerFactory.CreateLogger<DataContextSeed>();
                //    log.LogError(ex.Message);
                //    await SeedAsync(dataContext, loggerFactory, retryForAvailability);
                //}
            }
        }

        private static async Task<IEnumerable<CategoryDispute>> GetPreconfiguredCategoryDisputeOut(DataContext dataContext)
        {
            _categoryDispute = new CategoryDispute { Name = "Исходящие", Description = "Исходящие документы" };
            await dataContext.CategoryDisputes.AddAsync(_categoryDispute);

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)"), Description = "Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)", Code = "1", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(1));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание финансовых санкций за непредставление в установленные сроки необходимых для осуществления индивидуального (персонифицированного) учета в системе обязательного пенсионного страхования сведений либо представление"), Description = "Взыскание финансовых санкций за непредставление в установленные сроки необходимых для осуществления индивидуального (персонифицированного) учета в системе обязательного пенсионного страхования сведений либо представление", Code = "2", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(2));

            _groupClaim = new GroupClaim { Name = StrTrim("Дела о банкротстве (в т.ч. территориальный орган ПФР – заинтересованное лицо)"), Description = "Дела о банкротстве (в т.ч. территориальный орган ПФР – заинтересованное лицо)", Code = "3", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(3));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание излишне выплаченных сумм пенсий, дополнительного материального обеспечения, компенсационных и иных социальных выплат"), Description = "Взыскание излишне выплаченных сумм пенсий, дополнительного материального обеспечения, компенсационных и иных социальных выплат", Code = "4", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(4));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)"), Description = "Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)", Code = "5", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(5));

            _groupClaim = new GroupClaim { Name = StrTrim("Индивидуальные сведения и иная информация"), Description = "Индивидуальные сведения и иная информация", Code = "6", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(6));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание неправомерно полученной пенсии и иных социальных выплат"), Description = "Взыскание неправомерно полученной пенсии и иных социальных выплат", Code = "7", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(7));

            _groupClaim = new GroupClaim { Name = StrTrim("Административные штрафы"), Description = "Административные штрафы", Code = "8", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(8));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание средств пенсионных накоплений в порядке регресса с правопреемников, которым выплачены средства пенсионных накоплений (п. 31 Правил, утвержденных постановлением Правительства Российской Федерации от 30.07.2014 № 711)"), Description = "Взыскание средств пенсионных накоплений в порядке регресса с правопреемников, которым выплачены средства пенсионных накоплений (п. 31 Правил, утвержденных постановлением Правительства Российской Федерации от 30.07.2014 № 711)", Code = "9", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(9));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация ФЗ от 29.12.2006 № 256-ФЗ «О дополнительных мерах государственной поддержки семей, имеющих детей»"), Description = "Реализация ФЗ от 29.12.2006 № 256-ФЗ «О дополнительных мерах государственной поддержки семей, имеющих детей»", Code = "10", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(10));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание незаконно полученных сумм пенсий и (или) иных социальных выплат в рамках рассмотрения уголовного дела"), Description = "Взыскание незаконно полученных сумм пенсий и (или) иных социальных выплат в рамках рассмотрения уголовного дела", Code = "11", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(11));

            _groupClaim = new GroupClaim { Name = StrTrim("О взыскании незаконно полученной компенсации стоимости проезда к месту отдыха и обратно"), Description = "О взыскании незаконно полученной компенсации стоимости проезда к месту отдыха и обратно", Code = "12", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(12));

            _groupClaim = new GroupClaim { Name = StrTrim("О взыскании незаконно полученной компенсации расходов, связанных с переездом из районов Крайнего Севера и приравненных к ним местностей, лицам, являющимся получателями трудовых пенсий и(или) пенсий по государственному пенсионному"), Description = "О взыскании незаконно полученной компенсации расходов, связанных с переездом из районов Крайнего Севера и приравненных к ним местностей, лицам, являющимся получателями трудовых пенсий и(или) пенсий по государственному пенсионному", Code = "13", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(13));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация постановления Правительства Российской Федерации от 16.07.2016 № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»"), Description = "Реализация постановления Правительства Российской Федерации от 16.07.2016 № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»", Code = "14", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(14));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»"), Description = "Реализация постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»", Code = "15", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(15));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»"), Description = "Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»", Code = "16", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(16));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с финансово-хозяйственной деятельностью территориальных органов ПФР, а также споры, вытекающие из договоров (государственных контрактов)"), Description = "Споры, связанные с финансово-хозяйственной деятельностью территориальных органов ПФР, а также споры, вытекающие из договоров (государственных контрактов)", Code = "17", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(17));

            _groupClaim = new GroupClaim { Name = StrTrim("Иные"), Description = "Иные", Code = "18", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(18));

            _groupClaim = new GroupClaim { Name = StrTrim("Иск пенсионера, работника или прокурора к работодателю об обязании уплатить страховые взносы, подать в ПФР сведения ПУ или расчет по начисленным и уплаченным страховым взносам, о взыскании суммы неполученной пенсии в связи с неуплатой"), Description = "Иск пенсионера, работника или прокурора к работодателю об обязании уплатить страховые взносы, подать в ПФР сведения ПУ или расчет по начисленным и уплаченным страховым взносам, о взыскании суммы неполученной пенсии в связи с неуплатой", Code = "19", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(19));

            _groupClaim = new GroupClaim { Name = StrTrim("Судебные расходы"), Description = "Судебные расходы", Code = "20", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsOut(20));

            return new List<CategoryDispute>() { _categoryDispute };
        }

        private static async Task<IEnumerable<CategoryDispute>> GetPreconfiguredCategoryDisputeIn(DataContext dataContext)
        {
            _categoryDispute = new CategoryDispute { Name = "Входящие", Description = "Входящие документы" };
            await dataContext.CategoryDisputes.AddAsync(_categoryDispute);
            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо), из них"), Description = "Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо), из них", Code = "1", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(1));

            _groupClaim = new GroupClaim { Name = StrTrim("Обжалование действий (бездействий), актов, решений территориальных органов ПФР"), Description = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР", Code = "2", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(2));

            _groupClaim = new GroupClaim { Name = StrTrim("Индивидуальные сведения и иная информация (ФЗ от 01.04.1996 № 27-ФЗ)"), Description = "Индивидуальные сведения и иная информация (ФЗ от 01.04.1996 № 27-ФЗ)", Code = "3", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(3));

            _groupClaim = new GroupClaim { Name = StrTrim("Дела о банкротстве"), Description = "Дела о банкротстве", Code = "4", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(4));

            _groupClaim = new GroupClaim { Name = StrTrim("Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках ФЗ  № 27-ФЗ"), Description = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках ФЗ  № 27-ФЗ", Code = "5", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(5));

            _groupClaim = new GroupClaim { Name = StrTrim("Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)"), Description = "Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)", Code = "6", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(6));

            _groupClaim = new GroupClaim { Name = StrTrim("Отношения, возникающие из договоров об обязательном пенсионном страховании (ФЗ от 07.05.1998 № 75-ФЗ)"), Description = "Отношения, возникающие из договоров об обязательном пенсионном страховании (ФЗ от 07.05.1998 № 75-ФЗ)", Code = "7", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(7));

            _groupClaim = new GroupClaim { Name = StrTrim("Компенсация расходов на оплату стоимости проезда пенсионерам, являющимся получателями страховых пенсий по старости и по инвалидности, к месту отдыха и обратно (ст. 34 Закона Российской Федерации № 4520-1)"), Description = "Компенсация расходов на оплату стоимости проезда пенсионерам, являющимся получателями страховых пенсий по старости и по инвалидности, к месту отдыха и обратно (ст. 34 Закона Российской Федерации № 4520-1)", Code = "8", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(8));

            _groupClaim = new GroupClaim { Name = StrTrim("О взыскании расходов, связанных с выездом из районов Крайнего Севера и приравненных к ним местностей на новое место жительства (ст. 35 Закона Российской Федерации № 4520-1)"), Description = "О взыскании расходов, связанных с выездом из районов Крайнего Севера и приравненных к ним местностей на новое место жительства (ст. 35 Закона Российской Федерации № 4520-1)", Code = "9", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(9));

            _groupClaim = new GroupClaim { Name = StrTrim("Иски по назначению и выплате пенсий и иных социальных выплат"), Description = "Иски по назначению и выплате пенсий и иных социальных выплат", Code = "10", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(10));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с софинансированием средств пенсионных накоплений (ФЗ от 30.04.2008 № 56-ФЗ)"), Description = "Споры, связанные с софинансированием средств пенсионных накоплений (ФЗ от 30.04.2008 № 56-ФЗ)", Code = "11", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(11));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация ФЗ от 29.12.2006 № 256-ФЗ «О дополнительных мерах государственной поддержки семей, имеющих детей»"), Description = "Реализация ФЗ от 29.12.2006 № 256-ФЗ «О дополнительных мерах государственной поддержки семей, имеющих детей»", Code = "12", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(12));

            _groupClaim = new GroupClaim { Name = StrTrim("Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»"), Description = "Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»", Code = "13", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(13));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры по ФЗ от 15.12.2001 № 166-ФЗ  «О государственном пенсионном обеспечении в Российской Федерации»"), Description = "Споры по ФЗ от 15.12.2001 № 166-ФЗ  «О государственном пенсионном обеспечении в Российской Федерации»", Code = "14", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(14));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, возникающие по вопросам применения законодательства Российской Федерации о противодействии коррупции"), Description = "Споры, возникающие по вопросам применения законодательства Российской Федерации о противодействии коррупции", Code = "15", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(15));

            _groupClaim = new GroupClaim { Name = StrTrim("Об обжаловании решений, действий (бездействий) при оказании территориальными органами ПФР и их должностными лицами государственных услуг"), Description = "Об обжаловании решений, действий (бездействий) при оказании территориальными органами ПФР и их должностными лицами государственных услуг", Code = "16", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(16));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с осуществлением  территориальными органами ПФР мероприятий по реализации государственной программы Российской Федерации «Доступная среда»"), Description = "Споры, связанные с осуществлением  территориальными органами ПФР мероприятий по реализации государственной программы Российской Федерации «Доступная среда»", Code = "17", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(17));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с реализацией постановления Правительства Российской Федерации от 16.07.2016  № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»"), Description = "Споры, связанные с реализацией постановления Правительства Российской Федерации от 16.07.2016  № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»", Code = "18", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(18));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с реализацией постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»"), Description = "Споры, связанные с реализацией постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»", Code = "19", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(19));

            _groupClaim = new GroupClaim { Name = StrTrim("Трудовые споры"), Description = "Трудовые споры", Code = "20", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(20));

            _groupClaim = new GroupClaim { Name = StrTrim("Споры, связанные с финансово-хозяйственной деятельностью территориальных органов ПФР, а также споры, вытекающие из договоров (государственных контрактов)"), Description = "Споры, связанные с финансово-хозяйственной деятельностью территориальных органов ПФР, а также споры, вытекающие из договоров (государственных контрактов)", Code = "21", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(21));

            _groupClaim = new GroupClaim { Name = StrTrim("Иные"), Description = "Иные", Code = "22", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(22));

            _groupClaim = new GroupClaim { Name = StrTrim("О взыскании средств пенсионных накоплений (накопительной  пенсии) правопреемниками умерших  застрахованных лиц ( восстановление срока для обращения за выплатой СПН)"), Description = "О взыскании средств пенсионных накоплений (накопительной  пенсии) правопреемниками умерших  застрахованных лиц ( восстановление срока для обращения за выплатой СПН)", Code = "23", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(23));

            _groupClaim = new GroupClaim { Name = StrTrim("Дела особого производства (территориальный органы ПФР - заинтересованные лица)"), Description = "Дела особого производства (территориальный органы ПФР - заинтересованные лица)", Code = "24", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(24));

            _groupClaim = new GroupClaim { Name = StrTrim("Судебные расходы"), Description = "Судебные расходы", Code = "25", CategoryDispute = _categoryDispute };
            await dataContext.GroupClaims.AddAsync(_groupClaim);
            await dataContext.SubjectClaims.AddRangeAsync(GetPreconfiguredSubjectClaimsIn(25));

            return new List<CategoryDispute>() { _categoryDispute };
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameTypeApplicant(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Тип контрагента" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(await GetPreconfiguredTypeApplicant(dataContext));

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameOpf(DataContext dataContext)
        {
            _dirName = new DirName { Name = "ОПФ" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredOpf());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameCourtDecision1(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Решения суда 1-ой инстанции" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredDecision1());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameCourtDecision2(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Решения суда 2-ой инстанции" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredDecision2());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameCourtDecision3(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Решения суда 3-ей инстанции" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredDecision3());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameCourtDecision4(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Решения суда 4-ой инстанции" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredDecision4());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static async Task<IEnumerable<DirName>> GetPreconfiguredDirNameCourt(DataContext dataContext)
        {
            _dirName = new DirName { Name = "Суд" };
            await dataContext.DirName.AddAsync(_dirName);
            await dataContext.Dir.AddRangeAsync(GetPreconfiguredCourt());

            var result = new List<DirName> { _dirName };
            return result;
        }
        private static IEnumerable<Dir> GetPreconfiguredCourt()
        {
            return new List<Dir>()
            {
                new Dir {Name="Арбитражный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Бондарсвкий районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Гавриловский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Жердевский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Знаменский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Инжавинский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Кирсановский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Котовский городской суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Ленинский районный суд г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд г. Котовска Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд г. Мичуринска Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Кирсановского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Ленинского района г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Мичуринского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Моршанского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Октябрьского района г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд по г. Уварово и Уваровскому району Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Рассказовского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Советского района г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Сосновкого района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд судебного участка Пичаевского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой суд Тамбовского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка  Мучкапского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Бондарского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Гавриловского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Жердевского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Знаменского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Инжавинского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Мордовского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Никифоровского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Первомайского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Петровского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Пичаевского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Ржаксинского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Сампурского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Староюрьевского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Токаревского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мировой судья судебного участка Уметского района Тамбовской области", DirName=_dirName },
                new Dir {Name="Мичуринский городской суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Мичуринский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Мордовский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Моршанский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Мучкапский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Никифоровский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Октябрьский районный суд г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Первомайский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Петровский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Пичаевский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Рассказовский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Ржаксинский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Сампурский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Советский районный суд г. Тамбова Тамбовской области", DirName=_dirName },
                new Dir {Name="Сосновский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Староюрьевский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Тамбовский областной суд", DirName=_dirName },
                new Dir {Name="Тамбовский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Токаревский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Уваровский районный суд Тамбовской области", DirName=_dirName },
                new Dir {Name="Уметский районный суд Тамбовской области", DirName=_dirName }
            };
        }

        private static async Task<IEnumerable<Dir>> GetPreconfiguredTypeApplicant(DataContext dataContext)
        {
            var result = new List<Dir>();

            _dir = new Dir { Name = "Физическое лицо", DirName = _dirName };
            result.Add(_dir);
            await dataContext.Dir.AddAsync(_dir);
            await dataContext.Applicant.AddRangeAsync(new List<Applicant>() { new Applicant { Name = "test", Description = "test", TypeApplicant = _dir }, new Applicant { Name = "test1", Description = "test1", TypeApplicant = _dir }
            });

            _dir = new Dir { Name = "Юридическое лицо", DirName = _dirName };
            await dataContext.Dir.AddAsync(_dir);
            result.Add(_dir);
            await dataContext.Applicant.AddRangeAsync(new List<Applicant>() { new Applicant { Name = "test2", Description = "test2", TypeApplicant = _dir }, new Applicant { Name = "test3", Description = "test3", TypeApplicant = _dir }
            });

            return result;
            //{
            //    new Dir {Name="Физическое лицо", DirName=dirName },
            //    new Dir {Name="Юридическое лицо", DirName=dirName }
            //};
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision1()
        {
            return new List<Dir>()
            {
                new Dir {Name="Удовлетворено (частично)", DirName=_dirName },
                new Dir {Name="Отказано", DirName=_dirName },
                new Dir {Name="Прекращено", DirName=_dirName },
                new Dir {Name ="Оставлено без рассмотрения", DirName=_dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision2()
        {
            return new List<Dir>()
            {
                new Dir {Name="Удовлетворено (частично)", DirName=_dirName },
                new Dir {Name="Отказано", DirName=_dirName },
                new Dir {Name="Прекращено", DirName=_dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision3()
        {
            return new List<Dir>()
            {
                new Dir {Name="Удовлетворено (частично)", DirName=_dirName },
                new Dir {Name="Отказано", DirName=_dirName },
                new Dir {Name="Прекращено", DirName=_dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredDecision4()
        {
            return new List<Dir>()
            {
                new Dir {Name="Удовлетворено (частично)", DirName=_dirName },
                new Dir {Name="Отказано", DirName=_dirName },
                new Dir {Name="Прекращено", DirName=_dirName }
            };
        }
        private static IEnumerable<Dir> GetPreconfiguredOpf()
        {
            return new List<Dir>()
            {
                new Dir {Name="ЗАО", DirName=_dirName },
                new Dir {Name="ООО", DirName=_dirName },
                new Dir {Name="ОАО", DirName=_dirName },
                new Dir {Name="АО", DirName=_dirName },
                new Dir {Name="МУП", DirName=_dirName },
                new Dir {Name="НАО", DirName=_dirName },
                new Dir {Name="ОДО", DirName=_dirName },
                new Dir {Name="ПАО", DirName=_dirName },
                new Dir {Name="ФГУП", DirName=_dirName },
                new Dir {Name="ДУП", DirName=_dirName },
                new Dir {Name="ПТ", DirName=_dirName },
                new Dir {Name="ПК", DirName=_dirName },
                new Dir {Name="ТВ", DirName=_dirName },
                new Dir {Name="ГУП", DirName=_dirName },
                new Dir {Name="АНО", DirName=_dirName },
                new Dir {Name="ГЖИ", DirName=_dirName },
                new Dir {Name="ГК", DirName=_dirName },
                new Dir {Name="ГОУ", DirName=_dirName },
                new Dir {Name="ГУ", DirName=_dirName },
                new Dir {Name="МДОУ", DirName=_dirName },
                new Dir {Name="МОУ", DirName=_dirName },
                new Dir {Name="ОГБУ", DirName=_dirName },
                new Dir {Name="СО", DirName=_dirName },
                new Dir {Name="ТОС", DirName=_dirName },
                new Dir {Name="ТСЖ", DirName=_dirName },
                new Dir {Name="ФГБУ", DirName=_dirName },
                new Dir {Name="ФГУ", DirName=_dirName },
                new Dir {Name="АБ", DirName=_dirName },
                new Dir {Name="АС", DirName=_dirName },
                new Dir {Name="АКХ", DirName=_dirName },
                new Dir {Name="АФХ", DirName=_dirName },
                new Dir {Name="ГК", DirName=_dirName },
                new Dir {Name="КО", DirName=_dirName },
                new Dir {Name="КА", DirName=_dirName },
                new Dir {Name="МУ", DirName=_dirName },
                new Dir {Name="НП", DirName=_dirName },
                new Dir {Name="ОО", DirName=_dirName },
                new Dir {Name="ОД", DirName=_dirName },
                new Dir {Name="ОФ", DirName=_dirName },
                new Dir {Name="ОВС", DirName=_dirName },
                new Dir {Name="ОМН РФ", DirName=_dirName },
                new Dir {Name="ООС", DirName=_dirName },
                new Dir {Name="ПП", DirName=_dirName },
                new Dir {Name="ПК", DirName=_dirName },
                new Dir {Name="Профсоюз", DirName=_dirName },
                new Dir {Name="РО", DirName=_dirName },
                new Dir {Name="СОДНО", DirName=_dirName },
                new Dir {Name="Фонд", DirName=_dirName },
                new Dir {Name="ЧУ", DirName=_dirName },
                new Dir {Name="Государственный орган", DirName=_dirName }
            };
        }

        private static IEnumerable<Performer> GetPreconfiguredPerformers()
        {
            return new List<Performer>()
            {
                new Performer {Name="Белякина Маргарита Александровна", Region = _region },
                new Performer {Name="Волосевич Юлия Сергеевна", Region = _region },
                new Performer {Name="Арзамасцева Елена Геннадьевна", Region = _region },
                new Performer {Name="Галузинская Екатерина Владимировна", Region = _region },
                new Performer {Name="Горшкова Ирина Геннадьевна", Region = _region },
                new Performer {Name="Грабко Александр Сергеевич", Region = _region },
                new Performer {Name="Гунько Анастасия Игоревна", Region = _region },
                new Performer {Name="Данилова Виктория Викторовна", Region = _region },
                new Performer {Name="Завражнева Ольга Анатольевна", Region = _region },
                new Performer {Name="Киянова Татьяна Ивановна", Region = _region },
                new Performer{Name="Колотуша Марта Анатольевна", Region = _region },
                new Performer{Name="Корнеева Елена Владимировна", Region = _region },
                new Performer{Name="Корнишина Лариса Анатольевна", Region = _region },
                new Performer{Name="Коханов Дмитрий Павлович", Region = _region },
                new Performer{Name="Курьянова Елена Николаевна", Region = _region },
                new Performer{Name="Лихачева Елена Николаевна", Region = _region },
                new Performer{Name="Ломовцева Татьяна Александровна", Region = _region },
                new Performer{Name="Николаев Алексей Евгеньевич", Region = _region },
                new Performer{Name="Панова Ольга Анатольевна", Region = _region },
                new Performer{Name="Платицына Елена Геннадьевна", Region = _region },
                new Performer{Name="Решетова Ирина Николаевна", Region = _region },
                new Performer{Name="Рыбкина Ольга Анатольевна", Region = _region },
                new Performer{Name="Рыжкова Юлия Владимировна", Region = _region },
                new Performer{Name="Сапрыкина Анастасия Александровна", Region = _region },
                new Performer{Name="Сиднева Галина Васильевна", Region = _region },
                new Performer{Name="Суворин Андрей Владимирович", Region = _region },
                new Performer{Name="Сычева Светлана Алексеевна", Region = _region },
                new Performer{Name="Тарнопольская Елена Сергеевна", Region = _region },
                new Performer{Name="Топорков Илья Николаевич", Region = _region },
                new Performer{Name="Труба Антонина Александровна", Region = _region },
                new Performer{Name="Фатахутдинов Денис Фаилевич", Region = _region },
                new Performer{Name="Черкасова Мария Сергеевна", Region = _region },
                new Performer{Name="Четверикова Елена Витальевна", Region = _region },
                new Performer{Name="Чубарова Юлия Юрьевна", Region = _region },
                new Performer{Name="Шишкова Елена Алексеевна", Region = _region },
                new Performer{Name="Яблочкина Татьяна Юрьевна", Region = _region }
            };
        }

        private static IEnumerable<Region> GetPreconfiguredRegions()
        {
            _region = new Region { Code = "079", Name = "Тамбовская область", Description = "Каталог ОПФР по Тамбовской области" };
            return new List<Region>() { _region, new Region { Code = "100", Name = "Московская область", Description = "Каталог ОПФР по Московской области" } };
        }

        private static IEnumerable<District> GetPreconfiguredDistricts()
        {
            return new List<District>() {
                new District {Code="001", Name = "Бондарский", Description = "ОПФР по Бондарскому району", Region = _region },
                new District {Code="002",  Name = "Гавриловский", Description = "ОПФР по Гавриловскому району", Region = _region },
                new District { Code="003", Name = "Жердевский", Description = "ОПФР по Жердевскому району", Region = _region },
                new District { Code="004", Name = "Знаменский", Description = "ОПФР по Знаменскому району", Region = _region },
                new District { Code="005", Name = "Инжавинский", Description = "ОПФР по Инжавинскому району", Region = _region },
                new District { Code="006", Name = "Кирсановский", Description = "ОПФР по Кирсановскому району", Region = _region },
                new District { Code="007", Name = "Мичуринский", Description = "ОПФР по Мичуринскому району", Region = _region },
                new District { Code="008", Name = "Мордовский", Description = "ОПФР по Мордовскому району", Region = _region },
                new District { Code="009", Name = "Моршанский", Description = "ОПФР по Моршанскому району", Region = _region },
                new District { Code="010", Name = "Мучкапский", Description = "ОПФР по Мучкапскому району", Region = _region },
                new District { Code="011", Name = "Никифоровский", Description = "ОПФР по Никифоровскому району", Region = _region },
                new District { Code="012", Name = "Первомайский", Description = "ОПФР по Первомайскому району", Region = _region },
                new District { Code="013", Name = "Петровский", Description = "ОПФР по Петровскому району", Region = _region },
                new District { Code="014", Name = "Пичаевский", Description = "ОПФР по Пичаевскому району", Region = _region },
                new District { Code="015", Name = "Рассказовский", Description = "ОПФР по Рассказовскому району", Region = _region },
                new District { Code="016", Name = "Ржаксинский", Description = "ОПФР по Ржаксинскому району", Region = _region },
                new District { Code="017", Name = "Сампурский", Description = "ОПФР по Сампурскому району", Region = _region },
                new District { Code="018", Name = "Сосновский", Description = "ОПФР по Сосновскому району", Region = _region },
                new District { Code="019", Name = "Староюрьевский", Description = "ОПФР по Староюрьевскому району", Region = _region },
                new District { Code="020", Name = "Тамбовский", Description = "ОПФР по Тамбовскому району", Region = _region },
                new District { Code="021", Name = "Токаревский", Description = "ОПФР по Токаревскому району", Region = _region },
                new District { Code="022", Name = "Уваровский", Description = "ОПФР по Уваровскому району", Region = _region },
                new District { Code="023", Name = "Уметский", Description = "ОПФР по Уметскому району", Region = _region },
                new District { Code="024", Name = "Котовск", Description = "ОПФР по г.Котовску", Region = _region },
                new District { Code="025", Name = "Тамбов", Description = "ОПФР по г.Тамбову", Region = _region } };
        }

        private static IEnumerable<SubjectClaim> GetPreconfiguredSubjectClaimsOut(int cod)
        {
            switch (cod)
            {
                case 1:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "1.1", Name = StrTrim("О взыскании недоимки по страховым взносам,  пеней и штрафов с организаций и индивидуальных предпринимателей"), GroupClaim = _groupClaim, Description = "О взыскании недоимки по страховым взносам,  пеней и штрафов с организаций и индивидуальных предпринимателей" }
                    };
                case 2:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "2.1", Name = StrTrim("О взыскании финансовых санкций за непредставление в установленные сроки сведений, необходимых для осуществления индивидуального (персонифицированного) учета (абз. 3 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за непредставление в установленные сроки сведений, необходимых для осуществления индивидуального (персонифицированного) учета (абз. 3 ст. 17 ФЗ № 27-ФЗ)" },
                        new SubjectClaim { Code = "2.2", Name = StrTrim("О взыскании финансовых санкций за представление страхователем неполных и (или) недостоверных сведений (абз. 3 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за представление страхователем неполных и (или) недостоверных сведений (абз. 3 ст. 17 ФЗ № 27-ФЗ)" },
                        new SubjectClaim { Code = "2.3", Name = StrTrim("О взыскании финансовых санкций за несоблюдение страхователем порядка представлений сведений для индивидуального (персонифицированного) учета в форме электронных документов (абз. 4 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за несоблюдение страхователем порядка представлений сведений для индивидуального (персонифицированного) учета в форме электронных документов (абз. 4 ст. 17 ФЗ № 27-ФЗ)" }
                    };
                case 3:
                    return new List<SubjectClaim>
                    {   new SubjectClaim { Code = "3.1", Name = StrTrim("О включении в реестр требований кредиторов задолженности по страховым взносам, пеней и штрафов"), GroupClaim = _groupClaim, Description = "О включении в реестр требований кредиторов задолженности по страховым взносам, пеней и штрафов" }
                    };
                case 4:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "4.1", Name = StrTrim("О взыскании ущерба с  работодателя в связи с представлением им неполных (или) недостоверных сведений, предусмотренных"), GroupClaim = _groupClaim, Description = "О взыскании ущерба с  работодателя в связи с представлением им неполных (или) недостоверных сведений, предусмотренных" },
                        new SubjectClaim { Code = "4.2", Name = StrTrim("О взыскании с кредитных организаций излишне перечисленных на счет пенсионеров после их смерти сумм пенсии, ЕДВ, ДМО, региональной социальной доплаты, иных выплат"), GroupClaim = _groupClaim, Description = "О взыскании с кредитных организаций излишне перечисленных на счет пенсионеров после их смерти сумм пенсии, ЕДВ, ДМО, региональной социальной доплаты, иных выплат" },
                        new SubjectClaim { Code = "4.3", Name = StrTrim("О взыскании с учреждения медико-социальной экспертизы ущерба, возникшего вследствие излишней выплаты пенсионеру пенсии, ЕДВ и других компенсационных выплат"), GroupClaim = _groupClaim, Description = "О взыскании с учреждения медико-социальной экспертизы ущерба, возникшего вследствие излишней выплаты пенсионеру пенсии, ЕДВ и других компенсационных выплат" }
                    };
                case 5:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "5.1", Name = StrTrim("О взыскании недоимки по страховым взносам, пеней и штрафов с физического лица, утратившего статус индивидуального предпринимателя"), GroupClaim = _groupClaim, Description = "О взыскании недоимки по страховым взносам, пеней и штрафов с физического лица, утратившего статус индивидуального предпринимателя" }
                    };
                case 6:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "6.1", Name = StrTrim("О взыскании финансовых санкций за непредставление в установленные сроки необходимых для осуществления индивидуального (персонифицированного) учета в системе обязательного пенсионного страхования сведений либо представление"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за непредставление в установленные сроки необходимых для осуществления индивидуального (персонифицированного) учета в системе обязательного пенсионного страхования сведений либо представление" },
                        new SubjectClaim { Code = "6.1.1", Name = StrTrim("О взыскании финансовых санкций за непредставление в установленные сроки сведений, необходимых для осуществления индивидуального (персонифицированного) учета (абз. 3 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за непредставление в установленные сроки сведений, необходимых для осуществления индивидуального (персонифицированного) учета (абз. 3 ст. 17 ФЗ № 27-ФЗ)" },
                        new SubjectClaim { Code = "6.1.2", Name = StrTrim("О взыскании финансовых санкций за представление страхователем неполных и (или) недостоверных сведений (абз. 3 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за представление страхователем неполных и (или) недостоверных сведений (абз. 3 ст. 17 ФЗ № 27-ФЗ)" },
                        new SubjectClaim { Code = "6.1.3", Name = StrTrim("О взыскании финансовых санкций за несоблюдение страхователем порядка представлений сведений для индивидуального (персонифицированного) учета в форме электронных документов (абз. 4 ст. 17 ФЗ № 27-ФЗ)"), GroupClaim = _groupClaim, Description = "О взыскании финансовых санкций за несоблюдение страхователем порядка представлений сведений для индивидуального (персонифицированного) учета в форме электронных документов (абз. 4 ст. 17 ФЗ № 27-ФЗ)" },
                        new SubjectClaim { Code = "6.2", Name = StrTrim("Об обязании представить индивидуальные сведения о застрахованных лицах"), GroupClaim = _groupClaim, Description = "Об обязании представить индивидуальные сведения о застрахованных лицах" },
                        new SubjectClaim { Code = "6.3", Name = StrTrim("О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица"), GroupClaim = _groupClaim, Description = "О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица" }
                    };
                case 7:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "7.1", Name = StrTrim("О взыскании излишне выплаченных сумм пенсий, ЕДВ, ДМО, федеральной социальной доплаты к пенсии и иных выплат"), GroupClaim = _groupClaim, Description = "О взыскании излишне выплаченных сумм пенсий, ЕДВ, ДМО, федеральной социальной доплаты к пенсии и иных выплат" },
                        new SubjectClaim { Code = "7.2", Name = StrTrim("О взыскании в регрессном порядке выплаченных сумм пенсии по инвалидности или по потере кормильца с лиц, виновных в причинении вреда здоровью или жизни кормильца"), GroupClaim = _groupClaim, Description = "О взыскании в регрессном порядке выплаченных сумм пенсии по инвалидности или по потере кормильца с лиц, виновных в причинении вреда здоровью или жизни кормильца" }
                    };
                case 8:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "8.1", Name = StrTrim("О взыскании штрафа за непредставление в установленный законодательством Российской Федерации срок либо отказ от представления, предоставление в неполном объеме или искаженном виде сведений, необходимых для ведения индивидуального"), GroupClaim = _groupClaim, Description = "О взыскании штрафа за непредставление в установленный законодательством Российской Федерации срок либо отказ от представления, предоставление в неполном объеме или искаженном виде сведений, необходимых для ведения индивидуального" }
                        };
                case 9:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "9", Name = StrTrim("Взыскание средств пенсионных накоплений в порядке регресса с правопреемников, которым выплачены средства пенсионных накоплений (п. 31 Правил, утвержденных постановлением Правительства Российской Федерации от 30.07.2014 № 711)"), GroupClaim = _groupClaim, Description = "Взыскание средств пенсионных накоплений в порядке регресса с правопреемников, которым выплачены средства пенсионных накоплений (п. 31 Правил, утвержденных постановлением Правительства Российской Федерации от 30.07.2014 № 711)" }
                      };
                case 10:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "10.1", Name = StrTrim("О признании государственного сертификата на материнский (семейный) капитал недействительным"), GroupClaim = _groupClaim, Description = "О признании государственного сертификата на материнский (семейный) капитал недействительным" },
                        new SubjectClaim { Code = "10.2", Name = StrTrim("О взыскании средств материнского (семейного) капитала"), GroupClaim = _groupClaim, Description = "О взыскании средств материнского (семейного) капитала" },
                        new SubjectClaim { Code = "10.3", Name = StrTrim("Об исполнении обязательства оформить жилое помещение, приобретенное с использованием средств МСК в общую долевую собственность"), GroupClaim = _groupClaim, Description = "Об исполнении обязательства оформить жилое помещение, приобретенное с использованием средств МСК в общую долевую собственность" }
                    };
                case 11:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "11.1", Name = StrTrim("О взыскании  незаконно полученных средств материнского (семейного) капитала в рамках рассмотрения уголовного дела"), GroupClaim = _groupClaim, Description = "О взыскании  незаконно полученных средств материнского (семейного) капитала в рамках рассмотрения уголовного дела" },
                        new SubjectClaim { Code = "11.2", Name = StrTrim("О взыскании незаконно полученной пенсии по инвалидности"), GroupClaim = _groupClaim, Description = "О взыскании незаконно полученной пенсии по инвалидности" }
                    };
                case 12:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "12.1", Name = StrTrim("О взыскании незаконно полученной компенсации стоимости проезда к месту отдыха за пределами территории Российской Федерации"), GroupClaim = _groupClaim, Description = "О взыскании незаконно полученной компенсации стоимости проезда к месту отдыха за пределами территории Российской Федерации" }
                    };
                case 13:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "13", Name = StrTrim("О взыскании незаконно полученной компенсации расходов, связанных с переездом из районов Крайнего Севера и приравненных к ним местностей, лицам, являющимся получателями трудовых пенсий и(или) пенсий по государственному пенсионному"), GroupClaim = _groupClaim, Description = "О взыскании незаконно полученной компенсации расходов, связанных с переездом из районов Крайнего Севера и приравненных к ним местностей, лицам, являющимся получателями трудовых пенсий и(или) пенсий по государственному пенсионному" }
                    };
                case 14:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "14", Name = StrTrim("Реализация постановления Правительства Российской Федерации от 16.07.2016 № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»"), GroupClaim = _groupClaim, Description = "Реализация постановления Правительства Российской Федерации от 16.07.2016 № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»" },
                        new SubjectClaim { Code = "14.1", Name = StrTrim("Непредставление должностным лицом сведений, подлежащих включению в федеральный реестр инвалидов, неразмещение или нарушение установленных законодательством Российской Федерации порядка и (или) сроков размещения в данном"), GroupClaim = _groupClaim, Description = "Непредставление должностным лицом сведений, подлежащих включению в федеральный реестр инвалидов, неразмещение или нарушение установленных законодательством Российской Федерации порядка и (или) сроков размещения в данном " }
                    };
                case 15:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "15", Name = StrTrim("Реализация постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»"), GroupClaim = _groupClaim, Description = "Реализация постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»" }
                     };
                case 16:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "16", Name = StrTrim("Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»"), GroupClaim = _groupClaim, Description = "Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»" }
                    };
                case 17:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "17.1", Name = StrTrim("О расторжении договора (государственного контракта) в случае неисполнения или ненадлежащего исполнения поставщиком (подрядчиком, исполнителем) обязательств, предусмотренных государственным контрактом и (или) взыскании неустойки"), GroupClaim = _groupClaim, Description = "О расторжении договора (государственного контракта) в случае неисполнения или ненадлежащего исполнения поставщиком (подрядчиком, исполнителем) обязательств, предусмотренных государственным контрактом и (или) взыскании неустойки" },
                        new SubjectClaim { Code = "17.2", Name = StrTrim("О взыскании с поставщиков (подрядчиков, исполнителей) сумм неустоек (штрафов, пеней) в случае просрочки исполнения поставщиком (подрядчиком, исполнителем) обязательств (в том числе гарантийного обязательства), предусмотренных "), GroupClaim = _groupClaim, Description = "О взыскании с поставщиков (подрядчиков, исполнителей) сумм неустоек (штрафов, пеней) в случае просрочки исполнения поставщиком (подрядчиком, исполнителем) обязательств (в том числе гарантийного обязательства), предусмотренных" },
                        new SubjectClaim { Code = "17.3", Name = StrTrim("Об обмене некачественного или некомплектного товара, взыскании долга и процентов за пользование чужими денежными средствами"), GroupClaim = _groupClaim, Description = "Об обмене некачественного или некомплектного товара, взыскании долга и процентов за пользование чужими денежными средствами" },
                        new SubjectClaim { Code = "17.4", Name = StrTrim("О признании незаконными решений федерального органа исполнительной власти, уполномоченного на осуществление контроля в сфере закупок (ФАС России, территориальных органов ФАС России)"), GroupClaim = _groupClaim, Description = "О признании незаконными решений федерального органа исполнительной власти, уполномоченного на осуществление контроля в сфере закупок (ФАС России, территориальных органов ФАС России)" },
                        new SubjectClaim { Code = "17.5", Name = StrTrim("О взыскании по ДТП"), GroupClaim = _groupClaim, Description = "О взыскании по ДТП" },
                        new SubjectClaim { Code = "17.6", Name = StrTrim("О возмещении ущерба, причиненного работодателю"), GroupClaim = _groupClaim, Description = "О возмещении ущерба, причиненного работодателю" }
                    };
                case 18:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "18", Name = StrTrim("Иные"), GroupClaim = _groupClaim, Description = "" }
                    };
                case 19:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "19", Name = StrTrim("Иск пенсионера, работника или прокурора к работодателю об обязании уплатить страховые взносы, подать в ПФР сведения ПУ или расчет по начисленным и уплаченным страховым взносам, о взыскании суммы неполученной пенсии в связи с неуплатой"), GroupClaim = _groupClaim, Description = "Иск пенсионера, работника или прокурора к работодателю об обязании уплатить страховые взносы, подать в ПФР сведения ПУ или расчет по начисленным и уплаченным страховым взносам, о взыскании суммы неполученной пенсии в связи с неуплатой" }
                    };
                case 20:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "20.1", Name = StrTrim("Оплата госпошлины (арбитраж)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "20.2", Name = StrTrim("Иные судебные издержки (арбитраж)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "20.3", Name = StrTrim("Оплата госпошлины (общая юрисдикция)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "20.4", Name = StrTrim("Иные судебные издержки (общая юрисдикция)"), GroupClaim = _groupClaim, Description = "" }
                    };
                default:
                    return null;
            }
        }
        private static IEnumerable<SubjectClaim> GetPreconfiguredSubjectClaimsIn(int cod)
        {
            switch (cod)
            {
                case 1:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("О признании исполненной обязанности по уплате страховых взносов, подлежащих уплате за отчетные (расчетные)  периоды, истекшие до 01.01.2017"), Code = "1.1", GroupClaim = _groupClaim, Description = "О признании исполненной обязанности по уплате страховых взносов, подлежащих уплате за отчетные (расчетные)  периоды, истекшие до 01.01.2017" },
                        new SubjectClaim { Name = StrTrim("О зачете (возврате) излишне уплаченных (взысканных) страховых взносов, пеней и штрафов за отчетные (расчетные) периоды, истекшие до 01.01.2017"), Code = "1.2", GroupClaim = _groupClaim, Description = "О зачете (возврате) излишне уплаченных (взысканных) страховых взносов, пеней и штрафов за отчетные (расчетные) периоды, истекшие до 01.01.2017" },
                        new SubjectClaim { Name = StrTrim("О признании невозможной ко взысканию недоимки по уплате страховых взносов, пеней и штрафов по искам к налоговым органам"), Code = "1.3", GroupClaim = _groupClaim, Description = "О признании невозможной ко взысканию недоимки по уплате страховых взносов, пеней и штрафов по искам к налоговым органам" }
                    };
                case 2:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("О признании незаконным решения территориального органа ПФР о взыскании недоимки по страховым взносам, пеней и штрафов за отчетные (расчетные) периоды, истекшие до 01.01.2017"), Code = "2.1", GroupClaim = _groupClaim, Description = "О признании незаконным решения территориального органа ПФР о взыскании недоимки по страховым взносам, пеней и штрафов за отчетные (расчетные) периоды, истекшие до 01.01.2017" },
                        new SubjectClaim { Name = StrTrim("Об обжаловании  решения территориального органа ПФР о привлечении к ответственности за нарушение законодательства об обязательном пенсионном страховании и законодательства Российской Федерации о страховых взносах"), Code = "2.2", GroupClaim = _groupClaim, Description = "Об обжаловании  решения территориального органа ПФР о привлечении к ответственности за нарушение законодательства об обязательном пенсионном страховании и законодательства Российской Федерации о страховых взносах" }
                    };
                case 3:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица"), Code = "3.1", GroupClaim = _groupClaim, Description = "О внесении изменений в индивидуальные сведения персонифицированного учета застрахованного лица" },
                        new SubjectClaim { Name = StrTrim("Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках абз. 3 ст. 17 ФЗ № 27-ФЗ"), Code = "3.2", GroupClaim = _groupClaim, Description = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках абз. 3 ст. 17 ФЗ № 27-ФЗ" },
                        new SubjectClaim { Name = StrTrim("Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках абз. 4 ст. 17 ФЗ № 27-ФЗ"), Code = "3.3", GroupClaim = _groupClaim, Description = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках абз. 4 ст. 17 ФЗ № 27-ФЗ" }
                    };
                case 4:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("Об оспаривании сделок"), Code = "4.1", GroupClaim = _groupClaim, Description = "Об оспаривании сделок" }
                    };
                case 5:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках ФЗ  № 27-ФЗ"), Code = "5", GroupClaim = _groupClaim, Description = "Обжалование действий (бездействий), актов, решений территориальных органов ПФР и их должностных лиц в рамках ФЗ  № 27-ФЗ" }
                    };
                case 6:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)"), Code = "6", GroupClaim = _groupClaim, Description = "Взыскание страховых взносов, пеней и штрафов (в т.ч. территориальный орган ПФР – третье лицо)" }
                    };
                case 7:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("О признании договора об обязательном пенсионном страховании недействительным и (или) внесении изменений в Единый реестр застрахованных лиц"), Code = "7.1", GroupClaim = _groupClaim, Description = "О признании договора об обязательном пенсионном страховании недействительным и (или) внесении изменений в Единый реестр застрахованных лиц" },
                        new SubjectClaim { Name = StrTrim("Споры по агентским договорам, заключенным с НПФ (территориальный орган ПФР – 3-е лицо)"), Code = "7.2", GroupClaim = _groupClaim, Description = "Споры по агентским договорам, заключенным с НПФ (территориальный орган ПФР – 3-е лицо)" }
                    };
                case 8:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Name = StrTrim("О взыскании компенсации стоимости проезда к месту отдыха за пределы Российской Федерации"), Code = "8.1", GroupClaim = _groupClaim, Description = "О взыскании компенсации стоимости проезда к месту отдыха за пределы Российской Федерации" }
                    };
                case 9:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "9", Name = StrTrim("О взыскании расходов, связанных с выездом из районов Крайнего Севера и приравненных к ним местностей на новое место жительства (ст. 35 Закона Российской Федерации № 4520-1)"), GroupClaim = _groupClaim, Description = "О взыскании расходов, связанных с выездом из районов Крайнего Севера и приравненных к ним местностей на новое место жительства (ст. 35 Закона Российской Федерации № 4520-1)" }
                    };
                case 10:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "10.1", Name = StrTrim("Индексация и валоризация  пенсий"), GroupClaim = _groupClaim, Description = "Индексация и валоризация  пенсий" },
                        new SubjectClaim { Code = "10.2.1", Name = StrTrim("Установление досрочных пенсий по ст. 30 ФЗ № 400-ФЗ, по п. 1 и 2 ч. 1 ст. 30"), GroupClaim = _groupClaim, Description = "Установление досрочных пенсий по ст. 30 ФЗ № 400-ФЗ, по п. 1 и 2 ч. 1 ст. 30" },
                        new SubjectClaim { Code = "10.2.2", Name = StrTrim("Установление досрочных пенсий по ст. 30 ФЗ № 400-ФЗ, по п. 19 и 20 ч. 1 ст. 30"), GroupClaim = _groupClaim, Description = "Установление досрочных пенсий по ст. 30 ФЗ № 400-ФЗ, по п. 19 и 20 ч. 1 ст. 30" },
                        new SubjectClaim { Code = "10.3.1", Name = StrTrim("Установление страховой пенсии, о праве на назначение пенсий (кроме ст. 30 ФЗ № 400-ФЗ)"), GroupClaim = _groupClaim, Description = "Установление страховой пенсии, о праве на назначение пенсий (кроме ст. 30 ФЗ № 400-ФЗ)" },
                        new SubjectClaim { Code = "10.3.2", Name = StrTrim("Установление страховой пенсии, об исчислении размера пенсии"), GroupClaim = _groupClaim, Description = "Установление страховой пенсии, об исчислении размера пенсии" },
                        new SubjectClaim { Code = "10.3.3", Name = StrTrim("Установление страховой пенсии, о сроках назначения пенсии"), GroupClaim = _groupClaim, Description = "Установление страховой пенсии, о сроках назначения пенсии" },
                        new SubjectClaim { Code = "10.3.4", Name = StrTrim("О праве на повышенную фиксированную выплату к страховой пенсии (ч. 14 ст. 17 ФЗ № 400-ФЗ)"), GroupClaim = _groupClaim, Description = "О праве на повышенную фиксированную выплату к страховой пенсии (ч. 14 ст. 17 ФЗ № 400-ФЗ)" },
                        new SubjectClaim { Code = "10.4", Name = StrTrim("Выплата пенсии"), GroupClaim = _groupClaim, Description = "Выплата пенсии" },
                        new SubjectClaim { Code = "10.5", Name = StrTrim("Компенсационные и ежемесячные выплаты трудоспособному лицу, доплаты к пенсии и иные социальные выплаты"), GroupClaim = _groupClaim, Description = "Компенсационные и ежемесячные выплаты трудоспособному лицу, доплаты к пенсии и иные социальные выплаты" },
                        new SubjectClaim { Code = "10.6", Name = StrTrim("Иски об установлении, перерасчете и выплате федеральной социальной доплаты к пенсии"), GroupClaim = _groupClaim, Description = "Иски об установлении, перерасчете и выплате федеральной социальной доплаты к пенсии" },
                        new SubjectClaim { Code = "10.7", Name = StrTrim("Споры, связанные с выплатой накопительной пенсии"), GroupClaim = _groupClaim, Description = "Споры, связанные с выплатой накопительной пенсии" }

                    };
                case 11:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "11", Name = StrTrim("Споры, связанные с софинансированием средств пенсионных накоплений (ФЗ от 30.04.2008 № 56-ФЗ)"), GroupClaim = _groupClaim, Description = "Споры, связанные с софинансированием средств пенсионных накоплений (ФЗ от 30.04.2008 № 56-ФЗ)" }
                    };
                case 12:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "12.1", Name = StrTrim("Об обжаловании решения об отказе в выдаче государственного сертификата на материнский (семейный) капитал  (ч. 6 ст. 5 ФЗ № 256-ФЗ)"), GroupClaim = _groupClaim, Description = "Об обжаловании решения об отказе в выдаче государственного сертификата на материнский (семейный) капитал  (ч. 6 ст. 5 ФЗ № 256-ФЗ)" },
                        new SubjectClaim { Code = "12.2.1", Name = StrTrim("Об обжаловании решений об отказе в удовлетворении заявления о распоряжении  средствами (частью средств) материнского  (семейного) капитала  (ст. 10 ФЗ № 256-ФЗ)"), GroupClaim = _groupClaim, Description = "Об обжаловании решений об отказе в удовлетворении заявления о распоряжении  средствами (частью средств) материнского  (семейного) капитала  (ст. 10 ФЗ № 256-ФЗ)" },
                        new SubjectClaim { Code = "12.3", Name = StrTrim("О восстановлении права на дополнительные меры государственной  поддержки и зачислении средств материнского (семейного)  капитала на лицевой счет владельца государственного сертификата"), GroupClaim = _groupClaim, Description = "О восстановлении права на дополнительные меры государственной  поддержки и зачислении средств материнского (семейного)  капитала на лицевой счет владельца государственного сертификата" }
                    };
                case 13:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "13", Name = StrTrim("Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»"), GroupClaim = _groupClaim, Description = "Реализация ФЗ от 28.12.2017 № 418-ФЗ «О ежемесячных выплатах семьям, имеющим детей»" }
                    };
                case 14:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "14", Name = StrTrim("Споры по ФЗ от 15.12.2001 № 166-ФЗ  «О государственном пенсионном обеспечении в Российской Федерации»"), GroupClaim = _groupClaim, Description = "Споры по ФЗ от 15.12.2001 № 166-ФЗ  «О государственном пенсионном обеспечении в Российской Федерации»" }
                    };
                case 15:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "15", Name = StrTrim("Споры, возникающие по вопросам применения законодательства Российской Федерации о противодействии коррупции"), GroupClaim = _groupClaim, Description = "Споры, возникающие по вопросам применения законодательства Российской Федерации о противодействии коррупции"}
                    };
                case 16:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "16", Name = StrTrim("Об обжаловании решений, действий (бездействий) при оказании территориальными органами ПФР и их должностными лицами государственных услуг"), GroupClaim = _groupClaim, Description = "Об обжаловании решений, действий (бездействий) при оказании территориальными органами ПФР и их должностными лицами государственных услуг" }
                    };
                case 17:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "17.1", Name = StrTrim("Иски органов прокуратуры, связанные с реализацией территориальными органами ПФР мероприятий по реализации государственной программы Российской Федерации «Доступная среда»"), GroupClaim = _groupClaim, Description = "Иски органов прокуратуры, связанные с реализацией территориальными органами ПФР мероприятий по реализации государственной программы Российской Федерации «Доступная среда»" }
                    };
                case 18:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "18", Name = StrTrim("Споры, связанные с реализацией постановления Правительства Российской Федерации от 16.07.2016  № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»"), GroupClaim = _groupClaim, Description = "Споры, связанные с реализацией постановления Правительства Российской Федерации от 16.07.2016  № 674 «О формировании и ведении федерального реестра инвалидов и об использовании содержащихся в нем сведений»" }
                    };
                case 19:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "19", Name = StrTrim("Споры, связанные с реализацией постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»"), GroupClaim = _groupClaim, Description = "Споры, связанные с реализацией постановления Правительства Российской Федерации от 14.02.2017 № 181 «О единой государственной информационной системе социального обеспечения»" }
                    };
                case 20:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "20.1", Name = StrTrim("О восстановлении на работе и оплате за время вынужденного прогула"), GroupClaim = _groupClaim, Description = "О восстановлении на работе и оплате за время вынужденного прогула" },
                        new SubjectClaim { Code = "20.2", Name = StrTrim("О взыскании заработной платы"), GroupClaim = _groupClaim, Description = "О взыскании заработной платы" },
                        new SubjectClaim { Code = "20.3", Name = StrTrim("Об оспаривании дисциплинарного взыскания"), GroupClaim = _groupClaim, Description = "Об оспаривании дисциплинарного взыскания" },
                        new SubjectClaim { Code = "20.4", Name = StrTrim("О взыскании компенсации стоимости проезда к месту отдыха и обратно работников ПФР"), GroupClaim = _groupClaim, Description = "О взыскании компенсации стоимости проезда к месту отдыха и обратно работников ПФР" },
                        new SubjectClaim { Code = "20.5", Name = StrTrim("Об обжаловании увольнения по п. 1, 2 ст. 81 ТК Российской Федерации"), GroupClaim = _groupClaim, Description = "Об обжаловании увольнения по п. 1, 2 ст. 81 ТК Российской Федерации" }
                    };
                case 21:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "21.1", Name = StrTrim("О признании торгов недействительными по иску заинтересованного лица и о применении последствий недействительности государственного контракта, заключенного по результатам таких торгов"), GroupClaim = _groupClaim, Description = "О признании торгов недействительными по иску заинтересованного лица и о применении последствий недействительности государственного контракта, заключенного по результатам таких торгов" },
                        new SubjectClaim { Code = "21.2", Name = StrTrim("О признании недействительными договоров (государственных контрактов) и применении последствий недействительной сделки"), GroupClaim = _groupClaim, Description = "О признании недействительными договоров (государственных контрактов) и применении последствий недействительной сделки" },
                        new SubjectClaim { Code = "21.3", Name = StrTrim("О расторжении договора (государственного контракта) и взыскании неустойки, штрафа, пеней за неисполнение или ненадлежащее исполнение обязательств по договору"), GroupClaim = _groupClaim, Description = "О расторжении договора (государственного контракта) и взыскании неустойки, штрафа, пеней за неисполнение или ненадлежащее исполнение обязательств по договору" },
                        new SubjectClaim { Code = "21.4", Name = StrTrim("О взыскании с заказчика сумм задолженности по оплате поставленного товара, выполненных работ, оказанных услуг по договору (государственному контракту)"), GroupClaim = _groupClaim, Description = "О взыскании с заказчика сумм задолженности по оплате поставленного товара, выполненных работ, оказанных услуг по договору (государственному контракту)" },
                        new SubjectClaim { Code = "21.5", Name = StrTrim("О взыскании с заказчика сумм неустоек (штрафов, пеней) в случае просрочки исполнения заказчиком обязательств, предусмотренных государственным контрактом, а также в иных случаях неисполнения или ненадлежащего исполнения заказчиком"), GroupClaim = _groupClaim, Description = "О взыскании с заказчика сумм неустоек (штрафов, пеней) в случае просрочки исполнения заказчиком обязательств, предусмотренных государственным контрактом, а также в иных случаях неисполнения или ненадлежащего исполнения заказчиком" },
                        new SubjectClaim { Code = "21.6", Name = StrTrim("О взыскании с заказчика процентов за пользование чужими денежными средствами, начисленных на сумму задолженности по государственному контракту, которая взыскана с ПФР на основании решения суда, вступившего в законную силу"), GroupClaim = _groupClaim, Description = "О взыскании с заказчика процентов за пользование чужими денежными средствами, начисленных на сумму задолженности по государственному контракту, которая взыскана с ПФР на основании решения суда, вступившего в законную силу" },
                        new SubjectClaim { Code = "21.7", Name = StrTrim("О взыскании неосновательного обогащения в связи с поставкой товара, выполнением работ, оказанием услуг в отсутствие государственного контракта"), GroupClaim = _groupClaim, Description = "О взыскании неосновательного обогащения в связи с поставкой товара, выполнением работ, оказанием услуг в отсутствие государственного контракта" },
                        new SubjectClaim { Code = "21.8", Name = StrTrim("О взыскании убытков (упущенной выгоды), причиненных незаконным отклонением заявки участника закупки"), GroupClaim = _groupClaim, Description = "О взыскании убытков (упущенной выгоды), причиненных незаконным отклонением заявки участника закупки" },
                        new SubjectClaim { Code = "21.9", Name = StrTrim("Взыскание по ДТП"), GroupClaim = _groupClaim, Description = "Взыскание по ДТП" },
                    };
                case 22:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "22", Name = StrTrim("Иные"), GroupClaim = _groupClaim, Description = "Иные" },
                    };
                case 23:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "23", Name = StrTrim("О взыскании средств пенсионных накоплений (накопительной  пенсии) правопреемниками умерших  застрахованных лиц ( восстановление срока для обращения за выплатой СПН)"), GroupClaim = _groupClaim, Description = "О взыскании средств пенсионных накоплений (накопительной  пенсии) правопреемниками умерших  застрахованных лиц ( восстановление срока для обращения за выплатой СПН)" },
                    };
                case 24:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "24.1", Name = StrTrim("Установление фактов, имеющих юридическое значение (территориальный орган ПФР – заинтересованное лицо)"), GroupClaim = _groupClaim, Description = "Установление фактов, имеющих юридическое значение (территориальный орган ПФР – заинтересованное лицо)" },
                        new SubjectClaim { Code = "24.2", Name = StrTrim("Иные дела, рассматриваемые судом в порядке особого производства (территориальный орган ПФР – заинтересованное лицо)"), GroupClaim = _groupClaim, Description = "Иные дела, рассматриваемые судом в порядке особого производства (территориальный орган ПФР – заинтересованное лицо)" }
                    };
                case 25:
                    return new List<SubjectClaim>
                    {
                        new SubjectClaim { Code = "25.1", Name = StrTrim("Оплата госпошлины(арбитраж)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "25.2", Name = StrTrim("Оплата услуг представителя(арбитраж)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "25.3", Name = StrTrim("Иные судебные издержки(арбитраж)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "25.4", Name = StrTrim("Оплата госпошлины(общая юрисдикция)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "25.5", Name = StrTrim("Оплата услуг представителя(общая юрисдикция)"), GroupClaim = _groupClaim, Description = "" },
                        new SubjectClaim { Code = "25.6", Name = StrTrim("Иные судебные издержки(общая юрисдикция)"), GroupClaim = _groupClaim, Description = "" }
                    };
                default:
                    return null;
            }
        }
    }
}

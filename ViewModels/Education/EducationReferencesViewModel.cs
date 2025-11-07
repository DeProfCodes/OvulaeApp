using System.Windows.Input;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.Education;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.ViewModels.Education
{
    public class EducationReferencesViewModel : BaseViewModel
    {
        public List<EducationReferenceLinkViewModel> _references;
        public List<EducationReferenceLinkViewModel> References 
        {
            get => _references;
            set
            {
                _references = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenReferenceCommand { get; }

        public EducationReferencesViewModel(IEducationService eduServ, int bookId)
        {
            References = GetAllBooksReferences();
            OpenReferenceCommand = new Command<string>(async (url) => await OpenUrlAsync(url));
        }

        private async Task OpenUrlAsync(string? url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return;

                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                }

                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    await Launcher.Default.OpenAsync(uri);
                }
            }
            catch
            {
            }
        }

        private List<EducationReferenceLinkViewModel> GetAllBooksReferences()
        {
            var references = new List<EducationReferenceLinkViewModel>()
            {
                new EducationReferenceLinkViewModel { Reference = "American Pregnancy Association. Early Signs of Pregnancy.", URL = "https://americanpregnancy.org/pregnancy-symptoms/early-signs-of-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "Merck Manual Consumer Version. Physical Changes During Pregnancy.", URL = "https://www.merckmanuals.com/home/women-s-health-issues/normal-pregnancy/physical-changes-during-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Morning Sickness: Nausea and Vomiting of Pregnancy.", URL = "https://www.acog.org/patient-resources/faqs/pregnancy/morning-sickness-nausea-and-vomiting-of-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "U.S Department of Health and Human Services, Office on Women's Health. Prenatal Care.", URL = "https://www.womenshealth.gov/a-z-topics/prenatal-care" },
                new EducationReferenceLinkViewModel { Reference = "ACOG. Fertility Awareness-Based Methods of Family Planning.", URL = "https://www.acog.org/womens-health/faqs/fertility-awareness-based-methods-of-family-planning" },
                new EducationReferenceLinkViewModel { Reference = "American Pregnancy Association. What is Ovulation?", URL = "https://americanpregnancy.org/getting-pregnant/infertility/understanding-ovulation/" },
                new EducationReferenceLinkViewModel { Reference = "American Society of Reproductive Medicine. Ovulation Detection & Optimizing Natural Fertility.", URL = "https://www.reproductivefacts.org/news-and-publications/patient-fact-sheets-and-booklets/documents/fact-sheets-and-info-booklets/ovulation-detection/" },
                new EducationReferenceLinkViewModel { Reference = "Holesh JE, Bass AN, Lord M. Physiology, Ovulation. 2022 May 8. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2022 Jan-. PMID: 28723025.", URL = "https://pubmed.ncbi.nlm.nih.gov/28723025/" },
                new EducationReferenceLinkViewModel { Reference = "National Health Service. How can I tell when I'm ovulating?", URL = "https://www.nhs.uk/common-health-questions/womens-health/how-can-i-tell-when-i-am-ovulating/" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health: U.S. Dept of Health. Trying to conceive & Your menstrual cycle.", URL = "https://www.womenshealth.gov/pregnancy/you-get-pregnant/trying-conceive" },
                new EducationReferenceLinkViewModel { Reference = "Abnormal Bleeding During Your Period. Am Fam Physician. 2012 Jan 1;85(1):44.", URL = "https://www.aafp.org/pubs/afp/issues/2012/0101/p44.html" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Abnormal Uterine Bleeding.", URL = "https://www.acog.org/Patients/FAQs/Abnormal-Uterine-Bleeding" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health and Human Services. Eunice Kennedy Shriver National Institute of Child Health and Human Development. About Menstruation.", URL = "https://www.nichd.nih.gov/health/topics/menstruation/conditioninfo" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health and Human Services. Office on Women's Health. Menstrual Cycle.", URL = "https://www.womenshealth.gov/menstrual-cycle" },
                new EducationReferenceLinkViewModel { Reference = "Carugno J, Fatehi M. Abdominal Hysterectomy. 2023 Jul 18. In: StatPearls. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://pubmed.ncbi.nlm.nih.gov/33232036/" },
                new EducationReferenceLinkViewModel { Reference = "Committee Opinion No 701: Choosing the Route of Hysterectomy for Benign Disease. Obstet Gynecol. 2017;129(6):e155-e159.", URL = "https://pubmed.ncbi.nlm.nih.gov/28538495/" },
                new EducationReferenceLinkViewModel { Reference = "InformedHealth.org [Internet]. Cologne, Germany: Institute for Quality and Efficiency in Health Care (IQWiG); 2006-. Hysterectomy (surgical removal of the womb).", URL = "https://www.ncbi.nlm.nih.gov/books/NBK525761/" },
                new EducationReferenceLinkViewModel { Reference = "Kahn RM, Gordhandas S, Godwin K, Stone RL, Worley MJ Jr, et al. Salpingectomy for the Primary Prevention of Ovarian Cancer: A Systematic Review. JAMA Surg. 2023 Nov 1;158(11):1204-1211.", URL = "https://pubmed.ncbi.nlm.nih.gov/37672283/" },
                new EducationReferenceLinkViewModel { Reference = "National Women's Health Network. Hysterectomy. Publication date 11/2022.", URL = "https://nwhn.org/hysterectomy/" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health (U.S.). Hysterectomy. Last updated 12/2022.", URL = "https://www.womenshealth.gov/a-z-topics/hysterectomy" },
                new EducationReferenceLinkViewModel { Reference = "Pillarisetty LS, Mahdy H. Vaginal Hysterectomy. 2024 Apr 24. In: StatPearls. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://pubmed.ncbi.nlm.nih.gov/32119369/" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Opinion 701. Choosing the Route of Hysterectomy for Benign Disease.", URL = "https://www.acog.org/clinical/clinical-guidance/committee-opinion/articles/2017/06/choosing-the-route-of-hysterectomy-for-benign-disease" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Hysterectomy. Last updated 1/2021.", URL = "https://www.acog.org/womens-health/faqs/hysterectomy" },
                new EducationReferenceLinkViewModel { Reference = "American College of Obstetricians and Gynecologists. Combined Hormonal Birth Control: Pill, Patch and Ring.", URL = "https://www.acog.org/patient-resources/faqs/contraception/combined-hormonal-birth-control-pill-patch-and-ring" },
                new EducationReferenceLinkViewModel { Reference = "American Academy of Family Physicians. Progestin-Only Birth Control Pills.", URL = "https://familydoctor.org/progestin-only-birth-control-pills/" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention. Reproductive Health. Contraception.", URL = "https://www.cdc.gov/reproductivehealth/contraception/index.htm" },
                new EducationReferenceLinkViewModel { Reference = "Cooper DB, Patel P, Mahdy H. Oral Contraceptive Pills. November 24, 2022. In: StatPearls. Treasure Island (FL): StatPearls Publishing; Jan 2023-.", URL = "https://pubmed.ncbi.nlm.nih.gov/28613632/" },
                new EducationReferenceLinkViewModel { Reference = "Department of Health and Human Services (HHS) Office of Population Affairs. Contraception and Preventing Pregnancy.", URL = "https://opa.hhs.gov/reproductive-health/preventing-pregnancy-contraception" },
                new EducationReferenceLinkViewModel { Reference = "Planned Parenthood. Birth Control Pill.", URL = "https://www.plannedparenthood.org/learn/birth-control/birth-control-pill" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention (U.S.). Human papillomavirus (HPV). Multiple pages reviewed. Last updated 7/2021.", URL = "https://www.cdc.gov/hpv/index.html" },
                new EducationReferenceLinkViewModel { Reference = "Chen CP, Kung PT, Wang YH, Tsai WC. Effect of time interval from diagnosis to treatment for cervical cancer on survival: A nationwide cohort study. PLoS One. 2019 Sep 4;14(9):e0221946.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6726236/" },
                new EducationReferenceLinkViewModel { Reference = "Luria L, Cardoza-Favarato G. Human Papillomavirus. 2023 Jan 16. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan-.", URL = "https://pubmed.ncbi.nlm.nih.gov/28846281/" },
                new EducationReferenceLinkViewModel { Reference = "Malagón T, Louvanto K, Wissing M, Burchell AN, et al. Hand-to-genital and genital-to-genital transmission of human papillomaviruses between male and female sexual partners (HITCH): a prospective cohort study. Lancet Infect Dis. 2019 Mar;19(3):317-326.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6404546/" },
                new EducationReferenceLinkViewModel { Reference = "National Cancer Institute (U.S). Human Papillomavirus (HPV) Vaccines. Last updated 5/2021.", URL = "https://www.cancer.gov/about-cancer/causes-prevention/risk/infectious-agents/hpv-vaccine-fact-sheet" },
                new EducationReferenceLinkViewModel { Reference = "Okunade KS. Human papillomavirus and cervical cancer. J Obstet Gynaecol. 2020 Jul;40(5):602-608.", URL = "https://pubmed.ncbi.nlm.nih.gov/31500479/" },
                new EducationReferenceLinkViewModel { Reference = "Quinlan JD. Human Papillomavirus: Screening, Testing, and Prevention. Am Fam Physician. 2021 Aug 1;104(2):152-159.", URL = "https://pubmed.ncbi.nlm.nih.gov/34383440/" },
                new EducationReferenceLinkViewModel { Reference = "Williamson AL. Recent Developments in Human Papillomavirus (HPV) Vaccinology. Viruses. 2023 Jun 26;15(7):1440.", URL = "https://pubmed.ncbi.nlm.nih.gov/37515128/" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention (U.S.). Screening for Cervical Cancer. Last updated 10/2023.", URL = "https://www.cdc.gov/cervical-cancer/screening/" },
                new EducationReferenceLinkViewModel { Reference = "Kitchen FL, Cox CM. Papanicolaou Smear. 2022 Oct 17. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://pubmed.ncbi.nlm.nih.gov/29262086/" },
                new EducationReferenceLinkViewModel { Reference = "Swailes AL, Hossler CE, Kesterson JP. Pathway to the Papanicolaou smear: The development of cervical cytology in twentieth-century America and implications in the present day. Gynecol Oncol. 2019 Jul;154(1):3-7.", URL = "https://pubmed.ncbi.nlm.nih.gov/30995961/" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health (U.S.). Pap and HPV Tests. Last updated 2/2021.", URL = "https://www.womenshealth.gov/a-z-topics/pap-hpv-tests" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Cervical Cancer Screening. Last reviewed 5/2023.", URL = "https://www.acog.org/womens-health/faqs/cervical-cancer-screening" },
                new EducationReferenceLinkViewModel { Reference = "InformedHealth.org [Internet]. Cologne, Germany: Institute for Quality and Efficiency in Health Care (IQWiG); 2006-. Overview: Vaginal yeast infection (thrush).", URL = "https://www.ncbi.nlm.nih.gov/books/NBK543220/#_ncbi_dlg_citbx_NBK543220" },
                new EducationReferenceLinkViewModel { Reference = "Nyirjesy P, Brookhart C, Lazenby G, Schwebke J, Sobel JD. Vulvovaginal Candidiasis: A Review of the Evidence for the 2021 Centers for Disease Control and Prevention of Sexually Transmitted Infections Treatment Guidelines. Clin Infect Dis. 2022 Apr 13;74(Suppl_2):S162-S168.", URL = "https://pubmed.ncbi.nlm.nih.gov/35416967/" },
                new EducationReferenceLinkViewModel { Reference = "R AN, Rafiq NB. Candidiasis. 2023 May 29. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2025 Jan-.", URL = "https://pubmed.ncbi.nlm.nih.gov/32809459/" },
                new EducationReferenceLinkViewModel { Reference = "Satora M, Grunwald A, et al. Treatment of Vulvovaginal Candidiasis-An Overview of Guidelines and the Latest Treatment Methods. J Clin Med. 2023 Aug 18;12(16):5376.", URL = "https://pmc.ncbi.nlm.nih.gov/articles/PMC10455317/" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health & Human Services. Office on Women's Health. Vaginal yeast infections. Last updated 2/2025.", URL = "https://womenshealth.gov/a-z-topics/vaginal-yeast-infections" },
                new EducationReferenceLinkViewModel { Reference = "Chen C, Gong X, Yang X, et al. The roles of estrogen and estrogen receptors in gastrointestinal disease. Oncol Lett. 2019;18(6):5673-5680.", URL = "https://pubmed.ncbi.nlm.nih.gov/31788039/" },
                new EducationReferenceLinkViewModel { Reference = "Chidi-Ogbolu N, Baar K. Effect of estrogen on musculoskeletal performance and injury risk. Front Physiol. 2019;9:1834.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6341375/" },
                new EducationReferenceLinkViewModel { Reference = "Erol A, Ho AM, Winham SJ, Karpyak VM. Sex hormones in alcohol consumption: a systematic review of evidence. Addict Biol. 2019;24(2):157-169.", URL = "https://pubmed.ncbi.nlm.nih.gov/29280252/" },
                new EducationReferenceLinkViewModel { Reference = "Fuentes N, Silveyra P. Estrogen receptor signaling mechanisms. Adv Protein Chem Struct Biol. 2019;116:135-170.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6533072/" },
                new EducationReferenceLinkViewModel { Reference = "PDQ® Screening and Prevention Editorial Board. PDQ Breast Cancer Prevention. Bethesda, MD: National Cancer Institute.", URL = "https://www.cancer.gov/types/breast/patient/breast-prevention-pdq" },
                new EducationReferenceLinkViewModel { Reference = "Schulster M, Bernie AM, Ramasamy R. The role of estradiol in male reproductive function. Asian J Androl. 2016;18(3):435-440.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4854098/" },
                new EducationReferenceLinkViewModel { Reference = "Breastcancer.org. Using HRT (Hormone Replacement Therapy). Last updated 1/2024.", URL = "https://www.breastcancer.org/risk/risk-factors/using-hormone-replacement-therapy" },
                new EducationReferenceLinkViewModel { Reference = "Harper-Harrison G, Shanahan MM. Hormone Replacement Therapy. 2023 Feb 20. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK493191/" },
                new EducationReferenceLinkViewModel { Reference = "Manson JE, Bassuk SS, Kaunitz AM, Pinkerton JV. The Women's Health Initiative trials of menopausal hormone therapy: lessons learned. Menopause. 2020;27(8):918-928.", URL = "https://pubmed.ncbi.nlm.nih.gov/32345788/" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Hormone Therapy for Menopause. Last reviewed 8/2022.", URL = "https://www.acog.org/womens-health/faqs/hormone-therapy-for-menopause" },
                new EducationReferenceLinkViewModel { Reference = "The North American Menopause Society. Hormone Therapy: Benefits & Risks. The Experts Do Agree About Hormone Therapy.", URL = "https://www.menopause.org/for-women/menopauseflashes/menopause-symptoms-and-treatments/hormone-therapy-benefits-risks" },
                new EducationReferenceLinkViewModel { Reference = "Valdes A, Bajaj T. Estrogen Therapy. 2023 May 22. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK541051/" },
                new EducationReferenceLinkViewModel { Reference = "Cortessis VK, Barrett M, Brown Wade N, et al. Intrauterine device use and cervical cancer risk: a systematic review and meta-analysis. Obstet Gynecol. 2017;130(6):1226-1236.", URL = "https://pubmed.ncbi.nlm.nih.gov/29112647/" },
                new EducationReferenceLinkViewModel { Reference = "Kavanaugh ML, Jerman J. Contraceptive method use in the United States: trends and characteristics between 2008, 2012 and 2014. Contraception. 2018;97(1):14-21.", URL = "https://pubmed.ncbi.nlm.nih.gov/29038071/" },
                new EducationReferenceLinkViewModel { Reference = "Soini T, Hurskainen R, Grénman S, Mäenpää J, Paavonen J, Pukkala E. Cancer risk in women using the levonorgestrel-releasing intrauterine system in Finland. Obstet Gynecol. 2014;124(2 Pt 1):292-299.", URL = "https://pubmed.ncbi.nlm.nih.gov/25004338/" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Food and Drug Administration. FDA-Approved Drugs. Multiple pages reviewed.", URL = "https://www.accessdata.fda.gov/scripts/cder/daf/index.cfm" },
                new EducationReferenceLinkViewModel { Reference = "Wheeler LJ, Desanto K, Teal SB, Sheeder J, Guntupalli SR. Intrauterine device use and ovarian cancer risk: a systematic review and meta-analysis. Obstet Gynecol. 2019;134(4):791-800.", URL = "https://pubmed.ncbi.nlm.nih.gov/31503144/" },
                new EducationReferenceLinkViewModel { Reference = "American Academy of Pediatrics. Warning Signs of Breastfeeding Problems. Last updated 3/25/2024.", URL = "https://www.healthychildren.org/English/ages-stages/baby/breastfeeding/Pages/Warning-Signs-of-Breastfeeding-Problems.aspx" },
                new EducationReferenceLinkViewModel { Reference = "Berens P, Labbok M; Academy of Breastfeeding Medicine. ABM Clinical Protocol #13: Contraception During Breastfeeding, Revised 2015. Breastfeed Med. 2015 Jan-Feb;10(1):3-12.", URL = "https://pubmed.ncbi.nlm.nih.gov/25551519/" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention (U.S.). Maternal Diet and Breastfeeding. Updated 2/9/2024.", URL = "https://www.cdc.gov/breastfeeding-special-circumstances/hcp/diet-micronutrients/maternal-diet.html" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention (U.S.). Mercury and Breastfeeding. Updated 2/12/2024.", URL = "https://www.cdc.gov/breastfeeding-special-circumstances/hcp/exposures/mercury.html" },
                new EducationReferenceLinkViewModel { Reference = "Harris M, Schiff DM, Saia K, Muftu S, Standish KR, Wachman EM. Academy of Breastfeeding Medicine Clinical Protocol #21: Breastfeeding in the Setting of Substance Use and Substance Use Disorder (Revised 2023). Breastfeed Med. 2023 Oct;18(10):715-733.", URL = "https://pubmed.ncbi.nlm.nih.gov/37856658/" },
                new EducationReferenceLinkViewModel { Reference = "Meek JY, Noble L; Section on Breastfeeding. Policy Statement: Breastfeeding and the Use of Human Milk. Pediatrics. 2022 Jul;150(1):e2022057988.", URL = "https://pubmed.ncbi.nlm.nih.gov/35921640/" },
                new EducationReferenceLinkViewModel { Reference = "American Family Physician. Nausea and Vomiting in Early Pregnancy.", URL = "https://www.aafp.org/afp/2015/0915/p516.html" },
                new EducationReferenceLinkViewModel { Reference = "American Pregnancy Association. Morning Sickness.", URL = "https://americanpregnancy.org/healthy-pregnancy/pregnancy-health-wellness/morning-sickness-during-pregnancy/" },
                new EducationReferenceLinkViewModel { Reference = "March of Dimes. Morning Sickness.", URL = "https://www.marchofdimes.org/pregnancy/morning-sickness.aspx" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Morning Sickness: Nausea and Vomiting of Pregnancy.", URL = "https://www.acog.org/womens-health/faqs/morning-sickness-nausea-and-vomiting-of-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "U.K. National Health Service. Vomiting and morning sickness.", URL = "https://www.nhs.uk/pregnancy/related-conditions/common-symptoms/vomiting-and-morning-sickness/" },
                new EducationReferenceLinkViewModel { Reference = "Hormone Health Network. Women's Health.", URL = "https://www.hormone.org/your-health-and-hormones/womens-health" },
                new EducationReferenceLinkViewModel { Reference = "McLaughlin MB, Jialal I. Biochemistry, Hormones. [Updated 2021 Jul 22]. In: StatPearls [Internet]. Treasure Island, FL: StatPearls Publishing; 2021 Jan.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK541112/" },
                new EducationReferenceLinkViewModel { Reference = "Society for Endocrinology. Hormones.", URL = "https://www.yourhormones.info/hormones/" },
                new EducationReferenceLinkViewModel { Reference = "Bulun SE, Yilmaz BD, Sison C, Miyazaki K, et al. Endometriosis. Endocr Rev. 2019 Aug 1;40(4):1048-1079.", URL = "https://pubmed.ncbi.nlm.nih.gov/30994890/" },
                new EducationReferenceLinkViewModel { Reference = "Cousins FL, McKinnon BD, Mortlock S, Fitzgerald HC, et al. New concepts on the etiology of endometriosis. J Obstet Gynaecol Res. 2023 Apr;49(4):1090-1105.", URL = "https://pubmed.ncbi.nlm.nih.gov/36746607/" },
                new EducationReferenceLinkViewModel { Reference = "Freytag D, Mettler L, Maass N, Günther V, Alkatout I. Uterine anomalies and endometriosis. Minerva Med. 2020 Feb;111(1):33-49.", URL = "https://pubmed.ncbi.nlm.nih.gov/31755672/" },
                new EducationReferenceLinkViewModel { Reference = "Horne AW, Missmer SA. Pathophysiology, diagnosis, and management of endometriosis. BMJ. 2022 Nov 14;379:e070750.", URL = "https://pubmed.ncbi.nlm.nih.gov/36375827/" },
                new EducationReferenceLinkViewModel { Reference = "Kalaitzopoulos DR, Samartzis N, Kolovos GN, Mareti E, et al. Treatment of endometriosis: a review with comparison of 8 guidelines. BMC Womens Health. 2021 Nov 29;21(1):397.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8628449/" },
                new EducationReferenceLinkViewModel { Reference = "Merck Manual Professional Version (U.S.). Endometriosis. Last reviewed 4/2024.", URL = "https://www.merckmanuals.com/professional/gynecology-and-obstetrics/endometriosis/endometriosis" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health; U.S. Dept. of Health & Human Services. Endometriosis. Last updated 2/2021.", URL = "https://www.womenshealth.gov/a-z-topics/endometriosis" },
                new EducationReferenceLinkViewModel { Reference = "Saunders PTK, Horne AW. Endometriosis: Etiology, pathobiology, and therapeutic prospects. Cell. 2021 May 27;184(11):2807-2824.", URL = "https://pubmed.ncbi.nlm.nih.gov/34048704/" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Endometriosis. Last updated 2/2021.", URL = "https://www.acog.org/womens-health/faqs/endometriosis" },
                new EducationReferenceLinkViewModel { Reference = "Tsamantioti ES, Mahdy H. Endometriosis. 2023 Jan 23. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan-.", URL = "https://pubmed.ncbi.nlm.nih.gov/33620854/" },
                new EducationReferenceLinkViewModel { Reference = "Chang JG, Lewis MN, Wertz MC. Managing Menopausal Symptoms: Common Questions and Answers. Am Fam Physician. 2023;108(1):28-39.", URL = "https://pubmed.ncbi.nlm.nih.gov/37440735/" },
                new EducationReferenceLinkViewModel { Reference = "Koothirezhi R, Ranganathan S. Postmenopausal Syndrome. 2023 Apr 24. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2024 Jan.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK560840/" },
                new EducationReferenceLinkViewModel { Reference = "National Health Service (UK). Postmenopausal Bleeding. Last reviewed 5/2023.", URL = "https://www.nhs.uk/conditions/post-menopausal-bleeding/" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health (U.S.). Menopause. Last updated 2/2021.", URL = "https://www.womenshealth.gov/menopause" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. The Menopause Years. Last reviewed 11/2023.", URL = "https://www.acog.org/womens-health/faqs/the-menopause-years" },
                new EducationReferenceLinkViewModel { Reference = "American College of Obstetricians and Gynecologists. Polycystic Ovary Syndrome (PCOS).", URL = "https://www.acog.org/womens-health/faqs/polycystic-ovary-syndrome-pcos" },
                new EducationReferenceLinkViewModel { Reference = "Deswal R, Narwal V, Dang A, Pundir CS. The Prevalence of Polycystic Ovary Syndrome: A Brief Systematic Review. J Hum Reprod Sci. 2020 Oct-Dec;13(4):261-271.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7879843/" },
                new EducationReferenceLinkViewModel { Reference = "González F. Inflammation in Polycystic Ovary Syndrome: underpinning of insulin resistance and ovarian dysfunction. Steroids. 2012;77(4):300-305.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3309040" },
                new EducationReferenceLinkViewModel { Reference = "PCOS Awareness Association. What is Polycystic ovary syndrome (PCOS)?", URL = "https://www.pcosaa.org/overview" },
                new EducationReferenceLinkViewModel { Reference = "U.K. National Health Service. Treatment: Polycystic Ovary Syndrome.", URL = "https://www.nhs.uk/conditions/polycystic-ovary-syndrome-pcos/treatment/" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health & Human Services, Office of Women's Health. Polycystic Ovary Syndrome.", URL = "https://www.womenshealth.gov/a-z-topics/polycystic-ovary-syndrome" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health & Human Services, Office of Women's Health. Polycystic ovary syndrome (PCOS) Fact Sheet.", URL = "https://owh-wh-d9-dev.s3.amazonaws.com/s3fs-public/documents/fact-sheet-pcos.pdf" },
                new EducationReferenceLinkViewModel { Reference = "U.S. National Institute of Health, Eunice Kennedy Shriver National Institute of Child Health and Human Development. Does PCOS affect pregnancy?", URL = "https://www.nichd.nih.gov/health/topics/pcos/more_information/FAQs/pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "American Urological Association. What is a Urinary Tract Infection (UTI) in Adults?", URL = "https://www.urologyhealth.org/urology-a-z/u/urinary-tract-infections-in-adults" },
                new EducationReferenceLinkViewModel { Reference = "Garofalo CK, Hooton TM, Martin SM, et al. Escherichia coli from urine of female patients with urinary tract infections is competent for intracellular bacterial community formation. Infect Immun. 2007;75(1):52-60.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC1828379/" },
                new EducationReferenceLinkViewModel { Reference = "Merck Manual, Consumer Version. Overview of Urinary Tract Infections (UTIs).", URL = "https://www.merckmanuals.com/home/kidney-and-urinary-tract-disorders/urinary-tract-infections-utis/overview-of-urinary-tract-infections-utis" },
                new EducationReferenceLinkViewModel { Reference = "National Institute of Diabetes and Digestive and Kidney Diseases. Definition & Facts of Bladder Infection in Adults.", URL = "https://www.niddk.nih.gov/health-information/urologic-diseases/bladder-infection-uti-in-adults/definition-facts" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Centers for Disease Control and Prevention. Urinary Tract Infection.", URL = "https://www.cdc.gov/antibiotic-use/uti.html" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health and Human Services, Office of Population Affairs. Urinary Tract Infection (UTI).", URL = "https://opa.hhs.gov/opa/reproductive-health/fact-sheets/urinary-tract-infections/index.html" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health and Human Services, Office on Women's Health. Urinary Tract Infections.", URL = "https://www.womenshealth.gov/a-z-topics/urinary-tract-infections" },
                new EducationReferenceLinkViewModel { Reference = "Merck Manual Professional Version. Uterine Fibroids.", URL = "https://www.merckmanuals.com/professional/gynecology-and-obstetrics/uterine-fibroids/uterine-fibroids" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Uterine Fibroids.", URL = "https://www.acog.org/womens-health/faqs/uterine-fibroids" },
                new EducationReferenceLinkViewModel { Reference = "US Department of Health and Human Services, Eunice Kennedy Shriver National Institute of Child Health and Human Development. Uterine Fibroids.", URL = "https://www.nichd.nih.gov/health/topics/uterine" },
                new EducationReferenceLinkViewModel { Reference = "US Department of Health and Human Services, Office on Women's Health. Uterine fibroids.", URL = "https://www.womenshealth.gov/a-z-topics/uterine-fibroids" },
                new EducationReferenceLinkViewModel { Reference = "US Food & Drug Administration. Uterine Fibroids.", URL = "https://www.fda.gov/consumers/womens-health-topics/uterine-fibroids" },
                new EducationReferenceLinkViewModel { Reference = "Bone Health and Osteoporosis Foundation. What is Osteoporosis?", URL = "https://www.bonehealthandosteoporosis.org/patients/" },
                new EducationReferenceLinkViewModel { Reference = "International Osteoporosis Foundation. Osteoporosis & Musculoskeletal Disorders.", URL = "https://www.iofbonehealth.org/osteoporosis-musculoskeletal-disorders" },
                new EducationReferenceLinkViewModel { Reference = "National Institute of Arthritis and Musculoskeletal Health and Skin Diseases (U.S.). Osteoporosis Overview.", URL = "https://www.bones.nih.gov/health-info/bone/osteoporosis/overview" },
                new EducationReferenceLinkViewModel { Reference = "National Library of Medicine (U.S.). Osteoporosis.", URL = "https://medlineplus.gov/osteoporosis.html" },
                new EducationReferenceLinkViewModel { Reference = "Cervical Cancer Guidelines: Recommendations for Practice (May 2020) British Gynaecological Cancer Society (BGCS)" },
                new EducationReferenceLinkViewModel { Reference = "Ask ACOG: Is it safe to have sex during pregnancy? American College of Obstetricians and Gynecologists.", URL = "https://www.acog.org/womens-health/experts-and-stories/ask-acog/is-it-safe-to-have-sex-during-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "Sex during pregnancy. Journal of Midwifery & Women's Health. 2022; doi:10.1111/jmwh.13351." },
                new EducationReferenceLinkViewModel { Reference = "Sex in pregnancy. National Health Service.", URL = "https://www.nhs.uk/pregnancy/keeping-well/sex/" },
                new EducationReferenceLinkViewModel { Reference = "FAQs: Early pregnancy loss. American College of Obstetricians and Gynecologists.", URL = "https://www.acog.org/womens-health/faqs/early-pregnancy-loss" },
                new EducationReferenceLinkViewModel { Reference = "Lockwood CJ, et al. Prenatal care: Patient education, health promotion, and safety of commonly used drugs.", URL = "https://www.uptodate.com/contents/search" },
                new EducationReferenceLinkViewModel { Reference = "Leite AP, Campos AA, Dias AR, Amed AM, De Souza E, Camano L. Prevalence of sexual dysfunction during pregnancy. Rev Assoc Med Bras. 2009;55(5):563–568. doi: 10.1590/s0104-42302009000500020." },
                new EducationReferenceLinkViewModel { Reference = "Bartellas E, Crane JM, Daley M, Bennett KA, Hutchens D. Sexuality and sexual activity in pregnancy. BJOG. 2000;107(8):964–968. doi: 10.1111/j.1471-0528.2000.tb10397.x." },
                new EducationReferenceLinkViewModel { Reference = "Shojaa M, Jouybari L, Sanagoo A. The sexual activity during pregnancy among a group of Iranian women. Arch Gynecol Obstet. 2009;279(3):353–356. doi: 10.1007/s00404-008-0735-z." },
                new EducationReferenceLinkViewModel { Reference = "Malarewicz A, Szymkiewicz J, Rogala J. Sexuality of pregnant women. Ginekol Pol. 2006;77(9):733–739." },
                new EducationReferenceLinkViewModel { Reference = "National Health Service (U.K.). 20-week screening scan. Last reviewed 10/2024.", URL = "https://www.nhs.uk/pregnancy/your-pregnancy-care/20-week-scan/" },
                new EducationReferenceLinkViewModel { Reference = "The American College of Obstetricians and Gynecologists. Ultrasound Exams. Last reviewed 1/2024.", URL = "https://www.acog.org/womens-health/faqs/ultrasound-exams" },
                new EducationReferenceLinkViewModel { Reference = "Ulrich CC, Dewald O. Pregnancy Ultrasound Evaluation (Archived). 2023 Feb 13. In: StatPearls [Internet]. Treasure Island (FL): StatPearls Publishing; 2025 Jan-.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK557572/" },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention. Facts about Down Syndrome.", URL = "https://www.cdc.gov/ncbddd/birthdefects/downsyndrome.html" },
                new EducationReferenceLinkViewModel { Reference = "Cuckle HS. Primary Prevention of Down's Syndrome. Int J Med Sci. 2005; 2(3):93–99.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC1168873/" },
                new EducationReferenceLinkViewModel { Reference = "March of Dimes. Down Syndrome.", URL = "https://www.marchofdimes.org/complications/down-syndrome.aspx" },
                new EducationReferenceLinkViewModel { Reference = "National Down Syndrome Society. About Down Syndrome.", URL = "https://ndss.org/about" },
                new EducationReferenceLinkViewModel { Reference = "National Library of Medicine. Down Syndrome.", URL = "https://medlineplus.gov/genetics/condition/down-syndrome/#inheritance" },
                new EducationReferenceLinkViewModel { Reference = "U.S. Department of Health and Human Services. Down Syndrome.", URL = "https://www.nichd.nih.gov/health/topics/down/conditioninfo" },
                new EducationReferenceLinkViewModel { Reference = "Down syndrome. Genetics and Rare Diseases Information Center." },
                new EducationReferenceLinkViewModel { Reference = "Down syndrome (Trisomy 21). Merck Manual Professional Version." },
                new EducationReferenceLinkViewModel { Reference = "Bull MJ. Down syndrome. New England Journal of Medicine. 2020; doi:10.1056/NEJMra1706537." },
                new EducationReferenceLinkViewModel { Reference = "Antonarakis SE, et al. Down syndrome. Nature Reviews: Disease Primer. 2020; doi:10.1038/s41572-019-0143-7." },
                new EducationReferenceLinkViewModel { Reference = "Tsou AY, et al. Medical care of adults with Down syndrome: A clinical guideline. JAMA. 2020; doi:10.1001/jama.2020.17024." },
                new EducationReferenceLinkViewModel { Reference = "Down syndrome: Guidelines for inclusive education. National Down Syndrome Society.", URL = "https://ndss.org/inclusive-education-guidelines" },
                new EducationReferenceLinkViewModel { Reference = "Early intervention. National Down Syndrome Society.", URL = "https://ndss.org/resources/early-intervention" },
                new EducationReferenceLinkViewModel { Reference = "Medical review (expert opinion). Mayo Clinic. Sept. 24, 2024." },
                new EducationReferenceLinkViewModel { Reference = "Lockwood CJ. Preterm labor: Clinical findings, diagnostic evaluation, and initial treatment.", URL = "https://www.uptodate.com/contents/search" },
                new EducationReferenceLinkViewModel { Reference = "Landon MB, et al., eds. Preterm labor and birth. In: Gabbe's Obstetrics: Normal and Problem Pregnancies. 8th ed. Elsevier; 2021.", URL = "https://www.clinicalkey.com" },
                new EducationReferenceLinkViewModel { Reference = "AskMayoExpert. Preterm labor. Mayo Clinic; 2024." },
                new EducationReferenceLinkViewModel { Reference = "FAQs: Preterm labor and birth. American College of Obstetricians and Gynecologists.", URL = "https://www.acog.org/womens-health/faqs/preterm-labor-and-birth" },
                new EducationReferenceLinkViewModel { Reference = "Dagklis T, et al. Management of preterm labor: Clinical practice guideline and recommendation by the WAPM and PMF. Eur J Obstet Gynecol Reprod Biol. 2023; doi:10.1016/j.ejogrb.2023.10.013." },
                new EducationReferenceLinkViewModel { Reference = "Asgharnia M, et al. Inter-pregnancy interval and incidence of preterm birth. J Fam Reprod Health. 2020; doi:10.18502/jfrh.v14i1.3788." },
                new EducationReferenceLinkViewModel { Reference = "Biggio J, et al. SMFM consult series #70: Management of short cervix in individuals without a history of preterm birth. SMFM. 2024; doi:10.1016/j.ajog.2024.05.006." },
                new EducationReferenceLinkViewModel { Reference = "Centers for Disease Control and Prevention. Bacterial Vaginosis – CDC Basic Fact Sheet.", URL = "https://www.cdc.gov/std/bv/stdfact-bacterial-vaginosis.htm" },
                new EducationReferenceLinkViewModel { Reference = "Kairys N, Garg M. Bacterial Vaginosis. StatPearls. Updated 2022 Jul 4.", URL = "https://www.ncbi.nlm.nih.gov/books/NBK459216/" },
                new EducationReferenceLinkViewModel { Reference = "March of Dimes. Bacterial vaginosis and pregnancy.", URL = "https://www.marchofdimes.org/find-support/topics/pregnancy/bacterial-vaginosis-and-pregnancy" },
                new EducationReferenceLinkViewModel { Reference = "Office on Women's Health. Bacterial vaginosis.", URL = "https://www.womenshealth.gov/a-z-topics/bacterial-vaginosis" },
                new EducationReferenceLinkViewModel { Reference = "Planned Parenthood. What is bacterial vaginosis?", URL = "https://www.plannedparenthood.org/learn/health-and-wellness/vaginitis/what-bacterial-vaginosis" },
                new EducationReferenceLinkViewModel { Reference = "Ferri FF. Pelvic organ prolapse. In: Ferri's Clinical Advisor 2022. Elsevier; 2022.", URL = "https://www.clinicalkey.com" },
                new EducationReferenceLinkViewModel { Reference = "Rogers RG, et al. Pelvic organ prolapse in females: Epidemiology, risk factors, manifestations, and management.", URL = "https://www.uptodate.com/contents/search" },
                new EducationReferenceLinkViewModel { Reference = "AskMayoExpert. Pelvic organ prolapse (adult). Mayo Clinic; 2022." },
                new EducationReferenceLinkViewModel { Reference = "Hoffman BL, et al. Pelvic organ prolapse. In: Williams Gynecology. 4th ed. McGraw Hill; 2020.", URL = "https://accessmedicine.mhmedical.com" },
                new EducationReferenceLinkViewModel { Reference = "Uterine and apical prolapse. Merck Manual Professional Version.", URL = "https://www.merckmanuals.com/professional/gynecology-and-obstetrics/pelvic-organ-prolapse-pop/uterine-and-apical-prolapse#" },
                new EducationReferenceLinkViewModel { Reference = "Nguyen H. Allscripts EPSi. Mayo Clinic. April 27, 2022." },
                new EducationReferenceLinkViewModel { Reference = "Kegel exercises. National Institute of Diabetes and Digestive and Kidney Diseases.", URL = "https://www.niddk.nih.gov/health-information/urologic-diseases/kegel-exercises" },
                new EducationReferenceLinkViewModel { Reference = "Jelovsek JE. Pelvic organ prolapse in women: Choosing a primary surgical procedure.", URL = "https://www.uptodate.com/contents/search" },
                new EducationReferenceLinkViewModel { Reference = "Medical review (expert opinion). Mayo Clinic. July 2, 2022." },
                new EducationReferenceLinkViewModel { Reference = "Continence Foundation of Australia. Pelvic Organ Prolapse. Last updated 5/2024.", URL = "https://www.continence.org.au/who-it-affects/women/prolapse" },
                new EducationReferenceLinkViewModel { Reference = "Iglesia CB, Smithling KR. Pelvic Organ Prolapse. Am Fam Physician. 2017;96(3):179-185.", URL = "https://pubmed.ncbi.nlm.nih.gov/28762694/" },
                new EducationReferenceLinkViewModel { Reference = "Ko KJ, Lee KS. Current surgical management of pelvic organ prolapse. Investig Clin Urol. 2019;60(6):413-424.", URL = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6821990/" },
                new EducationReferenceLinkViewModel { Reference = "Dang NT, Mukai R, Yoshida K, Ashida H. D-pinitol and myo-inositol stimulate translocation of glucose transporter 4 in skeletal muscle of C57BL/6 mice. Biosci Biotechnol Biochem. 2010;74(5):1062-1067." },
                new EducationReferenceLinkViewModel { Reference = "Clements RS Jr, Darnell B. Myo-inositol content of common foods: development of a high-myo-inositol diet. Am J Clin Nutr. 1980;33(9):1954-1967." },
                new EducationReferenceLinkViewModel { Reference = "Dinicola S, Minini M, Unfer V, et al. Nutritional and acquired deficiencies in inositol bioavailability. Int J Mol Sci. 2017;18(10):2187." },
                new EducationReferenceLinkViewModel { Reference = "Gonzalez-Uarquin F, Rodehutscord M, Huber K. Myo-inositol: its metabolism and implications for poultry nutrition. Poult Sci. 2020;99(2):893-905." },
                new EducationReferenceLinkViewModel { Reference = "Chang HH, Choong B, Phillips AR, Loomes KM. The diabetic rat kidney mediates inosituria and urinary partitioning of D-chiro-inositol. Exp Biol Med (Maywood). 2015;240(1):8-14." },
                new EducationReferenceLinkViewModel { Reference = "Holub BJ. The nutritional importance of inositol and the phosphoinositides. N Engl J Med. 1992;326(19):1285-1287." },
                new EducationReferenceLinkViewModel { Reference = "Carlomagno G, Unfer V. Inositol safety: clinical evidences. Eur Rev Med Pharmacol Sci. 2011;15(8):931-936." },
                new EducationReferenceLinkViewModel { Reference = "Formoso G, Baldassarre MPA, Ginestra F, et al. Inositol and antioxidant supplementation: safety and efficacy in pregnancy. Diabetes Metab Res Rev. 2019;35(5):e3154." },
                new EducationReferenceLinkViewModel { Reference = "FDA. Food and Drug Administration Department of Health and Human Services Subchapter B—Food for Human Consumption.", URL = "https://www.ecfr.gov/current/title-21/chapter-I/subchapter-B/part-184" },
                new EducationReferenceLinkViewModel { Reference = "Milewska EM, Czyzyk A, Meczekalski B, Genazzani AD. Inositol and human reproduction: from metabolism to clinical use. Gynecol Endocrinol. 2016;32(9):690-695." },
                new EducationReferenceLinkViewModel { Reference = "Kerr WG, Colucci F. Inositol phospholipid signaling and the biology of natural killer cells. J Innate Immun. 2011;3(3):249-257." },
                new EducationReferenceLinkViewModel { Reference = "Aouameur R, Da Cal S, Bissonnette P, et al. SMIT2 mediates all myo-inositol uptake in rat small intestine. Am J Physiol Gastrointest Liver Physiol. 2007;293(6):G1300-G1307." },
                new EducationReferenceLinkViewModel { Reference = "Scioscia M, Kunjara S, Gumaa K, et al. Altered urinary release of inositol phosphoglycan A-type in gestational diabetes. Gynecol Obstetric Investigat. 2007;64(4):217-223." },
                new EducationReferenceLinkViewModel { Reference = "Bevilacqua A, Bizzarri M. Inositols in insulin signaling and glucose metabolism. Int J Endocrinol. 2018;2018:1968450." },
                new EducationReferenceLinkViewModel { Reference = "Kunjara S, Wang DY, Greenbaum AL, et al. Inositol phosphoglycans in diabetes and obesity: urinary levels of IPG A-type and IPG P-type. Mol Genet Metab. 1999;68(4):488-502." },
                new EducationReferenceLinkViewModel { Reference = "Larner J, Brautigan DL, Thorner MO. D-chiro-inositol glycans in insulin signaling and resistance. Mol Med. 2010;16(11):543-552." },
                new EducationReferenceLinkViewModel { Reference = "Unfer V, Raffone E, Rizzo P, et al. Effect of myo-inositol plus melatonin on oocyte quality in IVF. Gynecol Endocrinol. 2011;27(11):857-861." },
                new EducationReferenceLinkViewModel { Reference = "Iuorno MJ, Jakubowicz DJ, Baillargeon JP, et al. Effects of d-chiro-inositol in lean women with PCOS. Endocr Pract. 2002;8(6):417-423." },
                new EducationReferenceLinkViewModel { Reference = "Giordano D, Corrado F, Santamaria A, et al. Effects of myo-inositol supplementation in postmenopausal women with metabolic syndrome. Menopause. 2011;18(1):102-104." },
                new EducationReferenceLinkViewModel { Reference = "Zheng X, Liu Z, Zhang Y, et al. Myo-inositol supplementation and gestational diabetes: a meta-analysis. Medicine. 2015;94(42):e1604." },
                new EducationReferenceLinkViewModel { Reference = "Shafrir AL, Farland LV, Shah DK, et al. Risk for and consequences of endometriosis: A critical epidemiologic review. Best Pract Res Clin Obstet Gynaecol. 2018; doi:10.1016/j.bpobgyn.2018.06.001." },
                new EducationReferenceLinkViewModel { Reference = "Zondervan KT, Becker CM, Koga K, et al. Endometriosis. Nature Reviews Disease Primers. 2018;4(1):9. doi:10.1038/s41572-018-0008-5." },
                new EducationReferenceLinkViewModel { Reference = "Kvaskoff M, Mu F, Terry KL, et al. Endometriosis: a high-risk population for chronic diseases? Hum Reprod Update. 2015; doi:10.1093/humupd/dmv013." },
                new EducationReferenceLinkViewModel { Reference = "Parazzini F, Esposito G, Tozzi L, et al. Epidemiology of endometriosis and comorbidities. Eur J Obstet Gynecol Reprod Biol. 2017;209:3–7." },
                new EducationReferenceLinkViewModel { Reference = "Mu F, Rich-Edwards J, Rimm EB, et al. Association Between Endometriosis and Hypercholesterolemia or Hypertension. Hypertension. 2017;70(1):59–65." },
                new EducationReferenceLinkViewModel { Reference = "Melo AS, Rosa-e-Silva JC, Rosa-e-Silva AC, et al. Unfavorable lipid profile in women with endometriosis. Fertil Steril. 2010;93(7):2433–2436." },
                new EducationReferenceLinkViewModel { Reference = "Verit FF, Erel O, Celik N. Serum paraoxonase-1 activity in women with endometriosis. Hum Reprod. 2008;23(1):100–104." },
                new EducationReferenceLinkViewModel { Reference = "Turgut A, Ozler A, Goruk NY, et al. Copper, ceruloplasmin and oxidative stress in advanced-stage endometriosis. Eur Rev Med Pharmacol Sci. 2013;17(11):1472–1478." },
                new EducationReferenceLinkViewModel { Reference = "Pretta S, Remorgida V, Abbamonte LH, et al. Atherosclerosis in women with endometriosis. Eur J Obstet Gynecol Reprod Biol. 2007;132(2):226–231." },
                new EducationReferenceLinkViewModel { Reference = "Mu F, Rich-Edwards J, Rimm EB, et al. Endometriosis and Risk of Coronary Heart Disease. Circ Cardiovasc Qual Outcomes. 2016;9(3):257–264." },
                new EducationReferenceLinkViewModel { Reference = "Harada T, Iwabe T, Terakawa N. Role of cytokines in endometriosis. Fertil Steril. 2001;76(1):1–10." },
                new EducationReferenceLinkViewModel { Reference = "Lebovic DI, Mueller MD, Taylor RN. Immunobiology of endometriosis. Fertil Steril. 2001;75(1):1–10." },
                new EducationReferenceLinkViewModel { Reference = "Agic A, Xu H, Altgassen C, et al. Vitamin D receptor expression in endometriosis and cancers. Reprod Sci. 2007;14(5):486–497." },
                new EducationReferenceLinkViewModel { Reference = "Bedaiwy MA, Falcone T, Sharma RK, et al. Prediction of endometriosis with serum and peritoneal markers. Hum Reprod. 2002;17(2):426–431." }
            };

            for (int i = 0; i < references.Count; i++)
            {
                references[i].Prefix = $"{(i + 1)}. ";
            }

            return references;
        }
    }
}

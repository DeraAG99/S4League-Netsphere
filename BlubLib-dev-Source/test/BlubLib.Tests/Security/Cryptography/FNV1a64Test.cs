using System.Collections.Generic;
using System.Text;
using BlubLib.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.Security.Cryptography
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class FNV1a64Test
    {
        #region Vectors

        private static readonly IReadOnlyDictionary<string, ulong> s_vectors = new Dictionary<string, ulong>
        {
            {"", 14695981039346656037},
            {"a", 12638187200555641996},
            {"b", 12638190499090526629},
            {"c", 12638189399578898418},
            {"d", 12638183902020757363},
            {"e", 12638182802509129152},
            {"f", 12638186101044013785},
            {"fo", 619342838404076354},
            {"foo", 15902901984413996407},
            {"foob", 15929810745020453551},
            {"fooba", 14610070471194899466},
            {"foobar", 9625390261332436968},
            {"\0", 12638153115695167455},
            {"a\0", 620337896427418084},
            {"b\0", 623207621776481119},
            {"c\0", 622251046660126774},
            {"d\0", 617468171078355049},
            {"e\0", 616511595962000704},
            {"f\0", 619381321311063739},
            {"fo\0", 15902925074158188838},
            {"foo\0", 15929918497160018229},
            {"foob\0", 14610036386334424925},
            {"fooba\0", 9625269315053333758},
            {"foobar\0", 3770388817002598200},
            {"ch", 622154289636844206},
            {"cho", 17724743273852505587},
            {"chon", 17470474028714172871},
            {"chong", 3209988420318208736},
            {"chongo", 16235591609357940989},
            {"chongo ", 17535472964312440711},
            {"chongo w", 15126976569931941072},
            {"chongo wa", 3053131832420579523},
            {"chongo was", 15923486988007691024},
            {"chongo was ", 4635570120480447120},
            {"chongo was h", 17801093208756065128},
            {"chongo was he", 5504114770167619351},
            {"chongo was her", 11372141362099525791},
            {"chongo was here", 11808022321033277646},
            {"chongo was here!", 9623681831573775901},
            {"chongo was here!\n", 5080352029159061781},
            {"ch\0", 17724722383131569578},
            {"cho\0", 17470568586714199017},
            {"chon\0", 3210031301271708965},
            {"chong\0", 16235680669799826080},
            {"chongo\0", 17535508148684543463},
            {"chongo \0", 15126861121210978917},
            {"chongo w\0", 3053165917281054064},
            {"chongo wa\0", 15923507878728627033},
            {"chongo was\0", 4635534936108344368},
            {"chongo was \0", 17800978859546731184},
            {"chongo was h\0", 5504214825725786552},
            {"chongo was he\0", 11372055600192525333},
            {"chongo was her\0", 11807922265475110445},
            {"chongo was here\0", 9623645547690044938},
            {"chongo was here!\0", 5080358626228831047},
            {"chongo was here!\n\0", 14068064737283161775},
            {"cu", 622133398915908197},
            {"cur", 17706461694013836565},
            {"curd", 5518741951276603139},
            {"curds", 510960464256930640},
            {"curds ", 6496807775049451344},
            {"curds a", 410884947093010499},
            {"curds an", 13342556948115027063},
            {"curds and", 8344569556455308361},
            {"curds and ", 15387757671860545131},
            {"curds and w", 16684535352299274644},
            {"curds and wh", 12724629860117618484},
            {"curds and whe", 12980998448219666083},
            {"curds and whey", 2586509717993665646},
            {"curds and whey\n", 1876669316956653036},
            {"cu\0", 17706547455920837023},
            {"cur\0", 5518640796206807727},
            {"curd\0", 510840617489455641},
            {"curds\0", 6496772590677348592},
            {"curds \0", 410919031953485040},
            {"curds a\0", 13342581137370847705},
            {"curds an\0", 8344679507618129461},
            {"curds and\0", 15387722487488442379},
            {"curds and \0", 16684622213717903313},
            {"curds and w\0", 12724515510908284540},
            {"curds and wh\0", 12980966562382447964},
            {"curds and whe\0", 2586449244854114041},
            {"curds and whey\0", 1876680312072935146},
            {"curds and whey\n\0", 6101203576291102724},
            {"hi", 628919584683901914},
            {"hi\0", 3707399385412717422},
            {"hello", 11831194018420276491},
            {"hello\0", 12230803299529341361},
            {"\xff\x00\x00\x01", 7593378366456883245},
            {"\x01\x00\x00\xff", 12478262318420582377},
            {"\xff\x00\x00\x02", 7593375067921998612},
            {"\x02\x00\x00\xff", 10167918016021074486},
            {"\xff\x00\x00\x03", 7593376167433626823},
            {"\x03\x00\x00\xff", 17086759824739546311},
            {"\xff\x00\x00\x04", 7593372868898742190},
            {"\x04\x00\x00\xff", 14788606620820090268},
            {"\x40\x51\x4e\x44", 16407569562670520024},
            {"\x44\x4e\x51\x40", 17833991134593459782},
            {"\x40\x51\x4e\x4a", 16407567363647263602},
            {"\x4a\x4e\x51\x40", 6946763845996827736},
            {"\x40\x51\x4e\x54", 16407587154856571400},
            {"\x54\x4e\x51\x40", 5438959643590670646},
            {"127.0.0.1", 12302425093482026174},
            {"127.0.0.1\0", 17641470601885269722},
            {"127.0.0.2", 12302423993970397963},
            {"127.0.0.2\0", 17640514026768915377},
            {"127.0.0.3", 12302422894458769752},
            {"127.0.0.3\0", 17639557451652561032},
            {"64.81.78.68", 16657050055612093351},
            {"64.81.78.68\0", 8411603860306502853},
            {"64.81.78.74", 16656053898077123410},
            {"64.81.78.74\0", 7753001892737692758},
            {"64.81.78.84", 16658888439054083693},
            {"64.81.78.84\0", 9594887279154514743},
            {"feedface", 757669545398053564},
            {"feedface\0", 8293469090240503156},
            {"feedfacedaffdeed", 4496514191561895073},
            {"feedfacedaffdeed\0", 10765455398888101267},
            {"feedfacedeadbeef", 14611160925141102536},
            {"feedfacedeadbeef\0", 12080002150020411608},
            {"line 1\nline 2\nline 3", 8658598129674203459},
            {"chongo <Landon Curt Noll> /\\../\\", 3210869287809699590},
            {"chongo <Landon Curt Noll> /\\../\\\0", 15226161625275877170},
            {"chongo (Landon Curt Noll) /\\../\\", 3892696313174502833},
            {"chongo (Landon Curt Noll) /\\../\\\0", 630815673453683651},
            {"http://antwrp.gsfc.nasa.gov/apod/astropix.html", 7774222482946959066},
            {"http://en.wikipedia.org/wiki/Fowler_Noll_Vo_hash", 15688667514616124613},
            {"http://epod.usra.edu/", 413825227233299091},
            {"http://exoplanet.eu/", 673272027354330236},
            {"http://hvo.wr.usgs.gov/cam3/", 15721155546005355095},
            {"http://hvo.wr.usgs.gov/cams/HMcam/", 8364151083356514288},
            {"http://hvo.wr.usgs.gov/kilauea/update/deformation.html", 12894566977709511602},
            {"http://hvo.wr.usgs.gov/kilauea/update/images.html", 12925489047820217860},
            {"http://hvo.wr.usgs.gov/kilauea/update/maps.html", 5348735187644944483},
            {"http://hvo.wr.usgs.gov/volcanowatch/current_issue.html", 6573706195063278160},
            {"http://neo.jpl.nasa.gov/risk/", 5258765199482339899},
            {"http://norvig.com/21-days.html", 552436699312819098},
            {"http://primes.utm.edu/curios/home.php", 3415195636368828541},
            {"http://slashdot.org/", 11602151761755363228},
            {"http://tux.wr.usgs.gov/Maps/155.25-19.5.html", 11752433599892759671},
            {"http://volcano.wr.usgs.gov/kilaueastatus.php", 684229642546314115},
            {"http://www.avo.alaska.edu/activity/Redoubt.php", 13050490812685653787},
            {"http://www.dilbert.com/fast/", 15941631722496783081},
            {"http://www.fourmilab.ch/gravitation/orbits/", 9713462482532990328},
            {"http://www.fpoa.net/", 16623373360958737432},
            {"http://www.ioccc.org/index.html", 17031213707701719187},
            {"http://www.isthe.com/cgi-bin/number.cgi", 5000784022031222245},
            {"http://www.isthe.com/chongo/bio.html", 10121287884111677703},
            {"http://www.isthe.com/chongo/index.html", 9919956240827471657},
            {"http://www.isthe.com/chongo/src/calc/lucas-calc", 3660981361825850371},
            {"http://www.isthe.com/chongo/tech/astro/venus2004.html", 7010614017101479425},
            {"http://www.isthe.com/chongo/tech/astro/vita.html", 18042672362657100432},
            {"http://www.isthe.com/chongo/tech/comp/c/expert.html", 11411406683428678506},
            {"http://www.isthe.com/chongo/tech/comp/calc/index.html", 13173357477882173522},
            {"http://www.isthe.com/chongo/tech/comp/fnv/index.html", 9836546955241618161},
            {"http://www.isthe.com/chongo/tech/math/number/howhigh.html", 4923379914270032125},
            {"http://www.isthe.com/chongo/tech/math/number/number.html", 1626794522465679144},
            {"http://www.isthe.com/chongo/tech/math/prime/mersenne.html", 17765201475684631076},
            {"http://www.isthe.com/chongo/tech/math/prime/mersenne.html#largest", 10270414863573137539},
            {"http://www.lavarnd.org/cgi-bin/corpspeak.cgi", 2980299705268052958},
            {"http://www.lavarnd.org/cgi-bin/haiku.cgi", 3615406723804111222},
            {"http://www.lavarnd.org/cgi-bin/rand-none.cgi", 9945730348810728040},
            {"http://www.lavarnd.org/cgi-bin/randdist.cgi", 17259717979274989425},
            {"http://www.lavarnd.org/index.html", 11408786398605536852},
            {"http://www.lavarnd.org/what/nist-test.html", 17431897762504180862},
            {"http://www.macosxhints.com/", 840958833075205993},
            {"http://www.mellis.com/", 5418747155555615022},
            {"http://www.nature.nps.gov/air/webcams/parks/havoso2alert/havoalert.cfm", 15084250374632780636},
            {"http://www.nature.nps.gov/air/webcams/parks/havoso2alert/timelines_24.cfm", 6566536375687189221},
            {"http://www.paulnoll.com/", 5035813428190462503},
            {"http://www.pepysdiary.com/", 5813644938528348116},
            {"http://www.sciencenews.org/index/home/activity/view", 12269363181749269524},
            {"http://www.skyandtelescope.com/", 13956661207887070846},
            {"http://www.sput.nl/~rob/sirius.html", 6221558447533146249},
            {"http://www.systemexperts.com/", 13772172998064509566},
            {"http://www.tq-international.com/phpBB3/index.php", 6241852210477795458},
            {"http://www.travelquesttours.com/index.htm", 16278105803210091680},
            {"http://www.wunderground.com/global/stations/89606.html", 5543250198237776580},
            {"21701217012170121701217012170121701217012170121701", 14128126261984920315},
            {"M21701M21701M21701M21701M21701M21701M21701M21701M21701M21701", 15441242044846279549},
            {
                "2^21701-12^21701-12^21701-12^21701-12^21701-12^21701-12^21701-12^21701-12^21701-12^21701-1",
                5547364034008759483
            },
            {"\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5", 577400028449276821},
            {"\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54\xc5\x54", 12255830522019983357},
            {"23209232092320923209232092320923209232092320923209", 7587374486350140921},
            {"M23209M23209M23209M23209M23209M23209M23209M23209M23209M23209", 11806128317529134881},
            {
                "2^23209-12^23209-12^23209-12^23209-12^23209-12^23209-12^23209-12^23209-12^23209-12^23209-1",
                6303965854087626661
            },
            {"\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9", 13253121848731743425},
            {"\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a\xa9\x5a", 6727530009018870553},
            {
                "391581216093391581216093391581216093391581216093391581216093391581216093391581216093391581216093391581216093391581216093",
                3727129175175946097
            },
            {
                "391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1391581*2^216093-1",
                15583872494445013945
            },
            {
                "\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81\x05\xf9\x9d\x03\x4c\x81",
                9571405150286619605
            },
            {
                "FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210",
                9463275800519734181
            },
            {
                "EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301EFCDAB8967452301",
                14347126740011658181
            },
            {
                "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF",
                13181770479829331669
            },
            {
                "1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE1032547698BADCFE",
                10531959093189811109
            },
            {
                "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00",
                2297958300397344437
            },
            {
                "\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07\x07",
                13996800394042738065
            },
            {
                "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                13956394377465476533
            },
            {
                "\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f\x7f",
                4173132127015395873
            }
        };
        #endregion

        [TestMethod]
        public void TestVectors()
        {
            
        }
    }
}

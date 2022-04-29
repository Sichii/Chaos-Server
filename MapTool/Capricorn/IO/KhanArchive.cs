namespace ChaosTool.Capricorn.IO
{
    public class KhanArchive
    {
        public DATArchive AD { get; set; }

        public DATArchive EH { get; set; }

        public DATArchive IM { get; set; }

        public DATArchive NS { get; set; }

        public DATArchive TZ { get; set; }

        public KhanArchive(string path, bool male)
        {
            var str = male ? "m" : "w";
            this.AD = DATArchive.FromFile(path + "\\khan" + str + "ad.dat");
            this.EH = DATArchive.FromFile(path + "\\khan" + str + "eh.dat");
            this.IM = DATArchive.FromFile(path + "\\khan" + str + "im.dat");
            this.NS = DATArchive.FromFile(path + "\\khan" + str + "ns.dat");
            this.TZ = DATArchive.FromFile(path + "\\khan" + str + "tz.dat");
        }
    }
}
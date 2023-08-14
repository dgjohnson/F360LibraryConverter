using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace _360LibraryConverter
{
    public class ToolEntry
    {
        public string Name { get; set; }
        public string NOF { get; set; }
        public string DC { get; set; }
        public string DN { get; set; }
        public string DCON { get; set; }
        public string APMX { get; set; }
        public string LN { get; set; }
        public string LT { get; set; }
        public string LXP { get; set; }
        public string LF { get; set; }
        public string FHA { get; set; }
        public string RE { get; set; }
        public string GAMF { get; set; }
        public string GAMP { get; set; }
        public string EDRD { get; set; }
        public string MaterialType { get; set; }
        public string CobaltPercent { get; set; }
        public string HelixVariation { get; set; }
        public string FluteSpread { get; set; }
        public string OptMaterial { get; set; }
        public string KAPR { get; set; }
    }

    public class ToolEntryClassMap : ClassMap<ToolEntry>
    {
        public ToolEntryClassMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.NOF).Name(" NOF");
            Map(m => m.DC).Name(" DC");
            Map(m => m.DN).Name(" DN");
            Map(m => m.DCON).Name(" DCON");
            Map(m => m.APMX).Name(" APMX");
            Map(m => m.LN).Name(" LN");
            Map(m => m.LT).Name(" LT");
            Map(m => m.LXP).Name(" LXP");
            Map(m => m.LF).Name(" LF");
            Map(m => m.FHA).Name(" FHA");
            Map(m => m.RE).Name(" RE");
            Map(m => m.GAMF).Name(" GAMF");
            Map(m => m.GAMP).Name(" GAMP");
            Map(m => m.EDRD).Name(" EDRD");
            Map(m => m.MaterialType).Name(" MaterialType");
            Map(m => m.CobaltPercent).Name(" CobaltPercent");
            Map(m => m.HelixVariation).Name(" HelixVariation");
            Map(m => m.FluteSpread).Name(" FluteSpread");
            Map(m => m.OptMaterial).Name(" OptMaterial");
            Map(m => m.KAPR).Name(" KAPR");
        }
    }
}

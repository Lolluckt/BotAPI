using NewsAPI.Constants;
using System;

namespace MyApplication.Helpers
{
    public static class CountryConverter
    {
        public static Countries? ConvertToCountry(string countryCode)
        {
            switch (countryCode.ToLower())
            {
                case "ae": return Countries.AE;
                case "ar": return Countries.AR;
                case "at": return Countries.AT;
                case "au": return Countries.AU;
                case "be": return Countries.BE;
                case "bg": return Countries.BG;
                case "br": return Countries.BR;
                case "ca": return Countries.CA;
                case "ch": return Countries.CH;
                case "cn": return Countries.CN;
                case "co": return Countries.CO;
                case "cu": return Countries.CU;
                case "cz": return Countries.CZ;
                case "de": return Countries.DE;
                case "eg": return Countries.EG;
                case "fr": return Countries.FR;
                case "gb": return Countries.GB;
                case "gr": return Countries.GR;
                case "hk": return Countries.HK;
                case "hu": return Countries.HU;
                case "id": return Countries.ID;
                case "ie": return Countries.IE;
                case "il": return Countries.IL;
                case "in": return Countries.IN;
                case "it": return Countries.IT;
                case "jp": return Countries.JP;
                case "kr": return Countries.KR;
                case "lt": return Countries.LT;
                case "lv": return Countries.LV;
                case "ma": return Countries.MA;
                case "mx": return Countries.MX;
                case "my": return Countries.MY;
                case "ng": return Countries.NG;
                case "nl": return Countries.NL;
                case "no": return Countries.NO;
                case "nz": return Countries.NZ;
                case "ph": return Countries.PH;
                case "pl": return Countries.PL;
                case "pt": return Countries.PT;
                case "ro": return Countries.RO;
                case "rs": return Countries.RS;
                case "ru": return Countries.RU;
                case "sa": return Countries.SA;
                case "se": return Countries.SE;
                case "sg": return Countries.SG;
                case "si": return Countries.SI;
                case "sk": return Countries.SK;
                case "th": return Countries.TH;
                case "tr": return Countries.TR;
                case "tw": return Countries.TW;
                case "ua": return Countries.UA;
                case "us": return Countries.US;
                case "ve": return Countries.VE;
                case "za": return Countries.ZA;
                default: return null;
            }
        }
    }
}
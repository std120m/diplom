/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Resume
{
    /// <summary>
    /// Анализатор резюме (специфический анализатор)
    /// </summary>
    public class ResumeAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("RESUME")
        /// </summary>
        public const string ANALYZER_NAME = "RESUME";
        public override string Name
        {
            get
            {
                return ANALYZER_NAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Резюме";
            }
        }
        public override string Description
        {
            get
            {
                return "Текст содержит одно резюме";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new ResumeAnalyzer();
        }
        /// <summary>
        /// Специфический анализатор
        /// </summary>
        public override bool IsSpecific
        {
            get
            {
                return true;
            }
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {MetaResume.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(MetaResume.ImageId.ToString(), Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("resume.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == ResumeItemReferent.OBJ_TYPENAME) 
                return new ResumeItemReferent();
            return null;
        }
        public override int ProgressWeight
        {
            get
            {
                return 1;
            }
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            Pullenti.Ner.Core.AnalyzerData ad = kit.GetAnalyzerData(this);
            bool hasSex = false;
            bool hasMoney = false;
            bool hasPos = false;
            bool hasSpec = false;
            bool hasSkills = false;
            bool hasExp = false;
            bool hasEdu = false;
            bool hasAbout = false;
            Pullenti.Ner.ReferentToken rt;
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (!t.IsNewlineBefore) 
                    continue;
                if (!hasSex) 
                {
                    rt = _parseSex(t, ad);
                    if (rt != null) 
                    {
                        hasSex = true;
                        t = rt;
                        continue;
                    }
                }
                if (_checkGeo(t)) 
                    continue;
                if (!hasMoney && (t.GetReferent() is Pullenti.Ner.Money.MoneyReferent)) 
                {
                    ResumeItemReferent money = new ResumeItemReferent();
                    hasMoney = true;
                    money.Typ = ResumeItemType.Money;
                    money.Ref = t.GetReferent();
                    rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(money), t, t);
                    kit.EmbedToken(rt);
                    t = rt;
                    continue;
                }
                if (!hasExp) 
                {
                    rt = _parseExperience(t, ad);
                    if (rt != null) 
                    {
                        hasExp = true;
                        t = rt;
                        continue;
                    }
                }
                if (!hasSpec && t.IsValue("СПЕЦИАЛИЗАЦИЯ", null)) 
                {
                    if (t.Next != null && t.Next.IsChar(':')) 
                        t = t.Next;
                    rt = _parseList(t.Next, ad, ResumeItemType.Speciality);
                    if (rt != null) 
                    {
                        hasSpec = true;
                        t = rt;
                        continue;
                    }
                }
                if (!hasSkills && t.IsValue2("КЛЮЧЕВЫЕ", "НАВЫКИ")) 
                {
                    rt = _parseList(t.Next.Next, ad, ResumeItemType.Skill);
                    if (rt != null) 
                    {
                        hasSkills = true;
                        t = rt;
                        continue;
                    }
                }
                if (!hasAbout && ((t.IsValue2("О", "МНЕ") || t.IsValue2("О", "СЕБЕ")))) 
                {
                    rt = _parseAboutMe(t.Next.Next, ad);
                    if (rt != null) 
                    {
                        hasAbout = true;
                        t = rt;
                        continue;
                    }
                }
                if (!hasSpec && hasSex && !hasPos) 
                {
                    rt = _parseList(t, ad, ResumeItemType.Position);
                    if (rt != null) 
                    {
                        hasPos = true;
                        t = rt;
                        continue;
                    }
                }
                if (!hasEdu) 
                {
                    Pullenti.Ner.MetaToken mt = _parseEducation(t);
                    if (mt != null) 
                    {
                        ResumeItemReferent edu = new ResumeItemReferent();
                        hasEdu = true;
                        edu.Typ = ResumeItemType.Education;
                        edu.Value = mt.Tag as string;
                        rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(edu), mt.BeginToken, mt.EndToken);
                        kit.EmbedToken(rt);
                        t = rt;
                        continue;
                    }
                }
                rt = _parseDriving(t, ad);
                if (rt != null) 
                {
                    t = rt;
                    continue;
                }
            }
        }
        static Pullenti.Ner.ReferentToken _parseSex(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad)
        {
            if (!t.IsValue("МУЖЧИНА", null) && !t.IsValue("ЖЕНЩИНА", null)) 
                return null;
            ResumeItemReferent sex = new ResumeItemReferent();
            sex.Typ = ResumeItemType.Sex;
            sex.Value = (t.IsValue("МУЖЧИНА", null) ? "муж" : "жен");
            Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(sex), t, t);
            t.Kit.EmbedToken(rt);
            t = rt;
            for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore) 
                    break;
                if ((tt is Pullenti.Ner.NumberToken) && tt.Next != null) 
                {
                    if (tt.Next.IsValue("ГОД", null) || tt.Next.IsValue("ЛЕТ", null)) 
                    {
                        ResumeItemReferent age = new ResumeItemReferent();
                        age.Typ = ResumeItemType.Age;
                        age.Value = (tt as Pullenti.Ner.NumberToken).Value;
                        rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(age), tt, tt.Next);
                        t.Kit.EmbedToken(rt);
                        t = rt;
                        break;
                    }
                }
            }
            return rt;
        }
        static Pullenti.Ner.ReferentToken _parseExperience(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad)
        {
            if (!t.IsValue2("ОПЫТ", "РАБОТЫ")) 
                return null;
            for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore) 
                    break;
                if ((tt is Pullenti.Ner.NumberToken) && tt.Next != null) 
                {
                    if (tt.Next.IsValue("ГОД", null) || tt.Next.IsValue("ЛЕТ", null)) 
                    {
                        ResumeItemReferent experience = new ResumeItemReferent();
                        experience.Typ = ResumeItemType.Experience;
                        experience.Value = (tt as Pullenti.Ner.NumberToken).Value;
                        Pullenti.Ner.Token tt1 = tt.Next;
                        if ((tt1.Next is Pullenti.Ner.NumberToken) && tt1.Next.Next != null && tt1.Next.Next.IsValue("МЕСЯЦ", null)) 
                        {
                            double d = Math.Round((tt as Pullenti.Ner.NumberToken).RealValue + (((tt1.Next as Pullenti.Ner.NumberToken).RealValue / 12)), 1);
                            experience.Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(d);
                            tt1 = tt1.Next.Next;
                        }
                        Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(experience), tt, tt1);
                        t.Kit.EmbedToken(rt);
                        return rt;
                    }
                }
            }
            return null;
        }
        static Pullenti.Ner.MetaToken _parseEducation(Pullenti.Ner.Token t)
        {
            bool hi = false;
            bool middl = false;
            bool prof = false;
            bool spec = false;
            bool tech = false;
            bool neok = false;
            bool keyword = false;
            Pullenti.Ner.Token t0 = t;
            Pullenti.Ner.Token t1 = t;
            for (; t != null; t = t.Next) 
            {
                if (t0 != t && t.IsNewlineBefore) 
                    break;
                if (t.IsValue("СРЕДНИЙ", null) || t.IsValue("СРЕДНЕ", null) || t.IsValue("СРЕДН", null)) 
                    middl = true;
                else if (t.IsValue("ВЫСШИЙ", null) || t.IsValue("ВЫСШ", null)) 
                    hi = true;
                else if (t.IsValue("НЕОКОНЧЕННЫЙ", null)) 
                    neok = true;
                else if (t.IsValue("ПРОФЕССИОНАЛЬНЫЙ", null) || t.IsValue("ПРОФ", null) || t.IsValue("ПРОФИЛЬНЫЙ", null)) 
                    prof = true;
                else if ((t.IsValue("СПЕЦИАЛЬНЫЙ", null) || t.IsValue("СПЕЦ", null) || t.IsValue2("ПО", "СПЕЦИАЛЬНОСТЬ")) || t.IsValue2("ПО", "НАПРАВЛЕНИЕ")) 
                    spec = true;
                else if ((t.IsValue("ТЕХНИЧЕСКИЙ", null) || t.IsValue("ТЕХ", null) || t.IsValue("ТЕХН", null)) || t.IsValue("ТЕХНИЧ", null)) 
                    tech = true;
                else if (t.IsValue("ОБРАЗОВАНИЕ", null)) 
                {
                    keyword = true;
                    t1 = t;
                }
                else 
                    break;
            }
            if (!keyword) 
                return null;
            if (!hi && !middl) 
            {
                if ((spec || prof || tech) || neok) 
                    middl = true;
                else 
                    return null;
            }
            string val = (hi ? "ВО" : "СО");
            if (spec) 
                val += ",спец";
            if (prof) 
                val += ",проф";
            if (tech) 
                val += ",тех";
            if (neok) 
                val += ",неоконч";
            return new Pullenti.Ner.MetaToken(t0, t1) { Tag = val };
        }
        static Pullenti.Ner.MetaToken _parseMoral(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Core.TerminToken tok = Pullenti.Ner.Vacance.Internal.VacanceToken.m_Termins.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null || tok.Termin.Tag2 != null) 
                return null;
            Pullenti.Ner.Vacance.Internal.VacanceTokenType ty = (Pullenti.Ner.Vacance.Internal.VacanceTokenType)tok.Termin.Tag;
            if (ty != Pullenti.Ner.Vacance.Internal.VacanceTokenType.Moral) 
                return null;
            string val = string.Format("{0}{1}", tok.Termin.CanonicText[0], tok.Termin.CanonicText.Substring(1).ToLower());
            Pullenti.Ner.Token t1 = tok.EndToken;
            for (Pullenti.Ner.Token tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
            {
                if (tt.WhitespacesBeforeCount > 2) 
                    break;
                if (Pullenti.Ner.Vacance.Internal.VacanceToken.m_Termins.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                    break;
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePreposition | Pullenti.Ner.Core.NounPhraseParseAttr.ParsePronouns, 0, null);
                if (npt == null) 
                    break;
                tt = (t1 = npt.EndToken);
            }
            if (t1.EndChar > tok.EndChar) 
                val = string.Format("{0} {1}", val, Pullenti.Ner.Core.MiscHelper.GetTextValue(tok.EndToken.Next, t1, Pullenti.Ner.Core.GetTextAttr.KeepQuotes | Pullenti.Ner.Core.GetTextAttr.KeepRegister));
            return new Pullenti.Ner.MetaToken(t, t1) { Tag = val };
        }
        static Pullenti.Ner.ReferentToken _parseDriving(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Token t1 = null;
            if ((t.IsValue2("ВОДИТЕЛЬСКИЕ", "ПРАВА") || t.IsValue2("ПРАВА", "КАТЕГОРИИ") || t.IsValue2("ВОДИТЕЛЬСКОЕ", "УДОСТОВЕРЕНИЕ")) || t.IsValue2("УДОСТОВЕРЕНИЕ", "ВОДИТЕЛЯ") || t.IsValue2("ПРАВА", "ВОДИТЕЛЯ")) 
                t1 = t.Next.Next;
            if (t1 == null) 
                return null;
            Pullenti.Ner.Token t0 = t;
            string val = null;
            for (t = t1; t != null; t = t.Next) 
            {
                if ((t.IsHiphen || t.IsCharOf(":.") || t.IsValue("КАТЕГОРИЯ", null)) || t.IsValue("КАТ", null)) 
                    continue;
                if ((t is Pullenti.Ner.TextToken) && t.LengthChar <= 3 && t.Chars.IsLetter) 
                {
                    val = (t as Pullenti.Ner.TextToken).Term;
                    t1 = t;
                    for (t = t.Next; t != null; t = t.Next) 
                    {
                        if (t.WhitespacesBeforeCount > 2) 
                            break;
                        else if (t.IsChar('.') || t.IsCommaAnd) 
                            continue;
                        else if (t.LengthChar == 1 && t.Chars.IsAllUpper && t.Chars.IsLetter) 
                        {
                            val = string.Format("{0}{1}", val, (t as Pullenti.Ner.TextToken).Term);
                            t1 = t;
                        }
                        else 
                            break;
                    }
                    val = val.Replace("А", "A").Replace("В", "B").Replace("С", "C");
                    break;
                }
                break;
            }
            if (val == null) 
                return null;
            ResumeItemReferent drv = new ResumeItemReferent();
            drv.Typ = ResumeItemType.DrivingLicense;
            drv.Value = val;
            Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(drv), t0, t1);
            t0.Kit.EmbedToken(rt);
            return rt;
        }
        static Pullenti.Ner.MetaToken _parseOnto(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            if (t.Kit.Ontology == null) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> lii = t.Kit.Ontology.AttachToken(ANALYZER_NAME, t);
            if (lii == null || lii.Count == 0) 
                return null;
            if (!(lii[0].Item.Referent is ResumeItemReferent)) 
                return null;
            string val = (lii[0].Item.Referent as ResumeItemReferent).Value;
            val = string.Format("{0}{1}", val[0], val.Substring(1).ToLower());
            return new Pullenti.Ner.MetaToken(t, lii[0].EndToken) { Tag = val };
        }
        static Pullenti.Ner.ReferentToken _parseList(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad, ResumeItemType typ)
        {
            Pullenti.Ner.ReferentToken rt = null;
            ResumeItemReferent spec;
            Pullenti.Ner.Token t0 = t;
            for (; t != null; t = t.Next) 
            {
                if (t.IsNewlineBefore) 
                {
                    if (t.NewlinesBeforeCount > 1 && t != t0) 
                        break;
                    if (t.IsValue2("О", "МНЕ") || t.IsValue2("О", "СЕБЕ")) 
                        break;
                    if (t == t0 && typ == ResumeItemType.Position) 
                    {
                    }
                    else if (typ == ResumeItemType.Skill) 
                    {
                    }
                    else 
                        break;
                }
                if (t.IsCharOf(";,")) 
                    continue;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                {
                    Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                    if (br != null) 
                    {
                        spec = new ResumeItemReferent();
                        spec.Typ = typ;
                        spec.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t.Next, br.EndToken.Previous, Pullenti.Ner.Core.GetTextAttr.KeepQuotes | Pullenti.Ner.Core.GetTextAttr.KeepRegister).Replace(" - ", "-");
                        rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(spec), t, br.EndToken);
                        t.Kit.EmbedToken(rt);
                        t = rt;
                        continue;
                    }
                }
                Pullenti.Ner.Token t1 = t;
                for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
                {
                    if (tt.IsNewlineBefore) 
                        break;
                    if (tt.IsCharOf(";,")) 
                        break;
                    t1 = tt;
                }
                if (t1 == null) 
                    break;
                Pullenti.Ner.ReferentToken rt1 = _parseDriving(t, ad);
                if (rt1 != null) 
                {
                    t = rt1;
                    rt = rt1;
                    continue;
                }
                Pullenti.Ner.MetaToken mt = _parseMoral(t);
                if (mt != null) 
                {
                    ResumeItemReferent mor = new ResumeItemReferent();
                    mor.Typ = ResumeItemType.Moral;
                    mor.Value = mt.Tag as string;
                    rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(mor), t, mt.EndToken);
                }
                else 
                {
                    spec = new ResumeItemReferent();
                    spec.Typ = typ;
                    spec.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, t1, Pullenti.Ner.Core.GetTextAttr.KeepQuotes | Pullenti.Ner.Core.GetTextAttr.KeepRegister).Replace(" - ", "-");
                    rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(spec), t, t1);
                }
                t.Kit.EmbedToken(rt);
                t = rt;
            }
            return rt;
        }
        static Pullenti.Ner.ReferentToken _parseAboutMe(Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad)
        {
            Pullenti.Ner.Token t0 = t;
            Pullenti.Ner.ReferentToken rt = null;
            for (; t != null; t = t.Next) 
            {
                if (t.IsNewlineBefore) 
                {
                    if (_parseEducation(t) != null) 
                        break;
                }
                Pullenti.Ner.MetaToken mt = _parseMoral(t);
                if (mt != null) 
                {
                    ResumeItemReferent mor = new ResumeItemReferent();
                    mor.Typ = ResumeItemType.Moral;
                    mor.Value = mt.Tag as string;
                    rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(mor), t, mt.EndToken);
                    t.Kit.EmbedToken(rt);
                    t = rt;
                    continue;
                }
                mt = _parseOnto(t);
                if (mt != null) 
                {
                    ResumeItemReferent mor = new ResumeItemReferent();
                    mor.Typ = ResumeItemType.Skill;
                    mor.Value = mt.Tag as string;
                    rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(mor), t, mt.EndToken);
                    t.Kit.EmbedToken(rt);
                    t = rt;
                    continue;
                }
            }
            return rt;
        }
        static bool _checkGeo(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return false;
            if (t.IsValue2("УКАЗАН", "ПРИМЕРНЫЙ")) 
                return true;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt != t && tt.IsNewlineBefore) 
                    break;
                Pullenti.Ner.Referent r = tt.GetReferent();
                if ((r is Pullenti.Ner.Geo.GeoReferent) || (r is Pullenti.Ner.Address.StreetReferent) || (r is Pullenti.Ner.Address.AddressReferent)) 
                    return true;
                if (tt.IsValue("ГОТОВ", null) || tt.IsValue("ПЕРЕЕЗД", null) || tt.IsValue("КОМАНДИРОВКА", null)) 
                    return true;
            }
            return false;
        }
        public override Pullenti.Ner.ReferentToken ProcessOntologyItem(Pullenti.Ner.Token begin)
        {
            for (Pullenti.Ner.Token t = begin; t != null; t = t.Next) 
            {
                if (t.Next == null) 
                {
                    ResumeItemReferent re = new ResumeItemReferent();
                    re.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(begin, t, Pullenti.Ner.Core.GetTextAttr.No);
                    return new Pullenti.Ner.ReferentToken(re, begin, t);
                }
            }
            return null;
        }
        static bool m_Initialized = false;
        static object m_Lock = new object();
        public static void Initialize()
        {
            lock (m_Lock) 
            {
                if (m_Initialized) 
                    return;
                m_Initialized = true;
                MetaResume.Initialize();
                Pullenti.Ner.Vacance.Internal.VacanceToken.Initialize();
                Pullenti.Ner.ProcessorService.RegisterAnalyzer(new ResumeAnalyzer());
            }
        }
    }
}
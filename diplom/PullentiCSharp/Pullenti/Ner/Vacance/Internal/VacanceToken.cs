/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Ner.Vacance.Internal
{
    class VacanceToken : Pullenti.Ner.MetaToken
    {
        public VacanceToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public VacanceTokenType Typ = VacanceTokenType.Undefined;
        public List<Pullenti.Ner.Referent> Refs = new List<Pullenti.Ner.Referent>();
        public string Value;
        public string Value2;
        bool IsSkill
        {
            get
            {
                return (((Typ == VacanceTokenType.Expierence || Typ == VacanceTokenType.Education || Typ == VacanceTokenType.Skill) || Typ == VacanceTokenType.Language || Typ == VacanceTokenType.Plus) || Typ == VacanceTokenType.Moral || Typ == VacanceTokenType.License) || Typ == VacanceTokenType.Driving;
            }
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (Typ != VacanceTokenType.Undefined) 
                tmp.AppendFormat("{0}: ", Typ);
            if (Value != null) 
                tmp.AppendFormat("\"{0}\" ", Value);
            if (Value2 != null) 
                tmp.AppendFormat("\"{0}\" ", Value2);
            foreach (Pullenti.Ner.Referent r in Refs) 
            {
                tmp.AppendFormat("[{0}] ", r.ToString());
            }
            tmp.AppendFormat(" {0}", this.GetSourceText());
            return tmp.ToString();
        }
        public static List<VacanceToken> TryParseList(Pullenti.Ner.Token t)
        {
            List<VacanceToken> res = new List<VacanceToken>();
            for (; t != null; t = t.Next) 
            {
                VacanceToken prev = null;
                if (res.Count > 0 && res[res.Count - 1].EndToken.Next == t) 
                    prev = res[res.Count - 1];
                VacanceToken vv = TryParse(t, prev);
                if (vv == null) 
                    break;
                if (vv.LengthChar > 3) 
                    res.Add(vv);
                t = vv.EndToken;
            }
            for (int i = 0; i < res.Count; i++) 
            {
                VacanceToken it = res[i];
                if (it.Typ == VacanceTokenType.Date) 
                {
                    it.Typ = VacanceTokenType.Undefined;
                    continue;
                }
                if (it.Typ == VacanceTokenType.Dummy) 
                    continue;
                if (it.Typ == VacanceTokenType.Undefined && it.Refs.Count > 0) 
                {
                    if (it.Refs[0] is Pullenti.Ner.Uri.UriReferent) 
                        continue;
                }
                if (it.Typ == VacanceTokenType.Skill && ((i + 1) < res.Count) && res[i + 1].Typ == VacanceTokenType.Money) 
                    it.Typ = VacanceTokenType.Undefined;
                if (it.Typ == VacanceTokenType.Expired) 
                    continue;
                if (it.Typ != VacanceTokenType.Undefined) 
                    break;
                it.Typ = VacanceTokenType.Name;
                if (((i + 2) < res.Count) && ((res[i + 1].Typ == VacanceTokenType.Undefined || res[i + 1].Typ == VacanceTokenType.Skill))) 
                {
                    if (res[i + 2].Typ == VacanceTokenType.Money) 
                    {
                        it.EndToken = res[i + 1].EndToken;
                        res.RemoveAt(i + 1);
                    }
                    else if (res[i + 2].Typ == VacanceTokenType.Money) 
                    {
                        if (res[i + 2]._tryParseMoney()) 
                        {
                            it.EndToken = res[i + 1].EndToken;
                            res.RemoveAt(i + 1);
                        }
                    }
                }
                it._getValue();
                if (((i + 1) < res.Count) && res[i + 1].Typ == VacanceTokenType.Undefined) 
                {
                    if (res[i + 1]._tryParseMoney()) 
                    {
                        for (int j = i + 2; j < res.Count; j++) 
                        {
                            if (res[j].Typ == VacanceTokenType.Money) 
                                res[j].Typ = VacanceTokenType.Undefined;
                        }
                    }
                }
                break;
            }
            for (int i = 1; i < res.Count; i++) 
            {
                VacanceToken it = res[i];
                if (it.Typ != VacanceTokenType.Undefined) 
                    continue;
                if (!res[i - 1].IsSkill) 
                    continue;
                for (int j = i + 1; (j < res.Count) && (j < (i + 2)); j++) 
                {
                    if (res[j].IsSkill) 
                    {
                        if (res[j].Typ == VacanceTokenType.Plus || res[j].Typ == VacanceTokenType.Moral) 
                            res[i].Typ = res[i].Typ;
                        else 
                            res[i].Typ = VacanceTokenType.Skill;
                        break;
                    }
                }
            }
            for (int i = 0; i < res.Count; i++) 
            {
                VacanceToken it = res[i];
                if (it.IsSkill && it.Value == null) 
                    it._getValue();
                if (it.Typ == VacanceTokenType.Skill || it.Typ == VacanceTokenType.Moral || it.Typ == VacanceTokenType.Plus) 
                {
                    for (int j = i + 1; j < res.Count; j++) 
                    {
                        if (res[j].Typ != it.Typ) 
                            break;
                        else 
                        {
                            it.EndToken = res[j].EndToken;
                            res.RemoveAt(j);
                            j--;
                        }
                    }
                    List<VacanceToken> li = _tryParseSkills(it.BeginToken, it.EndToken);
                    if (li != null && li.Count > 0) 
                    {
                        res.RemoveAt(i);
                        res.InsertRange(i, li);
                    }
                }
            }
            return res;
        }
        public static VacanceToken TryParse(Pullenti.Ner.Token t, VacanceToken prev)
        {
            if (t == null) 
                return null;
            if (t.IsValue2("НА", "ПОСТОЯННУЮ")) 
            {
            }
            VacanceToken res = new VacanceToken(t, t);
            int skills = 0;
            int dummy = 0;
            int lang = 0;
            int edu = 0;
            int moral = 0;
            int lic = 0;
            int plus = 0;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore && tt != t) 
                {
                    if (Pullenti.Ner.Core.MiscHelper.CanBeStartOfSentence(tt)) 
                        break;
                    if (tt.IsHiphen) 
                        break;
                    bool cr = true;
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt.Previous, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.EndChar >= tt.BeginChar) 
                        cr = false;
                    else if (tt.Previous.GetMorphClassInDictionary().IsNoun && tt.Chars.IsAllLower) 
                    {
                        npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                        if (npt != null && npt.Morph.Case.IsGenitive && !npt.Morph.Case.IsNominative) 
                            cr = false;
                    }
                    else if (tt.Previous is Pullenti.Ner.NumberToken) 
                    {
                        if (tt.IsValue("РАЗРЯД", null)) 
                            cr = false;
                    }
                    if (cr) 
                        break;
                }
                if (tt.IsChar(';')) 
                    break;
                res.EndToken = tt;
                Pullenti.Ner.Core.TerminToken tok = m_Termins.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok != null) 
                {
                    VacanceTokenType ty = (VacanceTokenType)tok.Termin.Tag;
                    if (ty == VacanceTokenType.Stop && tt == res.BeginToken) 
                        return null;
                    res.EndToken = (tt = tok.EndToken);
                    if (ty == VacanceTokenType.Expired) 
                    {
                        res.Typ = VacanceTokenType.Expired;
                        continue;
                    }
                    if (ty == VacanceTokenType.Dummy) 
                    {
                        dummy++;
                        continue;
                    }
                    if (ty == VacanceTokenType.Education) 
                    {
                        edu++;
                        continue;
                    }
                    if (ty == VacanceTokenType.Language) 
                    {
                        lang++;
                        for (Pullenti.Ner.Token ttt = tt.Previous; ttt != null && ttt.BeginChar >= t.BeginChar; ttt = ttt.Previous) 
                        {
                            if ((ttt.IsValue("ПЕДАГОГ", null) || ttt.IsValue("УЧИТЕЛЬ", null) || ttt.IsValue("РЕПЕТИТОР", null)) || ttt.IsValue("ПРЕПОДАВАТЕЛЬ", null)) 
                            {
                                lang--;
                                break;
                            }
                        }
                        continue;
                    }
                    if (ty == VacanceTokenType.Moral) 
                    {
                        moral++;
                        continue;
                    }
                    if (ty == VacanceTokenType.Plus) 
                    {
                        plus++;
                        continue;
                    }
                    if (ty == VacanceTokenType.License) 
                    {
                        lic++;
                        Pullenti.Ner.Token ttt = tok.BeginToken.Previous;
                        if (ttt != null) 
                        {
                            if (ttt.IsValue("ОФОРМЛЯТЬ", null) || ttt.IsValue("ОФОРМИТЬ", null) || ttt.IsValue("ОФОРМЛЕНИЕ", null)) 
                                lic--;
                        }
                        continue;
                    }
                    if (ty == VacanceTokenType.Skill) 
                    {
                        if (tok.Termin.Tag2 != null && (tok.BeginChar - res.BeginChar) > 3) 
                            continue;
                        skills++;
                        if (tt.IsValue("ОПЫТ", null) || tt.IsValue("СТАЖ", null)) 
                        {
                            if (res._tryParseExp()) 
                                tt = res.EndToken;
                            else if (prev != null && prev.Typ == VacanceTokenType.Plus) 
                            {
                                skills--;
                                plus++;
                            }
                        }
                        continue;
                    }
                    if (ty == VacanceTokenType.Expierence) 
                    {
                        if (res._tryParseExp()) 
                            tt = res.EndToken;
                        else 
                            skills++;
                        continue;
                    }
                    if (ty == VacanceTokenType.Money) 
                    {
                        if (res._tryParseMoney()) 
                            tt = res.EndToken;
                        continue;
                    }
                    if (ty == VacanceTokenType.Driving) 
                    {
                        if (res._tryParseDriving()) 
                        {
                            tt = res.EndToken;
                            break;
                        }
                        else 
                            lic++;
                    }
                    continue;
                }
                Pullenti.Ner.Referent r = tt.GetReferent();
                if (r is Pullenti.Ner.Date.DateReferent) 
                {
                    Pullenti.Ner.Date.DateReferent dd = r as Pullenti.Ner.Date.DateReferent;
                    if (dd.Year > 0 && dd.Month > 0 && dd.Day > 0) 
                        res.Refs.Add(dd);
                }
                else if (r is Pullenti.Ner.Uri.UriReferent) 
                    dummy++;
                else if (r != null && !res.Refs.Contains(r)) 
                {
                    if ((r is Pullenti.Ner.Money.MoneyReferent) && (((t.BeginChar - res.BeginChar)) < 10)) 
                    {
                        if (res._tryParseMoney()) 
                        {
                            t = res.EndToken;
                            continue;
                        }
                    }
                    res.Refs.Add(r);
                }
            }
            if (res.Typ == VacanceTokenType.Undefined) 
            {
                if (dummy > 0) 
                    res.Typ = VacanceTokenType.Dummy;
                else if (lang > 0) 
                    res.Typ = VacanceTokenType.Language;
                else if (edu > 0) 
                {
                    res.Typ = VacanceTokenType.Education;
                    res._tryParseEducation();
                }
                else if (res.Refs.Count > 0 && (res.Refs[0] is Pullenti.Ner.Date.DateReferent)) 
                    res.Typ = VacanceTokenType.Date;
                else if (moral > 0) 
                    res.Typ = VacanceTokenType.Moral;
                else if (lic > 0) 
                    res.Typ = VacanceTokenType.License;
                else if (plus > 0) 
                    res.Typ = VacanceTokenType.Plus;
                else if (skills > 0) 
                    res.Typ = VacanceTokenType.Skill;
            }
            return res;
        }
        void _getValue()
        {
            Pullenti.Ner.Token t0 = BeginToken;
            Pullenti.Ner.Token t1 = EndToken;
            for (Pullenti.Ner.Token t = t0; t != null && (t.EndChar < EndChar); t = t.Next) 
            {
                if ((t is Pullenti.Ner.TextToken) && t.LengthChar == 1 && !t.Chars.IsLetter) 
                    t0 = t.Next;
                else if (t.IsValue("ИМЕТЬ", null) || t.IsValue("ВЛАДЕТЬ", null) || t.IsValue("ЕСТЬ", null)) 
                    t0 = t.Next;
                else if (t.IsValue2("У", "ВАС") && t.Next.Next != null && t.Next.Next.IsValue("ЕСТЬ", null)) 
                {
                    t = t.Next.Next.Next;
                    t0 = t.Next;
                }
                else 
                {
                    Pullenti.Ner.Core.TerminToken tok = m_Termins.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok != null && tok.Termin.Tag2 != null) 
                    {
                        t = tok.EndToken;
                        t0 = t.Next;
                        continue;
                    }
                    break;
                }
            }
            if (t1.IsCharOf(".;:,") || t1.IsHiphen) 
                t1 = t1.Previous;
            if (Typ == VacanceTokenType.Name) 
            {
                for (Pullenti.Ner.Token t = t0.Next; t != null && (t.EndChar < EndChar); t = t.Next) 
                {
                    if (t.IsCharOf("(,") && t.Next != null) 
                    {
                        if ((t.Next.GetReferent() != null || t.Next.IsValue("М", null) || t.Next.IsValue("СТ", null)) || t.Next.IsValue("СТАНЦИЯ", null) || t.Next.Chars.IsCapitalUpper) 
                            t1 = t.Previous;
                        break;
                    }
                }
            }
            else 
                for (Pullenti.Ner.Token t = t1; t != null && t.BeginChar > t0.BeginChar; t = t.Previous) 
                {
                    Pullenti.Ner.Core.TerminToken tok = m_Termins.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok != null && tok.Termin.Tag2 != null && tok.EndToken == t1) 
                    {
                        t1 = t.Previous;
                        VacanceTokenType ty = (VacanceTokenType)tok.Termin.Tag;
                        if (ty == VacanceTokenType.Plus && Typ == VacanceTokenType.Skill) 
                            Typ = VacanceTokenType.Plus;
                        for (; t1 != null && t1 != t0; t1 = t1.Previous) 
                        {
                            if (t1.IsValue("БЫТЬ", null) || t1.IsValue("ЯВЛЯТЬСЯ", null)) 
                            {
                            }
                            else 
                                break;
                        }
                        break;
                    }
                }
            Pullenti.Ner.Core.GetTextAttr attr = Pullenti.Ner.Core.GetTextAttr.KeepRegister | Pullenti.Ner.Core.GetTextAttr.KeepQuotes;
            if (Typ == VacanceTokenType.Moral) 
            {
                Pullenti.Ner.Core.TerminToken tok1 = m_Termins.TryParse(t0, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok1 != null && tok1.Termin.Tag2 == null && ((VacanceTokenType)tok1.Termin.Tag) == Typ) 
                {
                    Value = tok1.Termin.CanonicText.ToLower();
                    if (tok1.EndChar < t1.EndChar) 
                        Value = string.Format("{0} {1}", Value, Pullenti.Ner.Core.MiscHelper.GetTextValue(tok1.EndToken.Next, t1, attr));
                }
            }
            if (Value == null) 
            {
                if (t0.IsValue("ПРАВО", null)) 
                {
                }
                else 
                    attr |= Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominative;
                Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t0, t1, attr);
            }
            if (!string.IsNullOrEmpty(Value) && !t0.Chars.IsAllUpper && char.IsLower(Value[0])) 
                Value = string.Format("{0}{1}", char.ToUpper(Value[0]), Value.Substring(1));
        }
        bool _tryParseExp()
        {
            Pullenti.Ner.Token t = EndToken.Next;
            for (; t != null; t = t.Next) 
            {
                if (t.IsValue("РАБОТА", null) || t.IsHiphen || t.IsChar(':')) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            if (t.IsValue2("НЕ", "ТРЕБОВАТЬСЯ")) 
            {
                EndToken = t.Next;
                Typ = VacanceTokenType.Expierence;
                Value = "0";
                return true;
            }
            Pullenti.Ner.Measure.Internal.NumbersWithUnitToken uni = Pullenti.Ner.Measure.Internal.NumbersWithUnitToken.TryParse(t, null, Pullenti.Ner.Measure.Internal.NumberWithUnitParseAttr.No);
            if (uni == null) 
                return false;
            if (uni.Units.Count != 1 || uni.Units[0].Unit == null || uni.Units[0].Unit.Kind != Pullenti.Ner.Measure.MeasureKind.Time) 
                return false;
            EndToken = uni.EndToken;
            Typ = VacanceTokenType.Expierence;
            if (uni.SingleVal != null) 
                Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(uni.SingleVal.Value);
            else if (uni.FromVal != null) 
            {
                Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(uni.FromVal.Value);
                if (uni.ToVal != null) 
                    Value = string.Format("{0}-{1}", Value, (Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(uni.ToVal.Value)));
            }
            else if (uni.ToVal != null) 
                Value = Pullenti.Ner.Core.NumberHelper.DoubleToString(uni.ToVal.Value);
            return true;
        }
        bool _tryParseMoney()
        {
            for (Pullenti.Ner.Token t = BeginToken; t != null; t = t.Next) 
            {
                Pullenti.Ner.Money.MoneyReferent m = t.GetReferent() as Pullenti.Ner.Money.MoneyReferent;
                if (m != null) 
                {
                    if (t.EndChar > EndChar) 
                        EndToken = t;
                    if (!Refs.Contains(m)) 
                        Refs.Add(m);
                    Typ = VacanceTokenType.Money;
                    if (t.Next != null && ((t.Next.IsHiphen || t.Next.IsValue("ДО", null)))) 
                    {
                        if (t.Next.Next != null && (t.Next.Next.GetReferent() is Pullenti.Ner.Money.MoneyReferent)) 
                        {
                            if (t.Next.Next.EndChar > EndToken.EndChar) 
                            {
                                EndToken = t.Next.Next;
                                Refs.Add(EndToken.GetReferent());
                            }
                        }
                    }
                    return true;
                }
                if (t.IsNewlineBefore && t != BeginToken) 
                    break;
                if ((t.BeginChar - BeginChar) > 20) 
                    break;
            }
            return false;
        }
        bool _tryParseDriving()
        {
            for (Pullenti.Ner.Token t = EndToken.Next; t != null; t = t.Next) 
            {
                if ((t.IsHiphen || t.IsCharOf(":.") || t.IsValue("КАТЕГОРИЯ", null)) || t.IsValue("КАТ", null)) 
                    continue;
                if ((t is Pullenti.Ner.TextToken) && t.LengthChar <= 3 && t.Chars.IsLetter) 
                {
                    Typ = VacanceTokenType.Driving;
                    Value = (t as Pullenti.Ner.TextToken).Term;
                    EndToken = t;
                    for (t = t.Next; t != null; t = t.Next) 
                    {
                        if (t.IsChar('.') || t.IsCommaAnd) 
                            continue;
                        else if (t.LengthChar == 1 && t.Chars.IsAllUpper && t.Chars.IsLetter) 
                        {
                            Value = string.Format("{0}{1}", Value, (t as Pullenti.Ner.TextToken).Term);
                            EndToken = t;
                        }
                        else 
                            break;
                    }
                    Value = Value.Replace("А", "A").Replace("В", "B").Replace("С", "C");
                    return true;
                }
                break;
            }
            return false;
        }
        bool _tryParseEducation()
        {
            bool hi = false;
            bool middl = false;
            bool prof = false;
            bool spec = false;
            bool tech = false;
            for (Pullenti.Ner.Token t = BeginToken; t != null && t.EndChar <= EndChar; t = t.Next) 
            {
                if (t.IsValue("СРЕДНИЙ", null) || t.IsValue("СРЕДНЕ", null) || t.IsValue("СРЕДН", null)) 
                    middl = true;
                else if (t.IsValue("ВЫСШИЙ", null) || t.IsValue("ВЫСШ", null)) 
                    hi = true;
                else if (t.IsValue("ПРОФЕССИОНАЛЬНЫЙ", null) || t.IsValue("ПРОФ", null) || t.IsValue("ПРОФИЛЬНЫЙ", null)) 
                    prof = true;
                else if ((t.IsValue("СПЕЦИАЛЬНЫЙ", null) || t.IsValue("СПЕЦ", null) || t.IsValue2("ПО", "СПЕЦИАЛЬНОСТЬ")) || t.IsValue2("ПО", "НАПРАВЛЕНИЕ")) 
                    spec = true;
                else if ((t.IsValue("ТЕХНИЧЕСКИЙ", null) || t.IsValue("ТЕХ", null) || t.IsValue("ТЕХН", null)) || t.IsValue("ТЕХНИЧ", null)) 
                    tech = true;
            }
            if (!hi && !middl) 
            {
                if (spec || prof || tech) 
                    middl = true;
            }
            if (hi || middl) 
            {
                Value = (hi ? "ВО" : "СО");
                if (spec) 
                    Value += ",спец";
                if (prof) 
                    Value += ",проф";
                if (tech) 
                    Value += ",тех";
                return true;
            }
            this._getValue();
            return false;
        }
        static List<VacanceToken> _tryParseSkills(Pullenti.Ner.Token t0, Pullenti.Ner.Token t1)
        {
            List<VacanceToken> res = new List<VacanceToken>();
            VacanceToken ski = null;
            bool hasVerb = false;
            VacanceTokenType ty0 = VacanceTokenType.Undefined;
            for (Pullenti.Ner.Token t = t0; t != null && t.EndChar <= t1.EndChar; t = t.Next) 
            {
                bool keyword = false;
                Pullenti.Ner.Core.TerminToken tok = m_Termins.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok == null) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt1 = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt1 != null && npt1.EndToken != t) 
                        tok = m_Termins.TryParse(npt1.EndToken, Pullenti.Ner.Core.TerminParseAttr.No);
                }
                if (tok != null) 
                {
                    VacanceTokenType ty = (VacanceTokenType)tok.Termin.Tag;
                    if (ty == VacanceTokenType.Skill || ty == VacanceTokenType.Moral || ty == VacanceTokenType.Plus) 
                    {
                        keyword = true;
                        ty0 = ty;
                    }
                }
                if (Pullenti.Ner.Core.MiscHelper.CanBeStartOfSentence(t)) 
                    ski = null;
                else if (ski != null && ski.BeginToken != t && keyword) 
                {
                    if (t.Chars.IsCapitalUpper) 
                        ski = null;
                    else if (t.Previous != null && t.Previous.IsCommaAnd) 
                    {
                        ski.EndToken = ski.EndToken.Previous;
                        ski = null;
                    }
                }
                if (ski == null) 
                {
                    ski = new VacanceToken(t, t) { Typ = (ty0 == VacanceTokenType.Undefined ? VacanceTokenType.Skill : ty0) };
                    hasVerb = false;
                    res.Add(ski);
                }
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null) 
                {
                    ski.EndToken = (t = npt.EndToken);
                    continue;
                }
                Pullenti.Ner.Core.VerbPhraseToken verb = Pullenti.Ner.Core.VerbPhraseHelper.TryParse(t, false, false, false);
                if (verb != null) 
                {
                    ski.EndToken = (t = verb.EndToken);
                    hasVerb = true;
                    continue;
                }
                if (t.IsChar(';')) 
                {
                    ski = null;
                    continue;
                }
                if (t.IsComma) 
                {
                }
                ski.EndToken = t;
            }
            for (int i = 0; i < res.Count; i++) 
            {
                res[i]._getValue();
                if (res[i].LengthChar < 5) 
                {
                    res.RemoveAt(i);
                    i--;
                }
            }
            return res;
        }
        public static Pullenti.Ner.Core.TerminCollection m_Termins;
        public static void Initialize()
        {
            if (m_Termins != null) 
                return;
            m_Termins = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("ЗАРАБОТНАЯ ПЛАТА") { Tag = VacanceTokenType.Money };
            t.AddAbridge("З/П");
            m_Termins.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОПЫТ РАБОТЫ") { Tag = VacanceTokenType.Expierence };
            t.AddVariant("СТАЖ РАБОТЫ", false);
            t.AddVariant("РАБОЧИЙ СТАЖ", false);
            m_Termins.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБРАЗОВАНИЕ") { Tag = VacanceTokenType.Education };
            m_Termins.Add(t);
            foreach (string s in new string[] {"АНГЛИЙСКИЙ", "НЕМЕЦКИЙ", "ФРАНЦУЗСКИЙ", "ИТАЛЬЯНСКИЙ", "ИСПАНСКИЙ", "КИТАЙСКИЙ"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Language });
            }
            foreach (string s in new string[] {"ВОДИТЕЛЬСКИЕ ПРАВА", "ПРАВА КАТЕГОРИИ", "ВОДИТЕЛЬСКОЕ УДОСТОВЕРЕНИЕ", "УДОСТОВЕРЕНИЕ ВОДИТЕЛЯ", "ПРАВА ВОДИТЕЛЯ"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Driving });
            }
            foreach (string s in new string[] {"УДОСТОВЕРЕНИЕ", "ВОДИТЕЛЬСКАЯ МЕДСПРАВКА", "ВОДИТЕЛЬСКАЯ МЕД.СПРАВКА", "ВОЕННЫЙ БИЛЕТ", "МЕДИЦИНСКАЯ КНИЖКА", "МЕДКНИЖКА", "МЕД.КНИЖКА", "АТТЕСТАТ", "АТТЕСТАЦИЯ", "СЕРТИФИКАТ", "ДОПУСК", "ГРУППА ДОПУСКА"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.License });
            }
            foreach (string s in new string[] {"ЖЕЛАНИЕ;ЖЕЛАТЬ", "ЖЕЛАНИЕ И СПОСОБНОСТЬ", "ГОТОВНОСТЬ К;ГОТОВЫЙ К", "ДОБРОСОВЕСТНОСТЬ;ДОБРОСОВЕСТНЫЙ", "ГИБКОСТЬ", "РАБОТА В КОМАНДЕ;УМЕНИЕ РАБОТАТЬ В КОМАНДЕ", "ОБЩИТЕЛЬНОСТЬ;ОБЩИТЕЛЬНЫЙ;УМЕНИЕ ОБЩАТЬСЯ С ЛЮДЬМИ;УМЕНИЕ ОБЩАТЬСЯ;КОНТАКТ С ЛЮДЬМИ", "ОТВЕТСТВЕННОСТЬ;ОТВЕТСТВЕННЫЙ", "АКТИВНАЯ ЖИЗНЕННАЯ ПОЗИЦИЯ", "КОММУНИКАБЕЛЬНОСТЬ;КОММУНИКАБЕЛЬНЫЙ", "ЛОЯЛЬНОСТЬ;ЛОЯЛЬНЫЙ", "ИСПОЛНИТЕЛЬНОСТЬ;ИСПОЛНИТЕЛЬНЫЙ", "РЕЗУЛЬТАТИВНОСТЬ;РЕЗУЛЬТАТИВНЫЙ", "ПУНКТУАЛЬНОСТЬ;ПУНКТУАЛЬНЫЙ", "ДИСЦИПЛИНИРОВАННОСТЬ;ДИСЦИПЛИНИРОВАННЫЙ", "ТРУДОЛЮБИЕ;ТРУДОЛЮБИВЫЙ", "ЦЕЛЕУСТРЕМЛЕННОСТЬ;ЦЕЛЕУСТРЕМЛЕННЫЙ", "РАБОТОСПОСОБНОСТЬ;РАБОТОСПОСОБНЫЙ", "ОПРЯТНОСТЬ;ОПРЯТНЫЙ", "ВЕЖЛИВОСТЬ;ВЕЖЛИВЫЙ", "ВЫНОСЛИВОСТЬ;ВЫНОСЛИВЫЙ", "АКТИВНОСТЬ;АКТИВНЫЙ", "ОБУЧАЕМОСТЬ;ОБУЧАЕМЫЙ;СПОСОБНОСТЬ К ОБУЧЕНИЮ;ЛЕГКО ОБУЧАЕМЫЙ;ЛЕГКООБУЧАЕМЫЙ;БЫСТРО ОБУЧАТЬСЯ", "ОБРАЗОВАННОСТЬ", "ОТЛИЧНОЕ НАСТРОЕНИЕ", "ХОРОШЕЕ НАСТРОЕНИЕ", "ГРАМОТНАЯ РЕЧЬ", "ГРАМОТНОЕ ПИСЬМО", "ГРАМОТНОЕ ПИСЬМО И РЕЧЬ", "НАЦЕЛЕННОСТЬ НА РЕЗУЛЬТАТ;НАЦЕЛЕННЫЙ НА РЕЗУЛЬТАТ", "ОПТИМИЗМ;ОПТИМИСТИЧНЫЙ", "КОММУНИКАБЕЛЬНОСТЬ;КОММУНИКАБЕЛЬНЫЙ", "ПРИВЕТЛИВОСТЬ;ПРИВЕТЛИВЫЙ", "ЖЕЛАНИЕ РАБОТАТЬ;ЖЕЛАТЬ РАБОТАТЬ", "ЖЕЛАНИЕ ЗАРАБАТЫВАТЬ;ЖЕЛАТЬ ЗАРАБАТЫВАТЬ", "ОБЯЗАТЕЛЬНОСТЬ", "ПУНКТУАЛЬНОСТЬ;ПУНКТУАЛЬНЫЙ", "ГРАМОТНОСТЬ", "ИНИЦИАТИВНОСТЬ;ИНИЦИАТИВНЫЙ", "ОРГАНИЗОВАННОСТЬ", "АККУРАТНОСТЬ;АККУРАТНЫЙ", "ВНИМАТЕЛЬНОСТЬ;ВНИМАТЕЛЬНЫЙ", "ДИСЦИПЛИНИРОВАННОСТЬ;ДИСЦИПЛИНИРОВАННЫЙ;ПОВЫШЕННЫЕ ТРЕБОВАНИЯ К ДИСЦИПЛИНЕ", "БЕЗ ВРЕДНЫХ ПРИВЫЧЕК;ОТСУТСТВИЕ ВРЕДНЫХ ПРИВЫЧЕК;ВРЕДНЫЕ ПРИВЫЧКИ ОТСУТСТВУЮТ"}) 
            {
                string[] pp = s.Split(';');
                Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(pp[0]) { Tag = VacanceTokenType.Moral };
                for (int ii = 1; ii < pp.Length; ii++) 
                {
                    te.AddVariant(pp[ii], false);
                }
                m_Termins.Add(te);
            }
            foreach (string s in new string[] {"ОПЫТ", "ЗНАНИЕ", "ВЛАДЕНИЕ", "НАВЫК", "УМЕНИЕ", "ПОНИМАНИЕ", "ОРГАНИЗАТОРСКИЕ НАВЫКИ", "ОРГАНИЗАТОРСКИЕ СПОСОБНОСТИ", "ПОЛЬЗОВАТЕЛЬ ПК"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Skill });
            }
            foreach (string s in new string[] {"НУЖНО", "НЕОБХОДИМО", "ТРЕБОВАТЬСЯ", "НАЛИЧИЕ", "ДЛЯ РАБОТЫ ТРЕБУЕТСЯ", "ОБЯЗАТЕЛЬНО", "ОБЯЗАТЕЛЕН"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Skill, Tag2 = true });
            }
            foreach (string s in new string[] {"ЖЕЛАТЕЛЬНО", "ПРИВЕТСТВОВАТЬСЯ", "ЯВЛЯТЬСЯ ПРЕИМУЩЕСТВОМ", "КАК ПЛЮС", "БУДЕТ ПРЕИМУЩЕСТВОМ", "БУДЕТ ЯВЛЯТЬСЯ ПРЕИМУЩЕСТВОМ", "МЫ ЦЕНИМ"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Plus, IgnoreTermsOrder = true, Tag2 = true });
            }
            foreach (string s in new string[] {"НЕЗАМЕНИМЫЙ ОПЫТ", "ОСТАВИТЬ ОТЗЫВ", "КЛЮЧЕВЫЕ НАВЫКИ", "ПОЛНАЯ ЗАНЯТОСТЬ", "КОРПОРАТИВНЫЕ ЗАНЯТИЯ", "КОМПЕНСАЦИЯ", "ОПЛАТА БОЛЬНИЧНЫХ", "ПРЕМИЯ", "ВОЗМОЖНОСТЬ ПОЛУЧИТЬ", "УСЛОВИЯ ДЛЯ", "СПЕЦИАЛЬНЫЕ НАВЫКИ И ЗНАНИЯ", "ПРОГРАММА ЛОЯЛЬНОСТИ", "СИСТЕМА ЛОЯЛЬНОСТИ", "КОРПОРАТИВНЫЙ", "ИНТЕРЕСНАЯ РАБОТА", "НА ПОСТОЯННУЮ РАБОТУ", "ПРОФСОЮЗ"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Dummy });
            }
            foreach (string s in new string[] {"ВАКАНСИЯ В АРХИВЕ", "В АРХИВЕ С"}) 
            {
                m_Termins.Add(new Pullenti.Ner.Core.Termin(s) { Tag = VacanceTokenType.Expired });
            }
        }
    }
}
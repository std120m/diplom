/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Vacance
{
    /// <summary>
    /// Анализатор вакансий (специфический анализатор)
    /// </summary>
    public class VacanceAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("VACANCE")
        /// </summary>
        public const string ANALYZER_NAME = "VACANCE";
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
                return "Вакансия";
            }
        }
        public override string Description
        {
            get
            {
                return "Текст содержит одну вакансию";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new VacanceAnalyzer();
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
                return new Pullenti.Ner.Metadata.ReferentClass[] {MetaVacance.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(MetaVacance.ImageId.ToString(), Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("vacance.png"));
                return res;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == VacanceItemReferent.OBJ_TYPENAME) 
                return new VacanceItemReferent();
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
            List<Pullenti.Ner.Vacance.Internal.VacanceToken> li = Pullenti.Ner.Vacance.Internal.VacanceToken.TryParseList(kit.FirstToken);
            if (li == null || (li.Count < 1)) 
                return;
            bool isExpired = false;
            foreach (Pullenti.Ner.Vacance.Internal.VacanceToken v in li) 
            {
                if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Expired) 
                    isExpired = true;
            }
            bool hasDate = false;
            bool hasSkills = false;
            foreach (Pullenti.Ner.Vacance.Internal.VacanceToken v in li) 
            {
                if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Undefined || v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Dummy) 
                    continue;
                if (string.IsNullOrEmpty(v.Value) && v.Refs.Count == 0) 
                    continue;
                VacanceItemReferent it = new VacanceItemReferent();
                if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Date) 
                {
                    it.Typ = VacanceItemType.Date;
                    hasDate = true;
                }
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Expierence) 
                    it.Typ = VacanceItemType.Experience;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Money) 
                    it.Typ = VacanceItemType.Money;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Name) 
                {
                    it.Typ = VacanceItemType.Name;
                    if (isExpired) 
                        it.Expired = true;
                }
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Education) 
                    it.Typ = VacanceItemType.Education;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Language) 
                    it.Typ = VacanceItemType.Language;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Driving) 
                    it.Typ = VacanceItemType.DrivingLicense;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.License) 
                    it.Typ = VacanceItemType.License;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Moral) 
                    it.Typ = VacanceItemType.Moral;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Plus) 
                    it.Typ = VacanceItemType.Plus;
                else if (v.Typ == Pullenti.Ner.Vacance.Internal.VacanceTokenType.Skill) 
                {
                    it.Typ = VacanceItemType.Skill;
                    hasSkills = true;
                }
                else 
                    continue;
                if (v.Value != null) 
                    it.Value = v.Value;
                foreach (Pullenti.Ner.Referent r in v.Refs) 
                {
                    it.AddSlot(VacanceItemReferent.ATTR_REF, r, false, 0);
                }
                Pullenti.Ner.ReferentToken rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(it), v.BeginToken, v.EndToken);
                kit.EmbedToken(rt);
            }
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
                MetaVacance.Initialize();
                Pullenti.Ner.Vacance.Internal.VacanceToken.Initialize();
                Pullenti.Ner.ProcessorService.RegisterAnalyzer(new VacanceAnalyzer());
            }
        }
    }
}
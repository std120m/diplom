/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Vacance
{
    internal class MetaVacance : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaVacance();
            Types = GlobalMeta.AddFeature(VacanceItemReferent.ATTR_TYPE, "Тип", 1, 1);
            Types.AddValue(VacanceItemType.Name.ToString().ToLower(), "Наименование", null, null);
            Types.AddValue(VacanceItemType.Date.ToString().ToLower(), "Дата", null, null);
            Types.AddValue(VacanceItemType.Education.ToString().ToLower(), "Образование", null, null);
            Types.AddValue(VacanceItemType.Experience.ToString().ToLower(), "Опыт работы", null, null);
            Types.AddValue(VacanceItemType.Language.ToString().ToLower(), "Язык", null, null);
            Types.AddValue(VacanceItemType.Money.ToString().ToLower(), "Зарплата", null, null);
            Types.AddValue(VacanceItemType.DrivingLicense.ToString().ToLower(), "Водительские права", null, null);
            Types.AddValue(VacanceItemType.License.ToString().ToLower(), "Лицензия", null, null);
            Types.AddValue(VacanceItemType.Moral.ToString().ToLower(), "Моральное требование", null, null);
            Types.AddValue(VacanceItemType.Skill.ToString().ToLower(), "Требование", null, null);
            Types.AddValue(VacanceItemType.Plus.ToString().ToLower(), "Пожелание", null, null);
            GlobalMeta.AddFeature(VacanceItemReferent.ATTR_VALUE, "Значение", 0, 1);
            GlobalMeta.AddFeature(VacanceItemReferent.ATTR_REF, "Ссылка", 0, 0);
            GlobalMeta.AddFeature(VacanceItemReferent.ATTR_EXPIRED, "Признак неактуальности", 0, 1);
        }
        public static Pullenti.Ner.Metadata.Feature Types;
        public override string Name
        {
            get
            {
                return VacanceItemReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Вакансия";
            }
        }
        public static string ImageId = "vacance";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        public static MetaVacance GlobalMeta;
    }
}
/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Resume
{
    internal class MetaResume : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaResume();
            Types = GlobalMeta.AddFeature(ResumeItemReferent.ATTR_TYPE, "Тип", 1, 1);
            Types.AddValue(ResumeItemType.Position.ToString().ToLower(), "Позиция", null, null);
            Types.AddValue(ResumeItemType.Age.ToString().ToLower(), "Возраст", null, null);
            Types.AddValue(ResumeItemType.Sex.ToString().ToLower(), "Пол", null, null);
            Types.AddValue(ResumeItemType.Education.ToString().ToLower(), "Образование", null, null);
            Types.AddValue(ResumeItemType.Experience.ToString().ToLower(), "Опыт работы", null, null);
            Types.AddValue(ResumeItemType.Language.ToString().ToLower(), "Язык", null, null);
            Types.AddValue(ResumeItemType.Money.ToString().ToLower(), "Зарплата", null, null);
            Types.AddValue(ResumeItemType.DrivingLicense.ToString().ToLower(), "Водительские права", null, null);
            Types.AddValue(ResumeItemType.License.ToString().ToLower(), "Лицензия", null, null);
            Types.AddValue(ResumeItemType.Speciality.ToString().ToLower(), "Специальность", null, null);
            Types.AddValue(ResumeItemType.Moral.ToString().ToLower(), "Моральное качество", null, null);
            Types.AddValue(ResumeItemType.Skill.ToString().ToLower(), "Навык", null, null);
            Types.AddValue(ResumeItemType.Hobby.ToString().ToLower(), "Хобби", null, null);
            GlobalMeta.AddFeature(ResumeItemReferent.ATTR_VALUE, "Значение", 0, 1);
            GlobalMeta.AddFeature(ResumeItemReferent.ATTR_REF, "Ссылка", 0, 0);
            GlobalMeta.AddFeature(ResumeItemReferent.ATTR_EXPIRED, "Признак неактуальности", 0, 1);
        }
        public static Pullenti.Ner.Metadata.Feature Types;
        public override string Name
        {
            get
            {
                return ResumeItemReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Резюме";
            }
        }
        public static string ImageId = "resume";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        public static MetaResume GlobalMeta;
    }
}
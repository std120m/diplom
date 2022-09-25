/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Resume
{
    /// <summary>
    /// Элемент резюме
    /// </summary>
    public class ResumeItemReferent : Pullenti.Ner.Referent
    {
        public ResumeItemReferent() : base(OBJ_TYPENAME)
        {
            InstanceOf = MetaResume.GlobalMeta;
        }
        /// <summary>
        /// Имя типа сущности TypeName ("RESUME")
        /// </summary>
        public const string OBJ_TYPENAME = "RESUME";
        /// <summary>
        /// Имя атрибута - тип элемента (см. ResumeItemType)
        /// </summary>
        public const string ATTR_TYPE = "TYPE";
        /// <summary>
        /// Имя атрибута - значение
        /// </summary>
        public const string ATTR_VALUE = "VALUE";
        /// <summary>
        /// Имя атрибута - ссылка на сущность
        /// </summary>
        public const string ATTR_REF = "REF";
        /// <summary>
        /// Имя атрибута - признак снятия резюме
        /// </summary>
        public const string ATTR_EXPIRED = "EXPIRED";
        /// <summary>
        /// Тип элемента
        /// </summary>
        public ResumeItemType Typ
        {
            get
            {
                string str = this.GetStringValue(ATTR_TYPE);
                if (str == null) 
                    return ResumeItemType.Undefined;
                try 
                {
                    return (ResumeItemType)Enum.Parse(typeof(ResumeItemType), str, true);
                }
                catch(Exception ex4380) 
                {
                }
                return ResumeItemType.Undefined;
            }
            set
            {
                this.AddSlot(ATTR_TYPE, value.ToString().ToLower(), true, 0);
            }
        }
        /// <summary>
        /// Значение элемента
        /// </summary>
        public string Value
        {
            get
            {
                return this.GetStringValue(ATTR_VALUE);
            }
            set
            {
                this.AddSlot(ATTR_VALUE, value, true, 0);
            }
        }
        /// <summary>
        /// Ссылка на сущность, если есть
        /// </summary>
        public Pullenti.Ner.Referent Ref
        {
            get
            {
                return this.GetSlotValue(ATTR_REF) as Pullenti.Ner.Referent;
            }
            set
            {
                this.AddSlot(ATTR_REF, value, true, 0);
            }
        }
        /// <summary>
        /// Признак снятия вакансии
        /// </summary>
        public bool Expired
        {
            get
            {
                return this.GetStringValue(ATTR_EXPIRED) == "true";
            }
            set
            {
                this.AddSlot(ATTR_EXPIRED, (value ? "true" : null), true, 0);
            }
        }
        public override string ToStringEx(bool shortVariant, Pullenti.Morph.MorphLang lang = null, int lev = 0)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("{0}: ", MetaResume.Types.ConvertInnerValueToOuterValue(this.GetStringValue(ATTR_TYPE), null));
            if (Value != null) 
                tmp.Append(Value);
            else if (Ref != null) 
                tmp.Append(Ref.ToStringEx(shortVariant, lang, lev + 1));
            if (Expired) 
                tmp.Append(" (не актуально)");
            return tmp.ToString();
        }
        public override Pullenti.Ner.Core.IntOntologyItem CreateOntologyItem()
        {
            Pullenti.Ner.Core.IntOntologyItem oi = new Pullenti.Ner.Core.IntOntologyItem(this);
            oi.Termins.Add(new Pullenti.Ner.Core.Termin(Value));
            return oi;
        }
    }
}
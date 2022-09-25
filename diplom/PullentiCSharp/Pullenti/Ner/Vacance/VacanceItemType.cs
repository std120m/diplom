/*
 * SDK Pullenti Lingvo, version 4.14, september 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Vacance
{
    /// <summary>
    /// Тип элемента вакансии
    /// </summary>
    public enum VacanceItemType : int
    {
        Undefined,
        /// <summary>
        /// Наименование вакансии
        /// </summary>
        Name,
        /// <summary>
        /// Актуальная дата
        /// </summary>
        Date,
        /// <summary>
        /// Предлагаемая зарплата
        /// </summary>
        Money,
        /// <summary>
        /// Требуемое образование
        /// </summary>
        Education,
        /// <summary>
        /// Требуемый опыт работы
        /// </summary>
        Experience,
        /// <summary>
        /// Язык(и)
        /// </summary>
        Language,
        /// <summary>
        /// Водительские права
        /// </summary>
        DrivingLicense,
        /// <summary>
        /// Какое-то иное удостоверение или права на что-либо
        /// </summary>
        License,
        /// <summary>
        /// Моральное качество
        /// </summary>
        Moral,
        /// <summary>
        /// Требуемый навык
        /// </summary>
        Skill,
        /// <summary>
        /// Дополнительный навык (пожелание)
        /// </summary>
        Plus,
    }
}
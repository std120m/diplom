using System.Text.Json.Serialization;

namespace diplom.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameRu { get; set; }
        [JsonIgnore]
        public virtual ICollection<Share> Shares { get; set; }

        private Dictionary<string, string> sectorsNameReadable = new Dictionary<string, string>()
        {
            { "Consumer", "Потребительские товары и услуги" },
            { "It", "Информационные технологии" },
            { "Health_care", "Здравоохранение" },
            { "Green_energy", "Зеленая энергетика" },
            { "Ecomaterials", "Материалы для эко-технологии" },
            { "Real_estate", "Недвижимость" },
            { "Materials", "Сырьевая промышленность" },
            { "Telecom", "Телекоммуникации" },
            { "Financial", "Финансовый сектор" },
            { "Electrocars", "Электротранспорт и комплектующие" },
            { "Utilities", "Электроэнергетика" },
            { "Energy", "Энергетика" },
            { "Green_buildings", "Энергоэффективные здания" },
            { "industrials", "Машиностроение и транспорт" },
            { "Other", "Другое" },
    };

        public Sector(string? name)
        {
            Shares = new List<Share>();
            Name = name;
            string nameRu;
            if (name != null && this.sectorsNameReadable.TryGetValue(name, out nameRu))
            {
                NameRu = nameRu;
            } else
            {
                NameRu = null;
            };
        }
    }
}

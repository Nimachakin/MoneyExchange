namespace ExchangeApi.Models.MoneyExchangeModels
{
    /// <summary>
    /// Хранилище купюр автомата по обмену мелочи
    /// </summary>
    public class BanknoteStock
    {
        /// <summary>
        /// Стоимость хранимых купюр
        /// </summary>
        /// <value></value>
        public int Value { get; private set; }

        /// <summary>
        /// Ёмкость хранилища купюр
        /// </summary>
        /// <value></value>
        public int Capacity { get; private set; }

        /// <summary>
        /// Количество купюр в хранилище
        /// </summary>
        /// <value></value>
        public int Count { get; private set; }

        /// <summary>
        /// Определяет, нет ли купюр в хранилище
        /// </summary>
        /// <value></value>
        public bool Empty { get => Count == 0; }

        /// <summary>
        /// Конструктор хранилища купюр: 
        /// value - стоимость купюры, 
        /// capacity - ёмкость хранилища
        /// </summary>
        /// <param name="value">стоимость купюры</param>
        /// <param name="capacity">ёмкость хранилища</param>
        public BanknoteStock(int value, int capacity)
        {
            // Value = value;
            // Capacity = capacity;
            // Count = capacity;
            SetParameters(value, capacity);
        }

        /// <summary>
        /// Выдаёт купюру из хранилища купюр
        /// </summary>
        public void GiveBanknote()
        {
            Count--;
        }

        /// <summary>
        /// Задаёт настройки хранилища купюр: 
        /// value - стоимость купюры, 
        /// capacity - ёмкость хранилища
        /// </summary>
        /// <param name="value">стоимость купюры</param>
        /// <param name="capacity">ёмкость хранилища</param>
        public void SetParameters(int value, int capacity)
        {
            Value = value > 0 ? value : 10;
            Capacity = capacity > 0 ? capacity : 20;
            Count = Capacity;
        }
    }
}
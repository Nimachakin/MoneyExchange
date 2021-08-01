namespace ExchangeApi.Models.MoneyExchangeModels
{
    /// <summary>
    /// Монетоприёмник автомата по обмену мелочи
    /// </summary>
    public class CoinReceiver
    {
        /// <summary>
        /// Стоимость принимаемых монет
        /// </summary>
        /// <value></value>
        public int Value { get; private set; }

        /// <summary>
        /// Ёмкость монетоприёмника
        /// </summary>
        /// <value></value>
        public int Capacity { get; private set; }

        /// <summary>
        /// Количество монет в монетоприёмнике
        /// </summary>
        /// <value></value>
        public int Count { get; private set; }

        /// <summary>
        /// Определяет свободное место в монетоприёмнике
        /// </summary>
        /// <value></value>
        public int FreeSpace { get => Capacity - Count; }

        /// <summary>
        /// Сумма монет, которую можно передать в приёмник
        /// </summary>
        /// <value></value>
        public int AviableSumToStore { get => Value * FreeSpace; }

        /// <summary>
        /// Определяет, заполнен ли монетоприёмник
        /// </summary>
        /// <value></value>
        public bool Full { get => Count == Capacity; }

        /// <summary>
        /// Конструктор монетоприёмника: 
        /// value - стоимость монеты, 
        /// capacity - ёмкость приёмника
        /// </summary>
        /// <param name="value">стоимость монеты</param>
        /// <param name="capacity">ёмкость приёмника</param>
        public CoinReceiver(int value, int capacity)
        {
            // Value = value;
            // Capacity = capacity;
            // Count = 0;
            SetParameters(value, capacity);
        }
        
        /// <summary>
        /// Помещает монету в монетоприёмник
        /// </summary>
        public void StoreCoin()
        {
            Count++;
        }

        /// <summary>
        /// Задаёт настройки монетоприёмника: 
        /// value - стоимость монеты, 
        /// capacity - ёмкость монетоприёмника
        /// </summary>
        /// <param name="value">стоимость монеты</param>
        /// <param name="capacity">ёмкость монетоприёмника</param>
        public void SetParameters(int value, int capacity)
        {
            Value = value > 0 ? value : 1;
            Capacity = capacity > 0 ? capacity : 100;
            Count = 0;
        }
    }
}
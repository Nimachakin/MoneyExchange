using System;

namespace ExchangeApi.Models.DatabaseModels
{
    public class ExchangeLog
    {
        // public const string ExchangeText = 
        //     "В автомате: монет ценой {0} руб. - {1} шт. из {2}, монет ценой {3} руб. - {4} шт. из {5}, " + 
        //     "купюр ценой {6} руб. - {7} шт. из {8}, купюр ценой {9} руб. - {10} шт. из {11}";
        public const string USER_INPUT_TEXT = 
            "Внесено монет: {0} руб. - {1}, {2} руб. - {3}";
        public const string EXCHANGE_RESULT_TEXT = 
            "Выдано купюр: {0} руб. - {1}, {2} руб. - {3}. " + 
            "Выдано монет: {4} руб. - {5}, {6} руб. - {7}";
        public const string MACHINE_STATUS_TEXT = 
            "Монеты: {0} руб. - {1} шт. из {2}; {3} руб. - {4} шт. из {5}. " + 
            "Купюры: {6} руб. - {7} шт. из {8}, {9} руб. - {10} шт. из {11}";
        public int Id { get; set; }
        public string UserInput { get; set; }
        public string ExchangeResult { get; set; }
        public string BeforeExchangeData { get; set; }
        public string AfterExchangeData { get; set; }
        public DateTime ExchangeDate { get; set; }

    }
}
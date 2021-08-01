using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExchangeApi.Models.DatabaseModels;

namespace ExchangeApi.Models.MoneyExchangeModels
{
    /// <summary>
    /// Автомат по обмену мелочи
    /// </summary>
    public class ExchangeMachine
    {
        /// <summary>
        /// Монетоприёмник крупных монет
        /// </summary>
        /// <value></value>
        public CoinReceiver BigCoinReceiver { get; private set; } 

        /// <summary>
        /// Монетоприёмник небольших монет
        /// </summary>
        /// <value></value>
        public CoinReceiver SmallCoinReceiver { get; private set; } 

        /// <summary>
        /// Хранилище крупных купюр
        /// </summary>
        /// <value></value>
        public BanknoteStock BigBanknoteStock { get; private set; } 

        /// <summary>
        /// Хранилище небольших купюр
        /// </summary>
        /// <value></value>
        public BanknoteStock SmallBanknoteStock { get; private set; } 

        /// <summary>
        /// Карман для выдачи денег пользователю 
        /// как результат обмена
        /// </summary>
        /// <value></value>
        public List<int> MoneybackStorage { get; private set; }

        public ExchangeLog LogInfo { get; private set; }

        public ExchangeMachine()
        {
            BigCoinReceiver = new CoinReceiver(10, 50); 
            SmallCoinReceiver = new CoinReceiver(5, 100); 
            BigBanknoteStock = new BanknoteStock(100, 10); 
            SmallBanknoteStock = new BanknoteStock(50, 20); 
            MoneybackStorage = new List<int>();
            LogInfo = new ExchangeLog();
        }

        /// <summary>
        /// Производит обмен монет на купюры
        /// </summary>
        /// <param name="coinsInput">Набор монет, помещённых в приёмное отделение автомата</param>
        /// <returns>Возвращает набор купюр и оставшихся монет</returns>
        public List<int> Exchange(List<int> coinsInput)
        {
            try
            {
                if(!IsExchangePossible(coinsInput))
                {
                    LogInfo = null;
                    return coinsInput;
                }
                
                PrepareLogInfo(coinsInput);
                // количество монет на входе большего номинала
                int bigCoinsInputCount = coinsInput.Where(c => c == BigCoinReceiver.Value).Count();
                // количество монет на входе меньшего номинала
                int smallCoinsInputCount = coinsInput.Count - bigCoinsInputCount;
                int bigCoinsToStoreCount = 0; // количество монет, которые попадут в монетоприёмник крупных монет
                int smallCoinsToStoreCount = 0; // количество монет, которые попадут в монетоприёмник мелких монет

                DistributeExtraInput(BigCoinReceiver, ref bigCoinsInputCount, ref bigCoinsToStoreCount);
                DistributeExtraInput(SmallCoinReceiver, ref smallCoinsInputCount, ref smallCoinsToStoreCount);
                
                // предварительная сумма монет для монетоприёмников автомата
                int coinsToStoreSum = 
                    (bigCoinsToStoreCount * BigCoinReceiver.Value) + 
                    (smallCoinsToStoreCount * SmallCoinReceiver.Value); 
                // предварительная сумма монет на возврат пользователю
                int coinsToReturnSum = coinsToStoreSum;

                DistributeBanknotes(BigBanknoteStock, ref coinsToReturnSum);
                DistributeBanknotes(SmallBanknoteStock, ref coinsToReturnSum);

                DistributeCoins(BigCoinReceiver, ref coinsToReturnSum, ref bigCoinsToStoreCount);
                coinsToReturnSum = coinsToReturnSum % BigCoinReceiver.Value;
                DistributeCoins(SmallCoinReceiver, ref coinsToReturnSum, ref smallCoinsToStoreCount);

                LogExchangeInfo();
                List<int> result = MoneybackStorage.OrderByDescending(c => c).ToList();
                MoneybackStorage.Clear();
                return result;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MoneybackStorage.Clear();
                return coinsInput;
            }
        }

        /// <summary>
        /// Рассчитывает и передаёт в карман для выдачи денег пользователю 
        /// монеты, которые не удастся поместить в указанный монетоприёмник
        /// </summary>
        /// <param name="coinReceiver">монетоприёмник</param>
        /// <param name="coinsInputCount">монеты, подаваемые на вход пользователем</param>
        /// <param name="coinsForReceiverCount">монеты, которые можно поместить в монетоприёмник</param>
        private void DistributeExtraInput(
            CoinReceiver coinReceiver, 
            ref int coinsInputCount, 
            ref int coinsForReceiverCount)
        {
            // монет определённого номинала на входе больше, чем можно поместить в монетоприёмник данного номинала
            if(coinsInputCount > coinReceiver.FreeSpace)
            {
                coinsForReceiverCount = coinReceiver.FreeSpace;
                coinsInputCount = coinsInputCount - coinsForReceiverCount;

                for(int i = 0; i < coinsInputCount; i++) // фиксируем в хранилище на возврат лишние монеты
                {
                    MoneybackStorage.Add(coinReceiver.Value);
                }
            }
            else
            {
                coinsForReceiverCount = coinsInputCount; 
            }
        }

        /// <summary>
        /// Рассчитывает и передаёт в карман для выдачи денег пользователю 
        /// купюры на обмен мелочи из хранилища купюр
        /// </summary>
        /// <param name="banknoteStock">хранилище купюр</param>
        /// <param name="coinsToReturnSum">предварительная сумма монет на возврат пользователю</param>
        private void DistributeBanknotes(BanknoteStock banknoteStock, ref int coinsToReturnSum)
        {
            if(coinsToReturnSum >= banknoteStock.Value) // сумма монет для монетоприёмников выше стоимости большей купюры
            {
                int banknotesToTake = coinsToReturnSum / banknoteStock.Value;

                if(banknotesToTake >= banknoteStock.Count) // в хранилище купюр их количество меньше, чем можно было бы взять
                {
                    banknotesToTake = banknoteStock.Count;
                    coinsToReturnSum = 
                        coinsToReturnSum - 
                        (banknoteStock.Count * banknoteStock.Value);
                }
                else
                {
                    coinsToReturnSum = coinsToReturnSum % banknoteStock.Value;
                }
                
                for(int i = 0; i < banknotesToTake; i++)
                {
                    MoneybackStorage.Add(banknoteStock.Value);
                    banknoteStock.GiveBanknote();
                }
            }
        }

        /// <summary>
        /// Рассчитывает и передаёт в указанный монетоприёмник 
        /// монеты, внесённые пользователем
        /// </summary>
        /// <param name="coinReceiver">монетоприёмник</param>
        /// <param name="coinsToReturnSum">предварительная сумма монет на возврат пользователю</param>
        /// <param name="coinsToStoreCount">предварительное количество монет для монетоприёмника</param>
        private void DistributeCoins(CoinReceiver coinReceiver, ref int coinsToReturnSum, ref int coinsToStoreCount)
        {
            int coinsToReturnCount = coinsToReturnSum / coinReceiver.Value;

            for(int i = 0; i < coinsToReturnCount; i++)
            {
                MoneybackStorage.Add(coinReceiver.Value);
            }

            coinsToStoreCount = coinsToStoreCount - coinsToReturnCount;

            for(int i = 0; i < coinsToStoreCount; i++)
            {
                coinReceiver.StoreCoin();
            }
        }

        /// <summary>
        /// Проверяет условия, при которых обмен мелочи автоматом невозможен. 
        /// coinsInput - монеты, подаваемые на вход пользователем
        /// </summary>
        /// <param name="coinsInput">монеты, подаваемые на вход пользователем</param>
        /// <returns></returns>
        private bool IsExchangePossible(List<int> coinsInput)
        {
            bool result = true;
            
            if(BigCoinReceiver.Full && SmallCoinReceiver.Full) // монетоприёмники автомата заполнены
            {
                result = false;
            }
            
            if(BigBanknoteStock.Empty && SmallBanknoteStock.Empty) // хранилища купюр автомата пусты
            {
                result = false;
            }

            int sumToExchange = coinsInput.Sum(); // общая сумма монет на входе

            // монет на обмен нет или их сумма меньше стоимости купюры хранилища меньших купюр
            if(coinsInput.Count == 0 || sumToExchange < SmallBanknoteStock.Value) 
            {
                result = false;
            } 

            // общая сумма монет на приём в свободное место монетоприёмников автомата
            int totalAviableCoinsToStoreSum = 
                BigCoinReceiver.AviableSumToStore + SmallCoinReceiver.AviableSumToStore; 
            
            if(totalAviableCoinsToStoreSum < SmallBanknoteStock.Value) // coinsToReceiveSum меньше стоимости меньшей купюры
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Собирает и фиксирует в систему логгирования информацию 
        /// о состоянии автомата перед процедурой обмена мелочи. 
        /// userInput - монеты, подаваемые на вход пользователем
        /// </summary>
        /// <param name="coinsInput">монеты, подаваемые на вход пользователем</param>
        private void PrepareLogInfo(List<int> coinsInput)
        {
            LogInfo = new ExchangeLog();
            LogInfo.UserInput = GetUserInputData(coinsInput);
            LogInfo.BeforeExchangeData = GetExchangeMachineState();
        }

        /// <summary>
        /// Собирает и фиксирует в систему логгирования информацию 
        /// о состоянии автомата после процедуры обмена мелочи
        /// </summary>
        private void LogExchangeInfo()
        {
            LogInfo.ExchangeResult = GetExchangeResultText();
            LogInfo.AfterExchangeData = GetExchangeMachineState();
            LogInfo.ExchangeDate = DateTime.Now;            
        }

        /// <summary>
        /// Возвращает информацию о состоянии монетоприёмников 
        /// и хранилищ купюр автомата 
        /// </summary>
        /// <returns></returns>
        private string GetExchangeMachineState()
        {
            string result = string.Format(
                ExchangeLog.MACHINE_STATUS_TEXT, 
                BigCoinReceiver.Value, 
                BigCoinReceiver.Count, 
                BigCoinReceiver.Capacity, 
                SmallCoinReceiver.Value, 
                SmallCoinReceiver.Count, 
                SmallCoinReceiver.Capacity, 
                BigBanknoteStock.Value, 
                BigBanknoteStock.Count, 
                BigBanknoteStock.Capacity, 
                SmallBanknoteStock.Value, 
                SmallBanknoteStock.Count, 
                SmallBanknoteStock.Capacity);
            return result;
        }

        /// <summary>
        /// Возвращает информацию о монетах,  
        /// поданых на вход автомата пользователем
        /// для обмена мелочи 
        /// </summary>
        /// <param name="coinsInput">монеты, подаваемые на вход пользователем</param>
        /// <returns></returns>
        private string GetUserInputData(List<int> coinsInput)
        {
            int bigCoinsCount = coinsInput
                .Where(c => c == BigCoinReceiver.Value).Count();
            int smallCoinsCount = coinsInput.Count - bigCoinsCount;
            string result = string.Format(
                ExchangeLog.USER_INPUT_TEXT, 
                BigCoinReceiver.Value, 
                bigCoinsCount,
                SmallCoinReceiver.Value, 
                smallCoinsCount);
            return result;
        }

        /// <summary>
        /// Возвращает информацию о деньгах,  
        /// выданых пользователю как результат обмена  
        /// </summary>
        /// <returns></returns>
        private string GetExchangeResultText()
        {
            int bigBanknotesCount = MoneybackStorage
                .Where(c => c == BigBanknoteStock.Value).Count();
            int smallBanknotesCount = MoneybackStorage
                .Where(c => c == SmallBanknoteStock.Value).Count();
            int bigCoinsCount = MoneybackStorage
                .Where(c => c == BigCoinReceiver.Value).Count();
            int smallCoinsCount = MoneybackStorage
                .Where(c => c == SmallCoinReceiver.Value).Count();
            string result = string.Format(
                ExchangeLog.EXCHANGE_RESULT_TEXT, 
                BigBanknoteStock.Value, bigBanknotesCount,
                SmallBanknoteStock.Value, smallBanknotesCount, 
                BigCoinReceiver.Value, bigCoinsCount, 
                SmallCoinReceiver.Value, smallCoinsCount);
            return result;
        }

        public void Restart()
        {
            BigCoinReceiver = new CoinReceiver(10, 50); 
            SmallCoinReceiver = new CoinReceiver(5, 100); 
            BigBanknoteStock = new BanknoteStock(100, 10); 
            SmallBanknoteStock = new BanknoteStock(50, 20); 
            MoneybackStorage = new List<int>();
            LogInfo = new ExchangeLog();
        }
    }
}
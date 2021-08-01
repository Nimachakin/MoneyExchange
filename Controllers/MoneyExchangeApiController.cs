using System.Collections.Generic;
using ExchangeApi.Data;
using ExchangeApi.Models.MoneyExchangeModels;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class MoneyExchangeApiController : Controller
    {
        private readonly ExchangeMachine exchangeMachine;
        private readonly ExchangeMachineContext context;

        public MoneyExchangeApiController(ExchangeMachine _exchangeMachine, ExchangeMachineContext db)
        {
            exchangeMachine = _exchangeMachine;
            context = db;
        }

        [HttpPost("exchange")]
        public List<int> Exchange([FromBody] List<int> coins)
        {
            List<int> result = exchangeMachine.Exchange(coins);

            if(exchangeMachine.LogInfo != null)
            {
                context.ExchangeLogs.Add(exchangeMachine.LogInfo); 
                context.SaveChanges();
            }

            return result;
        }

        [HttpPut("restart")]
        public IActionResult RestartExchangeMachine()
        {
            exchangeMachine.Restart();
            return Ok(
                "Машина обновлена! " + 
                "Монетоприёмники опустошены, " + 
                "a хранилища купюр заполнены до отказа.");
        }

        [HttpGet("coins")]
        public IActionResult GetCoinsValues()
        {
            int bigCoinValue = exchangeMachine.BigCoinReceiver.Value;
            int smallCoinValue = exchangeMachine.SmallCoinReceiver.Value;
            string result = string.Format(
                "Автомат принимает монеты номиналом {0} и {1} руб.", 
                bigCoinValue, smallCoinValue);
            return Ok(result);
        }

        [HttpGet("banknotes")]
        public IActionResult GetBanknotesValues()
        {
            int bigBanknoteValue = exchangeMachine.BigBanknoteStock.Value;
            int smallBanknoteValue = exchangeMachine.SmallBanknoteStock.Value;
            string result = string.Format(
                "Автомат выдаёт купюры номиналом {0} и {1} руб.", 
                bigBanknoteValue, smallBanknoteValue);
            return Ok(result);
        }

        [HttpGet("receiver/big")]
        public IActionResult GetBigCoinReceiverInfo()
        {
            int value = exchangeMachine.BigCoinReceiver.Value;
            int count = exchangeMachine.BigCoinReceiver.Count;
            int capacity = exchangeMachine.BigCoinReceiver.Capacity;
            string result = string.Format(
                "Настройки первого монетоприёмника автомата: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpPut("receiver/big")]
        public IActionResult SetBigCoinReceiver(int newValue, int newCapacity)
        {
            exchangeMachine.BigCoinReceiver.SetParameters(newValue, newCapacity);
            int value = exchangeMachine.BigCoinReceiver.Value;
            int count = exchangeMachine.BigCoinReceiver.Count;
            int capacity = exchangeMachine.BigCoinReceiver.Capacity;
            string result = string.Format(
                "Первый монетоприёмник обновлён! " + 
                "Новые настройки: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpGet("receiver/small")]
        public IActionResult GetSmallCoinReceiverInfo()
        {
            int value = exchangeMachine.SmallCoinReceiver.Value;
            int count = exchangeMachine.SmallCoinReceiver.Count;
            int capacity = exchangeMachine.SmallCoinReceiver.Capacity;
            string result = string.Format(
                "Настройки второго монетоприёмника автомата: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpPut("receiver/small")]
        public IActionResult SetSmallCoinReceiver(int newValue, int newCapacity)
        {
            exchangeMachine.SmallCoinReceiver.SetParameters(newValue, newCapacity);
            int value = exchangeMachine.SmallCoinReceiver.Value;
            int count = exchangeMachine.BigCoinReceiver.Count;
            int capacity = exchangeMachine.BigCoinReceiver.Capacity;
            string result = string.Format(
                "Второй монетоприёмник обновлён! " + 
                "Новые настройки: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpGet("stock/big")]
        public IActionResult GetBigBanknotStockInfo()
        {
            int value = exchangeMachine.BigBanknoteStock.Value;
            int count = exchangeMachine.BigBanknoteStock.Count;
            int capacity = exchangeMachine.BigBanknoteStock.Capacity;
            string result = string.Format(
                "Настройки первого хранилища купюр автомата: " + 
                "стоимость купюры - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }        

        [HttpPut("stock/big")]
        public IActionResult SetBigBanknoteStock(int newValue, int newCapacity)
        {
            exchangeMachine.BigBanknoteStock.SetParameters(newValue, newCapacity);
            int value = exchangeMachine.BigBanknoteStock.Value;
            int count = exchangeMachine.BigBanknoteStock.Count;
            int capacity = exchangeMachine.BigBanknoteStock.Capacity;
            string result = string.Format(
                "Первое хранилище купюр обновлёно! " + 
                "Новые настройки: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpGet("stock/small")]
        public IActionResult GetSmallBanknotStockInfo()
        {
            int value = exchangeMachine.SmallBanknoteStock.Value;
            int count = exchangeMachine.SmallBanknoteStock.Count;
            int capacity = exchangeMachine.SmallBanknoteStock.Capacity;
            string result = string.Format(
                "Состояние второго хранилища купюр автомата: " + 
                "стоимость купюры - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }

        [HttpPut("stock/small")]
        public IActionResult SetSmallBanknotStock(int newValue, int newCapacity)
        {
            exchangeMachine.SmallBanknoteStock.SetParameters(newValue, newCapacity);
            int value = exchangeMachine.SmallBanknoteStock.Value;
            int count = exchangeMachine.SmallBanknoteStock.Count;
            int capacity = exchangeMachine.SmallBanknoteStock.Capacity;
            string result = string.Format(
                "Второй монетоприёмник обновлён! " + 
                "Новые настройки: " + 
                "стоимость монет - {0} руб., текущее количество - {1} шт., " + 
                "текущая ёмкость - {2} шт.", 
                value, count, capacity);
            return Ok(result);
        }
    }
}
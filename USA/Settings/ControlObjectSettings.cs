using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCA.ControlObjectSettings
{
    public class PowerSupplyMode
    {

        public enum SupplyVoltageValue
        {
            Nominal,
            Min,
            Max
        }
    }

    public class ControlObjectSettings
    {
        public struct Settings
        {
            public Regime Regime;
            public string Comment;
            public int FactoryNumber;
            public string OperatorName;
        }        

        public static Regime GetRegimeAsEnum(string regimeName)
        {
            switch(regimeName)
            {
                case "Номинальный":
                    return Regime.Nominal;
                case "Минимальный":
                    return Regime.Min;
                case "Максимальный":
                    return Regime.Max;
                default:
                    throw new Exception($"Выбран неизвестный режим - {regimeName}");
            }
        }

        public static string GetRegimeAsString(Regime regime)
        {
            switch (regime)
            {
                case Regime.Max:
                    return "Максимальный";
                case Regime.Min:
                    return "Минимальный";
                case Regime.Nominal:
                    return "Номинальный";
                default:
                    throw new Exception($"Выбран неизвестный режим - {regime}");
            }
        }

        public enum Regime 
        {
            Nominal,
            Min,
            Max
        }

        public enum CheckingType
        {
            SelfChecking,
            Algorithm
        }

        public static string GetCheckingTypeAsString(CheckingType checkingType)
        {
            switch (checkingType)
            {
                case CheckingType.SelfChecking:
                    return "Самопроверка";
                case CheckingType.Algorithm:
                    return "Алгоритм";
                default:
                    throw new Exception($"Выбран неизвестный тип проверки - {checkingType}");
            }
        }
    }
}

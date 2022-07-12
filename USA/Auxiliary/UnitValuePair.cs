using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker.Auxiliary
{
    public class UnitValuePair
    {
        public static string GetValueUnitPair(double value, UnitType unitType)
        {
            if (Math.Abs(value) * Math.Pow(10, 6) < 1000)
            {
                return $"{Math.Round(value * Math.Pow(10, 6), 3)} мк{GetUnit(value, unitType)}";
            }
            else if (Math.Abs(value) * Math.Pow(10, 3) < 1000)
            {
                return $"{Math.Round(value * Math.Pow(10, 3), 3)} м{GetUnit(value, unitType)}";
            }
            else
            {
                return $"{Math.Round(value, 3)} {GetUnit(value, unitType)}";
            }
        }

        private static string GetUnit(double value, UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Current:
                    return "А";
                case UnitType.Voltage:
                    return "В";
                case UnitType.Power:
                    return "Вт";
                case UnitType.Frequency:
                    return "Гц";
                case UnitType.Resistance:
                    return "Ом";
                default:
                    throw new Exception("Неизвестный тип величины");
            }
        }

        public enum UnitType
        {
            Voltage, 
            Current,
            Power,
            Frequency,
            Resistance
        }

        public static UnitType GetUnitType (string unitName)
        {
            switch (unitName)
            {
                case "V":
                    return UnitType.Voltage;
                case "A":
                    return UnitType.Current;
                case "W":
                    return UnitType.Power;
                case "Hz":
                    return UnitType.Frequency;
                case "Ohm":
                    return UnitType.Resistance;
                default:
                    throw new Exception("Неизвестный тип величины");
            }
        }

        public class ValueKeys
        {
            public UnitType UnitType { get; set; }
            public string[] Keys { get; set; }
            public ValueKeys(UnitType unitType, string[] keys)
            {
                UnitType = unitType;
                Keys = keys;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCA.Auxiliary
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
                default:
                    throw new Exception("Неизвестный тип величины");
            }
        }

        public enum UnitType
        {
            Voltage, 
            Current,
            Power,
            Frequency
        }
    }
}

﻿using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using UCA.DeviceDrivers;

namespace UCA.Devices
{
    class Commutator_device : IDeviceInterface
    {
        readonly Commutator Commutator;

        public Commutator_device(SerialPort serialPort)
        {
            Commutator = new Commutator(serialPort);
        }

        /// дописать OpenAllRelays
        public DeviceResult DoCommand(DeviceData deviceData)
        {
            switch (deviceData.Command)
            {
                case DeviceCommands.CloseRelays:
                    string[] closableRelays = deviceData.Argument.Replace(" ", "").Split(',');
                    Commutator.CloseRelays(closableRelays);
                    // Проверим, всё ли замкнулось:
                    var actualClosedRelayNames = Commutator.GetClosedRelayNames();
                    bool isAllTheseRelaysClosed = false;
                    string relayName = "relay";
                    foreach (var relay in closableRelays)
                    {
                        isAllTheseRelaysClosed = actualClosedRelayNames.Contains(relay);
                        if (!isAllTheseRelaysClosed)
                        {
                            relayName = relay;
                            break;
                        }
                    }
                    if (isAllTheseRelaysClosed)
                    {
                        return DeviceResult.ResultOk("Замыкание реле прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: реле {relayName} не замкнуто");
                    }
                case DeviceCommands.OpenRelays:
                    string[] breakableRelays = deviceData.Argument.Replace(" ", "").Split(',');
                    Commutator.OpenRelays(breakableRelays);
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames1 = Commutator.GetClosedRelayNames();
                    bool anyOfTheseRelaysClosed = false;
                    var closedRelayName = "relay";
                    foreach (var relay in breakableRelays)
                    {
                        anyOfTheseRelaysClosed = actualClosedRelayNames1.Contains(relay);
                        if (anyOfTheseRelaysClosed)
                        {
                            closedRelayName = relay;
                            break;
                        }
                    }
                    if (!anyOfTheseRelaysClosed)
                    {
                        return DeviceResult.ResultOk("Размыкание реле прошло успешно");
                    }
                    return DeviceResult.ResultError($"ОШИБКА: реле {closedRelayName} замкнуто");
                case DeviceCommands.OpenAllRelays:
                    string[] breakableRelays1 = new string[] { "all" };
                    Commutator.OpenRelays(breakableRelays1);
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames2 = Commutator.GetClosedRelayNames();
                    bool anyOfTheseRelaysClosed2 = actualClosedRelayNames2[0] == "none";
                    if (anyOfTheseRelaysClosed2)
                    {
                        return DeviceResult.ResultOk("Размыкание реле прошло успешно");
                    }
                    return DeviceResult.ResultError("ОШИБКА: реле остались замкнутыми");
                case DeviceCommands.CheckClosedRelays:
                    string[] signalNames = deviceData.ExpectedValue.Replace(" ", "").Split(',');
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames3 = Commutator.GetClosedRelayNames();
                    bool isSignalExist = false;
                    foreach (var signal in signalNames)
                    {
                        isSignalExist = actualClosedRelayNames3.Contains(signal);
                        if (!isSignalExist)
                        {
                            break;
                        }
                    }
                    if (isSignalExist)
                    {
                        return DeviceResult.ResultOk($"Сигнал {signalNames[0]} присутствует");
                    }
                    return DeviceResult.ResultError($"Сигнал {signalNames[0]} отсутствует");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {deviceData.Command}");
            }
        }
    }
}
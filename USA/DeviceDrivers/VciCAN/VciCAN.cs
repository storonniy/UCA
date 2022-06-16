using System;
using System.Collections;
using System.Threading;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
/*using Ixxat.Vci3;
using Ixxat.Vci3.Bal;
using Ixxat.Vci3.Bal.Can;*/

namespace VciCAN
{

    //##########################################################################
    /// <summary>
    ///   This class provides the entry point for the IXXAT VCI .NET 2.0 API
    ///   demo application. 
    /// </summary>
    //##########################################################################
    public class CanConNet
    {
        public CanConNet(byte deviceNumber)
        {
            SelectDevice();
            InitSocket(deviceNumber);
        }

        #region Member variables

        /// <summary>
        ///   Reference to the used VCI device.
        /// </summary>
        IVciDevice mDevice;

        /// <summary>
        ///   Reference to the CAN controller.
        /// </summary>
        ICanControl mCanCtl;

        /// <summary>
        ///   Reference to the CAN message communication channel.
        /// </summary>
        ICanChannel mCanChn;

        /// <summary>
        ///   Reference to the message writer of the CAN message channel.
        /// </summary>
        ICanMessageWriter mWriter;

        /// <summary>
        ///   Reference to the message reader of the CAN message channel.
        /// </summary>
        ICanMessageReader mReader;


        /// <summary>
        ///   Quit flag for the receive thread.
        /// </summary>
        long mMustQuit = 0;

        /// <summary>
        ///   Event that's set if at least one message was received.
        /// </summary>
        AutoResetEvent mRxEvent;

        #endregion

        #region Device selection

        //************************************************************************
        /// <summary>
        ///   Selects the first CAN adapter.
        /// </summary>
        //************************************************************************
        public IVciDevice SelectDevice()
        {
            IVciDeviceManager deviceManager = null;
            //IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;
            IEnumerator deviceEnum = null;

            try
            {
                //
                // Get device manager from VCI server
                //
                deviceManager = VciServer.Instance().DeviceManager;
                //deviceManager = VciServer.GetDeviceManager();
                //
                // Get the list of installed VCI devices
                //
                deviceList = deviceManager.GetDeviceList();
                //
                // Get enumerator for the list of devices
                //
                deviceEnum = deviceList.GetEnumerator();

                //
                // Get first device
                //
                deviceEnum.MoveNext();
                mDevice = deviceEnum.Current as IVciDevice;
                // show the device name and serial number
                /*                object serialNumberGuid = mDevice.UniqueHardwareId;
                                string serialNumberText = GetSerialNumberText(ref serialNumberGuid);*/
            }
            catch (Exception)
            {

            }
            finally
            {
                //
                // Dispose device manager ; it's no longer needed.
                //
                DisposeVciObject(deviceManager);

                //
                // Dispose device list ; it's no longer needed.
                //
                DisposeVciObject(deviceList);

                //
                // Dispose device list ; it's no longer needed.
                //
                DisposeVciObject(deviceEnum);
            }
            return mDevice;
        }

        #endregion

        #region Opening socket

        //************************************************************************
        /// <summary>
        ///   Opens the specified socket, creates a message channel, initializes
        ///   and starts the CAN controller.
        /// </summary>
        /// <param name="canNo">
        ///   Number of the CAN controller to open.
        /// </param>
        /// <returns>
        ///   A value indicating if the socket initialization succeeded or failed.
        /// </returns>
        //************************************************************************
        public bool InitSocket(Byte canNo)
        {
            IBalObject bal = null;
            bool succeeded = false;

            try
            {

                //
                // Open bus access layer
                //
                bal = mDevice.OpenBusAccessLayer();

                //
                // Open a message channel for the CAN controller
                //
                mCanChn = bal.OpenSocket(canNo, typeof(ICanChannel)) as ICanChannel;

                // Initialize the message channel
                mCanChn.Initialize(1024, 128, false);

                // Get a message reader object
                mReader = mCanChn.GetMessageReader();

                // Initialize message reader
                mReader.Threshold = 1;

                // Create and assign the event that's set if at least one message
                // was received.
                mRxEvent = new AutoResetEvent(false);
                mReader.AssignEvent(mRxEvent);

                // Get a message wrtier object
                mWriter = mCanChn.GetMessageWriter();

                // Initialize message writer
                mWriter.Threshold = 1;

                // Activate the message channel
                mCanChn.Activate();



                //
                // Open the CAN controller
                //
                mCanCtl = bal.OpenSocket(canNo, typeof(ICanControl)) as ICanControl;

                // Initialize the CAN controller
                mCanCtl.InitLine(CanOperatingModes.Extended
                                , CanBitrate.Cia250KBit);

                // Set the acceptance filter
                mCanCtl.SetAccFilter(CanFilter.Ext,
                                     (uint)CanAccCode.All, (uint)CanAccMask.All);
                // Start the CAN controller
                mCanCtl.StartLine();

                succeeded = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Initializing socket failed");
            }
            finally
            {
                //
                // Dispose bus access layer
                //
                DisposeVciObject(bal);
            }

            return succeeded;
        }

        #endregion

        #region Message transmission

        /// <summary>
        ///   Transmits a CAN message with ID 0x100.
        /// </summary
        /// 
        /// 

        public void TransmitData(byte[] message, uint ID)
        {

            IMessageFactory factory = VciServer.Instance().MsgFactory;
            ICanMessage canMsg = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));

            canMsg.TimeStamp = 0;
            canMsg.Identifier = 0x100;
            canMsg.FrameType = CanMsgFrameType.Data;
            canMsg.DataLength = 8;
            canMsg.SelfReceptionRequest = true;  // show this message in the console window

            for (Byte i = 0; i < canMsg.DataLength; i++)
            {
                canMsg[i] = message[i];
            }
        }

        #endregion

        #region Message reception

        //************************************************************************
        /// <summary>
        ///   This method is the works as receive thread.
        /// </summary>
        //************************************************************************
        static bool __newMessage = false;

        /// <summary>
        /// CAN Message data structure
        /// </summary>
        /// <param name = "message"> CAN message </param>
        /// <param name = "ID"> ID of CAN message </param>
        /// <param name = "Length"> Message length </param>
        /// <param name = "Number"> Number of received messages </param>
        public struct DataBuf
        {
            public byte[] message;
            public uint ID;
            public int Length;
            public int Number;
        }

        public DataBuf __dataBuf;
        readonly object __locker = new object();

        public void ReceiveThreadFunc()
        {
            int msgNumber = 0;
            IMessageFactory factory = VciServer.Instance().MsgFactory;
            ICanMessage canMessage = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));
            do
            {
                // Wait 100 msec for a message reception
                if (mRxEvent.WaitOne(100, false))
                {
                    while (mReader.ReadMessage(out canMessage))
                    {
                        __newMessage = !canMessage.RemoteTransmissionRequest;
                        lock (__locker)
                        {
                            if (!canMessage.RemoteTransmissionRequest)
                            {
                                msgNumber++;
                                byte[] msg = new byte[canMessage.DataLength];
                                for (int index = 0; index < canMessage.DataLength; index++)
                                {
                                    msg[index] = canMessage[index];
                                }
                                DataBuf thisData = new DataBuf();
                                thisData.Length = canMessage.DataLength;
                                thisData.ID = canMessage.Identifier;
                                thisData.message = msg;
                                thisData.Number = msgNumber;
                                __dataBuf = thisData;
                                __newMessage = false;
                            }
                        }
                    }
                }
            } while (0 == mMustQuit);
        }

        public DataBuf GetData()
        {
            lock (__locker)
                return __dataBuf;
        }


        public bool ThereIsANewMessage()
        {
            lock (__locker)
                return __newMessage;
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Returns the UniqueHardwareID GUID number as string which
        /// shows the serial number.
        /// Note: This function will be obsolete in later version of the VCI.
        /// Until VCI Version 3.1.4.1784 there is a bug in the .NET API which
        /// returns always the GUID of the interface. In later versions there
        /// the serial number itself will be returned by the UniqueHardwareID property.
        /// </summary>
        /// <param name="serialNumberGuid">Data read from the VCI.</param>
        /// <returns>The GUID as string or if possible the  serial number as string.</returns>
        string GetSerialNumberText(ref object serialNumberGuid)
        {
            string resultText;

            // check if the object is really a GUID type
            if (serialNumberGuid.GetType() == typeof(System.Guid))
            {
                // convert the object type to a GUID
                System.Guid tempGuid = (System.Guid)serialNumberGuid;

                // copy the data into a byte array
                byte[] byteArray = tempGuid.ToByteArray();

                // serial numbers starts always with "HW"
                if (((char)byteArray[0] == 'H') && ((char)byteArray[1] == 'W'))
                {
                    // run a loop and add the byte data as char to the result string
                    resultText = "";
                    int i = 0;
                    while (true)
                    {
                        // the string stops with a zero
                        if (byteArray[i] != 0)
                            resultText += (char)byteArray[i];
                        else
                            break;
                        i++;

                        // stop also when all bytes are converted to the string
                        // but this should never happen
                        if (i == byteArray.Length)
                            break;
                    }
                }
                else
                {
                    // if the data did not start with "HW" convert only the GUID to a string
                    resultText = serialNumberGuid.ToString();
                }
            }
            else
            {
                // if the data is not a GUID convert it to a string
                string tempString = (string)(string)serialNumberGuid;
                resultText = "";
                for (int i = 0; i < tempString.Length; i++)
                {
                    if (tempString[i] != 0)
                        resultText += tempString[i];
                    else
                        break;
                }
            }

            return resultText;
        }


        //************************************************************************
        /// <summary>
        ///   Finalizes the application 
        /// </summary>
        //************************************************************************
        public void FinalizeApp()
        {
            //
            // Dispose all hold VCI objects.
            //

            // Dispose message reader
            DisposeVciObject(mReader);


            // Dispose message writer 
            DisposeVciObject(mWriter);

            // Dispose CAN channel
            DisposeVciObject(mCanChn);

            // Dispose CAN controller
            DisposeVciObject(mCanCtl);

            // Dispose VCI device
            DisposeVciObject(mDevice);
        }


        //************************************************************************
        /// <summary>
        ///   This method tries to dispose the specified object.
        /// </summary>
        /// <param name="obj">
        ///   Reference to the object to be disposed.
        /// </param>
        /// <remarks>
        ///   The VCI interfaces provide access to native driver resources. 
        ///   Because the .NET garbage collector is only designed to manage memory, 
        ///   but not native OS and driver resources the application itself is 
        ///   responsible to release these resources via calling 
        ///   IDisposable.Dispose() for the obects obtained from the VCI API 
        ///   when these are no longer needed. 
        ///   Otherwise native memory and resource leaks may occure.  
        /// </remarks>
        //************************************************************************
        void DisposeVciObject(object obj)
        {
            if (null != obj)
            {
                IDisposable dispose = obj as IDisposable;
                if (null != dispose)
                {
                    dispose.Dispose();
                    obj = null;
                }
            }
        }

        #endregion
    }
}


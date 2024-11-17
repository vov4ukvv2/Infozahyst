﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infozahyst
{
    public class UdpMessageProcessor
    {
        private readonly ControlItemManager controlItemManager;
        public UdpMessageProcessor(ControlItemManager controlItemManager)
        {
            this.controlItemManager = controlItemManager;
        }
        public void ProcessUdpMessage(byte[] packet)
        {
            if (IsNakMessage(packet))
            {
                Console.WriteLine("Received NAK: Control Item is not supported.");
                HandleNakResponse();
                return;
            }
            if (IsAckMessage(packet))
            {
                byte dataItem = packet[2];  // Assume that the third byte contains the Data Item
                Console.WriteLine($"Received ACK for Data Item {dataItem}");
                HandleAckResponse(dataItem);
                return;
            }
            Console.WriteLine("Received unrecognized message.");
        }
        private bool IsNakMessage(byte[] packet)
        {
            return packet.Length == 2 && packet[0] == 0x02 && packet[1] == 0x00;
        }
        private bool IsAckMessage(byte[] packet)
        {
            return packet.Length == 3 && packet[0] == 0x03 && packet[1] == 0x60;
        }
        private void HandleNakResponse()
        {
            controlItemManager.RemoveUnsupportedControlItems();
        }
        private void HandleAckResponse(byte dataItem)
        {
            controlItemManager.MarkControlItemAsAcked(dataItem);
        }
    }
}

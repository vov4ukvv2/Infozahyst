using System;
using System.Collections.Generic;

namespace Infozahyst
{
    public class ControlItemManager
    {
        private HashSet<byte> supportedControlItems;

        public ControlItemManager()
        {
            supportedControlItems = new HashSet<byte>();
        }

        public void AddSupportedControlItem(byte controlItem)
        {
            if (supportedControlItems.Add(controlItem))
            {
                Console.WriteLine($"Control Item {controlItem} added to supported list.");
            }
        }

        public void RemoveControlItem(byte controlItem)
        {
            if (supportedControlItems.Remove(controlItem))
            {
                Console.WriteLine($"Control Item {controlItem} removed from supported list.");
            }
        }

        public void RemoveUnsupportedControlItems()
        {
            supportedControlItems.Clear();
            Console.WriteLine("All unsupported control items have been removed.");
        }

        public void MarkControlItemAsAcked(byte dataItem)
        {
            if (supportedControlItems.Add(dataItem))
            {
                Console.WriteLine($"Data Item {dataItem} marked as ACK'd.");
            }
        }

        public HashSet<byte> GetSupportedControlItems()
        {
            return new HashSet<byte>(supportedControlItems);
        }
    }
}

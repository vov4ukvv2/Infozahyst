using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infozahyst
{
    public class ControlItemManager
    {
        private List<byte> supportedControlItems;

        public ControlItemManager()
        {
            supportedControlItems = new List<byte>();
        }
        public void AddSupportedControlItem(byte controlItem)
        {
            if (!supportedControlItems.Contains(controlItem))
            {
                supportedControlItems.Add(controlItem);
                Console.WriteLine($"Control Item {controlItem} added to supported list.");
            }
        }
        public void RemoveControlItem(byte controlItem)
        {
            if (supportedControlItems.Contains(controlItem))
            {
                supportedControlItems.Remove(controlItem);
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
            if (!supportedControlItems.Contains(dataItem))
            {
                supportedControlItems.Add(dataItem);
                Console.WriteLine($"Data Item {dataItem} marked as ACK'd.");
            }
        }

        public List<byte> GetSupportedControlItems()
        {
            return supportedControlItems;
        }
    }
}

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infozahyst
{
    [MemoryDiagnoser]
    public class BenchmarkProgram
    {
        private ControlItemManager controlItemManager;

        [GlobalSetup]
        public void Setup()
        {
            controlItemManager = new ControlItemManager();
        }

        [Benchmark]
        public void TestAddControlItem()
        {
            for (byte i = 0; i < 100; i++)
            {
                controlItemManager.AddSupportedControlItem(i);
            }
        }

        [Benchmark]
        public void TestRemoveControlItem()
        {
            for (byte i = 0; i < 100; i++)
            {
                controlItemManager.RemoveControlItem(i);
            }
        }

        [Benchmark]
        public void TestRemoveUnsupportedControlItems()
        {
            controlItemManager.RemoveUnsupportedControlItems();
        }

        [Benchmark]
        public void TestMarkControlItemAsAcked()
        {
            for (byte i = 0; i < 100; i++)
            {
                controlItemManager.MarkControlItemAsAcked(i);
            }
        }

        [Benchmark]
        public void TestGetSupportedControlItems()
        {
            var items = controlItemManager.GetSupportedControlItems();
        }
    }
}

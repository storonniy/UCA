using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using UCA.Steps;

namespace UPD.Device
{
    public abstract class Command
    {
        public abstract void Do(Step step);
    }
}

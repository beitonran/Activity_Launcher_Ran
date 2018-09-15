using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace LauncherMvvmLight.Domain.DataLayer.DB
{
    public enum Modules
    {
        California,
        [Description("New Mexico")]
        NewMexico,
        [Description("New York")]
        NewYork,
        [Description("South Carolina")]
        SouthCarolina,
        Tennessee,
        Washington
    }


//EXC4500_BRDTYPE_PCIE_VPX = 0xE450
//EXC_BRDTYPE_1394PCIE = 0xEF00



    public enum States
    {
        //[Description("Undefined")]
        //,
        [Description("2000PCI Series")]
        EXC2000_BRDTYPE_PCI = 0x4006,
        [Description("EXC-2000PCIe")]
        EXC2000_BRDTYPE_PCIE,
        [Description("EXC-4000PCI")]
        EXC4000_BRDTYPE_PCI = 0x4000,
        [Description("EXC-4000cPCI")]
        EXC4000_BRDTYPE_CPCI = 0x4001,
        [Description("EXC-4000PCI/HC")]
        EXC4000_BRDTYPE_PCIHC = 0x4008,
        [Description("EXC-4000PCIe")]
        EXC4000_BRDTYPE_PCIE = 0xE400,
        [Description("EXC-4000PCIe64")]
        EXC4000_BRDTYPE_PCIE64 = 0xE464,
        [Description("EXC-4000P104plus")]
        EXC4000_BRDTYPE_P104P = 0x4007,
        //[Description("1553PCI/MCH")]
        //,
        [Description("EXC-1553cPCI/MCH")]
        EXC4000_BRDTYPE_MCH_CPCI = 0x4003,
        [Description("EXC-1553PMC/MCH")]
        EXC4000_BRDTYPE_MCH_PMC = 0x4004,
        [Description("EXC-1553ExCARD/Px")]
        EXCARD_BRDTYPE_1553PX,
        //[Description("EXC-1553mPCIe/Px")]
        //,
        [Description("EXC-1553PMC/Px")]
        EXC4000_BRDTYPE_1553PX_PMC = 0x400A,
        [Description("1533PCI/MCH")]
        EXC4000_BRDTYPE_MCH_PCI = 0x4002,
        //[Description("1553PCMCIA")]
        //,
        //[Description("1553PCMCIA/EP")]
        //,
        //[Description("1553PCMCIA/PX")]
        //,
        //[Description("1553PCMCIA/Px or /EP")]
        //,
        [Description("DAS-429PMC/RTx")]
        EXC4000_BRDTYPE_429_PMC = 0x4005,
        [Description("DAS-429ExCARD/RTx")]
        EXCARD_BRDTYPE_429RTX = 0xE402,
        //[Description("DAS-429mPCIe/RTx")]
        //,
        //[Description("429PCMCIA")]
        //,
        [Description("EXC-708ccPMC")]
        EXC4000_BRDTYPE_708_PMC = 0x4009,
        [Description("EXC-DiscretePCI/Dx")]
        EXC4000_BRDTYPE_DISCR_PCI = 0x400D,
        //[Description("EXC-4500ccVPX")]
        //,
        //[Description("EXC-1394PCI")]
        //,
        [Description("EXC-1394PCIe")]
        EXC_BRDTYPE_1394PCIE = 0xEF00,
        //[Description("3910PCI")]
        //,
        //[Description("EXC_BOARD_3910PCI")]
        //,
        [Description("1394PCI Series")]
        EXC_BRDTYPE_1394PCI = 0x1394,
        [Description("ES-1553RUNET/Px")]
        EXC_BRDTYPE_RNET = 0x5505,
        //[Description("DAS-429UNET/RTx")]
        //,
        //[Description("ES-1553UNET/Px")]
        //,
        //[Description("EthernetPCIe")]
        //,
        [Description("EXC-1553UNET/Px")]
        EXC_BRDTYPE_UNET = 0x5502,
        //[Description("EXC-9800MACC-II")]
        //,
        [Description("EXC-664PCIe")]
        EXC_BRDTYPE_664PCIE = 0xE664,
        //[Description("664PCI")]
        //,
        //[Description("miniPCIe Card")]
        //,
        //[Description("Express Card")]
        //
    }
}
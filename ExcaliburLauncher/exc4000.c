#include <stdio.h>
#include "exc4000.h"
#include "deviceio.h"
#include "excsysio.h"
#include "error_devio.h"

#define GETBIT(x,y) (((x) & (1<<(y))) != 0)

WORD g_preloadval;

static INSTANCE_EXC4000 exc4000card[NUMBOARDS];
static int m_firsttimeTimers = 1;

char errStr[50];
char * Print_Error_4000 (int errorcode);
/* int TinyDelay_4000(void); */

/*
Description: Get_4000Board_Type extracts the board "device instance id" as listed in the hardware signature for device device_num.

Input parameter: device_num
Output parameter: boardtype
Return values: 0 - success
		other kernel level errors
- - - - - - - - - -
For example, a 4000PCI board has the following signature:
PCI\VEN_1405&DEV_4000&SUBSYS_00000000&REV_01\4&31B6CD7&0&10F0

Extract the 4-digit number following the string "DEV_".
Convert to HEX, and return it as boardtype.
*/

int __declspec(dllexport) Get_4000Board_Type(WORD device_num, WORD *boardtype)
{
	int iError, retVal;
	WORD wBoardType;
	unsigned long dwCardType;
	t_globalRegs4000 *globreg;

	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;
	

	if (IsWin2000())
	{
		
		retVal =  GetCardType(device_num, &dwCardType);
		if (retVal < 0) 
		{
			CloseKernelDevice(device_num);
			return retVal;
		}

		
		//EXC-4000[c]PCI[e] Family
		if ((dwCardType == 	EXC_BRDTYPE_4000PCI)	||
			(dwCardType == 	EXC_BRDTYPE_AFDX)		||
			(dwCardType == 	EXC_BRDTYPE_1394))
		{

			retVal = GetBoardType(device_num, boardtype);

		}
		else //PCMCIA and Old Cards
		{
			*boardtype = (WORD)dwCardType;
		}

	}
	else
	{
		
		iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
		if (iError)
		{
			CloseKernelDevice(device_num);
			return iError; /* error from mapmemory */
		}
		
		/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
		wBoardType = (WORD)(globreg->boardSigId & EXC4000PCI_BOARDSIGMASK);
		if ((wBoardType != EXC4000PCI_BOARDSIGVALUE) && (wBoardType != EXC4000PCIE_BOARDSIGVALUE) && (wBoardType != EXCAFDXPCI_BOARDSIGVALUE)) 
		{
			CloseKernelDevice(device_num);
			return ekernelnot4000card;
		}
		
		*boardtype = wBoardType;
		
		retVal = 0;
	}

	CloseKernelDevice(device_num);

	return retVal;


}

int __declspec(dllexport) Get_4000Module_Type(WORD device_num, WORD module_num, WORD *modtype)
{
	int iError;
	t_globalRegs4000 *globreg;

	if (module_num >= MAX_MODULES_ON_BOARD)
		return emodnum;

	
	
	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;
	
	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError)
	{
		CloseKernelDevice(device_num);
		return iError; /* error from mapmemory */
	}
	
	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	
	if (module_num < 4) {
		*modtype = (WORD)(globreg->moduleInfo[module_num] & EXC4000PCI_MODULETYPEMASK);
	}
	else {
		*modtype = (WORD)(globreg->moduleInfoSecondGroup[module_num-4] & EXC4000PCI_MODULETYPEMASK);
	}

	/* Ethernet and AFDX have the same values in the global module type, so we have to distinguish between them by looking in the module: */
	if ((*modtype == EXC4000_MODTYPE_AFDX_TX) || (*modtype == EXC4000_MODTYPE_AFDX_RX))
	{
		unsigned short *bank0;
		iError = MapMemory(device_num, (void **)&(bank0), 0);
		if (iError)
		{
			CloseKernelDevice(device_num);
			return iError; /* error from mapmemory */
		}
		if (bank0[0x8E / sizeof(unsigned short)] == 0x8023)
			*modtype = EXC4000_MODTYPE_ETHERNET;
	}
	
	CloseKernelDevice(device_num);
	return 0;
}

int __declspec(dllexport) Get_UniqueID_P4000(WORD device_num, WORD *pwUniqueID)
{
	int iError;
	t_globalRegs4000 *globreg;

	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;
	
	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError) 
	{
		CloseKernelDevice(device_num);
		return iError; 
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	

	*pwUniqueID = (WORD)(globreg->boardSigId & EXC4000PCI_BOARDIDMASK);

	CloseKernelDevice(device_num);
	return 0;
}

int __declspec(dllexport) Get_4000Interface_Rev(WORD device_num,WORD *interface_rev)
{
	int iError;
	t_globalRegs4000 *globreg;

	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	*interface_rev = globreg->fpgaRevision;

	CloseKernelDevice(device_num);

	return 0;


}
/*
Select_Time_Tag_Source	

Description: Selects the source of the time tag used on all modules
INTERNAL CLOCK uses the on board 4 usec. timer. See EXTTCLKx pin in
connectors in the  User’s Manual.

Input parameters:  source  EXC4000_INTERNAL_CLOCK or EXC4000_EXTERNAL_CLOCK

Output parameters: none

Return values:		einval				If parameter is out of range
					ekernelnot4000card	if not a 4000 card
					kernel errors		(error returns from OpenKernelDevice
										and MapMemory)
					0					If successful

*/
int __declspec(dllexport) Select_Time_Tag_Source_4000 (WORD device_num, WORD source)
{
	int iError;
	t_globalRegs4000 *globreg;

	
	if ((source != EXC4000_INTERNAL_CLOCK) && (source != EXC4000_EXTERNAL_CLOCK))
		return einvalttagsource;

	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError)
	{
		CloseKernelDevice(device_num);
		return iError;
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	/* ExCARD does not support this function */
	if ((globreg->boardType == EXCARD_BRDTYPE_1553PX) || (globreg->boardType == EXCARD_BRDTYPE_429RTX))
	{
		CloseKernelDevice(device_num);
		return enotforexcard;
	}

	(WORD)globreg->clocksourceSelect = source;

	CloseKernelDevice(device_num);
	return(0);
}
/*
Get_Time_Tag_Source

Description: Returns the source of the time tag used on all modules

Output parameters:  source  EXC4000_INTERNAL_CLOCK or EXC4000_EXTERNAL_CLOCK

Return values:		
					ekernelnot4000card if not a 4000 card
					kernel errors (error returns from OpenKernelDevice
					and MapMemory)
					0	   If successful
*/
int __declspec(dllexport) Get_Time_Tag_Source_4000 (WORD device_num, WORD *source)
{
	int iError;
	t_globalRegs4000 *globreg;

	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError)
	{
		CloseKernelDevice(device_num);
		return iError;
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	/* ExCARD does not support this function */
	if ((globreg->boardType == EXCARD_BRDTYPE_1553PX) || (globreg->boardType == EXCARD_BRDTYPE_429RTX))
	{
		CloseKernelDevice(device_num);
		return enotforexcard;
	}


	*source = (WORD)globreg->clocksourceSelect;
	if (*source == 0xFFFF) /* if rev b or below, has to be internal */
		*source = EXC4000_INTERNAL_CLOCK;

	CloseKernelDevice(device_num);
	return(0);
}

int __declspec(dllexport) Reset_Module_4000(WORD device_num, WORD module_num)
{
	int iError;
	t_globalRegs4000 *globreg;

	if (module_num >= MAX_MODULES_ON_BOARD)
		return emodnum;

	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError) 
	{
		CloseKernelDevice(device_num);
		return iError; 
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}

	/* 16nov10: Added fix to support newer boards with more than 4 modules; 
	eg, AFDX has 6 modules, EXC-4000P104plus board has 5 modules */
	if (module_num > 3) module_num += 1;
	globreg->softwareReset = 1<<module_num;

	CloseKernelDevice(device_num);
	return 0;
}

int __declspec(dllexport) Reset_Timetags_On_All_Modules_4000(WORD device_num)
{
	int iError;
	t_globalRegs4000 *globreg;

	
	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(globreg), GLOBALREG_BANK);
	if (iError) 
	{
		CloseKernelDevice(device_num);
		return iError; 
	}

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Verify4000SeriesBoard(globreg);
	if (iError < 0) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	globreg->softwareReset = EXC4000PCI_GLOBAL_TTAG_RESET;

	CloseKernelDevice(device_num);
	return 0;
}

int __declspec(dllexport) Init_Timers_4000 (WORD device_num, int *handle)
{
	int iError, i;
	WORD wBoardType, wBoardRev;
	

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	iError = Get_4000Board_Type(device_num,&wBoardType);
	if (iError < 0) return iError;

	if (m_firsttimeTimers)
	{
		for (i=0; i<NUMBOARDS; i++)
			exc4000card[i].allocCounter = 0;
		m_firsttimeTimers = 0;
	}
	else
	{	
		for (i=0; i<NUMBOARDS; i++)
			if ((exc4000card[i].device_num == device_num) && (exc4000card[i].allocCounter > 0))
			{
				*handle = i;
				exc4000card[i].allocCounter++;
				return 0;
			}	
	}
	for (i=0; i<NUMBOARDS; i++)
		if (exc4000card[i].allocCounter == 0)
			break;

	if (i == NUMBOARDS)
		return edevtoomany; 

	iError = OpenKernelDevice(device_num);
	if (iError)
		return iError;

	iError = MapMemory(device_num, (void **)&(exc4000card[i].globreg), GLOBALREG_BANK);
	if (iError) 
	{
		CloseKernelDevice(device_num);
		return iError;
	}
	
	/* verify that card revision supports timers and irig */
	wBoardRev = exc4000card[i].globreg->fpgaRevision;
	if (((wBoardRev == 0xFF) || (wBoardRev < 0x22)) && (!IsPciExpress_4000(wBoardType)))
	{
		CloseKernelDevice(device_num);
		return enotimersirig;
	}

	exc4000card[i].device_num = device_num;
	exc4000card[i].allocCounter++;
	*handle = i;	
	return 0;
}

int __declspec(dllexport) Release_Timers_4000 (int handle)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;
	
	exc4000card[handle].allocCounter--;
	if (exc4000card[handle].allocCounter == 0)
		CloseKernelDevice(exc4000card[handle].device_num);

	return 0;
}


/* 
Setting up IRIG B Mode
To set up IRIG B mode:
1.  Call Init_Timers_4000
2.	Call SetIrig_4000, to set up the IRIG B feature.
3.	Wait, in a loop, until availFlag is TRUE – see  IsIrigTimeavail_4000 on page 2-7.
4.	IRIG B time is then available either in seconds (see  GetIrigSeconds_4000 on page 2-6) or in a string (see  GetIrigTime_4000 on page 2-5).
*/

/*
SetIrig_4000

	SetIrig_4000 sets the board to receive IRIG B time.
	SetIrig_4000 (WORD device_num, WORD flag)	
Input parameters: 
		handle -- output of Init_Timers_4000	
		flag	IRIG_TIME_AND_RESET
				Sets the carrier board to receive IRIG B time. In addition for synchronization purposes, Time Tags on all modules on the carrier board will be reset to 0 when the IRIG B time comes in.
				IRIG_TIME
				Sets the carrier board to get IRIG B time but does not do a Time Tag reset.

Output parameters: none

Return values:
	eopenkernel	Cannot open kernel device; check Excalibur Configuration Utility settings 
	ekernelcantmap	Kernel driver cannot map memory 
	0	If successful
*/
int __declspec(dllexport) SetIrig_4000(int handle, WORD flag)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	/* ExCARD does not support IrigB */
	if ((exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_1553PX) || 
		(exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_429RTX))
		return enotforexcard;

	flag &= (IRIG_TIME_AND_RESET | IRIG_TIME); /* only allow these two settings */
	exc4000card[handle].globreg->IRcommand = flag;
	
	return 0;
}
/*
IsIrigTimeavail_4000

	IsIrigTimeavail_4000 indicates if IRIG B time is available. Must be called after SetIrig_4000.	

Input parameters:  
	handle -- output of Init_Timers_4000	

Output parameters:
	availFlag	1	IRIG B Time came in
				0	IRIG B Time not yet available

Return values:		
					ebaddevhandle	If handle is not a valid handle	number
					0				If successful
*/

int __declspec(dllexport) IsIrigTimeavail_4000(int handle, int *availFlag)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	/* ExCARD does not support IrigB */
	if ((exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_1553PX) || 
		(exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_429RTX))
		return enotforexcard;

	*availFlag = ((exc4000card[handle].globreg->IRcommand & IRIG_TIME_AVAIL) == IRIG_TIME_AVAIL);

	return 0;
}

/*
GetIrigSeconds_4000

Once IRIG B time is available, GetIrigSeconds_4000 returns the IRIG B time in seconds.

Input parameters:  
	handle -- output of Init_Timers_4000	

Output parameters:
	seconds	IRIG B time in seconds

Return values:		
	ebaddevhandle	If handle is not a valid handle
	0	If successful
*/

int __declspec(dllexport) GetIrigSeconds_4000(int handle, unsigned long *seconds)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	/* ExCARD does not support IrigB */
	if ((exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_1553PX) || 
		(exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_429RTX))
		return enotforexcard;

	*seconds = ((long) (exc4000card[handle].globreg->IRcommand & 1) << 16) + exc4000card[handle].globreg->IRsecondsOfDay + 1;

	return 0;
}
/*
GetIrigTime_4000

	GetIrigTime_4000 returns the IRIG B time as a string in the form of days, hours, minutes and seconds	
	GetIrigTime_4000 (WORD device_num, t_IrigTime *IrigTime)	
	device_num	
The define value EXC_4000PCI can be used instead of a device number. If more than one board is used, run ExcConfig.exe to set the device number

IrigTime	Days [1-366, number of days since January 1]: hours: minutes: seconds
return values:
eopenkernel	Cannot open kernel device; check Excalibur Configuration Utility settings
ekernelcantmap	Kernel driver cannot map memory
	0	If successful
*/
int __declspec(dllexport) GetIrigTime_4000(int handle, t_IrigTime *IrigTime)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;
	/* ExCARD does not support IrigB */
	if ((exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_1553PX) || 
		(exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_429RTX))
		return enotforexcard;


	IrigTime->days = (WORD)(((exc4000card[handle].globreg->IRdaysHours & 0xc000) >> 14) * 100);	/* first digit */
	IrigTime->days += (WORD)(((exc4000card[handle].globreg->IRdaysHours & 0x3c00) >> 10) * 10);	/* second digit */
	IrigTime->days += (WORD)((exc4000card[handle].globreg->IRdaysHours & 0x03c0) >> 6);		/* lowest digit */
	IrigTime->hours = (WORD)((((exc4000card[handle].globreg->IRdaysHours & 0x0030) >> 4) * 10) +
		(exc4000card[handle].globreg->IRdaysHours & 0x000f));
	IrigTime->minutes = (WORD)((((exc4000card[handle].globreg->IRminsSecs & 0x7000) >> 12) * 10) +
		((exc4000card[handle].globreg->IRminsSecs & 0x0f00) >> 8));

	IrigTime->seconds = (WORD)((((exc4000card[handle].globreg->IRminsSecs & 0x0070) >> 4) * 10) +
   	(exc4000card[handle].globreg->IRminsSecs & 0x000f) + 1);
	if (IrigTime->seconds == 60)
	{
		IrigTime->seconds = 0;
		IrigTime->minutes++;
		if (IrigTime->minutes == 60)
		{
			IrigTime->minutes = 0;
			IrigTime->hours++;
			if (IrigTime->hours == 24)
			{
				IrigTime->hours = 0;
				IrigTime->days++;
				if ((IrigTime->days == 366) || (IrigTime->days == 367))
					IrigTime->days = 0;
			}
		}
	}

	
	return 0;
}

int __declspec(dllexport) GetIrigControl_4000(int handle, unsigned long *control)
{
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;
	/* ExCARD does not support IrigB */
	if ((exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_1553PX) || 
		(exc4000card[handle].globreg->boardType == EXCARD_BRDTYPE_429RTX))
		return enotforexcard;


	*control = ((long)(exc4000card[handle].globreg->IRcontrol1 & 0x7ff) << 16) | exc4000card[handle].globreg->IRcontrol2;

	return 0;
}
/*
Section 1: Software Function Descriptions

StartTimer
· Description: Start the Timer on the EXC-4000 board.
· Input:  handle -- output of Init_Timers_4000	
· Ouput: (signed value) offset: difference between actual time being counted and time to count  requested (+ means the actual is greater, - means the actual is less than what was requested; 0 if same).
· Return: 0 if OK; illegal parameter value error codes, error if Timer is currently on.
*/
int __declspec(dllexport) StartTimer_4000(int handle, unsigned long microsecsToCount, int reload_flag, int interrupt_flag, int globreset_flag, int *timeoffset)
{
	WORD microsecPerTick, numberTicks, controlval;

	if (IsWinNT4())
		return einvalidOS;

	*timeoffset = 0;
	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;
	if ((reload_flag != TIMER_RELOAD) && (reload_flag != TIMER_NO_RELOAD))
		return eparmreload;
	if ((interrupt_flag != TIMER_INTERRUPT) && (interrupt_flag != TIMER_NO_INTERRUPT))
		return eparminterrupt;
	if ((globreset_flag != TIMER_GLOBRESET) && (globreset_flag != TIMER_NO_GLOBRESET))
		return eparmglobalreset;

   if (GETBIT(exc4000card[handle].globreg->TMcontrol, TM_STARTBIT))
   {
		return etimerrunning;
   }
	if (microsecsToCount == 0)
		return 0;

	/* exc4000card[handle].globreg->TMcontrol &= ~TM_STARTBITVAL;  /* Stop the Timer */

	/* The resolution, in microsecPerTick, goes in the prescale register;
	 numberTicks is loaded into the preload register*/

	microsecPerTick = (WORD)(((microsecsToCount-1) / 0xFFFF) + 1);
	numberTicks = (WORD)(microsecsToCount/microsecPerTick);
	*timeoffset = microsecsToCount - (microsecPerTick * numberTicks);

	exc4000card[handle].globreg->TMprescale = microsecPerTick;
	/* TinyDelay_4000(); */

	exc4000card[handle].globreg->TMpreload = numberTicks;


	controlval = (WORD)(TM_STARTBITVAL | (reload_flag << TM_RELOADBIT) | (interrupt_flag << TM_INTERRUPTBIT) | (globreset_flag << TM_GLOBRESETBIT));
	exc4000card[handle].globreg->TMcontrol = controlval;   /* Start the Timer with requested functionality*/

	return 0;
}

/*
int TinyDelay_4000(void)
{
	return 0;
}
*/

/*
StopTimer
· Description: Stop the Timer immediately
· Input:  handle -- output of Init_Timers_4000	
· Output: timer_value (last value of Timer, in usecs)
·  Return: 0 if OK; ebaddevhandle if invalid handle passed, einvalidOS
if running on NT4
*/
int __declspec(dllexport) StopTimer_4000(int handle, unsigned long *timervalue)
{
	if (IsWinNT4())
		return einvalidOS;

	*timervalue = 0;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	exc4000card[handle].globreg->TMcontrol &= ~TM_STARTBITVAL;  /* Stop the Timer */
	*timervalue = (long)(exc4000card[handle].globreg->TMcounter * exc4000card[handle].globreg->TMprescale);

	return 0;
}
/*
ReadTimerValue
· Description: Returns how many usecs are left until Timer completes.
· Input: device_num (value in ExcConfig, or default for one board)
· Output: timer_value (current value of Timer, in usecs)
· Return: 0 if OK; kernel errors if problem accessing board
*/
int __declspec(dllexport) ReadTimerValue_4000(int handle, unsigned long *timervalue)
{
	if (IsWinNT4())
		return einvalidOS;

	*timervalue = 0;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	*timervalue = (unsigned long)((unsigned long)exc4000card[handle].globreg->TMcounter * (unsigned long)exc4000card[handle].globreg->TMprescale);

	return 0;
}
/*
IsTimerRunning
· Description: Boolean. Returns 1 if Timer is running, 0 otherwise. This can be easily used in a loop, to wait for Timer to end. (Recommended for use in non- automatic-restart mode.) This is also useful for multiple modules on the board, to check the availability of the Timer.
· Input: device_num (value in ExcConfig, or default for one board)
· Output: error value, if there was one (kernel error if problem accessing board). 0 otherwise.
· Return: 1 if Timer is running (Timer value is >0); 0 if not (Timer value = 0) or error condition. (If value is 0, programmer can check the output value to see  if there is an error condition)
*/
BOOL __declspec(dllexport) IsTimerRunning_4000(int handle, int *errorcondition)
{
	BOOL retval;

	if (IsWinNT4())
   {
    	*errorcondition = einvalidOS;
		return FALSE;
   }

	*errorcondition = 0;
	if ((handle <0) || (handle >= NUMBOARDS))
		*errorcondition = ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		*errorcondition =  ebaddevhandle;

	if (GETBIT(exc4000card[handle].globreg->TMcontrol, TM_STARTBIT))
		retval = TRUE;
	else
		retval = FALSE;

	return retval;
}
/*
ResetWatchdog
· Description: Stops and re-starts Timer with the same value as the last time StartTimer was called.  (read and write back preload value)
· Input: device_num (value in ExcConfig, or default for one board)
· Return: 0 if OK; kernel error if problem accessing board; error if Timer not running.
*/
int __declspec(dllexport) ResetWatchdogTimer_4000(int handle)
{
	if (IsWinNT4())
		return einvalidOS;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;
	if (!GETBIT(exc4000card[handle].globreg->TMcontrol, TM_STARTBIT))
	{
		return etimernotrunning;
	}

	exc4000card[handle].globreg->TMcontrol &= ~TM_STARTBITVAL;  /* Stop the Timer */
	g_preloadval = exc4000card[handle].globreg->TMpreload; /*make it global variable to avoid optimization*/
	exc4000card[handle].globreg->TMpreload = g_preloadval;
	exc4000card[handle].globreg->TMcontrol |= TM_STARTBITVAL;   /* Restart the Timer */

	return 0;
}

int __declspec(dllexport) InitializeInterrupt_P4000(int handle)
{
	int retval, device_num;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	device_num = exc4000card[handle].device_num;
	retval = Exc_Initialize_Interrupt_For_Module(device_num, EXC4000_MODULE_TIMER);

	return retval;
}

int __declspec(dllexport) Wait_for_Interrupt_P4000(int handle, unsigned int timeout)
{
	int retval, device_num;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	device_num = exc4000card[handle].device_num;

	retval = Exc_Wait_For_Module_Interrupt(device_num, EXC4000_MODULE_TIMER, timeout);

	return retval;
}

int __declspec(dllexport) Get_Interrupt_Count_P4000(int handle, unsigned long *Sys_Interrupts_Ptr)
{
	int retval, device_num;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	device_num = exc4000card[handle].device_num;
	retval = Exc_Get_Channel_Interrupt_Count(device_num, EXC4000_MODULE_TIMER, Sys_Interrupts_Ptr);

	return retval;
}

/*
Description: IsDMASupported_4000 returns a boolean value telling if the base board supports DMA

Input value: device_num
Return values: Win9x - FALSE
		Win2000 family - TRUE for Express Cards, FALSE for PCI cards
*/

int __declspec(dllexport) IsDMASupported_4000(WORD device_num)
{
	int iError;
	WORD wBoardType;

	/* If the OS is Win9x, there's no DMA support. */
	if (IsWin9X() == TRUE) {
		// we don't support dma under 9x
		return FALSE;
	}

	iError = Get_4000Board_Type(device_num, &wBoardType);
	if (iError)
		return FALSE;

	/* If the card is a 4000PCI express  card, there is DMA support */
	if (
		(wBoardType == EXC4000_BRDTYPE_PCIE)	||	/* board is a 4000 express card! */
		(wBoardType == EXC2000_BRDTYPE_PCIE)	||	/* board is a 2000 express card */
		(wBoardType == EXC_BRDTYPE_1394PCIE)	||	/* board is a firewire express card */
		(wBoardType == EXC_BRDTYPE_664PCIE)		|| 	/* board is an AFDX card */
		(wBoardType == EXC4500_BRDTYPE_PCIE_VPX) ||	/* board is a EXC-4500VPX card */
		(wBoardType == EXCARD_BRDTYPE_1553PX)	||	/* board is a ExcCard/Px card */
		(wBoardType == EXCARD_BRDTYPE_429RTX)		/* board is a ExcCard/RTx card */
	)
		return TRUE;
	else
		return FALSE;
}

int __declspec(dllexport) IsRepetitiveDMASupported_4000(WORD device_num)
{
	int iError;
	WORD rev4000, wBoardType;

	/* If the OS is Win9x, there's no DMA support. */
	if (IsWin9X() == TRUE) {
		// we don't support dma under 9x
		return FALSE;
	}

	// Even though some boards do DMA, then make sure it is not an early revision
	// that does not support Repetitive (aka Limited) DMA.

	iError = Get_4000Board_Type(device_num, &wBoardType);
	if (iError)
		return FALSE;

	iError = Get_4000Interface_Rev((WORD) device_num, &rev4000);
	if (iError)
		return FALSE;

	// .. 4000PCIe does not do RepDMA before rev 1.5
	if ((wBoardType == EXC4000_BRDTYPE_PCIE) && (rev4000 >= 0x15))
		return TRUE;

	// .. 2000PCIe does not do RepDMA before rev 1.1
	if ((wBoardType == EXC2000_BRDTYPE_PCIE) && (rev4000 >= 0x11))
		return TRUE;

	// .. 4500ccVPX does not do RepDMA before 1.6
	if ((wBoardType == EXC4500_BRDTYPE_PCIE_VPX) && (rev4000 >= 0x16))
		return TRUE;

	// .. AFDX always did RepDMA
	if (wBoardType == EXC_BRDTYPE_664PCIE)
		return TRUE;

	return FALSE;
}

int __declspec(dllexport) IsExpressCard_4000(WORD wBoardType)
{
	/* This function has been deprecated, and was replaced with the function IsPciExpress_4000.
		The original name caused too much confusion between PCI Express and "ExpressCard". */
	return IsPciExpress_4000(wBoardType);
}

int __declspec(dllexport) IsPciExpress_4000(WORD wBoardType)
{
	if (
		(wBoardType == EXC4000_BRDTYPE_PCIE)	||		/* board is a 4000 express card! */
		(wBoardType == EXC2000_BRDTYPE_PCIE)	||		/* board is a 2000 express card */
		(wBoardType == EXC_BRDTYPE_1394PCIE)	||		/* board is a firewire express card */
		(wBoardType == EXC_BRDTYPE_664PCIE)		|| 	/* board is an AFDX card */
		(wBoardType == EXC4500_BRDTYPE_PCIE_VPX) ||	/* board is a EXC-4500VPX card */
		(wBoardType == EXCARD_BRDTYPE_1553PX)	||	/* board is a ExcCard/Px card */
		(wBoardType == EXCARD_BRDTYPE_429RTX)		/* board is a ExcCard/RTx card */
	)

		return TRUE;
	else
		return FALSE;
}


int __declspec(dllexport) Verify4000SeriesBoard(t_globalRegs4000 *globreg)
{
	WORD wBoardType;

	if (globreg == NULL) 
		return ekernelcantmap;

	/* verify that this is indeed a 4000PCI or 4000PCIe or AFDX */
	wBoardType = (WORD)(globreg->boardSigId & EXC4000PCI_BOARDSIGMASK);
	if ((wBoardType != EXC4000PCI_BOARDSIGVALUE) && (wBoardType != EXC4000PCIE_BOARDSIGVALUE) && (wBoardType != EXCAFDXPCI_BOARDSIGVALUE)) 
		return ekernelnot4000card;
	
	return 0;
		
}

/*
Get_Error_String_4000
               
Description: Accepts the error returns from other functions.
This routine returns the string containing a corresponding
error message.

Input parameters:  errorcode   The error code (a negative number)
		   len         Maximum size of errorstring to return
Output parameters: errorstring	An array of 'len' characters, the message 
				string that contains the corresponding error message. 
				In case of bad input this routine contains a string denoting
                               	that.

Return values:  0		Always

Use this routine as per the following example:
   int len=255;
   char ErrorStr[255];

   Get_Error_String_4000(errorcode, len, ErrorStr);
   printf("error is: %s", ErrorStr);
              
*/

int __declspec(dllexport) Get_Error_String_4000(int errcode,int errlen, char *errstring)
{
	char *localerr;
  	int i;

	localerr = Print_Error_4000(errcode);
  	/* do this so that there will be null termination */
	for (i=0; i<errlen; i++)
		errstring[i] = '\0';
	strncpy(errstring, localerr, errlen-1);
	return 0;
}



/*
Local - used by Get_Error_String
Input parameters:  errorcode   The error code
Output parameters: none
Return values:  char pointer  To a message string that contains a
                              corresponding error message. In case of bad
                              input this routine returns a string denoting
                              that.
*/

char * Print_Error_4000 (int errorcode)
{
	char *perrStr = errStr;

	switch (errorcode)
	{
	case emodnum: return ("Invalid module number specified [emodnum]\n");
	case ekernelnot4000card: return ("Error returned by kernel: designated board is not EXC-4000 Series board [ekernelnot4000card]\n");
	case enotimersirig: return ("Timers and IrigB are not supported on this version of EXC-4000 board [enotimersirig]\n");
	case eclocksource: return ("Invalid clock source specified [eclocksource]\n");
	case eparmglobalreset: return ("Illegal parameter used for globalreset_flag in the StartTimer_4000 function [eparmglobalreset]\n");	
	case etimernotrunning: return ("Timer not running when function was called; did nothing [etimernotrunning]\n");	
	case etimerrunning: return ("Timer already running; did nothing [etimerrunning]\n");	
	case eparmreload: return ("Illegal parameter used for reload_flag in StartTimer_4000 [eparmreload]\n");	
	case eparminterrupt: return ("Illegal parameter used for interrupt_flag in StartTimer_4000 [eparminterrupt]\n");	
	case ebaddevhandle: return ("Invalid handle specified; use value returned by Init_Timers_4000 [ebaddevhandle]\n");	
	case edevtoomany: return (" Init_Timers_4000 called for too many boards [edevtoomany]\n");	
	case einvalidOS: return ("Invalid operating system [einvalidOS]\n");	
		case einvalttagsource: return ("Invalid time tag source specified [einvalttagsource]\n");
	case enotforexcard: return ("ExCARD does not support this function [enotforexcard]\n");

		/* for 4000vme */
	case eviclosedev: return ("Error in ViClose device[eviclosedev]\n");
	case evicloserm: return ("Error in ViClose DefaultRM [evicloserm]\n");
	case eopendefaultrm: return ("Error in viOpenDefaultRM [eopendefaultrm]\n");
	case eviopen: return ("Error in viOpen [eviopen]\n");
	case evimapaddress: return ("Error in viMapAddress [evimapaddress]\n");
	case evicommand: return ("Error in VISA command [evicommand]\n");
	case einstallhandler: return ("Error in VISA install int handler [einstallhandler]\n");
	case eenableevent: return ("Error in VISA enable event [eenableevent]\n");
	case euninstallhandler: return ("Error VISA uninstall int handler [euninstallhandler]\n");
	case edevnum: return ("Illegal device number (must be 0-255) [edevnum]\n");
	case einstr: return ("Bad ViSession value (instr)[einstr]\n");

		/* for 4000pci */
	case eopenkernel: return ("Error opening kernel device; check ExcConfig settings [eopenkernel]\n");
	case ekernelcantmap: return ("Error mapping memory [ekernelcantmap]\n");
	case ereleventhandle: return ("Error releasing the event handle [ereleventhandle]\n");
	case egetintcount: return ("Error getting interrupt count  [egetintcount]\n");
	case egetchintcount: return ("Error getting channel interrupt count  [egetchintcount]\n");
	case egetintchannels: return ("Error getting interrupt channels [egetintchannels]\n");
	case ewriteiobyte: return ("Error writing I/O memory [ewriteiobyte]\n");
	case ereadiobyte: return("Error reading I/O memory [ereadiobyte]\n");
	case egeteventhand1: return("Error getting event handle (in first stage) [egeteventhand1]\n");
	case egeteventhand2: return("Error getting event handle (in second stage) [egeteventhand2]\n");
	case eopenscmant: return("Error opening Service Control Manager (in startkerneldriver) [eopenscmant]\n");
	case eopenservicet: return("Error opening Service Control Manager (in stopkerneldriver)  [eopenservicet]\n");
	case estartservice: return("Error starting kernel service (in startkerneldriver)  [estartservice]\n");
	case eopenscmanp: return("Error opening Service Control Manager (in stopkerneldriver)  [eopenscmanp]\n");
	case eopenservicep: return("Error opening kernel service (in stopkerneldriver)  [eopenservicep]\n");
	case econtrolservice: return("Error in control service (in stopkerneldriver) [econtrolservice]\n");
	case eunmapmem: return("Error unmapping memory [eunmapmem]\n");
	case egetirq: return("Error getting IRQ number [egetirq]\n");
	case eallocresources: return("Error allocating resources; see readme.pdf for details on resource allocation problems [eallocresources]\n");
	case egetramsize: return ("Error getting RAM size [egetramsize]\n");
	case ekernelwriteattrib: return ("Error writing attribute memory [ekernelwriteattrib]\n");
	case ekernelreadattrib: return ("Error reading attribute memory [ekernelreadattrib]\n");
	case ekernelfrontdesk: return ("Error opening kernel device; check ExcConfig set up [ekernelfrontdesk]\n");
	case ekernelOscheck: return ("Error determining operating system [ekernelOscheck]\n");
	case ekernelfrontdeskload: return ("Error loading frontdesk.dll [ekernelfrontdeskload]\n");
	case ekerneliswin2000compatible: return ("Error determining Windows 2000 compatibility [ekerneliswin2000compatible]\n");
	case ekernelbankramsize: return("Error determining memory size [ekernelbankramsize]\n");
	case ekernelgetcardtype: return ("Error getting card type [ekernelgetcardtype]\n");	
	case regnotset: return("Module not configured; reboot after ExcConfig is run and board is in slot [regnotset]\n");
	case ekernelbankphysaddr: return ("Error getting physical memory addressekernelbankphysaddr]\n");
	case ekernelclosedevice: return ("Error closing kernel device [ekernelclosedevice]\n");
	case ekerneldevicenotopen: return ("Error returned by kernel: device not open [ekerneldevicenotopen]\n");
	case ekernelinitmodule: return ("Error initializing kernel [ekernelinitmodule]\n");
	case ekernelbadparam: return ("Error returned by kernel: bad input parameter [ekernelbadparam]\n");
	case ekernelbadpointer: return ("Error returned by kernel: invalid pointer to output buffer [ekernelbadpointer]\n");
	case ekerneltimeout: return ("Timeout expired before interrupt occurred [ekerneltimeout]\n");
	case ekernelnotwin2000: return ("Error returned by kernel: operating system is not Windows 2000 compatible [ekernelnotwin2000]\n");
	case erequestnotification: return ("Error requesting interrupt notification [erequestnotification]\n");

	default: sprintf(errStr, "No such error %d\n", errorcode);
		return (perrStr);
	}
}

int __declspec(dllexport) Get_PtrBoardInstance_4000(int handle, INSTANCE_EXC4000 ** ppBoardInstance)
{
	*ppBoardInstance = NULL;

	if ((handle <0) || (handle >= NUMBOARDS))
		return ebaddevhandle;
	if (!exc4000card[handle].allocCounter)
		return ebaddevhandle;

	*ppBoardInstance = &exc4000card[handle];

	return 0;
}  /* -----  end of Get_PtrBoardInstance_4000()  ----- */

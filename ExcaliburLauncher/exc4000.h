
/* exc4000.h
    Routines for communicating with our 400PCI base board */

#ifndef __EXC4000_H_
#define __EXC4000_H_

#ifdef _WIN32
#include <windows.h>
#endif

#define NUMBOARDS 16  /* EXC4000PCI dip switch covers numbers 0-15 */

/* select_ and get__timetag_source */
#define EXC4000_INTERNAL_CLOCK	0
#define EXC4000_EXTERNAL_CLOCK	1

/* Irig stuff - for user */
#define IRIG_TIME_AND_RESET	0x100
#define IRIG_TIME		0x200

#define TIMER_RELOAD		1
#define TIMER_NO_RELOAD		0
#define TIMER_INTERRUPT		1
#define TIMER_NO_INTERRUPT	0
#define TIMER_GLOBRESET		1
#define TIMER_NO_GLOBRESET	0

/* the timer on M4K cards is treated as module 4 */
#define EXC4000_MODULE_TIMER	4

#ifndef MAX_MODULES_ON_BOARD
#define MAX_MODULES_ON_BOARD	8
#endif

#define GLOBALREG_BANK	4

#define EXC4000PCI_BOARDIDMASK		0x000F
#define EXC4000PCI_BOARDSIGMASK		0xFF00
#define EXC4000PCI_BOARDSIGVALUE	0x4000
#define EXC4000PCIE_BOARDSIGVALUE	0x4E00
#define EXCAFDXPCI_BOARDSIGVALUE	0xAF00

#define EXC4000PCI_MODULETYPEMASK	0x1F

#define EXC4000PCI_GLOBAL_TTAG_RESET	0x0010

#define IRIG_TIME_AVAIL			0x400

#define TM_STARTBITVAL	1

#define TM_STARTBIT	0
#define TM_RELOADBIT	1
#define TM_INTERRUPTBIT 2
#define TM_GLOBRESETBIT 3

typedef struct{
	unsigned short int days;
	unsigned short int hours;
	unsigned short int minutes;
	unsigned short int seconds;
}t_IrigTime;


typedef struct
	{
		WORD boardSigId;
		WORD softwareReset;
		WORD interruptStatus;
		WORD interruptReset;
		WORD moduleInfo[4];
		WORD clocksourceSelect;
		WORD reserved;
		WORD IRcommand;
		WORD IRsecondsOfDay;
		WORD IRdaysHours;
		WORD IRminsSecs;
		WORD IRcontrol1;
		WORD IRcontrol2;
		WORD fpgaRevision;
		WORD TMprescale;
		WORD TMpreload;
		WORD TMcontrol;
		WORD TMcounter;
		WORD reserved_2[4];
		WORD boardType;
		WORD boardInfo;
		WORD reserved_3;
		WORD moduleInfoSecondGroup[4];
	} t_globalRegs4000;

typedef struct
{
	int device_num;
	int allocCounter;
	t_globalRegs4000 *globreg;


} INSTANCE_EXC4000;

/* Prototypes  --  Exported */

#ifdef __cplusplus
extern "C"
{
#endif
	int __declspec(dllexport) Get_4000Board_Type(WORD device_num, WORD *boardtype);
	int __declspec(dllexport) Get_4000Module_Type(WORD device_num, WORD module_num, WORD *modtype);
	int __declspec(dllexport) Get_UniqueID_P4000(WORD device_num, WORD *pwUniqueID);
	int __declspec(dllexport) Select_Time_Tag_Source_4000 (WORD device_num, WORD source);
	int __declspec(dllexport) Get_Time_Tag_Source_4000 (WORD device_num, WORD *source);
	int __declspec(dllexport) Reset_Module_4000(WORD device_num, WORD module_num);
	int __declspec(dllexport) Reset_Timetags_On_All_Modules_4000(WORD device_num);

	int __declspec(dllexport) Init_Timers_4000 (WORD device_num, int *handle);
	int __declspec(dllexport) Release_Timers_4000 (int handle);

	int __declspec(dllexport) SetIrig_4000(int handle, WORD flag);
	int __declspec(dllexport) IsIrigTimeavail_4000(int handle, int *availFlag);
	int __declspec(dllexport) GetIrigSeconds_4000(int handle, unsigned long *seconds);
	int __declspec(dllexport) GetIrigTime_4000(int handle, t_IrigTime *IrigTime);
	int __declspec(dllexport) GetIrigControl_4000(int handle, unsigned long *control);

	int __declspec(dllexport) StartTimer_4000(int handle, unsigned long microsecsToCount, int reload_flag, int interrupt_flag, int globalreset_flag, int *timeoffset);
	int __declspec(dllexport) StopTimer_4000(int handle, unsigned long *timervalue);
	int __declspec(dllexport) ReadTimerValue_4000(int handle, DWORD *timervalue);
	BOOL __declspec(dllexport) IsTimerRunning_4000(int handle, int *errorcondition);
	int __declspec(dllexport) ResetWatchdogTimer_4000(int handle);

	int __declspec(dllexport) InitializeInterrupt_P4000(int handle);
	int __declspec(dllexport) Wait_for_Interrupt_P4000(int handle, unsigned int timeout);
	int __declspec(dllexport) Get_Interrupt_Count_P4000(int handle, unsigned long *Sys_Interrupts_Ptr);

	int __declspec(dllexport) IsDMASupported_4000(WORD device_num);
	int __declspec(dllexport) IsRepetitiveDMASupported_4000(WORD device_num);
	int __declspec(dllexport) IsExpressCard_4000(WORD wBoardType);
	int __declspec(dllexport) IsPciExpress_4000(WORD wBoardType);
	int __declspec(dllexport) Verify4000SeriesBoard(t_globalRegs4000 *globreg);



	int __declspec(dllexport) Get_Error_String_4000(int errcode,int errlen, char *errstring);
	int __declspec(dllexport) Get_4000Interface_Rev(WORD device_num,WORD *interface_rev);
	int __declspec(dllexport) Get_PtrBoardInstance_4000(int handle, INSTANCE_EXC4000 ** ppBoardInstance);

#ifdef __cplusplus
}
#endif

#endif

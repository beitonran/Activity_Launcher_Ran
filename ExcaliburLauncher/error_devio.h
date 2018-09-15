#ifndef _DEVIO_ERROR_H
#define _DEVIO_ERROR_H

/* Error codes for the Excalibur card Kernel Drivers (non-VME/VXI) */

#define eopenkernel					-1001
#define ekernelcantmap	   			-1002
#define ereleventhandle				-1003
#define egetintcount				-1004
#define egetchintcount				-1005
#define egetintchannels				-1006
#define ewriteiobyte				-1007
#define ereadiobyte					-1008
#define egeteventhand1				-1009
#define egeteventhand2				-1010
#define eopenscmant					-1011
#define eopenservicet				-1012
#define estartservice				-1013
#define eopenscmanp					-1014
#define eopenservicep				-1015
#define econtrolservice				-1016
#define eunmapmem					-1017
#define egetirq						-1018
#define eallocresources				-1019
#define egetramsize					-1020
#define ekernelwriteattrib			-1021
#define ekernelreadattrib			-1022
#define ekernelfrontdesk			-1023
#define ekernelOscheck				-1024
#define ekernelfrontdeskload		-1025
#define ekerneliswin2000compatible	-1026
#define ekernelbankramsize 			-1027
#define ekernelgetcardtype			-1028
#define regnotset					-1030
#define ekernelbankphysaddr			-1031
#define ekernelclosedevice			-1032
#define ekerneldevicenotopen		-1034
#define ekernelinitmodule			-1035
#define ekernelbadparam				-1036
#define ekernelbadpointer			-1037
#define ekerneltimeout				-1038
#define ekernelnotwin2000			-1039
#define erequestnotification		-1040

/* Error codes for VMIC */ 
#define evmic_vmeInit				-1043
#define evmic_lock16window			-1044
#define evmic_get16windowadr		-1045
#define evmic_lockmemwindow			-1046
#define evmic_getmemwindowadr		-1047
#define evmic_unlockwindow			-1048
#define evmic_freewindowadr			-1049
#define evmic_intnum				-1070
#define evmic_nointnumset			-1071

/* Error codes for initialization of Excalibur VME/VXI/VISA cards */ 

#define eviclosedev					-1050
#define evicloserm					-1051
#define eopendefaultrm				-1052
#define eviopen						-1053
#define evimapaddress				-1054
#define evicommand					-1055
#define einstallhandler				-1056
#define eenableevent				-1057
#define euninstallhandler			-1058
#define edevnum						-1060
#define einstr						-1061
#define eirq						-1084
#define ebadrsrcnamepointer			-1086

/* Error codes for vxworks for powerpc */

#define evmea16						-1072
#define evmea24						-1073
#define evmea32						-1074

/* error codes for vxworks for pentium and 6100/radstone */
#define epciglob4000badr			-1075
#define epcibadr					-1076
#define edevnotfound				-1077
#define edipnum						-1078
#define eboardtypenotfound			-1079
#define	einterruptFunc				-1080
#define evbadr						-1081
#define efuncinvalid				-1082
#define ebyteswapping				-1085

/* Error codes for exc4000.c */

#define emodnum						-1029 /*returned by init_module */
#define ekernelnot4000card			-1041 /*returned by init_module */
#define enotimersirig				-1042
#define eclocksource				-1059
#define eparmglobalreset			-1062
#define etimernotrunning			-1063
#define etimerrunning				-1064
#define eparmreload					-1065
#define eparminterrupt				-1066
#define ebaddevhandle				-1067
#define edevtoomany					-1068
#define einvalidOS					-1069
#define einvalttagsource			-1087
#define enotforexcard				-1088

/* Error codes for EXC1394.c */
#define enodnum				-1083
#define ekernelnot1394card	-1084

/* Error codes for DMA */
#define edmareadfail		-1100
#define edmawritefail		-1101
#define edmanotsupported	-1102
#define eisdmasupported     -1103
#define einvaliddmaparam	-1104
#define ekernelwaitinterrupt -1105
#define enotimplemented -1106

/* Error codes for dispatch DLL used with LabVIEW */
#define edispatchertargetDLLmissing		-1200
#define edispatchertargetDLLincorrect	-1201
#define edispatcherbaddispatchhandle	-1202
#define edispatcherbaddispatchhandle1	-1221
#define edispatcherbaddispatchhandle2	-1222
#define edispatchercantfindfunction		-1203
#define edispatchbadDLLName				-1204
#define edispatchbadIniFileSection		-1205
#define edispatcherbadchannelhandle		-1206

#define edispatchertargetDLLincorrect1	-1211
#define edispatchertargetDLLincorrect2	-1212
#define edispatchertargetDLLincorrect3	-1213
#define edispatchertargetDLLincorrect4	-1214
#define edispatchertargetDLLincorrect5	-1215
#define edispatchertargetDLLincorrect6	-1216
#define edispatchertargetDLLincorrect7	-1217
#define edispatchertargetDLLincorrect8	-1218

/******************************************
 NOT USED: 
     -1088 through -1099
     -1107 through -1199
     -1207 and up
     
******************************************/

#endif

/*
Codes:	000: m4kpx
	150: microace
	200: serial, 708, can
	300: m4kmch
	400: m4krtx
	500: discretes
	600: firewire
	1000: devio
*/





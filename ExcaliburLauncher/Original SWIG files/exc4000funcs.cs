/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.10
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;
using System.Runtime.InteropServices;

public class exc4000funcs {
  public static int Get_UniqueID_P4000(ushort device_num, out ushort OUTPUT) {
    int ret = exc4000funcsPINVOKE.Get_UniqueID_P4000(device_num, out OUTPUT);
    return ret;
  }

  public static int Get_4000Board_Type(ushort device_num, out ushort OUTPUT) {
    int ret = exc4000funcsPINVOKE.Get_4000Board_Type(device_num, out OUTPUT);
    return ret;
  }

  public static int Get_4000Module_Type(ushort device_num, ushort module_num, out ushort OUTPUT) {
    int ret = exc4000funcsPINVOKE.Get_4000Module_Type(device_num, module_num, out OUTPUT);
    return ret;
  }

}
using System;

namespace Atomixilc
{
    internal enum ILCode : ushort
    {
        //
        // Summary:
        //     Adds two values and pushes the result onto the evaluation stack.
        Add = 0x0058,
        Add_Ovf = 0x00D6,
        //
        // Summary:
        //     Adds two unsigned integer values, performs an overflow check, and pushes the
        //     result onto the evaluation stack.
        Add_Ovf_Un = 0x00D7,
        //
        // Summary:
        //     Computes the bitwise AND of two values and pushes the result onto the evaluation
        //     stack.
        And = 0x005F,
        //
        // Summary:
        //     Returns an unmanaged pointer to the argument list of the current method.
        Arglist = 0xFE00,
        //
        // Summary:
        //     Transfers control to a target instruction if two values are equal.
        Beq = 0x003B,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if two values are equal.
        Beq_S = 0x002E,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     or equal to the second value.
        Bge = 0x003C,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than or equal to the second value.
        Bge_S = 0x002F,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value, when comparing unsigned integer values or unordered float values.
        Bge_Un = 0x0041,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Bge_Un_S = 0x0034,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value.
        Bgt = 0x003D,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value.
        Bgt_S = 0x0030,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value, when comparing unsigned integer values or unordered float values.
        Bgt_Un = 0x0042,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Bgt_Un_S = 0x0035,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than or
        //     equal to the second value.
        Ble = 0x003E,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than or equal to the second value.
        Ble_S = 0x0031,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than or
        //     equal to the second value, when comparing unsigned integer values or unordered
        //     float values.
        Ble_Un = 0x0043,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than or equal to the second value, when comparing unsigned integer values
        //     or unordered float values.
        Ble_Un_S = 0x0036,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than the
        //     second value.
        Blt = 0x003F,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than the second value.
        Blt_S = 0x0032,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than the
        //     second value, when comparing unsigned integer values or unordered float values.
        Blt_Un = 0x0044,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Blt_Un_S = 0x0037,
        //
        // Summary:
        //     Transfers control to a target instruction when two unsigned integer values or
        //     unordered float values are not equal.
        Bne_Un = 0x0040,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) when two unsigned integer
        //     values or unordered float values are not equal.
        Bne_Un_S = 0x0033,
        //
        // Summary:
        //     Converts a value type to an object reference (type O).
        Box = 0x008C,
        //
        // Summary:
        //     Unconditionally transfers control to a target instruction.
        Br = 0x0038,
        //
        // Summary:
        //     Signals the Common Language Infrastructure (CLI) to inform the debugger that
        //     a break point has been tripped.
        Break = 0x0001,
        //
        // Summary:
        //     Transfers control to a target instruction if value is false, a null reference
        //     (Nothing in Visual Basic), or zero.
        Brfalse = 0x0039,
        //
        // Summary:
        //     Transfers control to a target instruction if value is false, a null reference,
        //     or zero.
        Brfalse_S = 0x002C,
        //
        // Summary:
        //     Transfers control to a target instruction if value is true, not null, or non-zero.
        Brtrue = 0x003A,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if value is true, not
        //     null, or non-zero.
        Brtrue_S = 0x002D,
        //
        // Summary:
        //     Unconditionally transfers control to a target instruction (short form).
        Br_S = 0x002B,
        //
        // Summary:
        //     Calls the method indicated by the passed method descriptor.
        Call = 0x0028,
        //
        // Summary:
        //     Calls the method indicated on the evaluation stack (as a pointer to an entry
        //     point) with arguments described by a calling convention.
        Calli = 0x0029,
        //
        // Summary:
        //     Calls a late-bound method on an object, pushing the return value onto the evaluation
        //     stack.
        Callvirt = 0x006F,
        //
        // Summary:
        //     Attempts to cast an object passed by reference to the specified class.
        Castclass = 0x0074,
        //
        // Summary:
        //     Compares two values. If they are equal, the integer value 1 (int32) is pushed
        //     onto the evaluation stack, otherwise 0 (int32) is pushed onto the evaluation
        //     stack.
        Ceq = 0xFE01,
        //
        // Summary:
        //     Compares two values. If the first value is greater than the second, the integer
        //     value 1 (int32) is pushed onto the evaluation stack, otherwise 0 (int32) is pushed
        //     onto the evaluation stack.
        Cgt = 0xFE02,
        //
        // Summary:
        //     Compares two unsigned or unordered values. If the first value is greater than
        //     the second, the integer value 1 (int32) is pushed onto the evaluation stack,
        //     otherwise 0 (int32) is pushed onto the evaluation stack.
        Cgt_Un = 0xFE03,
        //
        // Summary:
        //     Throws System.ArithmeticException if value is not a finite number.
        Ckfinite = 0x00C3,
        //
        // Summary:
        //     Compares two values. If the first value is less than the second, the integer
        //     value 1 (int32) is pushed onto the evaluation stack, otherwise 0 (int32) is pushed
        //     onto the evaluation stack.
        Clt = 0xFE04,
        //
        // Summary:
        //     Compares the unsigned or unordered values value1 and value2. If value1 is less
        //     than value2, then the integer value 1 (int32) is pushed onto the evaluation stack,
        //     otherwise 0 (int32) is pushed onto the evaluation stack.
        Clt_Un = 0xFE05,
        //
        // Summary:
        //     Constrains the type on which a virtual method call is made.
        Constrained = 0xFE16,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to native int.
        Conv_I = 0x00D3,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int8, then extends (pads)
        //     it to int32.
        Conv_I1 = 0x0067,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int16, then extends (pads)
        //     it to int32.
        Conv_I2 = 0x0068,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int32.
        Conv_I4 = 0x0069,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int64.
        Conv_I8 = 0x006A,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_I = 0x00D4,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int8 and extends
        //     it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I1 = 0x00B3,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I1_Un = 0x0082,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int16 and
        //     extending it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I2 = 0x00B5,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int16 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I2_Un = 0x0083,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I4 = 0x00B7,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I4_Un = 0x0084,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I8 = 0x00B9,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I8_Un = 0x0085,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_I_Un = 0x008A,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U = 0x00D5,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U1 = 0x00B4,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U1_Un = 0x0086,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int16 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U2 = 0x00B6,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int16
        //     and extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U2_Un = 0x0087,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_U4 = 0x00B8,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int32,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U4_Un = 0x0088,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_U8 = 0x00BA,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int64,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U8_Un = 0x0089,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned native
        //     int, throwing System.OverflowException on overflow.
        Conv_Ovf_U_Un = 0x008B,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to float32.
        Conv_R4 = 0x006B,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to float64.
        Conv_R8 = 0x006C,
        //
        // Summary:
        //     Converts the unsigned integer value on top of the evaluation stack to float32.
        Conv_R_Un = 0x0076,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned native int, and
        //     extends it to native int.
        Conv_U = 0x00E0,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int8, and extends
        //     it to int32.
        Conv_U1 = 0x00D2,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int16, and extends
        //     it to int32.
        Conv_U2 = 0x00D1,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int32, and extends
        //     it to int32.
        Conv_U4 = 0x006D,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int64, and extends
        //     it to int64.
        Conv_U8 = 0x006E,
        //
        // Summary:
        //     Copies a specified number bytes from a source address to a destination address.
        Cpblk = 0xFE17,
        //
        // Summary:
        //     Copies the value type located at the address of an object (type &, * or native
        //     int) to the address of the destination object (type &, * or native int).
        Cpobj = 0x0070,
        //
        // Summary:
        //     Divides two values and pushes the result as a floating-point (type F) or quotient
        //     (type int32) onto the evaluation stack.
        Div = 0x005B,
        //
        // Summary:
        //     Divides two unsigned integer values and pushes the result (int32) onto the evaluation
        //     stack.
        Div_Un = 0x005C,
        //
        // Summary:
        //     Copies the current topmost value on the evaluation stack, and then pushes the
        //     copy onto the evaluation stack.
        Dup = 0x0025,
        //
        // Summary:
        //     Transfers control from the filter clause of an exception back to the Common Language
        //     Infrastructure (CLI) exception handler.
        Endfilter = 0xFE11,
        //
        // Summary:
        //     Transfers control from the fault or finally clause of an exception block back
        //     to the Common Language Infrastructure (CLI) exception handler.
        Endfinally = 0x00DC,
        //
        // Summary:
        //     Initializes a specified block of memory at a specific address to a given size
        //     and initial value.
        Initblk = 0xFE18,
        //
        // Summary:
        //     Initializes each field of the value type at a specified address to a null reference
        //     or a 0 of the appropriate primitive type.
        Initobj = 0xFE15,
        //
        // Summary:
        //     Tests whether an object reference (type O) is an instance of a particular class.
        Isinst = 0x0075,
        //
        // Summary:
        //     Exits current method and jumps to specified method.
        Jmp = 0x0027,
        //
        // Summary:
        //     Loads an argument (referenced by a specified index value) onto the stack.
        Ldarg = 0xFE09,
        //
        // Summary:
        //     Load an argument address onto the evaluation stack.
        Ldarga = 0xFE0A,
        //
        // Summary:
        //     Load an argument address, in short form, onto the evaluation stack.
        Ldarga_S = 0x000F,
        //
        // Summary:
        //     Loads the argument at index 0 onto the evaluation stack.
        Ldarg_0 = 0x0002,
        //
        // Summary:
        //     Loads the argument at index 1 onto the evaluation stack.
        Ldarg_1 = 0x0003,
        //
        // Summary:
        //     Loads the argument at index 2 onto the evaluation stack.
        Ldarg_2 = 0x0004,
        //
        // Summary:
        //     Loads the argument at index 3 onto the evaluation stack.
        Ldarg_3 = 0x0005,
        //
        // Summary:
        //     Loads the argument (referenced by a specified short form index) onto the evaluation
        //     stack.
        Ldarg_S = 0x000E,
        //
        // Summary:
        //     Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        Ldc_I4 = 0x0020,
        //
        // Summary:
        //     Pushes the integer value of 0 onto the evaluation stack as an int32.
        Ldc_I4_0 = 0x0016,
        //
        // Summary:
        //     Pushes the integer value of 1 onto the evaluation stack as an int32.
        Ldc_I4_1 = 0x0017,
        //
        // Summary:
        //     Pushes the integer value of 2 onto the evaluation stack as an int32.
        Ldc_I4_2 = 0x0018,
        //
        // Summary:
        //     Pushes the integer value of 3 onto the evaluation stack as an int32.
        Ldc_I4_3 = 0x0019,
        //
        // Summary:
        //     Pushes the integer value of 4 onto the evaluation stack as an int32.
        Ldc_I4_4 = 0x001A,
        //
        // Summary:
        //     Pushes the integer value of 5 onto the evaluation stack as an int32.
        Ldc_I4_5 = 0x001B,
        //
        // Summary:
        //     Pushes the integer value of 6 onto the evaluation stack as an int32.
        Ldc_I4_6 = 0x001C,
        //
        // Summary:
        //     Pushes the integer value of 7 onto the evaluation stack as an int32.
        Ldc_I4_7 = 0x001D,
        //
        // Summary:
        //     Pushes the integer value of 8 onto the evaluation stack as an int32.
        Ldc_I4_8 = 0x001E,
        //
        // Summary:
        //     Pushes the integer value of -1 onto the evaluation stack as an int32.
        Ldc_I4_M1 = 0x0015,
        //
        // Summary:
        //     Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        Ldc_I4_S = 0x001F,
        //
        // Summary:
        //     Pushes a supplied value of type int64 onto the evaluation stack as an int64.
        Ldc_I8 = 0x0021,
        //
        // Summary:
        //     Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        Ldc_R4 = 0x0022,
        //
        // Summary:
        //     Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        Ldc_R8 = 0x0023,
        //
        // Summary:
        //     Loads the element at a specified array index onto the top of the evaluation stack
        //     as the type specified in the instruction.
        Ldelem = 0x00A3,
        //
        // Summary:
        //     Loads the address of the array element at a specified array index onto the top
        //     of the evaluation stack as type & (managed pointer).
        Ldelema = 0x008F,
        //
        // Summary:
        //     Loads the element with type native int at a specified array index onto the top
        //     of the evaluation stack as a native int.
        Ldelem_I = 0x0097,
        //
        // Summary:
        //     Loads the element with type int8 at a specified array index onto the top of the
        //     evaluation stack as an int32.
        Ldelem_I1 = 0x0090,
        //
        // Summary:
        //     Loads the element with type int16 at a specified array index onto the top of
        //     the evaluation stack as an int32.
        Ldelem_I2 = 0x0092,
        //
        // Summary:
        //     Loads the element with type int32 at a specified array index onto the top of
        //     the evaluation stack as an int32.
        Ldelem_I4 = 0x0094,
        //
        // Summary:
        //     Loads the element with type int64 at a specified array index onto the top of
        //     the evaluation stack as an int64.
        Ldelem_I8 = 0x0096,
        //
        // Summary:
        //     Loads the element with type float32 at a specified array index onto the top of
        //     the evaluation stack as type F (float).
        Ldelem_R4 = 0x0098,
        //
        // Summary:
        //     Loads the element with type float64 at a specified array index onto the top of
        //     the evaluation stack as type F (float).
        Ldelem_R8 = 0x0099,
        //
        // Summary:
        //     Loads the element containing an object reference at a specified array index onto
        //     the top of the evaluation stack as type O (object reference).
        Ldelem_Ref = 0x009A,
        //
        // Summary:
        //     Loads the element with type unsigned int8 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U1 = 0x0091,
        //
        // Summary:
        //     Loads the element with type unsigned int16 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U2 = 0x0093,
        //
        // Summary:
        //     Loads the element with type unsigned int32 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U4 = 0x0095,
        //
        // Summary:
        //     Finds the value of a field in the object whose reference is currently on the
        //     evaluation stack.
        Ldfld = 0x007B,
        //
        // Summary:
        //     Finds the address of a field in the object whose reference is currently on the
        //     evaluation stack.
        Ldflda = 0x007C,
        //
        // Summary:
        //     Pushes an unmanaged pointer (type native int) to the native code implementing
        //     a specific method onto the evaluation stack.
        Ldftn = 0xFE06,
        //
        // Summary:
        //     Loads a value of type native int as a native int onto the evaluation stack indirectly.
        Ldind_I = 0x004D,
        //
        // Summary:
        //     Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
        Ldind_I1 = 0x0046,
        //
        // Summary:
        //     Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
        Ldind_I2 = 0x0048,
        //
        // Summary:
        //     Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
        Ldind_I4 = 0x004A,
        //
        // Summary:
        //     Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
        Ldind_I8 = 0x004C,
        //
        // Summary:
        //     Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
        Ldind_R4 = 0x004E,
        //
        // Summary:
        //     Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
        Ldind_R8 = 0x004F,
        //
        // Summary:
        //     Loads an object reference as a type O (object reference) onto the evaluation
        //     stack indirectly.
        Ldind_Ref = 0x0050,
        //
        // Summary:
        //     Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
        Ldind_U1 = 0x0047,
        //
        // Summary:
        //     Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
        Ldind_U2 = 0x0049,
        //
        // Summary:
        //     Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
        Ldind_U4 = 0x004B,
        //
        // Summary:
        //     Pushes the number of elements of a zero-based, one-dimensional array onto the
        //     evaluation stack.
        Ldlen = 0x008E,
        //
        // Summary:
        //     Loads the local variable at a specific index onto the evaluation stack.
        Ldloc = 0xFE0C,
        //
        // Summary:
        //     Loads the address of the local variable at a specific index onto the evaluation
        //     stack.
        Ldloca = 0xFE0D,
        //
        // Summary:
        //     Loads the address of the local variable at a specific index onto the evaluation
        //     stack, short form.
        Ldloca_S = 0x0012,
        //
        // Summary:
        //     Loads the local variable at index 0 onto the evaluation stack.
        Ldloc_0 = 0x0006,
        //
        // Summary:
        //     Loads the local variable at index 1 onto the evaluation stack.
        Ldloc_1 = 0x0007,
        //
        // Summary:
        //     Loads the local variable at index 2 onto the evaluation stack.
        Ldloc_2 = 0x0008,
        //
        // Summary:
        //     Loads the local variable at index 3 onto the evaluation stack.
        Ldloc_3 = 0x0009,
        //
        // Summary:
        //     Loads the local variable at a specific index onto the evaluation stack, short
        //     form.
        Ldloc_S = 0x0011,
        //
        // Summary:
        //     Pushes a null reference (type O) onto the evaluation stack.
        Ldnull = 0x0014,
        //
        // Summary:
        //     Copies the value type object pointed to by an address to the top of the evaluation
        //     stack.
        Ldobj = 0x0071,
        //
        // Summary:
        //     Pushes the value of a static field onto the evaluation stack.
        Ldsfld = 0x007E,
        //
        // Summary:
        //     Pushes the address of a static field onto the evaluation stack.
        Ldsflda = 0x007F,
        //
        // Summary:
        //     Pushes a new object reference to a string literal stored in the metadata.
        Ldstr = 0x0072,
        //
        // Summary:
        //     Converts a metadata token to its runtime representation, pushing it onto the
        //     evaluation stack.
        Ldtoken = 0x00D0,
        //
        // Summary:
        //     Pushes an unmanaged pointer (type native int) to the native code implementing
        //     a particular virtual method associated with a specified object onto the evaluation
        //     stack.
        Ldvirtftn = 0xFE07,
        //
        // Summary:
        //     Exits a protected region of code, unconditionally transferring control to a specific
        //     target instruction.
        Leave = 0x00DD,
        //
        // Summary:
        //     Exits a protected region of code, unconditionally transferring control to a target
        //     instruction (short form).
        Leave_S = 0x00DE,
        //
        // Summary:
        //     Allocates a certain number of bytes from the local dynamic memory pool and pushes
        //     the address (a transient pointer, type *) of the first allocated byte onto the
        //     evaluation stack.
        Localloc = 0xFE0F,
        //
        // Summary:
        //     Pushes a typed reference to an instance of a specific type onto the evaluation
        //     stack.
        Mkrefany = 0x00C6,
        //
        // Summary:
        //     Multiplies two values and pushes the result on the evaluation stack.
        Mul = 0x005A,
        //
        // Summary:
        //     Multiplies two integer values, performs an overflow check, and pushes the result
        //     onto the evaluation stack.
        Mul_Ovf = 0x00D8,
        //
        // Summary:
        //     Multiplies two unsigned integer values, performs an overflow check, and pushes
        //     the result onto the evaluation stack.
        Mul_Ovf_Un = 0x00D9,
        //
        // Summary:
        //     Negates a value and pushes the result onto the evaluation stack.
        Neg = 0x0065,
        //
        // Summary:
        //     Pushes an object reference to a new zero-based, one-dimensional array whose elements
        //     are of a specific type onto the evaluation stack.
        Newarr = 0x008D,
        //
        // Summary:
        //     Creates a new object or a new instance of a value type, pushing an object reference
        //     (type O) onto the evaluation stack.
        Newobj = 0x0073,
        //
        // Summary:
        //     Fills space if opcodes are patched. No meaningful operation is performed although
        //     a processing cycle can be consumed.
        Nop = 0x0000,
        //
        // Summary:
        //     Computes the bitwise complement of the integer value on top of the stack and
        //     pushes the result onto the evaluation stack as the same type.
        Not = 0x0066,
        //
        // Summary:
        //     Compute the bitwise complement of the two integer values on top of the stack
        //     and pushes the result onto the evaluation stack.
        Or = 0x0060,
        //
        // Summary:
        //     Removes the value currently on top of the evaluation stack.
        Pop = 0x0026,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix1 = 0x00FE,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix2 = 0x00FD,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix3 = 0x00FC,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix4 = 0x00FB,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix5 = 0x00FA,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix6 = 0x00F9,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix7 = 0x00F8,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefixref = 0x00FF,
        //
        // Summary:
        //     Specifies that the subsequent array address operation performs no type check
        //     at run time, and that it returns a managed pointer whose mutability is restricted.
        Readonly = 0xFE1E,
        //
        // Summary:
        //     Retrieves the type token embedded in a typed reference.
        Refanytype = 0xFE1D,
        //
        // Summary:
        //     Retrieves the address (type &) embedded in a typed reference.
        Refanyval = 0x00C2,
        //
        // Summary:
        //     Divides two values and pushes the remainder onto the evaluation stack.
        Rem = 0x005D,
        //
        // Summary:
        //     Divides two unsigned values and pushes the remainder onto the evaluation stack.
        Rem_Un = 0x005E,
        //
        // Summary:
        //     Returns from the current method, pushing a return value (if present) from the
        //     callee's evaluation stack onto the caller's evaluation stack.
        Ret = 0x002A,
        //
        // Summary:
        //     Rethrows the current exception.
        Rethrow = 0xFE1A,
        //
        // Summary:
        //     Shifts an integer value to the left (in zeroes) by a specified number of bits,
        //     pushing the result onto the evaluation stack.
        Shl = 0x0062,
        //
        // Summary:
        //     Shifts an integer value (in sign) to the right by a specified number of bits,
        //     pushing the result onto the evaluation stack.
        Shr = 0x0063,
        //
        // Summary:
        //     Shifts an unsigned integer value (in zeroes) to the right by a specified number
        //     of bits, pushing the result onto the evaluation stack.
        Shr_Un = 0x0064,
        //
        // Summary:
        //     Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
        Sizeof = 0xFE1C,
        //
        // Summary:
        //     Stores the value on top of the evaluation stack in the argument slot at a specified
        //     index.
        Starg = 0xFE0B,
        //
        // Summary:
        //     Stores the value on top of the evaluation stack in the argument slot at a specified
        //     index, short form.
        Starg_S = 0x0010,
        //
        // Summary:
        //     Replaces the array element at a given index with the value on the evaluation
        //     stack, whose type is specified in the instruction.
        Stelem = 0x00A4,
        //
        // Summary:
        //     Replaces the array element at a given index with the native int value on the
        //     evaluation stack.
        Stelem_I = 0x009B,
        //
        // Summary:
        //     Replaces the array element at a given index with the int8 value on the evaluation
        //     stack.
        Stelem_I1 = 0x009C,
        //
        // Summary:
        //     Replaces the array element at a given index with the int16 value on the evaluation
        //     stack.
        Stelem_I2 = 0x009D,
        //
        // Summary:
        //     Replaces the array element at a given index with the int32 value on the evaluation
        //     stack.
        Stelem_I4 = 0x009E,
        //
        // Summary:
        //     Replaces the array element at a given index with the int64 value on the evaluation
        //     stack.
        Stelem_I8 = 0x009F,
        //
        // Summary:
        //     Replaces the array element at a given index with the float32 value on the evaluation
        //     stack.
        Stelem_R4 = 0x00A0,
        //
        // Summary:
        //     Replaces the array element at a given index with the float64 value on the evaluation
        //     stack.
        Stelem_R8 = 0x00A1,
        //
        // Summary:
        //     Replaces the array element at a given index with the object ref value (type O)
        //     on the evaluation stack.
        Stelem_Ref = 0x00A2,
        //
        // Summary:
        //     Replaces the value stored in the field of an object reference or pointer with
        //     a new value.
        Stfld = 0x007D,
        //
        // Summary:
        //     Stores a value of type native int at a supplied address.
        Stind_I = 0x00DF,
        //
        // Summary:
        //     Stores a value of type int8 at a supplied address.
        Stind_I1 = 0x0052,
        //
        // Summary:
        //     Stores a value of type int16 at a supplied address.
        Stind_I2 = 0x0053,
        //
        // Summary:
        //     Stores a value of type int32 at a supplied address.
        Stind_I4 = 0x0054,
        //
        // Summary:
        //     Stores a value of type int64 at a supplied address.
        Stind_I8 = 0x0055,
        //
        // Summary:
        //     Stores a value of type float32 at a supplied address.
        Stind_R4 = 0x0056,
        //
        // Summary:
        //     Stores a value of type float64 at a supplied address.
        Stind_R8 = 0x0057,
        //
        // Summary:
        //     Stores a object reference value at a supplied address.
        Stind_Ref = 0x0051,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at a specified index.
        Stloc = 0xFE0E,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 0.
        Stloc_0 = 0x000A,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 1.
        Stloc_1 = 0x000B,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 2.
        Stloc_2 = 0x000C,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 3.
        Stloc_3 = 0x000D,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index (short form).
        Stloc_S = 0x0013,
        //
        // Summary:
        //     Copies a value of a specified type from the evaluation stack into a supplied
        //     memory address.
        Stobj = 0x0081,
        //
        // Summary:
        //     Replaces the value of a static field with a value from the evaluation stack.
        Stsfld = 0x0080,
        //
        // Summary:
        //     Subtracts one value from another and pushes the result onto the evaluation stack.
        Sub = 0x0059,
        //
        // Summary:
        //     Subtracts one integer value from another, performs an overflow check, and pushes
        //     the result onto the evaluation stack.
        Sub_Ovf = 0x00DA,
        //
        // Summary:
        //     Subtracts one unsigned integer value from another, performs an overflow check,
        //     and pushes the result onto the evaluation stack.
        Sub_Ovf_Un = 0x00DB,
        //
        // Summary:
        //     Implements a jump table.
        Switch = 0x0045,
        //
        // Summary:
        //     Performs a postfixed method call instruction such that the current method's stack
        //     frame is removed before the actual call instruction is executed.
        Tailcall = 0xFE14,
        //
        // Summary:
        //     Throws the exception object currently on the evaluation stack.
        Throw = 0x007A,
        //
        // Summary:
        //     Indicates that an address currently atop the evaluation stack might not be aligned
        //     to the natural size of the immediately following ldind, stind, ldfld, stfld,
        //     ldobj, stobj, initblk, or cpblk instruction.
        Unaligned = 0xFE12,
        //
        // Summary:
        //     Converts the boxed representation of a value type to its unboxed form.
        Unbox = 0x0079,
        //
        // Summary:
        //     Converts the boxed representation of a type specified in the instruction to its
        //     unboxed form.
        Unbox_Any = 0x00A5,
        //
        // Summary:
        //     Specifies that an address currently atop the evaluation stack might be volatile,
        //     and the results of reading that location cannot be cached or that multiple stores
        //     to that location cannot be suppressed.
        Volatile = 0xFE13,
        //
        // Summary:
        //     Computes the bitwise XOR of the top two values on the evaluation stack, pushing
        //     the result onto the evaluation stack.
        Xor = 0x0061,
    }
}

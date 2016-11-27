using System;

namespace Atomixilc
{
    internal enum ILCode
    {
        //
        // Summary:
        //     Adds two values and pushes the result onto the evaluation stack.
        Add,
        Add_Ovf,
        //
        // Summary:
        //     Adds two unsigned integer values, performs an overflow check, and pushes the
        //     result onto the evaluation stack.
        Add_Ovf_Un,
        //
        // Summary:
        //     Computes the bitwise AND of two values and pushes the result onto the evaluation
        //     stack.
        And,
        //
        // Summary:
        //     Returns an unmanaged pointer to the argument list of the current method.
        Arglist,
        //
        // Summary:
        //     Transfers control to a target instruction if two values are equal.
        Beq,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if two values are equal.
        Beq_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     or equal to the second value.
        Bge,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than or equal to the second value.
        Bge_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value, when comparing unsigned integer values or unordered float values.
        Bge_Un,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Bge_Un_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value.
        Bgt,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value.
        Bgt_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is greater than
        //     the second value, when comparing unsigned integer values or unordered float values.
        Bgt_Un,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     greater than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Bgt_Un_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than or
        //     equal to the second value.
        Ble,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than or equal to the second value.
        Ble_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than or
        //     equal to the second value, when comparing unsigned integer values or unordered
        //     float values.
        Ble_Un,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than or equal to the second value, when comparing unsigned integer values
        //     or unordered float values.
        Ble_Un_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than the
        //     second value.
        Blt,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than the second value.
        Blt_S,
        //
        // Summary:
        //     Transfers control to a target instruction if the first value is less than the
        //     second value, when comparing unsigned integer values or unordered float values.
        Blt_Un,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if the first value is
        //     less than the second value, when comparing unsigned integer values or unordered
        //     float values.
        Blt_Un_S,
        //
        // Summary:
        //     Transfers control to a target instruction when two unsigned integer values or
        //     unordered float values are not equal.
        Bne_Un,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) when two unsigned integer
        //     values or unordered float values are not equal.
        Bne_Un_S,
        //
        // Summary:
        //     Converts a value type to an object reference (type O).
        Box,
        //
        // Summary:
        //     Unconditionally transfers control to a target instruction.
        Br,
        //
        // Summary:
        //     Signals the Common Language Infrastructure (CLI) to inform the debugger that
        //     a break point has been tripped.
        Break,
        //
        // Summary:
        //     Transfers control to a target instruction if value is false, a null reference
        //     (Nothing in Visual Basic), or zero.
        Brfalse,
        //
        // Summary:
        //     Transfers control to a target instruction if value is false, a null reference,
        //     or zero.
        Brfalse_S,
        //
        // Summary:
        //     Transfers control to a target instruction if value is true, not null, or non-zero.
        Brtrue,
        //
        // Summary:
        //     Transfers control to a target instruction (short form) if value is true, not
        //     null, or non-zero.
        Brtrue_S,
        //
        // Summary:
        //     Unconditionally transfers control to a target instruction (short form).
        Br_S,
        //
        // Summary:
        //     Calls the method indicated by the passed method descriptor.
        Call,
        //
        // Summary:
        //     Calls the method indicated on the evaluation stack (as a pointer to an entry
        //     point) with arguments described by a calling convention.
        Calli,
        //
        // Summary:
        //     Calls a late-bound method on an object, pushing the return value onto the evaluation
        //     stack.
        Callvirt,
        //
        // Summary:
        //     Attempts to cast an object passed by reference to the specified class.
        Castclass,
        //
        // Summary:
        //     Compares two values. If they are equal, the integer value 1 (int32) is pushed
        //     onto the evaluation stack, otherwise 0 (int32) is pushed onto the evaluation
        //     stack.
        Ceq,
        //
        // Summary:
        //     Compares two values. If the first value is greater than the second, the integer
        //     value 1 (int32) is pushed onto the evaluation stack, otherwise 0 (int32) is pushed
        //     onto the evaluation stack.
        Cgt,
        //
        // Summary:
        //     Compares two unsigned or unordered values. If the first value is greater than
        //     the second, the integer value 1 (int32) is pushed onto the evaluation stack,
        //     otherwise 0 (int32) is pushed onto the evaluation stack.
        Cgt_Un,
        //
        // Summary:
        //     Throws System.ArithmeticException if value is not a finite number.
        Ckfinite,
        //
        // Summary:
        //     Compares two values. If the first value is less than the second, the integer
        //     value 1 (int32) is pushed onto the evaluation stack, otherwise 0 (int32) is pushed
        //     onto the evaluation stack.
        Clt,
        //
        // Summary:
        //     Compares the unsigned or unordered values value1 and value2. If value1 is less
        //     than value2, then the integer value 1 (int32) is pushed onto the evaluation stack,
        //     otherwise 0 (int32) is pushed onto the evaluation stack.
        Clt_Un,
        //
        // Summary:
        //     Constrains the type on which a virtual method call is made.
        Constrained,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to native int.
        Conv_I,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int8, then extends (pads)
        //     it to int32.
        Conv_I1,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int16, then extends (pads)
        //     it to int32.
        Conv_I2,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int32.
        Conv_I4,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to int64.
        Conv_I8,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_I,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int8 and extends
        //     it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I1,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I1_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int16 and
        //     extending it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I2,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int16 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_I2_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I4,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I4_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to signed int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I8,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_I8_Un,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to signed native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_I_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned native int,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U1,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int8 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U1_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int16 and
        //     extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U2,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int16
        //     and extends it to int32, throwing System.OverflowException on overflow.
        Conv_Ovf_U2_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int32, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_U4,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int32,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U4_Un,
        //
        // Summary:
        //     Converts the signed value on top of the evaluation stack to unsigned int64, throwing
        //     System.OverflowException on overflow.
        Conv_Ovf_U8,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned int64,
        //     throwing System.OverflowException on overflow.
        Conv_Ovf_U8_Un,
        //
        // Summary:
        //     Converts the unsigned value on top of the evaluation stack to unsigned native
        //     int, throwing System.OverflowException on overflow.
        Conv_Ovf_U_Un,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to float32.
        Conv_R4,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to float64.
        Conv_R8,
        //
        // Summary:
        //     Converts the unsigned integer value on top of the evaluation stack to float32.
        Conv_R_Un,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned native int, and
        //     extends it to native int.
        Conv_U,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int8, and extends
        //     it to int32.
        Conv_U1,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int16, and extends
        //     it to int32.
        Conv_U2,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int32, and extends
        //     it to int32.
        Conv_U4,
        //
        // Summary:
        //     Converts the value on top of the evaluation stack to unsigned int64, and extends
        //     it to int64.
        Conv_U8,
        //
        // Summary:
        //     Copies a specified number bytes from a source address to a destination address.
        Cpblk,
        //
        // Summary:
        //     Copies the value type located at the address of an object (type &, * or native
        //     int) to the address of the destination object (type &, * or native int).
        Cpobj,
        //
        // Summary:
        //     Divides two values and pushes the result as a floating-point (type F) or quotient
        //     (type int32) onto the evaluation stack.
        Div,
        //
        // Summary:
        //     Divides two unsigned integer values and pushes the result (int32) onto the evaluation
        //     stack.
        Div_Un,
        //
        // Summary:
        //     Copies the current topmost value on the evaluation stack, and then pushes the
        //     copy onto the evaluation stack.
        Dup,
        //
        // Summary:
        //     Transfers control from the filter clause of an exception back to the Common Language
        //     Infrastructure (CLI) exception handler.
        Endfilter,
        //
        // Summary:
        //     Transfers control from the fault or finally clause of an exception block back
        //     to the Common Language Infrastructure (CLI) exception handler.
        Endfinally,
        //
        // Summary:
        //     Initializes a specified block of memory at a specific address to a given size
        //     and initial value.
        Initblk,
        //
        // Summary:
        //     Initializes each field of the value type at a specified address to a null reference
        //     or a 0 of the appropriate primitive type.
        Initobj,
        //
        // Summary:
        //     Tests whether an object reference (type O) is an instance of a particular class.
        Isinst,
        //
        // Summary:
        //     Exits current method and jumps to specified method.
        Jmp,
        //
        // Summary:
        //     Loads an argument (referenced by a specified index value) onto the stack.
        Ldarg,
        //
        // Summary:
        //     Load an argument address onto the evaluation stack.
        Ldarga,
        //
        // Summary:
        //     Load an argument address, in short form, onto the evaluation stack.
        Ldarga_S,
        //
        // Summary:
        //     Loads the argument at index 0 onto the evaluation stack.
        Ldarg_0,
        //
        // Summary:
        //     Loads the argument at index 1 onto the evaluation stack.
        Ldarg_1,
        //
        // Summary:
        //     Loads the argument at index 2 onto the evaluation stack.
        Ldarg_2,
        //
        // Summary:
        //     Loads the argument at index 3 onto the evaluation stack.
        Ldarg_3,
        //
        // Summary:
        //     Loads the argument (referenced by a specified short form index) onto the evaluation
        //     stack.
        Ldarg_S,
        //
        // Summary:
        //     Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        Ldc_I4,
        //
        // Summary:
        //     Pushes the integer value of 0 onto the evaluation stack as an int32.
        Ldc_I4_0,
        //
        // Summary:
        //     Pushes the integer value of 1 onto the evaluation stack as an int32.
        Ldc_I4_1,
        //
        // Summary:
        //     Pushes the integer value of 2 onto the evaluation stack as an int32.
        Ldc_I4_2,
        //
        // Summary:
        //     Pushes the integer value of 3 onto the evaluation stack as an int32.
        Ldc_I4_3,
        //
        // Summary:
        //     Pushes the integer value of 4 onto the evaluation stack as an int32.
        Ldc_I4_4,
        //
        // Summary:
        //     Pushes the integer value of 5 onto the evaluation stack as an int32.
        Ldc_I4_5,
        //
        // Summary:
        //     Pushes the integer value of 6 onto the evaluation stack as an int32.
        Ldc_I4_6,
        //
        // Summary:
        //     Pushes the integer value of 7 onto the evaluation stack as an int32.
        Ldc_I4_7,
        //
        // Summary:
        //     Pushes the integer value of 8 onto the evaluation stack as an int32.
        Ldc_I4_8,
        //
        // Summary:
        //     Pushes the integer value of -1 onto the evaluation stack as an int32.
        Ldc_I4_M1,
        //
        // Summary:
        //     Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        Ldc_I4_S,
        //
        // Summary:
        //     Pushes a supplied value of type int64 onto the evaluation stack as an int64.
        Ldc_I8,
        //
        // Summary:
        //     Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        Ldc_R4,
        //
        // Summary:
        //     Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        Ldc_R8,
        //
        // Summary:
        //     Loads the element at a specified array index onto the top of the evaluation stack
        //     as the type specified in the instruction.
        Ldelem,
        //
        // Summary:
        //     Loads the address of the array element at a specified array index onto the top
        //     of the evaluation stack as type & (managed pointer).
        Ldelema,
        //
        // Summary:
        //     Loads the element with type native int at a specified array index onto the top
        //     of the evaluation stack as a native int.
        Ldelem_I,
        //
        // Summary:
        //     Loads the element with type int8 at a specified array index onto the top of the
        //     evaluation stack as an int32.
        Ldelem_I1,
        //
        // Summary:
        //     Loads the element with type int16 at a specified array index onto the top of
        //     the evaluation stack as an int32.
        Ldelem_I2,
        //
        // Summary:
        //     Loads the element with type int32 at a specified array index onto the top of
        //     the evaluation stack as an int32.
        Ldelem_I4,
        //
        // Summary:
        //     Loads the element with type int64 at a specified array index onto the top of
        //     the evaluation stack as an int64.
        Ldelem_I8,
        //
        // Summary:
        //     Loads the element with type float32 at a specified array index onto the top of
        //     the evaluation stack as type F (float).
        Ldelem_R4,
        //
        // Summary:
        //     Loads the element with type float64 at a specified array index onto the top of
        //     the evaluation stack as type F (float).
        Ldelem_R8,
        //
        // Summary:
        //     Loads the element containing an object reference at a specified array index onto
        //     the top of the evaluation stack as type O (object reference).
        Ldelem_Ref,
        //
        // Summary:
        //     Loads the element with type unsigned int8 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U1,
        //
        // Summary:
        //     Loads the element with type unsigned int16 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U2,
        //
        // Summary:
        //     Loads the element with type unsigned int32 at a specified array index onto the
        //     top of the evaluation stack as an int32.
        Ldelem_U4,
        //
        // Summary:
        //     Finds the value of a field in the object whose reference is currently on the
        //     evaluation stack.
        Ldfld,
        //
        // Summary:
        //     Finds the address of a field in the object whose reference is currently on the
        //     evaluation stack.
        Ldflda,
        //
        // Summary:
        //     Pushes an unmanaged pointer (type native int) to the native code implementing
        //     a specific method onto the evaluation stack.
        Ldftn,
        //
        // Summary:
        //     Loads a value of type native int as a native int onto the evaluation stack indirectly.
        Ldind_I,
        //
        // Summary:
        //     Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
        Ldind_I1,
        //
        // Summary:
        //     Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
        Ldind_I2,
        //
        // Summary:
        //     Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
        Ldind_I4,
        //
        // Summary:
        //     Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
        Ldind_I8,
        //
        // Summary:
        //     Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
        Ldind_R4,
        //
        // Summary:
        //     Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
        Ldind_R8,
        //
        // Summary:
        //     Loads an object reference as a type O (object reference) onto the evaluation
        //     stack indirectly.
        Ldind_Ref,
        //
        // Summary:
        //     Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
        Ldind_U1,
        //
        // Summary:
        //     Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
        Ldind_U2,
        //
        // Summary:
        //     Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
        Ldind_U4,
        //
        // Summary:
        //     Pushes the number of elements of a zero-based, one-dimensional array onto the
        //     evaluation stack.
        Ldlen,
        //
        // Summary:
        //     Loads the local variable at a specific index onto the evaluation stack.
        Ldloc,
        //
        // Summary:
        //     Loads the address of the local variable at a specific index onto the evaluation
        //     stack.
        Ldloca,
        //
        // Summary:
        //     Loads the address of the local variable at a specific index onto the evaluation
        //     stack, short form.
        Ldloca_S,
        //
        // Summary:
        //     Loads the local variable at index 0 onto the evaluation stack.
        Ldloc_0,
        //
        // Summary:
        //     Loads the local variable at index 1 onto the evaluation stack.
        Ldloc_1,
        //
        // Summary:
        //     Loads the local variable at index 2 onto the evaluation stack.
        Ldloc_2,
        //
        // Summary:
        //     Loads the local variable at index 3 onto the evaluation stack.
        Ldloc_3,
        //
        // Summary:
        //     Loads the local variable at a specific index onto the evaluation stack, short
        //     form.
        Ldloc_S,
        //
        // Summary:
        //     Pushes a null reference (type O) onto the evaluation stack.
        Ldnull,
        //
        // Summary:
        //     Copies the value type object pointed to by an address to the top of the evaluation
        //     stack.
        Ldobj,
        //
        // Summary:
        //     Pushes the value of a static field onto the evaluation stack.
        Ldsfld,
        //
        // Summary:
        //     Pushes the address of a static field onto the evaluation stack.
        Ldsflda,
        //
        // Summary:
        //     Pushes a new object reference to a string literal stored in the metadata.
        Ldstr,
        //
        // Summary:
        //     Converts a metadata token to its runtime representation, pushing it onto the
        //     evaluation stack.
        Ldtoken,
        //
        // Summary:
        //     Pushes an unmanaged pointer (type native int) to the native code implementing
        //     a particular virtual method associated with a specified object onto the evaluation
        //     stack.
        Ldvirtftn,
        //
        // Summary:
        //     Exits a protected region of code, unconditionally transferring control to a specific
        //     target instruction.
        Leave,
        //
        // Summary:
        //     Exits a protected region of code, unconditionally transferring control to a target
        //     instruction (short form).
        Leave_S,
        //
        // Summary:
        //     Allocates a certain number of bytes from the local dynamic memory pool and pushes
        //     the address (a transient pointer, type *) of the first allocated byte onto the
        //     evaluation stack.
        Localloc,
        //
        // Summary:
        //     Pushes a typed reference to an instance of a specific type onto the evaluation
        //     stack.
        Mkrefany,
        //
        // Summary:
        //     Multiplies two values and pushes the result on the evaluation stack.
        Mul,
        //
        // Summary:
        //     Multiplies two integer values, performs an overflow check, and pushes the result
        //     onto the evaluation stack.
        Mul_Ovf,
        //
        // Summary:
        //     Multiplies two unsigned integer values, performs an overflow check, and pushes
        //     the result onto the evaluation stack.
        Mul_Ovf_Un,
        //
        // Summary:
        //     Negates a value and pushes the result onto the evaluation stack.
        Neg,
        //
        // Summary:
        //     Pushes an object reference to a new zero-based, one-dimensional array whose elements
        //     are of a specific type onto the evaluation stack.
        Newarr,
        //
        // Summary:
        //     Creates a new object or a new instance of a value type, pushing an object reference
        //     (type O) onto the evaluation stack.
        Newobj,
        //
        // Summary:
        //     Fills space if opcodes are patched. No meaningful operation is performed although
        //     a processing cycle can be consumed.
        Nop,
        //
        // Summary:
        //     Computes the bitwise complement of the integer value on top of the stack and
        //     pushes the result onto the evaluation stack as the same type.
        Not,
        //
        // Summary:
        //     Compute the bitwise complement of the two integer values on top of the stack
        //     and pushes the result onto the evaluation stack.
        Or,
        //
        // Summary:
        //     Removes the value currently on top of the evaluation stack.
        Pop,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix1,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix2,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix3,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix4,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix5,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix6,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefix7,
        //
        // Summary:
        //     This is a reserved instruction.
        Prefixref,
        //
        // Summary:
        //     Specifies that the subsequent array address operation performs no type check
        //     at run time, and that it returns a managed pointer whose mutability is restricted.
        Readonly,
        //
        // Summary:
        //     Retrieves the type token embedded in a typed reference.
        Refanytype,
        //
        // Summary:
        //     Retrieves the address (type &) embedded in a typed reference.
        Refanyval,
        //
        // Summary:
        //     Divides two values and pushes the remainder onto the evaluation stack.
        Rem,
        //
        // Summary:
        //     Divides two unsigned values and pushes the remainder onto the evaluation stack.
        Rem_Un,
        //
        // Summary:
        //     Returns from the current method, pushing a return value (if present) from the
        //     callee's evaluation stack onto the caller's evaluation stack.
        Ret,
        //
        // Summary:
        //     Rethrows the current exception.
        Rethrow,
        //
        // Summary:
        //     Shifts an integer value to the left (in zeroes) by a specified number of bits,
        //     pushing the result onto the evaluation stack.
        Shl,
        //
        // Summary:
        //     Shifts an integer value (in sign) to the right by a specified number of bits,
        //     pushing the result onto the evaluation stack.
        Shr,
        //
        // Summary:
        //     Shifts an unsigned integer value (in zeroes) to the right by a specified number
        //     of bits, pushing the result onto the evaluation stack.
        Shr_Un,
        //
        // Summary:
        //     Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
        Sizeof,
        //
        // Summary:
        //     Stores the value on top of the evaluation stack in the argument slot at a specified
        //     index.
        Starg,
        //
        // Summary:
        //     Stores the value on top of the evaluation stack in the argument slot at a specified
        //     index, short form.
        Starg_S,
        //
        // Summary:
        //     Replaces the array element at a given index with the value on the evaluation
        //     stack, whose type is specified in the instruction.
        Stelem,
        //
        // Summary:
        //     Replaces the array element at a given index with the native int value on the
        //     evaluation stack.
        Stelem_I,
        //
        // Summary:
        //     Replaces the array element at a given index with the int8 value on the evaluation
        //     stack.
        Stelem_I1,
        //
        // Summary:
        //     Replaces the array element at a given index with the int16 value on the evaluation
        //     stack.
        Stelem_I2,
        //
        // Summary:
        //     Replaces the array element at a given index with the int32 value on the evaluation
        //     stack.
        Stelem_I4,
        //
        // Summary:
        //     Replaces the array element at a given index with the int64 value on the evaluation
        //     stack.
        Stelem_I8,
        //
        // Summary:
        //     Replaces the array element at a given index with the float32 value on the evaluation
        //     stack.
        Stelem_R4,
        //
        // Summary:
        //     Replaces the array element at a given index with the float64 value on the evaluation
        //     stack.
        Stelem_R8,
        //
        // Summary:
        //     Replaces the array element at a given index with the object ref value (type O)
        //     on the evaluation stack.
        Stelem_Ref,
        //
        // Summary:
        //     Replaces the value stored in the field of an object reference or pointer with
        //     a new value.
        Stfld,
        //
        // Summary:
        //     Stores a value of type native int at a supplied address.
        Stind_I,
        //
        // Summary:
        //     Stores a value of type int8 at a supplied address.
        Stind_I1,
        //
        // Summary:
        //     Stores a value of type int16 at a supplied address.
        Stind_I2,
        //
        // Summary:
        //     Stores a value of type int32 at a supplied address.
        Stind_I4,
        //
        // Summary:
        //     Stores a value of type int64 at a supplied address.
        Stind_I8,
        //
        // Summary:
        //     Stores a value of type float32 at a supplied address.
        Stind_R4,
        //
        // Summary:
        //     Stores a value of type float64 at a supplied address.
        Stind_R8,
        //
        // Summary:
        //     Stores a object reference value at a supplied address.
        Stind_Ref,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at a specified index.
        Stloc,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 0.
        Stloc_0,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 1.
        Stloc_1,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 2.
        Stloc_2,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index 3.
        Stloc_3,
        //
        // Summary:
        //     Pops the current value from the top of the evaluation stack and stores it in
        //     a the local variable list at index (short form).
        Stloc_S,
        //
        // Summary:
        //     Copies a value of a specified type from the evaluation stack into a supplied
        //     memory address.
        Stobj,
        //
        // Summary:
        //     Replaces the value of a static field with a value from the evaluation stack.
        Stsfld,
        //
        // Summary:
        //     Subtracts one value from another and pushes the result onto the evaluation stack.
        Sub,
        //
        // Summary:
        //     Subtracts one integer value from another, performs an overflow check, and pushes
        //     the result onto the evaluation stack.
        Sub_Ovf,
        //
        // Summary:
        //     Subtracts one unsigned integer value from another, performs an overflow check,
        //     and pushes the result onto the evaluation stack.
        Sub_Ovf_Un,
        //
        // Summary:
        //     Implements a jump table.
        Switch,
        //
        // Summary:
        //     Performs a postfixed method call instruction such that the current method's stack
        //     frame is removed before the actual call instruction is executed.
        Tailcall,
        //
        // Summary:
        //     Throws the exception object currently on the evaluation stack.
        Throw,
        //
        // Summary:
        //     Indicates that an address currently atop the evaluation stack might not be aligned
        //     to the natural size of the immediately following ldind, stind, ldfld, stfld,
        //     ldobj, stobj, initblk, or cpblk instruction.
        Unaligned,
        //
        // Summary:
        //     Converts the boxed representation of a value type to its unboxed form.
        Unbox,
        //
        // Summary:
        //     Converts the boxed representation of a type specified in the instruction to its
        //     unboxed form.
        Unbox_Any,
        //
        // Summary:
        //     Specifies that an address currently atop the evaluation stack might be volatile,
        //     and the results of reading that location cannot be cached or that multiple stores
        //     to that location cannot be suppressed.
        Volatile,
        //
        // Summary:
        //     Computes the bitwise XOR of the top two values on the evaluation stack, pushing
        //     the result onto the evaluation stack.
        Xor,
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL Opcode
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Atomix
{
    public static class BodyProcesser
    {
        /// <summary>
        /// MSIL Low
        /// </summary>
        private static OpCode[] OpCodeLo = new OpCode[256];
        /// <summary>
        /// MSIL High
        /// </summary>
        private static OpCode[] OpCodeHi = new OpCode[256];

        /// <summary>
        /// Load MSIL byte codes into array =P
        /// </summary>
        public static void Start()
        {
            // Just lookup into IL list
            foreach (var xField in typeof(OpCodes).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
            {
                var xOpCode = (OpCode)xField.GetValue(null);
                var xValue = (ushort)xOpCode.Value;

                // Classify each by high low
                if (xValue <= 0xFF)
                    OpCodeLo[xValue] = xOpCode;
                else
                    OpCodeHi[xValue & 0xFF] = xOpCode;
            }
        }

        /// <summary>
        /// This method just process method and output MSIL List
        /// Here is one more thing i.e. LabelTarget,
        /// Well it is just a list of il position where we have to break and create one more label with give position
        /// This is used when we have to make branch or jump operation to an IL
        /// </summary>
        /// <param name="aMethod"></param>
        /// <param name="LabelTarget"></param>
        /// <returns></returns>
        public static List<ILOpCode> Process(this MethodBase aMethod, List<int> LabelTarget)
        {
            List<ILOpCode> xResult = new List<ILOpCode>();
            var xBody = aMethod.GetMethodBody();
            Type[] xTypeGenArgs = null;
            Type[] xMethodGenArgs = null;

            if (aMethod.DeclaringType.IsGenericType)
                xTypeGenArgs = aMethod.DeclaringType.GetGenericArguments();

            if (aMethod.IsGenericMethod)
                xMethodGenArgs = aMethod.GetGenericArguments();

            if (xBody == null)
                return null;

            // Get IL as a byte Array using microsoft implementation
            var msIL = xBody.GetILAsByteArray();

            /* Exactly below code just simplfy the IL byte array and give us IL class and operand type
             * This is just to make the life easier
             */

            // Set current position of IL as zero
            int xPos = 0;
            while (xPos < msIL.Length)
            {
                /* Calculate Exception handling label for current IL
                 * It just do check if this IL is inside try catch, if yes than set xCurrentHandler else null
                 */
                ExceptionHandlingClause xCurrentHandler = null;
                #region Exception
                foreach (ExceptionHandlingClause xHandler in xBody.ExceptionHandlingClauses)
                {
                    //We can have Try in the beginning so it can be equals to zero :)
                    if (xHandler.TryOffset >= 0)
                    {
                        if (xHandler.TryOffset <= xPos && (xHandler.TryLength + xHandler.TryOffset + 1) > xPos) // + 1 because index should be less than the try
                        {
                            if (xCurrentHandler == null)
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                            else if (xHandler.TryOffset > xCurrentHandler.TryOffset && (xHandler.TryLength + xHandler.TryOffset) < (xCurrentHandler.TryLength + xCurrentHandler.TryOffset))
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                        }
                    }
                    if (xHandler.HandlerOffset > 0)
                    {
                        if (xHandler.HandlerOffset <= xPos && (xHandler.HandlerOffset + xHandler.HandlerLength + 1) > xPos)
                        {
                            if (xCurrentHandler == null)
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                            else if (xHandler.HandlerOffset > xCurrentHandler.HandlerOffset && (xHandler.HandlerOffset + xHandler.HandlerLength) < (xCurrentHandler.HandlerOffset + xCurrentHandler.HandlerLength))
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                        }
                    }
                    if ((xHandler.Flags & ExceptionHandlingClauseOptions.Filter) > 0)
                    {
                        if (xHandler.FilterOffset > 0)
                        {
                            if (xHandler.FilterOffset <= xPos)
                            {
                                if (xCurrentHandler == null)
                                {
                                    xCurrentHandler = xHandler;
                                    continue;
                                }
                                else if (xHandler.FilterOffset > xCurrentHandler.FilterOffset)
                                {
                                    xCurrentHandler = xHandler;
                                    continue;
                                }
                            }
                        }
                    }
                }
                #endregion
                if (xCurrentHandler != null)
                {
                    LabelTarget.Add(xCurrentHandler.HandlerOffset);
                }

                // Well here we have an op code holder and also a operand type holder
                ILCode xOpCodeVal;
                OpCode xOpCode;
                int xOpPos = xPos;
                if (msIL[xPos] == 0xFE)
                {
                    xOpCodeVal = (ILCode)(0xFE00 | msIL[xPos + 1]);
                    xOpCode = OpCodeHi[msIL[xPos + 1]];
                    xPos = xPos + 2;
                }
                else
                {
                    xOpCodeVal = (ILCode)msIL[xPos];
                    xOpCode = OpCodeLo[msIL[xPos]];
                    xPos++;
                }

                // This will be our final OpCode class =)
                // We just set its value and add it to above list
                ILOpCode xILOpCode = null;

                // And below is some magic :P , which give us OpCode class, exactly a hardcoded classification
                // So, i'm not going to take any garrentee if you touch this :P
                // handle with care =P
                // And we also add the labels breakpoints here, so take that in mind before merging IL
                // As we don't want waste labels =)
                switch (xOpCode.OperandType)
                {
                    #region Inline None
                    case OperandType.InlineNone:
                        {
                            switch (xOpCodeVal)
                            {
                                #region Ldarg
                                case ILCode.Ldarg_0:
                                        xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarg, xOpPos, xPos, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_1:
                                        xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarg, xOpPos, xPos, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_2:
                                        xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarg, xOpPos, xPos, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_3:
                                        xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarg, xOpPos, xPos, 3, xCurrentHandler);
                                    break;
                                #endregion
                                #region Ldc_I4
                                case ILCode.Ldc_I4_0:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_1:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_2:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_3:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 3, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_4:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 4, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_5:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 5, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_6:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 6, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_7:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 7, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_8:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, 8, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_M1:
                                    xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos, -1, xCurrentHandler);
                                    break;
                                #endregion
                                #region Ldloc
                                case ILCode.Ldloc_0:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloc, xOpPos, xPos, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_1:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloc, xOpPos, xPos, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_2:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloc, xOpPos, xPos, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_3:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloc, xOpPos, xPos, 3, xCurrentHandler);
                                    break;
                                #endregion
                                #region Stloc
                                case ILCode.Stloc_0:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Stloc, xOpPos, xPos, 0, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_1:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Stloc, xOpPos, xPos, 1, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_2:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Stloc, xOpPos, xPos, 2, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_3:
                                    xILOpCode = new ILOpCodes.OpVar(ILCode.Stloc, xOpPos, xPos, 3, xCurrentHandler);
                                    break;
                                #endregion
                                default:
                                    xILOpCode = new ILOpCodes.OpNone(xOpCodeVal, xOpPos, xPos, xCurrentHandler);
                                    break;
                            }
                        }
                        break;
                    #endregion
                    #region Inline Branch
                    case OperandType.ShortInlineBrTarget:
                        {
                            int xTarget = xPos + 1 + (sbyte)msIL[xPos];
                            LabelTarget.Add(xTarget);
                            switch (xOpCodeVal)
                            {
                                case ILCode.Beq_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Beq, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bge_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Bge, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bge_Un_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Bge_Un, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bgt_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Bgt, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bgt_Un_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Bgt_Un, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Ble_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Ble, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Ble_Un_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Ble_Un, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Blt_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Blt, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Blt_Un_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Blt_Un, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bne_Un_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Bne_Un, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Br_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Br, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Brfalse_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Brfalse, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Brtrue_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Brtrue, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Leave_S:
                                    xILOpCode = new ILOpCodes.OpBranch(ILCode.Leave, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                                default:
                                    xILOpCode = new ILOpCodes.OpBranch(xOpCodeVal, xOpPos, xPos + 1, xTarget, xCurrentHandler);
                                    break;
                            }
                            xPos += 1;
                        }
                        break;
                    #endregion
                    case OperandType.InlineBrTarget:
                        {
                            int xTarget = xPos + 4 + (Int32)BitConverter.ToInt32(msIL, xPos);
                            LabelTarget.Add(xTarget);
                            xILOpCode = new ILOpCodes.OpBranch(xOpCodeVal, xOpPos, xPos + 4, xTarget, xCurrentHandler);
                            xPos = xPos + 4;
                        }
                        break;
                    case OperandType.ShortInlineI:
                        switch (xOpCodeVal)
                        {
                            case ILCode.Ldc_I4_S:
                                xILOpCode = new ILOpCodes.OpInt(ILCode.Ldc_I4, xOpPos, xPos + 1, ((sbyte)msIL[xPos]), xCurrentHandler);
                                break;
                            default:
                                xILOpCode = new ILOpCodes.OpInt(xOpCodeVal, xOpPos, xPos + 1, ((sbyte)msIL[xPos]), xCurrentHandler);
                                break;
                        }
                        xPos = xPos + 1;
                        break;
                    case OperandType.InlineI:
                        xILOpCode = new ILOpCodes.OpInt(xOpCodeVal, xOpPos, xPos + 4, BitConverter.ToInt32(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 4;
                        break;
                    case OperandType.InlineI8:
                        xILOpCode = new ILOpCodes.OpInt64(xOpCodeVal, xOpPos, xPos + 8, BitConverter.ToUInt64(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 8;
                        break;

                    case OperandType.ShortInlineR:
                        xILOpCode = new ILOpCodes.OpSingle(xOpCodeVal, xOpPos, xPos + 4, BitConverter.ToSingle(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 4;
                        break;
                    case OperandType.InlineR:
                        xILOpCode = new ILOpCodes.OpDouble(xOpCodeVal, xOpPos, xPos + 8, BitConverter.ToDouble(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 8;
                        break;

                    case OperandType.InlineField:
                        {
                            var xValue = aMethod.Module.ResolveField((int)BitConverter.ToInt32(msIL, xPos), xTypeGenArgs, xMethodGenArgs);
                            xILOpCode = new ILOpCodes.OpField(xOpCodeVal, xOpPos, xPos + 4, xValue, xCurrentHandler);
                            xPos = xPos + 4;
                            break;
                        }

                    case OperandType.InlineMethod:
                        {
                            var xValue = aMethod.Module.ResolveMethod((int)BitConverter.ToInt32(msIL, xPos), xTypeGenArgs, xMethodGenArgs);
                            xILOpCode = new ILOpCodes.OpMethod(xOpCodeVal, xOpPos, xPos + 4, xValue, xCurrentHandler);
                            xPos = xPos + 4;
                            break;
                        }

                    case OperandType.InlineSig:
                        xILOpCode = new ILOpCodes.OpSig(xOpCodeVal, xOpPos, xPos + 4, BitConverter.ToInt32(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 4;
                        break;

                    case OperandType.InlineString:
                        xILOpCode = new ILOpCodes.OpString(xOpCodeVal, xOpPos, xPos + 4, aMethod.Module.ResolveString((int)BitConverter.ToInt32(msIL, xPos)), xCurrentHandler);
                        xPos = xPos + 4;
                        break;

                    case OperandType.InlineSwitch:
                        {
                            int xCount = (int)BitConverter.ToInt32(msIL, xPos);
                            xPos = xPos + 4;
                            int xNextOpPos = xPos + xCount * 4;
                            int[] xBranchLocations = new int[xCount];
                            for (int i = 0; i < xCount; i++)
                            {
                                xBranchLocations[i] = xNextOpPos + (int)BitConverter.ToInt32(msIL, xPos + i * 4);
                                LabelTarget.Add(xBranchLocations[i]);
                            }
                            xILOpCode = new ILOpCodes.OpSwitch(xOpCodeVal, xOpPos, xNextOpPos, xBranchLocations, xCurrentHandler);
                            xPos = xNextOpPos;
                            break;
                        }
                    case OperandType.InlineTok:
                        xILOpCode = new ILOpCodes.OpToken(xOpCodeVal, xOpPos, xPos + 4, BitConverter.ToInt32(msIL, xPos), aMethod.Module, xTypeGenArgs, xMethodGenArgs, xCurrentHandler);
                        xPos = xPos + 4;
                        break;
                    case OperandType.InlineType:
                        {
                            var xValue = aMethod.Module.ResolveType((int)BitConverter.ToInt32(msIL, xPos), xTypeGenArgs, xMethodGenArgs);
                            xILOpCode = new ILOpCodes.OpType(xOpCodeVal, xOpPos, xPos + 4, xValue, xCurrentHandler);
                            xPos = xPos + 4;
                            break;
                        }
                    #region ShortInlineVar
                    case OperandType.ShortInlineVar:
                        switch (xOpCodeVal)
                        {
                            case ILCode.Ldloc_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloc, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            case ILCode.Ldloca_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Ldloca, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            case ILCode.Ldarg_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarg, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            case ILCode.Ldarga_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Ldarga, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            case ILCode.Starg_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Starg, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            case ILCode.Stloc_S:
                                xILOpCode = new ILOpCodes.OpVar(ILCode.Stloc, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                            default:
                                xILOpCode = new ILOpCodes.OpVar(xOpCodeVal, xOpPos, xPos + 1, msIL[xPos], xCurrentHandler);
                                break;
                        }
                        xPos = xPos + 1;
                        break;
                    #endregion
                    case OperandType.InlineVar:
                        xILOpCode = new ILOpCodes.OpVar(xOpCodeVal, xOpPos, xPos + 2, BitConverter.ToUInt16(msIL, xPos), xCurrentHandler);
                        xPos = xPos + 2;
                        break;
                    default:
                        throw new Exception("Internal Compiler error" + xOpCode.OperandType);
                }
                // Add few more label lists
                switch (xILOpCode.Code)
                {
                    case ILCode.Call:
                    case ILCode.Callvirt:
                    case ILCode.Newobj:
                        LabelTarget.Add(xILOpCode.NextPosition);
                        break;
                    default:
                        break;
                }

                // Add our result of magic code to list
                xResult.Add(xILOpCode);
            }
            // Return the magic code result and be happy =)
            return xResult;
        }
    }
}
